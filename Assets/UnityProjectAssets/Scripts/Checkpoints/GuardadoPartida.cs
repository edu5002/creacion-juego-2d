using UnityEngine;

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

    public static void BorrarPartida()
    {
        PlayerPrefs.DeleteKey(ClaveNivel);
        PlayerPrefs.DeleteKey(ClaveCheckpoint);
        PlayerPrefs.Save();
    }
}