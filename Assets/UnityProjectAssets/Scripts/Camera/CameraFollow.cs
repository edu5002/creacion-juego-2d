using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform objetivo;

    [Header("Seguimiento")]
    public float suavizado = 5f;

    [Header("Desplazamiento")]
    public float offsetX = 2f;
    public float offsetY = 1f;

    private float posicionZ;

    private void Start()
    {
        posicionZ = transform.position.z;
    }

    private void LateUpdate()
    {
        if (objetivo == null)
        {
            return;
        }

        Vector3 posicionDeseada = new Vector3(
            objetivo.position.x + offsetX,
            objetivo.position.y + offsetY,
            posicionZ
        );

        transform.position = Vector3.Lerp(
            transform.position,
            posicionDeseada,
            suavizado * Time.deltaTime
        );
    }
}