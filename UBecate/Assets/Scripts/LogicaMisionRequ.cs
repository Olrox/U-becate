using System.Collections;
using UnityEngine;
using TMPro;

public class LogicaMisionRequ : MonoBehaviour
{
    [Header("Referencias principales")]
    public LogicaPC logicaPC; // referencia al script de misión principal
    public string nombreObjeto = "Requisito"; // nombre del objeto recolectado
    public GameObject panelRecoleccion; // panel que muestra el mensaje de recolección
    public TextMeshProUGUI textoPanel; // texto dentro del panel
    public GameObject simboloMision;

    [Header("Efectos opcionales")]
    public AudioSource sonidoRecoleccion; // sonido al recoger
    public float duracionPanel = 2f; // tiempo visible del panel
    public float duracionDesvanecer = 1.5f; // tiempo del efecto visual
    public float intensidadBrillo = 2f; // nivel de brillo visual al recoger

    private bool jugadorCerca = false;
    private bool recolectado = false;
    private Transform jugador;
    private Material materialOriginal;
    private Renderer renderObjeto;

    void Start()
    {
        if (panelRecoleccion != null)
            panelRecoleccion.SetActive(false);

        // Obtener el Renderer del objeto
        renderObjeto = GetComponentInChildren<Renderer>();
        if (renderObjeto != null)
            materialOriginal = renderObjeto.material;
    }

    void Update()
    {
        // Solo puede recoger si está cerca y no lo ha hecho antes
        if (jugadorCerca && !recolectado && Input.GetKeyDown(KeyCode.E))
        {
            RecolectarObjeto();
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            jugadorCerca = true;
            jugador = col.transform;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            jugadorCerca = false;
        }
    }

    void RecolectarObjeto()
    {
        recolectado = true;

        // 🔹 Actualiza progreso de misión
        logicaPC.numObjetivos--;
        logicaPC.textoMision.text = "Busca los requisitos de la beca" + "\nRestantes: " + logicaPC.numObjetivos;

        if (logicaPC.numObjetivos <= 0)
        {
            //Cursor.visible = true;
            //Cursor.lockState = CursorLockMode.None;
            logicaPC.textoMision.text = "¡Completaste la búsqueda! Regresa a la computadora para Hacer el registro.";
            // Cambiar color antes de activar
            Renderer renderer = simboloMision.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.blue; // O el color que quieras
            }
            simboloMision.SetActive(true);
            logicaPC.diaDPago = true;
            //logicaPC.botonMision.SetActive(true);
        }

        // 🔹 Sonido
        if (sonidoRecoleccion != null)
            sonidoRecoleccion.Play();

        // 🔹 Animación (por código)
            StartCoroutine(EfectoFlotarBrilloDesvanecer());

        // 🔹 Panel informativo
        if (panelRecoleccion != null && textoPanel != null)
            StartCoroutine(MostrarPanelRecoleccion());

        // 🔹 Desactivar objeto después de un pequeño retraso
        StartCoroutine(DesactivarObjeto());
    }

    IEnumerator MostrarPanelRecoleccion()
    {
        textoPanel.text = "Has recolectado requisito: <color=green>" + nombreObjeto + "</color>";
        panelRecoleccion.SetActive(true);
        yield return new WaitForSeconds(duracionPanel);
        panelRecoleccion.SetActive(false);
    }

    IEnumerator DesactivarObjeto()
    {
        yield return new WaitForSeconds(duracionDesvanecer + 0.5f);
        transform.parent.gameObject.SetActive(false);
    }

    // 🔹 Animación visual por código (sin necesidad de Animator)
    IEnumerator EfectoFlotarBrilloDesvanecer()
    {
        float tiempo = 0f;
        Vector3 posInicial = transform.position;

        if (renderObjeto == null)
            yield break;

        // Guardar color original
        Color colorInicial = renderObjeto.material.color;

        while (tiempo < duracionDesvanecer)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracionDesvanecer;

            // Movimiento de flotación
            transform.position = posInicial + Vector3.up * Mathf.Sin(t * Mathf.PI) * 0.5f;

            // Efecto de brillo (aumenta la emisión temporalmente)
            float intensidad = Mathf.Lerp(1f, intensidadBrillo, Mathf.PingPong(t * 2f, 1f));
            renderObjeto.material.color = colorInicial * intensidad;

            // Desvanecer gradualmente
            Color c = renderObjeto.material.color;
            c.a = Mathf.Lerp(1f, 0f, t);
            renderObjeto.material.color = c;

            yield return null;
        }
    }
}