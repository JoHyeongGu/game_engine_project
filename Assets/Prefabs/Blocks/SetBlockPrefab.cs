using UnityEngine;

[System.Serializable]
public struct PrefabItem
{
    public Block.COLOR key;
    public GameObject prefab;
}

public class SetBlockPrefab : MonoBehaviour
{
    public PrefabItem[] prefabList;

    void Update()
    {
        InitPrefabList();
    }

    void InitPrefabList()
    {
        if (prefabList.Length == 0) return;
    }
}
