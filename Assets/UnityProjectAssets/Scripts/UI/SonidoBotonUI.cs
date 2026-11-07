using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SonidoBotonUI : MonoBehaviour, IPointerEnterHandler
{
    private Button boton;

    private void Awake()
    {
        boton = GetComponent<Button>();
        boton.onClick.AddListener(ReproducirClic);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isActiveAndEnabled ||
            !boton.IsActive() ||
            !boton.IsInteractable())
            return;

        if (AudioBotonesUI.Instancia != null)
            AudioBotonesUI.Instancia.ReproducirSeleccion();
    }

    private void ReproducirClic()
    {
        if (AudioBotonesUI.Instancia != null)
            AudioBotonesUI.Instancia.ReproducirClic();
    }

    private void OnDestroy()
    {
        if (boton != null)
            boton.onClick.RemoveListener(ReproducirClic);
    }
}