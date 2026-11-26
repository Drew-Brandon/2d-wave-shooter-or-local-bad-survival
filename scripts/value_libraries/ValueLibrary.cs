namespace ValueLibraries;
using System;
using Godot;
using Godot.Collections;

/// <summary>
/// Represents a library of values of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">
/// The type of values in the library.
/// </typeparam>
[Tool]
public abstract partial class ValueLibrary<[MustBeVariant] T> : Resource
{
    private static readonly StringName DEFAULT_VALUE_PROPERTY_NAME = "default_value";
    private static readonly StringName LIBRARY_PROPERTY_NAME = "library";

    private T _defVal;

    /// <summary>
    /// Gets the default value of the library.
    /// This is returned when a value is not found in the library.
    /// </summary>
    public T DefVal { get => _defVal; }

    private Dictionary<StringName, T> _library = null;

    /// <summary>
    /// Checks whether the library contains a value with the specified name.
    /// </summary>
    /// <param name="valueName">
    /// The name of the value to check.
    /// </param>
    /// <returns>
    /// Whether or not the library contains a value with the specified name.
    /// </returns>
    public bool HasValue(StringName valueName)
    {
        return _library.ContainsKey(valueName);
    }

    /// <summary>
    /// Gets the value with the specified name.
    /// </summary>
    /// <param name="valueName">
    /// The name of the value to get.
    /// </param>
    /// <returns>
    /// The value with the specified name, or the default value if it is not found.
    /// </returns>
    public T GetValue(StringName valueName)
    {
        if (HasValue(valueName))
        {
            return _library[valueName];
        }
        
        return _defVal;
    }
    
    public override Array<Dictionary> _GetPropertyList()
    {
        // Get the type information for the default value.
        Type valueType = typeof(T);
        string valueTypeString = TypeUtils.GetTypeName(valueType);
        Variant.Type valueVariantType = TypeUtils.GetVariantType(valueType);
        PropertyHint valueHint = PropertyHint.None;
        string valueHintString = "";

        // If the value type is a GodotObject, ensure it is a Resource and change the property hint to reflect so.
        if (valueVariantType == Variant.Type.Object)
        {
            if (!valueType.IsSubclassOf(typeof(Resource)))
            {
                throw new InvalidOperationException("ValueLibrary cannot have GodotObject types unless they are resources.");
            }

            valueHint = PropertyHint.ResourceType;
            valueHintString = valueTypeString;
        }

        Dictionary defaultValueProperty = MiscUtils.MakeProperty(
            DEFAULT_VALUE_PROPERTY_NAME,
            valueVariantType,
            valueHint,
            valueHintString
        );

        Dictionary libraryProperty = MiscUtils.MakeProperty(
            LIBRARY_PROPERTY_NAME,
            Variant.Type.Dictionary,
            PropertyHint.DictionaryType,
            $"StringName;{valueTypeString}"
        );

        return [defaultValueProperty, libraryProperty];
    }

    public override Variant _Get(StringName property)
    {
        if (property == DEFAULT_VALUE_PROPERTY_NAME)
        {
            return Variant.From(_defVal);
        }
        else if (property == LIBRARY_PROPERTY_NAME)
        {
            return _library;
        }

        return default;
    }

    public override bool _Set(StringName property, Variant val)
    {
        if (property == DEFAULT_VALUE_PROPERTY_NAME)
        {
            _defVal = val.As<T>();
            return true;
        }
        else if (property == LIBRARY_PROPERTY_NAME)
        {
            _library = val.As<Dictionary<StringName, T>>();
            return true;
        }

        return false;
    }

    public override bool _PropertyCanRevert(StringName property)
    {
        return property == DEFAULT_VALUE_PROPERTY_NAME || property == LIBRARY_PROPERTY_NAME;
    }
}
