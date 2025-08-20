using System;
using UnityEngine;
using UnityEngine.Serialization;

public class ObjectManager : MonoBehaviour
{
    #region ==========Properties==========
    public static ObjectManager Instance { get; private set; } = null;

    #endregion

    #region ==========Fields==========

    [Header("Prefabs")]
    [SerializeField] private GameObject prefabOperationTile;
    [SerializeField] private GameObject prefabSwapTile;
    [SerializeField] private GameObject prefabWall;
    [SerializeField] private GameObject prefabBox;
    [SerializeField] private GameObject prefabDrone;

    //pools
    private GameObject[] _objOperationTiles;
    private GameObject[] _objSwapTiles;
    private GameObject[] _objWalls;
    private GameObject[] _objBoxes;
    private GameObject[] _objDrones;
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
        _objOperationTiles = new GameObject[128];
        _objSwapTiles = new GameObject[128];
        _objWalls = new GameObject[128];
        _objBoxes = new GameObject[128];
        _objDrones = new GameObject[16];

        //Operation Tiles
        for (int i = 0; i < _objOperationTiles.Length; i++)
        {
            _objOperationTiles[i] = Instantiate(prefabOperationTile, this.transform);
            _objOperationTiles[i].SetActive(false);
        }

        //Swap Tiles
        for (int i = 0; i < _objSwapTiles.Length; i++)
        {
            _objSwapTiles[i] = Instantiate(prefabSwapTile, this.transform);
            _objSwapTiles[i].SetActive(false);
        }

        //Walls
        for (int i = 0; i < _objWalls.Length; i++)
        {
            _objWalls[i] = Instantiate(prefabWall, this.transform);
            _objWalls[i].SetActive(false);
        }

        //Boxes
        for (int i = 0; i < _objBoxes.Length; i++)
        {
            _objBoxes[i] = Instantiate(prefabBox, this.transform);
            _objBoxes[i].SetActive(false);
        }

        //Drones
        for (int i = 0; i < _objDrones.Length; i++)
        {
            _objDrones[i] = Instantiate(prefabDrone, this.transform);
            _objDrones[i].SetActive(false);
        }
    }

    public GameObject GetObject(ObjectID id)
    {
        GameObject[] pool = id switch
        {
            ObjectID.OperationTile => _objOperationTiles,
            ObjectID.SwapTile => _objSwapTiles,
            ObjectID.Wall => _objWalls,
            ObjectID.Box => _objBoxes,
            //ObjectID.Portal => obj_portals,
            ObjectID.Drone => _objDrones,
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
