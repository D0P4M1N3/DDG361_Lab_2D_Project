using Unity.Cinemachine;
using UnityEngine;

public class CameraManagerTest : MonoBehaviour
{
    [SerializeField] private CinemachineCamera[] cameras;

    [SerializeField] private CinemachineCamera defaultCamera;
    private CinemachineCamera currentCamera;

    private void Start()
    {
        SetActiveCamera(defaultCamera);
    }

    public void SwitchCamera(CinemachineCamera newCamera)
    {
        SetActiveCamera(newCamera);
    }

    public void SwitchToPrimaryCamera()
    {
        SetActiveCamera(defaultCamera);
    }

    private void SetActiveCamera(CinemachineCamera cam)
    {
        currentCamera = cam;

        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].Priority = (cameras[i] == currentCamera) ? 20 : 10;
        }
    }
}
