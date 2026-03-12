using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OpcionesManager : MonoBehaviour
{
    [Header("Sliders de Volumen")]
    [SerializeField] private Slider sliderVolumenMusica;
    [SerializeField] private Slider sliderVolumenDialogos;

    [Header("Textos de Volumen")]
    [SerializeField] private TextMeshProUGUI textoVolumenMusica;
    [SerializeField] private TextMeshProUGUI textoVolumenDialogos;

    [Header("Botones de Voz")]
    [SerializeField] private Button botonVoz1;
    [SerializeField] private Button botonVoz2;
    [SerializeField] private Image indicadorVoz1;
    [SerializeField] private Image indicadorVoz2;

    [Header("Referencias")]
    [SerializeField] private Menu menuManager;

    void OnEnable()
    {
        CargarConfiguracion();
    }

    void Start()
    {
        // Configurar listeners
        if (sliderVolumenMusica != null)
        {
            sliderVolumenMusica.onValueChanged.AddListener(CambiarVolumenMusica);
        }

        if (sliderVolumenDialogos != null)
        {
            sliderVolumenDialogos.onValueChanged.AddListener(CambiarVolumenDialogos);
        }

        if (botonVoz1 != null)
        {
            botonVoz1.onClick.AddListener(() => CambiarVoz(0));
        }

        if (botonVoz2 != null)
        {
            botonVoz2.onClick.AddListener(() => CambiarVoz(1));
        }
    }

    void CargarConfiguracion()
    {
        // Cargar volúmenes
        if (sliderVolumenMusica != null)
        {
            sliderVolumenMusica.value = ConfiguracionAudio.VolumenMusica;
        }

        if (sliderVolumenDialogos != null)
        {
            sliderVolumenDialogos.value = ConfiguracionAudio.VolumenDialogos;
        }

        ActualizarTextosVolumen();
        ActualizarIndicadoresVoz();
    }

    void CambiarVolumenMusica(float valor)
    {
        ConfiguracionAudio.VolumenMusica = valor;
        ActualizarTextosVolumen();
        
        // Aplicar a la música del menú si está sonando
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audio in audioSources)
        {
            if (audio.CompareTag("MusicaMenu"))  // Asegúrate de etiquetar tu AudioSource de música
            {
                audio.volume = valor;
            }
        }
    }

    void CambiarVolumenDialogos(float valor)
    {
        ConfiguracionAudio.VolumenDialogos = valor;
        ActualizarTextosVolumen();
    }

    void CambiarVoz(int tipoVoz)
    {
        ConfiguracionAudio.TipoVozJugador = tipoVoz;
        ActualizarIndicadoresVoz();
    }

    void ActualizarTextosVolumen()
    {
        if (textoVolumenMusica != null)
        {
            textoVolumenMusica.text = $"{Mathf.RoundToInt(ConfiguracionAudio.VolumenMusica * 100)}%";
        }

        if (textoVolumenDialogos != null)
        {
            textoVolumenDialogos.text = $"{Mathf.RoundToInt(ConfiguracionAudio.VolumenDialogos * 100)}%";
        }
    }

    void ActualizarIndicadoresVoz()
    {
        if (indicadorVoz1 != null)
        {
            indicadorVoz1.color = ConfiguracionAudio.TipoVozJugador == 0 ? Color.green : Color.gray;
        }

        if (indicadorVoz2 != null)
        {
            indicadorVoz2.color = ConfiguracionAudio.TipoVozJugador == 1 ? Color.green : Color.gray;
        }
    }

    public void VolverAlMenu()
    {
        if (menuManager != null)
        {
            menuManager.VolverAlMenu();
        }
    }
}