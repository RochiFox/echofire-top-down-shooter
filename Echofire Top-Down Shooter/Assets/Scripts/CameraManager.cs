using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    private CinemachineVirtualCamera virtualCamera;
    private CinemachineFramingTransposer transposer;

    [Header("Camera distance")] [SerializeField]
    private bool canChangeCameraDistance;

    [SerializeField] private float distanceChangeRate;
    private float targetCameraDistance;

    private void Awake()
    {
        if (!instance)
            instance = this;
        else
        {
            Debug.LogWarning("You had more than one camera manager.");
            Destroy(gameObject);
        }

        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    private void Update()
    {
        UpdateCameraDistance();
    }

    private void UpdateCameraDistance()
    {
        if (canChangeCameraDistance == false)
            return;

        float currentDistance = transposer.m_CameraDistance;

        if (Mathf.Abs(targetCameraDistance - currentDistance) < 0.1f)
            return;

        transposer.m_CameraDistance = Mathf.Lerp(currentDistance, targetCameraDistance,
            distanceChangeRate * Time.deltaTime);
    }

    public void ChangeCameraDistance(float distance) => targetCameraDistance = distance;
}