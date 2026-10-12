using UnityEngine;

public class Poza : PickupItem
{
    [SerializeField] private float scatterRadius = 3f;
    [SerializeField] private float scatterAngle = 80f;
    [SerializeField] private float scatterMinDistance = 0.4f;
    [SerializeField] private int mushroomCount = 1;
    [SerializeField] private float scatterCooldown = 3f;

    float nextScatterTime;

    public override void UseLeftClick()
    {
        if (playerPickup != null)
            playerPickup.Throw();
    }

    public override void UseRightClick()
    {
        TryScatter();
    }

    public void TryScatter()
    {
        if (Time.time < nextScatterTime)
            return;

        Transform owner = playerPickup != null ? playerPickup.transform : transform;
        Vector3 origin = owner.position;
        Vector3 forward = owner.forward;
        forward.y = 0f;
        if (forward.sqrMagnitude < 0.0001f)
            forward = Vector3.forward;
        else
            forward.Normalize();

        Scatter(origin, forward, scatterMinDistance, scatterRadius, scatterAngle, mushroomCount);
        nextScatterTime = Time.time + scatterCooldown;
    }

    public void Scatter(Vector3 origin, Vector3 forward, float minDistance, float radius, float angle, int plantCount)
    {
        forward.y = 0f;
        if (forward.sqrMagnitude < 0.0001f)
            return;
        forward.Normalize();

        float halfAngle = Mathf.Clamp(angle * 0.5f, 0f, 180f);
        radius = Mathf.Max(0f, radius);
        minDistance = Mathf.Clamp(minDistance, 0f, radius);

        Collider[] hits = Physics.OverlapSphere(origin, radius, Physics.AllLayers, QueryTriggerInteraction.Collide);
        Farm[] farms = new Farm[hits.Length];
        int farmCount = 0;
        for (int i = 0; i < hits.Length; i++)
        {
            Farm farm = hits[i].GetComponent<Farm>();
            if (farm == null)
                continue;

            bool already = false;
            for (int j = 0; j < farmCount; j++)
            {
                if (farms[j] == farm)
                {
                    already = true;
                    break;
                }
            }
            if (!already)
                farms[farmCount++] = farm;
        }

        int left = Mathf.Max(1, plantCount);
        while (left > 0)
        {
            bool planted = false;
            for (int i = 0; i < farmCount; i++)
            {
                int swap = Random.Range(i, farmCount);
                Farm picked = farms[i];
                farms[i] = farms[swap];
                farms[swap] = picked;

                if (farms[i].TryPlantInSector(origin, forward, minDistance, radius, halfAngle))
                {
                    planted = true;
                    break;
                }
            }

            if (!planted)
                break;
            left--;
        }
    }

    void OnDrawGizmosSelected()
    {
        Vector3 forward = transform.forward;
        forward.y = 0f;
        if (forward.sqrMagnitude < 0.0001f)
            return;
        forward.Normalize();

        Vector3 origin = transform.position;
        float halfAngle = scatterAngle * 0.5f;
        Vector3 left = Quaternion.AngleAxis(-halfAngle, Vector3.up) * forward;
        Vector3 right = Quaternion.AngleAxis(halfAngle, Vector3.up) * forward;

        Gizmos.color = new Color(0.55f, 0.9f, 0.35f, 0.9f);
        Gizmos.DrawLine(origin + left * scatterMinDistance, origin + left * scatterRadius);
        Gizmos.DrawLine(origin + right * scatterMinDistance, origin + right * scatterRadius);

        const int steps = 16;
        Vector3 previousOuter = origin + left * scatterRadius;
        Vector3 previousInner = origin + left * scatterMinDistance;
        for (int i = 1; i <= steps; i++)
        {
            float t = i / (float)steps;
            float angle = Mathf.Lerp(-halfAngle, halfAngle, t);
            Vector3 dir = Quaternion.AngleAxis(angle, Vector3.up) * forward;
            Vector3 outer = origin + dir * scatterRadius;
            Vector3 inner = origin + dir * scatterMinDistance;
            Gizmos.DrawLine(previousOuter, outer);
            Gizmos.DrawLine(previousInner, inner);
            previousOuter = outer;
            previousInner = inner;
        }
    }
}
