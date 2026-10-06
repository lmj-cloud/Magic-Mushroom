using UnityEngine;

public class Poza : MonoBehaviour
{
    // 밭에 심는 건 Farm이 처리하고, 엉뚱한 데 닿은 포자는 그냥 사라진다
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Farm"))
            return;

        Destroy(gameObject);
    }
}
