using UnityEngine;
using System.Collections;

public class PigMovement : MonoBehaviour
{
    public float moveSpeed = 2f;         // 移动速度
    public float waitTime = 3f;          // 在新方向前等待的时间
    public float moveDistance = 5f;      // 随机移动的最大距离

    private Animator animator;           // 动画控制器
    private Vector3 targetPosition;      // 目标位置
                                         // private bool isMoving = false;       // 是否在移动

    void Start()
    {
        animator = GetComponent<Animator>(); // 获取 Animator 组件
        StartCoroutine(MoveRandomly());
    }

    IEnumerator MoveRandomly()
    {
        while (true) // 无限循环，让猪一直移动
        {
            // 等待指定的时间，播放坐下动画
            animator.SetBool("isWalk", false);
            animator.SetBool("isSit", true);  // 开始坐下动画
            yield return new WaitForSeconds(waitTime);

            // 计算一个新的随机目标位置
            Vector3 randomDirection = new Vector3(
                Random.Range(-moveDistance, moveDistance),
                0,
                Random.Range(-moveDistance, moveDistance)
            );
            targetPosition = transform.position + randomDirection;

            // 切换到走路动画
            animator.SetBool("isSit", false);
            animator.SetBool("isWalk", true);

            // 移动到目标位置
            //isMoving = true;
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPosition,
                    moveSpeed * Time.deltaTime
                );

                // 保持朝向目标方向
                Vector3 direction = (targetPosition - transform.position).normalized;
                if (direction != Vector3.zero)
                {
                    transform.forward = direction;
                }

                yield return null; // 等待下一帧
            }

            // 停止移动，切换回坐下动画
            animator.SetBool("isWalk", false);
            animator.SetBool("isSit", true);

            //isMoving = false;
        }
    }
}
