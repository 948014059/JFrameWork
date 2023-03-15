using Assets.ManagerHotFix.JFramework.Base;
using Assets.ManagerHotFix.JFramework.Tips;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ManagerHotFix.JFramework.Manager
{

    public class TipsManager : BaseSingleTon<TipsManager>
    {
        private TipsData tipsData;
        private ConfigDataManager configDataManager;


        private void Awake()
        {
            configDataManager = ConfigDataManager.GetInstance();
            tipsData = configDataManager.GetConfigDataByType<TipsData>();
        }

        public void ShowTipsByID(int id)
        {
            TipsData localdata = tipsData.GetDataByID(id);

            //MessageObj<TipsData> ojdata = new MessageObj<TipsData>();
            //ojdata._object = localdata;
            //ojdata._callBack = () => { Debug.Log("这是一个回调"); };

            if (!ModuleManager.GetInstance().IsModuleOpen<TipsModule>())
            {
                ModuleManager.GetInstance().OpenModule(typeof(TipsModule), null, localdata);
            }


        }

        public string GetTipStringByID(int id)
        {
            return tipsData.GetDataByID(id).tips;
        }

    }

}
