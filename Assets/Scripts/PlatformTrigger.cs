using UnityEngine;

public class PlatformTrigger : MonoBehaviour
{
    [SerializeField] private PlatformBase parentPlatform;

    private void Awake()
    {
        // Safety Check: Ensure transform scale is positive immediately
        Vector3 validScale = new Vector3(
            Mathf.Abs(transform.localScale.x),
            Mathf.Abs(transform.localScale.y),
            Mathf.Abs(transform.localScale.z)
        );
        transform.localScale = validScale;

        // Auto-find parent if not assigned, assuming this script is on a child object
        if (parentPlatform == null)
        {
            parentPlatform = GetComponentInParent<PlatformBase>();
        }
    }

    private void Start()
    {
        // Safety Check: Ensure BoxCollider size is positive to prevent Unity warnings
        BoxCollider box = GetComponent<BoxCollider>();
        if (box != null)
        {
            Vector3 validSize = new Vector3(
                Mathf.Abs(box.size.x),
                Mathf.Abs(box.size.y),
                Mathf.Abs(box.size.z)
            );
            box.size = validSize;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (parentPlatform != null)
            {
                parentPlatform.OnPlayerStep(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (parentPlatform != null)
            {
                parentPlatform.OnPlayerLeave(other.gameObject);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (parentPlatform != null)
            {
                // Continuously confirm player presence to prevent accidental detachment
                parentPlatform.OnPlayerStep(other.gameObject);
            }
        }
    }

    private void OnValidate()
    {
        // Fix negative scale
        if (transform.localScale.x < 0 || transform.localScale.y < 0 || transform.localScale.z < 0)
        {
            Vector3 validScale = new Vector3(
                Mathf.Abs(transform.localScale.x),
                Mathf.Abs(transform.localScale.y),
                Mathf.Abs(transform.localScale.z)
            );
            transform.localScale = validScale;
        }

        // Fix negative collider size
        BoxCollider box = GetComponent<BoxCollider>();
        if (box != null)
        {
            if (box.size.x < 0 || box.size.y < 0 || box.size.z < 0)
            {
                Vector3 validSize = new Vector3(
                    Mathf.Abs(box.size.x),
                    Mathf.Abs(box.size.y),
                    Mathf.Abs(box.size.z)
                );
                box.size = validSize;
            }
        }
    }
}
