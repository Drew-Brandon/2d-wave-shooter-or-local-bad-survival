using System;
using Godot;
using GC = Godot.Collections;
using SCG = System.Collections.Generic;

[Tool, GlobalClass]
public partial class NodeRefRelay : Resource
{
    private static readonly StringName REF_NAMES_CHANGED_PROPERTY_NAME = "ref_names_changed";

    private bool _refNamesChanged = false;

    private StringName[] _refNamesArray = null;

    [Export]
    private StringName[] _refNamesArrayProperty
    {
        get => _refNamesArray;

        set
        {
            _refNamesArray = value;
            _refNamesChanged = true;
        }
    }

    private SCG.HashSet<StringName> _refNamesSet = null;

    [ExportToolButton("Compile Reference Names")]
    private Callable _compileReferenceNames => Callable.From(CompileReferenceNames);

    [Signal]
    public delegate void RefUpdatedEventHandler(StringName refName, Node node);

    private void CompileReferenceNames()
    {
        if (_refNamesArray == null)
        {
            _refNamesSet = null;
        }
        else
        {
            _refNamesSet = [.. _refNamesArray];
        }

        _refNamesChanged = false;
    }

    public void UpdateRef(StringName refName, Node node)
    {
        if (_refNamesSet.Contains(refName))
        {
            EmitSignal(SignalName.RefUpdated, refName, node);
        }
        else
        {
            throw new ArgumentException($"Reference of the name \"{refName}\" is not recognized.");
        }
    }

    public string GetRefNames()
    {
        bool notFirst = true;
        string refNames = "";

        if (_refNamesSet != null)
        {
            foreach (StringName stringName in _refNamesSet)
            {
                if (notFirst)
                {
                    refNames += ",";
                }

                refNames += stringName;
                notFirst = true;
            }
        }

        return refNames;
    }

    public override GC.Array<GC.Dictionary> _GetPropertyList()
    {
        GC.Dictionary refNamesDesyncedProperty = MiscUtils.MakeProperty(
            REF_NAMES_CHANGED_PROPERTY_NAME,
            Variant.Type.Bool
        );

        return [refNamesDesyncedProperty];
    }

    public override Variant _Get(StringName property)
    {
        if (property == REF_NAMES_CHANGED_PROPERTY_NAME)
        {
            return _refNamesChanged;
        }
        
        return default;
    }
}
