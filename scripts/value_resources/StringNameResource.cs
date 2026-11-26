using Godot;

/// <summary>
/// A resource for storing a string name.
/// This is useful for when multiple objects need to reference the same string name.
/// Note that this resource supports equality with both regular string names and string name resources.
/// It also gets the hash code from the stored name and thus can be used as a key in a dictionary.
/// </summary>
[GlobalClass]
public partial class StringNameResource : Resource
{
    [Export]
    private StringName _name = "";

    /// <summary>
    /// The stored name.
    /// </summary>
    public StringName Name { get => _name; }

    public override bool Equals(object obj)
    {
        // Supports equality with both regular string names and string name resources.
        StringName name = obj as StringName;

        if (name != null)
        {
            return Name == name;
        }

        StringNameResource nameResource = obj as StringNameResource;

        if (nameResource != null)
        {
            return Name == nameResource.Name;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return _name.GetHashCode();
    }
}
