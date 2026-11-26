using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public static class MiscUtils
{
    public static string FormatStringCollection(IEnumerable<string> strings, string stringStart = "", string delimiter = "", string stringEnd = "")
    {
        if (strings.Count() == 0)
        {
            return "";
        }

        string formattedString = $"{stringStart}{strings.ElementAt(0)}{stringEnd}";

        for (int i = 1; i < strings.Count(); i++)
        {
            formattedString += $"{delimiter}{stringStart}{strings.ElementAt(i)}{stringEnd}";
        }

        return formattedString;
    }

    public static char UserToSpecialChar(char @char)
    {
        switch (@char)
        {
            case 'n':
                return '\n';

            case 't':
                return '\t';
            
            default:
                return '\0';
        }
    }

    public static char SpecialToUserChar(char @char)
    {
        switch (@char)
        {
            case '\n':
                return 'n';
            
            case '\t':
                return 't';
            
            default:
                return '\0';
        }
    }

    public static string CompileUserString(string userString)
    {
        string compiled = "";

        for (int i = 0; i < userString.Length; i++)
        {
            if (userString[i] == '\\' && i != userString.Length - 1 && userString[i + 1] != '\\')
            {
                char specialChar = UserToSpecialChar(userString[i]);

                if (specialChar != '\0')
                {
                    compiled += specialChar;
                }

                i++;
                continue;
            }
            
            compiled += userString[i];
        }

        return compiled;
    }

    public static string DecompileDevString(string devString)
    {
        string decompiled = "";

        for (int i = 0; i < devString.Length; i++)
        {
            char converted = SpecialToUserChar(devString[i]);

            if (converted == '\0')
            {
                decompiled += devString[i];
            }
            else
            {
                decompiled += $"\\{converted}";
            }
        }
        
        return decompiled;
    }

    public static Dictionary MakeProperty(StringName name, Variant.Type type, PropertyHint hint = PropertyHint.None, string hintString = "")
    {
        return new()
        {
            {"name", name},
			{"type", (int)type},
			{"hint", (int)hint},
			{"hint_string", hintString}
        };
    }

    public static void CopyPropertiesTo(GodotObject from, GodotObject to)
    {
        Array<Dictionary> properties = from.GetPropertyList();

        for (int i = 0; i < properties.Count; i++)
        {
            StringName property = properties[i]["name"].AsStringName();
            to.Set(property, from.Get(property));
        }
    }

    public static GodotObject CloneObject(GodotObject @object)
    {
        GodotObject clone = (GodotObject)Activator.CreateInstance(@object.GetType());
        CopyPropertiesTo(@object, clone);
        return clone;
    }

    public static Node GetSceneNodeOf(Node node)
    {
        if (node == null)
        {
            throw new NullReferenceException("Cannot get scene node of a node that is null.");
        }

        Node root = node.GetTree().Root;
        Node currentNode = node;
        
        while (currentNode.GetParent() != root)
        {
            currentNode = currentNode.Owner == null ? currentNode.GetParent() : currentNode.Owner;

            if (currentNode == null)
            {
                throw new NullReferenceException("Cannot get scene node of a node that is null.");
            }
        }

        return currentNode;
    }
}
