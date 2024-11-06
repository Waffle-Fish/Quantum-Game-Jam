using System;
using System.Collections.Generic;
using System.Data;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GameBoardManager : MonoBehaviour
{
    [Serializable]
    public struct Tile
    {
        [Min(0f)]
        public float Chance;
        public GameObject TileObj;
    }
    public static GameBoardManager Instance { get; private set;}
    public List<List<Tuple<float,GameObject>>> GameBoard {get; private set;} = new();
    public enum MoveDir { DL, DR, UR, UL, UP, DOWN, NA } // DL = Down Left, UR = Up Right
    
    [Header("Board Config")]
    [SerializeField]
    [Tooltip("If true, the gameBoard will be initialized to it's children objects. If false, the gameboard will be auto generated")]
    private bool readChildren;
    [SerializeField]
    private int numRows;
    [SerializeField]
    private int numCols;
    [SerializeField]
    [Tooltip("0.866 is the height / length of the side of a unit hexagon ")]
    private Vector2 center;

    [Header("Game Tiles")]
    [SerializeField]
    private List<Tile> tiles;
    [SerializeField]
    private GameObject rssTile;
    [SerializeField]
    [Tooltip("Clamps from <0,0> to max width and height of board")]
    private Vector2Int rssTileBoardPos;

    [Header("Select Tile")]
    [SerializeField]
    [Tooltip("Will display on currently selected tile")]
    private GameObject tileHighlight;
    private GameObject currentlySelectedTile;
    private Vector2Int currentTileBoardPos;

    [Header("Player")]
    private GameObject player;
    private Vector2Int playerBoardPos = new(0,0);

    [Header("Debug Values")]
    private Dictionary<string, int> tileCountMap = new();

    private void Awake() {
        if (Instance != null && Instance != this) Destroy(this); 
        else Instance = this; 
    }

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        AdjustTilesPercentage();
        InitializeHexBoard();
        player.transform.position = GetWorldPosOnBoard(playerBoardPos);
        
        tileHighlight.SetActive(false);
    }

    
    public void SelectTile(GameObject chosenTile)
    {
        currentlySelectedTile = chosenTile;
        currentTileBoardPos = FindTileBoardPos(chosenTile);
        tileHighlight.SetActive(true);
        tileHighlight.transform.position = currentlySelectedTile.transform.position;
    }

    public void MovePlayer()
    {
        // Base Conditions
        if (!currentlySelectedTile) {
            Debug.Log("No Tile Currently Selected");
            return; 
        }
        if (currentlySelectedTile.GetComponent<BlackHole>()) return; 

        MoveDir dirRelativeToPlayer = IsTileAdjacentToPlayer(currentlySelectedTile);
        if (IsTileAdjacentToPlayer(currentlySelectedTile) == MoveDir.NA) return;
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

        // Debug.Log("Moving " + dirRelativeToPlayer);
        player.transform.position = GetWorldPosOnBoard(playerBoardPos);
        tileHighlight.SetActive(false);

        // Win Condition
        // if (currentlySelectedTile.GetComponent<RS>()) {

        // }
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

    // Makes all the tile chances proportionally add up to 1f
    private void AdjustTilesPercentage()
    {
        float totalPercentage = 0f;
        foreach (Tile tile in tiles) totalPercentage += tile.Chance;
        if (Mathf.Approximately(totalPercentage, 0f))
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                Tile newTile;
                newTile.Chance = 1f / tiles.Count;
                newTile.TileObj = tiles[i].TileObj;
                tiles[i] = newTile;
            }
        }
        else if (!Mathf.Approximately(totalPercentage, 1f))
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                Tile newTile;
                newTile.Chance = tiles[i].Chance / totalPercentage;
                newTile.TileObj = tiles[i].TileObj;
                tiles[i] = newTile;
            }
        }
    }

    private void InitializeHexBoard()
    {
        // Set up tile objects
        const float UNIT_HEX_HALF_HEIGHT = 0.866f; // sqrt(3)/2 == height of a unit hexagon
        List<float> chanceTable = new() { 0f };
        for (int i = 0; i < tiles.Count-1; i++)
        {
            chanceTable.Add(tiles[i].Chance + chanceTable[i]);
        }

        GameObject blankObj = new();
        for (int i = 0; i < numRows; i++)
        {
            GameObject r = Instantiate(blankObj,transform);
            r.name = "row " + i;
            r.transform.localPosition = new(transform.position.x, transform.position.y + i * UNIT_HEX_HALF_HEIGHT * tiles[0].TileObj.transform.localScale.x);
            for (int j = 0; j < numCols; j++)
            {
                // Randomly Get Tile
                float roll = UnityEngine.Random.Range(0f, 1f);
                int tileInd = 0;
                for (int k = chanceTable.Count-1; k >= 0; k--)
                {
                    if (roll >= chanceTable[k]) {
                        tileInd = k;
                        break;
                    }
                }
                GameObject tile = tiles[tileInd].TileObj;
                if (!tileCountMap.TryAdd(tile.name, 1)) { tileCountMap[tile.name]++;}

                // Place tile down
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

        // Place Research Space Station down
        Vector2Int newRSSPos = rssTileBoardPos;
        if (newRSSPos == playerBoardPos) newRSSPos = new(int.MaxValue, int.MaxValue);
        newRSSPos = new(Math.Clamp(newRSSPos.x,0,numCols-1), Math.Clamp(newRSSPos.y, 0,numRows-1));
        rssTileBoardPos = newRSSPos;
        GameObject rssObj = Instantiate(rssTile,GetWorldPosOnBoard(newRSSPos.x, newRSSPos.y),Quaternion.identity, transform.GetChild(newRSSPos.y));
        rssObj.transform.SetSiblingIndex(newRSSPos.x);
        Destroy(GameBoard[newRSSPos.y][newRSSPos.x].Item2);
        GameBoard[newRSSPos.y][newRSSPos.x] = new(GameBoard[newRSSPos.y][newRSSPos.x].Item1, rssObj);

        transform.position = center;
        RevealTileProportion();
    }

    // Returns the transform position of the tile on the board
    private Vector3 GetWorldPosOnBoard(int x, int y) {
        return GameBoard[y][x].Item2.transform.position;
    }

    // Returns the transform position of the tile on the board
    private Vector3 GetWorldPosOnBoard(Vector2Int boardPos) {
        return GameBoard[boardPos.y][boardPos.x].Item2.transform.position;
    }

    private Vector2Int FindTileBoardPos(GameObject tile) {
        for (int y = 0; y < GameBoard.Count; y++)
        {
            for (int x = 0; x < GameBoard[y].Count; x++)
            {
                if (GetWorldPosOnBoard(x,y) == tile.transform.position) return new(x,y);
            }
        }
        Debug.LogError("Tile Not Found");
        return new(-1,-1);
    }

    private void RevealTileProportion() {
        float count = numCols * numRows;
        foreach (var tile in tileCountMap)
        {
            Debug.Log(tile.Key + " count: " + tile.Value + " | proportion: " + tile.Value/count);
        }
    }
}
