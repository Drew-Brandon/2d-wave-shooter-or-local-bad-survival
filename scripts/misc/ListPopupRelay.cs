using Godot;
using System;

public partial class ListPopupRelay : Node
{
    private int _curListingCount = 0;

    public int CurListingCount { get => _curListingCount; }

    [Export(PropertyHint.MultilineText)]
    private string _elementStart = "";

    [Export(PropertyHint.MultilineText)]
    private string _elementDelimiter = "\n";

    [Export(PropertyHint.MultilineText)]
    private string _elementEnd = "";

    [Signal]
    public delegate void ListRecievedEventHandler(string text);

    [Signal]
    public delegate void ListLostEventHandler();

    public void SendList(string[] strings)
    {
        if (strings == null)
        {
            throw new NullReferenceException("Cannot send a null list to popup.");
        }

        _curListingCount++;
        string formatted = MiscUtils.FormatStringCollection(strings, _elementStart, _elementDelimiter, _elementEnd);
        EmitSignal(SignalName.ListRecieved, formatted);
    }

    public void LoseList()
    {
        if (_curListingCount == 0)
        {
            throw new InvalidOperationException("Cannot lose a list when no lists are currently active for the popup.");
        }

        _curListingCount--;
        EmitSignal(SignalName.ListLost);
    }
}
