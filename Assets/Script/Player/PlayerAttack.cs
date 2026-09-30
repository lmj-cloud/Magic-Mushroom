using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private PlayerPickup playerpickup;
    private Coroutine attackdelay;

    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius = 2f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private LayerMask enemyLayerMask = ~0;
    
    public bool attacked;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (playerpickup.heldObject != null || attackdelay != null)
            {
                return;
            }
            else
            {
                Attack();
                AttackDelay();
                attacked = true;
            }
        }
    }
    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(1.0f);
        attackdelay = null;
        attacked = false;
    }
    void Attack()
    {
        Vector3 center = attackPoint != null ? attackPoint.position : transform.position;
        Collider[] hitEnemies = Physics.OverlapSphere(center, attackRadius, enemyLayerMask);
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.CompareTag("Grome"))
            {
                enemy.GetComponent<GromeStatus>().TakeDamage(attackDamage);
                Debug.Log("적이 맞았다!!!!!! 아프다");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 center = attackPoint != null ? attackPoint.position : transform.position;
        Gizmos.DrawWireSphere(center, attackRadius);
    }
}
