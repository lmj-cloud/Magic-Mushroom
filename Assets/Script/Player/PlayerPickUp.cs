using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickup : MonoBehaviour
{
    public float interactDistance = 3f;
    public float interactRadius = 0.5f;
    public Transform holdPoint;
    public Collider playerCollider;

    public GameObject heldObject;
    //임시로 빠따를 하나 만들 건데 옵젝 들고있는 와중에 때릴 수는 없으니 임시로 보호를 변경할게요


    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (heldObject == null)
                TryPickup();
            else
                Drop();
        }

        if (heldObject != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                UseLeftClick();
            }

            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                UseRightClick();
            }
        }
    }

    void TryPickup()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.SphereCast(ray, interactRadius, out RaycastHit hit, interactDistance))
        {
            if (hit.collider.CompareTag("Pickup"))
            {
                heldObject = hit.collider.gameObject;

                Rigidbody rb = heldObject.GetComponent<Rigidbody>();
                Collider objectCollider = heldObject.GetComponent<Collider>();

                if (objectCollider != null)
                {
                    Physics.IgnoreCollision(playerCollider, objectCollider, true);
                }

                if (rb != null)
                {
                    rb.useGravity = false;
                    rb.isKinematic = true;
                }

                heldObject.transform.SetParent(holdPoint);
                heldObject.transform.localPosition = Vector3.zero;
                heldObject.transform.localRotation = Quaternion.identity;
            }
        }
    }

    void Drop()
    {
        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        Collider objectCollider = heldObject.GetComponent<Collider>();

        heldObject.transform.SetParent(null);

        if (objectCollider != null)
        {
            Physics.IgnoreCollision(playerCollider, objectCollider, false);
        }

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        heldObject = null;
    }

    void UseLeftClick()
    {
        Debug.Log("들고 있는 물건 던지거나 사용");
    }

    void UseRightClick()
    {
        Debug.Log("들고 있는 물건 던지거나 사용");
    }
}