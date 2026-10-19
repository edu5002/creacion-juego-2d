using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemyAudio : MonoBehaviour
{
    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoDanio;
    [SerializeField] private AudioClip sonidoMuerte;

    [Header("Volumen")]
    [Range(0f, 1f)]
    [SerializeField] private float volumenDanio = 0.6f;

    [Range(0f, 1f)]
    [SerializeField] private float volumenMuerte = 0.7f;

    private AudioSource fuente;
    private bool muerteReproducida;

    private void Awake()
    {
        fuente = GetComponent<AudioSource>();

        fuente.playOnAwake = false;
        fuente.loop = false;
        fuente.spatialBlend = 0f;
    }

    public void ReproducirDanio()
    {
        if (muerteReproducida || sonidoDanio == null)
            return;

        fuente.PlayOneShot(sonidoDanio, volumenDanio);
    }

    public void ReproducirMuerte()
    {
        if (muerteReproducida)
            return;

        muerteReproducida = true;
        fuente.Stop();

        if (sonidoMuerte == null)
            return;

        // Objeto independiente para que el audio termine
        // aunque el enemigo desaparezca.
        GameObject objetoSonido =
            new GameObject("SonidoMuerteEnemigo");

        objetoSonido.transform.position = transform.position;

        AudioSource audioMuerte =
            objetoSonido.AddComponent<AudioSource>();

        audioMuerte.playOnAwake = false;
        audioMuerte.loop = false;
        audioMuerte.spatialBlend = 0f;
        audioMuerte.volume = fuente.volume * volumenMuerte;
        audioMuerte.outputAudioMixerGroup =
            fuente.outputAudioMixerGroup;

        audioMuerte.clip = sonidoMuerte;
        audioMuerte.Play();

        Destroy(objetoSonido, sonidoMuerte.length + 0.1f);
    }
}