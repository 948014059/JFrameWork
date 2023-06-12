using Assets.ManagerHotFix.JFramework.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Assets.HotFix.UI.Tips
{
    public class TipsModule : BaseModule
    {

        public TipsModule()
        {
            PreFabs = "Perfabs/Panel/TipsPanel";
            layer = LayerType.Tips;
        }


        public override Type GetView()
        {
            return typeof(TipsView);
        }

    }

}
