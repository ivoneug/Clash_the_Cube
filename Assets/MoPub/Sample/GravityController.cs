using UnityEngine;

public class GravityController : MonoBehaviour
{
#if mopub_native_beta
    public float Force = 9.8f;

    private void FixedUpdate()
    {
        Vector2 dir;
        dir.x = Input.acceleration.x;
        dir.y = Input.acceleration.y;
#if UNITY_EDITOR
        dir = Vector2.down;
#endif
        Physics.gravity = dir * Force;
    }
#endif
}
