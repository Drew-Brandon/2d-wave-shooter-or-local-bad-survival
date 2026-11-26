using Godot;
using Godot.Collections;

/// <summary>
/// A resource for storing a node path.
/// This is useful for when multiple objects need to reference the same node path.
/// </summary>
[Tool, GlobalClass]
public partial class NodePathResource : Resource
{
    private static readonly StringName PATH_PROPERTY_NAME = "path";

    private NodePath _path = new("");

    /// <summary>
    /// The stored path.
    /// </summary>
    public NodePath Path { get => _path; }

    public override Array<Dictionary> _GetPropertyList()
    {
        Dictionary pathProperty = MiscUtils.MakeProperty(PATH_PROPERTY_NAME, Variant.Type.String);
        return [pathProperty];
    }

    public override Variant _Get(StringName property)
    {
        if (property == PATH_PROPERTY_NAME)
        {
            return _path;
        }

        return default;
    }

    public override bool _Set(StringName property, Variant value)
    {
        if (property == PATH_PROPERTY_NAME)
        {
            _path = value.AsString();
            return true;
        }

        return false;
    }
}
