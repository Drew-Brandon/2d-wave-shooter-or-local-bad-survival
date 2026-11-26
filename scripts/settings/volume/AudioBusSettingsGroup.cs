namespace Settings.Volume;
using System;
using Godot;

/// <summary>
/// Represents a group of audio busses and their affiliated settings.
/// </summary>
[GlobalClass]
public partial class AudioBusSettingsGroup : Resource
{
    [Export]
    private StringName[] _audioBusses = null;

    [Export]
    private float _maxVolume = 2f;

    /// <summary>
    /// The maximum volume that for the audio busses.
    /// </summary>
    public float MaxVolume { get => _maxVolume; }

    /// <summary>
    /// Gets the audio bus at the specified index.
    /// </summary>
    /// <param name="index">
    /// The index of the audio bus to get.
    /// </param>
    /// <returns>
    /// The audio bus at the specified index.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the index is out of range.
    /// </exception>
    public StringName GetAudioBuss(int index)
    {
        if (index < 0 || index >= _audioBusses.Length)
        {
            throw new ArgumentOutOfRangeException("index");
        }

        return _audioBusses[index];
    }

    /// <summary>
    /// Gets the volume of the audio bus group.
    /// </summary>
    /// <returns>
    /// The volume of the audio bus group.
    /// </returns>
    /// <exception cref="NullReferenceException">
    /// Thrown if the audio busses reference is null.
    /// </exception>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown if the audio busses array is empty.
    /// </exception>
    public float GetVolume()
    {
        if (_audioBusses == null)
        {
            throw new NullReferenceException("Audio busses reference cannot be null for settings group.");
        }

        if (_audioBusses.Length == 0)
        {
            throw new IndexOutOfRangeException("Audio busses array cannot be empty for settings group.");
        }

        return AudioServer.GetBusVolumeLinear(AudioServer.GetBusIndex(_audioBusses[0]));
    }

    /// <summary>
    /// Sets the volume of the audio bus group.
    /// </summary>
    /// <param name="volume">
    /// The volume to set for the audio bus group.
    /// </param>
    /// <exception cref="NullReferenceException">
    /// Thrown if the audio busses reference is null.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the volume is out of range.
    /// </exception>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown if an audio bus is not recognized by the audio server.
    /// </exception>
    public void SetVolume(float volume)
    {
        if (_audioBusses == null)
        {
            throw new NullReferenceException("Audio busses reference cannot be null for settings group.");
        }

        if (volume < 0f || volume > _maxVolume)
        {
            throw new ArgumentOutOfRangeException("volume");
        }

        for (int i = 0; i < _audioBusses.Length; i++)
        {
            int busIndex = AudioServer.GetBusIndex(_audioBusses[i]);

            if (busIndex == -1)
            {
                throw new IndexOutOfRangeException($"The bus \"{_audioBusses[i]}\" is not recognized by the audio server.");
            }

            AudioServer.SetBusVolumeLinear(busIndex, volume);
        }
    }
}
