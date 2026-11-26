namespace Inventory;
using Godot;
using ItemInstances;

/// <summary>
/// A blueprint for generating item instances.
/// </summary>
[GlobalClass, Tool]
public partial class ItemInstanceBlueprint : GodotObjectBlueprint<BaseItemInstance>;
