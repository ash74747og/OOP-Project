using UnityEngine;

public class StaticPlatform : PlatformBase
{
    // Polymorphism: Overriding the abstract method from the base class
    protected override void SetupPlatform()
    {
        // Simply set the scale to the Size property defined in the base class
        transform.localScale = Size;
    }

    // Polymorphism: Overriding the abstract method
    public override void OnPlayerStep(GameObject player)
    {
        // Static platform does nothing when stepped on
    }
}
