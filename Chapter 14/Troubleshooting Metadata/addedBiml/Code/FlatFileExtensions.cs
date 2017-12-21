using System;
using System.Data;
using System.IO;
using System.Xml;
using Varigence.Biml.Extensions;
using Varigence.Languages.Biml.Cube;
using Varigence.Languages.Biml;
using Varigence.Languages.Biml.FileFormat;
using Varigence.Languages.Biml.Table;

static class FlatFileExtensions
{
  public static AstFlatFileFormatNode GetFlatFileFormatFromXml(this string xmlFile)
  {
    var xmldoc = new XmlDocument();
    xmldoc.Load(xmlFile);
    var records = xmldoc.GetElementsByTagName("RECORD").Item(0).ChildNodes;
    var rows = xmldoc.GetElementsByTagName("ROW").Item(0).ChildNodes;
    var flatFileFormat = new AstFlatFileFormatNode(null)
    {
      Locale = Language.Lcid1033,
      Name = Path.GetFileNameWithoutExtension(xmlFile),
      RowDelimiter = ConvertDelimiter(records[records.Count - 1].Attributes["TERMINATOR"].Value),
      ColumnNamesInFirstDataRow = false,
      IsUnicode = false,
      TextQualifier = "_x0022_"
    };
    foreach (XmlNode record in records)
    {
      if (record.Attributes == null) continue;
      var nuID = Convert.ToInt32(record.Attributes["ID"].Value) - 1;
      var row = rows.Item(nuID);
      var csvDataType = row.Attributes["xsi:type"].Value;
      var dataTypeID = ConvertDataType(csvDataType);
      var column = new AstFlatFileColumnNode(flatFileFormat)
      {
        Name = row.Attributes["NAME"].Value,
        Delimiter = ConvertDelimiter(record.Attributes["TERMINATOR"].Value),
        DataType = dataTypeID ?? DbType.String
      };
      if (dataTypeID == null)
      {
        // By default, we want out strings to be 1000 Characters
        column.Length = 1000;
      }
      else if (dataTypeID == DbType.AnsiString | dataTypeID == DbType.String)
      {
        column.Length = Convert.ToInt32(record.Attributes["MAX_LENGTH"].Value);
      }
      else if (dataTypeID == DbType.VarNumeric)
      {
        column.Precision = 32;
        column.Scale = 16;
      }

      if (dataTypeID == null)
      {
        var columnannotation = new AstAnnotationNode(null) {Tag = "Original Datatype", Text = csvDataType};
        column.Annotations.Add(columnannotation);
      }
      flatFileFormat.Columns.Add(column);
    }
    return flatFileFormat;
  }

  public static DbType? ConvertDataType(string csvType)
  {
    switch (csvType)
    {
      case "SQLINT": return DbType.Int32;
      case "SQLSMALLINT": return DbType.Int16;
      case "SQLVARCHAR": return DbType.AnsiString;
      case "SQLDATETIME": return DbType.DateTime;
      case "SQLMONEY": return DbType.Currency;
      case "SQLNUMERIC": return DbType.Double;
      case "SQLNVARCHAR": return DbType.String;
      case "SQLUNIQUEID": return DbType.String;
      default: return null;
    }
  }

  public static string ConvertDelimiter(string csvDelimiter)
  {
    return csvDelimiter == "\\r\\n" ? "CRLF" : csvDelimiter;
  }
}
