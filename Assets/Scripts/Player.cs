using UnityEngine;

public class Player : MonoBehaviour
{
    public float Speed = 5f;
    public float RotationSpeed = 5f;
    public float JumpForce = 5f;

    private bool isGrounded;
    private Rigidbody rb;
    private Vector2 turn;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Update()
    {
        CameraLogic();
    }

    void FixedUpdate()
    {
        JumpLogic();
        MovementLogic();
    }

    private void CameraLogic()
    {
        turn.x += Input.GetAxis("Mouse X");
        turn.y += Input.GetAxis("Mouse Y");

        transform.localRotation = Quaternion.Euler(turn.y, turn.x, 0);
    }

    private void MovementLogic()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");

        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        // что бы скорость была стабильной в любом случае
        // и учитывая что мы вызываем из FixedUpdate мы умножаем на fixedDeltaTimе
        transform.Translate(movement * Speed * Time.fixedDeltaTime);
    }

    private void JumpLogic()
    {
        if (Input.GetAxis("Jump") > 0)
        {
            if (isGrounded)
            {
                rb.AddForce(Vector3.up * JumpForce);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        IsGroundedUpate(collision, true);
    }

    void OnCollisionExit(Collision collision)
    {
        IsGroundedUpate(collision, false);
    }

    private void IsGroundedUpate(Collision collision, bool value)
    {
        if (collision.gameObject.tag == ("Ground"))
        {
            isGrounded = value;
        }
    }
}
