using UnityEngine;


[CreateAssetMenu(fileName = "BossStageMap", menuName = "Game/Boss Stage Map")]
public class BossStageMap : ScriptableObject
{
    [Header("Stage Info")]
    public int phase = 1;


    [TextArea(6, 6)]
    public string stageMap;
    public int startValue;
    public int moveCount;
    public int bossTargetValue;
}
