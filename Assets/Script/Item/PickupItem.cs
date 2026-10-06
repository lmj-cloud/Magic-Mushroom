using UnityEngine;

public class PickupItem : MonoBehaviour
{
    protected PlayerPickup playerPickup;

    public void SetPlayerPickup(PlayerPickup pickup)
    {
        playerPickup = pickup;
    }
    public virtual void UseLeftClick()
    {
    }

    public virtual void UseRightClick()
    {
    }
}
