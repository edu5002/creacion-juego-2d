using UnityEngine;
using UnityEngine.Events;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Animator))]
public class PalancaInteractiva : MonoBehaviour
{
    [SerializeField] private Transform jugador;
    [SerializeField, Min(0.1f)] private float distanciaActivacion = 1.5f;
    [SerializeField] private GameObject textoAviso;

    [SerializeField] private UnityEvent alActivar = new UnityEvent();

    private Animator animador;
    private bool activada;

    private void Awake()
    {
        animador = GetComponent<Animator>();
        MostrarAviso(false);
    }

    private void Update()
    {
        if (activada || jugador == null || Time.timeScale == 0f)
        {
            MostrarAviso(false);
            return;
        }

        float distancia = Vector2.Distance(
            transform.position,
            jugador.position
        );

        bool estaCerca = distancia <= distanciaActivacion;
        MostrarAviso(estaCerca);

        if (estaCerca && PulsarInteraccion())
        {
            activada = true;
            MostrarAviso(false);

            animador.SetTrigger("Activar");
            alActivar.Invoke();
        }
    }

    private void MostrarAviso(bool mostrar)
    {
        if (textoAviso != null && textoAviso.activeSelf != mostrar)
            textoAviso.SetActive(mostrar);
    }

    private void OnDisable()
    {
        MostrarAviso(false);
    }

    private bool PulsarInteraccion()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null
            && Keyboard.current.eKey.wasPressedThisFrame;
#elif ENABLE_LEGACY_INPUT_MANAGER
        return Input.GetKeyDown(KeyCode.E);
#else
        return false;
#endif
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaActivacion);
    }
}