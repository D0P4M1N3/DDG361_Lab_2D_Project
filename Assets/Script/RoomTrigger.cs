using Unity.Cinemachine;
using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    private VirtualCameraManager VirtualCameraManager;
    public CinemachineCamera cinemachineCamera;


    private void Start()
    {
        VirtualCameraManager = FindObjectOfType<VirtualCameraManager>();
        cinemachineCamera = GetComponentInChildren<CinemachineCamera>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            VirtualCameraManager.SwitchCamera(cinemachineCamera);

        }
    }
}
