using Cinemachine;
using UnityEngine;

public class CinemachineController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    private void Awake()
    {
        if (virtualCamera == null)
            virtualCamera = GetComponent<CinemachineVirtualCamera>();

    }

    public void SetFollowTarget(Transform target)
    {
        if (virtualCamera != null && target != null)
        {
            virtualCamera.Follow = target;
            // Loại bỏ Look At để tránh rotation tự động
            virtualCamera.LookAt = target;
        }
    }
}