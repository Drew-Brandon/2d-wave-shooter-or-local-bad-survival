namespace Inventory;
using Godot;

public partial class GunInstance : ItemInstanceGeneric<GunItem>
{
	/// <summary>
	/// The amount of ammo currently in the gun's clip.
	/// </summary>
	[Export]
	public int ClipAmmo = 0;

	public GunInstance() : base()
	{
		ClipAmmo = 0;
	}

	public GunInstance(int clipAmmo, GunItem item) : base(item)
	{
		ClipAmmo = clipAmmo;
	}
}
