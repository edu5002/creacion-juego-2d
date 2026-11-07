using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPrincipalUI : MonoBehaviour
{
    [Header("Escenas")]
    public string primerNivel;

    [Header("Botones")]
    public Button botonContinuar;

    [Header("Ajustes")]
    public GameObject panelAjustes;
    public Button botonNuevoJuego;
    public Button botonAjustes;
    public Button botonSalir;

    private bool cargando;

    private void Awake()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        if (panelAjustes != null)
            panelAjustes.SetActive(false);
    }

    private void Start()
    {
        if (botonContinuar != null)
        {
            botonContinuar.interactable =
                GuardadoPartida.HayPartida &&
                EscenaDisponible(GuardadoPartida.NivelGuardado);
        }
    }

    private bool EscenaDisponible(string nombre)
    {
        return !string.IsNullOrWhiteSpace(nombre) &&
               nombre != gameObject.scene.name &&
               Application.CanStreamedLevelBeLoaded(nombre);
    }

    public void NuevoJuego()
    {
        if (cargando)
            return;

        // Comprueba el destino antes de borrar la partida.
        if (!EscenaDisponible(primerNivel))
        {
            Debug.LogWarning(
                "Revisa el nombre del primer nivel y la Scene List.",
                this
            );
            return;
        }

        GuardadoPartida.BorrarPartida();
        GuardadoPartida.GuardarInicioNivel(primerNivel);

        CargarNivel(primerNivel);
    }

    public void Continuar()
    {
        if (cargando || !GuardadoPartida.HayPartida)
            return;

        string destino = GuardadoPartida.NivelGuardado;

        if (!EscenaDisponible(destino))
        {
            Debug.LogWarning(
                $"No se puede cargar la escena guardada: {destino}",
                this
            );
            return;
        }

        CargarNivel(destino);
    }

    private void CargarNivel(string nombre)
    {
        cargando = true;
        Time.timeScale = 1f;
        AudioListener.pause = false;

        SceneManager.LoadSceneAsync(nombre);
    }

    public void AbrirAjustes()
    {
        if (cargando || panelAjustes == null)
            return;

        panelAjustes.SetActive(true);
        panelAjustes.transform.SetAsLastSibling();

        CambiarBotonesMenu(false);
    }

    public void CerrarAjustes()
    {
        if (panelAjustes != null)
            panelAjustes.SetActive(false);

        CambiarBotonesMenu(true);
    }

    private void CambiarBotonesMenu(bool activos)
    {
        if (botonNuevoJuego != null)
            botonNuevoJuego.interactable = activos;

        if (botonAjustes != null)
            botonAjustes.interactable = activos;

        if (botonSalir != null)
            botonSalir.interactable = activos;

        if (botonContinuar != null)
        {
            botonContinuar.interactable =
                activos &&
                GuardadoPartida.HayPartida &&
                EscenaDisponible(GuardadoPartida.NivelGuardado);
        }
    }

    public void Salir()
    {
        Application.Quit();
        Debug.Log("Salir funciona en la versión compilada.");
    }
}