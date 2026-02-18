using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PantallaCarga : MonoBehaviour
{
    [System.Serializable]
    public class ImagenPorEscena
    {
        public string nombreEscena;
        public Sprite imagenFade;
    }

    [Header("Configuración de Imágenes")]
    public ImagenPorEscena[] imagenesPorEscena;

    [Header("Panel Fade")]
    public GameObject panelFade;
    public Image imagenFade;
    public CanvasGroup canvasGroup;

    [Header("Configuración de Tiempo")]
    public float tiempoMostrarImagen = 2f;
    public float tiempoDesvanecimiento = 1.5f;

    [Header("Bloqueo de Interacción")]
    public bool bloquearInteraccionDuranteFade = true;

    // EVENTO ESTÁTICO que otros scripts pueden escuchar
    public static event Action OnFadeCompletado;
    
    // PROPIEDAD ESTÁTICA para verificar si el fade ha terminado
    public static bool FadeCompletado { get; private set; } = false;

    private void Awake()
    {
        // Resetear el estado al cargar la escena
        FadeCompletado = false;
    }

    private void Start()
    {
        ConfigurarPanel();
        StartCoroutine(EfectoFadeInicial());
    }

    void ConfigurarPanel()
    {
        if (panelFade != null)
        {
            panelFade.SetActive(true);
        }

        if (canvasGroup == null && panelFade != null)
        {
            canvasGroup = panelFade.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = panelFade.AddComponent<CanvasGroup>();
            }
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            
            if (bloquearInteraccionDuranteFade)
            {
                canvasGroup.blocksRaycasts = true;
            }
        }

        string escenaActual = SceneManager.GetActiveScene().name;
        Sprite imagenParaEstaEscena = ObtenerImagenParaEscena(escenaActual);

        if (imagenParaEstaEscena != null && imagenFade != null)
        {
            imagenFade.sprite = imagenParaEstaEscena;
        }
        else
        {
            Debug.LogWarning($"No se encontró imagen para la escena '{escenaActual}'");
        }
    }

    Sprite ObtenerImagenParaEscena(string nombreEscena)
    {
        foreach (var imagenConfig in imagenesPorEscena)
        {
            if (imagenConfig.nombreEscena == nombreEscena)
            {
                return imagenConfig.imagenFade;
            }
        }
        return null;
    }

    IEnumerator EfectoFadeInicial()
    {
        // 1. Mostrar la imagen durante el tiempo especificado
        yield return new WaitForSeconds(tiempoMostrarImagen);

        // 2. Desvanecer gradualmente la imagen
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < tiempoDesvanecimiento)
        {
            tiempoTranscurrido += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, tiempoTranscurrido / tiempoDesvanecimiento);
            
            if (canvasGroup != null)
            {
                canvasGroup.alpha = alpha;
            }

            yield return null;
        }

        // 3. Asegurarse de que esté completamente transparente
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
        }

        // 4. Desactivar el panel
        if (panelFade != null)
        {
            panelFade.SetActive(false);
        }

        // 5. NOTIFICAR QUE EL FADE HA TERMINADO
        FadeCompletado = true;
        OnFadeCompletado?.Invoke();

        Debug.Log("Fade completado - El nivel puede comenzar");
    }

    public void ReiniciarFade()
    {
        FadeCompletado = false;
        StopAllCoroutines();
        ConfigurarPanel();
        StartCoroutine(EfectoFadeInicial());
    }
}