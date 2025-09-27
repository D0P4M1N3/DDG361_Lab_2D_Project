using Unity.Cinemachine;
using UnityEngine;

public class RoomCamera : MonoBehaviour
{
    public CinemachineCamera cinemachineCamera;

    private void Start()
    {
        cinemachineCamera = GetComponentInChildren<CinemachineCamera>();
    }


}
