using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(BricksLogic))]
public class BricksChange : Editor
{
    private static  string Name;
    private static  string Location;
    public override void OnInspectorGUI()
    {
        BricksLogic BL = (BricksLogic)target;
        /*GUILayout.Label("Name");
        Name = GUILayout.TextField(Name);
        GUILayout.Label("Location");
        Location = GUILayout.TextField(Location);
        GUILayout.Label(BL.BrickSet(Name, Location));*/

        DrawDefaultInspector();
    }
}
