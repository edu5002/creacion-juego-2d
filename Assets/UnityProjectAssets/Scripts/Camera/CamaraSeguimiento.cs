using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CamaraSeguimiento : MonoBehaviour
{
    public Transform jugador;

    [Header("Encuadre")]
    public Vector2 desplazamiento = new Vector2(0f, 1f);
    public bool seguirVerticalmente = true;

    [Header("Suavidad")]
    public float tiempoSuavizado = 0.15f;

    [Header("Bordes del escenario")]
    public Transform limiteCamaraInferior;
    public Transform limiteCamaraSuperior;

    private Camera camara;
    private Vector3 velocidad;
    private float posicionZ;
    private float alturaInicial;

    private void Awake()
    {
        camara = GetComponent<Camera>();
    }

    private void Start()
    {
        posicionZ = transform.position.z;
        alturaInicial = transform.position.y;

        if (jugador != null)
            transform.position = Limitar(ObtenerDestino());
    }

    private void LateUpdate()
    {
        if (jugador == null)
            return;

        Vector3 destino = Limitar(ObtenerDestino());

        Vector3 posicion = Vector3.SmoothDamp(
            transform.position,
            destino,
            ref velocidad,
            Mathf.Max(0.01f, tiempoSuavizado)
        );

        transform.position = Limitar(posicion);
    }

    private Vector3 ObtenerDestino()
    {
        float altura = seguirVerticalmente
            ? jugador.position.y + desplazamiento.y
            : alturaInicial;

        return new Vector3(
            jugador.position.x + desplazamiento.x,
            altura,
            posicionZ
        );
    }

    private Vector3 Limitar(Vector3 posicion)
    {
        if (limiteCamaraInferior == null ||
            limiteCamaraSuperior == null)
            return posicion;

        float izquierda = Mathf.Min(
            limiteCamaraInferior.position.x,
            limiteCamaraSuperior.position.x
        );

        float derecha = Mathf.Max(
            limiteCamaraInferior.position.x,
            limiteCamaraSuperior.position.x
        );

        float abajo = Mathf.Min(
            limiteCamaraInferior.position.y,
            limiteCamaraSuperior.position.y
        );

        float arriba = Mathf.Max(
            limiteCamaraInferior.position.y,
            limiteCamaraSuperior.position.y
        );

        float mitadAlto = camara.orthographicSize;
        float mitadAncho = mitadAlto * camara.aspect;

        posicion.x = LimitarEje(
            posicion.x, izquierda, derecha, mitadAncho
        );

        posicion.y = LimitarEje(
            posicion.y, abajo, arriba, mitadAlto
        );

        return posicion;
    }

    private float LimitarEje(
        float posicion, float minimo, float maximo, float margen)
    {
        if (maximo - minimo <= margen * 2f)
            return (minimo + maximo) / 2f;

        return Mathf.Clamp(
            posicion, minimo + margen, maximo - margen
        );
    }
}