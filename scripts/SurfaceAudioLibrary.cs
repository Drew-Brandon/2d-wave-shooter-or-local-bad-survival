using Godot;
using Godot.Collections;
using System.IO;

[GlobalClass]
public partial class SurfaceAudioLibrary : Resource
{
	[Export]
	private string _surfaceTypeName = "surface_data";
	public string SurfaceTypeName { get => _surfaceTypeName; }

	[Export]
	private AudioStream _defAudio = null;

	[Export]
	private Dictionary<StringName, AudioStream> _surfaceAudio = new();

	public AudioStream GetStream(StringName surface)
	{
		if (_surfaceAudio.ContainsKey(surface))
		{
			return _surfaceAudio[surface];
		}

		return _defAudio;
	}

	public AudioStream GetStream(TileMapLayerPlus tileMap, Vector2 pos)
	{
		TileData data = tileMap.GetCellTileDataGlobal(pos);

		if (data != null && data.HasCustomData(_surfaceTypeName))
		{
			Variant surface = data.GetCustomData(_surfaceTypeName);

			if (surface.VariantType == Variant.Type.StringName)
			{
				return GetStream(surface.AsStringName());
			}
		}

		return _defAudio;
	}

	public void UpdatePlayer(AudioStreamPlayer2D player, TileMapLayerPlus tileMap, float speed, float minSpeed, float baseSpeed)
	{
		if (tileMap != null && speed >= minSpeed)
		{
			/* 
			 * Get the audio track that should play when walking over the tile cell the puppet
			 * is currently right above.
			 */
			AudioStream stream = GetStream(tileMap, player.GlobalPosition);

			if (stream != player.Stream)
			{
				player.Stream = stream;
			}

			// Adjust the pitch to scale with the puppet's speed.
            player.PitchScale = speed / baseSpeed;

            if (!player.Playing)
            {
                player.Play();
            }
        }
        else if (player.Playing)
        {
            player.Stop();
			player.Stream = null;
        }
    }
}
