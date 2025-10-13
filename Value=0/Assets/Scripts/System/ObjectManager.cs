using System;
using UnityEngine;
using static GLOBAL;

public class ObjectManager : MonoBehaviour
{
    #region =====Properties=====

    public static ObjectManager Instance { get; private set; }

    #endregion

    #region =====Fields=====

    [Header("Prefabs")]
    [SerializeField] private GameObject prefab_Tile;
    [SerializeField] private GameObject prefab_Firewall;
    [SerializeField] private GameObject prefab_Observer;

    //Pools
    private GameObject[] _pool_Tile, _pool_Firewall, _pool_Observer;

    #endregion

    #region =====Unity Events=====

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        _pool_Tile = new GameObject[MAX_TILE_COUNT];
        _pool_Firewall = new GameObject[MAX_FIREWALL_COUNT];
        _pool_Observer = new GameObject[MAX_OBSERVER_COUNT];
        
        InitPool();
    }
    
    #endregion

    #region =====Methods=====

    private void InitPool()
    {
        for (int i = 0; i < MAX_TILE_COUNT; i++)
        {
            _pool_Tile[i] = Instantiate(prefab_Tile);
            _pool_Tile[i].SetActive(false);
        }

        for (int i = 0; i < MAX_FIREWALL_COUNT; i++)
        {
            _pool_Firewall[i] = Instantiate(prefab_Firewall);
            _pool_Firewall[i].SetActive(false);
        }
        
        for (int i = 0; i < MAX_OBSERVER_COUNT; i++)
        {
            _pool_Observer[i] = Instantiate(prefab_Observer);
            _pool_Observer[i].SetActive(false);
        }
    }

    public GameObject GetObject(ObjectID objID, bool active = true)
    {
        GameObject[] pool = objID switch
        {
            ObjectID.Tile => _pool_Tile,
            ObjectID.Firewall => _pool_Firewall,
            ObjectID.Observer => _pool_Observer,
            _ => null
        };

        if (pool == null) throw new Exception("Object not found");

        foreach (GameObject obj in pool)
        {
            if (obj.activeSelf) continue;
            obj.SetActive(active);
            return obj;
        }

        return null;
    }

    #endregion
}