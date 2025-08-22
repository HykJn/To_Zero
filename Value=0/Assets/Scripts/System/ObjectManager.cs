using System;
using UnityEngine;
using static GlobalDefines;

public class ObjectManager : MonoBehaviour
{
    #region ==========Properties==========

    public static ObjectManager Instance { get; private set; }

    #endregion

    #region ==========Fields==========

    [Header("Prefabs")]
    [SerializeField] private GameObject prefab_Tile;

    [SerializeField] private GameObject prefab_Wall;
    [SerializeField] private GameObject prefab_Box;
    [SerializeField] private GameObject prefab_Drone;

    //pools
    private GameObject[] _obj_Tiles;
    private GameObject[] _obj_Walls;
    private GameObject[] _obj_Boxes;
    private GameObject[] _obj_Drones;

    #endregion

    #region ==========Unity Methods==========

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        InitPool();
    }

    #endregion

    #region ==========Methods==========

    private void InitPool()
    {
        _obj_Tiles = new GameObject[MAX_TILE_COUNT];
        _obj_Walls = new GameObject[MAX_WALL_COUNT];
        _obj_Boxes = new GameObject[MAX_BOX_COUNT];
        _obj_Drones = new GameObject[MAX_DRONE_COUNT];

        //Operation Tiles
        for (int i = 0; i < _obj_Tiles.Length; i++)
        {
            _obj_Tiles[i] = Instantiate(prefab_Tile, this.transform);
            _obj_Tiles[i].SetActive(false);
        }

        //Walls
        for (int i = 0; i < _obj_Walls.Length; i++)
        {
            _obj_Walls[i] = Instantiate(prefab_Wall, this.transform);
            _obj_Walls[i].SetActive(false);
        }

        //Boxes
        for (int i = 0; i < _obj_Boxes.Length; i++)
        {
            _obj_Boxes[i] = Instantiate(prefab_Box, this.transform);
            _obj_Boxes[i].SetActive(false);
        }

        //Drones
        for (int i = 0; i < _obj_Drones.Length; i++)
        {
            _obj_Drones[i] = Instantiate(prefab_Drone, this.transform);
            _obj_Drones[i].SetActive(false);
        }
    }

    public GameObject GetObject(ObjectID id)
    {
        GameObject[] pool = id switch
        {
            ObjectID.Tile => _obj_Tiles,
            ObjectID.Wall => _obj_Walls,
            ObjectID.Box => _obj_Boxes,
            //ObjectID.Portal => obj_portals,
            ObjectID.Drone => _obj_Drones,
            _ => throw new ArgumentException("Invalid ObjectID", nameof(id))
        };

        foreach (GameObject obj in pool)
        {
            if (obj.activeSelf) continue;

            obj.SetActive(true);
            return obj;
        }

        throw new InvalidOperationException("No available objects in the pool for " + id);
    }

    public GameObject GetObject(ObjectID id, Vector3 position)
    {
        GameObject obj = GetObject(id);
        obj.transform.position = position;
        return obj;
    }

    public GameObject GetObject(ObjectID id, Transform parent) => GetObject(id, parent, Vector3.zero);

    public GameObject GetObject(ObjectID id, Transform parent, Vector3 position, bool isLocal = true)
    {
        GameObject obj = GetObject(id);
        obj.transform.SetParent(parent);
        if (isLocal) obj.transform.localPosition = position;
        else obj.transform.position = position;
        return obj;
    }

    public void ReleaseObject(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(this.transform);
        obj.transform.localPosition = Vector3.zero;
    }

    #endregion
}