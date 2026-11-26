using Godot;

public static class BBUtils
{
	public static bool BlackboardValueIsValid(Blackboard bb, BBVariant val)
	{
		if (val.ValueSource == BBParam.ValueSourceEnum.BlackboardVar)
		{
			return bb.HasVar(val.Variable);
		}

		return val.SavedValue.VariantType != Variant.Type.Nil;
	}

	public static Variant GetBlackboardValue(Blackboard bb, BBVariant val)
	{
		if (val.ValueSource == BBParam.ValueSourceEnum.BlackboardVar)
		{
			return bb.GetVar(val.Variable);
		}

		return val.SavedValue;
	}

	public static T GetBlackboardValue<[MustBeVariant] T>(Blackboard bb, BBVariant val)
	{
		if (val.ValueSource == BBParam.ValueSourceEnum.BlackboardVar)
		{
			return bb.GetVar(val.Variable).As<T>();
		}

		return val.SavedValue.As<T>();
	}

	public static string GetBlackboardValueString(Blackboard bb, BBVariant val)
	{
		if (val.ValueSource == BBParam.ValueSourceEnum.BlackboardVar)
		{
			return LimboUtility.DecorateVar(val.Variable);
		}
		else if (val.SavedValue.VariantType == Variant.Type.Nil)
		{
			return "nil";
		}

		return val.SavedValue.ToString();
	}

	public static Vector2 GetBlackboardPosition(Blackboard bb, BBVariant val)
	{
		Variant bbVal = GetBlackboardValue(bb, val);

		switch (bbVal.VariantType)
		{
			case Variant.Type.Vector2:
				return bbVal.AsVector2();
			
			case Variant.Type.Object:
				return bbVal.As<Node2D>().GlobalPosition;
			
			default:
				return Vector2.Zero;
		}
	}
}
