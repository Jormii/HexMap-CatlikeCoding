using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HexMapEditor : MonoBehaviour {

    public Color[] colors;
    public HexGrid hexGrid;

    private int activeElevation;
    private Color activeColor;
    int brushSize;
    bool applyColor;
    bool applyElevation = true;

    private void Awake () {
        activeElevation = 0;
    }

    public void SelectColor (int index) {
        applyColor = index >= 0;
        if (applyColor) {
            activeColor = colors[index];
        }
    }

    public void SetElevationToggle (Toggle toggle) {
        applyElevation = toggle.isOn;
    }

    public void SetElevation (Slider slider) {
        SetElevation (slider.value);
    }

    public void SetElevation (float elevation) {
        activeElevation = (int) elevation;
    }

    public void SetBrushSize (Slider brushSizeSlider) {
        SetBrushSize (brushSizeSlider.value);
    }

    void SetBrushSize (float size) {
        brushSize = (int) size;
    }

    public void ShowUI (Toggle toggle) {
        hexGrid.ShowUI (toggle.isOn);
    }

    private void Update () {
        if (Input.GetMouseButtonDown (0) && !EventSystem.current.IsPointerOverGameObject ()) {
            HandleInput ();
        }
    }

    private void HandleInput () {
        Ray inputRay = Camera.main.ScreenPointToRay (Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast (inputRay, out hit)) {
            EditCells (hexGrid.GetCell (hit.point));
        }
    }

    void EditCells (HexCell center) {
        int centerX = center.coordinates.X;
        int centerZ = center.coordinates.Z;

        for (int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++) {
            for (int x = centerX - r; x <= centerX + brushSize; x++) {
                EditCell (hexGrid.GetCell (new HexCoordinates (x, z)));
            }
        }

        for (int r = 0, z = centerZ + brushSize; z > centerZ; z--, r++) {
            for (int x = centerX - brushSize; x <= centerX + r; x++) {
                EditCell (hexGrid.GetCell (new HexCoordinates (x, z)));
            }
        }
    }

    private void EditCell (HexCell cell) {
        if (cell) {
            if (applyColor) {
                cell.Color = activeColor;
            }
            if (applyElevation) {
                cell.Elevation = activeElevation;
            }
        }
    }
}