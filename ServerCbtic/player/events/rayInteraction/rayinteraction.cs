using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class RayInteraction : XRSimpleInteractable
{
    private Dictionary<ActionType, Action> caseMap;
    private readonly Events eventTarget = Events.RayInteraction;
    public string idRayInteraction;
    private SelectEnterEventArgs selectEnterEvent;
    private SelectExitEventArgs selectExitEvent;
    private SelectEnterEventArgs selectEntering;
    private SelectExitEventArgs selectExiting;
    // HOVERS
    private HoverEnterEventArgs qfirstHoverEntered;
    private HoverExitEventArgs qlastHoverExited;
    private HoverEnterEventArgs qhoverEntered;
    private HoverExitEventArgs qhoverExited;
    // FOCUS
    private FocusEnterEventArgs qfirstFocusEntered;
    private FocusExitEventArgs qlastFocusExited;
    private FocusEnterEventArgs qfocusEntered;
    private FocusExitEventArgs qfocusExited;
    // ACT
    private ActivateEventArgs qactivated;
    private DeactivateEventArgs qdeactivated;
    private Conn _conn;
    void Start()
    {
        _conn = GetComponent<Conn>();


        if (idRayInteraction.Length <= 0)
        {
            Debug.LogError("idRayInteraction no fue asignado. Asegúrate de configurarlo en el inspector.");
            return;
        }

        caseMap = new Dictionary<ActionType, Action>
        {
            /*
            { ActionType.FirstSelectEntered, () => { base.OnSelectEntering(selectEntering); } }, //SelectEntering
            { ActionType.LastSelectExited, () => { base.OnSelectExiting(selectExiting); } }, //SelectExiting
            */
            { ActionType.SelectEntered, () => { base.OnSelectEntered(selectEnterEvent); } }, //SelectEnteredEvent
            { ActionType.SelectExited, () => { base.OnSelectExited(selectExitEvent); } }, //SelectExitEvent

            /*
            { ActionType.FirstHoverEntered, () => { base.OnHoverEntering(qfirstHoverEntered); } }, //firstHoverEntered
            { ActionType.LastHoverExited, () => { base.OnHoverExiting(qlastHoverExited); } }, //lastHoverExited
            { ActionType.HoverEntered, () => { base.OnHoverEntered(qhoverEntered); } }, //hoverEntered
            { ActionType.HoverExited, () => { base.OnHoverExited(qhoverExited); } }, //hoverExited

            { ActionType.FirstFocusEntered, () => { base.OnFocusEntering(qfirstFocusEntered); } }, //firstFocusEntered
            { ActionType.LastFocusExited, () => { base.OnFocusExiting(qlastFocusExited); } }, //lastFocusExited
            { ActionType.FocusEntered, () => { base.OnFocusEntered(qfocusEntered); } }, //focusEntered
            { ActionType.FocusExited, () => { base.OnFocusExited(qfocusExited); } }, //focusExited
            
            { ActionType.Activated, () => { base.OnActivated(qactivated); } }, //activated
            { ActionType.Desactivated, () => { base.OnDeactivated(qdeactivated); } }, //deactivated
            */
        };
    }

    private async Task SendEvent(ActionType action)
    {
        MessageRayInteraction msg = new()
        {
            idRayInteraction = idRayInteraction,
            actionType = action
        };
        string json = JsonUtility.ToJson(msg);
        await _conn.SendMessage(json, eventTarget);
    }

    protected override async void OnSelectEntered(SelectEnterEventArgs args)
    {
        selectEnterEvent = args;
        await SendEvent(ActionType.SelectEntered);
    }
    protected override async void OnSelectExited(SelectExitEventArgs args)
    {
        selectExitEvent = args;
        await SendEvent(ActionType.SelectExited);
    }

    /*
    protected override async void OnSelectEntering(SelectEnterEventArgs args)
    {
        selectEntering = args;
        await ConvertToJson(ActionType.FirstSelectEntered);
    }
    protected override async void OnSelectExiting(SelectExitEventArgs args)
    {
        selectExiting = args;
        await ConvertToJson(ActionType.LastSelectExited);
    }

    
        // HOVER
        protected override async void OnHoverEntering(HoverEnterEventArgs args)
        {
            qfirstHoverEntered = args;
            await ConvertToJson(ActionType.FirstHoverEntered);
        }
        protected override async void OnHoverExiting(HoverExitEventArgs args)
        {
            qlastHoverExited = args;
            await ConvertToJson(ActionType.LastHoverExited);
        }
        protected override async void OnHoverEntered(HoverEnterEventArgs args)
        {
            qhoverEntered = args;
            await ConvertToJson(ActionType.HoverEntered);
        }
        protected override async void OnHoverExited(HoverExitEventArgs args)
        {
            qhoverExited = args;
            await ConvertToJson(ActionType.HoverExited);
        }

        // FOCUS
        protected override async void OnFocusEntering(FocusEnterEventArgs args)
        {
            qfirstFocusEntered = args;
            await ConvertToJson(ActionType.FirstFocusEntered);
        }
        protected override async void OnFocusExiting(FocusExitEventArgs args)
        {
            qlastFocusExited = args;
            await ConvertToJson(ActionType.LastFocusExited);
        }
        protected override async void OnFocusEntered(FocusEnterEventArgs args)
        {
            qfocusEntered = args;
            await ConvertToJson(ActionType.FocusEntered);
        }
        protected override async void OnFocusExited(FocusExitEventArgs args)
        {
            qfocusExited = args;
            await ConvertToJson(ActionType.FocusExited);
        }
        // Activate
        protected override async void OnActivated(ActivateEventArgs args)
        {
            qactivated = args;
            await ConvertToJson(ActionType.Activated);
        }
        protected override async void OnDeactivated(DeactivateEventArgs args)
        {
            qdeactivated = args;
            await ConvertToJson(ActionType.Desactivated);
        }
    */

    public void ExecutedAction(ActionType actionType)
    {
        if (caseMap.TryGetValue(actionType, out Action action))
        {
            action.Invoke();
        }
        else
        {
            Debug.LogWarning($"No se encontró acción para el tipo: {actionType}");
        }
    }
}