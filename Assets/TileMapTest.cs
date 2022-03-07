using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class TileMapTest : MonoBehaviour
{
    public Tilemap tileMap;
    public TileBase tileBase;

    public GameObject player;
    public GameObject enemy;


    private List<Vector3Int> List_Ground = new List<Vector3Int>();
    private Grid tileMapGrid;

    private Vector3Int[] Dir = new Vector3Int[4]  {new Vector3Int(0, 1, 0), new Vector3Int(1, 0, 0), new Vector3Int(0, -1, 0), new Vector3Int(-1, 0, 0) };

    private Queue<Vector3Int> Q_Root = new Queue<Vector3Int>();
    private List<Vector3Int> List_Root = new List<Vector3Int>();

    private bool[,] visit;

    private void Start()
    {
        
    }
    public void Reset()
    {
        foreach (var pos in tileMap.cellBounds.allPositionsWithin)
        {
            if (tileMap.HasTile(pos))
            {
                Vector3Int local = List_Ground.Where(x => x == pos).FirstOrDefault();

                if(local!=null)
                {
                    tileMap.SetTile(local, null);
                }
                else
                    tileMap.SetColor(pos, Color.white);
            }
        }

        Q_Root.Clear();
        List_Root.Clear();
        List_Ground.Clear();
    }
    public void CheckSize()
    {
       
    }

    public void CheckTileCoord(Vector3Int pos)
    {
        tileMap.SetColor(pos, Color.green);
    }

    public void CheckTile()
    {
        Reset();
        foreach(var pos in tileMap.cellBounds.allPositionsWithin)
        {
            bool isGround = false;
            if (tileMap.HasTile(pos))
            {
                

                if(pos.x!=tileMap.cellBounds.xMin&&pos.x!=tileMap.cellBounds.xMax-1&&pos.y!=tileMap.cellBounds.yMax-1)
                {
                    isGround = true;
                    Vector3Int nowPos = pos;
                    nowPos.y += 1;
                    List_Ground.Add(nowPos);
                    if (pos.y!=tileMap.cellBounds.yMin)
                    {
                        Vector3Int downYPos = List_Ground.Where(x => x == pos).FirstOrDefault();
                        if(downYPos!=null)
                        {
                            List_Ground.Remove(downYPos);
                        }
                    }
                   
                }

                if(!isGround)
                tileMap.SetColor(pos, Color.green);
      
              
            }       
        }
        
        foreach(var pos in List_Ground)
        {
            tileMap.SetTile(pos, tileBase);
        }
      
    }

    public void PathFinding()
    {
        Reset();
        CheckTile();
        CheckPlayerNode();

    }



    private void CheckPlayerNode()
    {
        tileMapGrid = tileMap.layoutGrid;
        

        Vector3Int playerGridPos = tileMapGrid.WorldToCell(player.transform.position);
        tileMap.SetColor(playerGridPos, Color.black);

        Vector3Int enemyGridPos = tileMapGrid.WorldToCell(enemy.transform.position);
        tileMap.SetColor(enemyGridPos, Color.black);

        RootPath(playerGridPos,enemyGridPos);
    }

    private void RootPath(Vector3Int playerPos, Vector3Int enemyPos)
    {
        visit = new bool[tileMap.cellBounds.xMax-tileMap.cellBounds.xMin, tileMap.cellBounds.yMax - tileMap.cellBounds.yMin];

        bool isFind = false;

        if(playerPos!=enemyPos)
            Q_Root.Enqueue(enemyPos);
        else
            return;

        Debug.Log("player" + playerPos);
        Debug.Log("enemy" + enemyPos);

        while (!isFind&&Q_Root.Count!=0)
        {
            Vector3Int nowPos = Q_Root.Dequeue();
            for(int i=0;i<4;i++)
            {
                Vector3Int nextPos = nowPos + Dir[i];
                Vector2Int index = new Vector2Int(nextPos.x - tileMap.cellBounds.xMin, nextPos.y - tileMap.cellBounds.yMin);
            

                if(index.x>=0&&index.y>=0&&index.x< tileMap.cellBounds.xMax &&index.y< tileMap.cellBounds.yMax&& !visit[index.x, index.y])
                {
                    visit[index.x, index.y] = true;

                    if (playerPos == nextPos)
                    {
                        isFind = true;
                        List_Root.Add(nextPos);
                        break;
                    }
                    else if (List_Ground.Where(x=>x==nextPos).FirstOrDefault()!=null)
                    {
                        int Xdistance = Mathf.Abs(playerPos.x - nowPos.x);
                        int Ydistance = Mathf.Abs(playerPos.y - nowPos.y);

                        int NextXdistance = Mathf.Abs(playerPos.x - nextPos.x);
                        int NextYdistance = Mathf.Abs(playerPos.y - nextPos.y);


                        if (NextXdistance <= Xdistance && NextYdistance <= Ydistance)
                        {
                            Debug.Log(nextPos);
                            List_Root.Add(nextPos);
                            Q_Root.Enqueue(nextPos);
                        }
                    }
                }
            }
        }

        foreach(var pos in List_Root)
        {
            tileMap.SetColor(pos, Color.black);
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
        tm.tileBase = EditorGUILayout.ObjectField("TileBase Source", tm.tileBase, typeof(TileBase), true) as TileBase;

        tm.player = EditorGUILayout.ObjectField("Player Source", tm.player, typeof(GameObject), true) as GameObject;
        tm.enemy = EditorGUILayout.ObjectField("Enemy Source", tm.enemy, typeof(GameObject), true) as GameObject;

        if (GUILayout.Button("Check Size"))
            tm.CheckSize();

        if (GUILayout.Button("Check Tile"))
            tm.CheckTile();

        if (GUILayout.Button("Reset"))
            tm.Reset();

        if (GUILayout.Button("PathFinding"))
            tm.PathFinding();

     
        pos=EditorGUILayout.Vector3IntField("Tile Coord",pos);
        if (GUILayout.Button("Check Tile Coord"))
            tm.CheckTileCoord(pos);
    }

  
}
