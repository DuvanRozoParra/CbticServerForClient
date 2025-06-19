using System;

[Serializable]
public enum Events
{
    // client
    RayInteraction,
    MovePlayer,
    ActionHandsPlayer,

    // server
    AddPlayer,
    UpdatePlayer,
    RemovePlayer,
    IdentifyPlayer,
}

[Serializable]
public class MessageServer
{
    public string data;
    public string from;
    public Events events;
}


[Serializable]
public class ClientToServer<T>
{
    public T data;
    public string from;
    public Events events;
}

[Serializable]
public class ConvertData
{
    public string data;
    public string from;
    public Events events;
}