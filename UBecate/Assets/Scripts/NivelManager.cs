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
    public GameObject panelDialogoArriba; // Panel de diálogo en posición superior
    public TextMeshProUGUI textoDialogo;
    public TextMeshProUGUI textoDialogoArriba; // Texto para panel superior
    public TextMeshProUGUI nombreNarradorText;
    public TextMeshProUGUI nombreNarradorTextArriba; // Nombre para panel superior

    [Header("Textos Automáticos (TMP)")]
    public TextMeshProUGUI textoAutomatico1; // Para imagen 1
    [TextArea(3, 5)]
    public string contenidoTextoAutomatico1; // Contenido editable en inspector
    
    [Header("Textos Automáticos Imagen 3 (8 textos)")]
    public TextMeshProUGUI[] textosAutomaticos3; // Array de 8 textos para imagen 3
    [TextArea(3, 5)]
    public string[] contenidosTextosAutomaticos3 = new string[8]; // Contenidos editables
    
    [Header("Textos Automáticos Imagen 2 (Segunda vez)")]
    public TextMeshProUGUI textoAutomatico2_1; // Primer texto para segunda vuelta a imagen 2
    [TextArea(3, 5)]
    public string contenidoTextoAutomatico2_1;
    
    public TextMeshProUGUI textoAutomatico2_2; // Segundo texto para segunda vuelta a imagen 2
    [TextArea(3, 5)]
    public string contenidoTextoAutomatico2_2;

    [Header("Botones de Avance")]
    public Button botonImagen1;
    public Button botonImagen2_A; // Primer botón de imagen 2 (va a imagen 3)
    public Button botonImagen2_B; // Segundo botón de imagen 2 (va a imagen 4)
    public Button botonImagen3;
    public Button botonImagen4;
    public Button botonImagen5;

    [Header("Panel de Transición")]
    public GameObject panelTransicion; // Panel oscuro para la transición
    public TextMeshProUGUI textoTransicion;

    [Header("Panel Final")]
    public GameObject panelFinal;
    public TextMeshProUGUI textoFinal;

    [Header("Configuración")]
    public float velocidadEscritura = 0.05f;
    public float tiempoTransicion = 3f; // Tiempo que dura el panel de transición
    public float tiempoAntesSiguienteNivel = 3f;

    [Header("Escena Siguiente")]
    public string siguienteNivel = "NivelSiguiente";

    private bool esperandoClick = false;
    private bool clickRecibido = false;
    private bool primeraVezImagen2 = true; // Para controlar qué botón activar

    // Diálogos para cada imagen
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
        // Inicializar todo desactivado excepto la primera imagen
        InicializarEstado();
        StartCoroutine(SecuenciaNivel());
    }

    void Update()
    {
        // Detectar clic izquierdo para avanzar diálogos
        if (esperandoClick && Input.GetMouseButtonDown(0))
        {
            clickRecibido = true;
            esperandoClick = false;
        }
    }

    void InicializarEstado()
    {
        // Desactivar todas las imágenes
        if (imagen1 != null) imagen1.SetActive(false);
        if (imagen2 != null) imagen2.SetActive(false);
        if (imagen3 != null) imagen3.SetActive(false);
        if (imagen4 != null) imagen4.SetActive(false);
        if (imagen5 != null) imagen5.SetActive(false);

        // Desactivar paneles
        if (panelDialogo != null) panelDialogo.SetActive(false);
        if (panelDialogoArriba != null) panelDialogoArriba.SetActive(false);
        if (panelTransicion != null) panelTransicion.SetActive(false);
        if (panelFinal != null) panelFinal.SetActive(false);

        // Desactivar textos automáticos
        if (textoAutomatico1 != null) textoAutomatico1.gameObject.SetActive(false);
        if (textoAutomatico2_1 != null) textoAutomatico2_1.gameObject.SetActive(false);
        if (textoAutomatico2_2 != null) textoAutomatico2_2.gameObject.SetActive(false);
        
        foreach (var texto in textosAutomaticos3)
        {
            if (texto != null) texto.gameObject.SetActive(false);
        }

        // Desactivar botones
        if (botonImagen1 != null) botonImagen1.gameObject.SetActive(false);
        if (botonImagen2_A != null) botonImagen2_A.gameObject.SetActive(false);
        if (botonImagen2_B != null) botonImagen2_B.gameObject.SetActive(false);
        if (botonImagen3 != null) botonImagen3.gameObject.SetActive(false);
        if (botonImagen4 != null) botonImagen4.gameObject.SetActive(false);
        if (botonImagen5 != null) botonImagen5.gameObject.SetActive(false);
    }

    IEnumerator SecuenciaNivel()
    {
        // ===== IMAGEN 1 =====
        yield return StartCoroutine(SecuenciaImagen1());

        // ===== IMAGEN 2 (Primera vez) =====
        yield return StartCoroutine(SecuenciaImagen2PrimeraVez());

        // ===== IMAGEN 3 =====
        yield return StartCoroutine(SecuenciaImagen3());

        // ===== IMAGEN 2 (Segunda vez con transición) =====
        yield return StartCoroutine(SecuenciaImagen2SegundaVez());

        // ===== IMAGEN 4 =====
        yield return StartCoroutine(SecuenciaImagen4());

        // ===== IMAGEN 5 (Final) =====
        yield return StartCoroutine(SecuenciaImagen5());
    }

    // ===== SECUENCIA IMAGEN 1 =====
    IEnumerator SecuenciaImagen1()
    {
        imagen1.SetActive(true);

        // Diálogo del narrador
        DialogoNarrador[] dialogos = new DialogoNarrador[]
        {
            new DialogoNarrador("Yo", "Leyendo la convocatoria descubrí que debo registrar  la solicitud de la beca a través del Sistema Integra. ")
        };

        yield return StartCoroutine(MostrarDialogos(dialogos, panelDialogo, textoDialogo, nombreNarradorText));

        // Activar y escribir texto automático
        textoAutomatico1.gameObject.SetActive(true);
        yield return StartCoroutine(EscribirTextoAutomatico(textoAutomatico1, contenidoTextoAutomatico1));

        // Activar botón
        botonImagen1.gameObject.SetActive(true);
        botonImagen1.onClick.AddListener(() => CambiarImagen(imagen1, imagen2));

        // Esperar a que se presione el botón
        yield return new WaitUntil(() => !imagen1.activeSelf);
    }

    // ===== SECUENCIA IMAGEN 2 (Primera vez) =====
    IEnumerator SecuenciaImagen2PrimeraVez()
    {
        // Diálogo del narrador
        DialogoNarrador[] dialogos = new DialogoNarrador[]
        {
            new DialogoNarrador("Goyo", "Es hora de crear tu perfil en INTEGRA, es importante pensar que cuando llenes el formulario, tendrás que validar que tus datos sean correctos ")
            
        };

        yield return StartCoroutine(MostrarDialogos(dialogos, panelDialogo, textoDialogo, nombreNarradorText));

        // Activar SOLO el primer botón (va a imagen 3)
        botonImagen2_A.gameObject.SetActive(true);
        botonImagen2_A.onClick.RemoveAllListeners();
        botonImagen2_A.onClick.AddListener(() => CambiarImagen(imagen2, imagen3));

        // Esperar a que se presione el botón
        yield return new WaitUntil(() => !imagen2.activeSelf);
    }

    // ===== SECUENCIA IMAGEN 3 =====
    IEnumerator SecuenciaImagen3()
    {
        botonImagen2_A.gameObject.SetActive(false);
        // Diálogos del narrador (múltiples)
        DialogoNarrador[] dialogos = new DialogoNarrador[]
        {
            new DialogoNarrador("Goyo", "Es importante que en caso de corrección de nombre y/o género validez que esos datos se encuentren en el DGAE, antes de registrarte con la nueva información, una vez terminado, picar a registrar."),
            new DialogoNarrador("Goyo", "Para esta parte la llenaré por ti, pero recuerda que fuera de este juego lo deberás de llenar con tu información real."),
            new DialogoNarrador("Yo", "Espera, ¿Estamos en un juego?")
        };

        yield return StartCoroutine(MostrarDialogos(dialogos, panelDialogo, textoDialogo, nombreNarradorText));

        // Escribir 8 textos automáticos uno tras otro (cada uno con su propio contenido)
        for (int i = 0; i < textosAutomaticos3.Length && i < 8; i++)
        {
            if (textosAutomaticos3[i] != null && i < contenidosTextosAutomaticos3.Length)
            {
                textosAutomaticos3[i].gameObject.SetActive(true);
                yield return StartCoroutine(EscribirTextoAutomatico(
                    textosAutomaticos3[i], 
                    contenidosTextosAutomaticos3[i]
                ));
                yield return new WaitForSeconds(0.5f);
            }
        }

        // Activar botón para volver a imagen 2
        botonImagen3.gameObject.SetActive(true);
        botonImagen3.onClick.RemoveAllListeners();
        botonImagen3.onClick.AddListener(() => CambiarImagen(imagen3, imagen2));

        // Esperar a que se presione el botón
        yield return new WaitUntil(() => !imagen3.activeSelf);
        
        primeraVezImagen2 = false; // Ya no es la primera vez en imagen 2
    }

    // ===== SECUENCIA IMAGEN 2 (Segunda vez con transición) =====
    IEnumerator SecuenciaImagen2SegundaVez()
    {
        // Diálogos con ambos personajes
        DialogoNarrador[] dialogosAntes = new DialogoNarrador[]
        {
            new DialogoNarrador("Goyo", "Ahora que ya estamos dentro del portal y tenemos nuestro usuario, procederemos a iniciar sesión."),
            new DialogoNarrador("Yo", "La verdad ya me cansé, me gustaría terminar este proceso más tarde."),
            new DialogoNarrador("Goyo", "¡De acuerdo! Es posible retomar el proceso, sin embargo, no es recomendable dejarlo para los últimos días debido a que puede saturarse el portal o ya no entrar la respuesta por fallos en el sistema. ")
        };

        yield return StartCoroutine(MostrarDialogos(dialogosAntes, panelDialogo, textoDialogo, nombreNarradorText));

        // TRANSICIÓN (sin cerrar diálogo)
        yield return StartCoroutine(MostrarTransicion("Unas horas más tarde..."));


        // Continuar diálogos después de la transición
        DialogoNarrador[] dialogosDespues = new DialogoNarrador[]
        {
            new DialogoNarrador("Yo", "Ya descanse, sigamos con el proceso de registro."),
            new DialogoNarrador("Goyo", "¡Excelente! Sigamos adelante...")
        };

        yield return StartCoroutine(MostrarDialogos(dialogosDespues, panelDialogo, textoDialogo, nombreNarradorText));

        // Escribir dos textos automáticos
        textoAutomatico2_1.gameObject.SetActive(true);
        yield return StartCoroutine(EscribirTextoAutomatico(textoAutomatico2_1, contenidoTextoAutomatico2_1));
        
        textoAutomatico2_2.gameObject.SetActive(true);
        yield return StartCoroutine(EscribirTextoAutomatico(textoAutomatico2_2, contenidoTextoAutomatico2_2));

        // AHORA SÍ activar el segundo botón (va a imagen 4) después de que termine el texto
        botonImagen2_B.gameObject.SetActive(true);
        botonImagen2_B.onClick.RemoveAllListeners();
        botonImagen2_B.onClick.AddListener(() => CambiarImagen(imagen2, imagen4));

        // Esperar a que se presione el botón
        yield return new WaitUntil(() => !imagen2.activeSelf);
    }

    // ===== SECUENCIA IMAGEN 4 =====
    IEnumerator SecuenciaImagen4()
    {
        // Diálogo del narrador
        DialogoNarrador[] dialogos = new DialogoNarrador[]
        {
            new DialogoNarrador("Goyo", "Una vez dentro iremos a “solicitud” ")
        };

        yield return StartCoroutine(MostrarDialogos(dialogos, panelDialogo, textoDialogo, nombreNarradorText));

        // Activar botón
        botonImagen4.gameObject.SetActive(true);
        botonImagen4.onClick.RemoveAllListeners();
        botonImagen4.onClick.AddListener(() => CambiarImagen(imagen4, imagen5));

        // Esperar a que se presione el botón
        yield return new WaitUntil(() => !imagen4.activeSelf);
    }

    // ===== SECUENCIA IMAGEN 5 (Final) =====
    IEnumerator SecuenciaImagen5()
    {
        // Diálogos en panel superior
        DialogoNarrador[] dialogos = new DialogoNarrador[]
        {
            new DialogoNarrador("Goyo", "Iremos a solicitar la beca de manutención, la cuál consta de un monto único (para esta beca en específico)."),
            new DialogoNarrador("Goyo", "Es importante revisar compatibilidades e incompatibilidades, además de motivos de cancelación de la misma en la convocatoria de interés.")
        };

        yield return StartCoroutine(MostrarDialogos(dialogos, panelDialogoArriba, textoDialogoArriba, nombreNarradorTextArriba));

        // Activar botón que mostrará panel final
        botonImagen5.gameObject.SetActive(true);
        botonImagen5.onClick.RemoveAllListeners();
        botonImagen5.onClick.AddListener(() => MostrarPanelFinal());

        // Esperar a que se presione el botón
        yield return new WaitUntil(() => panelFinal.activeSelf);

        // Mostrar texto final y esperar
        textoFinal.text = "Ahora toca esperar y saber si fuí seleccionado.";
        yield return new WaitForSeconds(tiempoAntesSiguienteNivel);

        // Cargar siguiente nivel
        SceneManager.LoadScene(siguienteNivel);
    }

    // ===== MÉTODOS AUXILIARES =====

    // Mostrar diálogos con click para avanzar Y EFECTO DE ESCRITURA
    IEnumerator MostrarDialogos(DialogoNarrador[] dialogos, GameObject panelActivo, TextMeshProUGUI textoDialogoTMP, TextMeshProUGUI nombreTMP)
    {
        panelActivo.SetActive(true);

        foreach (var dialogo in dialogos)
        {
            nombreTMP.text = dialogo.nombreHablante;
            
            // Escribir el diálogo letra por letra
            yield return StartCoroutine(EscribirTextoAutomatico(textoDialogoTMP, dialogo.texto));

            // Esperar click del usuario
            esperandoClick = true;
            clickRecibido = false;
            yield return new WaitUntil(() => clickRecibido);
            yield return new WaitForSeconds(0.2f);
        }

        panelActivo.SetActive(false);
    }

    // Escribir texto automáticamente
    IEnumerator EscribirTextoAutomatico(TextMeshProUGUI textoTMP, string textoCompleto)
    {
        textoTMP.text = "";

        foreach (char letra in textoCompleto)
        {
            textoTMP.text += letra;
            yield return new WaitForSeconds(velocidadEscritura);
        }
    }

    // Mostrar transición
    IEnumerator MostrarTransicion(string textoTransicionTexto)
    {
        panelTransicion.SetActive(true);
        textoTransicion.text = textoTransicionTexto;

        yield return new WaitForSeconds(tiempoTransicion);

        panelTransicion.SetActive(false);
    }

    // Cambiar de imagen
    void CambiarImagen(GameObject imagenActual, GameObject imagenSiguiente)
    {
        imagenActual.SetActive(false);
        imagenSiguiente.SetActive(true);
    }

    // Mostrar panel final
    void MostrarPanelFinal()
    {
        panelDialogoArriba.SetActive(false);
        panelFinal.SetActive(true);
    }
}