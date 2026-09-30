using UnityEngine;

public class GromeStatus : MonoBehaviour
{
    public int Hp = 30;
    [SerializeField] private Renderer cachedRenderer;

    public void TakeDamage(int attackDamage)
    {

        Hp = Hp - attackDamage;
        Debug.Log("맞았다.");
    }
    void Update()
    {
        if (Hp <= 0)
        {
            Destroy(gameObject);
            Debug.Log("쥬금");
        }
    }
}
