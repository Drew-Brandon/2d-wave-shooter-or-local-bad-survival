using Godot;
using System;

[GlobalClass]
public partial class Spawner2D : Node2D
{
    [Export]
    private NodePathResource _spawnPath = null;

    [Export]
    private PackedScene _spawnScene = null;

    protected virtual void OnSpawn(Vector2 position, float rotation, Node2D spawned)
    {
        spawned.GlobalPosition = position;
        spawned.Rotation = rotation;
    }

    public T SpawnAt<T>(Vector2 position, float rotation) where T : Node2D
    {
        if (_spawnScene == null)
        {
            throw new NullReferenceException("Spawn scene was not set.");
        }

        T clone = _spawnScene.Instantiate<T>();
        Node sceneNode = MiscUtils.GetSceneNodeOf(this);
        Node parent = sceneNode.GetNode(_spawnPath == null ? "." : _spawnPath.Path);

        if (parent != null)
        {
            parent.AddChild(clone);
            OnSpawn(position, rotation, clone);
        }

        return clone;
    }

    public Node2D SpawnAt(Vector2 position, float rotation) 
    {
        return SpawnAt<Node2D>(position, rotation);
    }

    public T Spawn<T>() where T : Node2D
    {
        return SpawnAt<T>(GlobalPosition, GlobalRotation);
    }

    public Node2D Spawn()
    {
        return Spawn<Node2D>();
    }
}
