Imports Varigence.Biml.Extensions
Imports Varigence.Languages.Biml
Imports Varigence.Languages.Biml.FileFormat
Imports Varigence.Languages.Biml.Table
Imports System.IO
Imports System.Xml
Imports System.Data
Imports System.Runtime.CompilerServices

Module FlatFileExtension
  Public Locale As Varigence.Languages.Biml.Cube.Language = Varigence.Languages.Biml.Cube.Language.Lcid1033

  <Extension()>
  Public Function GetFlatFileFormatfromXml(xmlFile As String) As AstFlatFileFormatNode
    Dim flatFileFormat As New AstFlatFileFormatNode(Nothing)
    Dim xmldoc As New XmlDocument
    xmldoc.Load(XmlFile)
    Dim records As XmlNodeList = xmldoc.GetElementsByTagName("RECORD").item(0).childnodes
    Dim rows As XmlNodeList = xmldoc.GetElementsByTagName("ROW").item(0).childnodes
    Dim row As xmlnode
    flatFileFormat.Locale = Locale
    flatFileFormat.Name = path.GetFileNameWithoutExtension(XmlFile)
    flatFileFormat.RowDelimiter = ConvertDelimiter(records.item(records.count - 1).attributes("TERMINATOR").value)
    flatFileFormat.ColumnNamesInFirstDataRow = False
    flatFileFormat.isunicode = False
    flatFileFormat.TextQualifier = "_x0022_"
    For Each record As xmlnode In records
      row = rows.item(record.attributes("ID").value - 1)
      Dim dataType As String = row.attributes("xsi:type").value
      Dim datatypeId As Integer = ConvertDatatype(dataType)
      Dim column As New AstFlatFileColumnNode(flatFileFormat)
      column.name = row.attributes("NAME").value
      column.Delimiter = ConvertDelimiter(record.attributes("TERMINATOR").value)
      If datatypeId = Nothing Then
        ' By default, we will make this a string!
        column.DataType = DbType.String
      Else
        column.DataType = datatypeId
      End If
      If datatypeId = Nothing Then
        ' By default, we want out strings to be 1000 Characters
        column.Length = 1000
      ElseIf datatypeId = dbtype.AnsiString Or datatypeId = DbType.String Then
        column.Length = record.attributes("MAX_LENGTH").value
      End If
      If ConvertDatatype(dataType) = dbtype.VarNumeric Then
        column.Precision = 32
        column.Scale = 16
      End If
      If datatypeId = Nothing Then
        Dim columnannotation As New AstAnnotationNode(Nothing)
        columnannotation.Tag = "Original Datatype"
        columnannotation.Text = dataType
        column.Annotations.Add(columnannotation)
      End If
      flatFileFormat.Columns.Add(column)
    Next
    Return flatFileFormat
  End Function

  Public Function ConvertDatatype(csvType As String) As String
    Select Case csvType
      Case "SQLINT"
        Return dbtype.Int32
      Case "SQLSMALLINT"
        Return dbtype.int16
      Case "SQLVARCHAR"
        Return dbtype.AnsiString
      Case "SQLDATETIME"
        Return dbtype.DateTime
      Case "SQLMONEY"
        Return dbtype.Currency
      Case "SQLNUMERIC"
        Return dbtype.Double
      Case "SQLNVARCHAR"
        Return DbType.String
      Case "SQLUNIQUEID"
        ' GUIDs should be interpreted as strings
        Return DbType.String
      Case Else
        Return Nothing
    End Select
  End Function

  Public Function ConvertDelimiter(csvDelimiter As String) As String
    Select Case csvDelimiter
      Case "\r\n"
        Return "CRLF"
      Case Else
        Return csvDelimiter
    End Select
  End Function
End Module
