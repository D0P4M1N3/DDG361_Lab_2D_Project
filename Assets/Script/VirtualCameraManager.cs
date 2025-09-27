using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class VirtualCameraManager : MonoBehaviour
{
    public List<GameObject> virtualCam = new List<GameObject>();

    private void Start()
    {

    }

    public void SwitchCamera(CinemachineCamera currentCam)
    {
        foreach (var cam in virtualCam)
        {
            cam.SetActive(false);
        }

        currentCam.gameObject.SetActive(true);
    }

}
