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

            RaycastHit hit;
            bool didHit = Physics.Raycast(transform.position, rayDirection, out hit, viewRange);

            Vector3 hitPosition;
            if (didHit)
            {
                hitPosition = hit.point;
            }
            else
            {
                hitPosition = rayDirection * viewRange;
            }

            hitInfo.Add(new HitInfo(didHit, hitPosition, rayAngle));

            Debug.DrawRay(transform.position, rayDirection * hit.distance);
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

    struct HitInfo
    {
        public bool didHit;
        public Vector3 hitPosition;
        public float angle;

        public HitInfo(bool _didHit, Vector3 _hitPosition, float _angle)
        {
            didHit = _didHit;
            angle = _angle;
            hitPosition = _hitPosition;
        }
    }
}