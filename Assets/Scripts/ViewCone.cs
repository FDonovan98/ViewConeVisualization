using UnityEngine;

using static UnityMiscUtils.VectorRotation;

using System.Collections.Generic;

public class ViewCone : MonoBehaviour
{
    public MeshFilter meshFilter = null;
    [Range(0.0f, 360.0f)]
    public float viewAngle = 60.0f;
    public float viewRange = 100.0f;
    public int rayCount = 10;

    public int edgeIterations = 6;

    private void Start()
    {
        if (meshFilter == null)
        {
            meshFilter = GetComponent<MeshFilter>();
        }
    }

    private void Update()
    {
        List<HitInfo> hitInfo = new List<HitInfo>();
        GetCollisionPoints(ref hitInfo);

        CreateViewMesh(hitInfo);
    }

    void GetCollisionPoints(ref List<HitInfo> hitInfo)
    {
        for (int i = 0; i < rayCount + 1; i++)
        {
            float rayAngle = -viewAngle / 2 + (viewAngle / rayCount) * i;
            rayAngle *= Mathf.Deg2Rad;

            Vector3 rayDirection = RotateVector(rayAngle, Vector3.up, transform.forward);
            
            Ray ray = new Ray(transform.position, rayDirection.normalized);

            Debug.DrawRay(transform.position, rayDirection * viewRange, Color.red, Time.deltaTime);

            RaycastHit hit;
            bool didHit = Physics.Raycast(ray, out hit, viewRange);

            Vector3 hitPosition;
            if (didHit)
            {
                hitPosition = hit.point;
                Debug.DrawRay(transform.position, hitPosition - transform.position, Color.green, Time.deltaTime);
            }
            else
            {
                hitPosition = ray.GetPoint(viewRange);
                Debug.DrawRay(transform.position, hitPosition - transform.position, Color.yellow, Time.deltaTime);
            }

            // if (i > 0 && (hitInfo[hitInfo.Count - 1].didHit ^ didHit))
            // {
            //     hitInfo.Add(FindEdge(new HitInfo(didHit, hitPosition), hitInfo[hitInfo.Count - 1]));
            // }

            hitInfo.Add(new HitInfo(didHit, hitPosition));
        }
    }

    HitInfo FindEdge(HitInfo minBound, HitInfo maxBound)
    {
        HitInfo returnedInfo = new HitInfo();
        for (int i = 0; i < edgeIterations; i++)
        {
            Vector3 newDirection = Vector3.Lerp(minBound.hitPosition, maxBound.hitPosition, 0.5f);

            
        }

        return returnedInfo;
    }

    void CreateViewMesh(List<HitInfo> hitInfo)
    {
        Vector3[] vertices = new Vector3[hitInfo.Count + 1];
        vertices[0] = Vector3.zero;

        int[] triangles = new int[3 * (vertices.Length - 2)];
        int triangleIndex = 0;

        for (int i = 0; i < hitInfo.Count; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(hitInfo[i].hitPosition);

            if (i >= 1)
            {
                triangles[triangleIndex] = 0;
                triangles[triangleIndex + 1] = i;
                triangles[triangleIndex + 2] = i + 1;

                triangleIndex += 3;
            }
        }

        Mesh viewMesh = new Mesh();
        meshFilter.mesh = viewMesh;

        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
    }

    struct HitInfo
    {
        public bool didHit;
        public Vector3 hitPosition;

        public HitInfo(bool _didHit, Vector3 _hitPosition)
        {
            didHit = _didHit;
            hitPosition = _hitPosition;
        }
    }
}