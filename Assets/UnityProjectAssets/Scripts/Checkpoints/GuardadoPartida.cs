using UnityEngine;
using System;
using System.Collections.Generic;
public static class GuardadoPartida
{
    private const string ClaveNivel = "UltimaChispa_Nivel";
    private const string ClaveCheckpoint = "UltimaChispa_Checkpoint";

    public static bool HayPartida =>
        PlayerPrefs.HasKey(ClaveNivel) &&
        PlayerPrefs.HasKey(ClaveCheckpoint);

    public static string NivelGuardado =>
        PlayerPrefs.GetString(ClaveNivel, "");

    public static int ObtenerCheckpoint(string nombreNivel)
    {
        if (!HayPartida || NivelGuardado != nombreNivel)
            return 0;

        return PlayerPrefs.GetInt(ClaveCheckpoint, 0);
    }

    public static void Guardar(string nombreNivel, int orden)
    {
        if (string.IsNullOrEmpty(nombreNivel) || orden < 1)
            return;

        PlayerPrefs.SetString(ClaveNivel, nombreNivel);
        PlayerPrefs.SetInt(ClaveCheckpoint, orden);
        PlayerPrefs.Save();

        Debug.Log(
            $"Partida guardada: {nombreNivel}, checkpoint {orden}"
        );
    }

    public static void GuardarInicioNivel(string nombreNivel)
    {
        if (string.IsNullOrWhiteSpace(nombreNivel))
            return;

        PlayerPrefs.SetString(ClaveNivel, nombreNivel);
        PlayerPrefs.SetInt(ClaveCheckpoint, 0);
        PlayerPrefs.Save();

        Debug.Log($"Partida guardada: inicio de {nombreNivel}");
    }

    private const string ClaveCristales = "UltimaChispa_Cristales";

    [Serializable]
    private class CristalGuardado
    {
        public string nivel;
        public string id;
    }

    [Serializable]
    private class DatosCristales
    {
        public List<CristalGuardado> recogidos = new List<CristalGuardado>();
    }

    private static DatosCristales LeerCristales()
    {
        string json = PlayerPrefs.GetString(ClaveCristales, "");

        if (string.IsNullOrEmpty(json))
            return new DatosCristales();

        DatosCristales datos = JsonUtility.FromJson<DatosCristales>(json);

        if (datos == null)
            datos = new DatosCristales();

        if (datos.recogidos == null)
            datos.recogidos = new List<CristalGuardado>();

        return datos;
    }

    public static bool CristalRecogido(string nivel, string id)
    {
        DatosCristales datos = LeerCristales();

        return datos.recogidos.Exists(
            cristal => cristal.nivel == nivel && cristal.id == id
        );
    }

    public static bool GuardarCristal(string nivel, string id)
    {
        if (string.IsNullOrWhiteSpace(nivel) ||
            string.IsNullOrWhiteSpace(id))
            return false;

        DatosCristales datos = LeerCristales();

        bool repetido = datos.recogidos.Exists(
            cristal => cristal.nivel == nivel && cristal.id == id
        );

        if (repetido)
            return false;

        datos.recogidos.Add(new CristalGuardado
        {
            nivel = nivel,
            id = id
        });

        PlayerPrefs.SetString(
            ClaveCristales,
            JsonUtility.ToJson(datos)
        );

        // Permite continuar incluso si aún no tocaste un checkpoint.
        if (!HayPartida)
        {
            PlayerPrefs.SetString(ClaveNivel, nivel);
            PlayerPrefs.SetInt(ClaveCheckpoint, 0);
        }

        PlayerPrefs.Save();
        return true;
    }

    public static int ObtenerCristalesNivel(string nivel)
    {
        return LeerCristales().recogidos.FindAll(
            cristal => cristal.nivel == nivel
        ).Count;
    }

    public static int ObtenerTotalCristales()
    {
        return LeerCristales().recogidos.Count;
    }

    public static void BorrarPartida()
    {
        PlayerPrefs.DeleteKey(ClaveNivel);
        PlayerPrefs.DeleteKey(ClaveCheckpoint);
        PlayerPrefs.DeleteKey(ClaveCristales);
        PlayerPrefs.Save();
    }
}