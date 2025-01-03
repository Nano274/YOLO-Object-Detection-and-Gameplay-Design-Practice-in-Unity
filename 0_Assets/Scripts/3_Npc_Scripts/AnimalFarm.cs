using UnityEngine;
using System.Collections;

public class AnimalFarm : MonoBehaviour
{
    public Object[] animalPrefabList;    // ����Ԥ�����б�
    public Transform[] spawnPoints;     // ���ɵ�

    public int spawnCount = 3;          // ÿ���ص����ɵĶ�������
    public float spawnRadius = 5f;      // ���ɵ������뾶
    public float raycastHeight = 50f;   // ���߳�ʼ�߶�
    public LayerMask groundLayer;       // ���β㼶�����ڼ�����

    private void Start()
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            for (int i = 0; i < spawnCount; i++)
            {
                // �����ɵ㸽���������λ��
                Vector3 randomOffset = new Vector3(
                    Random.Range(-spawnRadius, spawnRadius),
                    0,
                    Random.Range(-spawnRadius, spawnRadius)
                );
                Vector3 spawnPosition = spawnPoint.position + randomOffset;

                // �������߼����θ߶�
                if (Physics.Raycast(spawnPosition + Vector3.up * raycastHeight, Vector3.down, out RaycastHit hit, raycastHeight * 2, groundLayer))
                {
                    spawnPosition = hit.point; // ��������λ�õ�����
                    Debug.Log($"Raycast hit at: {hit.point}");
                }
                else
                {
                    Debug.LogWarning($"Raycast failed for position: {spawnPosition}");
                }

                // ���ѡ��һ������Ԥ����
                int randomIndex = Random.Range(0, animalPrefabList.Length);
                Object prefab = animalPrefabList[randomIndex];

                if (prefab != null)
                {
                    GameObject agent = Instantiate(prefab, spawnPosition, Quaternion.identity) as GameObject;

                    // ��̬���� Rigidbody �Ķ���Լ��
                    Rigidbody rb = agent.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.constraints = RigidbodyConstraints.None; // ����Լ��
                    }

                    // �Ժ�����Ӧ�ö���Լ������ѡ��
                    StartCoroutine(ReapplyConstraints(rb));
                }
            }
        }
    }

    // Э�̣������ɺ��ʱ�������¶���Լ��
    private IEnumerator ReapplyConstraints(Rigidbody rb)
    {
        yield return new WaitForSeconds(0.1f); // �ȴ� 0.1 ��
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            ; // ���¶���λ�ú���ת
            Debug.Log("Constraints reapplied.");
        }
    }
}
