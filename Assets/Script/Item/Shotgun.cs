using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shotgun : PickupItem
{
    [SerializeField] private Transform muzzle;
    [SerializeField] private float range = 8f;
    [Range(0f, 360f)]
    [SerializeField] private float spreadAngle = 60f;
    [SerializeField] private int damage = 9999;
    [SerializeField] private bool horizontalOnly = true;
    [SerializeField] private LayerMask enemyLayerMask = ~0;
    [SerializeField] private LayerMask obstacleLayerMask = 0;

    private readonly HashSet<GromeStatus> hitThisShot = new HashSet<GromeStatus>();
    private int lastFiredFrame = -1;

    /*void Update()
    {
        if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame)
            return;

        // 손에 붙어 있을 때만 쏜다. 땅에 떨어진 총은 부모가 없으므로 격발되지 않는다
        if (transform.parent == null)
            return;

        Fire();
    }*/

    public override void UseLeftClick()
    {
        if (playerPickup != null)
         playerPickup.Throw();
         //던질 수 있어야 할 것 같아서 넣었음. 다른 스크립트랑 호환이 안 되서 기능을 총에 넣어둠.
    }

    // PlayerPickup으로 주워서 든 경우엔 이쪽으로도 들어온다
    public override void UseRightClick()
    {
        Fire();
    }

    void Fire()
    {
        // 두 입력 경로가 같은 프레임에 겹쳐도 한 발만 나가게
        if (lastFiredFrame == Time.frameCount)
            return;

        lastFiredFrame = Time.frameCount;

        Transform origin = muzzle != null ? muzzle : transform;
        Vector3 center = origin.position;
        Vector3 aim = GetAimDirection(origin, out bool flatten);

        float cosHalf = Mathf.Cos(spreadAngle * 0.5f * Mathf.Deg2Rad);

        hitThisShot.Clear();
        Collider[] candidates = Physics.OverlapSphere(center, range, enemyLayerMask, QueryTriggerInteraction.Ignore);

        foreach (Collider candidate in candidates)
        {
            if (!candidate.CompareTag("Grome"))
                continue;

            Vector3 target = candidate.bounds.center;
            Vector3 toTarget = target - center;
            if (flatten)
                toTarget.y = 0f;

            if (toTarget.sqrMagnitude < 0.0001f)
                continue;

            // 각도 비교는 Dot이 Vector3.Angle보다 싸다. cosHalf보다 작으면 부채꼴 밖
            if (Vector3.Dot(aim, toTarget.normalized) < cosHalf)
                continue;

            if (obstacleLayerMask.value != 0 &&
                Physics.Linecast(center, target, obstacleLayerMask, QueryTriggerInteraction.Ignore))
                continue;

            GromeStatus status = candidate.GetComponentInChildren<GromeStatus>();
            if (status == null)
                status = candidate.GetComponentInParent<GromeStatus>();

            // 콜라이더가 여러 개 달린 노움이 한 발에 두 번 맞지 않게
            if (status == null || !hitThisShot.Add(status))
                continue;

            status.TakeDamage(damage);
        }

        Debug.Log("빵! " + hitThisShot.Count + "마리 발할라 직행");
    }

    private Vector3 GetAimDirection(Transform origin, out bool flatten)
    {
        Vector3 aim = origin.forward;
        Vector3 flatAim = new Vector3(aim.x, 0f, aim.z);

        // 총구를 하늘이나 바닥으로 완전히 세우면 수평 성분이 사라져서 방향을 못 구한다
        flatten = horizontalOnly && flatAim.sqrMagnitude > 0.0001f;

        return flatten ? flatAim.normalized : aim.normalized;
    }

    private void OnDrawGizmosSelected()
    {
        Transform origin = muzzle != null ? muzzle : transform;
        Vector3 aim = GetAimDirection(origin, out bool flatten);
        Vector3 axis = flatten ? Vector3.up : origin.up;

        Gizmos.color = Color.red;

        const int segments = 12;
        float half = spreadAngle * 0.5f;

        for (int i = 0; i <= segments; i++)
        {
            float angle = Mathf.Lerp(-half, half, i / (float)segments);
            Gizmos.DrawRay(origin.position, Quaternion.AngleAxis(angle, axis) * aim * range);
        }
    }
}
