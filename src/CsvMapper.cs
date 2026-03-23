using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace Philiprehberger.CsvKit;

/// <summary>
/// Maps CSV header/value pairs to strongly-typed objects using reflection and type conversion.
/// </summary>
public static class CsvMapper
{
    /// <summary>
    /// Maps a single row of values to a new instance of <typeparamref name="T"/> using the provided headers.
    /// Property matching is case-insensitive.
    /// </summary>
    /// <typeparam name="T">The target type. Must have a parameterless constructor.</typeparam>
    /// <param name="headers">The column headers.</param>
    /// <param name="values">The field values for the row.</param>
    /// <param name="culture">The culture to use for type conversion.</param>
    /// <returns>A new instance of <typeparamref name="T"/> with properties set from the CSV values.</returns>
    public static T Map<T>(string[] headers, string[] values, CultureInfo? culture = null) where T : new()
    {
        var obj = new T();
        var type = typeof(T);
        var cultureInfo = culture ?? CultureInfo.InvariantCulture;
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        var propMap = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);
        foreach (var prop in properties)
        {
            if (prop.CanWrite)
            {
                propMap[prop.Name] = prop;
            }
        }

        int count = Math.Min(headers.Length, values.Length);

        for (int i = 0; i < count; i++)
        {
            var header = headers[i].Trim();

            if (!propMap.TryGetValue(header, out var prop))
                continue;

            var value = values[i];

            try
            {
                var converted = ConvertValue(value, prop.PropertyType, cultureInfo);
                prop.SetValue(obj, converted);
            }
            catch (Exception ex) when (ex is not OutOfMemoryException)
            {
                throw new InvalidOperationException(
                    $"Failed to convert value '{value}' to type '{prop.PropertyType.Name}' for property '{prop.Name}'.",
                    ex);
            }
        }

        return obj;
    }

    /// <summary>
    /// Gets the property names of a type as header values for CSV writing.
    /// </summary>
    /// <typeparam name="T">The type to extract headers from.</typeparam>
    /// <returns>An array of property names.</returns>
    public static string[] GetHeaders<T>()
    {
        return typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead)
            .Select(p => p.Name)
            .ToArray();
    }

    /// <summary>
    /// Gets the property values of an object as string values for CSV writing.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="obj">The object to extract values from.</param>
    /// <param name="culture">The culture to use for string conversion.</param>
    /// <returns>An array of string values.</returns>
    public static string[] GetValues<T>(T obj, CultureInfo? culture = null)
    {
        var cultureInfo = culture ?? CultureInfo.InvariantCulture;
        return typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead)
            .Select(p =>
            {
                var val = p.GetValue(obj);
                if (val is null)
                    return string.Empty;
                if (val is IFormattable formattable)
                    return formattable.ToString(null, cultureInfo);
                return val.ToString() ?? string.Empty;
            })
            .ToArray();
    }

    private static object? ConvertValue(string value, Type targetType, CultureInfo culture)
    {
        var underlying = Nullable.GetUnderlyingType(targetType);
        if (underlying is not null)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            targetType = underlying;
        }

        if (targetType == typeof(string))
            return value;

        if (targetType == typeof(int))
            return int.Parse(value, culture);

        if (targetType == typeof(long))
            return long.Parse(value, culture);

        if (targetType == typeof(double))
            return double.Parse(value, culture);

        if (targetType == typeof(decimal))
            return decimal.Parse(value, culture);

        if (targetType == typeof(float))
            return float.Parse(value, culture);

        if (targetType == typeof(bool))
            return bool.Parse(value);

        if (targetType == typeof(DateTime))
            return DateTime.Parse(value, culture);

        if (targetType == typeof(DateTimeOffset))
            return DateTimeOffset.Parse(value, culture);

        if (targetType == typeof(DateOnly))
            return DateOnly.Parse(value, culture);

        if (targetType == typeof(Guid))
            return Guid.Parse(value);

        if (targetType.IsEnum)
            return Enum.Parse(targetType, value, ignoreCase: true);

        var converter = TypeDescriptor.GetConverter(targetType);
        if (converter.CanConvertFrom(typeof(string)))
            return converter.ConvertFromString(null, culture, value);

        throw new NotSupportedException($"Cannot convert string to type '{targetType.Name}'.");
    }
}
