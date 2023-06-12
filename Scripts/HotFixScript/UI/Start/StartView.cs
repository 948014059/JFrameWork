using Assets.ManagerHotFix.JFramework.Base;
using Assets.ManagerHotFix.JFramework.Manager;
using LitJson;
using System;
using UnityEngine;

namespace Assets.HotFix.UI.Start
{
    public class StartView : BaseView
    {
		public override void AddEventListener()
        {
        }

        public override void ShowView(object obj)
        {

        }


        private void ShowTips()
        {
            //TipsManager.GetInstance().ShowTipsByID(1002);
            Debug.Log("1111");
            JsonData json = new JsonData();
            json["gameId"] = "0";
            HttpManager.GetInstance().StartHttpPOST("http://192.168.1.14:29010/game/settlement",
                json.ToJson(),(data)=> {
                    TipsManager.GetInstance().ShowTipByString(data);
                }, (data) => {
                    TipsManager.GetInstance().ShowTipByString(data);
                });
        }
        public override void OnBtnClickEvent()
        {
            AddBtnClickEvent(transform.Find("ShowTipBtn"), ShowTips);
        }

        public override void RemoveEventListener()
        {
        }

	}
}