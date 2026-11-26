using Godot;
using Godot.Collections;

[GlobalClass, Tool]
public partial class NodeReference : Resource
{
    /*private static readonly StringName NODE_TYPE_PROPERTY_NAME = "node_type";

    private int _nodeTypeInt = 0;*/

    [Export]
    private string _nodeType = "";
    public string NodeType { get => _nodeType; }

    [Export]
    private StringName _nodeName = "";
    public StringName NodeName { get => _nodeName; }

    /*public override Array<Dictionary> _GetPropertyList()
    {
        string nodeTypeHintString = "";
        string[] classes = ClassDB.GetInheritersFromClass("Node");
        
        nodeTypeHintString += classes[0];
        for (int i = 1; i < classes.Length; i++)
        {
            nodeTypeHintString += $",{classes[i]}";
        }

        Dictionary nodeTypeProperty = MiscUtils.MakeProperty(
            NODE_TYPE_PROPERTY_NAME,
            Variant.Type.Int,
            PropertyHint.Enum,
            nodeTypeHintString
        );

        return [nodeTypeProperty];
    }

    public override Variant _Get(StringName property)
    {
        if (property == NODE_TYPE_PROPERTY_NAME)
        {
            return _nodeTypeInt;
        }

        return default;
    }

    public override bool _Set(StringName property, Variant value)
    {
        if (property == NODE_TYPE_PROPERTY_NAME)
        {
            _nodeTypeInt = value.AsInt32();
            _nodeType = ClassDB.GetInheritersFromClass("Node")[_nodeTypeInt];
            return true;
        }

        return false;
    }

    public override bool _PropertyCanRevert(StringName property)
    {
        return property == NODE_TYPE_PROPERTY_NAME;
    }*/
}
