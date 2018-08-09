using System;
using UnityEngine;
using GPVUDK;

/// <summary>
/// Manager for XML configuration files
/// </summary>
abstract public class XmlConfig : MonoBehaviour
{
    public AppUtil appUtil;

    [Tooltip("Load the configuration file at startup")]
    public bool autoLoadConfiguration = false;
    [Tooltip("Create the configuration file if not present at startup")]
    public bool autoCreateConfiguration = false;

    [Tooltip("Configuration file path, relative to Base Path")]
    public string configFile = "settings.xml";

    protected object configData;

    private string configFilePath;

    public event Action<object> ConfigurationLoaded;

    public bool LoadConfiguration()
    {
        if(configData==null)
        {
            Debug.Log(GetType().Name + ": null configuration data.");
            return false;
        }
        UpdateFilePath();
        if (XmlUtil.ReadFromFile(ref configData, configFilePath))
        {
            OnConfigurationLoaded();
        }
        else
        {
            Debug.Log(GetType().Name + ": failed to load configuration from " + configFilePath);
            return false;
        }
        return true;
    }

    // Wrapper for UI
    public void Save()
    {
        SaveConfiguration();
    }

    public bool SaveConfiguration()
    {
        UpdateFilePath();
        if (XmlUtil.WriteToFile(configData, configFilePath))
        {
            Debug.Log(GetType().Name + ": configuration saved to " + configFilePath);
        }
        else
        {
            Debug.Log(GetType().Name + ": failed to save configuration to " + configFilePath);
            return false;
        }
        return true;
    }

    protected virtual void OnConfigurationLoaded()
    {
        Debug.Log(GetType().Name + ": configuration loaded from " + configFilePath);
        if (ConfigurationLoaded != null)
        {
            ConfigurationLoaded(configData);
        }
    }

    protected virtual void Awake()
    {
        if (autoLoadConfiguration)
        {
            if (!LoadConfiguration())
            {
                // if not available create it
                if (autoCreateConfiguration)
                {
                    SaveConfiguration();
                }
            }
        }
    }

    protected void UpdateFilePath()
    {
        string dataPath = appUtil.GetDataPath();
        configFilePath = dataPath + "/" + configFile;
    }

    protected virtual void OnValidate()
    {
        if (appUtil == null)
        {
            appUtil = GetComponent<AppUtil>();
        }
        if (appUtil == null)
        {
            appUtil = FindObjectOfType<AppUtil>();
        }
        if (appUtil == null)
        {
            Debug.LogWarningFormat("{0}: App Util field must be set.", GetType().Name);
        }
    }
}
