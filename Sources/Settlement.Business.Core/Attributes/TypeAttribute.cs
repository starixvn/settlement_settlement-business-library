namespace Settlement.Business.Core.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class TypeAttribute : Attribute
{
    public readonly byte DecimalPlaces;

    public TypeAttribute(byte decimalPlaces)
    {
        DecimalPlaces = decimalPlaces;
    }
}
