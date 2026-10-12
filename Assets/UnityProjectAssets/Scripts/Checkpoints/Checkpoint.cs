using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Checkpoint : MonoBehaviour
{
    public SpriteRenderer imagenAltar;
    public Sprite spriteApagado;
    public Sprite spriteEncendido;
    public Transform puntoReaparicion;

    [Min(1)]
    public int orden = 1;

    private bool activado;

    private void Awake()
    {
        if (imagenAltar != null && spriteApagado != null)
            imagenAltar.sprite = spriteApagado;
    }

    private void Start()
    {
        int guardado = GuardadoPartida.ObtenerCheckpoint(
            gameObject.scene.name
        );

        // Los altares hasta el progreso guardado aparecen encendidos.
        if (guardado >= orden)
            Encender();
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        if (activado)
            return;

        PlayerVida jugador =
            otro.GetComponentInParent<PlayerVida>();

        if (jugador == null || jugador.EstaMuerto)
            return;

        if (puntoReaparicion == null ||
            imagenAltar == null ||
            spriteEncendido == null)
        {
            Debug.LogWarning(
                "Faltan referencias en el checkpoint.", this
            );
            return;
        }

        jugador.ActivarCheckpoint(
            puntoReaparicion.position, orden
        );

        Encender();
    }

    private void Encender()
    {
        activado = true;

        if (imagenAltar != null && spriteEncendido != null)
            imagenAltar.sprite = spriteEncendido;
    }
}