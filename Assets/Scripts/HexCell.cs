using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell : MonoBehaviour {

    public HexCoordinates coordinates;
    public Color color;

    [SerializeField]
    private HexCell[] neighbors = new HexCell[6];

    public HexCell GetNeighbour (HexDirection direction) {
        return neighbors[(int) direction];
    }

    public void SetNeighbour (HexCell cell, HexDirection direction) {
        neighbors[(int) direction] = cell;
        cell.neighbors[(int) direction.Opposite ()] = this;
    }

}