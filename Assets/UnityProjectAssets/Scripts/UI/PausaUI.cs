using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PausaUI : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject panelPausa;
    public PlayerVida jugador;
    public Button botonContinuar;
    public Button botonMenu;

    [Header("Ajustes")]
    public GameObject panelAjustes;
    public Button botonAjustes;
    public Button botonVolverAjustes;

    [Header("Menú principal")]
    public string nombreEscenaMenu = "";

    public bool EstaPausado { get; private set; }

    private bool cursorVisibleAnterior;
    private CursorLockMode bloqueoCursorAnterior;
    private float escalaTiempoAnterior = 1f;

    private void Awake()
    {
        if (panelPausa != null)
            panelPausa.SetActive(false);

        if (panelAjustes != null)
            panelAjustes.SetActive(false);
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
            return;

        // No abre pausa durante la muerte o Game Over.
        if (jugador == null || jugador.EstaMuerto)
            return;

 if (EstaPausado)
{
    if (panelAjustes != null && panelAjustes.activeSelf)
        CerrarAjustes();
    else
        Continuar();
}
else
{
    Pausar();
}
    }

    public void Pausar()
    {
        if (EstaPausado || panelPausa == null ||
            jugador == null || jugador.EstaMuerto)
            return;

        // No interviene si otro sistema ya pausó el juego.
        if (Time.timeScale == 0f)
            return;

        escalaTiempoAnterior = Time.timeScale;
        cursorVisibleAnterior = Cursor.visible;
        bloqueoCursorAnterior = Cursor.lockState;

        EstaPausado = true;
        Time.timeScale = 0f;

        panelPausa.SetActive(true);
        panelPausa.transform.SetAsLastSibling();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (botonMenu != null)
            botonMenu.interactable = MenuDisponible();

        if (EventSystem.current != null &&
            botonContinuar != null)
        {
            EventSystem.current.SetSelectedGameObject(
                botonContinuar.gameObject
            );
        }
    }

    public void Continuar()
    {
        if (!EstaPausado)
            return;

        EstaPausado = false;
        panelPausa.SetActive(false);
        if (panelAjustes != null)
            panelAjustes.SetActive(false);

        Time.timeScale = escalaTiempoAnterior;
        Cursor.visible = cursorVisibleAnterior;
        Cursor.lockState = bloqueoCursorAnterior;

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

    public void AbrirAjustes()
{
    if (!EstaPausado || panelAjustes == null)
        return;

    if (EventSystem.current != null)
        EventSystem.current.SetSelectedGameObject(null);

    panelPausa.SetActive(false);
    panelAjustes.SetActive(true);
    panelAjustes.transform.SetAsLastSibling();

    if (EventSystem.current != null &&
        botonVolverAjustes != null)
    {
        EventSystem.current.SetSelectedGameObject(
            botonVolverAjustes.gameObject
        );
    }
}

public void CerrarAjustes()
{
    if (!EstaPausado || panelAjustes == null)
        return;

    if (EventSystem.current != null)
        EventSystem.current.SetSelectedGameObject(null);

    panelAjustes.SetActive(false);
    panelPausa.SetActive(true);
    panelPausa.transform.SetAsLastSibling();

    if (EventSystem.current != null &&
        botonAjustes != null)
    {
        EventSystem.current.SetSelectedGameObject(
            botonAjustes.gameObject
        );
    }
}

    public void IrAlMenu()
    {
        if (!EstaPausado || !MenuDisponible())
            return;

        EstaPausado = false;
        panelPausa.SetActive(false);
        if (panelAjustes != null)
            panelAjustes.SetActive(false);

        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SceneManager.LoadScene(nombreEscenaMenu);
    }

    private bool MenuDisponible()
    {
        return !string.IsNullOrWhiteSpace(nombreEscenaMenu) &&
               Application.CanStreamedLevelBeLoaded(nombreEscenaMenu);
    }
}