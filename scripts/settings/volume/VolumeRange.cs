namespace Settings.Volume;
using Godot;
using System;

/// <summary>
/// A range control for adjusting the volume of a group of audio busses.
/// </summary>
public partial class VolumeRange : Godot.Range
{
	[Export]
    private AudioBusSettingsGroup _busGroup = null;

	private void OnValueChanged(double value)
	{
		try
		{
			_busGroup.SetVolume((float)value);
		}
		catch (Exception exception)
		{
			throw new NodeException(GetPath(), exception);
		}
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (_busGroup == null)
        {
            throw new NodeException(
				GetPath(),
				new NullReferenceException("Volume editor must have a reference to a bus settings group.")
			);
        }

		MinValue = 0f;

		try
		{
			Value = _busGroup.GetVolume();
		}
		catch (Exception exception)
		{
			throw new NodeException(GetPath(), exception);
		}
		
		MaxValue = _busGroup.MaxVolume;
		ValueChanged += OnValueChanged;
	}
}
