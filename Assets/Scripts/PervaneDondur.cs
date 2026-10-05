using UnityEngine;

/// <summary>
/// Spins the propeller (Pervane) around its own hub.
/// Attach only to the Pervane object.
/// </summary>
public class PervaneDondur : MonoBehaviour
{
    [Tooltip("Rotation speed in degrees per second.")]
    [SerializeField] private float rotationSpeed = 1200f;

    [Tooltip("Rotation axis in the propeller's LOCAL space (the hub axis).")]
    [SerializeField] private Vector3 localRotationAxis = Vector3.up;

    private void Update()
    {
        if (localRotationAxis.sqrMagnitude < 0.0001f) return;
        transform.Rotate(localRotationAxis.normalized, rotationSpeed * Time.deltaTime, Space.Self);
    }
}
