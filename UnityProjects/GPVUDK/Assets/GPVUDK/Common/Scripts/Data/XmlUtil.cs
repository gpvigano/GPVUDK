using System.IO;
using System.Xml;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

namespace GPVUDK
{
    /// <summary>
    /// Utility for XML serialization.
    /// </summary>
    public class XmlUtil
    {
        /// <summary>
        /// Read an object from an XML file.
        /// </summary>
        /// <typeparam name="T">Serializable type.</typeparam>
        /// <param name="dataHolder">Object to be read.</param>
        /// <param name="filePath">Path to the XML file.</param>
        /// <returns>true on success or false on error.</returns>
        static public bool ReadFromFile<T>(ref T dataHolder, string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                Debug.LogErrorFormat("Empty input file path");
                return false;
            }

            try
            {
                // Load XmlSceneData from XML
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                FileStream stream = new FileStream(filePath, FileMode.Open);
                dataHolder = (T)serializer.Deserialize(stream);
                stream.Dispose();
            }
            catch (IOException)
            {
                Debug.LogErrorFormat("Unable to read XML file {0}", filePath);
                return false;
            }
            return true;
        }


        /// <summary>
        /// Serialize an object to an XML file.
        /// </summary>
        /// <typeparam name="T">Serializable type.</typeparam>
        /// <param name="dataHolder">Object to be serialized.</param>
        /// <param name="filePath">Path to the XML file.</param>
        /// <returns>true on success or false on error.</returns>
        static public bool WriteToFile<T>(T dataHolder, string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                Debug.LogErrorFormat("Empty output file path");
                return false;
            }

            try
            {
                // Setup XML output settings
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "    ";
                settings.Encoding = Encoding.UTF8;
                settings.CheckCharacters = true;
                // Write data to XML
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                FileStream stream = new FileStream(filePath, FileMode.Create);
                XmlWriter w = XmlWriter.Create(stream, settings);
                serializer.Serialize(w, dataHolder);
                stream.Dispose();
            }
            catch (IOException e)
            {
                Debug.LogErrorFormat("Unable to write XML file {0}\n{1}", filePath, e);
                return false;
            }
            return true;
        }
   }
}
