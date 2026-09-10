using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    public float speed = 5f;
    public float runSpeed = 10f;
    public float jumpPower = 5f;

    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 move = Vector3.zero;

        if (Keyboard.current.wKey.isPressed)
            move += transform.forward;

        if (Keyboard.current.sKey.isPressed)
            move -= transform.forward;

        if (Keyboard.current.aKey.isPressed)
            move -= transform.right;

        if (Keyboard.current.dKey.isPressed)
            move += transform.right;

        move.y = 0f;
        move = move.normalized;

        float currentSpeed = speed;

        if (Keyboard.current.shiftKey.isPressed)
        {
            currentSpeed = runSpeed;
        }

        rb.linearVelocity = new Vector3(
            move.x * currentSpeed,
            rb.linearVelocity.y,
            move.z * currentSpeed
        );

        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }
}