using UnityEngine;

public class PhotoSpot : MonoBehaviour
{
    public Vector3 Target => target.position + offset;

    [SerializeField] private Transform target;

    [SerializeField] private Vector3 offset = new Vector3(0f, 1f, 0f);

    private void OnDrawGizmosSelected()
    {
        if (target != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position + new Vector3(0,1,0) , target.position + offset);
        }
    }
}
