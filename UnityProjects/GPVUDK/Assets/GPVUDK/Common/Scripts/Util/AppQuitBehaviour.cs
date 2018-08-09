using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppQuitBehaviour : MonoBehaviour
{
    public void AppQuit()
    {

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
