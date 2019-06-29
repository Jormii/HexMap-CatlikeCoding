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

    private void Awake () {
        activeElevation = 0;
    }

    public void SelectColor (int index) {
        activeColor = colors[index];
    }

    public void SetElevation (Slider slider) {
        SetElevation (slider.value);
    }

    public void SetElevation (float elevation) {
        activeElevation = (int) elevation;
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
            EditCell (hexGrid.GetCell (hit.point));
        }
    }

    private void EditCell (HexCell cell) {
        cell.color = activeColor;
        cell.Elevation = activeElevation;
        hexGrid.Refresh ();
    }
}