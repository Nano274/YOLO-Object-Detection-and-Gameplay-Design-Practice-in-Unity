using UnityEngine;

public class remove : MonoBehaviour
{
    // 移动速度参数
    public float walkSpeed = 5f;     // 正常移动速度
    public float sprintSpeed = 10f;  // 冲刺速度
    public float crouchSpeed = 2f;   // 蹲下速度
    public float jumpForce = 5f;     // 跳跃力度

    // 内部状态
    private bool isGrounded;         // 是否在地面上
    private float currentSpeed;      // 当前移动速度
    private Rigidbody rb;            // Rigidbody 组件

    // 摄像机相关
    public Transform cameraTransform;   // 摄像机的 Transform
    public float mouseSensitivity = 100f; // 鼠标灵敏度
    private float xRotation = 0f;        // X轴旋转角度

    // 检测地面相关
    public Transform groundCheck;    // 地面检测点
    public LayerMask groundLayer;    // 地面层级
    public float groundDistance = 0.4f;

    void Start()
    {
        // 获取 Rigidbody 组件
        rb = GetComponent<Rigidbody>();

        // 初始化速度
        currentSpeed = walkSpeed;

        // 隐藏并锁定鼠标指针
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // 摄像机旋转
        HandleCameraRotation();

        // 移动
        MovePlayer();

        // 跳跃
        Jump();

        // 检测冲刺和蹲下
        HandleSprintAndCrouch();
    }

    void MovePlayer()
    {
        // 获取输入（WASD 和 上下左右键）
        float moveX = Input.GetAxis("Horizontal"); // 左右（A/D 或 左/右）
        float moveZ = Input.GetAxis("Vertical");   // 前后（W/S 或 上/下）

        // 计算移动方向
        Vector3 moveDirection = transform.right * moveX + transform.forward * moveZ;

        // 应用移动速度
        Vector3 moveVelocity = moveDirection * currentSpeed;
        moveVelocity.y = rb.linearVelocity.y; // 保持Y轴速度不变

        // 赋予刚体速度
        rb.linearVelocity = moveVelocity;
    }

    void Jump()
    {
        // 检测是否在地面上
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);

        // 如果按下空格且在地面上，则跳跃
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }
    }

    void HandleSprintAndCrouch()
    {
        // 检测按下 Shift 键进行冲刺
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = sprintSpeed;
        }
        // 检测按下 Ctrl 键进行蹲下
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            currentSpeed = crouchSpeed;
        }
        // 正常速度
        else
        {
            currentSpeed = walkSpeed;
        }
    }

    void HandleCameraRotation()
    {
        // 按住鼠标右键时，锁定视角，不执行旋转逻辑
        if (Input.GetMouseButton(1)) // 鼠标右键（Mouse1）
        {
            return; // 直接返回，不执行后续的旋转代码
        }

        // 获取鼠标输入
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 计算 X 轴旋转角度（垂直旋转）
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // 限制上下旋转角度

        // 应用摄像机旋转
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX); // 水平旋转角色
    }


}
