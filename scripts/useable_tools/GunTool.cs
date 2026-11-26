using Godot;
using Inventory;
using Inventory.ItemHandlers;
using Inventory.ItemInstances;
using System;
using System.Collections;

public partial class GunTool : PreciseUseableTool
{
    protected bool InternalIsReloading = false;
    public bool IsReloading { get => InternalIsReloading; }
    public bool IsCancellingReload { get => _reloadCoroutine.IsCancelled; }

    [Export]
    private int _reloadTime = 1000;
    public int ReloadTime { get => _reloadTime; }

    public override Type ItemHandlerType { get => typeof(GunItemHandler); }

    public GunItemHandler @GunItemHandler { get => (GunItemHandler)ItemHandler; }

    private CoroutineSource _reloadCoroutine = new();

    private CoroutineSource _cancelReloadCoroutine = new();

    [Signal]
    public delegate void StartedReloadEventHandler();

    [Signal]
    public delegate void CanceledReloadEventHandler();

    [Signal]
    public delegate void FinishedReloadEventHandler();

    public override bool CanUse()
    {
        return !InternalIsReloading && !IsCancellingReload;;
    }

    protected override bool ChargeHandler()
    {
        if (GunItemHandler.Ammo > 0)
        {
            GunItemHandler.AddAmmo(-1);
            return true;
        }
        
        return false;
    }

    protected virtual IEnumerable ReloadRoutine(IndexedInventory inventory)
    {
        yield return _reloadTime;
        int ammoBeforeReload = GunItemHandler.Ammo;
        GunItemHandler.FullReloadWith(inventory);
        InternalIsReloading = false;

        if (ammoBeforeReload != GunItemHandler.Ammo)
        {
            EmitSignal(SignalName.FinishedReload);
        }
    }

    protected void ResetReload()
    {
        _reloadCoroutine = new();
    }

    protected virtual IEnumerable CancelReloadRoutine()
    {
        yield return (int)UseCooldown;
        ResetReload();
    }

    public void Reload(IndexedInventory inventory)
    {
        if (InternalIsReloading)
        {
            throw new InvalidOperationException("Gun is already reloading.");
        }

        InternalIsReloading = true;
        _reloadCoroutine.StartCoroutine(ReloadRoutine(inventory));
        EmitSignal(SignalName.StartedReload);
    }

    public void CancelReload(IndexedInventory inventory)
    {
        if (!InternalIsReloading)
        {
            throw new InvalidOperationException("Cannot cancel gun from reloading as it is not doing so.");
        }

        if (IsCancellingReload)
        {
            throw new InvalidOperationException("Gun is already cancelling reload.");
        }

        InternalIsReloading = false;
        _reloadCoroutine.IsCancelled = true;
        _cancelReloadCoroutine.StartCoroutine(CancelReloadRoutine());
        EmitSignal(SignalName.CanceledReload);
    }

    public override void _ExitTree()
    {
        _reloadCoroutine.IsCancelled = true;
        _cancelReloadCoroutine.IsCancelled = true;
    }
}
