using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AjustesAudio : MonoBehaviour
{
    [Header("Referencias")]
    public AudioMixer mezclador;
    public Slider sliderMusica;
    public Slider sliderEfectos;

    private const string ClaveMusica = "Audio_VolumenMusica";
    private const string ClaveEfectos = "Audio_VolumenEfectos";

    private bool guardadoPendiente;
    private float momentoGuardar;

    private void Start()
    {
        if (mezclador == null ||
            sliderMusica == null ||
            sliderEfectos == null)
        {
            Debug.LogWarning(
                "Faltan referencias en AjustesAudio.", this
            );

            enabled = false;
            return;
        }

        PrepararSlider(sliderMusica);
        PrepararSlider(sliderEfectos);

        float musica = Mathf.Clamp01(
            PlayerPrefs.GetFloat(ClaveMusica, 1f)
        );

        float efectos = Mathf.Clamp01(
            PlayerPrefs.GetFloat(ClaveEfectos, 1f)
        );

        sliderMusica.SetValueWithoutNotify(musica);
        sliderEfectos.SetValueWithoutNotify(efectos);

        AplicarVolumen("VolumenMusica", musica);
        AplicarVolumen("VolumenEfectos", efectos);

        sliderMusica.onValueChanged.AddListener(CambiarMusica);
        sliderEfectos.onValueChanged.AddListener(CambiarEfectos);
    }

    private void PrepararSlider(Slider slider)
    {
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.wholeNumbers = false;
    }

    private void CambiarMusica(float valor)
    {
        AplicarVolumen("VolumenMusica", valor);
        PlayerPrefs.SetFloat(ClaveMusica, valor);
        ProgramarGuardado();
    }

    private void CambiarEfectos(float valor)
    {
        AplicarVolumen("VolumenEfectos", valor);
        PlayerPrefs.SetFloat(ClaveEfectos, valor);
        ProgramarGuardado();
    }

    private void AplicarVolumen(string parametro, float valor)
    {
        float decibelios = valor <= 0f
            ? -80f
            : Mathf.Log10(valor) * 20f;

        if (!mezclador.SetFloat(parametro, decibelios))
        {
            Debug.LogWarning(
                "No se encontró el parámetro: " + parametro,
                this
            );
        }
    }

    private void ProgramarGuardado()
    {
        guardadoPendiente = true;
        momentoGuardar = Time.unscaledTime + 0.5f;
    }

    private void Update()
    {
        if (guardadoPendiente &&
            Time.unscaledTime >= momentoGuardar)
        {
            Guardar();
        }
    }

    private void Guardar()
    {
        if (!guardadoPendiente)
            return;

        PlayerPrefs.Save();
        guardadoPendiente = false;
    }

    private void OnApplicationPause(bool pausada)
    {
        if (pausada)
            Guardar();
    }

    private void OnDisable()
    {
        Guardar();
    }

    private void OnDestroy()
    {
        if (sliderMusica != null)
            sliderMusica.onValueChanged.RemoveListener(CambiarMusica);

        if (sliderEfectos != null)
            sliderEfectos.onValueChanged.RemoveListener(CambiarEfectos);
    }
}