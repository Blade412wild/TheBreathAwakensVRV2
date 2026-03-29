using UnityEngine;

public class TerrainLayerChecker : MonoBehaviour
{
    [SerializeField] private TerrainLayerDetector terraindetector;
    [SerializeField] private TerrainCollider targetCollder;
    [SerializeField] private Terrain terrain;

    [SerializeField] private float groundDistance;
    [SerializeField] private bool shoot;

    TerrainLayer layer;

    TerrainCollider collider;
    private TerrainData data;
    private Vector3 defaultPoint = new Vector3(0, 1000, 0);


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        data = terrain.terrainData;
    }

    // Update is called once per frame
    void Update()
    {
        if (shoot)
        {
            shoot = false;
            ShootRay();
        }
    }

    public Vector3 ShootRay()
    {

        Ray hit;

        if (Physics.Raycast(transform.position, Vector3.down,out RaycastHit hitInfo ,groundDistance))
        {
            if(hitInfo.collider == targetCollder)
            {
                return hitInfo.point;
            }
            else
            {
                Debug.Log("No terraincollider");
                return defaultPoint;
            }
        }
        else
        {
            Debug.Log("No htt");
            return defaultPoint;
        }
    }
}
