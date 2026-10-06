using System.Collections;
using UnityEngine;

public class MushroomGrowing : MonoBehaviour
{
    Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(MushroomGrow());
    }

    IEnumerator MushroomGrow()
    {
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
        transform.localScale = Vector3.one * 100.0f;
        yield return new WaitForSeconds(10.0f);
        gameObject.tag = "Pickup"; // 코드로 따로 바꾸던가 혹은 버섯을 새로 생성하던가
        transform.localScale = Vector3.one * 200.0f;        
        rb.constraints = RigidbodyConstraints.None;

    }
}
