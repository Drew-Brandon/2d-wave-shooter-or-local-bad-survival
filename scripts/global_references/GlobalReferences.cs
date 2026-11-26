using Godot;

[GlobalClass]
public partial class GlobalReferences : Resource
{
    private NodePath _source = null;

    [Signal]
    public delegate void SourceSetEventHandler(GlobalReferencesSource source);

    public GlobalReferencesSource GetSource(SceneTree tree)
    {
        if (_source == null)
        {
            return null;
        }

        return tree.Root.GetNode<GlobalReferencesSource>(_source);
    }

    public void SetSource(GlobalReferencesSource source)
    {
        if (source == null)
        {
            _source = null;
        }
        else
        {
            _source = source.GetPath();
        }
    }
}
