using UnityEditor;
using UnityEngine;

namespace SacredTreeStudios.DungeonGeneration
{
    [CustomEditor(typeof(DungeonGenerator))]
    public class DungeonGeneratorEditor : Editor {

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            DungeonGenerator dungeonGenerator = (DungeonGenerator)target;
            if (GUILayout.Button("Generate Dungeon"))
            {
                dungeonGenerator.GenerateNewDungeon();
            }
        }
    }
}