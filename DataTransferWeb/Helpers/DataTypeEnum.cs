using System.ComponentModel;

public enum DataTypeEnum
{
    [Description("String")]
    String = 1,

    [Description("Integer")]
    Integer = 2,

    [Description("Decimal")]
    Decimal = 3,

    [Description("Date")]
    Date = 4,
    
    [Description("DateTime")]
    DateTime = 5,
}