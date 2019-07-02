using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell : MonoBehaviour {

    public HexGridChunk chunk;
    public HexCoordinates coordinates;

    public RectTransform uiRect;

    int elevation = int.MinValue;
    bool hasIncomingRiver, hasOutgoingRiver;
    HexDirection incomingRiver, outgoingRiver;
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

    public void SetOutgoingRiver (HexDirection direction) {
        if (hasOutgoingRiver && outgoingRiver == direction) {
            return;
        }

        HexCell neighbor = GetNeighbour (direction);
        if (!neighbor || elevation < neighbor.elevation) {
            return;
        }

        RemoveOutgoingRiver ();
        if (hasIncomingRiver && incomingRiver == direction) {
            RemoveIncomingRiver ();
        }

        hasOutgoingRiver = true;
        outgoingRiver = direction;
        RefreshSelfOnly ();

        neighbor.RemoveIncomingRiver ();
        neighbor.hasIncomingRiver = true;
        neighbor.incomingRiver = direction.Opposite ();
        neighbor.RefreshSelfOnly ();
    }

    public void RemoveRiver () {
        RemoveOutgoingRiver ();
        RemoveIncomingRiver ();
    }

    public void RemoveIncomingRiver () {
        if (!hasIncomingRiver) {
            return;
        }
        hasIncomingRiver = false;
        RefreshSelfOnly ();

        HexCell neighbor = GetNeighbour (incomingRiver);
        neighbor.hasOutgoingRiver = false;
        neighbor.RefreshSelfOnly ();
    }

    public void RemoveOutgoingRiver () {
        if (!hasOutgoingRiver) {
            return;
        }
        hasOutgoingRiver = false;
        RefreshSelfOnly ();

        HexCell neighbor = GetNeighbour (outgoingRiver);
        neighbor.hasIncomingRiver = false;
        neighbor.RefreshSelfOnly ();
    }

    void RefreshSelfOnly () {
        chunk.Refresh ();
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

            if (hasOutgoingRiver && elevation < GetNeighbour (outgoingRiver).elevation) {
                RemoveOutgoingRiver ();
            }
            if (hasIncomingRiver && elevation > GetNeighbour (incomingRiver).elevation) {
                RemoveIncomingRiver ();
            }

            Refresh ();
        }
    }

    public bool HasIncomingRiver {
        get {
            return hasIncomingRiver;
        }
    }

    public bool HasOutgoingRiver {
        get {
            return hasOutgoingRiver;
        }
    }

    public HexDirection IncomingRiver {
        get {
            return incomingRiver;
        }
    }

    public HexDirection OutgoingRiver {
        get {
            return outgoingRiver;
        }
    }

    public bool HasRiver {
        get {
            return hasIncomingRiver || hasOutgoingRiver;
        }
    }

    public bool HasRiverBeginOrEnd {
        get {
            return hasIncomingRiver != hasOutgoingRiver;
        }
    }

    public bool HasRiverThroughEdge (HexDirection direction) {
        return hasIncomingRiver && incomingRiver == direction ||
            hasOutgoingRiver && outgoingRiver == direction;
    }

    public float StreamBedY {
        get {
            return (elevation + HexMetrics.streamBedElevationOffset) *
                HexMetrics.elevationStep;
        }
    }

    public float RiverSurfaceY {
        get {
            return (elevation + HexMetrics.riverSurfaceElevationOffset) *
                HexMetrics.elevationStep;
        }
    }

}