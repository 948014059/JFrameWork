using Assets.ManagerHotFix.JFramework.Base;
using System;

namespace Assets.HotFix.UI.PromptBox
{
	public class PromptBoxModule : BaseModule
    {
		public PromptBoxModule()
        {
            PreFabs = "Perfabs/Panel/PromptBoxPanel";
        }

        public override Type GetView()
        {
            return typeof(PromptBoxView);
        }
	}
}