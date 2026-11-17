
using UnityEngine;

public class PhotoHintTrigger : MonoBehaviour
{
    public GameObject photoHintImage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            photoHintImage.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            photoHintImage.SetActive(false);
        }
    }
}
