using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPVUDK
{
    /// <summary>
    /// Application utility.
    /// </summary>
    public class AppUtil : MonoBehaviour
    {
        /// <summary>
        /// Base data path.
        /// </summary>
        public enum BasePathEnum
        {
            /// <summary>
            /// Application.dataPath.
            /// </summary>
            DataPath,
            /// <summary>
            /// Application.persistentDataPath.
            /// </summary>
            PersistentDataPath,
            /// <summary>
            /// dataPath/.. in Editor and Standalone, persistentDataPath else.
            /// </summary>
            Auto
        }

        /// <summary>
        /// Base data path:
        /// Application.dataPath
        /// Application.persistentDataPath
        /// Auto (dataPath/.. in Editor and Standalone, persistentDataPath else).
        /// </summary>
        [Tooltip("Base data path:\nApplication.dataPath\nApplication.persistentDataPath\nAuto (<dataPath>/.. in Editor and Standalone, persistentDataPath else)")]
        public BasePathEnum baseDataPath = BasePathEnum.PersistentDataPath;

        /// <summary>
        /// Get a data path according to the configuration.
        /// </summary>
        /// <returns>The proper data path.</returns>
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
                    dataPath = Application.dataPath + "/..";
#else
                dataPath = Application.persistentDataPath;
#endif
                    break;
            }
            return dataPath;
        }


        /// <summary>
        /// Quit the application or pause Unity Player.
        /// </summary>
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
