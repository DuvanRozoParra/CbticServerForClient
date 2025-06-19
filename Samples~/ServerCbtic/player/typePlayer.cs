using System;
using UnityEngine;

[Serializable]
public class FormantPlayer
{
    public Vector3 position;
    public Quaternion rotation;
}

[Serializable]
public class PlayerGeneral
{
    public string id;
    public FormantPlayer body;
    public FormantPlayer head;
    public FormantPlayer handLeft;
    public FormantPlayer handRight;
}

[Serializable]
public class PlayersMultiplayers
{
    public PlayerGeneral[] players;
}

[Serializable]
public class PlayerView
{
    public string id;
    public GameObject head;
    public GameObject body;
    public GameObject handLeft;
    public GameObject handRight;
    public GameObject Antenna;
    public PlayerGeneral targetData;
}

[Serializable]
public class PlayersAll
{
    public PlayerGeneral[] data;
    public string from;
    public Events events;
}

[Serializable]
public class Wrapper<T>
{
    public T data;
}

[Serializable]
public class AddPlayerMsg
{
    public string id;
    public string color;
}