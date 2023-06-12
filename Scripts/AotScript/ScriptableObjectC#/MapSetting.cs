using System.Collections.Generic;
using UnityEngine;


// ��ͼ����
public enum MapThemeType
{
    Desert, // ɳĮ
    Snowfield,// ѩ��
    Swamp //����
}

// ������������
public enum MapItemType
{
    Node, // ��ͨ����
    Obstacle, //�ϰ�����

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
    // ����Ĭ�����ɸ�������
    public List<MapTheme> MapTheme = new List<MapTheme>();

    //˵�и�������
    public List<MapMsg> MapNodesSetting = new List<MapMsg>();
}