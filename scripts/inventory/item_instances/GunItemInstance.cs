namespace Inventory.ItemInstances;
using Godot;
using System;
using System.Collections.Generic;
using Items;
using Inventory.ItemHandlers;

[GlobalClass, Tool]
public partial class GunItemInstance : BaseItemInstance
{
	public override Type ItemTypeClass { get => typeof(GunItemType); }

    public override Type HandlerType { get => typeof(GunItemHandler); }

	/// <summary>
	/// The amount of ammo currently in the gun's clip.
	/// </summary>
	[Export]
	public int Ammo = 0;

	public GunItemType @GunItemType { get => (GunItemType)BaseItemType; set => BaseItemType = value; }

    protected override void AddFullDisplayTextTo(List<string> strings)
    {
        base.AddFullDisplayTextTo(strings);
        strings.Add($"Clip Ammo: {Ammo}/{GunItemType.MaxAmmo}");
        //string ammoTypeStrings = MiscUtils.FormatStringCollection(GunItemType.GetAmmoTypeStrings(), "\t");
        strings.Add($"Ammo Type: {GunItemType.AmmoType}");
        strings.Add($"Base Damage: {GunItemType.Projectile.BaseDamage}");
    }

    public override string GetShortenedDisplayText()
    {
        return $"{Ammo}";
    }

    public override bool EqualsInstance(BaseItemInstance instance)
    {
        bool equalBase = base.EqualsInstance(instance);

        if (!equalBase)
        {
            return false;
        }

        GunItemInstance gun = instance as GunItemInstance;
        return gun != null && Ammo == gun.Ammo;
    }

    public override BaseItemHandler CreateHandlerFrom(bool isRestricted = true)
    {
        return new GunItemHandler(this, isRestricted);
    }
}
