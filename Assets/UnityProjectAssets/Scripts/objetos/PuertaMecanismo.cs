using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Animator))]
public class PuertaMecanismo : MonoBehaviour
{
    [SerializeField] private Collider2D colliderBloqueo;
    [SerializeField] private GameObject textoAviso;
    [SerializeField] private FundidoEntrada fundidoPuertas;

    [Header("Conexión entre puertas")]
    [SerializeField] private Transform jugador;
    [SerializeField] private PuertaMecanismo puertaConectada;
    [SerializeField] private Transform puntoLlegada;
    [Header("Límites de la zona de destino")]
    [SerializeField] private Transform limiteInferiorDestino;
    [SerializeField] private Transform limiteSuperiorDestino;
    [SerializeField, Min(0.1f)] private float distanciaEntrada = 1.5f;

    [Header("Efectos de apertura")]
    [SerializeField] private bool reproducirEfectosApertura = true;
    [SerializeField] private CamaraSeguimiento camaraSeguimiento;
    [SerializeField, Min(0f)] private float duracionTemblor = 0.3f;
    [SerializeField, Min(0f)] private float intensidadTemblor = 0.06f;

    private Animator animador;
    private AudioSource fuenteAudio;
    private bool abierta;
    private bool aperturaTerminada;
    private float entradaBloqueadaHasta;

    private void Awake()
    {
        animador = GetComponent<Animator>();
        fuenteAudio = GetComponent<AudioSource>();
        MostrarAviso(false);
    }

    private void Update()
    {
        bool puedeEntrar =
            aperturaTerminada &&
            jugador != null &&
            puntoLlegada != null &&
            puertaConectada != null &&
            puertaConectada.aperturaTerminada &&
            (fundidoPuertas == null || !fundidoPuertas.EnCurso) &&
            Time.timeScale > 0f &&
            Time.time >= entradaBloqueadaHasta;

        if (!puedeEntrar)
        {
            MostrarAviso(false);
            return;
        }

        float distancia = Vector2.Distance(
            transform.position, jugador.position
        );

        bool estaCerca = distancia <= distanciaEntrada;
        MostrarAviso(estaCerca);

        if (estaCerca && PulsarInteraccion())
        {
            MostrarAviso(false);

            if (fundidoPuertas != null)
                fundidoPuertas.CruzarPuerta(Entrar);
            else
                Entrar();
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

    public void Abrir()
    {
        if (abierta)
            return;

        abierta = true;
        animador.SetTrigger("Abrir");

        if (reproducirEfectosApertura)
        {
            if (fuenteAudio != null && fuenteAudio.clip != null)
                fuenteAudio.Play();

            if (camaraSeguimiento != null)
            {
                camaraSeguimiento.Sacudir(
                    duracionTemblor, intensidadTemblor
                );
            }
        }

        // Abre también el acceso de regreso.
        if (puertaConectada != null)
            puertaConectada.Abrir();
    }

    // Conserva este evento al final de la animación.
    public void FinalizarApertura()
    {
        if (!abierta)
            return;

        aperturaTerminada = true;

        if (colliderBloqueo != null)
            colliderBloqueo.enabled = false;
    }

    private void Entrar()
    {
        if (puntoLlegada == null || puertaConectada == null)
            return;

        if (!puertaConectada.aperturaTerminada)
            return;

        MostrarAviso(false);
        puertaConectada.MostrarAviso(false);

        // Evita regresar accidentalmente en el mismo instante.
        entradaBloqueadaHasta = Time.time + 0.4f;
        puertaConectada.entradaBloqueadaHasta = Time.time + 0.4f;

        Vector3 destino = new Vector3(
            puntoLlegada.position.x,
            puntoLlegada.position.y,
            jugador.position.z
        );

        Rigidbody2D cuerpo = jugador.GetComponent<Rigidbody2D>();

        if (cuerpo != null)
        {
            cuerpo.linearVelocity = Vector2.zero;
            cuerpo.position = new Vector2(destino.x, destino.y);
        }

        jugador.position = destino;

        if (camaraSeguimiento != null)
        {
            if (limiteInferiorDestino != null &&
                limiteSuperiorDestino != null)
            {
                camaraSeguimiento.CambiarLimites(
                    limiteInferiorDestino,
                    limiteSuperiorDestino
                );
            }
            else
            {
                camaraSeguimiento.RecolocarEnJugador();
            }
        }
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
}