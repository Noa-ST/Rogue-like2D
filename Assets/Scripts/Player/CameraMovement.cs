using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform target; // Tham chiếu đến player, gán qua Inspector hoặc GameManager
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f); // Offset mặc định (z = -10 cho 2D)
    [SerializeField] private float smoothSpeed = 0.125f; // Tốc độ mượt mà (0-1, 0 là tức thời, 1 là rất mượt)
    [SerializeField] private float viewSize = 10f; // Tăng kích thước hiển thị (tương đương Ortho Size)

    [SerializeField] private Vector2 minBounds = new Vector2(-10f, -10f); // Giới hạn tối thiểu
    [SerializeField] private Vector2 maxBounds = new Vector2(10f, 10f); // Giới hạn tối đa

    private Camera cam;
    private Vector3 velocity = Vector3.zero;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("Camera component not found on " + gameObject.name + "!");
        }
        if (cam.orthographic)
        {
            cam.orthographicSize = viewSize; // Đặt kích thước ban đầu
        }
        else
        {
            Debug.LogWarning("Camera is not in Orthographic mode! Switching to Orthographic.");
            cam.orthographic = true;
            cam.orthographicSize = viewSize;
        }
    }

    private void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("CameraMovement target not set at Start, waiting for GameManager...");
        }
    }

    private void LateUpdate() // Sử dụng LateUpdate để đảm bảo target đã cập nhật vị trí
    {
        if (target == null)
        {
            Debug.LogWarning("CameraMovement target is null!");
            return;
        }

        // Tính vị trí mục tiêu của camera
        Vector3 targetPosition = target.position + offset;

        // Giới hạn vị trí camera trong ranh giới
        targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);

        // Sử dụng SmoothDamp để di chuyển mượt mà
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothSpeed);

        // Debug vị trí camera
        Debug.Log("Camera position: " + transform.position + ", Target position: " + target.position + ", View Size: " + viewSize);
    }

    // Phương thức công khai để gán target từ GameManager
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (target != null)
        {
            Debug.Log("CameraMovement target set to: " + target.name);
        }
        else
        {
            Debug.LogWarning("Attempted to set null target for CameraMovement!");
        }
    }

    // Phương thức để điều chỉnh kích thước hiển thị (tương tự Ortho Size)
    public void SetViewSize(float size)
    {
        if (cam != null && cam.orthographic)
        {
            viewSize = size;
            cam.orthographicSize = viewSize;
            Debug.Log("Camera view size set to: " + viewSize);
        }
    }
}