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

            RaycastHit hit;
            bool didHit = Physics.Raycast(ray, out hit, viewRange);

            HitInfo newHit = SetHitInfo(didHit, rayAngle, hit, ray);

            if (i > 0 && (hitInfo[hitInfo.Count - 1].didHit ^ didHit))
            {
                AddCorner(ref hitInfo, hitInfo[hitInfo.Count - 1], new HitInfo(didHit, rayAngle, newHit.hitPosition));
                
                Debug.DrawRay(transform.position, newHit.hitPosition - transform.position, Color.red);

                Debug.DrawRay(transform.position, hitInfo[hitInfo.Count - 1].hitPosition - transform.position, Color.green);
            }

            // if (didHit)
            // {
            //     Debug.DrawRay(transform.position, newHit.hitPosition - transform.position, Color.red);
            // }
            // else
            // {
            //     Debug.DrawRay(transform.position, newHit.hitPosition - transform.position, Color.green);
            // }

            hitInfo.Add(newHit);
        }
    }

    HitInfo SetHitInfo(bool rayDidHit, float angle, RaycastHit rayHit, Ray ray)
    {
        if (rayDidHit)
        {
           return new HitInfo(rayDidHit, angle, rayHit.point);
        }
        else
        {
            return new HitInfo(rayDidHit, angle, ray.GetPoint(viewRange));
        }
    }

    void AddCorner(ref List<HitInfo> hitInfo, HitInfo minBound, HitInfo maxBound)
    {
        HitInfo hitInfoToAdd = null;
        for (int i = 0; i < edgeIterations; i++)
        {
            float angle = (minBound.angle + maxBound.angle) / 2;
            Vector3 newDirection = RotateVector(angle, Vector3.up, transform.forward);

            Ray ray = new Ray(transform.position, newDirection.normalized);

            RaycastHit hit;
            bool didHit = Physics.Raycast(ray, out hit, viewRange);

            if (didHit)
            {
                hitInfoToAdd = SetHitInfo(didHit, angle, hit, ray);
            }

            if (minBound.didHit == didHit)
            {
                minBound = SetHitInfo(didHit, angle, hit, ray);
            }
            else
            {
                maxBound = SetHitInfo(didHit, angle, hit, ray);
            }
        }

        if (hitInfoToAdd != null)
        {
            Debug.DrawRay(transform.position, minBound.hitPosition - transform.position, Color.yellow);
            Debug.DrawRay(transform.position, maxBound.hitPosition - transform.position, Color.blue);

            hitInfo.Add(minBound);
            hitInfo.Add(maxBound);
        }
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

    class HitInfo
    {
        public bool didHit;
        public float angle;
        public Vector3 hitPosition;

        public HitInfo(bool _didHit, float _angle, Vector3 _hitPosition)
        {
            didHit = _didHit;
            angle = _angle;
            hitPosition = _hitPosition;
        }
    }
}