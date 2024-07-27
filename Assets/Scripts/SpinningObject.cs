using UnityEngine;

public class SpinningObject : MonoBehaviour
{
    [SerializeField] private Vector3 spinAxis = Vector3.up;
    [SerializeField][Min(0f)] private float spinSpeed = 45f;

    void Update()
    {
        var spinDelta = spinSpeed * Time.deltaTime;
        transform.RotateAround(transform.position, spinAxis, spinDelta);
    }
}