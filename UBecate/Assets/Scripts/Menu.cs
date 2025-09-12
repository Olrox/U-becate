using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    //Metodo para pasar a la escena de juego
    public void Jugar()
    {
        SceneManager.LoadScene("Lvl1");
    }
    //Metodo para salir del juego
    public void Salir()
    {
        Application.Quit();
    }
}