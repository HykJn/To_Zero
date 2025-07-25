using UnityEngine;
using System;

public class ObjectManager : MonoBehaviour
{
    #region ==========Properties==========
    public static ObjectManager Instance => instance;
    #endregion

    #region ==========Fields==========
    private static ObjectManager instance = null;

    //Prefabs
    [SerializeField] private GameObject prefab_tile;
    [SerializeField] private GameObject prefab_timeTile;
    [SerializeField] private GameObject prefab_wall;

    //Pool
    private GameObject[] obj_tiles;
    private GameObject[] obj_timeTiles;
    private GameObject[] obj_walls;
    #endregion

    #region ==========Unity Methods==========
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this.gameObject);

        //Init Pools
        obj_tiles = new GameObject[64];
        obj_timeTiles = new GameObject[64];
        obj_walls = new GameObject[64];

        //Instantiate
        for (int i = 0; i < obj_tiles.Length; i++)
        {
            obj_tiles[i] = Instantiate(prefab_tile, this.transform);
            obj_tiles[i].SetActive(false);
        }
        for (int i = 0; i < obj_timeTiles.Length; i++)
        {
            obj_timeTiles[i] = Instantiate(prefab_timeTile, this.transform);
            obj_timeTiles[i].SetActive(false);
        }
        for (int i = 0; i < obj_walls.Length; i++)
        {
            obj_walls[i] = Instantiate(prefab_wall, this.transform);
            obj_walls[i].SetActive(false);
        }
    }
    #endregion

    #region ==========Methods==========
    public GameObject GetObject(ObjectID objID)
    {
        GameObject[] pool = objID switch
        {
            ObjectID.Tile => obj_tiles,
            ObjectID.TimeTile => obj_timeTiles,
            ObjectID.Wall => obj_walls,
            _ => throw new ArgumentException()
        };

        foreach (GameObject obj in pool)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }
        throw new Exception();
    }
    #endregion
}
