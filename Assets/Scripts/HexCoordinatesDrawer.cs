using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer (typeof (HexCoordinates))]
public class HexCoordinatesDrawer : PropertyDrawer {

    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
        HexCoordinates coordinates = new HexCoordinates (
            property.FindPropertyRelative ("xCoordinate").intValue,
            property.FindPropertyRelative ("zCoordinate").intValue
        );

        position = EditorGUI.PrefixLabel (position, label);
        GUI.Label (position, coordinates.ToString ());
    }

}