using System;
using System.Linq;
using System.Reflection;
using SCG = System.Collections.Generic;
using Godot;
using GC = Godot.Collections;

/// <summary>
/// A resource class for generating item instances.
/// </summary>
[Tool]
public partial class GodotObjectBlueprint<T> : Resource where T : GodotObject
{
	/// <summary>
	/// The name that should be used for the type property.
	/// </summary>
	protected static readonly StringName TYPE_PROPERTY_NAME = "object_type";
	protected static readonly StringName SCRIPT_PROPERTY_NAME = "script";

	private int _curTypeIndex = 0;
	private string _typesString = "";
	private Type[] _typesArray = null;
	
	private GC.Array<GC.Dictionary> _objectPropertyList = new();
	private GC.Dictionary<StringName, PropertyValueInfo> _objectPropertyValues = new();
	
	private void AddType(int curTypeCount, Type type)
	{
		if (!type.IsAbstract)
		{
			if (curTypeCount != 0)
			{
				_typesString += ",";
			}

			// Add the type to the string and array.
			string typeStr = type.ToString();
			_typesString += typeStr;

			_typesArray[curTypeCount] = type;
		}
	}

	private void AddAllTypes()
	{
		// Get all of the child classes that of this classes's generic type.
		SCG.IEnumerable<Type> childTypes = Assembly
			.GetAssembly(typeof(T))
			.GetTypes()
			.Where(t => t.IsSubclassOf(typeof(T)));
		int typeArrayLength = childTypes.Count() + 1;
		
		// Reset the type related variables and add the generic type.
		_typesString = "";
		_typesArray = new Type[typeArrayLength];
		AddType(0, typeof(T));
		
		// Add all of the child classes types.
		for (int i = 0; i < childTypes.Count(); i++)
		{
			Type type = childTypes.ElementAt(i);
			AddType(i + 1, type);
		}
	}

	/// <summary>
	/// Creates the object that the blueprint is specified to create.
	/// </summary>
	/// <returns>
	/// The created object.
	/// </returns>
	public T CreateObject()
	{
		// Get the type of object to create and instantaniate it.
		T instance = (T)Activator.CreateInstance(_typesArray[_curTypeIndex]);
		
		// Fill out the object's properties.
		foreach (SCG.KeyValuePair<StringName, PropertyValueInfo> pair in _objectPropertyValues)
		{
			instance.Set(pair.Key, pair.Value.CurrentValue);
		}

		return instance;
	}

	/// <summary>
	/// Initializes the object's array of types and 
	/// </summary>
	public GodotObjectBlueprint()
	{
		AddAllTypes();
		AddObjectProperties(_typesArray[_curTypeIndex]);
	}
	
	private void AddObjectProperties(Type instanceType)
	{
		// Create an instance of the affiliated object's class and copy over all of its properties.
		T sample = (T)Activator.CreateInstance(instanceType);
		GC.Array<GC.Dictionary> properties = sample.GetPropertyList();

		// Add all of the object's properties that are used for the editor.
		for (int i = 0; i < properties.Count; i++)
		{
			GC.Dictionary property = properties[i];
			StringName propertyName = property["name"].AsStringName();

			if ((property["usage"].AsInt32() & (int)PropertyUsageFlags.Editor) > 0 &&
				propertyName != SCRIPT_PROPERTY_NAME)
			{
				Variant defaultValue = sample.Get(propertyName);
				PropertyValueInfo propertyValueInfo = new()
				{
					DefaultValue = defaultValue,
					CurrentValue = defaultValue
				};

				_objectPropertyList.Add(property);
				_objectPropertyValues.Add(propertyName, propertyValueInfo);
			}
		}
	}

	private void UpdateProperties()
	{
		_objectPropertyList.Clear();
		_objectPropertyValues.Clear();

		AddObjectProperties(_typesArray[_curTypeIndex]);
		NotifyPropertyListChanged();
	}

	public override Variant _Get(StringName property)
	{
		if (property == TYPE_PROPERTY_NAME)
		{
			return _curTypeIndex;
		}
		else if (_objectPropertyValues.ContainsKey(property))
		{
			return _objectPropertyValues[property].CurrentValue;
		}

		return default;
	}

	public override bool _Set(StringName property, Variant value)
	{
		// If the set property was defining the type of object being used, than update the properties list.
		if (property == TYPE_PROPERTY_NAME)
		{
			_curTypeIndex = value.AsInt32();
			UpdateProperties();
		}
		else if (_objectPropertyValues.ContainsKey(property))
		{
			_objectPropertyValues[property].CurrentValue = value;
			return true;
		}

		return false;
	}

	public override GC.Array<GC.Dictionary> _GetPropertyList()
	{
		GC.Dictionary typeProperty = MiscUtils.MakeProperty(
			TYPE_PROPERTY_NAME,
			Variant.Type.Int,
			PropertyHint.Enum,
			_typesString
		);

		GC.Array<GC.Dictionary> propertyList = _objectPropertyList.Duplicate();
		propertyList.Insert(0, typeProperty);

		return propertyList;
	}

    public override bool _PropertyCanRevert(StringName property)
    {
        return property == TYPE_PROPERTY_NAME || _objectPropertyValues.ContainsKey(property);
    }

    public override Variant _PropertyGetRevert(StringName property)
    {
		if (property == TYPE_PROPERTY_NAME)
		{
			return 0;
		}
        else if (_objectPropertyValues.ContainsKey(property))
		{
			return _objectPropertyValues[property].DefaultValue;
		}

		return default;
    }
}
