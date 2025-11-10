using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Tooltip("tes move")]
    public string gameSceneName = "tes move";


    public void StartGameButton()
    {
        Debug.Log("Mulai Game...");
        SceneManager.LoadScene(gameSceneName);
    }

    public void OptionsButton()
    {
        Debug.Log("Membuka Pengaturan...");
    }

    public void CreditButton()
    {
        Debug.Log("Membuka kredit");
    }

    public void QuitGameButton()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
