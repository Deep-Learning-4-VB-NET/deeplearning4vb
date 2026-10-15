Imports System
Imports System.IO
Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.datavec.api.transform.metadata
Imports ColumnType = DeepLearning4VB.org.datavec.api.transform.ColumnType
Imports DeepLearning4VB.org.datavec.api.transform.schema
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema

Namespace org.datavec.api.transform.schema

    ''' <summary><see cref="InferredSchema"/>: schema inference from the header and the first data line of a CSV file.</summary>
    <TestClass>
    Public Class InferredSchemaTest

        Private tmpDir As String

        <TestInitialize>
        Public Sub CreateTempDir()
            tmpDir = Path.Combine(Path.GetTempPath(), "dl4vb-tests", Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(tmpDir)
        End Sub

        <TestCleanup>
        Public Sub DeleteTempDir()
            If Directory.Exists(tmpDir) Then Directory.Delete(tmpDir, True)
        End Sub

        Private Function writeCsv(ByVal name As String, ByVal ParamArray lines() As String) As String
            Dim target As String = Path.Combine(tmpDir, name)
            File.WriteAllText(target, String.Join(vbLf, lines) & vbLf, New UTF8Encoding(False))
            Return target
        End Function

        <TestMethod>
        Public Sub testInferDefaultTypes()
            'anything parseable as a double is a double (integers included, as in Java), the rest is the default type
            Dim csv As String = writeCsv("a.csv", "price,count,id,name", "1.5,2,3000000000,hello")
            Dim schema As SchemaT = New InferredSchema(csv).build()
            assertEquals(4, schema.numColumns())
            assertEquals(Arrays.asList("price", "count", "id", "name"), schema.getColumnNames())
            assertEquals(Arrays.asList(ColumnType.[Double], ColumnType.[Double], ColumnType.[Double], ColumnType.[String]), schema.getColumnTypes())
            'doubles are added without NaN/infinite values allowed
            assertEquals(New DoubleMetaData("price", Nothing, Nothing, False, False), schema.getMetaData("price"))
            assertEquals(New StringMetaData("name"), schema.getMetaData("name"))
            'only the first data line matters
            Dim csv2 As String = writeCsv("b.csv", "x,y", "abc,1", "2,3")
            Dim schema2 As SchemaT = New InferredSchema(csv2).build()
            assertEquals(Arrays.asList(ColumnType.[String], ColumnType.[Double]), schema2.getColumnTypes())
        End Sub

        <TestMethod>
        Public Sub testDefaultTypeAndDelimiter()
            Dim csv As String = writeCsv("semi.csv", "a;b;c", "x;7;1,5")
            Dim schema As SchemaT = New InferredSchema(csv, InferredSchema.DataType.INTEGER, ";"c).build()
            assertEquals(Arrays.asList("a", "b", "c"), schema.getColumnNames())
            'a non-numeric sample gets the default type; "1,5" is not a number either
            assertEquals(Arrays.asList(ColumnType.[Integer], ColumnType.[Double], ColumnType.[Integer]), schema.getColumnTypes())
            assertEquals(New IntegerMetaData("a"), schema.getMetaData("a"))
            Dim asLong As SchemaT = New InferredSchema(csv, InferredSchema.DataType.LONG, ";"c).build()
            assertEquals(ColumnType.[Long], asLong.getType("a"))
            Dim asDouble As SchemaT = New InferredSchema(csv, InferredSchema.DataType.DOUBLE, ";"c).build()
            assertEquals(ColumnType.[Double], asDouble.getType("a"))
            'with the default comma delimiter the header is one column but the sample "x;7;1,5" is two, as in Java
            assertThrows(Of InvalidOperationException)(Sub() Call New InferredSchema(csv, InferredSchema.DataType.STRING).build())
            'a semicolon file without commas is read as one column per line
            Dim plain As String = writeCsv("plain.csv", "a;b;c", "x;7;1")
            Dim oneColumn As SchemaT = New InferredSchema(plain, InferredSchema.DataType.STRING).build()
            assertEquals(1, oneColumn.numColumns())
            assertEquals("a;b;c", oneColumn.getName(0))
            assertEquals(ColumnType.[String], oneColumn.getType(0))
        End Sub

        <TestMethod>
        Public Sub testQuotedAndEscapedFields()
            'a quoted field may contain the delimiter, a doubled quote is a literal quote, the escape character
            'makes the next quote literal
            Dim csv As String = writeCsv("q.csv", """first, name"",""say """"hi"""""",esc\""aped,n", """a, b"",2,3,4")
            Dim schema As SchemaT = New InferredSchema(csv, InferredSchema.DataType.STRING, ","c, """"c, "\"c).build()
            assertEquals(Arrays.asList("first, name", "say ""hi""", "esc""aped", "n"), schema.getColumnNames())
            assertEquals(Arrays.asList(ColumnType.[String], ColumnType.[Double], ColumnType.[Double], ColumnType.[Double]), schema.getColumnTypes())
            'a different quote character
            Dim csv2 As String = writeCsv("q2.csv", "'x,y'|z", "'1,2'|3")
            Dim schema2 As SchemaT = New InferredSchema(csv2, InferredSchema.DataType.STRING, "|"c, "'"c).build()
            assertEquals(Arrays.asList("x,y", "z"), schema2.getColumnNames())
            assertEquals(Arrays.asList(ColumnType.[String], ColumnType.[Double]), schema2.getColumnTypes())
            'an unterminated quote is an error
            Dim bad As String = writeCsv("bad.csv", """open,b", "1,2")
            assertThrows(Of IOException)(Sub() Call New InferredSchema(bad).build())
        End Sub

        <TestMethod>
        Public Sub testHeaderMismatchAndMissingFile()
            Dim csv As String = writeCsv("mismatch.csv", "a,b,c", "1,2")
            Dim ex As InvalidOperationException = assertThrows(Of InvalidOperationException)(Sub() Call New InferredSchema(csv).build())
            assertTrue(ex.Message.Contains("CSV headers length does not match"), ex.Message)
            'a missing file is logged and then fails on the missing header line
            assertThrows(Of Exception)(Sub() Call New InferredSchema(Path.Combine(tmpDir, "missing.csv")).build())
            'a file without a data line
            Dim headerOnly As String = writeCsv("header.csv", "a,b")
            assertThrows(Of ArgumentOutOfRangeException)(Sub() Call New InferredSchema(headerOnly).build())
        End Sub

    End Class

End Namespace
