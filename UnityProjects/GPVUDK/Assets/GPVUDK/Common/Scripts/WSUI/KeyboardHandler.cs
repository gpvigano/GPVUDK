using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GPVUDK
{
    /// <summary>
    /// Virtual keyboard handler
    /// </summary>
    /// <remarks>Even if Unity provides a virtual input keyboard,
    /// it cannot be used inside a volumetric view, but only into a 2D view:
    /// https://docs.microsoft.com/en-us/windows/mixed-reality/keyboard-input-in-unity
    /// </remarks>
    public class KeyboardHandler : MonoBehaviour
    {
        [Tooltip("Keboard object, holding all the key buttons")]
        [SerializeField]
        private GameObject keyboardPanel;
        [SerializeField]
        private InputField[] inputBoxList;
        [Tooltip("Image that changes color when Caps Lock key is pressed")]
        [SerializeField]
        private Image capsLockLed;

        private InputField editBox;
        private int currInputField = 0;
        private string[] savedText;
        private bool shiftOn = false;
        private bool capsLock = false;
        private List<Text> letterKeyLabel = new List<Text>();

        public Dictionary<string, string> SavedData { get; private set; }

        public event Action<Dictionary<string, string>> TextSubmit;

        public void SetText(string keyText)
        {
            editBox.text = keyText;
        }

        public void HandleInput(string keyText)
        {
            bool capitals = capsLock;
            if (shiftOn)
            {
                capitals = !capitals;
                shiftOn = false;
                SwitchShift();
            }
            if (capitals)
            {
                keyText = keyText.ToUpperInvariant();
            }
            int p1 = editBox.selectionAnchorPosition;
            int p2 = editBox.selectionFocusPosition;
            Debug.Log(p1.ToString() + " - " + p2.ToString());
            if (p1 != p2)
            {
                int pmin = Math.Min(p1, p2);
                int pmax = Math.Max(p1, p2);
                editBox.text = editBox.text.Remove(pmin, pmax - pmin).Insert(pmin, keyText);
            }
            else if (editBox.isFocused)
            {
                int pos = editBox.caretPosition;
                editBox.text = editBox.text.Insert(pos, keyText);
                editBox.caretPosition = pos + 1;
            }
            else
            {
                editBox.text += keyText;
                editBox.MoveTextEnd(false);
            }
        }

        public void HandleBackspace()
        {
            int pos = editBox.caretPosition;
            if (pos > 0)
            {
                editBox.text = editBox.text.Remove(pos - 1, 1);
                editBox.caretPosition = pos - 1;
            }
        }

        public void HandleShift()
        {
            shiftOn = true;
            SwitchShift();
        }

        public void HandleCapsLock()
        {
            capsLock = !capsLock;
            SwitchShift();
            if (capsLockLed != null)
            {
                capsLockLed.color = capsLock ? Color.green : Color.gray;
            }
        }

        public void SwitchShift()
        {
            bool capitals = shiftOn ? !capsLock : capsLock;
            foreach (Text label in letterKeyLabel)
            {
                label.text = capitals ? label.text.ToUpperInvariant() : label.text.ToLowerInvariant();
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

        private void Start()
        {
            if (inputBoxList == null || inputBoxList.Length == 0 && keyboardPanel != null)
            {
                inputBoxList = keyboardPanel.GetComponentsInChildren<InputField>();
            }
            // initialize the input boxes and the current edit box
            if (inputBoxList != null && inputBoxList.Length > 0)
            {
                editBox = inputBoxList[0];
                if (editBox != null)
                {
                    // initialize members and store initial data to enable "undo"
                    savedText = new string[inputBoxList.Length];
                    SavedData = new Dictionary<string, string>();
                    Save();
                }
            }
            // initialize the Caps Lock LED
            if (capsLockLed != null)
            {
                capsLockLed.color = capsLock ? Color.green : Color.gray;
            }
            // cache the button labels that can be Upper or lower case
            foreach (Button key in keyboardPanel.GetComponentsInChildren<Button>())
            {
                Text label = key.GetComponentInChildren<Text>();
                if (label != null && label.text.Length>0 && char.IsLetter(label.text[0]))
                {
                    letterKeyLabel.Add(label);
                }
            }

        }
    }
}