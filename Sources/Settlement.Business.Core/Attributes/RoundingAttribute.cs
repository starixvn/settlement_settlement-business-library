using AspectInjector.Broker;
using System.Reflection;

namespace Settlement.Business.Core.Attributes;

[Aspect(Scope.Global)]
[Injection(typeof(RoundingAttribute))]
public class RoundingAttribute : Attribute
{
    private static readonly Dictionary<MethodBase, PropertyInfo> Cache = new();

    [Advice(Kind.After, Targets = Target.Setter)]
    public void Getter(
        [Argument(Source.Method)] MethodBase method,
        [Argument(Source.Instance)] object obj,
        [Argument(Source.Arguments)] object[] args
    )
    {
        var prop = FindProperty(method);
        var type = prop.GetCustomAttribute<TypeAttribute>();
        var field = GetBackingField(prop);
        if (args.Length > 0 && args[0] is decimal decimalValue && type is not null)
        {
            field.SetValue(obj, Math.Round(decimalValue, type.DecimalPlaces, MidpointRounding.AwayFromZero));
        }
    }

    private FieldInfo GetBackingField(PropertyInfo property)
    {
        var backingFieldName = $"<{property.Name}>k__BackingField";
        return property.DeclaringType.GetField(backingFieldName, BindingFlags.NonPublic | BindingFlags.Instance);
    }

    private PropertyInfo FindProperty(MethodBase method)
    {
        if (!Cache.TryGetValue(method, out var prop))
        {
            var conventionalPropName = method.Name.Replace("set_", string.Empty);
            prop = method.DeclaringType.GetProperty(conventionalPropName);

            if (prop == null)
            {
                throw new Exception(
                    $"{conventionalPropName} not valid");

            }

            Cache[method] = prop;
        }
        return prop;
    }
}
