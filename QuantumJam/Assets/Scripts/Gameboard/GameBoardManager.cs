using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using QRG.QuantumForge.FaQtory;
using QRG.QuantumForge.Runtime;
using Unity.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class GameBoardManager : MonoBehaviour
{
    [Serializable]
    public struct RandomTile
    {
        [Min(0f)]
        public float Chance;
        public GameObject TileObj;
    }
    public static GameBoardManager Instance { get; private set;}
    public List<List<Tuple<float,GameObject>>> GameBoard {get; private set;} = new();

    [Header("Select Tile")]
    [SerializeField]
    [Tooltip("Will display on currently selected tile")]
    private GameObject tileHighlight;
    private GameObject currentlySelectedTile;
    private Vector2Int currentTileBoardPos;
    
    [Header("Board Config")]
    [SerializeField]
    [Tooltip("If true, the gameBoard will be initialized to it's children objects. If false, the gameboard will be auto generated")]
    private bool readChildren;
    [SerializeField]
    private int numRows;
    [SerializeField]
    private int numCols;
    [Tooltip("0.866 is also the length of the side of a unit hexagon ")]
    private const float UNIT_HEX_HALF_HEIGHT = 0.866f; // sqrt(3)/2 == height of a unit hexagon from center

    [Header("Game Tiles")]
    [SerializeField]
    private List<RandomTile> tiles;
    [SerializeField]
    private GameObject rssTile;
    [SerializeField]
    [Tooltip("Clamps from <0,0> to max width and height of board")]
    private Vector2Int rssTileBoardPos;
    [SerializeField]
    private GameObject startingTile;
    [SerializeField]
    [Tooltip("Clamps from <0,0> to max width and height of board")]
    private Vector2Int startingTileBoardPos;

    [Header("Player")]
    [SerializeField]
    private int maxFuel;
    public int CurrentFuel {get; private set;} = 0;
    [SerializeField]
    private int maxProbes;
    public int CurrentProbesCount {get; private set;} = 0;
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
        // SetUp Board
        if (readChildren) {
            ReadChildrenTiles();
        }
        else {
            AdjustTilesPercentage();
            InitializeHexBoard();
        }

        // Initialize Player
        player = GameObject.FindWithTag("Player");
        playerBoardPos = startingTileBoardPos;
        player.transform.position = GetWorldPosOnBoard(playerBoardPos);
        CurrentFuel = maxFuel;
        CurrentProbesCount = maxProbes;
        tileHighlight.SetActive(false);
    }

    private void ReadChildrenTiles() {
        numRows = transform.childCount;
        numCols = transform.GetChild(0).childCount;
        for (int i = 0; i < numRows; i++)
        {
            GameObject rowObj = transform.GetChild(i).gameObject;
            rowObj.transform.localPosition = new(transform.localPosition.x, transform.localPosition.y + i * UNIT_HEX_HALF_HEIGHT * tiles[0].TileObj.transform.localScale.x);
            List<Tuple<float, GameObject>> rowTup = new();
            for (int j = 0; j < numCols; j++)
            {
                Tile currentTile = transform.GetChild(i).GetChild(j).GetComponent<Tile>();
                float val = ((j % 2 == 0) ? 0f : 0.5f) + i;
                if (currentTile.TileType == Tile.Type.Blank) val = -1f;
                if (currentTile.TileType == Tile.Type.Start) {
                    startingTile = currentTile.gameObject;
                    startingTileBoardPos = new Vector2Int(j,i);
                    
                }
                float xPosCol = 0.75f * j * currentTile.transform.localScale.x;
                float yPosCol = (j % 2 == 0) ? 0 : +0.433f * currentTile.transform.localScale.x;
                currentTile.transform.localPosition = new(xPosCol, yPosCol);
                rowTup.Add(new(val, transform.GetChild(i).GetChild(j).gameObject));
            }
            GameBoard.Add(rowTup);
        }
    }
    
    public void SelectTile(GameObject chosenTile)
    {
        currentlySelectedTile = chosenTile;
        currentTileBoardPos = FindTileBoardPos(chosenTile);
        // Debug.Log(currentlySelectedTile + " val: " + GameBoard[currentTileBoardPos.y][currentTileBoardPos.x].Item1);
        tileHighlight.SetActive(true);
        tileHighlight.transform.position = currentlySelectedTile.transform.position;
    }

    public void MovePlayer()
    {
        // Helper functions
        bool CheckTop(int ind) { return ind < GameBoard.Count; }
        bool CheckBot(int ind) { return ind >= 0; }
        bool CheckLeft(int ind) { return ind >= 0; }
        bool CheckRight(int ind) { return ind < GameBoard[0].Count; }
        float AbsSub(float a, float b) { return Mathf.Abs(a - b); }

        // Base Conditions
        if (!currentlySelectedTile) {
            Debug.Log("No Tile Currently Selected");
            return; 
        }
        if (currentlySelectedTile.GetComponent<Tile>().TileType == Tile.Type.BlackHole) return; 
        if (CurrentFuel <= 0) {
            Debug.Log("Out of Fuel!");
            return;
        }

        // Prep for Move
        Vector3 tilePos = currentlySelectedTile.transform.position;
        Vector2Int pbPos = playerBoardPos;
        Vector2Int destPos = new(-1,-1);
        float curValue = GameBoard[pbPos.y][pbPos.x].Item1;

        // Try Move Up
        if (CheckTop(pbPos.y+1) && GetWorldPosOnBoard(pbPos.x, pbPos.y+1) == tilePos) destPos = new(pbPos.x, pbPos.y+1);
        // Try Move Down
        if (CheckBot(pbPos.y-1) && GetWorldPosOnBoard(pbPos.x, pbPos.y-1) == tilePos) destPos = new(pbPos.x, pbPos.y-1);
        // Try Move Up Left or Down Left
        if (CheckLeft(pbPos.x-1)) {
            if (CheckTop(pbPos.y+1) && AbsSub(GameBoard[pbPos.y+1][pbPos.x-1].Item1,curValue) <= 1f && GetWorldPosOnBoard(pbPos.x-1, pbPos.y+1) == tilePos) destPos = new(pbPos.x-1, pbPos.y+1);
            if (CheckBot(pbPos.y-1) && AbsSub(GameBoard[pbPos.y-1][pbPos.x-1].Item1,curValue) <= 1f && GetWorldPosOnBoard(pbPos.x-1, pbPos.y-1) == tilePos) destPos = new(pbPos.x-1, pbPos.y-1);
            if (GetWorldPosOnBoard(pbPos.x-1, pbPos.y) == tilePos) {
                destPos = new(pbPos.x-1, pbPos.y);
            }
        }
        // Try Move Up Right or Down Right
        if (CheckRight(pbPos.x+1)) {
            if (CheckTop(pbPos.y+1) && AbsSub(GameBoard[pbPos.y+1][pbPos.x+1].Item1,curValue) <= 1f && GetWorldPosOnBoard(pbPos.x+1, pbPos.y+1) == tilePos) destPos = new(pbPos.x+1, pbPos.y+1);
            if (CheckBot(pbPos.y-1) && AbsSub(GameBoard[pbPos.y-1][pbPos.x+1].Item1,curValue) <= 1f && GetWorldPosOnBoard(pbPos.x+1, pbPos.y-1) == tilePos) destPos = new(pbPos.x+1, pbPos.y-1);
            if (GetWorldPosOnBoard(pbPos.x+1, pbPos.y) == tilePos) {
                destPos = new(pbPos.x+1, pbPos.y);
            }
        }

        // Update player
        tileHighlight.SetActive(false);
        if (destPos == new Vector2Int(-1,-1)) {
            currentlySelectedTile = null;
            return;
        }
        playerBoardPos = destPos;
        player.transform.position = GetWorldPosOnBoard(playerBoardPos);
        CurrentFuel--;

        // Activate Tile Effect
        Tile currentTile = currentlySelectedTile.GetComponent<Tile>();
        switch (currentTile.TileType)
        {
            case Tile.Type.RSS:
                Debug.Log("You won!");
                break;
            case Tile.Type.QuantumZone:
                QuantumZone curTileQZ = currentTile.GetComponent<QuantumZone>();
                curTileQZ.Measure();
                StartCoroutine(CheckSafety(curTileQZ));
                break;
            default:
            break;
        }

        // Pickup Tile
        Pickup pickup = currentTile.pickup;
        if (pickup) {
            switch (pickup.ItemType) {
                case Pickup.Item.RepairKit:
                    player.GetComponent<PlayerHealth>().HealPlayer(pickup.val);
                    break;
                case Pickup.Item.Fuel:
                    CurrentFuel += pickup.val;
                    Math.Clamp(CurrentFuel, 0 , maxFuel);
                    break;
                case Pickup.Item.Probe:
                    CurrentProbesCount += pickup.val;
                    Math.Clamp(CurrentProbesCount, 0 , maxProbes);
                    break;
            }
            pickup.gameObject.SetActive(false);
        }

        currentlySelectedTile = null;
    }

    IEnumerator CheckSafety(QuantumZone qz) {
        yield return null;
        if (!qz.IsSafe()) player.GetComponent<PlayerHealth>().DamagePlayer();
    }

    public void ActivateProbe() {
        if (!currentlySelectedTile) {
            Debug.Log("No Tile Currently Selected");
            return; 
        }
        if (CurrentProbesCount <= 0) {
            Debug.Log("Out of Probes!");
            return;
        }

        // Activate TileEffect
        Tile currentTile = currentlySelectedTile.GetComponent<Tile>();
        if (currentTile.TileType == Tile.Type.QuantumZone) {
                QuantumZone curTileQZ = currentTile.GetComponent<QuantumZone>();
                curTileQZ.Measure();
        }
        CurrentProbesCount--;
        currentlySelectedTile = null;
        tileHighlight.SetActive(false);
    }

    public void PhaseRotateTile(float percentShift) {
        Tile currentTile = currentlySelectedTile.GetComponent<Tile>();
        QuantumZone curTileQZ = currentTile.GetComponent<QuantumZone>();
        if (currentTile.TileType != Tile.Type.QuantumZone) return;
        if (curTileQZ.IsMeasured) { return; }

        percentShift = math.clamp(percentShift, 0, 1f);
        curTileQZ.PhaseAll(math.PI * percentShift);
        currentlySelectedTile = null;
        tileHighlight.SetActive(false);
    }

    #region Auto-Generate Map
    // Makes all the tile chances proportionally add up to 1f
    private void AdjustTilesPercentage()
    {
        float totalPercentage = 0f;
        foreach (RandomTile tile in tiles) totalPercentage += tile.Chance;
        if (Mathf.Approximately(totalPercentage, 0f))
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                RandomTile newTile;
                newTile.Chance = 1f / tiles.Count;
                newTile.TileObj = tiles[i].TileObj;
                tiles[i] = newTile;
            }
        }
        else if (!Mathf.Approximately(totalPercentage, 1f))
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                RandomTile newTile;
                newTile.Chance = tiles[i].Chance / totalPercentage;
                newTile.TileObj = tiles[i].TileObj;
                tiles[i] = newTile;
            }
        }
    }

    private void InitializeHexBoard()
    {
        // Set up Random Tile Table
        List<float> chanceTable = new() { 0f };
        for (int i = 0; i < tiles.Count - 1; i++)
        {
            chanceTable.Add(tiles[i].Chance + chanceTable[i]);
        }

        // Clear Children
        foreach (Transform row in transform)
        {
            Destroy(row.gameObject);
        }

        // Place Down Tiles
        GameObject blankObj = new();
        for (int i = 0; i < numRows; i++)
        {
            GameObject r = Instantiate(blankObj, transform);
            r.name = "row " + i;
            r.transform.localPosition = new(transform.position.x, transform.position.y + i * UNIT_HEX_HALF_HEIGHT * tiles[0].TileObj.transform.localScale.x);
            for (int j = 0; j < numCols; j++)
            {
                // Randomly Get Tile
                float roll = UnityEngine.Random.Range(0f, 1f);
                int tileInd = 0;
                for (int k = chanceTable.Count - 1; k >= 0; k--)
                {
                    if (roll >= chanceTable[k])
                    {
                        tileInd = k;
                        break;
                    }
                }
                GameObject tile = tiles[tileInd].TileObj;
                if (!tileCountMap.TryAdd(tile.name, 1)) { tileCountMap[tile.name]++; }

                // Place tile down
                GameObject newTile = Instantiate(tile, r.transform);
                float xPosCol = newTile.transform.localPosition.x + 0.75f * j * tile.transform.localScale.x;
                float yPosCol = (j % 2 == 0) ? 0 : +0.433f * tile.transform.localScale.x;
                newTile.transform.localPosition = new(xPosCol, yPosCol);
            }
        }
        Destroy(blankObj);

        // Update GameBoard to match placed tiles
        for (int i = 0; i < transform.childCount; i++)
        {
            List<Tuple<float, GameObject>> r = new();
            for (int j = 0; j < transform.GetChild(i).childCount; j++)
            {
                float val = ((j % 2 == 0) ? 0f : 0.5f) + i;
                r.Add(new(val, transform.GetChild(i).GetChild(j).gameObject));
            }
            GameBoard.Add(r);
        }

        // Place Research Space Station down
        Vector2Int newRSSPos = rssTileBoardPos;
        if (newRSSPos == playerBoardPos) newRSSPos = new(int.MaxValue, int.MaxValue);
        newRSSPos = new(Math.Clamp(newRSSPos.x, 0, numCols - 1), Math.Clamp(newRSSPos.y, 0, numRows - 1));
        rssTileBoardPos = newRSSPos;
        GameObject rssObj = Instantiate(rssTile, GetWorldPosOnBoard(newRSSPos.x, newRSSPos.y), Quaternion.identity, transform.GetChild(newRSSPos.y));
        rssObj.transform.SetSiblingIndex(newRSSPos.x);
        Destroy(GameBoard[newRSSPos.y][newRSSPos.x].Item2);
        GameBoard[newRSSPos.y][newRSSPos.x] = new(GameBoard[newRSSPos.y][newRSSPos.x].Item1, rssObj);
        RevealTileProportion();
    }

    private void RevealTileProportion() {
        float count = numCols * numRows;
        foreach (var tile in tileCountMap)
        {
            Debug.Log(tile.Key + " count: " + tile.Value + " | proportion: " + tile.Value/count);
        }
    }
    #endregion

    #region Helper Functions
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
    #endregion
}