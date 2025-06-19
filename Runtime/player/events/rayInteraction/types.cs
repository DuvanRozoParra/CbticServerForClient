public enum ActionType
{
    FirstHoverEntered,
    LastHoverExited,
    HoverEntered,
    HoverExited,
    FirstSelectEntered,
    LastSelectExited,
    SelectEntered,
    SelectExited,
    FirstFocusEntered,
    LastFocusExited,
    FocusEntered,
    FocusExited,
    Activated,
    Desactivated
}

[System.Serializable]
public class MessageRayInteraction
{
    public string idRayInteraction;
    public ActionType actionType;
}
