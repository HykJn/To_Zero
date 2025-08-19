using System.Collections.Generic;
using UnityEngine;

public class TimeTile : OperationTile
{
    #region ==========Properties==========
    #endregion

    #region ==========Fields==========
    [SerializeField] private TileType[] opers;
    [SerializeField] private int[] values;
    [SerializeField] private readonly float time = 5f;
    private float tick_time = 0f;
    private int idx = 0;
    #endregion

    #region ==========Unity Methods==========
    private void Update()
    {
        SwitchValue();
    }
    #endregion

    #region ==========Methods==========
    public void Init()
    {
        opers = new TileType[4];
        values = new int[4];
        idx = 0;
        tick_time = 0f;

        opers[0] = this.oper;
        values[0] = this.value;

        for (int i = 1; i < 4; i++)
        {
            opers[i] = (TileType)Random.Range(1, 5);
            values[i] = opers[i] == TileType.Mul || opers[i] == TileType.Div
                ? Random.Range(2, 4)
                : Random.Range(1, 8);
        }
    }

    void SwitchValue()
    {
        tick_time += Time.deltaTime;

        if (tick_time >= time)
        {
            tick_time = 0f;
            idx = (idx + 1) % opers.Length;

            this.TileType = opers[idx];
            this.Value = values[idx];
        }
    }
    #endregion
}
