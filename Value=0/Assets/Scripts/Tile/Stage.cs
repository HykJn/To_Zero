using UnityEngine;
using System;

public class Stage : MonoBehaviour
{
    #region ==========Properties==========
    
    #endregion

    #region ==========Fields==========
    [SerializeField, TextArea(6, 6)] private string stageMap;
    [SerializeField] private int startNumber, moves;
    [SerializeField] private Vector3Int startPos;
    [SerializeField] private GameObject[] objs;
    #endregion

    #region ==========Unity Methods==========
    private void Start()
    {
        Init();
        OnEnable();
    }

    private void OnEnable()
    {
        Restart();
    }

    private void OnDisable()
    {
        foreach (GameObject obj in objs)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
    }
    #endregion

    #region ==========Methods==========
    private void Init()
    {
        GameObject.FindWithTag("Player").GetComponent<Player>().StartNumber = startNumber;
        GameObject.FindWithTag("Player").GetComponent<Player>().Moves = moves;
        objs = new GameObject[64];

        string[] lines = stageMap.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            string[] line = lines[i].Split(' ');
            for (int j = 0; j < line.Length; j++)
            {
                int z = (lines.Length / 2) - i;
                int x = j - (line.Length / 2);

                GameObject obj;

                if (line[j].Equals("W") || line[j].Equals("w")) obj = ObjectManager.Instance.GetObject(ObjectID.Wall);
                else if (line[j][0] == 'T' || line[j][0] == 't') obj = ObjectManager.Instance.GetObject(ObjectID.TimeTile);
                else obj = ObjectManager.Instance.GetObject(ObjectID.Tile);

                objs[i * line.Length + j] = obj;
                obj.transform.position = new Vector3(x, 0, z);
                if (!obj.TryGetComponent<LogicTile>(out LogicTile tile)) continue;

                if (tile is TimeTile timeTile)
                {
                    timeTile.Operator = line[j][1] switch
                    {
                        '+' => Operator.Add,
                        '-' => Operator.Sub,
                        '*' => Operator.Mul,
                        '/' => Operator.Div,
                        _ => throw new ArgumentException()
                    };
                    timeTile.Value = int.Parse(line[j][2..]);
                    timeTile.Init();
                }
                else if (line[j].Equals("N") || line[j].Equals("n") || line[j].Equals("0"))
                {
                    tile.Operator = Operator.None;
                    tile.Value = 0;
                }
                else if (line[j].Equals("P") || line[j].Equals("p"))
                {
                    tile.Operator = Operator.Portal;
                    tile.Value = 0;
                }
                else if (line[j].Equals("S") || line[j].Equals("s"))
                {
                    tile.Operator = Operator.Start;
                    tile.Value = 0;
                    startPos = new Vector3Int(x, 1, z);
                }
                else
                {
                    tile.Operator = line[j][0] switch
                    {
                        '+' => Operator.Add,
                        '-' => Operator.Sub,
                        '*' => Operator.Mul,
                        '/' => Operator.Div,
                        _ => throw new ArgumentException()
                    };
                    tile.Value = int.Parse(line[j][1..]);
                }
            }
        }
    }

    public void Restart()
    {
        GameObject.FindWithTag("Player").transform.position = startPos;
        GameObject.FindWithTag("Player").GetComponent<Player>().StartNumber = startNumber;
        GameObject.FindWithTag("Player").GetComponent<Player>().Moves = moves;
    }
    #endregion
}
