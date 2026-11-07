using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class AudioBotonesUI : MonoBehaviour
{
    public static AudioBotonesUI Instancia { get; private set; }

    public AudioClip sonidoSeleccion;
    public AudioClip sonidoClic;
    public AudioMixerGroup grupoEfectos;

    [Range(0f, 1f)]
    public float volumenSeleccion = 0.35f;

    [Range(0f, 1f)]
    public float volumenClic = 0.6f;

    private AudioSource fuente;

    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        Instancia = this;
        DontDestroyOnLoad(gameObject);

        fuente = GetComponent<AudioSource>();
        fuente.playOnAwake = false;
        fuente.loop = false;
        fuente.spatialBlend = 0f;
        fuente.volume = 1f;
        fuente.outputAudioMixerGroup = grupoEfectos;
        fuente.ignoreListenerPause = true;
    }

    public void ReproducirSeleccion()
    {
        if (sonidoSeleccion != null)
            fuente.PlayOneShot(sonidoSeleccion, volumenSeleccion);
    }

    public void ReproducirClic()
    {
        if (sonidoClic != null)
            fuente.PlayOneShot(sonidoClic, volumenClic);
    }

    private void OnDestroy()
    {
        if (Instancia == this)
            Instancia = null;
    }
}