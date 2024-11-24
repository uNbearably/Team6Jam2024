using UnityEngine;
using UnityEngine.SceneManagement;

public class menu_script : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void OnButtonClick()
    {
        SceneManager.LoadScene("ART ART ART");
    }
   

    public void QuitScene()
    {
        Application.Quit();
    }

 
}
