using System.Collections.Generic;
using UnityEngine;

public class ScreenSpaceDepthMeshGenerator : MonoBehaviour
{
    // Specifies the maximum distance between vertices of a single triangle to be rendered.
    private const float _triangleConnectivityCutOff = 1.0f;

    private static readonly Vector3 _defaultMeshOffset = new Vector3(-100, -100, -100);
    private static readonly string _vertexModelTransformPropertyName = "_VertexModelTransform";

    private Mesh _mesh;
    private bool _freezeMesh = false;
    private bool _initialized = false;
    private Texture2D _staticDepthTexture = null;

    public void FreezeDepthFrame()
    {
        _freezeMesh = true;
        _staticDepthTexture = DepthSource.GetDepthTextureSnapshot();

        Material material = GetComponent<Renderer>().material;
        material.SetTexture("_CurrentDepthTexture", _staticDepthTexture);
    }

    /// <summary>
    /// Resumes to update the depth texture on every frame.
    /// </summary>
    public void UnfreezeDepthFrame()
    {
        _freezeMesh = false;
        Material material = GetComponent<Renderer>().material;
        material.SetTexture("_CurrentDepthTexture", DepthSource.DepthTexture);
        Destroy(_staticDepthTexture);
        _staticDepthTexture = null;

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    private static int[] GenerateTriangles(int width, int height)
    {
        int[] indices = new int[(height - 1) * (width - 1) * 6];
        int idx = 0;
        for (int y = 0; y < (height - 1); y++)
        {
            for (int x = 0; x < (width - 1); x++)
            {
                //// Unity has a clockwise triangle winding order.
                //// Upper quad triangle
                //// Top left
                int idx0 = (y * width) + x;
                //// Top right
                int idx1 = idx0 + 1;
                //// Bottom left
                int idx2 = idx0 + width;

                //// Lower quad triangle
                //// Top right
                int idx3 = idx1;
                //// Bottom right
                int idx4 = idx2 + 1;
                //// Bottom left
                int idx5 = idx2;

                indices[idx++] = idx0;
                indices[idx++] = idx1;
                indices[idx++] = idx2;
                indices[idx++] = idx3;
                indices[idx++] = idx4;
                indices[idx++] = idx5;
            }
        }

        return indices;
    }

    private void InitializeMesh()
    {
        // Create template vertices.
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();

        // Create template vertices for the mesh object.
        for (int y = 0; y < DepthSource.DepthHeight; y++)
        {
            for (int x = 0; x < DepthSource.DepthWidth; x++)
            {
                Vector3 v = new Vector3(x * 0.01f, -y * 0.01f, 0) + _defaultMeshOffset;
                vertices.Add(v);
                normals.Add(Vector3.back);
            }
        }

        // Create template triangle list.
        int[] triangles = GenerateTriangles(DepthSource.DepthWidth, DepthSource.DepthHeight);

        // Create the mesh object and set all template data.
        _mesh = new Mesh();
        _mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        _mesh.SetVertices(vertices);
        _mesh.SetNormals(normals);
        _mesh.SetTriangles(triangles, 0);
        _mesh.bounds = new Bounds(Vector3.zero, new Vector3(50, 50, 50));
        _mesh.UploadMeshData(true);

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.sharedMesh = _mesh;

        // Sets camera intrinsics for depth reprojection.
        Material material = GetComponent<Renderer>().material;
        material.SetTexture("_CurrentDepthTexture", DepthSource.DepthTexture);
        material.SetFloat("_FocalLengthX", DepthSource.FocalLength.x);
        material.SetFloat("_FocalLengthY", DepthSource.FocalLength.y);
        material.SetFloat("_PrincipalPointX", DepthSource.PrincipalPoint.x);
        material.SetFloat("_PrincipalPointY", DepthSource.PrincipalPoint.y);
        material.SetInt("_ImageDimensionsX", DepthSource.ImageDimensions.x);
        material.SetInt("_ImageDimensionsY", DepthSource.ImageDimensions.y);
        material.SetFloat("_TriangleConnectivityCutOff", _triangleConnectivityCutOff);

        _initialized = true;
    }

    private void Update()
    {
        if (!_freezeMesh)
        {
            Material material = GetComponent<Renderer>().material;
            material.SetMatrix(_vertexModelTransformPropertyName, DepthSource.LocalToWorldMatrix);
        }

        if (!_initialized && DepthSource.Initialized)
        {
            InitializeMesh();
        }
    }
}