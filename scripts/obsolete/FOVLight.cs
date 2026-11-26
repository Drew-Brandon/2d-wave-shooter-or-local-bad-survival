using Godot;

[Tool]
public partial class FOVLight : PointLight2D
{
	[Export]
	private Vector2I _editorViewSize = new(1152, 648);

	[Export]
	private FOVTextureGenerator _fovGen = null;

	[Export]
	private Camera2D _cam;

	[ExportToolButton("Update Texture")]
	private Callable _updateTexture => Callable.From(() => UpdateTexture(GetSizeSetting()));

	private Vector2I GetSizeSetting()
	{
		int width = ProjectSettings.GetSetting("display/window/size/viewport_width").AsInt32();
		int height = ProjectSettings.GetSetting("display/window/size/viewport_height").AsInt32();
		return new Vector2I(width, height);
	}


	private void UpdateTexture(Vector2I viewSize)
	{
		float scale = 1f / _cam.Zoom.X;
		Texture = _fovGen.CreateFullScreenTexture(viewSize, scale);
		TextureScale = scale;
	}

	public void UpdateTexture()
	{
		UpdateTexture(GetWindow().Size);
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// UpdateTexture(GetWindow().Size);
	}
}
