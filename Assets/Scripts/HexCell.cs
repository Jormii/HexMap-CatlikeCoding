using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell : MonoBehaviour {

    public HexCoordinates coordinates;
    public Color color;
    public RectTransform uiRect;

    private int elevation;

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

    /*
    Properties
     */

    public int Elevation {
        get => elevation;
        set {
            elevation = value;
            Vector3 position = transform.localPosition;
            position.y = elevation * HexMetrics.elevationStep;
            transform.localPosition = position;

            Vector3 uiPosition = uiRect.localPosition;
            uiPosition.z = elevation * -HexMetrics.elevationStep;
            uiRect.localPosition = uiPosition;
        }
    }

}