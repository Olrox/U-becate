using UnityEngine;

public class ConfiguracionAudio : MonoBehaviour
{
    private static ConfiguracionAudio instance;

    // Valores por defecto
    private static float volumenMusica = 0.7f;
    private static float volumenDialogos = 1f;
    private static int tipoVozJugador = 0;  // 0 = Voz 1, 1 = Voz 2

    public static float VolumenMusica
    {
        get { return volumenMusica; }
        set
        {
            volumenMusica = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat("VolumenMusica", volumenMusica);
            PlayerPrefs.Save();
        }
    }

    public static float VolumenDialogos
    {
        get { return volumenDialogos; }
        set
        {
            volumenDialogos = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat("VolumenDialogos", volumenDialogos);
            PlayerPrefs.Save();
        }
    }

    public static int TipoVozJugador
    {
        get { return tipoVozJugador; }
        set
        {
            tipoVozJugador = Mathf.Clamp(value, 0, 1);
            PlayerPrefs.SetInt("TipoVozJugador", tipoVozJugador);
            PlayerPrefs.Save();
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            CargarConfiguracion();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void CargarConfiguracion()
    {
        volumenMusica = PlayerPrefs.GetFloat("VolumenMusica", 0.7f);
        volumenDialogos = PlayerPrefs.GetFloat("VolumenDialogos", 1f);
        tipoVozJugador = PlayerPrefs.GetInt("TipoVozJugador", 0);
    }
}