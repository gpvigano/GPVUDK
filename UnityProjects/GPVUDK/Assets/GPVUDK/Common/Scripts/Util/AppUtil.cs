using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPVUDK
{
    public class AppUtil : MonoBehaviour
    {
        public enum BasePathEnum
        {
            DataPath,
            PersistentDataPath,
            Auto
        }

        [Tooltip("Base data path:\nApplication.dataPath\nApplication.persistentDataPath\nAuto (<dataPath>/.. in Editor and Standalone, persistentDataPath else)")]
        public BasePathEnum baseDataPath = BasePathEnum.PersistentDataPath;

        public string GetDataPath()
        {
            string dataPath = string.Empty;
            switch (baseDataPath)
            {
                case BasePathEnum.DataPath:
                    dataPath = Application.dataPath;
                    break;
                case BasePathEnum.PersistentDataPath:
                    dataPath = Application.persistentDataPath;
                    break;
                case BasePathEnum.Auto:
#if UNITY_EDITOR || UNITY_STANDALONE
                    dataPath = Application.dataPath+"/..";
#else
                dataPath = Application.persistentDataPath;
#endif
                    break;
            }
            return dataPath;
        }

        public void Quit()
        {

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
