using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (MeshFilter), typeof (MeshRenderer))]
public class HexMesh : MonoBehaviour {

    private Mesh hexMesh;
    private MeshCollider meshCollider;
    private List<Vector3> vertices;
    private List<int> triangles;
    private List<Color> colors;

    private void Awake () {
        hexMesh = new Mesh ();
        hexMesh.name = "Hex Mesh";
        GetComponent<MeshFilter> ().mesh = hexMesh;
        meshCollider = gameObject.AddComponent<MeshCollider> ();

        vertices = new List<Vector3> ();
        triangles = new List<int> ();
        colors = new List<Color> ();
    }

    public void Triangulate (HexCell[] cells) {
        hexMesh.Clear ();
        vertices.Clear ();
        triangles.Clear ();
        colors.Clear ();

        for (int i = 0; i < cells.Length; ++i) {
            Triangulate (cells[i]);
        }

        hexMesh.vertices = vertices.ToArray ();
        hexMesh.triangles = triangles.ToArray ();
        hexMesh.colors = colors.ToArray ();
        hexMesh.RecalculateNormals ();
        meshCollider.sharedMesh = hexMesh;
    }

    private void Triangulate (HexCell cell) {
        Vector3 center = cell.transform.localPosition;
        for (int i = 0; i < 6; ++i) {
            AddTriangle (
                center,
                center + HexMetrics.corners[i],
                center + HexMetrics.corners[(i + 1) % 6]
            );
            AddTriangleColor (cell.color);
        }
    }

    private void AddTriangle (Vector3 v1, Vector3 v2, Vector3 v3) {
        int vertexIndex = vertices.Count;
        vertices.Add (v1);
        vertices.Add (v2);
        vertices.Add (v3);
        triangles.Add (vertexIndex);
        triangles.Add (vertexIndex + 1);
        triangles.Add (vertexIndex + 2);
    }

    private void AddTriangleColor (Color color) {
        colors.Add (color);
        colors.Add (color);
        colors.Add (color);
    }

}