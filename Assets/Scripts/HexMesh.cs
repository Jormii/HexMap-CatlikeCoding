using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HexEdgeType {
    Flat,
    Slope,
    Cliff
}

[RequireComponent (typeof (MeshFilter), typeof (MeshRenderer))]
public class HexMesh : MonoBehaviour {

    static List<Vector3> vertices = new List<Vector3> ();
    static List<Color> colors = new List<Color> ();
    static List<int> triangles = new List<int> ();

    private Mesh hexMesh;
    private MeshCollider meshCollider;

    private void Awake () {
        hexMesh = new Mesh ();
        hexMesh.name = "Hex Mesh";
        GetComponent<MeshFilter> ().mesh = hexMesh;
        meshCollider = gameObject.AddComponent<MeshCollider> ();
    }

    public void Clear () {
        hexMesh.Clear ();
        vertices.Clear ();
        colors.Clear ();
        triangles.Clear ();
    }

    public void Apply () {
        hexMesh.SetVertices (vertices);
        hexMesh.SetColors (colors);
        hexMesh.SetTriangles (triangles, 0);
        hexMesh.RecalculateNormals ();
        meshCollider.sharedMesh = hexMesh;
    }

    public void AddTriangle (Vector3 v1, Vector3 v2, Vector3 v3) {
        int vertexIndex = vertices.Count;
        vertices.Add (HexMetrics.Perturb (v1));
        vertices.Add (HexMetrics.Perturb (v2));
        vertices.Add (HexMetrics.Perturb (v3));
        triangles.Add (vertexIndex);
        triangles.Add (vertexIndex + 1);
        triangles.Add (vertexIndex + 2);
    }

    public void AddTriangleUnperturbed (Vector3 v1, Vector3 v2, Vector3 v3) {
        int vertexIndex = vertices.Count;
        vertices.Add (v1);
        vertices.Add (v2);
        vertices.Add (v3);
        triangles.Add (vertexIndex);
        triangles.Add (vertexIndex + 1);
        triangles.Add (vertexIndex + 2);
    }

    public void AddTriangleColor (Color color) {
        colors.Add (color);
        colors.Add (color);
        colors.Add (color);
    }

    public void AddTriangleColor (Color color1, Color color2, Color color3) {
        colors.Add (color1);
        colors.Add (color2);
        colors.Add (color3);
    }

    public void AddQuad (Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4) {
        int vertexIndex = vertices.Count;
        vertices.Add (HexMetrics.Perturb (v1));
        vertices.Add (HexMetrics.Perturb (v2));
        vertices.Add (HexMetrics.Perturb (v3));
        vertices.Add (HexMetrics.Perturb (v4));
        triangles.Add (vertexIndex);
        triangles.Add (vertexIndex + 2);
        triangles.Add (vertexIndex + 1);
        triangles.Add (vertexIndex + 1);
        triangles.Add (vertexIndex + 2);
        triangles.Add (vertexIndex + 3);
    }

    public void AddQuadColor (Color color) {
        colors.Add (color);
        colors.Add (color);
        colors.Add (color);
        colors.Add (color);
    }

    public void AddQuadColor (Color c1, Color c2) {
        colors.Add (c1);
        colors.Add (c1);
        colors.Add (c2);
        colors.Add (c2);
    }

    public void AddQuadColor (Color c1, Color c2, Color c3, Color c4) {
        colors.Add (c1);
        colors.Add (c2);
        colors.Add (c3);
        colors.Add (c4);
    }

}