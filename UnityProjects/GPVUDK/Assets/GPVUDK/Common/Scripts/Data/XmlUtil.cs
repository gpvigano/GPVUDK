using System.IO;
using System.Xml;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

namespace GPVUDK
{
    public class XmlUtil
    {

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
