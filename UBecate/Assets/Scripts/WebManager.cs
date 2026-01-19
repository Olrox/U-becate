using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class WebManager : MonoBehaviour
{
    [Header("Paneles UI")]
    public GameObject panel1;      // Panel informativo (fondo + texto + botón)
    public GameObject panel2;      // Panel de "screenshot" (imagen + botones)

    [Header("Elementos del Panel 1")]
    public Image fondoPanel1;      // Imagen de fondo informativa
    public TextMeshProUGUI textoPanel1;  // Texto descriptivo

    [Header("Elementos del Panel 2")]
    public Image screenshotImage;  // Image donde se muestra el "screenshot"
    public Sprite screenshotSimulado;  // Sprite estático que simula el screenshot del Panel1 (arrastra desde Assets)
    public Button btnTomarScreenshot;  // Botón "Tomar Screenshot"
    public Button btnSiguienteNivel;   // Botón para ir al siguiente nivel (inicialmente deshabilitado)

    [Header("Configuración")]
    public string textoInfo = "Tengo los requisitos para esta beca en especifico, ahora solo necesito revisar que sigue.";  // Texto por defecto
    public string nombreSiguienteEscena = "Lvl2";  // Nombre de la escena siguiente

    void Start()
    {
        // Estado inicial
        panel1.SetActive(true);
        panel2.SetActive(false);
        btnSiguienteNivel.interactable = false;  // Deshabilitado hasta tomar screenshot

        // Configurar Panel 1
        if (textoPanel1 != null)
            textoPanel1.text = textoInfo;
    }

    // Llamado desde el botón del Panel 1
    public void PasarAPanel2()
    {
        panel1.SetActive(false);
        panel2.SetActive(true);
    }

    // Llamado desde el botón "Tomar Screenshot" del Panel 2
    public void TomarScreenshot()
    {
        // Opción 1: SIMULACIÓN SIMPLE (recomendada - usa sprite predefinido)
        if (screenshotSimulado != null)
        {
            screenshotImage.sprite = screenshotSimulado;
            screenshotImage.gameObject.SetActive(true);
        }
        // Opción 2: CAPTURA REAL DE PANTALLA (descomenta si quieres)
        // StartCoroutine(CapturarPantallaReal());

        // Activar botón siguiente nivel
        btnSiguienteNivel.interactable = true;

        // Opcional: Desactivar botón de screenshot y mostrar mensaje
        if (btnTomarScreenshot != null)
            btnTomarScreenshot.interactable = false;
    }

    // Opción avanzada: Captura real de pantalla (descomenta en TomarScreenshot si la quieres)
    /*
    private IEnumerator CapturarPantallaReal()
    {
        yield return new WaitForEndOfFrame();  // Espera a que termine el frame actual
        Texture2D screenshot = ScreenCapture.CaptureScreenshotAsTexture();
        
        // Convertir a Sprite y asignar
        screenshotImage.sprite = Sprite.Create(
            screenshot, 
            new Rect(0, 0, screenshot.width, screenshot.height), 
            new Vector2(0.5f, 0.5f)
        );
        
        Destroy(screenshot);  // Liberar memoria
        screenshotImage.gameObject.SetActive(true);
    }
    */

    // Llamado desde el botón "Siguiente Nivel" del Panel 2
    public void IrASiguienteNivel()
    {
        SceneManager.LoadScene(nombreSiguienteEscena);
    }
}
