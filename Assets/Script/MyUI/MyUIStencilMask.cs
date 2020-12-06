using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class MyUIStencilMask : MonoBehaviour {
    public Bounds bound
    {
        get
        {
            return new Bounds(center, size);
        }
    }
    
    public Vector3 center = Vector3.zero;
    public Vector3 size = Vector3.one;
    public byte stencilValue;

    private bool isMaterialInstanciated = false;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private Material material;

    private MeshRenderer cachedMeshRenderer
    {
        get
        {
            if (meshRenderer == null)
                meshRenderer = GetComponent<MeshRenderer>();
            return meshRenderer;
        }
    }

    private MeshFilter cachedMeshFilter
    {
        get
        {
            if (meshFilter == null)
                meshFilter = GetComponent<MeshFilter>();
            return meshFilter;
        }
    }

    private Mesh mesh;

    private void Awake()
    {
        if(mesh == null)
            CreateMesh();
        if (cachedMeshRenderer.sharedMaterial == null)
            CreateMaterial();
    }

    private void OnDestroy()
    {
        if (isMaterialInstanciated)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                Destroy(cachedMeshRenderer.sharedMaterial);
            else
                DestroyImmediate(cachedMeshRenderer.sharedMaterial);
#else
        Destroy(cachedMeshRenderer.sharedMaterial);
#endif
        }
    }

    private void CreateMesh()
    {
        mesh = new Mesh();
        mesh.MarkDynamic();
        mesh.hideFlags = HideFlags.DontSave;
        cachedMeshFilter.mesh = mesh;
        
        UpdateMesh();
    }

    private void CreateMaterial()
    {
        isMaterialInstanciated = true;
        material = new Material(Shader.Find("Hidden/StencilWriter"));
        material.SetInt("_Stencil", stencilValue);
        material.hideFlags = HideFlags.DontSave;
        cachedMeshRenderer.material = material;
    }

    public void UpdateMaterialProp()
    {
        if (isMaterialInstanciated)
            material.SetInt("_Stencil", stencilValue);
    }

    private void UpdateMesh()
    {
        mesh.Clear();
        var halfSize = size * 0.5f;
        var right = halfSize.x * Vector3.right;
        var up = halfSize.y * Vector3.up;
        mesh.vertices = new Vector3[] { center - right + up, center + right + up, center - right - up, center + right - up };
        mesh.triangles = new int[] { 0, 2, 1, 1, 2, 3 };
    }

    public void Reshape(Vector3 dCenter, Vector3 dSize)
    {
        center += dCenter;
        size += dSize;

        if (mesh == null)
            CreateMesh();
        if (cachedMeshRenderer.sharedMaterial == null)
            CreateMaterial();

        UpdateMesh();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Color oldColor = Gizmos.color;

        Gizmos.matrix = transform.localToWorldMatrix;

        var halfSize = size * 0.5f;
        var right = halfSize.x * Vector3.right;
        var up = halfSize.y * Vector3.up;

        var upperLeft = center - right + up;
        var upperRight = center + right + up;
        var lowerLeft = center - right - up;
        var lowerRight = center + right - up;

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(upperLeft, upperRight);
        Gizmos.DrawLine(upperRight, lowerRight);
        Gizmos.DrawLine(lowerRight, lowerLeft);
        Gizmos.DrawLine(lowerLeft, upperLeft);

        Gizmos.matrix = oldMatrix;
        Gizmos.color = oldColor;
    }
#endif
}
