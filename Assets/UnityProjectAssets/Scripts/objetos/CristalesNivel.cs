using TMPro;
using UnityEngine;

public class CristalesNivel : MonoBehaviour
{
    [Header("Objetivo del nivel")]
    [Min(1)]
    public int cristalesNecesarios = 5;

    [Header("Interfaz")]
    public TMP_Text textoCristales;
    public TMP_Text textoTotal;

    private string NombreNivel => gameObject.scene.name;

    public int CristalesRecogidos =>
        GuardadoPartida.ObtenerCristalesNivel(NombreNivel);

    public bool ObjetivoCompletado =>
        CristalesRecogidos >= cristalesNecesarios;

    private void Start()
    {
        ActualizarTexto();
    }

    public bool RegistrarCristal(string idCristal)
    {
        bool registrado = GuardadoPartida.GuardarCristal(
            NombreNivel,
            idCristal
        );

        ActualizarTexto();
        return registrado;
    }

    private void ActualizarTexto()
    {
        if (textoCristales != null)
        {
            textoCristales.text =
                $"Cristales: {CristalesRecogidos}/{cristalesNecesarios}";
        }

        if (textoTotal != null)
        {
            textoTotal.text =
                $"Total recogido: {GuardadoPartida.ObtenerTotalCristales()}";
        }
    }
}