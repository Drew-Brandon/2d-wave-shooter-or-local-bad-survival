namespace Inventory.ItemInstances;
using Godot;
using GC = Godot.Collections;
using System;
using SCG = System.Collections.Generic;
using Items;
using ItemHandlers;

/// <summary>
/// Represents an instance of an item type.
/// This contains information that can be changed during runtime.
/// </summary>
[GlobalClass, Tool]
public partial class BaseItemInstance : RefCounted, ICloneable
{
    private static readonly string ITEM_PROPERTY_NAME = "item";

    /// <summary>
    /// The type of item that this instance can store.
    /// </summary>
    public virtual Type ItemTypeClass { get => typeof(BaseItemType); }

    /// <summary>
    /// The type of handler that can use this instance.
    /// </summary>
    public virtual Type HandlerType { get => typeof(BaseItemHandler); }

    private BaseItemType _item = null;

    /// <summary>
    /// A property that references the type of item that this instance stores.
    /// Note that the assigned value for this property should have the type specified
    /// by the ItemTypeClass.
    /// </summary>
    public BaseItemType BaseItemType
    {
        get => _item;
        set
        {
            if (!TypeUtils.CanBeType(value, ItemTypeClass))
            {
                throw new InvalidCastException($"Assigned item \"{value.DisplayName}\" for instance isn't assignable to the type \"{ItemTypeClass}\".");
            }

            _item = value;
        }
    }

    /// <summary>
    /// Creates a handler that uses this instance.
    /// </summary>
    /// <returns>
    /// A handler that uses this instance.
    /// </returns>
    public virtual BaseItemHandler CreateHandlerFrom(bool isRestricted = true)
    {
        return new BaseItemHandler(this, isRestricted);
    }

    /// <summary>
	/// Adds segment(s) of text to display for this item.
	/// </summary>
	/// <param name="strings">
	/// The list of segment(s) to add to.
	/// </param>
    protected virtual void AddFullDisplayTextTo(SCG.List<string> strings)
	{
		strings.Add($"Item: {_item.DisplayName}");
	}

    /// <summary>
    /// Gets the shortened text to display.
    /// </summary>
    /// <returns>
    /// The shortened text to display.
    /// </returns>
    public virtual string GetShortenedDisplayText()
    {
        return _item.DisplayName;
    }

    /// <summary>
	/// Gets the list of text meant to display for this item.
	/// </summary>
	/// <returns>
	/// The list of text meant for display.
	/// </returns>
	public string[] GetFullDisplayText()
	{
		SCG.List<string> stringsList = new();
		AddFullDisplayTextTo(stringsList);
		return stringsList.ToArray();
	}

	public override string ToString()
	{
		return MiscUtils.FormatStringCollection(GetFullDisplayText());
	}

    public object Clone()
    {
        return CloneInstance();
    }

    public BaseItemInstance CloneInstance()
    {
        return (BaseItemInstance)MiscUtils.CloneObject(this);
    }

    /// <summary>
    /// Checks whether this instance equals the given one.
    /// </summary>
    /// <param name="instance">
    /// The instance to compare this one to.
    /// </param>
    /// <returns>
    /// Whether or not this instance was equal to the given one.
    /// </returns>
    public virtual bool EqualsInstance(BaseItemInstance instance)
    {
        return BaseItemType == instance.BaseItemType;
    }

    public override bool Equals(object obj)
    {
        BaseItemInstance instance = obj as BaseItemInstance;

        if (instance != null)
        {
            return EqualsInstance(instance);
        }
        
        return false;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override GC.Array<GC.Dictionary> _GetPropertyList()
    {
        GC.Dictionary itemProperty = MiscUtils.MakeProperty(
            ITEM_PROPERTY_NAME,
            Variant.Type.Object,
            PropertyHint.ResourceType,
            TypeUtils.GetTypeName(ItemTypeClass)
        );

        return [itemProperty];
    }

    public override Variant _Get(StringName property)
    {
        if (property == ITEM_PROPERTY_NAME)
        {
            return _item;
        }

        return default;
    }

    public override bool _Set(StringName property, Variant value)
    {
        if (property == ITEM_PROPERTY_NAME)
        {
            @BaseItemType = value.As<BaseItemType>();
            return true;
        }

        return false;
    }
}
