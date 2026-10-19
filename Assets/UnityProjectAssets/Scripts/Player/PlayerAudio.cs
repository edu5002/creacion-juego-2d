using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(PlayerCombat))]
public class PlayerAudio : MonoBehaviour
{
    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoAtaque;
    [SerializeField] private AudioClip sonidoSalto;
    [SerializeField] private AudioClip[] sonidosPasos;
    [SerializeField] private AudioClip sonidoDanio;
    [SerializeField] private AudioClip sonidoMuerte;

    [Header("Volumen")]
    [Range(0f, 1f)]
    [SerializeField] private float volumenAtaque = 0.7f;

    [Range(0f, 1f)]
    [SerializeField] private float volumenSalto = 0.6f;

    [Range(0f, 1f)]
    [SerializeField] private float volumenPasos = 0.35f;

    [Range(0f, 1f)]
    [SerializeField] private float volumenDanio = 0.7f;

    [Range(0f, 1f)]
    [SerializeField] private float volumenMuerte = 0.8f;

    private AudioSource fuente;
    private PlayerCombat combate;
    private PlayerController movimiento;
    private Animator animator;
    private int ultimoPaso = -1;

    private void Awake()
    {
        fuente = GetComponent<AudioSource>();
        combate = GetComponent<PlayerCombat>();
        movimiento = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();

        fuente.playOnAwake = false;
        fuente.loop = false;
        fuente.spatialBlend = 0f;
    }

    public void ReproducirAtaque()
    {
        if (Time.timeScale == 0f ||
            !combate.enabled ||
            !combate.EstaAtacando ||
            animator.GetBool("muerto") ||
            animator.GetBool("recibeDanio"))
            return;

        Reproducir(sonidoAtaque, volumenAtaque);
    }

    public void ReproducirSalto()
    {
        if (Time.timeScale == 0f ||
            animator.GetBool("muerto") ||
            animator.GetBool("recibeDanio"))
            return;

        Reproducir(sonidoSalto, volumenSalto);
    }

    public void ReproducirPaso()
    {
        if (Time.timeScale == 0f ||
            !movimiento.enabled ||
            !movimiento.EnSuelo ||
            combate.EstaAtacando ||
            animator.GetBool("muerto") ||
            animator.GetBool("recibeDanio") ||
            animator.GetFloat("Speed") <= 0.1f)
            return;

        if (sonidosPasos == null || sonidosPasos.Length == 0)
            return;

        int indice;

        if (sonidosPasos.Length == 1)
        {
            indice = 0;
        }
        else if (ultimoPaso < 0)
        {
            indice = Random.Range(0, sonidosPasos.Length);
        }
        else
        {
            // Elige una variante diferente al paso anterior.
            indice = (ultimoPaso +
                      Random.Range(1, sonidosPasos.Length))
                      % sonidosPasos.Length;
        }

        ultimoPaso = indice;
        Reproducir(sonidosPasos[indice], volumenPasos);
    }

    public void ReproducirDanio()
    {
        if (Time.timeScale == 0f ||
            animator.GetBool("muerto"))
            return;

        Reproducir(sonidoDanio, volumenDanio);
    }

    public void ReproducirMuerte()
    {
        if (Time.timeScale == 0f)
            return;

        fuente.Stop();
        Reproducir(sonidoMuerte, volumenMuerte);
    }

    private void Reproducir(AudioClip clip, float volumen)
    {
        if (clip != null)
            fuente.PlayOneShot(clip, volumen);
    }
}