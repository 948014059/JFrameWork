using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ManagerHotFix.JFramework.Base
{
    public abstract class BaseModule
    {
        public enum LayerType
        {
            UI, // UI层
            PromptBox, //通用提示框
            Tips, // 提示层
            Log,
            
        }

        public string PreFabs = "";
        public LayerType layer = LayerType.UI;

        public abstract Type GetView();
    }
}
