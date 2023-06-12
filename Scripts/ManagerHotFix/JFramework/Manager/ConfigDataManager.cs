﻿using Assets.ManagerHotFix.JFramework.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ManagerHotFix.JFramework.Manager
{
    public class ConfigDataManager: BaseSingleTon<ConfigDataManager>
    {
        private Dictionary<string, BaseTable> AllTableDataDict = new Dictionary<string, BaseTable>();

        public void UpdateAllConfig()
        {
            Assembly ass = AppDomain.CurrentDomain.GetAssemblies().First(assembly => assembly.GetName().Name == "Assembly-CSharp");

            Debug.Log("开始更新配置表");
            string[] alltableKeys = AllTableDataDict.Keys.ToArray();
            for ( int i = 0; i< alltableKeys.Length; i++) 
            {                
                Type t =   ass.GetType("Assets.HotFix.ConfigData." + alltableKeys[i]);
                Debug.Log(t);
                BaseTable tableObj = (BaseTable)Activator.CreateInstance(t);
                string tablePath = "Configs/" + tableObj.GetTablePath();
                string tableData = ResourcesManager.GetTextAsset(tablePath);
                MethodInfo addConfigData =  this.GetType().GetMethod("AddConfigData").MakeGenericMethod(new Type[] { t });
                addConfigData.Invoke(this,new object[] { tableData, tableObj });
            }
        }


        public T GetConfigDataByType<T>() where T : BaseTable
        {
            BaseTable tableObj = System.Activator.CreateInstance<T>();
            if (tableObj == null)
            {
                Debug.Log("未找到对应配置表cs");
                return null;
            }
            if (AllTableDataDict.ContainsKey(typeof(T).Name))
            {
                return AllTableDataDict[typeof(T).Name] as T;
            }
            else
            {
                string tablePath = "Configs/" + tableObj.GetTablePath();
                string tableData = ResourcesManager.GetTextAsset(tablePath);
                return AddConfigData<T>(tableData, tableObj);
            }

        }

        public T AddConfigData<T>(string tableData, BaseTable table) where T : BaseTable
        {
            string[] dataLines = tableData.Split('\n');
            string[] variableNames = new string[] { };
            List<T> tableline = new List<T>();
            for (int i = 0; i < dataLines.Length; i++)
            {
                if (dataLines[i] == "")
                {
                    continue;
                }
                if (i == 0)
                {
                    variableNames = dataLines[0].Split(',');
                }
                else if (i >= 1)
                {
                    string[] lineSplit = dataLines[i].Split(',');
                    T configDataLineObj = Activator.CreateInstance<T>();

                    for (int j = 0; j < variableNames.Length; j++)
                    {
                        FieldInfo variable = typeof(T).GetField(variableNames[j].Trim());
                        switch (variable.FieldType.ToString())
                        {
                            case "System.Int32":
                                variable.SetValue(configDataLineObj, int.Parse(lineSplit[j]));
                                break;
                            case "System.String":
                                variable.SetValue(configDataLineObj, lineSplit[j]);
                                break;
                            default:
                                break;
                        }
                    }
                    tableline.Add(configDataLineObj);
                }
            }
            FieldInfo dataList = typeof(T).GetField("data");
            dataList.SetValue(table, tableline);
            if (AllTableDataDict.ContainsKey(typeof(T).Name))
            {
                AllTableDataDict[typeof(T).Name] = table;
            }
            else
            {
                AllTableDataDict.Add(typeof(T).Name, table);
            }
            return table as T;
        }
    }
}