using Unity.Cinemachine;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private CameraManagerTest cameraManager;
    [SerializeField] private CinemachineCamera targetCamera;
    [SerializeField] private LayerMask playerLayer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsPlayer(other))
        {
            cameraManager.SwitchCamera(targetCamera);
            Debug.Log("Switched to " + targetCamera.name);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (IsPlayer(other))
        {
            // Check if player is still inside ANY CameraSwitcher
            if (!IsPlayerInsideAnySwitcher(other))
            {
                cameraManager.SwitchToPrimaryCamera();
                Debug.Log("Switched back to Primary Camera");
            }
        }
    }

    private bool IsPlayer(Collider2D col)
    {
        return ((1 << col.gameObject.layer) & playerLayer) != 0;
    }

    private bool IsPlayerInsideAnySwitcher(Collider2D player)
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(player.transform.position);
        foreach (var hit in hits)
        {
            if (hit.GetComponent<CameraSwitcher>() != null)
            {
                return true; // Player still inside another CameraSwitcher
            }
        }
        return false;
    }
}
