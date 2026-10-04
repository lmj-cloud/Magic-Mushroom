using UnityEngine;

public class WindMushroom : PickupItem
{
    public override void UseLeftClick()
    {
        Debug.Log("바람 버섯 던지기!");
    }

    public override void UseRightClick()
    {
        Debug.Log("바람 버섯 내려놓기");
    }
}