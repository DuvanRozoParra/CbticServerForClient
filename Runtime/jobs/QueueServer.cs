using System;
using System.Collections.Generic;
using UnityEngine;

public class QueueServer : MonoBehaviour
{
    private readonly Queue<MessageServer> queue = new();
    private readonly object queueLock = new();
    private readonly Dictionary<Events, Action<MessageServer>> eventHandlers = new();


    [Header("Configuración")]
    public int maxPerFrame = 30;
    public int maxQueueSize = 30;

    [Header("Handlers")]
    public Players players;
    public RayInteractionManage rayInteractionManage;

    private Conn _conn;

    private void Start()
    {
        _conn = GetComponent<Conn>();
        InitializeHandlers();
    }

    private void InitializeHandlers()
    {
        eventHandlers.Add(Events.MovePlayer, HandleMove);
        eventHandlers.Add(Events.RayInteraction, HandleRay);
        eventHandlers.Add(Events.AddPlayer, HandleAddPlayer);
        eventHandlers.Add(Events.IdentifyPlayer, HandleIdentify);
        eventHandlers.Add(Events.RemovePlayer, HandleRemovePlayer);
        eventHandlers.Add(Events.ActionHandsPlayer, HandleActionHands);
        //... Agregar más handlers
    }

    public void AddQueue(MessageServer msg)
    {
        lock (queueLock)
        {
            if (queue.Count < maxQueueSize)
            {
                queue.Enqueue(msg);
            }
            else
            {
                Debug.LogWarning("Cola llena - Mensaje descartado");
            }
        }
    }

    void Update()
    {
        int processed = 0;
        while (queue.Count > 0)
        {
            MessageServer msg;
            lock (queueLock)
            {
                msg = queue.Dequeue();
            }
            ProcessMsg(msg);
            processed++;
        }
    }

    private void ProcessMsg(MessageServer message)
    {
        if (eventHandlers.TryGetValue(message.events, out var handler))
        {
            handler(message);
        }
        else
        {
            Debug.LogError($"Evento no manejado: {message.events}");
        }
    }

    private void HandleRay(MessageServer msg)
    {
        ClientToServer<MessageRayInteraction> data = HelperHandlers<MessageRayInteraction>(msg);
        rayInteractionManage.TriggerEvent(data.data);
    }

    private void HandleMove(MessageServer msg)
    {
        string originalJsonArray = msg.data;
        string wrappedJson = "{\"data\":" + originalJsonArray + "}";
        // Debug.Log($"HandleMove... {originalJsonArray}");
        Wrapper<PlayerGeneral[]> result = JsonUtility.FromJson<Wrapper<PlayerGeneral[]>>(wrappedJson);
        players.UpdatePlayers(result.data);
    }

    private void HandleAddPlayer(MessageServer msg)
    {
        string originalJsonArray = msg.data;
        string wrappedJson = "{\"data\":" + originalJsonArray + "}";
        Wrapper<AddPlayerMsg[]> result = JsonUtility.FromJson<Wrapper<AddPlayerMsg[]>>(wrappedJson);

        players.AddPlayer(result.data, _conn.idConnection);
    }

    private void HandleActionHands(MessageServer msg)
    {
        DataActionHand actionHand = JsonUtility.FromJson<DataActionHand>(msg.data);
        players.ActionHand(msg.from, actionHand);
    }

    private void HandleIdentify(MessageServer msg)
    {
        Debug.Log("recibira la identificacion del siguiente jugador " + msg.from);
        // players.IdentifyToPlayer(msg.from, msg.data);
    }


    private void HandleRemovePlayer(MessageServer msg)
    {
        players.RemovePlayer(msg.from);
    }
    private ClientToServer<T> HelperHandlers<T>(MessageServer msg)
    {
        ConvertData json = JsonUtility.FromJson<ConvertData>(msg.data);
        T handle = JsonUtility.FromJson<T>(json.data);

        return new ClientToServer<T>
        {
            from = json.from,
            events = json.events,
            data = handle
        };
    }
}