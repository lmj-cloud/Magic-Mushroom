using UnityEngine;
using UnityEngine.InputSystem;

public class Farm : MonoBehaviour
{
    //이코드는제정신이아니다완전히다르게바꿔야해
    public int currentLevel = 1;
    public GameObject mushroomPrefab;
    [SerializeField] private float mushroomSpacing = 0.5f;
    [SerializeField] private int randomTryCount = 20;

    Collider farmCollider;

    void Start()
    {
        farmCollider = GetComponent<Collider>();
        //ApplyFarmScale();
    }

    void Update()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            currentLevel++;
            //ApplyFarmScale();
        }
    }
    // 플레이어 앞 부채꼴 안에 걸친 이 밭의 빈 자리에 버섯 하나를 심는다
    public bool TryPlantInSector(Vector3 origin, Vector3 forward, float minDistance, float radius, float halfAngle)
    {
        if (mushroomPrefab == null)
            return false;

        if (farmCollider == null)
            farmCollider = GetComponent<Collider>();
        if (farmCollider == null)
            return false;

        forward.y = 0f;
        if (forward.sqrMagnitude < 0.0001f)
            return false;
        forward.Normalize();

        Vector3 spot;
        if (!FindEmptySpotInSector(origin, forward, minDistance, radius, halfAngle, out spot))
            return false;

        Instantiate(mushroomPrefab, spot, Quaternion.identity);
        return true;
    }
    bool FindEmptySpotInSector(Vector3 origin, Vector3 forward, float minDistance, float radius, float halfAngle, out Vector3 spot)
    {
        Bounds bounds = farmCollider.bounds;
        // 버섯이 밭 밖으로 삐져나오지 않게 가장자리를 간격만큼 잘라낸다
        float minX = bounds.min.x + mushroomSpacing;
        float maxX = bounds.max.x - mushroomSpacing;
        float minZ = bounds.min.z + mushroomSpacing;
        float maxZ = bounds.max.z - mushroomSpacing;
        if (minX > maxX)
            minX = maxX = bounds.center.x;
        if (minZ > maxZ)
            minZ = maxZ = bounds.center.z;
        float surfaceY = bounds.max.y;

        for (int i = 0; i < randomTryCount; i++)
        {
            spot = RandomPointInSector(origin, forward, minDistance, radius, halfAngle);
            spot.y = surfaceY;
            if (spot.x < minX || spot.x > maxX || spot.z < minZ || spot.z > maxZ)
                continue;
            if (IsEmpty(spot))
                return true;
        }

        // 랜덤으로 못 찾았다고 꽉 찬 건 아니니까, 부채꼴에 들어온 칸만 골라 하나를 뽑는다
        spot = Vector3.zero;
        int found = 0;
        for (float x = minX; x <= maxX + 0.001f; x += mushroomSpacing)
        {
            for (float z = minZ; z <= maxZ + 0.001f; z += mushroomSpacing)
            {
                Vector3 candidate = new Vector3(x, surfaceY, z);
                if (!IsInsideSector(candidate, origin, forward, minDistance, radius, halfAngle))
                    continue;
                if (!IsEmpty(candidate))
                    continue;

                found++;
                if (Random.Range(0, found) == 0)
                    spot = candidate;
            }
        }

        return found > 0;
    }
    static Vector3 RandomPointInSector(Vector3 origin, Vector3 forward, float minDistance, float radius, float halfAngle)
    {
        float angle = Random.Range(-halfAngle, halfAngle);
        float dist = Mathf.Sqrt(Mathf.Lerp(minDistance * minDistance, radius * radius, Random.value));
        Vector3 dir = Quaternion.AngleAxis(angle, Vector3.up) * forward;
        return origin + dir * dist;
    }
    static bool IsInsideSector(Vector3 point, Vector3 origin, Vector3 forward, float minDistance, float radius, float halfAngle)
    {
        Vector3 flat = point - origin;
        flat.y = 0f;
        float dist = flat.magnitude;
        if (dist < minDistance || dist > radius)
            return false;
        if (dist < 0.001f)
            return false;

        return Vector3.Angle(forward, flat) <= halfAngle;
    }
    bool IsEmpty(Vector3 spot)
    {
        // 버섯은 자라면서 태그가 바뀌니까 컴포넌트로 본다
        //사실 이러면 안됨. 나중에 가면 버섯이 많아지니까...
        Collider[] hits = Physics.OverlapSphere(spot, mushroomSpacing);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].GetComponentInParent<MushroomGrowing>() != null)
                return false;
        }
        return true;
    }
    void ApplyFarmScale()
    {
        if (currentLevel > 4)
            return;

        float scaleXZ;
        switch (currentLevel)
        {
            case 1:
                scaleXZ = 1f;
                break;
            case 2:
                scaleXZ = 2.5f;
                break;
            case 3:
                scaleXZ = 3.5f;
                break;
            case 4:
                scaleXZ = 6.0f;
                break;
            default:
                return;
        }

        GameObject[] farms = GameObject.FindGameObjectsWithTag("Farm");
        for (int i = 0; i < farms.Length; i++)
        {
            Vector3 scale = farms[i].transform.localScale;
            scale.x = scaleXZ;
            scale.z = scaleXZ;
            farms[i].transform.localScale = scale;
        }
    }
}
