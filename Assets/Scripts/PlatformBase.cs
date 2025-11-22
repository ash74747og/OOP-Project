using UnityEngine;

public abstract class PlatformBase : MonoBehaviour
{
    // Encapsulation: Private serialized fields with public properties
    [SerializeField] private Vector3 size = new Vector3(2.5f, 0.25f, 2f);
    [SerializeField] private bool isCollidable;

    public Vector3 Size
    {
        get { return size; }
        set { size = value; }
    }

    public bool IsCollidable
    {
        get { return isCollidable; }
        set { isCollidable = value; }
    }

    protected virtual void Awake()
    {
        // Ensure a collider exists
        if (GetComponent<Collider>() == null)
        {
            // Adding a BoxCollider as a default if none exists, 
            // though specific platforms might want different colliders.
            gameObject.AddComponent<BoxCollider>();
        }

        SetupPlatform();
    }

    // Abstraction: Abstract methods to be implemented by derived classes
    protected abstract void SetupPlatform();
    
    public abstract void OnPlayerStep(GameObject player);

    // Virtual method that can be overridden
    public virtual void OnPlayerLeave(GameObject player)
    {
        // Default implementation does nothing
    }
}
