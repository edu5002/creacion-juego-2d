using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PuertaMecanismo : MonoBehaviour
{
    [SerializeField] private Collider2D colliderBloqueo;

    private Animator animador;
    private bool abierta;

    private void Awake()
    {
        animador = GetComponent<Animator>();
    }

    public void Abrir()
    {
        if (abierta)
            return;

        abierta = true;
        animador.SetTrigger("Abrir");
    }

    // Se llama desde el final de la animación de apertura.
    public void FinalizarApertura()
    {
        if (!abierta)
            return;

        if (colliderBloqueo != null)
            colliderBloqueo.enabled = false;
    }
}