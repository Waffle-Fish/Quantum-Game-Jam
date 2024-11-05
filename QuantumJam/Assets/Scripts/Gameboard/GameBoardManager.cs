using System;
using System.Collections.Generic;
using System.Data;
using Unity.Collections;
using UnityEngine;

public class GameBoardManager : MonoBehaviour
{
    public static GameBoardManager Instance { get; private set;}
    public List<List<Tuple<float,GameObject>>> GameBoard {get; private set;} = new();

    public enum MoveDir { DL, DR, UR, UL, UP, DOWN, NA } // DL = Down Left, UR = Up Right

    [SerializeField]
    [Tooltip("If true, the gameBoard will be initialized to it's children objects. If false, the gameboard will be auto generated")]
    private bool readChildren;
    [SerializeField]
    private int numRows;
    [SerializeField]
    private int numCols;
    [SerializeField]
    private GameObject tile;
    [SerializeField]
    [Tooltip("0.866 is the height / length of the side of a unit hexagon ")]
    private Vector2 center;

    private GameObject player;
    private Vector2Int playerBoardPos = new(0,0);

    private void Awake() {
        if (Instance != null && Instance != this) Destroy(this); 
        else Instance = this; 
    }

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        InitializeHexBoard();
        player.transform.position = GetWorldPosOnBoard(playerBoardPos);
    }

    private void InitializeHexBoard()
    {
        // Set up tile objects
        GameObject blankObj = new();
        for (int i = 0; i < numRows; i++)
        {
            GameObject r = Instantiate(blankObj,transform);
            r.name = "row " + i;
            // float xPosRow = ((i % 2 == 0) ? -0.375f : +0.375f) * tile.transform.localScale.x;
            // sqrt(3)/2 == height of a unit hexagon. 3sqrt(3)/4 == ~1.3f
            r.transform.localPosition = new(transform.position.x, transform.position.y + i * 1.3f * transform.localScale.x);
            // r.transform.position = new(transform.position.x, transform.position.y+i*0.433f * tile.transform.localScale.x);
            for (int j = 0; j < numCols; j++)
            {
                GameObject newTile = Instantiate(tile,r.transform);
                float xPosCol = newTile.transform.localPosition.x + 0.75f * j * tile.transform.localScale.x;
                float yPosCol = (j % 2 == 0) ? 0 : +0.433f * tile.transform.localScale.x;
                newTile.transform.localPosition = new(xPosCol, yPosCol);
            }
        }
        Destroy(blankObj);

        for (int i = 0; i < transform.childCount; i++)
        {
            List<Tuple<float,GameObject>> r = new();
            for (int j = 0; j < transform.GetChild(i).childCount; j++) {
                float val = ((j % 2 == 0) ? 0f : 0.5f) + i;
                r.Add(new(val, transform.GetChild(i).GetChild(j).gameObject));
            }
            GameBoard.Add(r);
        }
        transform.position = center;
    }

    public void SelectTile(GameObject chosenTile)
    {
        MovePlayer(chosenTile);
    }

    public void MovePlayer(GameObject chosenTile)
    {
        MoveDir dirRelativeToPlayer = IsTileAdjacentToPlayer(chosenTile);
        if (IsTileAdjacentToPlayer(chosenTile) == MoveDir.NA) return;
        float curVal = GameBoard[playerBoardPos.y][playerBoardPos.x].Item1;
        switch (dirRelativeToPlayer)
        {
            case MoveDir.UP:
                playerBoardPos.y++;
                break;
            case MoveDir.UL:
                playerBoardPos.x--;
                if (GameBoard[playerBoardPos.y][playerBoardPos.x].Item1 < curVal) playerBoardPos.y++;
                break;
            case MoveDir.UR:
                playerBoardPos.x++;
                if (GameBoard[playerBoardPos.y][playerBoardPos.x].Item1 < curVal) playerBoardPos.y++;
                break;
            case MoveDir.DOWN:
                playerBoardPos.y--;
                break;
            case MoveDir.DL:
                playerBoardPos.x--;
                if (GameBoard[playerBoardPos.y][playerBoardPos.x].Item1 > curVal) playerBoardPos.y--;
                break;
            case MoveDir.DR:
                playerBoardPos.x++;
                if (GameBoard[playerBoardPos.y][playerBoardPos.x].Item1 > curVal) playerBoardPos.y--;
                break;
            default:
            break;
        }
        Debug.Log("Moving " + dirRelativeToPlayer);
        player.transform.position = GetWorldPosOnBoard(playerBoardPos);
    }

    // Returns the direction of the relative to the player if adjacent
    private MoveDir IsTileAdjacentToPlayer(GameObject chosenTile) {
        bool CheckTop(int ind) { return ind < GameBoard.Count; }
        bool CheckBot(int ind) { return ind >= 0; }
        bool CheckLeft(int ind) { return ind >= 0; }
        bool CheckRight(int ind) { return ind < GameBoard[0].Count; }
        float AbsSub(float a, float b) { return Mathf.Abs(a - b); }

        Vector3 tilePos = chosenTile.transform.position;
        Vector2Int pbPos = playerBoardPos;
        float curValue = GameBoard[pbPos.y][pbPos.x].Item1;
        if (CheckTop(pbPos.y+1) && GetWorldPosOnBoard(pbPos.x, pbPos.y+1) == tilePos) return MoveDir.UP;
        if (CheckBot(pbPos.y-1) && GetWorldPosOnBoard(pbPos.x, pbPos.y-1) == tilePos) return MoveDir.DOWN;
        if (CheckLeft(pbPos.x-1)) {
            if (CheckTop(pbPos.y+1) && AbsSub(GameBoard[pbPos.y+1][pbPos.x-1].Item1,curValue) <= 1f && GetWorldPosOnBoard(pbPos.x-1, pbPos.y+1) == tilePos) return MoveDir.UL;
            if (CheckBot(pbPos.y-1) && AbsSub(GameBoard[pbPos.y-1][pbPos.x-1].Item1,curValue) <= 1f && GetWorldPosOnBoard(pbPos.x-1, pbPos.y-1) == tilePos) return MoveDir.DL;
            if (GetWorldPosOnBoard(pbPos.x-1, pbPos.y) == tilePos) {
                if (GameBoard[pbPos.y][pbPos.x-1].Item1 < curValue) return MoveDir.DL;
                else return MoveDir.UL;
            }
        }
        if (CheckRight(pbPos.x+1)) {
            if (CheckTop(pbPos.y+1) && AbsSub(GameBoard[pbPos.y+1][pbPos.x+1].Item1,curValue) <= 1f && GetWorldPosOnBoard(pbPos.x+1, pbPos.y+1) == tilePos) return MoveDir.UR;
            if (CheckBot(pbPos.y-1) && AbsSub(GameBoard[pbPos.y-1][pbPos.x+1].Item1,curValue) <= 1f && GetWorldPosOnBoard(pbPos.x+1, pbPos.y-1) == tilePos) return MoveDir.DR;
            if (GetWorldPosOnBoard(pbPos.x+1, pbPos.y) == tilePos) {
                if (GameBoard[pbPos.y][pbPos.x+1].Item1 < curValue) return MoveDir.DR;
                else return MoveDir.UR;
            }
        }
        return MoveDir.NA;
    }

    // Returns the transform position of the tile on the board
    private Vector3 GetWorldPosOnBoard(int x, int y) {
        return GameBoard[y][x].Item2.transform.position;
    }

    // Returns the transform position of the tile on the board
    private Vector3 GetWorldPosOnBoard(Vector2Int boardPos) {
        return GameBoard[boardPos.y][boardPos.x].Item2.transform.position;
    }
}
