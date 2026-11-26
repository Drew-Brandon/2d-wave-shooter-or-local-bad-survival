namespace Inventory.Items;
using Godot;
using Inventory.ItemInstances;
using System;
using System.Collections.Generic;

/// <summary>
/// Represents the base type of an item.
/// This defines the properties of an item that are effectively constant
/// and shouldn't be changed at runtime.
/// </summary>
[GlobalClass]
public partial class BaseItemType : Resource
{
	public virtual Type ItemInstanceType { get => typeof(BaseItemInstance); }

	[Export]
	private StringName _displayName = "";

	/// <summary>
	/// The name of this item.
	/// </summary>
	public StringName DisplayName { get => _displayName; }

	[Export]
	private Texture2D _icon;

	/// <summary>
	/// The icon used for this item.
	/// </summary>
	public Texture2D Icon { get => _icon; }
	
	[Export]
	private PackedScene _equipScene = null;

	/// <summary>
	/// The scene to instantaniate when the item is equipped.
	/// </summary>
	public PackedScene EquipScene { get => _equipScene; }

	[Export]
	private PackedScene _dropScene = null;
	public PackedScene DropScene { get => _dropScene; }

	/// <summary>
	/// Adds segment(s) of text to display for this item.
	/// </summary>
	/// <param name="strings">
	/// The list of segment(s) to add to.
	/// </param>
	protected virtual void AddFullDisplayTextTo(List<string> strings)
	{
		strings.Add(DisplayName);
	}

	/// <summary>
	/// Gets the list of text meant to display for this item.
	/// </summary>
	/// <returns>
	/// The list of text meant for display.
	/// </returns>
	public string[] GetFullDisplayText()
	{
		List<string> stringsList = new();
		AddFullDisplayTextTo(stringsList);
		return stringsList.ToArray();
	}

	public override string ToString()
	{
		return MiscUtils.FormatStringCollection(GetFullDisplayText());
	}
}
