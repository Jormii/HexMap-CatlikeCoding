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
        for (HexDirection direction = HexDirection.NE; direction <= HexDirection.NW; ++direction) {
            Triangulate (cell, direction);
        }
    }

    private void Triangulate (HexCell cell, HexDirection direction) {
        Vector3 center = cell.transform.localPosition;
        Vector3 v1 = center + HexMetrics.GetFirstSolidCorner (direction);
        Vector3 v2 = center + HexMetrics.GetSecondSolidCorner (direction);

        AddTriangle (center, v1, v2);
        AddTriangleColor (cell.color);

        Vector3 bridge = HexMetrics.GetBridge (direction);
        Vector3 v3 = center + HexMetrics.GetFirstCorner (direction);
        Vector3 v4 = center + HexMetrics.GetSecondCorner (direction);

        AddQuad (v1, v2, v3, v4);

        HexCell previousNeighbour = cell.GetNeighbour (direction.Previous ()) ?? cell;
        HexCell neighbour = cell.GetNeighbour (direction) ?? cell;
        HexCell nextNeighbour = cell.GetNeighbour (direction.Next ()) ?? cell;

        Color bridgeColor = (cell.color + neighbour.color) * 0.5f;
        AddQuadColor (cell.color, bridgeColor);

        AddTriangle (
            v1,
            center + HexMetrics.GetFirstCorner (direction),
            v3
        );
        AddTriangleColor (
            cell.color,
            (cell.color + previousNeighbour.color + neighbour.color) / 3.0f,
            bridgeColor
        );

        AddTriangle (v2, v4, center + HexMetrics.GetSecondCorner (direction));
        AddTriangleColor (
            cell.color,
            bridgeColor,
            (cell.color + neighbour.color + neighbour.color) / 3.0f
        );
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

    private void AddTriangleColor (Color color1, Color color2, Color color3) {
        colors.Add (color1);
        colors.Add (color2);
        colors.Add (color3);
    }

    private void AddQuad (Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4) {
        int vertexIndex = vertices.Count;
        vertices.Add (v1);
        vertices.Add (v2);
        vertices.Add (v3);
        vertices.Add (v4);
        triangles.Add (vertexIndex);
        triangles.Add (vertexIndex + 2);
        triangles.Add (vertexIndex + 1);
        triangles.Add (vertexIndex + 1);
        triangles.Add (vertexIndex + 2);
        triangles.Add (vertexIndex + 3);
    }

    void AddQuadColor (Color c1, Color c2, Color c3, Color c4) {
        colors.Add (c1);
        colors.Add (c2);
        colors.Add (c3);
        colors.Add (c4);
    }

    void AddQuadColor (Color c1, Color c2) {
        colors.Add (c1);
        colors.Add (c1);
        colors.Add (c2);
        colors.Add (c2);
    }

}