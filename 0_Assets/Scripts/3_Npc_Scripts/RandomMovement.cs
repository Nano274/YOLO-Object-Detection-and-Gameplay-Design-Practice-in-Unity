using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class RandomMovement : MonoBehaviour
{
    public float wanderRadius = 10f; // 随机移动的范围
    public float waitTime = 2f;      // 停留时间

    private NavMeshAgent agent;      // NavMeshAgent 组件
    private Animator animator;       // 动画控制器

    private void Start()
    {
        // 获取 NavMeshAgent 和 Animator 组件
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        StartCoroutine(Wander());
    }

    private IEnumerator Wander()
    {
        while (true)
        {
            // 停下并播放坐下动画
            animator.SetBool("isWalk", false);
            animator.SetBool("isSit", true);

            yield return new WaitForSeconds(waitTime); // 等待指定时间

            // 随机生成目标点
            Vector3 randomPoint = GetRandomPoint(transform.position, wanderRadius);

            // 设置目标点，进入行走状态
            if (agent.SetDestination(randomPoint))
            {
                // 等待路径计算完成
                while (agent.pathPending)
                {
                    yield return null; // 等待下一帧
                }

                // 切换到行走动画
                animator.SetBool("isSit", false);
                animator.SetBool("isWalk", true);

                // 等待 NPC 移动到目标点
                while (agent.remainingDistance > agent.stoppingDistance)
                {
                    yield return null; // 等待下一帧
                }

                // 到达目标点后，停止行走，切换回坐下动画
                animator.SetBool("isWalk", false);
                animator.SetBool("isSit", true);
            }
        }
    }


    private Vector3 GetRandomPoint(Vector3 center, float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius; // 在球形范围内生成随机点
        randomDirection += center;

        // 确保随机点落在 NavMesh 上
        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, radius, NavMesh.AllAreas))
        {
            return hit.position; // 返回有效的导航点
        }
        return center; // 如果随机点无效，返回原点
    }
}
