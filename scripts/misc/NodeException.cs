using Godot;
using System;

/// <summary>
/// An exception thrown from a node.
/// Often times the node throwing an exception can be vague.
/// This class aims to fix that by having users provide the node's path along with the exception itself.
/// So, when displayed, this class will output both the inner exception and the node's path.
/// </summary>
public partial class NodeException : Exception
{
    private NodePath _nodePath = null;

    /// <summary>
    /// The path of the node that threw the exception
    /// </summary>
    public NodePath @NodePath { get => _nodePath; }

    /// <summary>
    /// Initializes a new instance of the NodeException class with a specified NodePath
    /// of the node throwing this exception and an inner exception that caused this one.
    /// </summary>
    /// <param name="nodePath">
    /// The path to the node that is throwing the exception.
    /// </param>
    /// <param name="innerException">
    /// The inner exception that caused this one to occur.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the nodePath argument is null.
    /// </exception>
    public NodeException(NodePath nodePath, Exception innerException) : base(innerException.Message, innerException)
    {
        if (nodePath == null)
        {
            throw new ArgumentNullException("node");
        }

        _nodePath = nodePath;
    }

    public override string ToString()
    {
        return $"{_nodePath}: {InnerException}";
    }
}
