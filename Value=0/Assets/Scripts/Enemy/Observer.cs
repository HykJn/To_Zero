using UnityEngine;

public class Observer : MonoBehaviour
{
    #region ==========Properties==========

    #endregion

    #region ==========Fields==========
    [SerializeField] private Vector3 pointA, pointB;
    [SerializeField] private float speed = 3f;
    [Header("Scanner")]
    [SerializeField] private float width;
    private bool switching = false;
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
        DrawMesh();
        this.transform.position = pointA;
        this.transform.LookAt(pointB);
    }

    void Patrol()
    {
        if (switching)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, pointA, speed * Time.deltaTime);
        }
        else
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, pointB, speed * Time.deltaTime);
        }

        if (Vector3.Distance(this.transform.position, pointA) < 0.1f)
        {
            switching = false;
        }
        else if (Vector3.Distance(this.transform.position, pointB) < 0.1f)
        {
            switching = true;
        }
    }

    void DrawMesh()
    {
        MeshFilter filter = this.transform.GetChild(0).GetComponent<MeshFilter>();
        MeshRenderer renderer = this.transform.GetChild(0).GetComponent<MeshRenderer>();
        MeshCollider col = this.transform.GetChild(0).GetComponent<MeshCollider>();

        Mesh mesh = new Mesh();
        mesh.Clear();

        float x = this.transform.position.x;
        float y = pointA.y;
        float z = this.transform.position.z;
        Vector3[] vertices =
        {
            this.transform.position,
            new(x + width/2, 0.5f - y, z + 0.5f),
            new(x + width/2, 0.5f - y, z - 0.5f),
            new(x - width/2, 0.5f - y, z - 0.5f),
            new(x - width/2, 0.5f - y, z + 0.5f),
        };

        int[] indices =
        {
            0, 1, 2,
            0, 2, 3,
            0, 3, 4,
            0, 4, 1,
            1, 4, 3,
            1, 3, 2,
        };

        mesh.vertices = vertices;
        mesh.triangles = indices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        filter.mesh = mesh;
        col.sharedMesh = mesh;
    }
    #endregion
}
