using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Coordinates))]
public class SquareCoordinatesDrawer : PropertyDrawer {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		var coordinates = new Coordinates(
			property.FindPropertyRelative("x").intValue,
			property.FindPropertyRelative("z").intValue);
		position = EditorGUI.PrefixLabel(position, label);
		GUI.Label(position, coordinates.ToString());
	}

}
