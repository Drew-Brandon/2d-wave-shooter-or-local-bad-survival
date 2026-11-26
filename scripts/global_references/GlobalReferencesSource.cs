using Godot;
using System;

[GlobalClass]
public partial class GlobalReferencesSource : Node
{
    [Export]
    private TileMapLayer _tileMap = null;
    public TileMapLayer TileMap { get => _tileMap; }

    [Export]
    private GlobalReferences _globalReferences = null;

    public override void _Ready()
    {
        if (_globalReferences == null)
        {
            throw new NullReferenceException("Global references cannot be null for a source node.");
        }

        _globalReferences.SetSource(this);
    }
}
