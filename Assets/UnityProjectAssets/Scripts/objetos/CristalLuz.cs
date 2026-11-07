using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(Collider2D))]
public class CristalLuz : MonoBehaviour
{
    [Header("Contador del nivel")]
    public CristalesNivel controlCristales;

    [Header("Identificador único dentro del nivel")]
    public string idCristal;

    [Header("Flotación")]
    public Transform visual;

    [Min(0f)]
    public float alturaFlotacion = 0.12f;

    [Min(0f)]
    public float ciclosPorSegundo = 0.6f;

    [Header("Sonido")]
    public AudioClip sonidoRecoger;
    public AudioMixerGroup grupoEfectos;

    [Range(0f, 1f)]
    public float volumen = 0.6f;

    [Header("Efecto al recoger")]
    public ParticleSystem efectoRecoger;

    private Vector3 posicionVisualInicial;
    private float tiempoFlotacion;
    private bool recogido;

    private void Start()
    {
        if (visual != null && visual != transform)
            posicionVisualInicial = visual.localPosition;

        if (controlCristales == null)
        {
            Debug.LogWarning(
                "Asigna el ControlCristales del nivel a este cristal.",
                this
            );
        }
        if (string.IsNullOrWhiteSpace(idCristal))
        {
            Debug.LogError(
                "Asigna un identificador a este cristal.",
                this
            );
        }
        else if (GuardadoPartida.CristalRecogido(
            gameObject.scene.name,
            idCristal))
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (visual == null || visual == transform || recogido)
            return;

        tiempoFlotacion += Time.deltaTime;

        float desplazamiento = Mathf.Sin(
            tiempoFlotacion * ciclosPorSegundo * Mathf.PI * 2f
        ) * alturaFlotacion;

        visual.localPosition =
            posicionVisualInicial + Vector3.up * desplazamiento;
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        IntentarRecoger(otro);
    }

    private void OnTriggerStay2D(Collider2D otro)
    {
        IntentarRecoger(otro);
    }

    private void IntentarRecoger(Collider2D otro)
    {
        if (!isActiveAndEnabled || recogido || Time.timeScale == 0f)
            return;

        PlayerVida jugador = otro.GetComponentInParent<PlayerVida>();

        if (jugador == null || jugador.EstaMuerto)
            return;

        if (controlCristales == null)
            return;

        if (string.IsNullOrWhiteSpace(idCristal))
            return;

        if (controlCristales.gameObject.scene != gameObject.scene)
            return;

        bool registrado = controlCristales.RegistrarCristal(idCristal);

        recogido = true;

        if (!registrado)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
            return;
        }

        if (efectoRecoger != null)
        {
            Vector3 posicionEfecto =
                visual != null ? visual.position : transform.position;

            Instantiate(
                efectoRecoger,
                posicionEfecto,
                Quaternion.identity
            );
        }

        ReproducirSonido();

        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    private void ReproducirSonido()
    {
        if (sonidoRecoger == null)
            return;

        GameObject objetoAudio = new GameObject("SonidoCristalLuz");
        objetoAudio.transform.position = transform.position;

        AudioSource fuente = objetoAudio.AddComponent<AudioSource>();

        fuente.playOnAwake = false;
        fuente.loop = false;
        fuente.spatialBlend = 0f;
        fuente.outputAudioMixerGroup = grupoEfectos;
        fuente.volume = volumen;
        fuente.clip = sonidoRecoger;

        fuente.Play();

        Destroy(objetoAudio, sonidoRecoger.length + 0.1f);
    }
}