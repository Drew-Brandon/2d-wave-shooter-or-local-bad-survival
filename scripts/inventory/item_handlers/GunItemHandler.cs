namespace Inventory.ItemHandlers;
using Godot;
using System;
using ItemInstances;
using Inventory.Items;

[GlobalClass]
public partial class GunItemHandler : BaseItemHandler
{
    public int Ammo { get => GunInstance.Ammo; }

    public GunItemType @GunItemType { get => GunInstance.GunItemType; }

    public override Type ItemInstanceType { get => typeof(GunItemInstance); }

    protected GunItemInstance GunInstance { get => (GunItemInstance)BaseInstance; }

    public GunItemHandler(BaseItemHandler handler, bool isRestricted) : base(handler, isRestricted)
    { }

    public GunItemHandler(GunItemInstance instance, bool isRestricted = true) : base(instance, isRestricted)
    {
        SeekAmmo(instance.Ammo);
    }

    public void SeekAmmo(int targetAmmo)
    {
        if (targetAmmo <= 0)
        {
            GunInstance.Ammo = 0;
        }
        else if (IsRestricted && targetAmmo > GunInstance.GunItemType.MaxAmmo)
        {
            GunInstance.Ammo = GunInstance.GunItemType.MaxAmmo;
        }
        else
        {
            GunInstance.Ammo = targetAmmo;
        }

        Update();
    }

    public int AddAmmo(int amount)
    {
        int prevAmmo = GunInstance.Ammo;
        int targetAmmo = GunInstance.Ammo + amount;
        SeekAmmo(targetAmmo);
        return GunInstance.Ammo - prevAmmo;
    }

    public void InsertRoundWith(IndexedInventory inventory)
    {
        StackableItemInstance ammoNeeded = new()
        {
            Amount = 1,
            StackableItemType = GunItemType.AmmoType
        };
        
        StackableItemHandler ammoHandler = new(ammoNeeded, false);
        inventory.TryToTake(ammoHandler);
        
        AddAmmo(ammoNeeded.Amount - ammoHandler.Amount);
    }

    public void FullReloadWith(IndexedInventory inventory)
    {
        StackableItemInstance ammoNeeded = new()
        {
            Amount = GunItemType.MaxAmmo - GunInstance.Ammo,
            StackableItemType = GunItemType.AmmoType
        };
        
        StackableItemHandler ammoHandler = new(ammoNeeded, false);
        inventory.TryToTake(ammoHandler);
        
        AddAmmo(ammoNeeded.Amount - ammoHandler.Amount);
    }

    /*public override BaseItemHandler TakeOff(bool applyRestrictions = true)
    {
        return new GunItemHandler(this, applyRestrictions);
    }*/
}
