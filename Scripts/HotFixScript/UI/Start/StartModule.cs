using Assets.ManagerHotFix.JFramework.Base;
using System;

namespace Assets.HotFix.UI.Start
{
	public class StartModule : BaseModule
    {
		public StartModule()
        {
            PreFabs = "Perfabs/Panel/StartPanel";
        }

        public override Type GetView()
        {
            return typeof(StartView);
        }
	}
}