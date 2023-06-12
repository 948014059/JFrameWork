using Assets.HotFix.ConfigData;
using Assets.HotFix.UI.Start;
using Assets.ManagerHotFix.JFramework.Base;
using Assets.ManagerHotFix.JFramework.Manager;
using Assets.ManagerHotFix.JFramework.Update;
using System;
using UnityEngine;

namespace Assets.HotFix.Game
{
    public class GameManager : BaseSingleTon<GameManager>
    {
        public void HotFixGameStart()
        {
            ModuleManager.GetInstance().OpenModule<StartModule>(()=>{
                ModuleManager.GetInstance().CloseModule<UpdateModule>();

            });
             
        }

    }
}