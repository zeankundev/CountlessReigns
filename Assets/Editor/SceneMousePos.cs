using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class SceneMousePos
{
    static SceneMousePos()
    {
        // Unsubscribe first to avoid duplicate registrations during reloads
        SceneView.duringSceneGui -= OnSceneGUI;
        SceneView.duringSceneGui += OnSceneGUI;
    }

    static void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;

        // 1. Only run logic on MouseDown (left click = 0)
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            // 2. Account for High-DPI displays (Pixels vs Points)
            float ppx = EditorGUIUtility.pixelsPerPoint;
            Vector2 mousePos = e.mousePosition * ppx;
            float correctedY = sceneView.camera.pixelHeight - mousePos.y;

            // 3. Convert to World Position
            // We use a dummy Z depth of 10 just to see it in front of the camera
            Vector3 worldPos = sceneView.camera.ScreenToWorldPoint(new Vector3(mousePos.x, correctedY, 10f));

            Debug.Log($"Scene Clicked at: {worldPos}");

            // 4. Consume the event so Unity doesn't select an object behind your click
            // Remove this line if you still want to be able to select objects while logging
            // e.Use(); 
        }
    }
}