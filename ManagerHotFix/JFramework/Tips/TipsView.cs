using Assets.ManagerHotFix.JFramework.Base;
using Assets.ManagerHotFix.JFramework.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace Assets.ManagerHotFix.JFramework.Tips
{

    class TipsView : BaseView
    {
        private Text tipText;
        private TipsData tipsdata;


        public override void AddEventListener()
        {
        }

        public override void ShowView(object obj)
        {
            if (obj != null)
            {
                tipsdata = obj as TipsData;
            }
            tipText = transform.Find("TipBG/Text").GetComponent<Text>();
        }

        private void Start()
        {
            tipText.text = tipsdata.tips;
            Invoke("CloseThisTips", 2);
        }

        private void CloseThisTips()
        {
            ModuleManager.GetInstance().CloseModule<TipsModule>();
        }


        public override void OnBtnClickEvent()
        {
        }

        public override void RemoveEventListener()
        {
        }
    }

}
