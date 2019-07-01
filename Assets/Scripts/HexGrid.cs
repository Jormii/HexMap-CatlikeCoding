using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour {

    public int chunkCountX = 4, chunkCountZ = 3;

    public Color defaultColor = Color.white;
    public HexCell cellPrefab;
    public HexGridChunk chunkPrefab;
    public Text cellLabelPrefab;
    public Texture2D noiseSource;

    int cellCountX, cellCountZ;
    HexGridChunk[] chunks;
    private HexCell[] cells;

    private void OnEnable () {
        HexMetrics.noiseSource = noiseSource;
    }

    private void Awake () {
        HexMetrics.noiseSource = noiseSource;

        cellCountX = chunkCountX * HexMetrics.chunkSizeX;
        cellCountZ = chunkCountZ * HexMetrics.chunkSizeZ;

        CreateChunks ();
        CreateCells ();
    }

    void CreateChunks () {
        chunks = new HexGridChunk[chunkCountX * chunkCountZ];

        for (int z = 0, i = 0; z < chunkCountZ; z++) {
            for (int x = 0; x < chunkCountX; x++) {
                HexGridChunk chunk = chunks[i++] = Instantiate (chunkPrefab);
                chunk.transform.SetParent (transform);
            }
        }
    }

    void CreateCells () {
        cells = new HexCell[cellCountZ * cellCountX];

        for (int z = 0, i = 0; z < cellCountZ; z++) {
            for (int x = 0; x < cellCountX; x++) {
                CreateCell (x, z, i++);
            }
        }
    }

    private void CreateCell (int x, int z, int i) {
        Vector3 position = new Vector3 ();
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0.0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        HexCell cell = Instantiate<HexCell> (cellPrefab);
        cells[i] = cell;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates (x, z);
        cell.Color = defaultColor;
        cell.transform.localPosition = position;
        if (x > 0) {
            cell.SetNeighbour (cells[i - 1], HexDirection.W);
        }

        if (z > 0) {
            if ((z & 1) == 0) {
                cell.SetNeighbour (cells[i - cellCountX], HexDirection.SE);

                if (x > 0) {
                    cell.SetNeighbour (cells[i - cellCountX - 1], HexDirection.SW);
                }
            } else {
                cell.SetNeighbour (cells[i - cellCountX], HexDirection.SW);
                if (x < cellCountX - 1) {
                    cell.SetNeighbour (cells[i - cellCountX + 1], HexDirection.SE);
                }
            }
        }

        Text label = Instantiate<Text> (cellLabelPrefab);
        label.rectTransform.anchoredPosition = new Vector2 (position.x, position.z);
        label.text = cell.coordinates.ToStringOnSeparateLines ();
        cell.uiRect = label.rectTransform;

        cell.Elevation = 0;
        AddCellToChunk (x, z, cell);
    }

    void AddCellToChunk (int x, int z, HexCell cell) {
        int chunkX = x / HexMetrics.chunkSizeX;
        int chunkZ = z / HexMetrics.chunkSizeZ;
        HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

        int localX = x - chunkX * HexMetrics.chunkSizeX;
        int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
        chunk.AddCell (localX + localZ * HexMetrics.chunkSizeX, cell);
    }

    public HexCell GetCell (Vector3 position) {
        position = transform.InverseTransformPoint (position);
        HexCoordinates coordinates = HexCoordinates.FromPosition (position);

        int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
        return cells[index];
    }

    public HexCell GetCell (HexCoordinates coordinates) {
        int z = coordinates.Z;
        if (z < 0 || z >= cellCountZ) {
            return null;
        }

        int x = coordinates.X + z / 2;
        if (x < 0 || x >= cellCountX) {
            return null;
        }

        return cells[x + z * cellCountX];
    }

    public void ShowUI (bool visible) {
        for (int i = 0; i < chunks.Length; i++) {
            chunks[i].ShowUI (visible);
        }
    }

}