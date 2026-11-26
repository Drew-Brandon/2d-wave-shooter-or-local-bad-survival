using System;
using CastInfo2D;
using Godot;
using Inventory.ItemHandlers;
using Godot.Collections;

/// <summary>
/// A tool such as a weapon that can be used by a puppet body.
/// </summary>
public partial class UseableTool : Node2D
{
    [Export]
    private bool _isAuto = false;

    public bool IsAuto { get => _isAuto; }
    
    public bool IsEquipped { get => _user != null; }

    [Export]
    private uint _useCooldown = 500;
    public uint UseCooldown { get => _useCooldown; }

    private ulong _prevUseTime = 0;

    [Export]
    private float _range = 256f;
    public float Range { get => _range; }

    public virtual Type ItemHandlerType { get => typeof(BaseItemHandler); }
    
    private BaseItemHandler _itemHandler = null;
    public BaseItemHandler ItemHandler { get => _itemHandler; }

    public virtual Type UserType { get => typeof(Node2D); }

    [Export]
    private Node2D _user = null;
    public Node2D User { get => _user; }

    [Export]
    public RayCastInfo2D _lineOfSightRayCast = null;

    [Signal]
    public delegate void EquippedEventHandler(Node2D user);

    [Signal]
    public delegate void UnequippedEventHandler(Node2D user);

    [Signal]
    public delegate void UsedEventHandler(Vector2 usePosition, Node2D user);

    [Signal]
    public delegate void FailedUseEventHandler(Vector2 usePosition, Node2D user);

    [Signal]
    public delegate void SuccessfullyUsedEventHandler(Vector2 usePosition, Node2D user);

    public virtual bool CanUse()
    {
        return true;
    }

    public virtual bool ShouldUse(Vector2 usePosition, CollisionObject2D targetCollision = null)
    {
        if (_user.GlobalPosition.DistanceTo(usePosition) > _range)
        {
            return false;
        }

        PhysicsRayQueryParameters2D query = _lineOfSightRayCast.CreateQuery(_user.GlobalPosition, usePosition);
        PhysicsDirectSpaceState2D state = GetWorld2D().DirectSpaceState;
        RayCastResults2D results = RayCastInfo2D.IntersectRay(state, query);
        return results.Collider == targetCollision;
    }

    protected bool AttemptUse()
    {
        ulong currentTime = Time.GetTicksMsec();

        if (currentTime - _prevUseTime >= _useCooldown && (_itemHandler == null || ChargeHandler()) && CanUse())
        {
            _prevUseTime = currentTime;
            return true;
        }

        return false;
    }

    protected virtual void OnUse(Vector2 usePosition)
    {
        EmitSignal(SignalName.Used, usePosition, _user);
    }

    protected virtual void OnFailedUse(Vector2 usePosition)
    {
        EmitSignal(SignalName.FailedUse, usePosition, _user);
    }

    protected virtual void OnSuccessfullyUsed(Vector2 usePosition)
    {
        EmitSignal(SignalName.SuccessfullyUsed, usePosition, _user);
    }

    protected virtual bool ChargeHandler()
    {
        return true;
    }

    public bool Use(Vector2 usePosition)
    {
        if (AttemptUse())
        {
            OnUse(usePosition);
            OnSuccessfullyUsed(usePosition);
            return true;
        }
        
        OnUse(usePosition);
        OnFailedUse(usePosition);
        return false;
    }

    protected virtual bool OnHandlerSet(BaseItemHandler handler)
    {
        return true;
    }

    public void SetHandler(BaseItemHandler handler)
    {
        if (_itemHandler != null)
        {
            _itemHandler.Emptied -= QueueFree;
        }

        if (!TypeUtils.CanBeType(handler, ItemHandlerType))
        {
            throw new InvalidCastException($"Cannot assign item handler of type {ItemHandlerType} to a value of type {handler.GetType()}.");
        }

        if (!OnHandlerSet(handler))
        {
            return;
        }

        _itemHandler = handler;

        if (_itemHandler != null)
        {
            _itemHandler.Emptied += QueueFree;
        }
    }

    protected virtual bool OnEquip(Node2D user)
    {
        return true;
    }

    public void Equip(Node2D user)
    {
        if (user == null)
        {
            throw new ArgumentNullException("user");
        }

        if (!TypeUtils.IsOfType(user, UserType))
        {
            throw new InvalidCastException($"Cannot assign user of type {UserType} to a value of type {user.GetType()}.");
        }

        if (!OnEquip(user))
        {
            return;
        }

        _user = user;
        EmitSignal(SignalName.Equipped, _user);
    }

    protected virtual bool OnUnequip()
    {
        return true;
    }

    public void Unequip()
    {
        if (_user == null)
        {
            throw new InvalidOperationException("Cannot unequip tool because it is not currently equipped.");
        }

        if (!OnUnequip())
        {
            return;
        }

        Node2D previousUser = _user;
        _user = null;
        EmitSignal(SignalName.Unequipped, previousUser);
    }

    public override void _Ready()
    {
        _prevUseTime = Time.GetTicksMsec();
    }
}
