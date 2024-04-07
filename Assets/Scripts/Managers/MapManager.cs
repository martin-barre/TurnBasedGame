using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : Singleton<MapManager>
{
    public static event Action<Entity> OnOveredEntityChanged;

    [Header("Tilemaps")]
    [SerializeField] private Tilemap tilemapCollider;
    [SerializeField] private Tilemap tilemapSpawns;
    [SerializeField] private Tilemap tilemapOverlay1;
    [SerializeField] private Tilemap tilemapOverlay2;

    [Header("Tiles")]
    [SerializeField] private TileBase groundTile;
    [SerializeField] private TileBase wallTile;
    [SerializeField] private TileBase spawnRedTile;
    [SerializeField] private TileBase spawnBlueTile;

    private BoundsInt bounds;
    private Node[,] grid;
    private List<Node> spawnsRed;
    private List<Node> spawnsBlue;

    private Entity _overedEntity;


    protected override void Awake()
    {
        base.Awake();

        // INITIALIZE GRID
        tilemapCollider.CompressBounds();
        bounds = tilemapCollider.cellBounds;
        grid = new Node[bounds.xMax - bounds.xMin, bounds.yMax - bounds.yMin];
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                TileBase currentTile = tilemapCollider.GetTile(Vector3Int.CeilToInt(new Vector3(x, y)));

                NodeType type = NodeType.EMPTY;
                if (currentTile == null) type = NodeType.EMPTY;
                else if (currentTile.Equals(groundTile)) type = NodeType.GROUND;
                else if (currentTile.Equals(wallTile)) type = NodeType.WALL;

                Vector2Int gridPosition = Vector2Int.CeilToInt(new Vector2(x, y));
                Vector3 worldPosition = tilemapSpawns.GetCellCenterLocal(Vector3Int.CeilToInt(new Vector3(x, y)));

                grid[x - bounds.xMin, y - bounds.yMin] = new Node(gridPosition, worldPosition, type);
            }
        }

        // INITIALIZE SPAWNS
        spawnsRed = new List<Node>();
        spawnsBlue = new List<Node>();
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                TileBase currentTile = tilemapSpawns.GetTile(Vector3Int.CeilToInt(new Vector2(x, y)));
                if (currentTile == null) continue;
                if (currentTile.Equals(spawnRedTile))
                {
                    Node node = grid[x - bounds.xMin, y - bounds.yMin];
                    node.spawnTeam = Team.RED;
                    spawnsRed.Add(node);
                }
                if (currentTile.Equals(spawnBlueTile))
                {
                    Node node = grid[x - bounds.xMin, y - bounds.yMin];
                    node.spawnTeam = Team.BLUE;
                    spawnsBlue.Add(node);
                }
            }
        }
    }

    private void Update()
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var node = WorldPositionToMapNodes(mousePosition);
        var entity = node?.entity;

        if (_overedEntity != entity)
        {
            _overedEntity = entity;
            OnOveredEntityChanged?.Invoke(entity);
        }
    }

    public Node WorldPositionToMapNodes(Vector3 position)
    {
        Vector3Int cellPosition = tilemapCollider.WorldToCell(position);
        int x = cellPosition.x - bounds.xMin;
        int y = cellPosition.y - bounds.yMin;
        if (x < 0 || y < 0 || x >= grid.GetLength(0) || y >= grid.GetLength(1))
        {
            return new Node(Vector2Int.CeilToInt(new Vector2(cellPosition.x, cellPosition.y)), position, NodeType.WALL);
        }
        return grid[x, y];
    }

    public void MoveEntity(Entity entity, Node node, bool teleport = false)
    {
        if (node.type != NodeType.GROUND) return;
        if (entity == null) return;

        Node bufferNode = entity.node;
        Entity bufferEntity = node.entity;

        // Move entity 1
        node.entity = entity;
        entity.node = node;
        if (teleport) entity.transform.position = node.worldPosition;

        // Move entity 2
        bufferNode.entity = bufferEntity;
        if (bufferEntity != null)
        {
            bufferEntity.node = bufferNode;
            if (teleport) bufferEntity.transform.position = bufferNode.worldPosition;
        }
    }

    public Node GetRandomSpawns(Team team)
    {
        List<Node> nodes = team == Team.BLUE ? spawnsBlue : spawnsRed;
        foreach (Node node in nodes)
        {
            if (node.entity == null) return node;
        }
        return null;
    }

    public void ActiveTilemapSpawns(bool active)
    {
        tilemapSpawns.gameObject.SetActive(active);
    }

    public Node GetNode(Vector2Int position)
    {
        if (position.x < bounds.xMin || position.x >= bounds.xMax || position.y < bounds.yMin || position.y >= bounds.yMax)
        {
            Vector3 worldPosition = tilemapSpawns.GetCellCenterLocal(Vector3Int.CeilToInt(new Vector3(position.x, position.y)));
            return new Node(position, worldPosition, NodeType.EMPTY);
        }
        return grid[position.x - bounds.xMin, position.y - bounds.yMin];
    }

    public void AddOverlay1(List<Node> nodes)
    {
        foreach (var node in nodes)
        {
            tilemapOverlay1.SetTile((Vector3Int)node.gridPosition, spawnBlueTile);
        }
        tilemapOverlay1.RefreshAllTiles();
    }

    public void AddOverlay2(List<Node> nodes)
    {
        foreach (var node in nodes)
        {
            tilemapOverlay2.SetTile((Vector3Int)node.gridPosition, spawnRedTile);
        }
        tilemapOverlay2.RefreshAllTiles();
    }

    public void ClearOverlay1()
    {
        tilemapOverlay1.ClearAllTiles();
    }

    public void ClearOverlay2()
    {
        tilemapOverlay2.ClearAllTiles();
    }
}
