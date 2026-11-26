namespace Logic;
using Godot;

public partial class ShaderSetter : Node
{
    private ShaderMaterial _shaderMat = null;

    [Export]
    private CanvasItem _node = null;

    public override void _Ready()
    {
        _shaderMat = (ShaderMaterial)_node.Material;
    }

    public void SetShaderParam(StringName param, Variant val)
    {
        _shaderMat.SetShaderParameter(param, val);
    }

    public void SetShaderParamAlt(Variant val, StringName param)
    {
        _shaderMat.SetShaderParameter(param, val);
    }
}
