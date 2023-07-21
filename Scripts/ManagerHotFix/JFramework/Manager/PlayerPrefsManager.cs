using Assets.ManagerHotFix.JFramework.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ManagerHotFix.JFramework.Manager
{
    public class PlayerPrefsManager : BaseSingleTon<PlayerPrefsManager>
    {
        private void SetInt(string key , int value )
        {
            PlayerPrefs.SetInt(key, value);
            PlayerPrefs.Save();
        }

        private int GetInt(string key,int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key,defaultValue);
        }

        private void SetFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
            PlayerPrefs.Save();
        }

        private float GetFloat(string key , float defaultValue = 0f)
        {
            return PlayerPrefs.GetFloat(key, defaultValue);
        }

        private void SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
            PlayerPrefs.Save();
        }

        private string GetString(string key, string defaultValue = "")
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }


        public void DelAllPrefs()
        {
            PlayerPrefs.DeleteAll();
        }

        public void DelPrefsByKey(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }

        #region 游戏版本

        string VersionKey = "Game_Version_Key";
        public void SetGameVersion(string version)
        {
            SetString(VersionKey, version);
        }

        public string GetGameVersion()
        {
            return GetString(VersionKey);
        }


        #endregion


    }
}
