using System;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    #region ==========Properties==========
    public static ObjectManager Instance => instance;
    #endregion

    #region ==========Fields==========
    private static ObjectManager instance = null;

    [Header("Prefabs")]
    [SerializeField] private GameObject prefab_operationTile;
    [SerializeField] private GameObject prefab_swapTile;
    [SerializeField] private GameObject prefab_wall;
    [SerializeField] private GameObject prefab_box;
    [SerializeField] private GameObject prefab_drone;

    //pools
    GameObject[] obj_operationTiles;
    GameObject[] obj_swapTiles;
    GameObject[] obj_walls;
    GameObject[] obj_boxes;
    GameObject[] obj_drones;
    #endregion

    #region ==========Unity Methods==========
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
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
        obj_operationTiles = new GameObject[128];
        obj_swapTiles = new GameObject[128];
        obj_walls = new GameObject[128];
        obj_boxes = new GameObject[128];
        obj_drones = new GameObject[16];

        //Operation Tiles
        for (int i = 0; i < obj_operationTiles.Length; i++)
        {
            obj_operationTiles[i] = Instantiate(prefab_operationTile, this.transform);
            obj_operationTiles[i].SetActive(false);
        }

        //Swap Tiles
        for (int i = 0; i < obj_swapTiles.Length; i++)
        {
            obj_swapTiles[i] = Instantiate(prefab_swapTile, this.transform);
            obj_swapTiles[i].SetActive(false);
        }

        //Walls
        for (int i = 0; i < obj_walls.Length; i++)
        {
            obj_walls[i] = Instantiate(prefab_wall, this.transform);
            obj_walls[i].SetActive(false);
        }

        //Boxes
        for (int i = 0; i < obj_boxes.Length; i++)
        {
            obj_boxes[i] = Instantiate(prefab_box, this.transform);
            obj_boxes[i].SetActive(false);
        }

        //Drones
        for (int i = 0; i < obj_drones.Length; i++)
        {
            obj_drones[i] = Instantiate(prefab_drone, this.transform);
            obj_drones[i].SetActive(false);
        }
    }

    public GameObject GetObject(ObjectID id)
    {
        GameObject[] pool = id switch
        {
            ObjectID.OperationTile => obj_operationTiles,
            ObjectID.SwapTile => obj_swapTiles,
            ObjectID.Wall => obj_walls,
            ObjectID.Box => obj_boxes,
            //ObjectID.Portal => obj_portals,
            ObjectID.Drone => obj_drones,
            _ => throw new ArgumentException("Invalid ObjectID", nameof(id))
        };

        for (int i = 0; i < pool.Length; i++)
        {
            if (!pool[i].activeSelf)
            {
                pool[i].SetActive(true);
                return pool[i];
            }
        }
        throw new InvalidOperationException("No available objects in the pool for " + id);
    }

    public GameObject GetObject(ObjectID id, Vector3 position)
    {
        GameObject obj = GetObject(id);
        obj.transform.position = position;
        return obj;
    }
    #endregion
}
