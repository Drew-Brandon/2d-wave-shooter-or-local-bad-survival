using Godot;
using Inventory;
using System.Collections;

public partial class ManualReloadGunTool : GunTool
{
    [Export]
    private int _finishReloadTime = 300;

    [Signal]
    public delegate void RoundLoadedEventHandler();

    protected override IEnumerable ReloadRoutine(IndexedInventory inventory)
    {
        int ammoBeforeReload = GunItemHandler.Ammo;

        while (GunItemHandler.Ammo < GunItemHandler.GunItemType.MaxAmmo)
        {
            yield return ReloadTime;

            int previousAmmo = GunItemHandler.Ammo;
            GunItemHandler.InsertRoundWith(inventory);

            if (previousAmmo == GunItemHandler.Ammo)
            {
                break;
            }

            EmitSignal(SignalName.RoundLoaded);
        }

        yield return _finishReloadTime;
        InternalIsReloading = false;

        if (ammoBeforeReload != GunItemHandler.Ammo)
        {
            EmitSignal(SignalName.FinishedReload);
        }
    }

    protected override IEnumerable CancelReloadRoutine()
    {
        yield return UseCooldown + _finishReloadTime;
        ResetReload();
    }
}
