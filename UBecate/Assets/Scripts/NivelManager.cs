using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class NivelImagenesManager : MonoBehaviour
{
    [Header("Imágenes del Nivel")]
    public GameObject imagen1;
    public GameObject imagen2;
    public GameObject imagen3;
    public GameObject imagen4;
    public GameObject imagen5;

    [Header("Paneles de Diálogo")]
    public GameObject panelDialogo;
    public GameObject panelDialogoArriba;
    public TextMeshProUGUI textoDialogo;
    public TextMeshProUGUI textoDialogoArriba;
    public TextMeshProUGUI nombreNarradorText;
    public TextMeshProUGUI nombreNarradorTextArriba;

    [Header("Textos Automáticos (TMP)")]
    public TextMeshProUGUI textoAutomatico1;
    [TextArea(3, 5)]
    public string contenidoTextoAutomatico1;
    
    [Header("Textos Automáticos Imagen 3 (8 textos)")]
    public TextMeshProUGUI[] textosAutomaticos3;
    [TextArea(3, 5)]
    public string[] contenidosTextosAutomaticos3 = new string[8];
    
    [Header("Textos Automáticos Imagen 2 (Segunda vez)")]
    public TextMeshProUGUI textoAutomatico2_1;
    [TextArea(3, 5)]
    public string contenidoTextoAutomatico2_1;
    
    public TextMeshProUGUI textoAutomatico2_2;
    [TextArea(3, 5)]
    public string contenidoTextoAutomatico2_2;

    [Header("Botones de Avance")]
    public Button botonImagen1;
    public Button botonImagen2_A;
    public Button botonImagen2_B;
    public Button botonImagen3;
    public Button botonImagen4;
    public Button botonImagen5;

    [Header("Panel de Transición")]
    public GameObject panelTransicion;
    public TextMeshProUGUI textoTransicion;

    [Header("Panel Final")]
    public GameObject panelFinal;
    public TextMeshProUGUI textoFinal;

    [Header("Configuración")]
    public float velocidadEscritura = 0.05f;
    public float tiempoTransicion = 3f;
    public float tiempoAntesSiguienteNivel = 3f;

    [Header("Escena Siguiente")]
    public string siguienteNivel = "NivelSiguiente";

    // ─────────────────────────────────────────────────────────────
    [Header("Audio - Diálogos de Bequín")]
    public AudioSource audioSourceBequín;         // AudioSource dedicado a Bequín
    public AudioClip[] audioClipsBequín;          // Clips en orden: [0]=dialogo1, [1]=dialogo2...
    private int indiceAudioBequín = 0;            // Lleva la cuenta de qué clip reproducir

    [Header("Audio - Textos Automáticos")]
    public AudioSource audioSourceTextoAuto;    // AudioSource dedicado a textos automáticos
    public AudioClip audioTextoAutomatico;      // El único audio para todos los textos
    // ─────────────────────────────────────────────────────────────

    private bool esperandoClick = false;
    private bool clickRecibido = false;
    private bool primeraVezImagen2 = true;

    [System.Serializable]
    private class DialogoNarrador
    {
        public string nombreHablante;
        public string texto;

        public DialogoNarrador(string nombre, string texto)
        {
            this.nombreHablante = nombre;
            this.texto = texto;
        }
    }

    void Start()
    {
        InicializarEstado();
        StartCoroutine(SecuenciaNivel());
    }

    void Update()
    {
        if (esperandoClick && Input.GetMouseButtonDown(0))
        {
            clickRecibido = true;
            esperandoClick = false;
        }
    }

    void InicializarEstado()
    {
        if (imagen1 != null) imagen1.SetActive(false);
        if (imagen2 != null) imagen2.SetActive(false);
        if (imagen3 != null) imagen3.SetActive(false);
        if (imagen4 != null) imagen4.SetActive(false);
        if (imagen5 != null) imagen5.SetActive(false);

        if (panelDialogo != null) panelDialogo.SetActive(false);
        if (panelDialogoArriba != null) panelDialogoArriba.SetActive(false);
        if (panelTransicion != null) panelTransicion.SetActive(false);
        if (panelFinal != null) panelFinal.SetActive(false);

        if (textoAutomatico1 != null) textoAutomatico1.gameObject.SetActive(false);
        if (textoAutomatico2_1 != null) textoAutomatico2_1.gameObject.SetActive(false);
        if (textoAutomatico2_2 != null) textoAutomatico2_2.gameObject.SetActive(false);
        
        foreach (var texto in textosAutomaticos3)
            if (texto != null) texto.gameObject.SetActive(false);

        if (botonImagen1 != null) botonImagen1.gameObject.SetActive(false);
        if (botonImagen2_A != null) botonImagen2_A.gameObject.SetActive(false);
        if (botonImagen2_B != null) botonImagen2_B.gameObject.SetActive(false);
        if (botonImagen3 != null) botonImagen3.gameObject.SetActive(false);
        if (botonImagen4 != null) botonImagen4.gameObject.SetActive(false);
        if (botonImagen5 != null) botonImagen5.gameObject.SetActive(false);
    }

    IEnumerator SecuenciaNivel()
    {
        yield return StartCoroutine(SecuenciaImagen1());
        yield return StartCoroutine(SecuenciaImagen2PrimeraVez());
        yield return StartCoroutine(SecuenciaImagen3());
        yield return StartCoroutine(SecuenciaImagen2SegundaVez());
        yield return StartCoroutine(SecuenciaImagen4());
        yield return StartCoroutine(SecuenciaImagen5());
    }

    IEnumerator SecuenciaImagen1()
    {
        imagen1.SetActive(true);

        DialogoNarrador[] dialogos = new DialogoNarrador[]
        {
            new DialogoNarrador("Yo", "Leyendo la convocatoria descubrí que debo registrar la solicitud de la beca a través del Sistema Integra.")
        };

        yield return StartCoroutine(MostrarDialogos(dialogos, panelDialogo, textoDialogo, nombreNarradorText));

        textoAutomatico1.gameObject.SetActive(true);
        IniciarAudioTextoAuto(contenidoTextoAutomatico1);
        yield return StartCoroutine(EscribirTextoAutomatico(textoAutomatico1, contenidoTextoAutomatico1));

        botonImagen1.gameObject.SetActive(true);
        botonImagen1.onClick.AddListener(() => CambiarImagen(imagen1, imagen2));

        yield return new WaitUntil(() => !imagen1.activeSelf);
    }

    IEnumerator SecuenciaImagen2PrimeraVez()
    {
        DialogoNarrador[] dialogos = new DialogoNarrador[]
        {
            new DialogoNarrador("Bequín", "Si es tu primera vez usando este portal deberás registrarte primero.")
        };

        yield return StartCoroutine(MostrarDialogos(dialogos, panelDialogo, textoDialogo, nombreNarradorText));

        botonImagen2_A.gameObject.SetActive(true);
        botonImagen2_A.onClick.RemoveAllListeners();
        botonImagen2_A.onClick.AddListener(() => CambiarImagen(imagen2, imagen3));

        yield return new WaitUntil(() => !imagen2.activeSelf);
    }

    IEnumerator SecuenciaImagen3()
    {
        botonImagen2_A.gameObject.SetActive(false);

        DialogoNarrador[] dialogos = new DialogoNarrador[]
        {
            new DialogoNarrador("Bequín", "Es hora de crear tu perfil en INTEGRA. Es importante pensar que, cuando llenes el formulario, tendrás que validar que tus datos sean correctos."),
            new DialogoNarrador("Bequín", "Es importante que en caso de corrección de nombre y/o género, validez que esos datos se encuentren en el DGAE, antes de registrarte con la nueva información. Una vez terminado, picar a registrar."),
            new DialogoNarrador("Bequín", "Para esta parte la llenaré por ti, pero recuerda que fuera de este juego lo deberás de llenar con tu información real."),
            new DialogoNarrador("Yo", "Espera, ¿Estamos en un juego?")
        };

        yield return StartCoroutine(MostrarDialogos(dialogos, panelDialogo, textoDialogo, nombreNarradorText));

        for (int i = 0; i < textosAutomaticos3.Length && i < 8; i++)
        {
            if (textosAutomaticos3[i] != null && i < contenidosTextosAutomaticos3.Length)
            {
                textosAutomaticos3[i].gameObject.SetActive(true);
                IniciarAudioTextoAuto(contenidosTextosAutomaticos3[i]);
                yield return StartCoroutine(EscribirTextoAutomatico(
                    textosAutomaticos3[i],
                    contenidosTextosAutomaticos3[i]
                ));
                yield return new WaitForSeconds(0.5f);
            }
        }

        botonImagen3.gameObject.SetActive(true);
        botonImagen3.onClick.RemoveAllListeners();
        botonImagen3.onClick.AddListener(() => CambiarImagen(imagen3, imagen2));

        yield return new WaitUntil(() => !imagen3.activeSelf);
        primeraVezImagen2 = false;
    }

    IEnumerator SecuenciaImagen2SegundaVez()
    {
        DialogoNarrador[] dialogosAntes = new DialogoNarrador[]
        {
            new DialogoNarrador("Bequín", "Ahora que ya estamos dentro del portal y tenemos nuestro usuario, procederemos a iniciar sesión."),
            new DialogoNarrador("Yo", "La verdad, ya me cansé. Me gustaría terminar este proceso más tarde."),
            new DialogoNarrador("Bequín", "¡De acuerdo! Es posible retomar el proceso; sin embargo, no es recomendable dejarlo para los últimos días debido a que puede saturarse el portal o ya no entrar la respuesta por fallos en el sistema.")
        };

        yield return StartCoroutine(MostrarDialogos(dialogosAntes, panelDialogo, textoDialogo, nombreNarradorText));

        yield return StartCoroutine(MostrarTransicion("Unas horas más tarde..."));

        DialogoNarrador[] dialogosDespues = new DialogoNarrador[]
        {
            new DialogoNarrador("Yo", "Ya descansé, sigamos con el proceso de registro."),
            new DialogoNarrador("Bequín", "¡Excelente! Sigamos adelante...")
        };

        yield return StartCoroutine(MostrarDialogos(dialogosDespues, panelDialogo, textoDialogo, nombreNarradorText));

        textoAutomatico2_1.gameObject.SetActive(true);
        IniciarAudioTextoAuto(contenidoTextoAutomatico2_1);
        yield return StartCoroutine(EscribirTextoAutomatico(textoAutomatico2_1, contenidoTextoAutomatico2_1));

        textoAutomatico2_2.gameObject.SetActive(true);
        IniciarAudioTextoAuto(contenidoTextoAutomatico2_2);
        yield return StartCoroutine(EscribirTextoAutomatico(textoAutomatico2_2, contenidoTextoAutomatico2_2));

        botonImagen2_B.gameObject.SetActive(true);
        botonImagen2_B.onClick.RemoveAllListeners();
        botonImagen2_B.onClick.AddListener(() => CambiarImagen(imagen2, imagen4));

        yield return new WaitUntil(() => !imagen2.activeSelf);
    }

    IEnumerator SecuenciaImagen4()
    {
        DialogoNarrador[] dialogos = new DialogoNarrador[]
        {
            new DialogoNarrador("Bequín", "Una vez dentro iremos a 'Solicitudes'")
        };

        yield return StartCoroutine(MostrarDialogos(dialogos, panelDialogo, textoDialogo, nombreNarradorText));

        botonImagen4.gameObject.SetActive(true);
        botonImagen4.onClick.RemoveAllListeners();
        botonImagen4.onClick.AddListener(() => CambiarImagen(imagen4, imagen5));

        yield return new WaitUntil(() => !imagen4.activeSelf);
    }

    IEnumerator SecuenciaImagen5()
    {
        DialogoNarrador[] dialogos = new DialogoNarrador[]
        {
            new DialogoNarrador("Bequín", "Iremos a solicitar la Beca Manuela Garín Pinillos–Manutención, la cuál consta de un monto único (para esta beca en específico)."),
            new DialogoNarrador("Bequín", "Es importante revisar compatibilidades e incompatibilidades, además de motivos de cancelación de la misma en la convocatoria de interés.")
        };

        yield return StartCoroutine(MostrarDialogos(dialogos, panelDialogoArriba, textoDialogoArriba, nombreNarradorTextArriba));

        botonImagen5.gameObject.SetActive(true);
        botonImagen5.onClick.RemoveAllListeners();
        botonImagen5.onClick.AddListener(() => MostrarPanelFinal());

        yield return new WaitUntil(() => panelFinal.activeSelf);

        textoFinal.text = "Ahora toca esperar y saber si fuí seleccionado.";
        yield return new WaitForSeconds(tiempoAntesSiguienteNivel);

        SceneManager.LoadScene(siguienteNivel);
    }

    // ===== MÉTODOS AUXILIARES =====

    IEnumerator MostrarDialogos(DialogoNarrador[] dialogos, GameObject panelActivo, TextMeshProUGUI textoDialogoTMP, TextMeshProUGUI nombreTMP)
    {
        panelActivo.SetActive(true);

        foreach (var dialogo in dialogos)
        {
            nombreTMP.text = dialogo.nombreHablante;

            // ── Si es Bequín, reproduce su audio correspondiente ──
            if (dialogo.nombreHablante == "Bequín")
            {
                if (audioSourceBequín != null && audioClipsBequín != null &&
                    indiceAudioBequín < audioClipsBequín.Length &&
                    audioClipsBequín[indiceAudioBequín] != null)
                {
                    audioSourceBequín.Stop();
                    audioSourceBequín.clip = audioClipsBequín[indiceAudioBequín];
                    audioSourceBequín.Play();
                }
                indiceAudioBequín++; // Avanza al siguiente clip aunque no haya audio asignado
            }
            // ────────────────────────────────────────────────────

            yield return StartCoroutine(EscribirTextoAutomatico(textoDialogoTMP, dialogo.texto));

            // Detener audio de Bequín cuando el jugador hace click para avanzar
            esperandoClick = true;
            clickRecibido = false;
            yield return new WaitUntil(() => clickRecibido);

            if (dialogo.nombreHablante == "Bequín" && audioSourceBequín != null)
                audioSourceBequín.Stop(); // ← Corta el audio al avanzar

            yield return new WaitForSeconds(0.2f);
        }

        panelActivo.SetActive(false);
    }

    IEnumerator EscribirTextoAutomatico(TextMeshProUGUI textoTMP, string textoCompleto)
    {
        textoTMP.text = "";

        foreach (char letra in textoCompleto)
        {
            textoTMP.text += letra;
            yield return new WaitForSeconds(velocidadEscritura);
        }
    }

    private void IniciarAudioTextoAuto(string texto)
    {
        if (audioSourceTextoAuto == null || audioTextoAutomatico == null) return;
        float duracion = texto.Length * velocidadEscritura;
        audioSourceTextoAuto.clip = audioTextoAutomatico;
        audioSourceTextoAuto.loop = true;
        audioSourceTextoAuto.Play();
        StartCoroutine(DetenerAudioTextoAuto(duracion));
    }

    // ── Detiene el audio de texto automático después de N segundos ──
    private IEnumerator DetenerAudioTextoAuto(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (audioSourceTextoAuto != null)
        {
            audioSourceTextoAuto.Stop();
            audioSourceTextoAuto.loop = false;
        }
    }
    // ────────────────────────────────────────────────────────────────

    IEnumerator MostrarTransicion(string textoTransicionTexto)
    {
        panelTransicion.SetActive(true);
        textoTransicion.text = textoTransicionTexto;
        yield return new WaitForSeconds(tiempoTransicion);
        panelTransicion.SetActive(false);
    }

    void CambiarImagen(GameObject imagenActual, GameObject imagenSiguiente)
    {
        imagenActual.SetActive(false);
        imagenSiguiente.SetActive(true);
    }

    void MostrarPanelFinal()
    {
        panelDialogoArriba.SetActive(false);
        panelFinal.SetActive(true);
    }
}