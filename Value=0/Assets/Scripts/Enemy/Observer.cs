using UnityEngine;

public class Observer : MonoBehaviour
{
    #region ==========Properties==========

    #endregion

    #region ==========Fields==========
    [SerializeField] private Vector2 pointA, pointB;
    [SerializeField] private float speed = 3f;
    private bool switching = false;
    private GameObject scanner;
    #endregion

    #region ==========Unity Methods==========
    private void Start()
    {
        Init();
    }

    private void Update()
    {
        Patrol();
    }
    #endregion

    #region ==========Methods==========
    public void Init()
    {
        scanner = this.transform.GetChild(0).gameObject;
        this.transform.SetPositionAndRotation(pointA, Quaternion.FromToRotation(Vector3.up, pointB - pointA));
    }

    private void Patrol()
    {
        if (GoTo(switching ? pointA : pointB))
            if (Turn(switching ? pointB : pointA))
                switching = !switching;
    }

    private bool GoTo(Vector2 des)
    {
        scanner.SetActive(true);

        if (Vector2.Distance(this.transform.position, des) < 0.1f)
        {
            this.transform.position = des;
            return true;
        }
        this.transform.position = Vector2.MoveTowards(this.transform.position, des, speed * Time.deltaTime);
        return false;
    }

    private bool Turn(Vector2 dir)
    {
        scanner.SetActive(false);

        if (Vector2.Angle(this.transform.up, dir - (Vector2)this.transform.position) < 5f)
        {
            this.transform.rotation = Quaternion.FromToRotation(Vector3.up, dir - (Vector2)this.transform.position);
            return true;
        }

        this.transform.Rotate(0, 0, 180 / (3f / speed) * Time.deltaTime);

        return false;
    }
    #endregion
}
