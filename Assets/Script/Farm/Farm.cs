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
        ApplyFarmScale();
    }

    void Update()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            currentLevel++;
            ApplyFarmScale();
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Poza"))
            return;

        PlantMushroom();
        Destroy(other.gameObject);
    }
    void PlantMushroom()
    {
        if (mushroomPrefab == null || farmCollider == null)
            return;

        Vector3 spot;
        if (!FindEmptySpot(out spot))
            return; // 빈 땅이 없으면 안 심는다

        Instantiate(mushroomPrefab, spot, Quaternion.identity);
    }
    bool FindEmptySpot(out Vector3 spot)
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
            spot = new Vector3(Random.Range(minX, maxX), surfaceY, Random.Range(minZ, maxZ));
            if (IsEmpty(spot))
                return true;
        }

        // 랜덤으로 못 찾았다고 꽉 찬 건 아니니까 격자로 한 번 훑어본다
        for (float x = minX; x <= maxX + 0.001f; x += mushroomSpacing)
        {
            for (float z = minZ; z <= maxZ + 0.001f; z += mushroomSpacing)
            {
                spot = new Vector3(x, surfaceY, z);
                if (IsEmpty(spot))
                    return true;
            }
        }

        spot = Vector3.zero;
        return false;
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
                scaleXZ = 1.8f;
                break;
            case 3:
                scaleXZ = 2.2f;
                break;
            case 4:
                scaleXZ = 2.6f;
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
