using UnityEngine;

public class ExtraGravity : MonoBehaviour
{
    public float extraGravityScale = 2.0f; // �������ű���
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // �Զ�����������Ч��
        rb.AddForce(Vector3.down * extraGravityScale * Physics.gravity.magnitude, ForceMode.Acceleration);
    }
}
