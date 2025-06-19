using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WarningDistance : MonoBehaviour
{
    public TextMeshProUGUI text;
    public RawImage img;
    private bool lastState = false; // Guarda el último estado de isCollider

    void Start()
    {
        text.enabled = false;
        img.enabled = false;
    }

    void Update()
    {
        if (Player.Instance == null) return;
        bool currentState = Player.Instance.isCrash;

        if (currentState != lastState)
        {
            text.enabled = currentState;
            img.enabled = currentState;
            lastState = currentState;
        }
    }
}
