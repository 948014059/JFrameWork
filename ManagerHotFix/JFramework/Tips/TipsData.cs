using Assets.ManagerHotFix.JFramework.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ManagerHotFix.JFramework.Tips
{
    public class TipsData : BaseTable
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

        public List<TipsData> data = new List<TipsData>();

        public TipsData GetDataByID(int id)
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
            return "ConfigDataText/TipsConfig";
        }
    }

}
