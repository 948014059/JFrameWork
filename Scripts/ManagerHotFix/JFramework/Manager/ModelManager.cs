using Assets.ManagerHotFix.JFramework.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ManagerHotFix.JFramework.Manager
{

    /// <summary>
    ///  Model管理
    /// </summary>
    public class ModelManager : BaseSingleTon<ModelManager>
    {
        private Dictionary<string, BaseModel> ModelDict = new Dictionary<string, BaseModel>();
        public T GetModelInstence<T>() where T : BaseModel  // 获得对应类型的Model
        {
            if (ModelDict.ContainsKey(typeof(T).Name))
            {
                return (T)ModelDict[typeof(T).Name];
            }
            else
            {
                T model = (T)System.Activator.CreateInstance(typeof(T));
                ModelDict.Add(typeof(T).Name, model);

                MethodInfo moduleInfo = typeof(T).GetMethod("Init");
                moduleInfo.Invoke(model, null);
                return model;
            }
        }



        public void ClearAllModel()
        {
            ModelDict.Clear();
        }

        public void RestAllModel() // 重置所有数据   
        {
            foreach (var item in ModelDict)
            {
                item.Value.ResetModel();
            }
        }
    }


}
