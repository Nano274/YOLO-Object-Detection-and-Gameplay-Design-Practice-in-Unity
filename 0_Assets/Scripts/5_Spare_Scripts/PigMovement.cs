using UnityEngine;
using System.Collections;

public class PigMovement : MonoBehaviour
{
    public float moveSpeed = 2f;         // �ƶ��ٶ�
    public float waitTime = 3f;          // ���·���ǰ�ȴ���ʱ��
    public float moveDistance = 5f;      // ����ƶ���������

    private Animator animator;           // ����������
    private Vector3 targetPosition;      // Ŀ��λ��
                                         // private bool isMoving = false;       // �Ƿ����ƶ�

    void Start()
    {
        animator = GetComponent<Animator>(); // ��ȡ Animator ���
        StartCoroutine(MoveRandomly());
    }

    IEnumerator MoveRandomly()
    {
        while (true) // ����ѭ��������һֱ�ƶ�
        {
            // �ȴ�ָ����ʱ�䣬�������¶���
            animator.SetBool("isWalk", false);
            animator.SetBool("isSit", true);  // ��ʼ���¶���
            yield return new WaitForSeconds(waitTime);

            // ����һ���µ����Ŀ��λ��
            Vector3 randomDirection = new Vector3(
                Random.Range(-moveDistance, moveDistance),
                0,
                Random.Range(-moveDistance, moveDistance)
            );
            targetPosition = transform.position + randomDirection;

            // �л�����·����
            animator.SetBool("isSit", false);
            animator.SetBool("isWalk", true);

            // �ƶ���Ŀ��λ��
            //isMoving = true;
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPosition,
                    moveSpeed * Time.deltaTime
                );

                // ���ֳ���Ŀ�귽��
                Vector3 direction = (targetPosition - transform.position).normalized;
                if (direction != Vector3.zero)
                {
                    transform.forward = direction;
                }

                yield return null; // �ȴ���һ֡
            }

            // ֹͣ�ƶ����л������¶���
            animator.SetBool("isWalk", false);
            animator.SetBool("isSit", true);

            //isMoving = false;
        }
    }
}
