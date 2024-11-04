using System;
using System.Collections.Generic;
using System.Data;
using Unity.Collections;
using UnityEngine;

public class GameBoardManager : MonoBehaviour
{
    public static GameBoardManager Instance { get; private set;}
    public List<List<GameObject>> gameBoard {get; private set;} = new();

    public enum MoveDir { left, right, up, down, NA }

    [SerializeField]
    private int numRows;
    [SerializeField]
    private int numCols;
    [SerializeField]
    private GameObject tile;

    private GameObject player;
    private Vector2Int playerBoardPos = new(0,0);

    private void Awake() {
        if (Instance != null && Instance != this) Destroy(this); 
        else Instance = this; 
    }

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        InitializeBoard();
        player.transform.position = GetWorldPosOnBoard(playerBoardPos);
    }

    private void InitializeBoard()
    {
        GameObject blankObj = new();
        for (int i = 0; i < numRows; i++)
        {
            GameObject r = Instantiate(blankObj,transform);
            r.transform.position = new(transform.position.x, transform.position.y+i);
            for (int j = 0; j < numCols; j++)
            {
                GameObject newTile = Instantiate(tile,r.transform);
                newTile.transform.position = new(newTile.transform.position.x+j, newTile.transform.position.y);
            }
        }
        Destroy(blankObj);

        foreach (Transform row in transform)
        {
            List<GameObject> r = new();
            foreach (Transform col in row.transform)
            {
                r.Add(col.gameObject);
            }
            gameBoard.Add(r);
        }
        transform.position = new(-numCols /2f, -numRows/2f);
    }

    public void ProcessMovePlayer(GameObject chosenTile) {
        MoveDir tileDir = IsTileAdjacentToPlayer(chosenTile);
        if (IsTileAdjacentToPlayer(chosenTile) == MoveDir.NA) return;
        MovePlayer(tileDir);
    }

    // Returns the direction of the relative to the player if adjacent
    private MoveDir IsTileAdjacentToPlayer(GameObject chosenTile) {
        Vector3 tilePos = chosenTile.transform.position;
        Vector2Int pbPos = playerBoardPos;
        if (pbPos.x+1 < gameBoard[0].Count && GetWorldPosOnBoard(pbPos.x+1, pbPos.y) == tilePos) {
            return MoveDir.right;
        }
        if (pbPos.x-1 >= 0 && GetWorldPosOnBoard(pbPos.x-1, pbPos.y) == tilePos) {
            return MoveDir.left;
        }
        if (pbPos.y+1 < gameBoard.Count && GetWorldPosOnBoard(pbPos.x, pbPos.y+1) == tilePos) {
            return MoveDir.up;
        }
        if (pbPos.y-1 >= 0 && GetWorldPosOnBoard(pbPos.x, pbPos.y-1) == tilePos) {
            return MoveDir.down;
        }
        return MoveDir.NA;
    }

    private void MovePlayer(MoveDir dir) {
        switch (dir)
        {
            case MoveDir.left:
                playerBoardPos.x--;
                break;
            case MoveDir.right:
                playerBoardPos.x++;
                break;
            case MoveDir.up:
                playerBoardPos.y++;
                break;
            case MoveDir.down:
                playerBoardPos.y--;
                break;
            default:
            break;
        }
        player.transform.position = GetWorldPosOnBoard(playerBoardPos);
    }

    // Returns the transform position of the tile on the board
    private Vector3 GetWorldPosOnBoard(int x, int y) {
        return gameBoard[y][x].transform.position;
    }

    // Returns the transform position of the tile on the board
    private Vector3 GetWorldPosOnBoard(Vector2Int boardPos) {
        return gameBoard[boardPos.y][boardPos.x].transform.position;
    }


}
