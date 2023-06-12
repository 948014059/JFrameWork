using System.Collections.Generic;
using UnityEngine;


// 地图主题
public enum MapThemeType
{
    Desert, // 沙漠
    Snowfield,// 雪地
    Swamp //沼泽
}

// 格子类型设置
public enum MapItemType
{
    Node, // 普通格子
    Obstacle, //障碍格子

}


[System.Serializable]
public class MapMsg
{
    public MapItemType mapItemType;
    public MapThemeType mapThemeType;
    public List<GameObject> prefabs;
}
[System.Serializable]
public class MapTheme
{
    public MapThemeType mapThemeType;
    public GameObject defaultGo;
}


[CreateAssetMenu(fileName = "MapSetting", menuName = "Map/MapSetting",order =1)]
public class MapSetting : ScriptableObject
{
    // 主题默认生成格子设置
    public List<MapTheme> MapTheme = new List<MapTheme>();

    //说有格子类型
    public List<MapMsg> MapNodesSetting = new List<MapMsg>();
}