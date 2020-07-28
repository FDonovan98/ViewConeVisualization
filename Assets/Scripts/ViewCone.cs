using UnityEngine;
using UnityEngine.UI;

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
    public float edgeIndent = 5.0f;

    int raysCastThisFrame;
    public Text text;

    private void Start()
    {
        if (meshFilter == null)
        {
            meshFilter = GetComponent<MeshFilter>();
        }
    }

    private void Update()
    {
        raysCastThisFrame = 0;

        List<HitInfo> hitInfo = new List<HitInfo>();
        GetCollisionPoints(ref hitInfo);

        CreateViewMesh(hitInfo);

        text.text = raysCastThisFrame.ToString();
    }

    void GetCollisionPoints(ref List<HitInfo> hitInfo)
    {
        for (int i = 0; i < rayCount; i++)
        {
            float rayAngle = -viewAngle / 2 + (viewAngle / rayCount) * i;
            rayAngle *= Mathf.Deg2Rad;

            Vector3 rayDirection = RotateVector(rayAngle, Vector3.up, transform.forward);
            
            Ray ray = new Ray(transform.position, rayDirection.normalized);

            RaycastHit hit;
            bool didHit = Physics.Raycast(ray, out hit, viewRange);
            raysCastThisFrame++;

            HitInfo newHit = SetHitInfo(didHit, rayAngle, hit, ray);

            if (i > 0 && (hitInfo[hitInfo.Count - 1].didHit ^ didHit))
            {
                AddCorner(ref hitInfo, hitInfo[hitInfo.Count - 1], new HitInfo(didHit, rayAngle, newHit.hitPosition));
            }

            hitInfo.Add(newHit);
        }
    }

    HitInfo SetHitInfo(bool rayDidHit, float angle, RaycastHit rayHit, Ray ray)
    {
        if (rayDidHit)
        {
           return new HitInfo(rayDidHit, angle, rayHit.point, rayHit.normal);
        }
        else
        {
            return new HitInfo(rayDidHit, angle, ray.GetPoint(viewRange));
        }
    }

    void AddCorner(ref List<HitInfo> hitInfo, HitInfo minBound, HitInfo maxBound)
    {
        for (int i = 0; i < edgeIterations; i++)
        {
            float angle = (minBound.angle + maxBound.angle) / 2;
            Vector3 newDirection = RotateVector(angle, Vector3.up, transform.forward);

            Ray ray = new Ray(transform.position, newDirection.normalized);

            RaycastHit hit;
            bool didHit = Physics.Raycast(ray, out hit, viewRange);
            raysCastThisFrame++;

            if (minBound.didHit == didHit)
            {
                minBound = SetHitInfo(didHit, angle, hit, ray);
            }
            else
            {
                maxBound = SetHitInfo(didHit, angle, hit, ray);
            }
        }

        hitInfo.Add(minBound);
        hitInfo.Add(maxBound);
    }

    void CreateViewMesh(List<HitInfo> hitInfo)
    {
        Vector3[] vertices = new Vector3[hitInfo.Count + 1];
        vertices[0] = Vector3.zero;

        int[] triangles = new int[3 * (vertices.Length - 2)];
        int triangleIndex = 0;

        for (int i = 0; i < hitInfo.Count; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(hitInfo[i].hitPosition - hitInfo[i].hitNormal * edgeIndent);

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
        public Vector3 hitNormal;

        public HitInfo(bool _didHit, float _angle, Vector3 _hitPosition, Vector3? _hitNormal = null)
        {
            didHit = _didHit;
            angle = _angle;
            hitPosition = _hitPosition;
            hitNormal = _hitNormal ?? Vector3.zero;
        }
    }
}