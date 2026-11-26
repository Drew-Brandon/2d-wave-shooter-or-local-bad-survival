namespace Inventory.ItemHandlers;
using Godot;
using Godot.Collections;
using System;
using ItemInstances;
using Inventory.Items;

/// <summary>
/// Represents a handler of an item instance.
/// This contains utilities that allows for instances to be modified in their intended way.
/// </summary>
[GlobalClass]
public partial class BaseItemHandler : RefCounted, ICloneable
{
    private bool _isEmpty = false;

    /// <summary>
    /// Whether or not this handler is empty.
    /// A handler becomes empty when their instance is fully taken.
    /// </summary>
    public bool IsEmpty { get => _isEmpty; }

    private bool _isRestricted = true;

    /// <summary>
    /// Whether or not this handler is restricted.
    /// An unrestricted handler is not subject to limits like a stack's max size.
    /// or a gun's ammo capacity.
    /// </summary>
    public bool IsRestricted { get => _isRestricted; }

    /// <summary>
    /// The type of instance this handler works with.
    /// </summary>
    public virtual Type ItemInstanceType { get => typeof(BaseItemInstance); }
    
    private BaseItemInstance _instance = null;

    /// <summary>
    /// A reference to the base type of the handler's instance.
    /// </summary>
    protected BaseItemInstance BaseInstance { get => _instance; }

    public BaseItemType ItemType { get => _instance.BaseItemType; }

    /// <summary>
    /// Triggered when this handler is updated in some way.
    /// </summary>
    [Signal]
    public delegate void UpdatedEventHandler();

    /// <summary>
    /// Triggered when this handler is emptied.
    /// </summary>
    [Signal]
    public delegate void EmptiedEventHandler();

    /// <summary>
    /// Triggered when the handler has it's instance dropped.
    /// </summary>
    /// <param name="drop">
    /// The dropped item instance.
    /// </param>
    [Signal]
    public delegate void DroppedEventHandler(DroppedItemInstance drop);

    public BaseItemHandler(BaseItemHandler handler, bool isRestricted) : base()
    {
        if (handler == null)
        {
            throw new NullReferenceException("Hanlder to copy cannot be null.");
        }

        if (!TypeUtils.CanBeType(handler, GetType()))
        {
            throw new InvalidCastException($"Cannot copy a hanlder of type {handler.GetType()} into {GetType()}.");
        }

        _isRestricted = isRestricted;
        _instance = handler._instance.CloneInstance();
    }

    /// <summary>
    /// Creates a new handler that stores a clone of the given instance.
    /// </summary>
    /// <param name="instance">
    /// The instance to clone and store within this handler.
    /// </param>
    /// <param name="isRestricted">
    /// Whether or not this handler should have restrictions placed on it.
    /// </param>
    /// <exception cref="NullReferenceException">
    /// Thrown if either the instance or it's item type reference are null.
    /// </exception>
    public BaseItemHandler(BaseItemInstance instance, bool isRestricted = true) : base()
    {
        if (instance == null)
        {
            throw new ArgumentNullException("instance");
        }

        if (instance.BaseItemType == null)
        {
            throw new NullReferenceException("Instance for handler cannot have its item be null.");
        }

        _isRestricted = isRestricted;
        _instance = (BaseItemInstance)instance.Clone();
    }

    /*/// <summary>
    /// Creates a new handler that tries to take at least some of the instance from the given
    /// handler class.
    /// </summary>
    /// <param name="handler">
    /// The handler to take the instance from.
    /// </param>
    /// <param name="isRestricted">
    /// Whether or not this handler should have restrictions placed on it.
    /// </param>
    /// <exception cref="NullReferenceException">
    /// Thrown if the given handler was null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown if the handler was empty.
    /// </exception>
    public BaseItemHandler(BaseItemHandler handler, bool isRestricted = true)
    {
        if (handler == null)
        {
            throw new NullReferenceException("Cannot take from a null handler when using it to create a new one.");
        }

        if (handler.IsEmpty)
        {
            throw new ArgumentException("Cannot take from an empty handler when using it to create a new one.");
        }

        _isRestricted = isRestricted;
        _instance = handler._instance.EmptyClone();
        TakeFrom(handler);
    }*/

