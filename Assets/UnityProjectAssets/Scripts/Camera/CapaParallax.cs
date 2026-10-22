using UnityEngine;

[DefaultExecutionOrder(1000)]
public class CapaParallax : MonoBehaviour
{
    [SerializeField] private Transform camara;

    [Header("Cuánto acompaña a la cámara")]
    [Range(0f, 1f)]
    [SerializeField] private float factorHorizontal = 0.8f;

    [Range(0f, 1f)]
    [SerializeField] private float factorVertical = 0f;

    private Vector3 posicionInicial;
    private Vector3 posicionInicialCamara;

    private void Start()
    {
        if (camara == null && Camera.main != null)
            camara = Camera.main.transform;

        if (camara == null)
        {
            Debug.LogWarning(
                "Asigna la cámara en CapaParallax.", this
            );

            enabled = false;
            return;
        }

        posicionInicial = transform.position;
        posicionInicialCamara = camara.position;
    }

    private void LateUpdate()
    {
        Vector3 desplazamiento =
            camara.position - posicionInicialCamara;

        transform.position = posicionInicial + new Vector3(
            desplazamiento.x * factorHorizontal,
            desplazamiento.y * factorVertical,
            0f
        );
    }
}