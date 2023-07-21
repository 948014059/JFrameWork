using Assets.ManagerHotFix.JFramework.Base;
using System;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.HotFix.UI.PromptBox
{

    public class PromptBoxData
    {
        public string titleStr;
        public string infoStr;
        public UnityAction yesCallBack;
        public UnityAction noCallBack;
    }


    public class PromptBoxView : BaseView
    {

        private UnityAction yesCallBack = null;
        private UnityAction noCallBack = null;
        private PromptBoxData currBoxData;
        public override void AddEventListener()
        {

        }

        public override void ShowView(object obj)
        {
            if (obj != null)
            {
                currBoxData = obj as PromptBoxData;
                transform.Find("Text_Title").GetComponent<Text>().text = currBoxData.titleStr;
                transform.Find("Text_Info").GetComponent<Text>().text = currBoxData.infoStr;

                yesCallBack = currBoxData.yesCallBack;
                noCallBack = currBoxData.noCallBack;
            }
        }

        private void ClickYes()
        {
            yesCallBack?.Invoke();
        }
        private void ClickNo()
        {
            noCallBack?.Invoke();
        }


        public override void OnBtnClickEvent()
        {
            AddBtnClickEvent(transform.Find("Button_Yes"), ClickYes);
            AddBtnClickEvent(transform.Find("Button_No"), ClickNo);
        }

        public override void RemoveEventListener()
        {
        }

    }
}