using System;
using System.Collections.Generic;
using UnityEngine;

public class Players : MonoBehaviour
{
    [Header("Player Prefabs")]
    public Vector3 LeftHandScale = new(0.5f, 0.5f, 0.5f);
    public GameObject LeftHand;
    public Vector3 RightHandScale = new(0.5f, 0.5f, 0.5f);
    public GameObject RightHand;
    public Vector3 HeadHandScale = new(0.08f, 0.08f, 0.08f);
    public GameObject Head;
    public Vector3 BodyHandScale = new(0.08f, 0.08f, 0.08f);
    public GameObject Body;
    [Header("Indentify Players Prefabs")]
    public GameObject Antenna;
    public Material AntennaMaterial;
    [Header("Smoothing Parameters")]
    public float positionSmoothTime = 0.05f;
    public float rotationSmoothTime = 0.05f;

    private Dictionary<string, PlayerView> players;

    private Player py;

    void Start()
    {
        players = new();
        GameObject cbticSocket = GameObject.Find("CbticSocket");
        py = cbticSocket.GetComponent<Player>();

        if (LeftHand == null || RightHand == null || Head == null || Body == null)
        {
            throw new Exception("Uno o más prefabs de jugador no están asignados en el Inspector.");
        }
    }

    void Update()
    {
        if (players.Count > 0)
        {
            MovePlayers();
            ProximitToPlayer();
        }
    }

    public void IdentifyToPlayer(string id, string color)
    {
        GameObject AntennaInstance = Instantiate(Antenna, Vector3.zero, Quaternion.identity);
        Material antennaMaterialInstance = new(AntennaMaterial);

        if (ColorUtility.TryParseHtmlString(color, out Color colorHex))
            antennaMaterialInstance.color = colorHex;
        else throw new Exception($"Color HEX inválido: {colorHex}");

        if (AntennaInstance.TryGetComponent<Renderer>(out var antenaRenderer))
            antenaRenderer.material = antennaMaterialInstance;

        PlayerView player = FindPlayer(id);
        AntennaInstance.transform.SetParent(player.head.transform);
        AntennaInstance.transform.localScale = new Vector3(85f, 85f, 85f);
        AntennaInstance.transform.position = new Vector3(0f, 0.1f, 0.02f);
        AntennaInstance.transform.rotation = Quaternion.Euler(-90f, 0.5f, 0f);

        AntennaInstance.name = $"Player_{player.id}_Antenna";
    }

    public void AddPlayer(AddPlayerMsg[] playersAdd, string idConn)
    {
        foreach (var player in playersAdd)
        {
            if (player.id == idConn) continue;
            GameObject headInstance = Instantiate(Head, Vector3.zero, Quaternion.identity);
            GameObject bodyInstance = Instantiate(Body, Vector3.zero, Quaternion.identity);

            GameObject leftHandParent = new($"player_{player.id}_leftHandParent");
            GameObject rightHandParent = new($"player_{player.id}_rightHandParent");

            GameObject leftHandInstance = Instantiate(LeftHand, leftHandParent.transform);
            GameObject rightHandInstance = Instantiate(RightHand, rightHandParent.transform);

            PlayerView playerView = new()
            {
                id = player.id,
                head = headInstance,
                body = bodyInstance,
                handLeft = leftHandParent,
                handRight = rightHandParent,
                Antenna = null
            };

            headInstance.name = $"player_{player.id}_head";
            bodyInstance.name = $"player_{player.id}_body";
            leftHandInstance.name = $"player_{player.id}_leftHand";
            rightHandInstance.name = $"player_{player.id}_rightHand";

            headInstance.transform.localScale = HeadHandScale;
            bodyInstance.transform.localScale = BodyHandScale;
            leftHandParent.transform.localScale = LeftHandScale;
            rightHandParent.transform.localScale = RightHandScale;

            players.Add(player.id, playerView);
            IdentifyToPlayer(player.id, player.color);

            Debug.Log($"PLAYER CREATE SUCCESS... {player.id}");
        }

    }

    public void RemovePlayer(string id)
    {
        PlayerView player = FindPlayer(id);
        if (player == null) return;

        if (player.handLeft != null)
            Destroy(player.handLeft);
        if (player.handRight != null)
            Destroy(player.handRight);
        if (player.head != null)
            Destroy(player.head);
        if (player.body != null)
            Destroy(player.body);

        players.Remove(id);
        Debug.Log($"Jugador Eliminado: ${id}");
    }

    public void UpdatePlayers(PlayerGeneral[] playersCurrent)
    {
        foreach (var player in playersCurrent)
        {
            // Debug.Log($"PLAYER UPDATE SUCCESS... {player.id}");
            PlayerView currentPlayer = FindPlayer(player.id);
            if (currentPlayer == null) continue;
            currentPlayer.targetData = player;
        }

    }

    private void MovePlayers()
    {
        foreach (PlayerView player in players.Values)
        {
            if (player == null) continue;
            MoveObject(player.head, player.targetData.head.position, player.targetData.head.rotation);
            MoveObject(player.body, player.targetData.body.position, player.targetData.body.rotation);
            MoveObject(player.handLeft, player.targetData.handLeft.position, player.targetData.handLeft.rotation);
            MoveObject(player.handRight, player.targetData.handRight.position, player.targetData.handRight.rotation);
        }
    }

    private PlayerView FindPlayer(string id)
    {
        // Debug.Log($"AVER QUE MIERDA... {id}");
        if (players.TryGetValue(id, out PlayerView player))
        {
            return player;
        }
        Debug.LogWarning($"Jugador con ID {id} no encontrado.");
        return null;
    }

    private GameObject FindChildByName(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child.gameObject;

            GameObject result = FindChildByName(child, name);
            if (result != null) return result;
        }
        return null;
    }

    public void ProximitToPlayer()
    {
        FormantPlayer aver = py.GetHead();
        foreach (PlayerView player in players.Values)
        {
            if (player == null) continue;
            float distance = Vector3.Distance(
                player.head.transform.position,
                aver.position
            );
            Debug.Log("distance => " + distance);
            if (distance <= 1.0f && py.isCrash == false) py.isCrash = true;
            else if (distance > 1.0f && py.isCrash == true) py.isCrash = false;
        }
    }
    private void MoveObject(GameObject objectTarget, Vector3 position, Quaternion rotation)
    {
        objectTarget.transform.SetPositionAndRotation(
            Vector3.Lerp(objectTarget.transform.position, position, Time.deltaTime / positionSmoothTime),
            Quaternion.Slerp(objectTarget.transform.rotation, rotation, Time.deltaTime / rotationSmoothTime)
        );
    }

    public void ActionHand(string id, DataActionHand data)
    {
        PlayerView player = FindPlayer(id);
        if (player == null) return;

        GameObject handObject = (data.orientation == OrientationHand.Left)
            ? FindChildByName(player.handLeft.transform, "mano izquierda v2")
            : FindChildByName(player.handRight.transform, "mano derecha v2");

        if (handObject.TryGetComponent<HandsAnimation>(out var handScript))
        {
            switch (data.state)
            {
                case ActionHandEmun.Grab:
                    handScript.GrabAnimation();
                    break;
                case ActionHandEmun.Select:
                    handScript.SelectAnimation();
                    break;
                case ActionHandEmun.Idle:
                    handScript.HandIdleAnimation();
                    break;
                default:
                    Debug.LogWarning($"Estado de mano no reconocido: {data.state}");
                    break;
            }
        }
        else
        {
            Debug.LogError($"No se encontró el componente ActionHand en la mano {data.orientation} del jugador {id}");
            return;
        }

    }
}