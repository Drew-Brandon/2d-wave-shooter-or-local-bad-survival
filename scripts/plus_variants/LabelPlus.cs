namespace PlusVariants;
using Godot;

/// <summary>
/// An updated version of the label class which features additional functions.
/// </summary>
[GlobalClass]
public partial class LabelPlus : Label
{
	[Export]
	private string _prefix = "";

	/// <summary>
	/// Sets the label's text to the specified integer.
	/// </summary>
	/// <param name="number">
	/// The integer to set the text to.
	/// </param>
	public void SetTextWithInt(int number)
	{
		Text = number.ToString();
	}

    /// <summary>
	/// Sets the label's text to the specified float.
	/// </summary>
	/// <param name="number">
	/// The float to set the text to.
	/// </param>
	public void SetTextWithFloat(float number)
	{
		Text = number.ToString();
	}

	/// <summary>
	/// Sets the label's text to the ceilinged version of the specified float.
    /// </summary>
    /// <param name="number">
    /// The float to set the text to the ceilinged version of.
    /// </param>
    public void SetTextWithCeilFloat(float number)
	{
		Text = Mathf.CeilToInt(number).ToString();
	}

	/// <summary>
	/// Sets the text with a prefix inserted at the beginning.
	/// </summary>
	/// <param name="text">
	/// The text to set to.
	/// </param>
	public void SetTextWithPrefix(string text)
	{
		Text = _prefix + text;
	}

    /// <summary>
    /// Sets the text to an integer with a prefix inserted at the beginning.
    /// </summary>
    /// <param name="text">
    /// The integer to set the text to.
    /// </param>
    public void SetTextWithPrefixAndInt(int number)
	{
		Text = _prefix + number.ToString();
	}

    /// <summary>
    /// Sets the text to a float with a prefix inserted at the beginning.
    /// </summary>
    /// <param name="text">
    /// The float to set the text to.
    /// </param>
    public void SetTextWithPrefixAndFloat(float number)
	{
		Text = _prefix + number.ToString();
	}

    /// <summary>
    /// Sets the text to a ceilinged version of the specified float with a prefix inserted at the beginning.
    /// </summary>
    /// <param name="text">
    /// The float to set the text to the ceilinged version of.
    /// </param>
    public void SetTextWithPrefixAndCeilFloat(float number)
	{
		Text = _prefix + Mathf.CeilToInt(number).ToString();
	}
}
