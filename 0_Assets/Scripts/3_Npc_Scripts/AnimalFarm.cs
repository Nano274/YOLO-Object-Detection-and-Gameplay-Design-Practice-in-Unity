using UnityEngine;
using System.Collections;

public class AnimalFarm : MonoBehaviour
{
    public Object[] animalPrefabList;    // 动物预制体列表
    public Transform[] spawnPoints;     // 生成点

    public int spawnCount = 3;          // 每个地点生成的动物数量
    public float spawnRadius = 5f;      // 生成点的随机半径
    public float raycastHeight = 50f;   // 射线初始高度
    public LayerMask groundLayer;       // 地形层级，用于检测地形

    private void Start()
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            for (int i = 0; i < spawnCount; i++)
            {
                // 在生成点附近随机生成位置
                Vector3 randomOffset = new Vector3(
                    Random.Range(-spawnRadius, spawnRadius),
                    0,
                    Random.Range(-spawnRadius, spawnRadius)
                );
                Vector3 spawnPosition = spawnPoint.position + randomOffset;

                // 向下射线检测地形高度
                if (Physics.Raycast(spawnPosition + Vector3.up * raycastHeight, Vector3.down, out RaycastHit hit, raycastHeight * 2, groundLayer))
                {
                    spawnPosition = hit.point; // 调整生成位置到地面
                    Debug.Log($"Raycast hit at: {hit.point}");
                }
                else
                {
                    Debug.LogWarning($"Raycast failed for position: {spawnPosition}");
                }

                // 随机选择一个动物预制体
                int randomIndex = Random.Range(0, animalPrefabList.Length);
                Object prefab = animalPrefabList[randomIndex];

                if (prefab != null)
                {
                    GameObject agent = Instantiate(prefab, spawnPosition, Quaternion.identity) as GameObject;

                    // 动态解锁 Rigidbody 的冻结约束
                    Rigidbody rb = agent.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.constraints = RigidbodyConstraints.None; // 解锁约束
                    }

                    // 稍后重新应用冻结约束（可选）
                    StartCoroutine(ReapplyConstraints(rb));
                }
            }
        }
    }

    // 协程：在生成后短时间内重新冻结约束
    private IEnumerator ReapplyConstraints(Rigidbody rb)
    {
        yield return new WaitForSeconds(0.1f); // 等待 0.1 秒
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            ; // 重新冻结位置和旋转
            Debug.Log("Constraints reapplied.");
        }
    }
}
