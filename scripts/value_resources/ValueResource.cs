using Godot;
using Godot.Collections;

public partial class ValueResource<[MustBeVariant] T> : Resource
{
    private static readonly StringName VALUE_PROPERTY_NAME = "value";

    private Variant _value;

    public T Value { get => _value.As<T>(); }

    protected virtual Variant.Type ExportedType { get => TypeUtils.GetVariantType(typeof(T)); }

    protected virtual PropertyHint PropertyHint { get => PropertyHint.None; }

    protected virtual string PropertyHintString { get => ""; }

    public override Array<Dictionary> _GetPropertyList()
    {
        Dictionary valueProperty = MiscUtils.MakeProperty(
            VALUE_PROPERTY_NAME,
            ExportedType,
            PropertyHint,
            PropertyHintString
        );

        return [valueProperty];
    }

    public override Variant _Get(StringName property)
    {
        if (property == VALUE_PROPERTY_NAME)
        {
            return _value;
        }

        return default;
    }

    public override bool _Set(StringName property, Variant value)
    {
        if (property == VALUE_PROPERTY_NAME && value.VariantType == ExportedType)
        {
            _value = value;
            return true;
        }

        return false;
    }
}
