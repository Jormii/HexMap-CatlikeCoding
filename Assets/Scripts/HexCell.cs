using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell : MonoBehaviour {

    public HexGridChunk chunk;
    public HexCoordinates coordinates;

    public RectTransform uiRect;

    int elevation = int.MinValue;
    Color color;

    [SerializeField]
    private HexCell[] neighbors = new HexCell[6];

    public HexEdgeType GetEdgeType (HexDirection direction) {
        return HexMetrics.GetEdgeType (
            elevation,
            neighbors[(int) direction].elevation
        );
    }

    public HexEdgeType GetEdgeType (HexCell otherCell) {
        return HexMetrics.GetEdgeType (elevation, otherCell.elevation);
    }

    public HexCell GetNeighbour (HexDirection direction) {
        return neighbors[(int) direction];
    }

    public void SetNeighbour (HexCell cell, HexDirection direction) {
        neighbors[(int) direction] = cell;
        cell.neighbors[(int) direction.Opposite ()] = this;
    }

    void Refresh () {
        if (chunk) {
            chunk.Refresh ();

            for (int i = 0; i < neighbors.Length; i++) {
                HexCell neighbor = neighbors[i];
                if (neighbor != null && neighbor.chunk != chunk) {
                    neighbor.chunk.Refresh ();
                }
            }
        }
    }

    /*
    Properties
    */

    public Vector3 Position {
        get {
            return transform.localPosition;
        }
    }

    public Color Color {
        get {
            return color;
        }
        set {
            if (color == value) {
                return;
            }
            color = value;
            Refresh ();
        }
    }

    public int Elevation {
        get => elevation;
        set {
            if (elevation == value) {
                return;
            }

            elevation = value;
            Vector3 position = transform.localPosition;
            position.y = elevation * HexMetrics.elevationStep;
            position.y +=
                (HexMetrics.SampleNoise (position).y * 2f - 1f) *
                HexMetrics.elevationPerturbStrength;
            transform.localPosition = position;

            Vector3 uiPosition = uiRect.localPosition;
            uiPosition.z = -position.y;
            uiRect.localPosition = uiPosition;

            Refresh ();
        }
    }

}