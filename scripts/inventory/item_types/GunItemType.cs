namespace Inventory.Items;
using Godot;
using AttackInfo;
using Inventory.ItemInstances;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class GunItemType : BaseItemType
{
	public override Type ItemInstanceType { get => typeof(GunItemInstance); }

	[Export]
	private int _maxAmmo = 10;

	/// <summary>
	/// The maximum amount of ammo that can be in a single clip.
	/// </summary>
	public int MaxAmmo { get => _maxAmmo; }

	[Export]
	private ProjectileInfo2D _projectile = null;
	public ProjectileInfo2D Projectile { get => _projectile; }

	[Export]
	private StackableItemType _ammoType = null;
	public StackableItemType AmmoType { get => _ammoType; }

	/*public StackableItemType GetAmmoType(int index)
	{
		return _ammoTypes[index];
	}

	public string[] GetAmmoTypeStrings()
	{
		string[] ammoTypeStrings = new string[_ammoTypes.Length];

		for (int i = 1; i < _ammoTypes.Length; i++)
		{
			ammoTypeStrings[i] = _ammoTypes[i].DisplayName;
		}

		return ammoTypeStrings;
	}*/

	protected override void AddFullDisplayTextTo(List<string> strings)
	{
		base.AddFullDisplayTextTo(strings);
		strings.Add($"Clip Size: {_maxAmmo}");

		//string ammoTypeStrings = MiscUtils.FormatStringCollection(GetAmmoTypeStrings(), "\t");
		strings.Add($"Ammo Type: {_ammoType.DisplayName}");
	}
}
