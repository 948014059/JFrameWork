using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ManagerHotFix.JFramework.Base
{
    /// <summary>
    /// 基础单例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseSingleTon<T> : MonoBehaviour where T : Component
    {
        private static T _instance;

        public static T GetInstance()
        {

            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(T)) as T;
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.hideFlags = HideFlags.HideAndDontSave;
                    _instance = obj.AddComponent<T>();
                }
            }
            return _instance;

        }

    }
}
