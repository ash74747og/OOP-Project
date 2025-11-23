using UnityEngine;

public class StaticPlatform : PlatformBase
{
    // Polymorphism: Overriding the abstract method from the base class
    protected override void SetupPlatform()
    {
        // Safety Check: If a Rigidbody exists (accidentally added), ensure it's static
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    // Polymorphism: Overriding the abstract method
    public override void OnPlayerStep(GameObject player)
    {
        // Static platform does nothing when stepped on
    }
}
