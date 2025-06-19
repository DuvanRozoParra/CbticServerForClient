using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public enum RolPlayer
{
    Admin,
    Player,
}

public class Player : MonoBehaviour
{
    [Header("Settings Players")]
    public RolPlayer rol;
    private readonly string leftHandTag = "XR Origin (XR Rig)/Camera Offset/Left Controller";
    private readonly string rightHandTag = "XR Origin (XR Rig)/Camera Offset/Right Controller";
    private readonly string mainCameraTag = "Main Camera";
    // private readonly Events _events = Events.MovePlayer;
    private GameObject headPlayer;
    private GameObject leftHand;
    private GameObject rightHand;
    private Conn _conn;
    public Boolean isCrash;
    public static Player Instance { get; private set; }

    void Awake()
    {
        // Si ya existe una instancia y no somos nosotros, la destruimos
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Asignamos la instancia
        Instance = this;

        // Opcional: que no se destruya al cambiar de escena
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        _conn = GetComponent<Conn>();

        headPlayer = GameObject.Find(mainCameraTag);
        leftHand = GameObject.Find(leftHandTag);
        rightHand = GameObject.Find(rightHandTag);

        if (headPlayer == null)
            throw new Exception("No se encontró ninguna cámara con la etiqueta 'MainCamera'.");
        if (leftHand == null)
            throw new Exception($"No se encontró ningun objecto {leftHandTag}.");
        if (rightHand == null)
            throw new Exception($"No se encontró ningun objecto {rightHandTag}.");

        StartCoroutine(SendDataRoutine());
    }

    public FormantPlayer GetHead()
    {
        FormantPlayer data = new()
        {
            position = headPlayer.transform.position,
            rotation = headPlayer.transform.rotation,
        };
        return data;
    }
    private FormantPlayer GetBody()
    {
        headPlayer.transform.GetPositionAndRotation(out Vector3 headPosition, out Quaternion headRotation);
        headRotation = Quaternion.Euler(0, headRotation.eulerAngles.y, 0);
        FormantPlayer data = new()
        {
            position = headPosition,
            rotation = headRotation,
        };
        return data;
    }
    private FormantPlayer GetHandLeft()
    {
        FormantPlayer data = new()
        {
            position = leftHand.transform.position,
            rotation = leftHand.transform.rotation,
        };
        return data;
    }
    private FormantPlayer GetHandRight()
    {
        FormantPlayer data = new()
        {
            position = rightHand.transform.position,
            rotation = rightHand.transform.rotation,
        };
        return data;
    }

    private PlayerGeneral GetCurrentDataPlayer(string id)
    {
        FormantPlayer head = GetHead();
        FormantPlayer body = GetBody();
        FormantPlayer handLeft = GetHandLeft();
        FormantPlayer handRight = GetHandRight();

        PlayerGeneral player = new()
        {
            id = id,
            head = head,
            body = body,
            handLeft = handLeft,
            handRight = handRight
        };

        return player;
    }


    public Transform GetPositionHead()
    {
        return headPlayer.transform;
    }
    private IEnumerator SendDataRoutine()
    {
        while (true)
        {
            PlayerGeneral player = GetCurrentDataPlayer(_conn.idConnection);
            Task sendTask = SendData(player);
            yield return new WaitUntil(() => sendTask.IsCompleted);
            yield return new WaitForSeconds(0.017f); // 0.017f
        }
    }

    private async Task SendData(PlayerGeneral data)
    {

        try
        {
            string json = JsonUtility.ToJson(data);
            await _conn.SendMessage(json, Events.MovePlayer);
        }
        catch
        {
            Debug.LogError("No se pudo enviar datos: La conexión no está abierta");
        }
    }
}