using UnityEngine;

public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ constraint implement type
	// }} 

	// {{ AOT generic type
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
	// }}

	public void RefMethods()
	{

		AnimationClip animClip = new AnimationClip();
		AssetBundleRequest abr = new AssetBundleRequest();
		LODGroup lodg = new LODGroup();
		CapsuleCollider cc = new CapsuleCollider();
		CapsuleCollider capsuleCollider = new CapsuleCollider();
		TerrainData terrainData = new TerrainData();
		MeshCollider meshCollider = new MeshCollider();
		BillboardRenderer billboardRenderer = new BillboardRenderer();
		WaitForSeconds waitForSeconds = new WaitForSeconds(0.1f);
		SkinnedMeshRenderer skinnedMeshRenderer = new SkinnedMeshRenderer();
		BoxCollider boxCollider = new BoxCollider();
		Tree tree = new Tree();
		LightProbeGroup lightProbeGroup = new LightProbeGroup();
		SphereCollider sphereCollider = new SphereCollider();
		TerrainCollider terrainCollider = new TerrainCollider();
		CharacterController characterController = new CharacterController();
		Terrain terrain = new Terrain();
		//new SpeedTreeWindAsset();
		Avatar avatar = transform.GetComponent<Avatar>();
		Animator animator = new Animator();
		AudioSource audioSource = new AudioSource();



		// System.Object System.Activator::CreateInstance<System.Object>()
		// System.Object UnityEngine.AssetBundle::LoadAsset<System.Object>(System.String)
		// UnityEngine.AssetBundleRequest UnityEngine.AssetBundle::LoadAssetAsync<System.Object>(System.String)
		// System.Object UnityEngine.Component::GetComponent<System.Object>()
		// System.Object UnityEngine.GameObject::AddComponent<System.Object>()
		// System.Object UnityEngine.Object::Instantiate<System.Object>(System.Object)
	}
}