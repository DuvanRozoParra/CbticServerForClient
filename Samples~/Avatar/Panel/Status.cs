using TMPro;
using UnityEngine;

public class Status : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Conn _conn;
    void Start()
    {
        // text = GetComponent<TextMeshProUGUI>();
        _conn = GetComponent<Conn>();

        GameObject go = GameObject.Find("CbticSocket");
        _conn = go.GetComponent<Conn>();

        InvokeRepeating(nameof(ViewFPS), 1f, 2f);
    }

    public void ViewFPS()
    {
        if (text != null)
        {
            float current = (int)(1f / Time.deltaTime);
            text.text = "FPS: " + current.ToString() + "\nStatus: " + (_conn.IsConnect == true ? "Online" : "Offline");
        }
    }
}
