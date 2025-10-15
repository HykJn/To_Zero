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
    [SerializeField] private GameObject prefab_OperationTile;
    [SerializeField] private GameObject prefab_SwapTile;
    [SerializeField] private GameObject prefab_Firewall;
    [SerializeField] private GameObject prefab_Observer;

    //보스
    [SerializeField] private GameObject prefab_BossLaser;

    //Pools
    private GameObject[] _pool_OperationTile, _pool_SwapTile, _pool_Firewall, _pool_Observer, _obj_BossLasers;

    #endregion

    #region =====Unity Events=====

    private void Awake()
    {
        Instance = this;

        _pool_OperationTile = new GameObject[MAX_OPER_TILE_COUNT];
        _pool_SwapTile = new GameObject[MAX_SWAP_TILE_COUNT];
        _pool_Firewall = new GameObject[MAX_FIREWALL_COUNT];
        _pool_Observer = new GameObject[MAX_OBSERVER_COUNT];
        _obj_BossLasers = new GameObject[MAX_BOSSLASER_COUNT];

        InitPool();
    }

    #endregion

    #region =====Methods=====

    private void InitPool()
    {
        for (int i = 0; i < MAX_OPER_TILE_COUNT; i++)
        {
            _pool_OperationTile[i] = Instantiate(prefab_OperationTile);
            _pool_OperationTile[i].SetActive(false);
        }

        for (int i = 0; i < MAX_SWAP_TILE_COUNT; i++)
        {
            _pool_SwapTile[i] = Instantiate(prefab_SwapTile);
            _pool_SwapTile[i].SetActive(false);
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

        //bossLaser
        for (int i = 0; i < _obj_BossLasers.Length; i++)
        {
            _obj_BossLasers[i] = Instantiate(prefab_BossLaser, this.transform);
            _obj_BossLasers[i].SetActive(false);
        }
    }

    public GameObject GetObject(ObjectID objID, bool active = true)
    {
        GameObject[] pool = objID switch
        {
            ObjectID.OperationTile => _pool_OperationTile,
            ObjectID.SwapTile => _pool_SwapTile,
            ObjectID.Firewall => _pool_Firewall,
            ObjectID.Observer => _pool_Observer,
            //BOSSlASER
            ObjectID.BossLaser => _obj_BossLasers,
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

    //보스
    public void ReleaseObject(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(this.transform);
        obj.transform.localPosition = Vector3.zero;
    }

    #endregion
}