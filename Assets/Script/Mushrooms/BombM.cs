using UnityEngine;

public class BombMushroom : PickupItem
{
    public override void UseLeftClick()
    {
        playerPickup.Throw();
    }

    public override void UseRightClick()
    {
        Debug.Log("폭탄 버섯 내려놓기");
    }
}