public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ constraint implement type
	// }} 

	// {{ AOT generic type
	//LitJson.ImporterFunc`2<System.Int32,System.Int64>
	//System.Action`1<System.Object>
	//System.Action`1<System.Int64>
	//System.Action`2<System.Int32,System.Int32>
	//System.Action`2<System.Int64,System.Object>
	//System.Collections.Generic.Dictionary`2<System.Object,System.Object>
	//System.Collections.Generic.Dictionary`2/Enumerator<System.Object,System.Object>
	//System.Collections.Generic.IEnumerator`1<System.Object>
	//System.Collections.Generic.KeyValuePair`2<System.Object,System.Object>
	//System.Collections.Generic.List`1<System.Object>
	//System.Collections.Generic.List`1/Enumerator<System.Object>
	//System.Collections.Generic.Queue`1<System.Object>
	//System.Func`2<System.Object,System.Byte>
	// }}

	public void RefMethods()
	{
		// System.Void LitJson.JsonMapper::RegisterImporter<System.Int32,System.Int64>(LitJson.ImporterFunc`2<System.Int32,System.Int64>)
		// System.Object LitJson.JsonMapper::ToObject<System.Object>(System.String)
		// System.Object System.Activator::CreateInstance<System.Object>()
		// System.Object System.Linq.Enumerable::First<System.Object>(System.Collections.Generic.IEnumerable`1<System.Object>,System.Func`2<System.Object,System.Boolean>)
		// System.Object[] System.Linq.Enumerable::ToArray<System.Object>(System.Collections.Generic.IEnumerable`1<System.Object>)
		// System.Collections.Generic.List`1<System.Object> System.Linq.Enumerable::ToList<System.Object>(System.Collections.Generic.IEnumerable`1<System.Object>)
		// System.Byte UnityEngine.AndroidJavaObject::CallStatic<System.Byte>(System.String,System.Object[])
		// System.Object UnityEngine.AssetBundle::LoadAsset<System.Object>(System.String)
		// UnityEngine.AssetBundleRequest UnityEngine.AssetBundle::LoadAssetAsync<System.Object>(System.String)
		// System.Object UnityEngine.Component::GetComponent<System.Object>()
		// System.Object UnityEngine.GameObject::AddComponent<System.Object>()
		// System.Object UnityEngine.Object::Instantiate<System.Object>(System.Object,UnityEngine.Transform)
	}
}