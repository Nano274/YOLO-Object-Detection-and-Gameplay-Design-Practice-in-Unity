using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class RandomMovement : MonoBehaviour
{
    public float wanderRadius = 10f; // ����ƶ��ķ�Χ
    public float waitTime = 2f;      // ͣ��ʱ��

    private NavMeshAgent agent;      // NavMeshAgent ���
    private Animator animator;       // ����������

    private void Start()
    {
        // ��ȡ NavMeshAgent �� Animator ���
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        StartCoroutine(Wander());
    }

    private IEnumerator Wander()
    {
        while (true)
        {
            // ͣ�²��������¶���
            animator.SetBool("isWalk", false);
            animator.SetBool("isSit", true);

            yield return new WaitForSeconds(waitTime); // �ȴ�ָ��ʱ��

            // �������Ŀ���
            Vector3 randomPoint = GetRandomPoint(transform.position, wanderRadius);

            // ����Ŀ��㣬��������״̬
            if (agent.SetDestination(randomPoint))
            {
                // �ȴ�·���������
                while (agent.pathPending)
                {
                    yield return null; // �ȴ���һ֡
                }

                // �л������߶���
                animator.SetBool("isSit", false);
                animator.SetBool("isWalk", true);

                // �ȴ� NPC �ƶ���Ŀ���
                while (agent.remainingDistance > agent.stoppingDistance)
                {
                    yield return null; // �ȴ���һ֡
                }

                // ����Ŀ����ֹͣ���ߣ��л������¶���
                animator.SetBool("isWalk", false);
                animator.SetBool("isSit", true);
            }
        }
    }


    private Vector3 GetRandomPoint(Vector3 center, float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius; // �����η�Χ�����������
        randomDirection += center;

        // ȷ����������� NavMesh ��
        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, radius, NavMesh.AllAreas))
        {
            return hit.position; // ������Ч�ĵ�����
        }
        return center; // ����������Ч������ԭ��
    }
}
