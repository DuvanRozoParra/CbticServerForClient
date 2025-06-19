using System;
using System.Threading.Tasks;
using NativeWebSocket;
using UnityEngine;


public class Conn : MonoBehaviour
{
    [Header("Server")]
    public string idConnection;
    public readonly string BaseUrl = "ws://192.168.25.86:8080/api/v1/ws";
    [Header("Dependencies")]
    public QueueServer queueServer;
    private WebSocket ws;
    public bool IsConnect = false;

    public static Conn Instance;
    async void Awake()
    {
        try
        {

            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            Guid uuid = Guid.NewGuid();
            idConnection = uuid.ToString();

            ws = new WebSocket($"{BaseUrl}/{idConnection}");

            ws.OnOpen += OnOpen;
            ws.OnError += OnError;
            ws.OnClose += OnClose;
            ws.OnMessage += OnMessage;

            await ws.Connect();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error al conectar: {e.Message}");
        }
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        ws.DispatchMessageQueue();
#endif
    }

    void OnClose(WebSocketCloseCode closeCode)
    {
        Debug.LogWarning($"Conexión cerrada: {closeCode}");
        IsConnect = false;
    }
    void OnOpen()
    {
        Debug.Log("Connect player");
        IsConnect = true;
    }

    void OnError(string error)
    {
        Debug.LogError($"Error WS: {error}");
        IsConnect = false;
    }
    void OnMessage(byte[] bytes)
    {
        try
        {
            string rawMessage = System.Text.Encoding.UTF8.GetString(bytes);
            if (string.IsNullOrEmpty(rawMessage)) throw new Exception("Mensaje vacío recibido");
            var msg = JsonUtility.FromJson<MessageServer>(rawMessage) ?? throw new Exception($"Error deserializando mensaje: {rawMessage}");

            queueServer.AddQueue(msg);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error deserializando mensaje: {e.Message}");
        }
    }


    private async void OnApplicationQuit()
    {
        await CloseWebSocket();
    }

    private async void OnDestroy()
    {
        await CloseWebSocket();
    }

    private async Task CloseWebSocket()
    {
        if (ws != null)
        {
            try
            {
                await ws.Close();
                Debug.Log("Conexión WebSocket cerrada correctamente.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error cerrando WebSocket: {ex.Message}");
            }
        }
    }

    public async Task SendMessage<T>(T message, Events events)
    {

        try
        {
            if (ws?.State != WebSocketState.Open)
            {
                Debug.LogError($"Error de conexion: {ws?.State}");
                return;
            }

            ClientToServer<T> data = new()
            {
                data = message,
                from = idConnection,
                events = events
            };

            if (events != Events.MovePlayer)
            {
                // Debug.Log("Datos a enviar: " + JsonUtility.ToJson(data));
                // Debug.Log("Message: " + JsonUtility.ToJson(message));
            }

            await ws.SendText(JsonUtility.ToJson(data));
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error enviando mensaje: {ex.Message}");
        }

    }
}
