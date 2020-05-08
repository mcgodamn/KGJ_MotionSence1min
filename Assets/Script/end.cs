using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class end : MonoBehaviour
{
    public void Reload()
    {
        SceneManager.LoadScene(0);
    }

    public void leave()
    {
        Application.Quit();
    }
}
