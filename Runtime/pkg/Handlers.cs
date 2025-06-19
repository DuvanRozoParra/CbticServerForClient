using System;
using UnityEngine;

public class Handlers : MonoBehaviour
{
    public MessageServer ConvertToMsgServe(string data)
    {
        var msg = JsonUtility.FromJson<MessageServer>(data);
        if (msg == null)
        {

            throw new Exception($"Error deserializando mensaje: {msg}");
        }
        return msg;
    }
}