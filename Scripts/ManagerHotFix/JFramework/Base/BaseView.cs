using Assets.ManagerHotFix.JFramework.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.ManagerHotFix.JFramework.Base
{
    public abstract class BaseView : MonoBehaviour
    {
        public abstract void ShowView(System.Object obj);//Awake时传值
        public abstract void AddEventListener();
        public abstract void RemoveEventListener();

        public abstract void OnBtnClickEvent();

        public void AddBtnClickEvent(Transform btnRoot , UnityAction clickevent)
        {
            btnRoot.GetComponent<Button>().onClick.AddListener(clickevent);
        }

        public virtual void Awake()
        {
            System.Object openModuleObj = ModuleManager.GetInstance().openModelObj.Dequeue();
            ShowView(openModuleObj);
            AddEventListener();
            OnBtnClickEvent();
        }

        public virtual void OnDestroy()
        {
            RemoveEventListener();
        }



    }
}
