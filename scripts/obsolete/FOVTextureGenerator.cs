using Godot;

[GlobalClass, Tool]
public partial class FOVTextureGenerator : Resource
{
    [Export]
    private float _fovOriginRadius = 48f;

    private float _fovAngleRad = Mathf.DegToRad(70f);

    [Export]
    private float _fovAngleDeg { get => Mathf.RadToDeg(_fovAngleRad); set => _fovAngleRad = Mathf.DegToRad(value); }

    [Export]
    private Color _fovColor = new Color(1f, 1f, 1f);

    [Export]
    private Color _outterColor = new Color(0f, 0f, 0f);

    [Export]
    private Image.Format _imgFormat = Image.Format.L8;

    [Export]
    private PortableCompressedTexture2D.CompressionMode _compressionMode = PortableCompressedTexture2D.CompressionMode.Lossless;

    private Color GetPixelColor(Vector2I origin, Vector2I pixelPos, float pixelSize)
    {
        Vector2I disp = pixelPos - origin;

        if (disp.Length() * pixelSize <= _fovOriginRadius)
        {
            return _fovColor;
        }

        float angle = Mathf.Atan2(disp.Y, disp.X);

        if (angle <= _fovAngleRad / 2f && angle >= -_fovAngleRad / 2f)
        {
            return _fovColor;
        }

        return _outterColor;
    }

    private PortableCompressedTexture2D CreateTexture(Vector2I textureSize, float pixelSize)
    {
        Vector2I origin = textureSize / 2;
        Image img = Image.CreateEmpty(textureSize.X, textureSize.Y, false, _imgFormat);

        for (int x = 0; x < textureSize.X; x++)
        {
            for (int y = 0; y < textureSize.Y; y++)
            {
                Vector2I pixelPos = new(x, y);
                Color pixelColor = GetPixelColor(origin, pixelPos, pixelSize);
                img.SetPixelv(pixelPos, pixelColor);
            }
        }

        PortableCompressedTexture2D compressed = new();
        compressed.CreateFromImage(img, _compressionMode);
        return compressed;
    }

    public PortableCompressedTexture2D CreateFullScreenTexture(Vector2I viewSize, float pixelSize)
    {
        Vector2 halfSize = (Vector2)viewSize / 2;
        float viewRadius = halfSize.Length();
        Vector2 adjustment = Vector2.One * viewRadius - halfSize;
        Vector2I textureSize = (Vector2I)(viewSize + 2f * adjustment).Ceil();

        return CreateTexture(textureSize, pixelSize);
    }
}
