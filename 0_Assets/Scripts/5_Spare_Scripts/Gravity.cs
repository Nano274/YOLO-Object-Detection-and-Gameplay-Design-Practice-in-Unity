using UnityEngine;

public class ExtraGravity : MonoBehaviour
{
    public float extraGravityScale = 2.0f; // 重力缩放倍数
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // 自定义增加重力效果
        rb.AddForce(Vector3.down * extraGravityScale * Physics.gravity.magnitude, ForceMode.Acceleration);
    }
}
