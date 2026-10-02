using UnityEngine;
using UnityEngine.InputSystem;

public class Farm : MonoBehaviour
{
    //이코드는제정신이아니다완전히다르게바꿔야해
    public int currentLevel = 1;
    public GameObject mushroomPrefab;

    void Start()
    {
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
        PozaScan(other);
    }
    void PozaScan(Collider poza)
    {
        if (mushroomPrefab == null || !poza.CompareTag("Poza"))
            return;
        if (IsMushroomOverlapping(poza))
            return;
        // 트리거는 접촉점을 안 주니까 포자 콜라이더에서 밭에 가장 가까운 지점을 쓴다
        Vector3 contactPoint = poza.ClosestPoint(transform.position);
        Instantiate(mushroomPrefab, contactPoint, Quaternion.identity);
    }
    bool IsMushroomOverlapping(Collider poza)
    {
        Bounds bounds = poza.bounds;
        Collider[] hits = Physics.OverlapBox(bounds.center, bounds.extents, Quaternion.identity);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].CompareTag("Mushroom"))
                return true;
        }
        return false;
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
                scaleXZ = 1.5f;
                break;
            case 3:
                scaleXZ = 2f;
                break;
            case 4:
                scaleXZ = 2.5f;
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
