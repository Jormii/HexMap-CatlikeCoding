using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct HexCoordinates {

    [SerializeField]
    private int xCoordinate, zCoordinate;

    public HexCoordinates (int x, int z) {
        xCoordinate = x;
        zCoordinate = z;
    }

    public static HexCoordinates FromOffsetCoordinates (int x, int z) {
        return new HexCoordinates (x - z / 2, z);
    }

    public static HexCoordinates FromPosition (Vector3 position) {
        float x = position.x / (HexMetrics.innerRadius * 2.0f);
        float y = -x;

        float offset = position.z / (HexMetrics.outerRadius * 3.0f);
        x -= offset;
        y -= offset;

        int iX = Mathf.RoundToInt (x);
        int iY = Mathf.RoundToInt (y);
        int iZ = Mathf.RoundToInt (-x - y);

        if (iX + iY + iZ != 0) {
            float dX = Mathf.Abs (x - iX);
            float dY = Mathf.Abs (y - iY);
            float dZ = Mathf.Abs (-x - y - iZ);

            if (dX > dY && dX > dZ) {
                iX = -iY - iZ;
            } else if (dZ > dY) {
                iZ = -iX - iY;
            }
        }

        return new HexCoordinates (iX, iZ);
    }

    public string ToStringOnSeparateLines () {
        return string.Format ("{0}\n{1}\n{2}", X, Y, Z);
    }

    public override string ToString () {
        return string.Format ("({0}, {1}, {2})", X, Y, Z);
    }

    /*
    Properties
     */

    public int X {
        get => xCoordinate;
    }

    public int Y {
        get => -xCoordinate - zCoordinate;
    }

    public int Z {
        get => zCoordinate;
    }

}