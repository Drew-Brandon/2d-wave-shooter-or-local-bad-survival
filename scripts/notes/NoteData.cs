using Godot;
using System;

[GlobalClass]
public partial class NoteData : Resource
{
    [Export]
    private string _title = "";
    public string Title { get => _title; }

    [Export(PropertyHint.MultilineText)]
    private string _text = "";
    public string Text { get => _text; }

    [Export]
    private PackedScene _scene = null;
    public PackedScene Scene { get => _scene; }
}
