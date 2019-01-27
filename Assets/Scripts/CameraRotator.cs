using UnityEngine;
using System.Collections;

public class CameraRotator : MonoBehaviour
{

    [SerializeField]
    private Vector3 center;
    [SerializeField]
    private float speed;

    private void Update()
    {
        transform.RotateAround(center, Vector3.up, Time.deltaTime * speed);
    }
}
