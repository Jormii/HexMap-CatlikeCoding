using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapEditor : MonoBehaviour {

    public Color[] colors;
    public HexGrid hexGrid;

    private Color activeColor;

    private void Awake () {
        SelectColor (colors.Length - 1);
    }

    public void SelectColor (int index) {
        activeColor = colors[index];
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
            hexGrid.ColorCell (hit.point, activeColor);
        }
    }
}