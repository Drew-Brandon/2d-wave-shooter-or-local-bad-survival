namespace GlobalInterfaces;
using System;
using Godot;

/// <summary>
/// A class used to open scenes or quit the game.
/// Primarily meant to be used by connecting it to signals in the editor.
/// </summary>
[GlobalClass]
public partial class SceneOpener : Node
{
	/// <summary>
	/// Opens the specified scene.
	/// </summary>
	/// <param name="scenePath">
	/// The name of the scene to open.
	/// </param>
	public void OpenScene(string scenePath)
	{
		GetTree().ChangeSceneToFile(scenePath);
	}

	/// <summary>
	/// Reloads the current scene.
	/// </summary>
	public void ReloadScene()
	{
		if (!IsInsideTree())
		{
			throw new InvalidOperationException("Cannot reload the scene as one isn't loaded.");
		}

		// Queue the scene to be reloaded in order to avoid issues.
		Callable.From(() => GetTree().ReloadCurrentScene()).CallDeferred();
	}

	/// <summary>
	/// Quit the game.
	/// </summary>
	public void Quit()
	{
		GetTree().Quit();
	}
}
