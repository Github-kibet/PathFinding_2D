using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class TileMapTest : MonoBehaviour
{
    public Tilemap tileMap;


    public void Reset()
    {
        foreach (var pos in tileMap.cellBounds.allPositionsWithin)
        {
            if (tileMap.HasTile(pos))
            {
                tileMap.SetColor(pos, Color.white);
            }
        }

    }
    public void CheckSize()
    {
        Debug.Log(tileMap.size);
    }

    public void CheckTileCoord(Vector3Int pos)
    {
        tileMap.SetColor(pos, Color.green);
    }

    public void CheckTile()
    {
        foreach(var pos in tileMap.cellBounds.allPositionsWithin)
        {
            if (tileMap.HasTile(pos))
            {
                tileMap.SetColor(pos, Color.green);
            }       
        }    
      
    }
   
}

[CustomEditor(typeof(TileMapTest))]
public class TileMapTestEditor: Editor
{
    Vector3Int pos = new Vector3Int();
    public override void OnInspectorGUI()
    {
        TileMapTest tm = (TileMapTest)target;
        tm.tileMap = EditorGUILayout.ObjectField("TileMap Source",tm.tileMap,typeof(Tilemap),true) as Tilemap;

        if (GUILayout.Button("Check Size"))
            tm.CheckSize();

        if (GUILayout.Button("Check Tile"))
            tm.CheckTile();

        if (GUILayout.Button("Reset"))
            tm.Reset();

     
        pos=EditorGUILayout.Vector3IntField("Tile Coord",pos);
        if (GUILayout.Button("Check Tile Coord"))
            tm.CheckTileCoord(pos);
    }

}
