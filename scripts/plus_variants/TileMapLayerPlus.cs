using Godot;
using Godot.Collections;

/// <summary>
/// An updated version of the tile map class which features additional functions.
/// </summary>
[GlobalClass, Tool]
public partial class TileMapLayerPlus : TileMapLayer
{
	private static readonly long[] TILE_ROTATIONS =
	[
		0,
		TileSetAtlasSource.TransformTranspose | TileSetAtlasSource.TransformFlipH,
		TileSetAtlasSource.TransformFlipH | TileSetAtlasSource.TransformFlipV,
		TileSetAtlasSource.TransformTranspose | TileSetAtlasSource.TransformFlipV
	];

	[Export]
	private int _defSrcId = 0;

	[Export]
	private Vector2I _singleTile = Vector2I.Zero;

	[Export]
	private Vector2I _endTile = Vector2I.Zero;

	[Export]
	private Vector2I _straightTile = Vector2I.Zero;

	[Export]
	private Vector2I _cornerTile = Vector2I.Zero;

	[Export]
	private Vector2I _tTile = Vector2I.Zero;

	[Export]
	private Vector2I _plusTile = Vector2I.Zero;

	[ExportToolButton("Connect Cells")]
	private Callable _connectCells => Callable.From(ConnectCells);

	private bool IsConnectingCell(Vector2I cellCoords)
	{
		Vector2I atlasCoords = GetCellAtlasCoords(cellCoords);

		return atlasCoords == _singleTile || atlasCoords == _endTile || atlasCoords == _straightTile ||
			atlasCoords == _cornerTile || atlasCoords == _tTile || atlasCoords == _plusTile;
	}

	private Vector2I GetConnectingTile(byte connectionId)
	{
		switch (connectionId)
		{
			default:
				return _singleTile;

			case 1:
				return _endTile;

			case 2:
				return _straightTile;

			case 3:
				return _tTile;

			case 4:
				return _plusTile;

			case 5:
				return _cornerTile;
		}
	}

	private void ConnectCells()
	{
		Array<Vector2I> usedCells = [.. GetUsedCellsById(_defSrcId, _singleTile), .. GetUsedCellsById(_defSrcId, _endTile),
									.. GetUsedCellsById(_defSrcId, _straightTile), .. GetUsedCellsById(_defSrcId, _cornerTile),
									.. GetUsedCellsById(_defSrcId, _tTile), .. GetUsedCellsById(_defSrcId, _plusTile)];

		GD.Print(usedCells.Count);
		for (int i = 0; i < usedCells.Count; i++)
		{
			Vector2I cell = usedCells[i];

			byte connectionsCount = 0;
			byte[] connections = new byte[4];

			if (IsConnectingCell(cell + Vector2I.Right))
			{
				connections[connectionsCount] = 0;
				connectionsCount++;
			}

			if (IsConnectingCell(cell + Vector2I.Down))
			{
				connections[connectionsCount] = 1;
				connectionsCount++;
			}

			if (IsConnectingCell(cell + Vector2I.Left))
			{
				connections[connectionsCount] = 2;
				connectionsCount++;
			}

			if (IsConnectingCell(cell + Vector2I.Up))
			{
				connections[connectionsCount] = 3;
				connectionsCount++;
			}

			int altTile;
			Vector2I atlasCoords;

			if (connectionsCount == 0)
			{
				altTile = 0;
				atlasCoords = _singleTile;
			}
			else
			{
				byte connectionRange = (byte)(connections[connectionsCount - 1] - connections[0]);
				byte median = connections[connectionRange % 3];
				byte connectionId = (byte)(connectionsCount == 2 && connectionRange % 2 == 1 ? 5 : connectionsCount);
				
				altTile = (int)TILE_ROTATIONS[median];
				atlasCoords = GetConnectingTile(connectionId);
			}

			SetCell(cell, _defSrcId, atlasCoords, altTile);
		}
	}

	/// <summary>
	/// Converts the given position to a position in the tile map's grid system.
	/// </summary>
	/// <param name="pos">
	/// The positiont to convert.
	/// </param>
	/// <returns>
	/// The converted position.
	/// </returns>
	public Vector2I GetTilePos(Vector2 pos)
	{
		return (Vector2I)(pos / TileSet.TileSize).Floor();
	}

	/// <summary>
	/// Snaps the given position to a tile in the grid.
	/// </summary>
	/// <param name="pos">
	/// The position to snap.
	/// </param>
	/// <returns>
	/// The snapped position.
	/// </returns>
	public Vector2 TileSnap(Vector2 pos)
	{
		Vector2I tileSize = TileSet.TileSize;
		return (pos / tileSize).Floor() * tileSize + tileSize / 2;
	}

	/// <summary>
	/// Gets the cell tile data at the given position.
	/// </summary>
	/// <param name="pos">
	/// The position of the cell to get the tile data from.
	/// </param>
	/// <returns>
	/// The tile data of the cell at the given position.
	/// </returns>
	public TileData GetCellTileDataGlobal(Vector2 pos)
	{
		return GetCellTileData(GetTilePos(pos));
	}

	public void SetCellLine(Vector2I start, Vector2I dir, int length, int srcId = -1, Vector2I? atlasCoords = null, int altTile = 0)
	{
		Trace2D.TraceLine(start, dir, length, (Vector2I coords) => SetCell(coords, srcId, atlasCoords, altTile));
	}

	public void SetCellRect(Vector2I start, Vector2I stop, int srcId = -1, Vector2I? atlasCoords = null, int altTile = 0)
	{
		Trace2D.TraceRectCells(start, stop, (Vector2I coords) => SetCell(coords, srcId, atlasCoords, altTile));
	}

	public void SetCellHollowRect(Vector2I start, Vector2I stop, int srcId = -1, Vector2I? atlasCoords = null, int altTile = 0)
	{
		Trace2D.TraceHollowRectCells(start, stop, (Vector2I coords) => SetCell(coords, srcId, atlasCoords, altTile));
	}
}
