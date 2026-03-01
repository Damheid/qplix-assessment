
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace core;

class QPlixTypeConverter<TEnum> : EnumConverter where TEnum : struct, Enum
{
    public QPlixTypeConverter() : base(typeof(TEnum))
    {
    }

    public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        if (Enum.TryParse<TEnum>(text, out var result))
            return result;

        var TEnums = Enum.GetNames<TEnum>();

        throw new InvalidCastException($"Can only convert strings '{string.Join(", ", TEnums)}'. However, the value received was: {text}.");
    }

    public override string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
    {
        if (value is null)
            return "";

        if (Enum.TryParse<TEnum>(value as string, out var result))
            return Enum.GetName(result);

        var TEnums = Enum.GetNames<TEnum>();

        throw new InvalidCastException($"Can only convert types '{string.Join(", ", TEnums)}'. However, the value received was: {value}.");
    }
}
