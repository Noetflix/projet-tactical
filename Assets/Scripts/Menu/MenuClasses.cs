using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuClasses : MonoBehaviour
{
    public static string SelectedClass { get; private set; }

    public static void SetSelectedClass(string className)
    {
        SelectedClass = className; 
    }

    public void ChooseClass(string className)
    {
        SetSelectedClass(className);
        // Seulement quand on joue vraiment
        if (Application.isPlaying)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Level_01_Introduction");
        }
    }
}
