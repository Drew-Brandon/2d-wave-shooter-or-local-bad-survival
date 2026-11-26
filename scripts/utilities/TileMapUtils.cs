using Godot;
using System;

public static class TileMapUtils
{
    public static Vector2I GetCellFromGlobal(TileMapLayer tileMap, Vector2 pos)
    {
        return (Vector2I)(pos / tileMap.TileSet.TileSize).Floor();
    }

    public static Vector2 GlobalSnapToCell(TileMapLayer tileMap, Vector2 pos)
    {
        Vector2I tileSize = tileMap.TileSet.TileSize;
        return (pos / tileSize).Floor() * tileSize + tileSize / 2;
    }
}
