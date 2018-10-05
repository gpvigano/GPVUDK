using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GPVUDK
{
    public class InterfaceLayoutConfigurator : MonoBehaviour
    {
        public InterfaceLayout defaultInterfaceLayout;
        public InterfaceLayout[] interfaceLayouts;

        [Header("Debug")]
        [SerializeField]
        private string forceDevice = string.Empty;

#if UNITY_EDITOR
        [MenuItem("CONTEXT/InterfaceLayoutConfigurator/Apply Default Interface Layout")]
        static private void ApplyDefaultInterfaceLayout(MenuCommand menuCommand)
        {
            InterfaceLayoutConfigurator ilConf = menuCommand.context as InterfaceLayoutConfigurator;
            ilConf.defaultInterfaceLayout.Apply();
        }

        // Validate the menu item defined by the function above.
        [MenuItem("CONTEXT/InterfaceLayoutConfigurator/Apply Default Interface Layout", true)]
        static private bool ValidateApplyDefaultInterfaceLayout()
        {
            return DefaultInterfaceLayoutAvailable();
        }

        [MenuItem("CONTEXT/InterfaceLayoutConfigurator/Apply First Interface Layout")]
        static private void ApplyFirstInterfaceLayout(MenuCommand menuCommand)
        {
            InterfaceLayoutConfigurator ilConf = menuCommand.context as InterfaceLayoutConfigurator;
            ilConf.interfaceLayouts[0].Apply();
        }

        // Validate the menu item defined by the function above.
        [MenuItem("CONTEXT/InterfaceLayoutConfigurator/Apply First Interface Layout", true)]
        static private bool ValidateApplyFirstInterfaceLayout()
        {
            return InterfaceLayoutAvailable();
        }

        [MenuItem("CONTEXT/InterfaceLayoutConfigurator/Apply Last Interface Layout")]
        static private void ApplyLastInterfaceLayout(MenuCommand menuCommand)
        {
            InterfaceLayoutConfigurator ilConf = menuCommand.context as InterfaceLayoutConfigurator;
            ilConf.interfaceLayouts[ilConf.interfaceLayouts.Length - 1].Apply();
        }

        // Validate the menu item defined by the function above.
        [MenuItem("CONTEXT/InterfaceLayoutConfigurator/Apply Last Interface Layout", true)]
        static private bool ValidateApplyLastInterfaceLayout()
        {
            return InterfaceLayoutAvailable();
        }

        [MenuItem("CONTEXT/InterfaceLayoutConfigurator/Duplicate Last Interface Layout")]
        static private void DuplicateLastInterfaceLayout(MenuCommand menuCommand)
        {
            InterfaceLayoutConfigurator ilConf = menuCommand.context as InterfaceLayoutConfigurator;
            List<InterfaceLayout> layoutList = new List<InterfaceLayout>();
            layoutList.AddRange(ilConf.interfaceLayouts);
            InterfaceLayout lastItem = ilConf.interfaceLayouts[ilConf.interfaceLayouts.Length - 1];
            InterfaceLayout newItem = lastItem.Duplicate();
            newItem.name += " (copy)";
            layoutList.Add(newItem);
            ilConf.interfaceLayouts = layoutList.ToArray();
        }

        // Validate the menu item defined by the function above.
        [MenuItem("CONTEXT/InterfaceLayoutConfigurator/Duplicate Last Interface Layout", true)]
        static private bool ValidateDuplicateLastInterfaceLayout()
        {
            return InterfaceLayoutAvailable();
        }

        [MenuItem("CONTEXT/InterfaceLayoutConfigurator/Update Default Interface Layout")]
        static private void UpdateDefaultInterfaceLayout(MenuCommand menuCommand)
        {
            InterfaceLayoutConfigurator ilConf = menuCommand.context as InterfaceLayoutConfigurator;
            ilConf.defaultInterfaceLayout.Store();
        }

        // Validate the menu item defined by the function above.
        [MenuItem("CONTEXT/InterfaceLayoutConfigurator/Update Default Interface Layout", true)]
        static private bool ValidateUpdateDefaultInterfaceLayout()
        {
            return DefaultInterfaceLayoutAvailable();
        }

        [MenuItem("CONTEXT/InterfaceLayoutConfigurator/Update Last Interface Layout")]
        static private void UpdateLastInterfaceLayout(MenuCommand menuCommand)
        {
            InterfaceLayoutConfigurator ilConf = menuCommand.context as InterfaceLayoutConfigurator;
            ilConf.interfaceLayouts[ilConf.interfaceLayouts.Length - 1].Store();
        }

        // Validate the menu item defined by the function above.
        [MenuItem("CONTEXT/InterfaceLayoutConfigurator/Update Last Interface Layout", true)]
        static private bool ValidateUpdateLastInterfaceLayout()
        {
            return InterfaceLayoutAvailable();
        }

        static private bool DefaultInterfaceLayoutAvailable()
        {
            if (Selection.activeGameObject == null)
            {
                return false;
            }
            InterfaceLayoutConfigurator ifConf = Selection.activeGameObject.GetComponent<InterfaceLayoutConfigurator>();
            return ifConf != null && ifConf.defaultInterfaceLayout != null;
        }

        static private bool InterfaceLayoutAvailable()
        {
            if (Selection.activeGameObject == null)
            {
                return false;
            }
            InterfaceLayoutConfigurator ifConf = Selection.activeGameObject.GetComponent<InterfaceLayoutConfigurator>();
            return ifConf != null && ifConf.interfaceLayouts != null && ifConf.interfaceLayouts.Length > 0;
        }
#endif

        /// <summary>
        /// Apply the current layout on startup.
        /// </summary>
        private void Start()
        {
            string currDevice = VRSettings.loadedDeviceName;
#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(forceDevice))
            {
                currDevice = forceDevice;
            }
#endif
            if (interfaceLayouts != null)
            {
                bool applied = false;
                foreach (InterfaceLayout layout in interfaceLayouts)
                {
                    if (layout.name == currDevice)
                    {
                        layout.Apply();
                        applied = true;
                        break;
                    }
                }
                if (!applied)
                {
                    defaultInterfaceLayout.Apply();
                }
            }
        }

        /// <summary>
        /// Check and fix values in Unity Inspector.
        /// </summary>
        private void OnValidate()
        {
            if (defaultInterfaceLayout != null)
            {
                defaultInterfaceLayout.name = "default";
                if ((interfaceLayouts == null || interfaceLayouts.Length == 0))
                {
                    {
                        if (defaultInterfaceLayout.canvasTransform != null)
                        {
                            foreach (CanvasTransform layout in defaultInterfaceLayout.canvasTransform)
                            {
                                layout.Store();
                            }
                        }
                        // TODO :remove this after migration
                    }
                }
                foreach (CanvasTransform layout in defaultInterfaceLayout.canvasTransform)
                {
                    layout.Validate();
                }
            }
            if ((interfaceLayouts != null || interfaceLayouts.Length > 0))
            {
                foreach (InterfaceLayout interfaceLayout in interfaceLayouts)
                {
                    foreach (CanvasTransform layout in interfaceLayout.canvasTransform)
                    {
                        layout.Validate();
                    }
                }
            }
        }
    }
}
