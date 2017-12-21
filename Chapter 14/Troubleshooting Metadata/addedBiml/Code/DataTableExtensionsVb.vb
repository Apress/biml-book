Imports Varigence.Languages.Biml.Table
Imports System
Imports System.Data
Imports System.Linq
Imports System.Runtime.CompilerServices

Module DataTableExtensionsVb
  <Extension()>
  Public Function ToAstTableNode(dataTable As DataTable) As AstTableNode
    Dim table As New AstTableNode(Nothing)
    Dim schema As New AstSchemaNode(Nothing) With {
          .Name = dataTable.Rows.OfType (Of DataRow)().Select(Function(row) row("TABLE_SCHEMA").ToString).First()
          }
    table.Schema = schema
    table.Name = dataTable.Rows.OfType (Of DataRow)().Select(Function(row) row("TABLE_NAME").ToString).First()
    For Each row In dataTable.Rows
      Dim column As New AstTableColumnNode(table)
      column.Name = row("COLUMN_NAME")
      column.IsNullable = Convert.ToBoolean(row("IS_NULLABLE").ToString().Replace("YES", "true").Replace("NO", "false"))
      If Not row.IsNull("CHARACTER_MAXIMUM_LENGTH") Then _
        column.Length = Convert.ToInt32(row("CHARACTER_MAXIMUM_LENGTH"))
      column.DataType = row("DATA_TYPE").ToString().GetDbType()
      If Not row.IsNull("NUMERIC_PRECISION") Then column.Precision = Convert.ToInt32(row("NUMERIC_PRECISION"))
      If Not row.IsNull("NUMERIC_SCALE") Then column.Precision = Convert.ToInt32(row("NUMERIC_SCALE"))
      table.Columns.Add(column)
    Next
    Return table
  End Function

  <Extension()>
  Public Function GetDbType(type As String) As DbType
    Select Case type
      Case "bigint" : Return DbType.Int64
      Case "binary" : Return DbType.Binary
      Case "bit" : Return DbType.Boolean
      Case "char" : Return DbType.AnsiStringFixedLength
      Case "date" : Return DbType.Date
      Case "datetime2" : Return DbType.DateTime2
      Case "datetime" : Return DbType.DateTime
      Case "datetimeoffset" : Return DbType.DateTimeOffset
      Case "decimal" : Return DbType.Decimal
      Case "float" : Return DbType.Double
      Case "int" : Return DbType.Int32
      Case "money" : Return DbType.Currency
      Case "nchar" : Return DbType.StringFixedLength
      Case "numeric" : Return DbType.Decimal
      Case "nvarchar" : Return DbType.String
      Case "real" : Return DbType.Single
      Case "smalldatetime" : Return DbType.DateTime
      Case "smallint" : Return DbType.Int16
      Case "smallmoney" : Return DbType.Currency
      Case "time" : Return DbType.Time
      Case "timestamp" : Return DbType.Binary
      Case "tinyint" : Return DbType.Byte
      Case "uniqueidentifier" : Return DbType.Guid
      Case "varbinary" : Return DbType.Binary
      Case "varchar" : Return DbType.AnsiString
      Case "xml" : Return DbType.Xml
      Case Else : Return DbType.Object
    End Select
  End Function
End Module

