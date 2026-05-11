namespace Common.Core.Query;

public static class RawSqlFilterExtensions
{
    enum ComparisonType
    {
        eq,
        gt,
        lt,
        like,
        neq,
        gteq,
        lteq,
        between
    }

    public static (string, dynamic) WhereClauseBuilder(string fieldName, string comparision, string value, string valueType)
    {
        _ = Enum.TryParse(comparision, out ComparisonType comparisonType);

        var convertedFieldName = "";
        var convertedComparision = "";
        dynamic convertedValue = "";

        if (comparisonType == ComparisonType.eq)
        {
            convertedFieldName = fieldName;
            convertedComparision = "=";
            if (valueType is "long")
            {
                convertedValue = Convert.ToInt64(value);
            }
            else if (valueType is "decimal")
            {
                convertedValue = Convert.ToDecimal(value);
            }
            else if (valueType is "integer")
            {
                convertedValue = Convert.ToInt32(value);
            }
            else if (valueType is "boolean")
            {
                convertedValue = Convert.ToBoolean(value);
            }
            else if (valueType is "datetime")
            {
                convertedValue = Convert.ToDateTime(value).ToString();
            }
            else if (valueType is "string")
            {
                convertedValue = value;
            }
        }
        else if (comparisonType == ComparisonType.gt)
        {
            convertedFieldName = fieldName;
            convertedComparision = ">";
            if (valueType is "long")
            {
                convertedValue = Convert.ToInt64(value);
            }
            else if (valueType is "decimal")
            {
                convertedValue = Convert.ToDecimal(value);
            }
            else if (valueType is "integer")
            {
                convertedValue = Convert.ToInt32(value);
            }
            else if (valueType is "datetime")
            {
                convertedValue = Convert.ToDateTime(value).ToString();
            }
        }
        else if (comparisonType == ComparisonType.lt)
        {
            convertedFieldName = fieldName;
            convertedComparision = "<";
            if (valueType is "long")
            {
                convertedValue = Convert.ToInt64(value);
            }
            else if (valueType is "decimal")
            {
                convertedValue = Convert.ToDecimal(value);
            }
            else if (valueType is "integer")
            {
                convertedValue = Convert.ToInt32(value);
            }
            else if (valueType is "datetime")
            {
                convertedValue = Convert.ToDateTime(value).ToString();
            }
        }
        else if (comparisonType == ComparisonType.like && valueType is "string")
        {
            convertedFieldName = $"lower({fieldName})";
            convertedComparision = "like";
            convertedValue = $"%{value.Trim().ToLower()}%";
        }
        else if (comparisonType == ComparisonType.neq)
        {
            convertedFieldName = fieldName;
            convertedComparision = "<>";
            if (valueType is "long")
            {
                convertedValue = Convert.ToInt64(value);
            }
            else if (valueType is "decimal")
            {
                convertedValue = Convert.ToDecimal(value);
            }
            else if (valueType is "integer")
            {
                convertedValue = Convert.ToInt32(value);
            }
            else if (valueType is "datetime")
            {
                convertedValue = Convert.ToDateTime(value).ToString();
            }
            else if (valueType is "string")
            {
                convertedValue = value;
            }
        }
        else if (comparisonType == ComparisonType.gteq)
        {
            convertedFieldName = fieldName;
            convertedComparision = ">=";
            if (valueType is "long")
            {
                convertedValue = Convert.ToInt64(value);
            }
            else if (valueType is "decimal")
            {
                convertedValue = Convert.ToDecimal(value);
            }
            else if (valueType is "integer")
            {
                convertedValue = Convert.ToInt32(value);
            }
            else if (valueType is "datetime")
            {
                convertedValue = Convert.ToDateTime(value).ToString();
            }
        }
        else if (comparisonType == ComparisonType.lteq)
        {
            convertedFieldName = fieldName;
            convertedComparision = "<=";
            if (valueType is "long")
            {
                convertedValue = Convert.ToInt64(value);
            }
            else if (valueType is "decimal")
            {
                convertedValue = Convert.ToDecimal(value);
            }
            else if (valueType is "integer")
            {
                convertedValue = Convert.ToInt32(value);
            }
            else if (valueType is "datetime")
            {
                convertedValue = Convert.ToDateTime(value).ToString();
            }
        }
        else if (comparisonType == ComparisonType.between)
        {
            convertedFieldName = fieldName;
            convertedComparision = "between";

            var valueRange = value.Split(',');
            var valueOne = valueRange[0];
            var valueTwo = valueRange[1];

            if (valueType is "long")
            {
                convertedValue = $"{Convert.ToInt64(valueOne)} and {Convert.ToInt64(valueTwo)}";
            }
            else if (valueType is "decimal")
            {
                convertedValue = $"{Convert.ToDecimal(valueOne)} and {Convert.ToDecimal(valueTwo)}";
            }
            else if (valueType is "integer")
            {
                convertedValue = $"{Convert.ToInt32(valueOne)} and {Convert.ToInt32(valueTwo)}";
            }
            else if (valueType is "datetime")
            {
                convertedValue = $"'{Convert.ToDateTime(valueOne)}' and '{Convert.ToDateTime(valueTwo)}'";
            }
        }

        return ($"{convertedFieldName} {convertedComparision} @{fieldName}", convertedValue);
    }
}
