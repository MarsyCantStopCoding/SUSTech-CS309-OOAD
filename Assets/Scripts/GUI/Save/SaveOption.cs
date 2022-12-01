﻿using System.IO;
using Archive;
using GameData;
using Interfaces;
using StateMachine;
using TMPro;
using UnityEngine;
using Units;
using UnityEngine.Serialization;

namespace GUI.Save
{
    public class SaveOption: MonoBehaviour, IClickable
    {
        public TextMeshProUGUI saveName;
        private string _saveFileName;
        public TextMeshProUGUI time;
        public string Save;
        public bool isSave = false;

        public void SetSave(string s)
        {
            this.Save = s;
            FileInfo fi = new FileInfo(s);
            _saveFileName = fi.Name.Replace(fi.Extension, "");
            saveName.text = _saveFileName;
            time.text = fi.LastWriteTime.ToString();
        }

        public bool IsClicked()
        {
            if (isSave)
            {
                UIManager.Instance.DefaultUI.SetActive(true);
                MapSaver.Save($"{_saveFileName}.json");
                UIManager.Instance.LoadMenuUI.SetActive(false);
                return true;
            }
            else 
            {
                GameDataManager data = GameDataManager.Instance;
                data.PanelShowing = false;
                UIManager.Instance.LoadMenuUI.SetActive(false);

                ItemLoader.Instance.GoDefault(Save);
                return true;
            }
        }
    }
}