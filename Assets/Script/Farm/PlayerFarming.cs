using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFarming : MonoBehaviour
{
    public GameObject poza;
    [SerializeField] private Transform pozalocate;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            Instantiate(poza, pozalocate.position, Quaternion.identity);
        }
    }
}
