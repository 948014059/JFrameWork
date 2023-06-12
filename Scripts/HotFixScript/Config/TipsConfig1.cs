using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Assets.ManagerHotFix.JFramework.Base;

namespace Assets.HotFix.ConfigData
{
   public class TipsConfig1 : BaseTable
   {

        /// <summary>
        /// 唯一id
        /// </summary>
       public int id;

        /// <summary>
        /// 提示
        /// </summary>
       public string tips;

        /// <summary>
        /// 等级
        /// </summary>
       public string level;

       public List<TipsConfig1> data = new List<TipsConfig1>();

       public TipsConfig1 GetDataByID(int id)
       {
           foreach (var item in data)
           {
               if (item.id == id)
               {
                   return item;
               }
           }
           Debug.Log("未在配置表找到该id，请确认...");
           return null;
       }

       public override string GetTablePath()
       {
           return "TipsConfig1";
       }
   }
}
