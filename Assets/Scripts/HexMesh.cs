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
        Vector3 center = cell.Position;
        EdgeVertices e = new EdgeVertices (
            center + HexMetrics.GetFirstSolidCorner (direction),
            center + HexMetrics.GetSecondSolidCorner (direction)
        );

        TriangulateEdgeFan (center, e, cell.color);

        if (direction <= HexDirection.SE) {
            TriangulateConnection (direction, cell, e);
        }
    }

    private void AddTriangle (Vector3 v1, Vector3 v2, Vector3 v3) {
        int vertexIndex = vertices.Count;
        vertices.Add (Perturb (v1));
        vertices.Add (Perturb (v2));
        vertices.Add (Perturb (v3));
        triangles.Add (vertexIndex);
        triangles.Add (vertexIndex + 1);
        triangles.Add (vertexIndex + 2);
    }

    void AddTriangleUnperturbed (Vector3 v1, Vector3 v2, Vector3 v3) {
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

    void TriangulateConnection (
        HexDirection direction, HexCell cell,
        EdgeVertices e
    ) {
        HexCell neighbour = cell.GetNeighbour (direction);
        if (neighbour == null) {
            return;
        }

        Vector3 bridge = HexMetrics.GetBridge (direction);
        bridge.y = neighbour.Position.y - cell.Position.y;
        EdgeVertices e2 = new EdgeVertices (
            e.v1 + bridge,
            e.v4 + bridge
        );

        if (cell.GetEdgeType (direction) == HexEdgeType.Slope) {
            TriangulateEdgeTerraces (e, cell, e2, neighbour);
        } else {
            TriangulateEdgeStrip (e, cell.color, e2, neighbour.color);
        }

        HexCell nextNeighbour = cell.GetNeighbour (direction.Next ());
        if (direction <= HexDirection.E && nextNeighbour != null) {
            Vector3 v5 = e.v4 + HexMetrics.GetBridge (direction.Next ());
            v5.y = nextNeighbour.Position.y;

            if (cell.Elevation <= neighbour.Elevation) {
                if (cell.Elevation <= nextNeighbour.Elevation) {
                    TriangulateCorner (e.v4, cell, e2.v4, neighbour, v5, nextNeighbour);
                } else {
                    TriangulateCorner (v5, nextNeighbour, e.v4, cell, e2.v4, neighbour);
                }
            } else if (neighbour.Elevation <= nextNeighbour.Elevation) {
                TriangulateCorner (e2.v4, neighbour, v5, nextNeighbour, e.v4, cell);
            } else {
                TriangulateCorner (v5, nextNeighbour, e.v4, cell, e2.v4, neighbour);
            }
        }
    }

    private void AddQuad (Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4) {
        int vertexIndex = vertices.Count;
        vertices.Add (Perturb (v1));
        vertices.Add (Perturb (v2));
        vertices.Add (Perturb (v3));
        vertices.Add (Perturb (v4));
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

    void TriangulateEdgeFan (Vector3 center, EdgeVertices edge, Color color) {
        AddTriangle (center, edge.v1, edge.v2);
        AddTriangleColor (color);
        AddTriangle (center, edge.v2, edge.v3);
        AddTriangleColor (color);
        AddTriangle (center, edge.v3, edge.v4);
        AddTriangleColor (color);
    }

    void TriangulateEdgeStrip (
        EdgeVertices e1, Color c1,
        EdgeVertices e2, Color c2
    ) {
        AddQuad (e1.v1, e1.v2, e2.v1, e2.v2);
        AddQuadColor (c1, c2);
        AddQuad (e1.v2, e1.v3, e2.v2, e2.v3);
        AddQuadColor (c1, c2);
        AddQuad (e1.v3, e1.v4, e2.v3, e2.v4);
        AddQuadColor (c1, c2);
    }

    void TriangulateEdgeTerraces (
        EdgeVertices begin, HexCell beginCell,
        EdgeVertices end, HexCell endCell
    ) {
        EdgeVertices e2 = EdgeVertices.TerraceLerp (begin, end, 1);
        Color c2 = HexMetrics.TerraceLerp (beginCell.color, endCell.color, 1);

        TriangulateEdgeStrip (begin, beginCell.color, e2, c2);

        for (int i = 2; i < HexMetrics.terraceSteps; i++) {
            EdgeVertices e1 = e2;
            Color c1 = c2;
            e2 = EdgeVertices.TerraceLerp (begin, end, i);
            c2 = HexMetrics.TerraceLerp (beginCell.color, endCell.color, i);
            TriangulateEdgeStrip (e1, c1, e2, c2);
        }

        TriangulateEdgeStrip (e2, c2, end, endCell.color);
    }

    void TriangulateCorner (
        Vector3 bottom, HexCell bottomCell,
        Vector3 left, HexCell leftCell,
        Vector3 right, HexCell rightCell
    ) {
        HexEdgeType leftEdgeType = bottomCell.GetEdgeType (leftCell);
        HexEdgeType rightEdgeType = bottomCell.GetEdgeType (rightCell);

        if (leftEdgeType == HexEdgeType.Slope) {
            if (rightEdgeType == HexEdgeType.Slope) {
                TriangulateCornerTerraces (
                    bottom, bottomCell, left, leftCell, right, rightCell
                );
            } else if (rightEdgeType == HexEdgeType.Flat) {
                TriangulateCornerTerraces (
                    left, leftCell, right, rightCell, bottom, bottomCell
                );
            } else {
                TriangulateCornerTerracesCliff (
                    bottom, bottomCell, left, leftCell, right, rightCell
                );
            }
        } else if (rightEdgeType == HexEdgeType.Slope) {
            if (leftEdgeType == HexEdgeType.Flat) {
                TriangulateCornerTerraces (
                    right, rightCell, bottom, bottomCell, left, leftCell
                );
            } else {
                TriangulateCornerCliffTerraces (
                    bottom, bottomCell, left, leftCell, right, rightCell
                );
            }
        } else if (leftCell.GetEdgeType (rightCell) == HexEdgeType.Slope) {
            if (leftCell.Elevation < rightCell.Elevation) {
                TriangulateCornerCliffTerraces (
                    right, rightCell, bottom, bottomCell, left, leftCell
                );
            } else {
                TriangulateCornerTerracesCliff (
                    left, leftCell, right, rightCell, bottom, bottomCell
                );
            }
        } else {
            AddTriangle (bottom, left, right);
            AddTriangleColor (bottomCell.color, leftCell.color, rightCell.color);
        }
    }

    void TriangulateCornerTerraces (
        Vector3 begin, HexCell beginCell,
        Vector3 left, HexCell leftCell,
        Vector3 right, HexCell rightCell
    ) {
        Vector3 v3 = HexMetrics.TerraceLerp (begin, left, 1);
        Vector3 v4 = HexMetrics.TerraceLerp (begin, right, 1);
        Color c3 = HexMetrics.TerraceLerp (beginCell.color, leftCell.color, 1);
        Color c4 = HexMetrics.TerraceLerp (beginCell.color, rightCell.color, 1);

        AddTriangle (begin, v3, v4);
        AddTriangleColor (beginCell.color, c3, c4);

        for (int i = 2; i < HexMetrics.terraceSteps; i++) {
            Vector3 v1 = v3;
            Vector3 v2 = v4;
            Color c1 = c3;
            Color c2 = c4;
            v3 = HexMetrics.TerraceLerp (begin, left, i);
            v4 = HexMetrics.TerraceLerp (begin, right, i);
            c3 = HexMetrics.TerraceLerp (beginCell.color, leftCell.color, i);
            c4 = HexMetrics.TerraceLerp (beginCell.color, rightCell.color, i);
            AddQuad (v1, v2, v3, v4);
            AddQuadColor (c1, c2, c3, c4);
        }

        AddQuad (v3, v4, left, right);
        AddQuadColor (c3, c4, leftCell.color, rightCell.color);
    }

    void TriangulateCornerTerracesCliff (
        Vector3 begin, HexCell beginCell,
        Vector3 left, HexCell leftCell,
        Vector3 right, HexCell rightCell
    ) {
        float b = 1f / (rightCell.Elevation - beginCell.Elevation);
        if (b < 0) {
            b = -b;
        }
        Vector3 boundary = Vector3.Lerp (Perturb (begin), Perturb (right), b);
        Color boundaryColor = Color.Lerp (beginCell.color, rightCell.color, b);

        TriangulateBoundaryTriangle (
            begin, beginCell, left, leftCell, boundary, boundaryColor
        );

        if (leftCell.GetEdgeType (rightCell) == HexEdgeType.Slope) {
            TriangulateBoundaryTriangle (
                left, leftCell, right, rightCell, boundary, boundaryColor
            );
        } else {
            AddTriangleUnperturbed (Perturb (left), Perturb (right), boundary);
            AddTriangleColor (leftCell.color, rightCell.color, boundaryColor);
        }
    }

    void TriangulateCornerCliffTerraces (
        Vector3 begin, HexCell beginCell,
        Vector3 left, HexCell leftCell,
        Vector3 right, HexCell rightCell
    ) {
        float b = 1f / (leftCell.Elevation - beginCell.Elevation);
        if (b < 0) {
            b = -b;
        }
        Vector3 boundary = Vector3.Lerp (Perturb (begin), Perturb (left), b);
        Color boundaryColor = Color.Lerp (beginCell.color, leftCell.color, b);

        TriangulateBoundaryTriangle (
            right, rightCell, begin, beginCell, boundary, boundaryColor
        );

        if (leftCell.GetEdgeType (rightCell) == HexEdgeType.Slope) {
            TriangulateBoundaryTriangle (
                left, leftCell, right, rightCell, boundary, boundaryColor
            );
        } else {
            AddTriangleUnperturbed (Perturb (left), Perturb (right), boundary);
            AddTriangleColor (leftCell.color, rightCell.color, boundaryColor);
        }
    }

    void TriangulateBoundaryTriangle (
        Vector3 begin, HexCell beginCell,
        Vector3 left, HexCell leftCell,
        Vector3 boundary, Color boundaryColor
    ) {
        Vector3 v2 = Perturb (HexMetrics.TerraceLerp (begin, left, 1));
        Color c2 = HexMetrics.TerraceLerp (beginCell.color, leftCell.color, 1);

        AddTriangleUnperturbed (Perturb (begin), v2, boundary);
        AddTriangleColor (beginCell.color, c2, boundaryColor);

        for (int i = 2; i < HexMetrics.terraceSteps; i++) {
            Vector3 v1 = v2;
            Color c1 = c2;
            v2 = Perturb (HexMetrics.TerraceLerp (begin, left, i));
            c2 = HexMetrics.TerraceLerp (beginCell.color, leftCell.color, i);
            AddTriangleUnperturbed (v1, v2, boundary);
            AddTriangleColor (c1, c2, boundaryColor);
        }

        AddTriangleUnperturbed (v2, Perturb (left), boundary);
        AddTriangleColor (c2, leftCell.color, boundaryColor);
    }

    Vector3 Perturb (Vector3 position) {
        Vector4 sample = HexMetrics.SampleNoise (position);
        position.x += (sample.x * 2.0f - 1.0f) * HexMetrics.cellPerturbStrength;
        // position.y += (sample.y * 2.0f - 1.0f) * HexMetrics.cellPerturbStrength;
        position.z += (sample.z * 2.0f - 1.0f) * HexMetrics.cellPerturbStrength;
        return position;
    }
}