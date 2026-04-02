using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private readonly int hashSpeed = Animator.StringToHash("Move");

    public float moveSpeed = 5f;
    public float rotationSpeed = 15f;

    private Camera mainCamera;

    private Animator playerAnimator;
    private PlayerInput playerInput;
    private Rigidbody playerRigidbody;

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        playerRigidbody = GetComponent<Rigidbody>();

        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        Vector3 moveDirection = (Vector3.forward * playerInput.Vertical) + (Vector3.right * playerInput.Horizontal);

        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
        }

        Vector3 delta = moveDirection * moveSpeed * Time.deltaTime;
        playerRigidbody.MovePosition(playerRigidbody.position + delta);

        AimTowardsMouse();
    }

    private void AimTowardsMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        Plane groundPlane = new Plane(Vector3.up, transform.position);

        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 targetPoint = ray.GetPoint(rayDistance);

            Vector3 lookDirection = targetPoint - transform.position;

            lookDirection.y = 0;

            if (lookDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }

    private void UpdateAnimation()
    {
        Vector2 inputVector = new Vector2(playerInput.Horizontal, playerInput.Vertical);

        float currentSpeed = Mathf.Clamp01(inputVector.magnitude);

        playerAnimator.SetFloat(hashSpeed, currentSpeed);
    }
}
