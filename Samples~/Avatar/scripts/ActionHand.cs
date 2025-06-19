using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;



public class ActionHand : MonoBehaviour
{
    public InputActionProperty agarrar;
    public InputActionProperty click;
    public OrientationHand orientationCurrent;
    private Animator animator;
    private Conn conn;

    private int Grab;
    private int OnClick;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void GrabAnimation(bool sendNetwork)
    {
        animator.SetInteger("PULGAR", 1);
        animator.SetInteger("INDICE", 1);
        animator.SetInteger("CORAZON", 1);
        animator.SetInteger("ANULAR", 1);
        animator.SetInteger("MENIQUE", 1);

        if (sendNetwork) StartCoroutine(SendActionHand(ActionHandEmun.Grab));
    }

    public void SelectAnimation(bool sendNetwork)
    {
        animator.SetInteger("PULGAR", 2);
        animator.SetInteger("INDICE", 2);
        animator.SetInteger("CORAZON", 1);
        animator.SetInteger("ANULAR", 1);
        animator.SetInteger("MENIQUE", 1);

        if (sendNetwork) StartCoroutine(SendActionHand(ActionHandEmun.Select));
    }

    public void HandIdleAnimation(bool sendNetwork)
    {
        animator.SetInteger("PULGAR", 0);
        animator.SetInteger("INDICE", 0);
        animator.SetInteger("CORAZON", 0);
        animator.SetInteger("ANULAR", 0);
        animator.SetInteger("MENIQUE", 0);

        if (sendNetwork) StartCoroutine(SendActionHand(ActionHandEmun.Idle));
    }

    void Update()
    {
        int isGrab = (int)agarrar.action.ReadValue<float>();
        if (isGrab != Grab)
        {
            Grab = isGrab;
            if (isGrab > 0)
            {
                GrabAnimation(true);
            }
            else
            {
                HandIdleAnimation(true);
            }
        }

        int isClick = (int)click.action.ReadValue<float>();
        if (isClick != OnClick)
        {
            OnClick = isClick;
            if (isClick > 0)
            {
                SelectAnimation(true);
            }
            else
            {
                HandIdleAnimation(true);
            }
        }
    }

    private IEnumerator SendActionHand(ActionHandEmun states)
    {
        if (Conn.Instance == null) yield return new WaitUntil(() => Conn.Instance != null);

        conn = Conn.Instance;

        var data = new DataActionHand
        {
            orientation = orientationCurrent,
            state = states
        };

        string json = JsonUtility.ToJson(data);
        Task sendTask = conn.SendMessage(json, Events.ActionHandsPlayer);
        yield return new WaitUntil(() => sendTask.IsCompleted);
    }
}
