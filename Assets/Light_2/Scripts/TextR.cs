using UnityEngine;

public class BillboardText : MonoBehaviour
{
    void LateUpdate()
    {
        // Làm cho đối tượng nhìn về phía Camera
        transform.LookAt(
            transform.position + Camera.main.transform.rotation * Vector3.forward,
            Camera.main.transform.rotation * Vector3.up
        );
    }
}
