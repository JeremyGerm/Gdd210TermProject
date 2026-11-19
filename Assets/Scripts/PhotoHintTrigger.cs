
using UnityEngine;
using UnityEngine.InputSystem;

public class PhotoHintTrigger : MonoBehaviour
{
    public GameObject photoHintImage;
    private PhotoSpot activePhotoSpot;
    [SerializeField] private float dotThreshold = 0.2f;
    [SerializeField] private Transform cam;
    [SerializeField] private CameraToggle cameraToggle;

    private void Update()
    {
        if (activePhotoSpot != null && cameraToggle.handheldCamera.isActiveAndEnabled)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                float dot = Vector3.Dot(cam.forward, (activePhotoSpot.Target - transform.position).normalized);
                if (dot >= (1f - dotThreshold))
                {
                    Debug.Log("Photo Taken!");
                }
            }
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, 1f);
        for(int i = 0; i < hits.Length; i++)
        {
            if (hits[i].TryGetComponent<PhotoSpot>(out PhotoSpot photoSpot))
            {
                activePhotoSpot = photoSpot;
                photoHintImage.SetActive(true);
                return;
            }
        }

        if(activePhotoSpot != null)
        {
            activePhotoSpot = null;
            photoHintImage.SetActive(false);
        }

       
    }

    private void OnDrawGizmos()
    {
        if (activePhotoSpot != null)
        {
            float dot = Vector3.Dot(cam.forward, (activePhotoSpot.Target - transform.position).normalized);
            //Debug.Log(dot);
            if(dot >= (1f - dotThreshold))
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.red;
            }
            Gizmos.DrawLine(transform.position + new Vector3(0,1,0), activePhotoSpot.Target);
        }
    }
}
