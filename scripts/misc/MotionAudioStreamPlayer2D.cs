using Godot;
using System;
using ValueLibraries;

/// <summary>
/// An extension of the AudioStreamPlayer2D class that attempts to simulate the audio of some motion.
/// This means that the audio's pitch will scale with the player's speed and change based off the current surface.
/// </summary>
[GlobalClass]
public partial class MotionAudioStreamPlayer2D : AudioStreamPlayer2D
{
	[Export]
	private bool _autoUpdateSpeed = true;

	/// <summary>
	/// Whether or not this player should track it's own speed or rely on external input.
	/// </summary>
	public bool AutoUpdateSpeed { get => _autoUpdateSpeed; set => _autoUpdateSpeed = value; }

	[Export(PropertyHint.Range, "0,3.402823E+38,hide_control")]
	private float _minSpeed = 0.1f;

	/// <summary>
	/// The minimum speed that the player should be playing audio at.
	/// </summary>
	public float MinSpeed { get => _minSpeed; set => _minSpeed = value; }

	private float _baseVolume = 0f;

	[Export]
	private float _baseSpeed = 16f;

	/// <summary>
	/// The base speed that the player should be moving at.
	/// This acts as a reference point for how high the pitch should be.
	/// For example, if the base speed is 16 and the current speed is 16, then the pitch will be normal.
	/// If the base speed is 16 and the current speed is 32, then the pitch will be twice as high.
	/// </summary>
	public float BaseSpeed
	{
		get => _baseSpeed;

		set
		{
			_baseSpeed = value;
			UpdatePitchAndVolume();
		}
	}

	private float _curSpeed = 0f;

	/// <summary>
	/// The current speed that the player is reportedly moving at.
	/// </summary>
	public float CurSpeed
	{
		get => _curSpeed;
		
		set
		{
			if (_autoUpdateSpeed)
			{
				throw new InvalidOperationException("Cannot set the current speed when player is already automatically updating its speed.");
			}

			_curSpeed = value;

			UpdatePitchAndVolume();
			UpdatePlayingState();
		}
	}

	private Vector2 _prevPos = Vector2.Zero;

	[Export]
	private string _surfaceLayerName = "surface_type";

	[Export]
	private AudioStreamLibrary _audioLibrary = null;

	[Export]
	private TileMapLayer _tileMap = null;

	[Export]
	private GlobalReferences _references = null;

	private void UpdateSpeed(float delta)
	{
		_curSpeed = (GlobalPosition - _prevPos).Length() / delta;
		_prevPos = GlobalPosition;
		UpdatePitchAndVolume();
		UpdatePlayingState();
	}

	private void UpdatePitchAndVolume()
	{
		float newScale = _curSpeed / _baseSpeed;

		if (!Mathf.IsZeroApprox(newScale))
		{
			PitchScale = newScale;
			VolumeLinear = _baseVolume * newScale;
		}
	}

	private void UpdateStream()
	{
		// Only try changing streams if there is an assigned audio library and tilemap.
		if (_audioLibrary != null && _tileMap != null)
		{
			Vector2I cellCoords = TileMapUtils.GetCellFromGlobal(_tileMap, GlobalPosition);
			TileData tileData = _tileMap.GetCellTileData(cellCoords);
			AudioStream curStream;

			if (tileData == null)
			{
				curStream = _audioLibrary.DefVal;
			}
			else
			{
				StringName surfaceType = tileData.GetCustomData(_surfaceLayerName).AsStringName();
				curStream = _audioLibrary.GetValue(surfaceType);
			}

			if (curStream != Stream)
			{
				Stream = curStream;
			}
		}
	}

	private void UpdatePlayingState()
	{
		// Only play audio if the speed is high enough.
		if (_curSpeed >= _minSpeed)
		{
			if (!Playing)
            {
				UpdateStream();
                Play();
            }
		}
		else if (Playing)
        {
            Stop();
        }
	}

	private void OnGlobalReferencesSourceSet(GlobalReferencesSource source)
	{
		_tileMap = source.TileMap;
	}

    public override void _Ready()
    {
		if (_audioLibrary == null != (_tileMap == null && _references == null))
		{
			throw new NullReferenceException("The audio library and tile map variables should either both be null or set.");
		}

		_prevPos = GlobalPosition;
		_baseVolume = VolumeLinear;

		if (_references == null)
		{
			return;
		}
		
		GlobalReferencesSource source = _references.GetSource(GetTree());

		if (source != null)
		{
			OnGlobalReferencesSourceSet(source);
		}

		_references.SourceSet += OnGlobalReferencesSourceSet;
    }

	public override void _ExitTree()
    {
		if (_references != null)
		{
			_references.SourceSet -= OnGlobalReferencesSourceSet;
		}
    }

    public override void _PhysicsProcess(double delta)
    {
		if (_autoUpdateSpeed)
		{
			UpdateSpeed((float)delta);
		}

		if (Playing)
		{
			UpdateStream();
		}
    }
}
