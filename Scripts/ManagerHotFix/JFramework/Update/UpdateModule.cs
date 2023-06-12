using Assets.ManagerHotFix.JFramework.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ManagerHotFix.JFramework.Update
{
    public class UpdateModule : BaseModule
    {
        public UpdateModule()
        {
            PreFabs = "Perfabs/Panel/UpdatePanel";
        }

        public override Type GetView()
        {
            return typeof(UpdateView);
        }
    }
}
