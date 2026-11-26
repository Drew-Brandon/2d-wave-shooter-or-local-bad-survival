using Godot;
using System;
using System.Reflection;
using SLE = System.Linq.Expressions;

public static class TypeUtils
{
    public static bool IsOfType(object @object, Type type)
    {
        return @object.GetType().IsAssignableTo(type);
    }

    #nullable enable
    public static bool CanBeType(object? @object, Type type)
    {
        return @object == null || @object.GetType().IsAssignableTo(type);
    }
    #nullable disable

    public static Variant GetDefault(Type type)
    {
        if (type.IsValueType)
        {
            return (Variant)Activator.CreateInstance(type);
        }

        return default;
    }

    public static Variant.Type GetVariantType(Type type)
    {
        if (type == typeof(GodotObject) || type.IsSubclassOf(typeof(GodotObject)))
        {
            return Variant.Type.Object;
        }

        return Enum.Parse<Variant.Type>(type.ToString());
    }

    public static string GetTypeName(Type type)
    {
        string typeStr = type.ToString();
        int lastDot = typeStr.LastIndexOf('.');
        
        if (lastDot == -1)
        {
            return typeStr;
        }
        else
        {
            return typeStr.Substring(lastDot + 1);
        }
    }

    public static object CreateInstanceQuick(Type type)
    {
        // Code inspired by: https://vagifabilov.wordpress.com/2010/04/02/dont-use-activator-createinstance-or-constructorinfo-invoke-use-compiled-lambda-expressions/
        SLE.NewExpression newExpression = SLE.Expression.New(type);
        SLE.LambdaExpression lambda = SLE.Expression.Lambda(type, newExpression);
        return lambda.Compile();
    }

    public static object CreateInstanceQuick(Type type, ConstructorInfo constructor, params SLE.Expression[] arguments)
    {
        // Code inspired by: https://vagifabilov.wordpress.com/2010/04/02/dont-use-activator-createinstance-or-constructorinfo-invoke-use-compiled-lambda-expressions/
        SLE.NewExpression newExpression = SLE.Expression.New(constructor, arguments);
        SLE.LambdaExpression lambda = SLE.Expression.Lambda(type, newExpression);
        return lambda.Compile();
    }

    public static object CreateInstanceQuick(Type type, params object[] arguments)
    {
        Type[] argTypes = new Type[arguments.Length];
        SLE.Expression[] argExpressions = new SLE.Expression[arguments.Length];

        for (int i = 0; i < arguments.Length; i++)
        {
            argTypes[i] = arguments.GetType();
            argExpressions[i] = SLE.Expression.Constant(arguments[i]);
        }

        return CreateInstanceQuick(type, type.GetConstructor(argTypes), argExpressions);
    }
}
