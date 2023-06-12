using Assets.Assets.HotFix.UI.Tips;
using Assets.HotFix.ConfigData;
using Assets.ManagerHotFix.JFramework.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ManagerHotFix.JFramework.Manager
{

    public class TipsManager : BaseSingleTon<TipsManager>
    {
        private TipsConfig tipsData;
        private ConfigDataManager configDataManager;


        private void Awake()
        {
            configDataManager = ConfigDataManager.GetInstance();
        }

        public void ShowTipsByID(int id)
        {
            tipsData = configDataManager.GetConfigDataByType<TipsConfig>();
            TipsConfig localdata = tipsData.GetDataByID(id);

            if (!ModuleManager.GetInstance().IsModuleOpen<TipsModule>())
            {
                ModuleManager.GetInstance().OpenModule(typeof(TipsModule), null, localdata.tips);
            }

        }


        public void ShowTipByString(string Str)
        {
            if (!ModuleManager.GetInstance().IsModuleOpen<TipsModule>())
            {
                ModuleManager.GetInstance().OpenModule(typeof(TipsModule), null, Str);
            }
        }

        public string GetTipStringByID(int id)
        {
            return tipsData.GetDataByID(id).tips;
        }

    }

}
