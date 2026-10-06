using UnityEngine;

public class GromeStatus : MonoBehaviour
{
    [SerializeField] private Noum noum;
    public int Hp = 30;

    private bool dead;

    void Awake()
    {
        if (noum == null)
            noum = GetComponentInParent<Noum>();
    }

    public void TakeDamage(int attackDamage)
    {
        if (dead)
            return;

        Hp = Hp - attackDamage;
        Debug.Log("맞았다.");
    }

    void Update()
    {
        if (dead || Hp > 0)
            return;

        dead = true;
        DropHeld();

        // GromeStatus가 자식에 붙어 있어도 노움 본체까지 같이 지워야 한다
        Destroy(noum != null ? noum.gameObject : gameObject);
        Debug.Log("쥬금");
    }

    private void DropHeld()
    {
        if (noum == null || noum.held == null)
            return;

        Transform item = noum.held;
        noum.held = null;

        // 복제가 아니라 자식에서 떼어내야 노움이 지워질 때 같이 안 지워진다
        item.SetParent(null);
        item.position = noum.transform.position + Vector3.up * 0.5f;

        var col = item.GetComponent<Collider>();
        if (col != null)
            col.enabled = true;

        var rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        Debug.Log("템떨구고쥬금");
    }
}
