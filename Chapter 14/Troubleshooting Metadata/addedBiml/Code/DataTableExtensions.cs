using System;
using System.Data;
using System.Linq;
using Varigence.Languages.Biml.Table;

public static class DataTableExtensions
{
  public static AstTableNode ToAstTableNode(this DataTable dataTable)
  {
    var schema = new AstSchemaNode(null);
    var table = new AstTableNode(null);
    schema.Name = dataTable.Rows.OfType<DataRow>().Select(r => r["TABLE_SCHEMA"].ToString()).First();
    table.Schema = schema;
    table.Name = dataTable.Rows.OfType<DataRow>().Select(r => r["TABLE_NAME"].ToString()).First();
    foreach (DataRow row in dataTable.Rows)
    {
      var column = new AstTableColumnNode(table)
      {
        Name = row["COLUMN_NAME"].ToString(),
        DataType = row["DATA_TYPE"].ToString().GetDbType(),
        IsNullable = Convert.ToBoolean(row["IS_NULLABLE"].ToString().Replace("YES", "true").Replace("NO", "false"))
      };
      if (!row.IsNull("CHARACTER_MAXIMUM_LENGTH"))
      {
        column.Length = Convert.ToInt32(row["CHARACTER_MAXIMUM_LENGTH"]);
      }

      if (!row.IsNull("NUMERIC_PRECISION"))
      {
        column.Precision = Convert.ToInt32(row["NUMERIC_PRECISION"]);
      }

      if (!row.IsNull("NUMERIC_SCALE"))
      {
        column.Precision = Convert.ToInt32(row["NUMERIC_SCALE"]);
      }

      table.Columns.Add(column);
    }

    return table;
  }

  public static DbType GetDbType(this string type)
  {
    switch (type.ToLower())
    {
      case "bigint": return DbType.Int64;
      case "binary": return DbType.Binary;
      case "bit": return DbType.Boolean;
      case "char": return DbType.AnsiStringFixedLength;
      case "date": return DbType.Date;
      case "datetime2": return DbType.DateTime2;
      case "datetime": return DbType.DateTime;
      case "datetimeoffset": return DbType.DateTimeOffset;
      case "decimal": return DbType.Decimal;
      case "float": return DbType.Double;
      case "int": return DbType.Int32;
      case "money": return DbType.Currency;
      case "nchar": return DbType.StringFixedLength;
      case "numeric": return DbType.Decimal;
      case "nvarchar": return DbType.String;
      case "real": return DbType.Single;
      case "smalldatetime": return DbType.DateTime;
      case "smallint": return DbType.Int16;
      case "smallmoney": return DbType.Currency;
      case "time": return DbType.Time;
      case "timestamp": return DbType.Binary;
      case "tinyint": return DbType.Byte;
      case "uniqueidentifier": return DbType.Guid;
      case "varbinary": return DbType.Binary;
      case "varchar": return DbType.AnsiString;
      case "xml": return DbType.Xml;
      default: return DbType.Object;
    }
  }
}
