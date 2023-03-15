using Assets.ManagerHotFix.JFramework.Base;
using Assets.ManagerHotFix.JFramework.Manager;
using Assets.ManagerHotFix.JFramework.Update;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.HotFix.Game
{
    class GameManager : BaseSingleTon<GameManager>
    {
        public void HotFixGameStart()
        {
            ModuleManager.GetInstance().CloseModule<UpdateModule>();
            Debug.Log("进入到游戏逻辑中。。。");
            TipsManager.GetInstance().ShowTipsByID(1001);
        }
    }
}
