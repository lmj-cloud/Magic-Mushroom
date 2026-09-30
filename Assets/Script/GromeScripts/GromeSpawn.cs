using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject enemy;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private float minX = -61f;
    [SerializeField] private float maxX = 20f;
    [SerializeField] private float minZ = -19f;
    [SerializeField] private float maxZ = 63f;
    [SerializeField] private float spawnY = 0.2f;

    private Coroutine mouseSpawn; //안쓴다고 나와있는데 사실 사용하고 있음. 

    private void Start()
    {
        mouseSpawn = StartCoroutine(MouseSpawn());
    }

    private Vector3 GetPerimeterSpawnPosition()
    {
        float width = maxX - minX;
        float height = maxZ - minZ;
        float perimeter = 2f * (width + height);
        float distance = Random.Range(0f, perimeter);

        if (distance < width)
            return new Vector3(minX + distance, spawnY, minZ);

        distance -= width;
        if (distance < height)
            return new Vector3(maxX, spawnY, minZ + distance);

        distance -= height;
        if (distance < width)
            return new Vector3(maxX - distance, spawnY, maxZ);

        distance -= width;
        return new Vector3(minX, spawnY, maxZ - distance);
    }

    private IEnumerator MouseSpawn()
    {
        while (true)
        {
            Vector3 spawnPos = GetPerimeterSpawnPosition();
            Instantiate(enemy, spawnPos, Quaternion.identity);
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
