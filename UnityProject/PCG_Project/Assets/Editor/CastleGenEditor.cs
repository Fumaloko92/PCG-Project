using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(CastleGenerator))]
public class CastleGenEditor : EditorWindow {

    Vector2 castleDimension = new Vector2(100, 100);
    Vector2 castlePosition = new Vector2(0, 0);

    int minNodeSize = 10;
    int maxNodeSize = 15;
    float constructProbability = 0.5F;
    int buildingCount = 10;

    bool waitMouseInput = false;

    //make the window
    [MenuItem("Window/Castle Generator")]
    static void Init()
    {
        CastleGenEditor window = (CastleGenEditor)EditorWindow.GetWindow(typeof(CastleGenEditor));
        window.Show();
    }

    void OnEnable()
    {
        SceneView.onSceneGUIDelegate += SceneGUI;
    }

    void OnGUI()
    {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        castleDimension = EditorGUILayout.Vector2Field("Dimensions", castleDimension);
        constructProbability = EditorGUILayout.Slider("Construction Probability",constructProbability, 0, 1);
        buildingCount = EditorGUILayout.IntField("Number of Constructions", buildingCount);

        GUILayout.Label("Node Size");
        minNodeSize = EditorGUILayout.IntField("Min", minNodeSize);
        maxNodeSize = EditorGUILayout.IntField("Max", maxNodeSize);

        GUILayout.Label("Castle", EditorStyles.boldLabel);
        if (GUILayout.Button("Generate"))
        {
            Debug.Log("clicked btn");
            waitMouseInput = true;
        }

        if(waitMouseInput)
        {
            GUILayout.Label("Press anywhere to place castle(Hold Ctrl to place multiply)", EditorStyles.boldLabel);
        }
    }

    void SceneGUI(SceneView sceneView)
    {
        if(!waitMouseInput)
        {
            return;
        }


        //Event e = Event.current;

        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            Debug.Log("clicked scene");
            

            Ray r  = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hit;
 
            if(Physics.Raycast(r, out hit, 1000)){
                castlePosition = new Vector2(hit.point.x, hit.point.z);
                //castlePosition = hit.point;
                Debug.Log(hit.point);
            } 

            //Vector3 mousePosition = Event.current.mousePosition;
            //mousePosition.y = sceneView.camera.pixelHeight - mousePosition.y;
            //mousePosition = sceneView.camera.ray.ScreenToWorldPoint(mousePosition);
            //mousePosition.y = -mousePosition.y;

            CastleGenerator cg = new CastleGenerator(castleDimension, buildingCount);

            cg.runEvolution();
            //cg.generateCastle(castleDimension, castlePosition, minNodeSize, maxNodeSize, constructProbability, buildingCount);

            if (!Event.current.control)
            {
                waitMouseInput = false;
            }
            Event.current.Use();
        }

        /*if(Event.current.type == EventType.KeyDown && Event.current.control)
        {
            controlBtnDown = true;
            Event.current.Use();
        }
        if(Event.current.type == EventType.KeyUp && Event.current.control)
        {
            controlBtnDown = false;
            Event.current.Use();
        }*/
    }
}
