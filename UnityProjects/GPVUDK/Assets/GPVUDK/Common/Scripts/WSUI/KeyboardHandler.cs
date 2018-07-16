using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GPVUDK
{
    public class KeyboardHandler : MonoBehaviour
    {
        private InputField[] inputBoxList;

        private InputField editBox;
        private int currInputField = 0;
        private string[] savedText;

        public Dictionary<string, string> SavedData { get; private set; }

        public event Action<Dictionary<string, string>> TextSubmit;

        public void SetText(string keyText)
        {
            editBox.text = keyText;
        }

        public void HandleInput(string keyText)
        {
            editBox.text += keyText;
        }

        public void HandleBackspace()
        {
            if (editBox.text.Length > 0)
            {
                editBox.text = editBox.text.Remove(editBox.text.Length - 1);
            }
        }

        public void NextInputField()
        {
            editBox.interactable = false;
            currInputField = (currInputField + 1) % inputBoxList.Length;
            editBox = inputBoxList[currInputField];
            editBox.interactable = true;
        }

        public void SendData()
        {
            Save();
            Debug.Log("Entered: " + editBox.text);
            if (TextSubmit != null)
            {
                TextSubmit(SavedData);
            }
        }

        public void Clear()
        {
            editBox.text = "";
        }

        public void Undo()
        {
            for (int i = 0; i < inputBoxList.Length; i++)
            {
                inputBoxList[i].text = savedText[i];
            }
        }

        public void Save()
        {
            SavedData.Clear();
            for (int i = 0; i < inputBoxList.Length; i++)
            {
                savedText[i] = inputBoxList[i].text;
                SavedData.Add(inputBoxList[i].name, inputBoxList[i].text);
            }
        }

        private void Awake()
        {
            inputBoxList = GetComponentsInChildren<InputField>();
            editBox = GetComponentInChildren<InputField>();
            if (editBox != null)
            {
                savedText = new string[inputBoxList.Length];
                SavedData = new Dictionary<string, string>();
                Save();
            }
        }
    }
}