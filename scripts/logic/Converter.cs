using Godot;

[GlobalClass]
public partial class Converter : Node
{
	[Export]
	private Variant.Type _conversionType = Variant.Type.Nil;

	[Signal]
	public delegate void OnConversionEventHandler(Variant converted);

	public Variant Convert(Variant val)
	{
		Variant converted = GD.Convert(val, _conversionType);
		EmitSignal(SignalName.OnConversion, converted);
		return converted;
	}
}
