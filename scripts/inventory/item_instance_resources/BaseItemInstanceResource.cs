namespace Inventory;
using Godot;
using Godot.Collections;

/// <summary>
/// A resource class for generating item instances.
/// </summary>
[GlobalClass, Tool]
public partial class BaseItemInstanceResource : Resource
{
	private Array<Dictionary> _instanceProperties = new();
	private Dictionary<StringName, Variant> _instanceValues = new();
	
	protected T GetGenericInstance<T>() where T : BaseItemInstance, new()
	{
		T instance = new T();
		
		foreach (System.Collections.Generic.KeyValuePair<StringName, Variant> pair in _instanceValues)
		{
			instance.Set(pair.Key, pair.Value);
		}

		return instance;
	}

	protected void InitGenericProperties<T>(StringName exclude) where T : BaseItemInstance, new()
	{
		T sample = new();
		_instanceProperties.Clear();
		_instanceValues.Clear();
		Array<Dictionary> properties = sample.GetPropertyList();

		for (int i = 0; i < properties.Count; i++)
		{
			Dictionary property = properties[i];
			StringName name = property["name"].AsStringName();

			if ((property["usage"].AsInt32() & (int)PropertyUsageFlags.ScriptVariable) > 0 && name != exclude)
			{
				_instanceProperties.Add(property);
				_instanceValues.Add(name, sample.Get(name));
			}
		}
	}

	public override Variant _Get(StringName property)
	{
		if (_instanceValues.ContainsKey(property))
		{
			return _instanceValues[property];
		}

		return new Variant();
	}

	public override bool _Set(StringName property, Variant value)
	{
		if (_instanceValues.ContainsKey(property))
		{
			_instanceValues[property] = value;
			return true;
		}

		return false;
	}

	public override Array<Dictionary> _GetPropertyList()
	{
		return _instanceProperties;
	}

	public virtual BaseItemInstance GetInstance()
	{
		return GetGenericInstance<BaseItemInstance>();
	}

	public BaseItemInstanceResource()
	{
		InitGenericProperties<BaseItemInstance>($"");
	}
}
