public enum OrientationHand
{
    Left,
    Right,
}

public enum ActionHandEmun
{
    Grab,
    Select,
    Idle
}

public class DataActionHand
{
    public OrientationHand orientation;
    public ActionHandEmun state;

}