    protected void Update()
    {
        EmitSignal(SignalName.Updated);
    }

    /// <summary>
    /// Empties out the handler.
    /// </summary>
    public void Empty()
    {
        _isEmpty = true;
        EmitSignal(SignalName.Emptied);
    }

    /// <summary>
    /// Gets the shortened text meant for display from the instance.
    /// </summary>
    /// <returns>
    /// The shortened text meant for display from the instance.
    /// </returns>
    public string GetShortenedDisplayText()
    {
        return _instance.GetShortenedDisplayText();
    }

    public string[] GetFullDisplayText()
    {
        return _instance.GetFullDisplayText();
    }

    public BaseItemInstance GetInstanceCopy()
    {
        return (BaseItemInstance)_instance.Clone();
    }

    public DroppedItemInstance Drop()
    {
        DroppedItemInstance drop = _instance.BaseItemType.DropScene.Instantiate<DroppedItemInstance>();
        drop.Instance = _instance;
        EmitSignal(SignalName.Dropped, drop);
        Empty();

        return drop;
    }

    /*
    /// <summary>
    /// Adds a part of this handler to the given handler.
    /// </summary>
    /// <param name="other">
    /// The other handler to add to.
    /// </param>
    /// <returns>
    /// 
    /// </returns>
    protected virtual bool OnAddTo(BaseItemHandler other)
    {
        return false;
    }

    protected virtual bool OnTakeFrom(BaseItemHandler other)
    {
        return false;
    }

    protected virtual bool OnTakeAmount(BaseItemHandler other)
    {
        return false;
    }

    /// <summary>
    /// Adds this handler's instance to the one belonging to the other given handler.
    /// </summary>
    /// <param name="other">
    /// The handler containing the instance to add to.
    /// </param>
    public bool AddTo(BaseItemHandler other)
    {
        bool handled = OnAddTo(other);

        if (handled)
        {
            return true;
        }

        handled = other.OnTakeFrom(this);

        if (handled)
        {
            return true;
        }
        else if (BaseInstance.BaseItemType == other.BaseInstance.BaseItemType)
        {
            other._instance = (BaseItemInstance)_instance.Clone();
            Empty();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Takes from the instance of the other given handler and adds it to this one.
    /// </summary>
    /// <param name="other">
    /// The handler containing the instance to take from.
    /// </param>
    public bool TakeFrom(BaseItemHandler other)
    {
        bool handled = OnTakeFrom(other);

        if (handled)
        {
            return true;
        }

        handled = other.OnAddTo(this);

        if (handled)
        {
            return true;
        }
        else if (BaseInstance.BaseItemType == other.BaseInstance.BaseItemType)
        {
            _instance = (BaseItemInstance)other._instance.Clone();
            other.Empty();
            return true;
        }

        return false;
    }

    public bool TakeAmount(BaseItemHandler other)
    {
        return OnTakeAmount(other);
    }

    public virtual BaseItemHandler TakeOff(bool applyRestrictions = true)
    {
        return new BaseItemHandler(this, applyRestrictions);
    }*/

    public object Clone()
    {
        return CloneHandler(_isRestricted);
    }

    public BaseItemHandler CloneHandler(bool applyRestrictions = true)
    {
        return (BaseItemHandler)Activator.CreateInstance(GetType(), this, applyRestrictions);
    }

    public override Array<Dictionary> _GetPropertyList()
    {
        return _instance.GetPropertyList();
    }

    public override Variant _Get(StringName property)
    {
        return _instance.Get(property);
    }

    public override bool Equals(object obj)
    {
        BaseItemHandler augmenter = obj as BaseItemHandler;

        if (augmenter == null)
        {
            return BaseInstance.Equals(augmenter._instance);
        }

        return BaseInstance.Equals(obj);
    }

    public override int GetHashCode()
    {
        return BaseInstance.GetHashCode();
    }
}
