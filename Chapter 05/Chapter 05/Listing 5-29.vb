Imports Varigence.Languages.Biml
Imports Varigence.Languages.Biml.FileFormat
Imports Varigence.Languages.Biml.Table
Imports System.Runtime.CompilerServices

Module FlatFileExtension
    <Extension()>
    Public Function ToAstTableNode(FlatFile As AstFlatFileFormatNode, Schema As AstSchemaNode) As AstTableNode
        Dim BimlTable As New AstTableNode(Nothing)
        BimlTable.Name = "FF_" + FlatFile.name
        BimlTable.Schema = schema
        For Each flatFileColumn As astflatfilecolumnnode In FlatFile.Columns
            Dim tableColumn As New AstTableColumnNode(Nothing)
            tableColumn.Name = flatFileColumn.Name
            tableColumn.DataType = flatFileColumn.DataType
            tableColumn.Length = flatFileColumn.Length
            tableColumn.Precision = flatFileColumn.Precision
            tableColumn.Scale = flatFileColumn.Scale
            tableColumn.CodePage = flatFileColumn.CodePage
            BimlTable.Columns.Add(tableColumn)
        Next
        Return BimlTable
    End Function
End Module
