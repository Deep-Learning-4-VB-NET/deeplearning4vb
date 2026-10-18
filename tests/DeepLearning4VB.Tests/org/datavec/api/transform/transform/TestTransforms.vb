Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.JavaCompat
Imports DeepLearning4VB.org.datavec.api.transform.transforms.categorical
Imports DeepLearning4VB.org.datavec.api.transform.transforms.column
Imports DeepLearning4VB.org.datavec.api.transform.transforms.doubletransform
Imports DeepLearning4VB.org.datavec.api.transform.transforms.integer
Imports DeepLearning4VB.org.datavec.api.transform.transforms.string
Imports DeepLearning4VB.org.datavec.api.writable
Imports CategoricalMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.CategoricalMetaData
Imports ColumnType = DeepLearning4VB.org.datavec.api.transform.ColumnType
Imports ConditionalCopyValueTransform = DeepLearning4VB.org.datavec.api.transform.transforms.condition.ConditionalCopyValueTransform
Imports ConditionalReplaceValueTransform = DeepLearning4VB.org.datavec.api.transform.transforms.condition.ConditionalReplaceValueTransform
Imports ConditionalReplaceValueTransformWithDefault = DeepLearning4VB.org.datavec.api.transform.transforms.condition.ConditionalReplaceValueTransformWithDefault
Imports ConditionOp = DeepLearning4VB.org.datavec.api.transform.condition.ConditionOp
Imports ConditionT = DeepLearning4VB.org.datavec.api.transform.condition.Condition
Imports DateTimeFieldType = DeepLearning4VB.org.joda.time.DateTimeFieldType
Imports DateTimeZone = DeepLearning4VB.org.joda.time.DateTimeZone
Imports DeriveColumnsFromTimeTransform = DeepLearning4VB.org.datavec.api.transform.transforms.time.DeriveColumnsFromTimeTransform
Imports DoubleMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.DoubleMetaData
Imports IntegerColumnCondition = DeepLearning4VB.org.datavec.api.transform.condition.column.IntegerColumnCondition
Imports IntegerMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.IntegerMetaData
Imports LongColumnsMathOpTransform = DeepLearning4VB.org.datavec.api.transform.transforms.longtransform.LongColumnsMathOpTransform
Imports LongMathOpTransform = DeepLearning4VB.org.datavec.api.transform.transforms.longtransform.LongMathOpTransform
Imports LongMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.LongMetaData
Imports MathFunction = DeepLearning4VB.org.datavec.api.transform.MathFunction
Imports MathOp = DeepLearning4VB.org.datavec.api.transform.MathOp
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports SequenceDifferenceTransform = DeepLearning4VB.org.datavec.api.transform.transforms.sequence.SequenceDifferenceTransform
Imports SequenceOffsetTransform = DeepLearning4VB.org.datavec.api.transform.transforms.sequence.SequenceOffsetTransform
Imports SequenceSchema = DeepLearning4VB.org.datavec.api.transform.schema.SequenceSchema
Imports StringColumnCondition = DeepLearning4VB.org.datavec.api.transform.condition.column.StringColumnCondition
Imports StringToTimeTransform = DeepLearning4VB.org.datavec.api.transform.transforms.time.StringToTimeTransform
Imports TimeMathOpTransform = DeepLearning4VB.org.datavec.api.transform.transforms.time.TimeMathOpTransform
Imports TransformT = DeepLearning4VB.org.datavec.api.transform.Transform
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms

    ''' <summary>
    ''' Port of the DataVec <c>TestTransforms</c> JUnit class (the transforms live in the namespace
    ''' <c>org.datavec.api.transform.transforms</c>, see <c>BaseTransform</c>).
    ''' <para>Not ported here: testReduceSequenceTransform and testTrimSequenceTransform (the reduce/sequence packages),
    ''' testSequenceTrimToLengthTransform, testSequenceTrimToLengthTransformTrimOrPad and testFirstDigitTransform
    ''' (TransformProcess; FirstDigitTransform is covered in TransformImplementationsTest), the two
    ''' StringListTo*NDArrayTransform tests (INDArray). The JSON round trips at the end of
    ''' testConcatenateStringColumnsTransform and testChangeCaseStringTransform wait for the serde wave.</para>
    ''' </summary>
    <TestClass>
    Public Class TestTransforms

        ''' <summary>Java <c>TestTransforms.getSchema</c>.</summary>
        Public Shared Function getSchema(ByVal type As ColumnType, ByVal ParamArray colNames() As String) As SchemaT
            Return TransformTestSupport.getSchema(type, colNames)
        End Function

        ''' <summary>Java <c>Collections.singletonList((Writable) w)</c>.</summary>
        Private Shared Function singleRow(ByVal w As WritableT) As IList(Of WritableT)
            Return JavaCollections.singletonList(Of WritableT)(w)
        End Function

        ''' <summary>Java <c>Arrays.&lt;Writable&gt;asList(...)</c>.</summary>
        Private Shared Function row(ByVal ParamArray ws() As WritableT) As IList(Of WritableT)
            Return Arrays.asList(Of WritableT)(ws)
        End Function

        ''' <summary>Java serialization round trip (ObjectOutputStream/ObjectInputStream): SerializationUtils.</summary>
        Private Shared Function serializeRoundTrip(Of T)(ByVal obj As T) As T
            Dim bf As New ObjectFormatter()
            Using ms As New MemoryStream()
                bf.Serialize(ms, obj)
                Dim bytes() As Byte = ms.ToArray()
                Using bais As New MemoryStream(bytes)
                    Return DirectCast(bf.Deserialize(bais), T)
                End Using
            End Using
        End Function

        <TestMethod>
        Public Sub testCategoricalToInteger()
            Dim schema As SchemaT = getSchema(ColumnType.Categorical, "zero", "one", "two")

            Dim transform As TransformT = New CategoricalToIntegerTransform("column")
            transform.InputSchema = schema
            Dim [out] As SchemaT = transform.transform(schema)

            assertEquals(ColumnType.Integer, [out].getMetaData(0).ColumnType)
            Dim meta As IntegerMetaData = DirectCast([out].getMetaData(0), IntegerMetaData)
            assertNotNull(meta.getMinAllowedValue())
            assertEquals(0, meta.getMinAllowedValue().Value)

            assertNotNull(meta.getMaxAllowedValue())
            assertEquals(2, meta.getMaxAllowedValue().Value)

            assertEquals(0, transform.map(singleRow(New Text("zero")))(0).toInt())
            assertEquals(1, transform.map(singleRow(New Text("one")))(0).toInt())
            assertEquals(2, transform.map(singleRow(New Text("two")))(0).toInt())
        End Sub

        <TestMethod>
        Public Sub testCategoricalToOneHotTransform()
            Dim schema As SchemaT = getSchema(ColumnType.Categorical, "zero", "one", "two")

            Dim transform As TransformT = New CategoricalToOneHotTransform("column")
            transform.InputSchema = schema
            Dim [out] As SchemaT = transform.transform(schema)

            assertEquals(3, [out].getColumnMetaData().Count)
            For i As Integer = 0 To 2
                assertEquals(ColumnType.Integer, [out].getMetaData(i).ColumnType)
                Dim meta As IntegerMetaData = DirectCast([out].getMetaData(i), IntegerMetaData)
                assertNotNull(meta.getMinAllowedValue())
                assertEquals(0, meta.getMinAllowedValue().Value)

                assertNotNull(meta.getMaxAllowedValue())
                assertEquals(1, meta.getMaxAllowedValue().Value)
            Next i

            assertEquals(row(New IntWritable(1), New IntWritable(0), New IntWritable(0)), transform.map(singleRow(New Text("zero"))))
            assertEquals(row(New IntWritable(0), New IntWritable(1), New IntWritable(0)), transform.map(singleRow(New Text("one"))))
            assertEquals(row(New IntWritable(0), New IntWritable(0), New IntWritable(1)), transform.map(singleRow(New Text("two"))))
        End Sub

        <TestMethod>
        Public Sub testPivotTransform()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnString("otherCol").addColumnCategorical("key", Arrays.asList("first", "second", "third")).addColumnDouble("value").addColumnDouble("otherCol2").build()

            Dim t As TransformT = New PivotTransform("key", "value")
            t.InputSchema = schema
            Dim [out] As SchemaT = t.transform(schema)

            Dim expNames As IList(Of String) = Arrays.asList("otherCol", "key[first]", "key[second]", "key[third]", "otherCol2")
            Dim actNames As IList(Of String) = [out].getColumnNames()

            assertEquals(expNames, actNames)

            Dim columnTypesExp As IList(Of ColumnType) = Arrays.asList(ColumnType.String, ColumnType.Double, ColumnType.Double, ColumnType.Double, ColumnType.Double)
            assertEquals(columnTypesExp, [out].getColumnTypes())

            'Expand (second,100) into (0,100,0). Leave the remaining columns as is
            Dim e1 As IList(Of WritableT) = row(New DoubleWritable(1), New DoubleWritable(0), New DoubleWritable(100), New DoubleWritable(0), New DoubleWritable(-1))
            Dim a1 As IList(Of WritableT) = t.map(row(New DoubleWritable(1), New Text("second"), New DoubleWritable(100), New DoubleWritable(-1)))
            assertEquals(e1, a1)

            'Expand (third,200) into (0,0,200). Leave the remaining columns as is
            Dim e2 As IList(Of WritableT) = row(New DoubleWritable(1), New DoubleWritable(0), New DoubleWritable(0), New DoubleWritable(200), New DoubleWritable(-1))
            Dim a2 As IList(Of WritableT) = t.map(row(New DoubleWritable(1), New Text("third"), New DoubleWritable(200), New DoubleWritable(-1)))
            assertEquals(e2, a2)
        End Sub

        <TestMethod>
        Public Sub testIntegerToCategoricalTransform()
            Dim schema As SchemaT = getSchema(ColumnType.Integer)

            Dim transform As TransformT = New IntegerToCategoricalTransform("column", Arrays.asList("zero", "one", "two"))
            transform.InputSchema = schema
            Dim [out] As SchemaT = transform.transform(schema)

            assertEquals(1, [out].getColumnMetaData().Count)
            assertEquals(ColumnType.Categorical, [out].getMetaData(0).ColumnType)
            Dim meta As CategoricalMetaData = DirectCast([out].getMetaData(0), CategoricalMetaData)
            assertEquals(Arrays.asList("zero", "one", "two"), meta.getStateNames())

            assertEquals(singleRow(New Text("zero")), transform.map(singleRow(New IntWritable(0))))
            assertEquals(singleRow(New Text("one")), transform.map(singleRow(New IntWritable(1))))
            assertEquals(singleRow(New Text("two")), transform.map(singleRow(New IntWritable(2))))
        End Sub

        <TestMethod>
        Public Sub testIntegerToOneHotTransform()
            Dim schema As SchemaT = getSchema(ColumnType.Integer)

            Dim transform As TransformT = New IntegerToOneHotTransform("column", 3, 5)
            transform.InputSchema = schema
            Dim [out] As SchemaT = transform.transform(schema)

            assertEquals(3, [out].getColumnMetaData().Count)
            assertEquals(ColumnType.Integer, [out].getMetaData(0).ColumnType)
            assertEquals(ColumnType.Integer, [out].getMetaData(1).ColumnType)
            assertEquals(ColumnType.Integer, [out].getMetaData(2).ColumnType)

            assertEquals(Arrays.asList("column[3]", "column[4]", "column[5]"), [out].getColumnNames())

            assertEquals(row(New IntWritable(1), New IntWritable(0), New IntWritable(0)), transform.map(singleRow(New IntWritable(3))))
            assertEquals(row(New IntWritable(0), New IntWritable(1), New IntWritable(0)), transform.map(singleRow(New IntWritable(4))))
            assertEquals(row(New IntWritable(0), New IntWritable(0), New IntWritable(1)), transform.map(singleRow(New IntWritable(5))))
        End Sub

        <TestMethod>
        Public Sub testStringToCategoricalTransform()
            Dim schema As SchemaT = getSchema(ColumnType.String)

            Dim transform As TransformT = New StringToCategoricalTransform("column", Arrays.asList("zero", "one", "two"))
            transform.InputSchema = schema
            Dim [out] As SchemaT = transform.transform(schema)

            assertEquals(1, [out].getColumnMetaData().Count)
            assertEquals(ColumnType.Categorical, [out].getMetaData(0).ColumnType)
            Dim meta As CategoricalMetaData = DirectCast([out].getMetaData(0), CategoricalMetaData)
            assertEquals(Arrays.asList("zero", "one", "two"), meta.getStateNames())

            assertEquals(singleRow(New Text("zero")), transform.map(singleRow(New Text("zero"))))
            assertEquals(singleRow(New Text("one")), transform.map(singleRow(New Text("one"))))
            assertEquals(singleRow(New Text("two")), transform.map(singleRow(New Text("two"))))
        End Sub

        <TestMethod>
        Public Sub testConcatenateStringColumnsTransform()
            Const DELIMITER As String = " "
            Const NEW_COLUMN As String = "NewColumn"
            Dim CONCAT_COLUMNS As IList(Of String) = Arrays.asList("ConcatenatedColumn1", "ConcatenatedColumn2", "ConcatenatedColumn3")
            Dim ALL_COLUMNS As IList(Of String) = Arrays.asList("ConcatenatedColumn1", "OtherColumn4", "ConcatenatedColumn2", "OtherColumn5", "ConcatenatedColumn3", "OtherColumn6")
            Dim COLUMN_VALUES As IList(Of Text) = Arrays.asList(New Text("string1"), New Text("other4"), New Text("string2"), New Text("other5"), New Text("string3"), New Text("other6"))
            Const NEW_COLUMN_VALUE As String = "string1 string2 string3"

            Dim transform As TransformT = New ConcatenateStringColumns(NEW_COLUMN, DELIMITER, CONCAT_COLUMNS)
            Dim allColumns() As String = New List(Of String)(ALL_COLUMNS).ToArray()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnsString(allColumns).build()

            Dim outputColumns As IList(Of String) = New List(Of String)(ALL_COLUMNS)
            outputColumns.Add(NEW_COLUMN)
            Dim newSchema As SchemaT = transform.transform(schema)
            assertEquals(outputColumns, newSchema.getColumnNames())

            Dim input As New List(Of WritableT)()
            input.AddRange(COLUMN_VALUES)

            transform.InputSchema = schema
            Dim transformed As IList(Of WritableT) = transform.map(input)
            assertEquals(NEW_COLUMN_VALUE, transformed(transformed.Count - 1).ToString())

            Dim outputColumnValues As IList(Of Text) = New List(Of Text)(COLUMN_VALUES)
            outputColumnValues.Add(New Text(NEW_COLUMN_VALUE))
            assertEquals(outputColumnValues, transformed)

            ' JSON round trip (JsonMappers) not ported: needs the serde wave.
        End Sub

        <TestMethod>
        Public Sub testChangeCaseStringTransform()
            Const STRING_COLUMN As String = "StringColumn"
            Dim ALL_COLUMNS As IList(Of String) = Arrays.asList(STRING_COLUMN, "OtherColumn")
            Const TEXT_MIXED_CASE As String = "UPPER lower MiXeD"
            Dim TEXT_UPPER_CASE As String = TEXT_MIXED_CASE.ToUpperInvariant()
            Dim TEXT_LOWER_CASE As String = TEXT_MIXED_CASE.ToLowerInvariant()

            Dim transform As TransformT = New ChangeCaseStringTransform(STRING_COLUMN)
            Dim allColumns() As String = New List(Of String)(ALL_COLUMNS).ToArray()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnsString(allColumns).build()
            transform.InputSchema = schema
            Dim newSchema As SchemaT = transform.transform(schema)
            Dim outputColumns As IList(Of String) = New List(Of String)(ALL_COLUMNS)
            assertEquals(outputColumns, newSchema.getColumnNames())

            transform = New ChangeCaseStringTransform(STRING_COLUMN, ChangeCaseStringTransform.CaseType.LOWER)
            transform.InputSchema = schema
            Dim input As IList(Of WritableT) = New List(Of WritableT)()
            input.Add(New Text(TEXT_MIXED_CASE))
            input.Add(New Text(TEXT_MIXED_CASE))
            Dim output As IList(Of WritableT) = New List(Of WritableT)()
            output.Add(New Text(TEXT_LOWER_CASE))
            output.Add(New Text(TEXT_MIXED_CASE))
            Dim transformed As IList(Of WritableT) = transform.map(input)
            assertEquals(transformed(0).ToString(), TEXT_LOWER_CASE)
            assertEquals(transformed, output)

            transform = New ChangeCaseStringTransform(STRING_COLUMN, ChangeCaseStringTransform.CaseType.UPPER)
            transform.InputSchema = schema
            output.Clear()
            output.Add(New Text(TEXT_UPPER_CASE))
            output.Add(New Text(TEXT_MIXED_CASE))
            transformed = transform.map(input)
            assertEquals(transformed(0).ToString(), TEXT_UPPER_CASE)
            assertEquals(transformed, output)

            ' JSON round trip (JsonMappers) not ported: needs the serde wave.
        End Sub

        <TestMethod>
        Public Sub testRemoveColumnsTransform()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnDouble("first").addColumnString("second").addColumnInteger("third").addColumnLong("fourth").build()

            Dim transform As TransformT = New RemoveColumnsTransform("first", "fourth")
            transform.InputSchema = schema

            Dim [out] As SchemaT = transform.transform(schema)

            assertEquals(2, [out].getColumnMetaData().Count)
            assertEquals(ColumnType.String, [out].getMetaData(0).ColumnType)
            assertEquals(ColumnType.Integer, [out].getMetaData(1).ColumnType)

            assertEquals(row(New Text("one"), New IntWritable(1)), transform.map(row(New DoubleWritable(1.0), New Text("one"), New IntWritable(1), New LongWritable(1L))))
        End Sub

        <TestMethod>
        Public Sub testRemoveAllColumnsExceptForTransform()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnDouble("first").addColumnString("second").addColumnInteger("third").addColumnLong("fourth").build()

            Dim transform As TransformT = New RemoveAllColumnsExceptForTransform("second", "third")
            transform.InputSchema = schema

            Dim [out] As SchemaT = transform.transform(schema)

            assertEquals(2, [out].getColumnMetaData().Count)
            assertEquals(ColumnType.String, [out].getMetaData(0).ColumnType)
            assertEquals(ColumnType.Integer, [out].getMetaData(1).ColumnType)

            assertEquals(row(New Text("one"), New IntWritable(1)), transform.map(row(New DoubleWritable(1.0), New Text("one"), New IntWritable(1), New LongWritable(1L))))
        End Sub

        <TestMethod>
        Public Sub testReplaceEmptyIntegerWithValueTransform()
            Dim schema As SchemaT = getSchema(ColumnType.Integer)

            Dim transform As TransformT = New ReplaceEmptyIntegerWithValueTransform("column", 1000)
            transform.InputSchema = schema
            Dim [out] As SchemaT = transform.transform(schema)

            assertEquals(1, [out].getColumnMetaData().Count)
            assertEquals(ColumnType.Integer, [out].getMetaData(0).ColumnType)

            assertEquals(singleRow(New IntWritable(0)), transform.map(singleRow(New IntWritable(0))))
            assertEquals(singleRow(New IntWritable(1)), transform.map(singleRow(New IntWritable(1))))
            assertEquals(singleRow(New IntWritable(1000)), transform.map(singleRow(New Text(""))))
        End Sub

        <TestMethod>
        Public Sub testReplaceInvalidWithIntegerTransform()
            Dim schema As SchemaT = getSchema(ColumnType.Integer)

            Dim transform As TransformT = New ReplaceInvalidWithIntegerTransform("column", 1000)
            transform.InputSchema = schema
            Dim [out] As SchemaT = transform.transform(schema)

            assertEquals(1, [out].getColumnMetaData().Count)
            assertEquals(ColumnType.Integer, [out].getMetaData(0).ColumnType)

            assertEquals(singleRow(New IntWritable(0)), transform.map(singleRow(New IntWritable(0))))
            assertEquals(singleRow(New IntWritable(1)), transform.map(singleRow(New IntWritable(1))))
            assertEquals(singleRow(New IntWritable(1000)), transform.map(singleRow(New Text(""))))
        End Sub

        <TestMethod>
        Public Sub testLog2Normalizer()
            Dim schema As SchemaT = getSchema(ColumnType.Double)

            Dim mu As Double = 2.0
            Dim min As Double = 1.0
            Dim scale As Double = 0.5

            Dim transform As TransformT = New Log2Normalizer("column", mu, min, scale)
            transform.InputSchema = schema

            Dim [out] As SchemaT = transform.transform(schema)

            assertEquals(1, [out].getColumnMetaData().Count)
            assertEquals(ColumnType.Double, [out].getMetaData(0).ColumnType)
            Dim meta As DoubleMetaData = DirectCast([out].getMetaData(0), DoubleMetaData)
            assertNotNull(meta.getMinAllowedValue())
            assertEquals(0, meta.getMinAllowedValue().Value, 1e-6)
            assertNull(meta.getMaxAllowedValue())

            Dim loge2 As Double = Math.Log(2)
            assertEquals(0.0, transform.map(singleRow(New DoubleWritable(min)))(0).toDouble(), 1e-6)
            Dim d As Double = scale * Math.Log((10 - min) / (mu - min) + 1) / loge2
            assertEquals(d, transform.map(singleRow(New DoubleWritable(10)))(0).toDouble(), 1e-6)
            d = scale * Math.Log((3 - min) / (mu - min) + 1) / loge2
            assertEquals(d, transform.map(singleRow(New DoubleWritable(3)))(0).toDouble(), 1e-6)
        End Sub

        <TestMethod>
        Public Sub testDoubleMinMaxNormalizerTransform()
            Dim schema As SchemaT = getSchema(ColumnType.Double)

            Dim transform As TransformT = New MinMaxNormalizer("column", 0, 100)
            Dim transform2 As TransformT = New MinMaxNormalizer("column", 0, 100, -1, 1)
            transform.InputSchema = schema
            transform2.InputSchema = schema

            Dim [out] As SchemaT = transform.transform(schema)
            Dim out2 As SchemaT = transform2.transform(schema)

            assertEquals(1, [out].getColumnMetaData().Count)
            assertEquals(ColumnType.Double, [out].getMetaData(0).ColumnType)
            Dim meta As DoubleMetaData = DirectCast([out].getMetaData(0), DoubleMetaData)
            Dim meta2 As DoubleMetaData = DirectCast(out2.getMetaData(0), DoubleMetaData)
            assertEquals(0, meta.getMinAllowedValue().Value, 1e-6)
            assertEquals(1, meta.getMaxAllowedValue().Value, 1e-6)
            assertEquals(-1, meta2.getMinAllowedValue().Value, 1e-6)
            assertEquals(1, meta2.getMaxAllowedValue().Value, 1e-6)

            assertEquals(0.0, transform.map(singleRow(New DoubleWritable(0)))(0).toDouble(), 1e-6)
            assertEquals(1.0, transform.map(singleRow(New DoubleWritable(100)))(0).toDouble(), 1e-6)
            assertEquals(0.5, transform.map(singleRow(New DoubleWritable(50)))(0).toDouble(), 1e-6)

            assertEquals(-1.0, transform2.map(singleRow(New DoubleWritable(0)))(0).toDouble(), 1e-6)
            assertEquals(1.0, transform2.map(singleRow(New DoubleWritable(100)))(0).toDouble(), 1e-6)
            assertEquals(0.0, transform2.map(singleRow(New DoubleWritable(50)))(0).toDouble(), 1e-6)
        End Sub

        <TestMethod>
        Public Sub testStandardizeNormalizer()
            Dim schema As SchemaT = getSchema(ColumnType.Double)

            Dim mu As Double = 1.0
            Dim sigma As Double = 2.0

            Dim transform As TransformT = New StandardizeNormalizer("column", mu, sigma)
            transform.InputSchema = schema

            Dim [out] As SchemaT = transform.transform(schema)

            assertEquals(1, [out].getColumnMetaData().Count)
            assertEquals(ColumnType.Double, [out].getMetaData(0).ColumnType)
            Dim meta As DoubleMetaData = DirectCast([out].getMetaData(0), DoubleMetaData)
            assertNull(meta.getMinAllowedValue())
            assertNull(meta.getMaxAllowedValue())

            assertEquals(0.0, transform.map(singleRow(New DoubleWritable(mu)))(0).toDouble(), 1e-6)
            Dim d As Double = (10 - mu) / sigma
            assertEquals(d, transform.map(singleRow(New DoubleWritable(10)))(0).toDouble(), 1e-6)
            d = (-2 - mu) / sigma
            assertEquals(d, transform.map(singleRow(New DoubleWritable(-2)))(0).toDouble(), 1e-6)
        End Sub

        <TestMethod>
        Public Sub testSubtractMeanNormalizer()
            Dim schema As SchemaT = getSchema(ColumnType.Double)

            Dim mu As Double = 1.0

            Dim transform As TransformT = New SubtractMeanNormalizer("column", mu)
            transform.InputSchema = schema

            Dim [out] As SchemaT = transform.transform(schema)

            assertEquals(1, [out].getColumnMetaData().Count)
            assertEquals(ColumnType.Double, [out].getMetaData(0).ColumnType)
            Dim meta As DoubleMetaData = DirectCast([out].getMetaData(0), DoubleMetaData)
            assertNull(meta.getMinAllowedValue())
            assertNull(meta.getMaxAllowedValue())

            assertEquals(0.0, transform.map(singleRow(New DoubleWritable(mu)))(0).toDouble(), 1e-6)
            assertEquals(10 - mu, transform.map(singleRow(New DoubleWritable(10)))(0).toDouble(), 1e-6)
        End Sub

        <TestMethod>
        Public Sub testMapAllStringsExceptListTransform()
            Dim schema As SchemaT = getSchema(ColumnType.String)

            Dim transform As TransformT = New MapAllStringsExceptListTransform("column", "replacement", Arrays.asList("one", "two", "three"))
            transform.InputSchema = schema
            Dim [out] As SchemaT = transform.transform(schema)

            assertEquals(1, [out].getColumnMetaData().Count)
            assertEquals(ColumnType.String, [out].getMetaData(0).ColumnType)

            assertEquals(singleRow(New Text("one")), transform.map(singleRow(New Text("one"))))
            assertEquals(singleRow(New Text("two")), transform.map(singleRow(New Text("two"))))
            assertEquals(singleRow(New Text("replacement")), transform.map(singleRow(New Text("this should be replaced"))))
        End Sub

        <TestMethod>
        Public Sub testRemoveWhitespaceTransform()
            Dim schema As SchemaT = getSchema(ColumnType.String)

            Dim transform As TransformT = New RemoveWhiteSpaceTransform("column")
            transform.InputSchema = schema
            Dim [out] As SchemaT = transform.transform(schema)

            assertEquals(1, [out].getColumnMetaData().Count)
            assertEquals(ColumnType.String, [out].getMetaData(0).ColumnType)

            assertEquals(singleRow(New Text("one")), transform.map(singleRow(New Text("one "))))
            assertEquals(singleRow(New Text("two")), transform.map(singleRow(New Text("two" & vbTab))))
            assertEquals(singleRow(New Text("three")), transform.map(singleRow(New Text("three" & vbLf))))
            assertEquals(singleRow(New Text("one")), transform.map(singleRow(New Text(" o n e" & vbTab))))
        End Sub

        <TestMethod>
        Public Sub testReplaceEmptyStringTransform()
            Dim schema As SchemaT = getSchema(ColumnType.String)

            Dim transform As TransformT = New ReplaceEmptyStringTransform("column", "newvalue")
            transform.InputSchema = schema
            Dim [out] As SchemaT = transform.transform(schema)

            assertEquals(1, [out].getColumnMetaData().Count)
            assertEquals(ColumnType.String, [out].getMetaData(0).ColumnType)

            assertEquals(singleRow(New Text("one")), transform.map(singleRow(New Text("one"))))
            assertEquals(singleRow(New Text("newvalue")), transform.map(singleRow(New Text(""))))
            assertEquals(singleRow(New Text("three")), transform.map(singleRow(New Text("three"))))
        End Sub

        <TestMethod>
        Public Sub testAppendStringColumnTransform()
            Dim schema As SchemaT = getSchema(ColumnType.String)

            Dim transform As TransformT = New AppendStringColumnTransform("column", "_AppendThis")
            transform.InputSchema = schema
            Dim [out] As SchemaT = transform.transform(schema)

            assertEquals(1, [out].getColumnMetaData().Count)
            assertEquals(ColumnType.String, [out].getMetaData(0).ColumnType)

            assertEquals(singleRow(New Text("one_AppendThis")), transform.map(singleRow(New Text("one"))))
            assertEquals(singleRow(New Text("two_AppendThis")), transform.map(singleRow(New Text("two"))))
            assertEquals(singleRow(New Text("three_AppendThis")), transform.map(singleRow(New Text("three"))))
        End Sub

        <TestMethod>
        Public Sub testStringListToCategoricalSetTransform()
            'Idea: String list to a set of categories... "a,c" for categories {a,b,c} -> "true","false","true"

            Dim schema As SchemaT = getSchema(ColumnType.String)

            Dim transform As TransformT = New StringListToCategoricalSetTransform("column", Arrays.asList("a", "b", "c"), Arrays.asList("a", "b", "c"), ",")
            transform.InputSchema = schema

            Dim [out] As SchemaT = transform.transform(schema)
            assertEquals(3, [out].getColumnMetaData().Count)
            For i As Integer = 0 To 2
                assertEquals(ColumnType.Categorical, [out].getType(i))
                Dim meta As CategoricalMetaData = DirectCast([out].getMetaData(i), CategoricalMetaData)
                assertEquals(Arrays.asList("true", "false"), meta.getStateNames())
            Next i

            assertEquals(row(New Text("false"), New Text("false"), New Text("false")), transform.map(singleRow(New Text(""))))
            assertEquals(row(New Text("true"), New Text("false"), New Text("false")), transform.map(singleRow(New Text("a"))))
            assertEquals(row(New Text("false"), New Text("true"), New Text("false")), transform.map(singleRow(New Text("b"))))
            assertEquals(row(New Text("false"), New Text("false"), New Text("true")), transform.map(singleRow(New Text("c"))))
            assertEquals(row(New Text("true"), New Text("false"), New Text("true")), transform.map(singleRow(New Text("a,c"))))
            assertEquals(row(New Text("true"), New Text("true"), New Text("true")), transform.map(singleRow(New Text("a,b,c"))))
        End Sub

        <TestMethod>
        Public Sub testStringMapTransform()
            Dim schema As SchemaT = getSchema(ColumnType.String)

            Dim map As IDictionary(Of String, String) = New Dictionary(Of String, String)()
            map("one") = "ONE"
            map("two") = "TWO"
            Dim transform As TransformT = New StringMapTransform("column", map)
            transform.InputSchema = schema
            Dim [out] As SchemaT = transform.transform(schema)

            assertEquals(1, [out].getColumnMetaData().Count)
            assertEquals(ColumnType.String, [out].getMetaData(0).ColumnType)

            assertEquals(singleRow(New Text("ONE")), transform.map(singleRow(New Text("one"))))
            assertEquals(singleRow(New Text("TWO")), transform.map(singleRow(New Text("two"))))
            assertEquals(singleRow(New Text("three")), transform.map(singleRow(New Text("three"))))
        End Sub

        <TestMethod>
        Public Sub testStringToTimeTransform()
            testStringToDateTime("YYYY-MM-dd HH:mm:ss")
        End Sub

        <TestMethod>
        Public Sub testStringToTimeTransformNoDateTime()
            Dim schema As SchemaT = getSchema(ColumnType.String)
            Dim dateTime As String = "2017-09-21T17:06:29.064687"
            Dim dateTime2 As String = "2007-12-30"
            Dim dateTime3 As String = "12/1/2010 11:21"

            'http://www.joda.org/joda-time/apidocs/org/joda/time/format/DateTimeFormat.html
            Dim transform As New StringToTimeTransform("column", Nothing, DateTimeZone.forID("UTC"))
            transform.InputSchema = schema
            transform.map(New Text(dateTime3))
            transform.map(New Text(dateTime))
            transform.map(New Text(dateTime2))
            testStringToDateTime(Nothing)
        End Sub

        Private Sub testStringToDateTime(ByVal timeFormat As String)
            Dim schema As SchemaT = getSchema(ColumnType.String)

            'http://www.joda.org/joda-time/apidocs/org/joda/time/format/DateTimeFormat.html
            Dim transform As TransformT = New StringToTimeTransform("column", timeFormat, DateTimeZone.forID("UTC"))
            transform.InputSchema = schema

            Dim [out] As SchemaT = transform.transform(schema)

            assertEquals(1, [out].getColumnMetaData().Count)
            assertEquals(ColumnType.Time, [out].getMetaData(0).ColumnType)

            Dim in1 As String = "2016-01-01 12:30:45"
            Dim out1 As Long = 1451651445000L

            Dim in2 As String = "2015-06-30 23:59:59"
            Dim out2 As Long = 1435708799000L

            assertEquals(singleRow(New LongWritable(out1)), transform.map(singleRow(New Text(in1))))
            assertEquals(singleRow(New LongWritable(out2)), transform.map(singleRow(New Text(in2))))

            'Check serialization: things like DateTimeFormatter etc aren't serializable, hence we need custom serialization :/
            Dim deserialized As TransformT = serializeRoundTrip(transform)
            assertEquals(singleRow(New LongWritable(out1)), deserialized.map(singleRow(New Text(in1))))
            assertEquals(singleRow(New LongWritable(out2)), deserialized.map(singleRow(New Text(in2))))
        End Sub

        <TestMethod>
        Public Sub testDeriveColumnsFromTimeTransform()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnTime("column", DateTimeZone.forID("UTC")).addColumnString("otherColumn").build()

            Dim transform As TransformT = (New DeriveColumnsFromTimeTransform.Builder("column")).insertAfter("otherColumn").addIntegerDerivedColumn("hour", DateTimeFieldType.hourOfDay()).addIntegerDerivedColumn("day", DateTimeFieldType.dayOfMonth()).addIntegerDerivedColumn("second", DateTimeFieldType.secondOfMinute()).addStringDerivedColumn("humanReadable", "YYYY-MM-dd HH:mm:ss", DateTimeZone.UTC).build()

            transform.InputSchema = schema
            Dim [out] As SchemaT = transform.transform(schema)

            assertEquals(6, [out].getColumnMetaData().Count)
            assertEquals(ColumnType.Time, [out].getMetaData(0).ColumnType)
            assertEquals(ColumnType.String, [out].getMetaData(1).ColumnType)
            assertEquals(ColumnType.Integer, [out].getMetaData(2).ColumnType)
            assertEquals(ColumnType.Integer, [out].getMetaData(3).ColumnType)
            assertEquals(ColumnType.Integer, [out].getMetaData(4).ColumnType)
            assertEquals(ColumnType.String, [out].getMetaData(5).ColumnType)

            assertEquals("column", [out].getName(0))
            assertEquals("otherColumn", [out].getName(1))
            assertEquals("hour", [out].getName(2))
            assertEquals("day", [out].getName(3))
            assertEquals("second", [out].getName(4))
            assertEquals("humanReadable", [out].getName(5))

            Dim in1 As Long = 1451651445000L '"2016-01-01 12:30:45" GMT

            Dim out1 As IList(Of WritableT) = New List(Of WritableT)()
            out1.Add(New LongWritable(in1))
            out1.Add(New Text("otherColumnValue"))
            out1.Add(New IntWritable(12)) 'hour
            out1.Add(New IntWritable(1)) 'day
            out1.Add(New IntWritable(45)) 'second
            out1.Add(New Text("2016-01-01 12:30:45"))

            Dim in2 As Long = 1435708799000L '"2015-06-30 23:59:59" GMT
            Dim out2 As IList(Of WritableT) = New List(Of WritableT)()
            out2.Add(New LongWritable(in2))
            out2.Add(New Text("otherColumnValue"))
            out2.Add(New IntWritable(23)) 'hour
            out2.Add(New IntWritable(30)) 'day
            out2.Add(New IntWritable(59)) 'second
            out2.Add(New Text("2015-06-30 23:59:59"))

            assertEquals(out1, transform.map(row(New LongWritable(in1), New Text("otherColumnValue"))))
            assertEquals(out2, transform.map(row(New LongWritable(in2), New Text("otherColumnValue"))))

            'Check serialization: things like DateTimeFormatter etc aren't serializable, hence we need custom serialization :/
            Dim deserialized As TransformT = serializeRoundTrip(transform)
            assertEquals(out1, deserialized.map(row(New LongWritable(in1), New Text("otherColumnValue"))))
            assertEquals(out2, deserialized.map(row(New LongWritable(in2), New Text("otherColumnValue"))))
        End Sub

        <TestMethod>
        Public Sub testDuplicateColumnsTransform()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnString("stringCol").addColumnInteger("intCol").addColumnLong("longCol").build()

            Dim toDup As IList(Of String) = Arrays.asList("intCol", "longCol")
            Dim newNames As IList(Of String) = Arrays.asList("dup_intCol", "dup_longCol")

            Dim transform As TransformT = New DuplicateColumnsTransform(toDup, newNames)
            transform.InputSchema = schema

            Dim [out] As SchemaT = transform.transform(schema)
            assertEquals(5, [out].getColumnMetaData().Count)

            Dim expOutNames As IList(Of String) = Arrays.asList("stringCol", "intCol", "dup_intCol", "longCol", "dup_longCol")
            Dim expOutTypes As IList(Of ColumnType) = Arrays.asList(ColumnType.String, ColumnType.Integer, ColumnType.Integer, ColumnType.Long, ColumnType.Long)
            For i As Integer = 0 To 4
                assertEquals(expOutNames(i), [out].getName(i))
                assertEquals(expOutTypes(i), [out].getType(i))
            Next i

            Dim inList As IList(Of WritableT) = row(New Text("one"), New IntWritable(2), New LongWritable(3L))
            Dim outList As IList(Of WritableT) = row(New Text("one"), New IntWritable(2), New IntWritable(2), New LongWritable(3L), New LongWritable(3L))

            assertEquals(outList, transform.map(inList))
        End Sub

        <TestMethod>
        Public Sub testIntegerMathOpTransform()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnInteger("column", -1, 1).build()

            Dim transform As TransformT = New IntegerMathOpTransform("column", MathOp.Multiply, 5)
            transform.InputSchema = schema

            Dim [out] As SchemaT = transform.transform(schema)
            assertEquals(1, [out].getColumnMetaData().Count)
            assertEquals(ColumnType.Integer, [out].getType(0))
            Dim meta As IntegerMetaData = DirectCast([out].getMetaData(0), IntegerMetaData)
            assertEquals(-5, meta.getMinAllowedValue().Value)
            assertEquals(5, meta.getMaxAllowedValue().Value)

            assertEquals(singleRow(New IntWritable(-5)), transform.map(singleRow(New IntWritable(-1))))
            assertEquals(singleRow(New IntWritable(0)), transform.map(singleRow(New IntWritable(0))))
            assertEquals(singleRow(New IntWritable(5)), transform.map(singleRow(New IntWritable(1))))
        End Sub

        <TestMethod>
        Public Sub testIntegerColumnsMathOpTransform()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnInteger("first").addColumnString("second").addColumnInteger("third").build()

            Dim transform As TransformT = New IntegerColumnsMathOpTransform("out", MathOp.Add, "first", "third")
            transform.InputSchema = schema

            Dim [out] As SchemaT = transform.transform(schema)
            assertEquals(4, [out].numColumns())
            assertEquals(Arrays.asList("first", "second", "third", "out"), [out].getColumnNames())
            assertEquals(Arrays.asList(ColumnType.Integer, ColumnType.String, ColumnType.Integer, ColumnType.Integer), [out].getColumnTypes())

            assertEquals(row(New IntWritable(1), New Text("something"), New IntWritable(2), New IntWritable(3)), transform.map(row(New IntWritable(1), New Text("something"), New IntWritable(2))))
            assertEquals(row(New IntWritable(100), New Text("something2"), New IntWritable(21), New IntWritable(121)), transform.map(row(New IntWritable(100), New Text("something2"), New IntWritable(21))))
        End Sub

        <TestMethod>
        Public Sub testLongMathOpTransform()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnLong("column", -1L, 1L).build()

            Dim transform As TransformT = New LongMathOpTransform("column", MathOp.Multiply, 5)
            transform.InputSchema = schema

            Dim [out] As SchemaT = transform.transform(schema)
            assertEquals(1, [out].getColumnMetaData().Count)
            assertEquals(ColumnType.Long, [out].getType(0))
            Dim meta As LongMetaData = DirectCast([out].getMetaData(0), LongMetaData)
            assertEquals(-5L, meta.getMinAllowedValue().Value)
            assertEquals(5L, meta.getMaxAllowedValue().Value)

            assertEquals(singleRow(New LongWritable(-5)), transform.map(singleRow(New LongWritable(-1))))
            assertEquals(singleRow(New LongWritable(0)), transform.map(singleRow(New LongWritable(0))))
            assertEquals(singleRow(New LongWritable(5)), transform.map(singleRow(New LongWritable(1))))
        End Sub

        <TestMethod>
        Public Sub testLongColumnsMathOpTransform()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnLong("first").addColumnString("second").addColumnLong("third").build()

            Dim transform As TransformT = New LongColumnsMathOpTransform("out", MathOp.Add, "first", "third")
            transform.InputSchema = schema

            Dim [out] As SchemaT = transform.transform(schema)
            assertEquals(4, [out].numColumns())
            assertEquals(Arrays.asList("first", "second", "third", "out"), [out].getColumnNames())
            assertEquals(Arrays.asList(ColumnType.Long, ColumnType.String, ColumnType.Long, ColumnType.Long), [out].getColumnTypes())

            assertEquals(row(New LongWritable(1), New Text("something"), New LongWritable(2), New LongWritable(3)), transform.map(row(New LongWritable(1), New Text("something"), New LongWritable(2))))
            assertEquals(row(New LongWritable(100), New Text("something2"), New LongWritable(21), New LongWritable(121)), transform.map(row(New LongWritable(100), New Text("something2"), New LongWritable(21))))
        End Sub

        <TestMethod>
        Public Sub testTimeMathOpTransform()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnTime("column", DateTimeZone.UTC).build()

            Dim transform As TransformT = New TimeMathOpTransform("column", MathOp.Add, 12, TimeUnit.HOURS) '12 hours: 43200000 milliseconds
            transform.InputSchema = schema

            Dim [out] As SchemaT = transform.transform(schema)
            assertEquals(1, [out].getColumnMetaData().Count)
            assertEquals(ColumnType.Time, [out].getType(0))

            assertEquals(singleRow(New LongWritable(1000 + 43200000)), transform.map(singleRow(New LongWritable(1000))))
            assertEquals(singleRow(New LongWritable(1452441600000L + 43200000)), transform.map(singleRow(New LongWritable(1452441600000L))))
        End Sub

        <TestMethod>
        Public Sub testDoubleMathOpTransform()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnDouble("column", -1.0, 1.0).build()

            Dim transform As TransformT = New DoubleMathOpTransform("column", MathOp.Multiply, 5.0)
            transform.InputSchema = schema

            Dim [out] As SchemaT = transform.transform(schema)
            assertEquals(1, [out].getColumnMetaData().Count)
            assertEquals(ColumnType.Double, [out].getType(0))
            Dim meta As DoubleMetaData = DirectCast([out].getMetaData(0), DoubleMetaData)
            assertEquals(-5.0, meta.getMinAllowedValue().Value, 1e-6)
            assertEquals(5.0, meta.getMaxAllowedValue().Value, 1e-6)

            assertEquals(singleRow(New DoubleWritable(-5)), transform.map(singleRow(New DoubleWritable(-1))))
            assertEquals(singleRow(New DoubleWritable(0)), transform.map(singleRow(New DoubleWritable(0))))
            assertEquals(singleRow(New DoubleWritable(5)), transform.map(singleRow(New DoubleWritable(1))))
        End Sub

        <TestMethod>
        Public Sub testDoubleMathFunctionTransform()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnDouble("column").addColumnString("strCol").build()

            Dim transform As TransformT = New DoubleMathFunctionTransform("column", MathFunction.SIN)
            transform.InputSchema = schema

            Dim [out] As SchemaT = transform.transform(schema)
            assertEquals(2, [out].getColumnMetaData().Count)
            assertEquals(ColumnType.Double, [out].getType(0))
            assertEquals(ColumnType.String, [out].getType(1))

            assertEquals(row(New DoubleWritable(Math.Sin(1)), New Text("0")), transform.map(row(New DoubleWritable(1), New Text("0"))))
            assertEquals(row(New DoubleWritable(Math.Sin(2)), New Text("1")), transform.map(row(New DoubleWritable(2), New Text("1"))))
            assertEquals(row(New DoubleWritable(Math.Sin(3)), New Text("2")), transform.map(row(New DoubleWritable(3), New Text("2"))))
        End Sub

        <TestMethod>
        Public Sub testDoubleColumnsMathOpTransform()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnString("first").addColumnDouble("second").addColumnDouble("third").build()

            Dim transform As TransformT = New DoubleColumnsMathOpTransform("out", MathOp.Add, "second", "third")
            transform.InputSchema = schema

            Dim [out] As SchemaT = transform.transform(schema)
            assertEquals(4, [out].numColumns())
            assertEquals(Arrays.asList("first", "second", "third", "out"), [out].getColumnNames())
            assertEquals(Arrays.asList(ColumnType.String, ColumnType.Double, ColumnType.Double, ColumnType.Double), [out].getColumnTypes())

            assertEquals(row(New Text("something"), New DoubleWritable(1.0), New DoubleWritable(2.1), New DoubleWritable(3.1)), transform.map(row(New Text("something"), New DoubleWritable(1.0), New DoubleWritable(2.1))))
            assertEquals(row(New Text("something2"), New DoubleWritable(100.0), New DoubleWritable(21.1), New DoubleWritable(121.1)), transform.map(row(New Text("something2"), New DoubleWritable(100.0), New DoubleWritable(21.1))))
        End Sub

        <TestMethod>
        Public Sub testRenameColumnsTransform()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnDouble("col1").addColumnString("col2").addColumnInteger("col3").build()

            Dim transform As TransformT = New RenameColumnsTransform(Arrays.asList("col1", "col3"), Arrays.asList("column1", "column3"))
            transform.InputSchema = schema

            Dim [out] As SchemaT = transform.transform(schema)

            assertEquals(3, [out].getColumnMetaData().Count)
            assertEquals(ColumnType.Double, [out].getMetaData(0).ColumnType)
            assertEquals(ColumnType.String, [out].getMetaData(1).ColumnType)
            assertEquals(ColumnType.Integer, [out].getMetaData(2).ColumnType)

            assertEquals("column1", [out].getName(0))
            assertEquals("col2", [out].getName(1))
            assertEquals("column3", [out].getName(2))
        End Sub

        <TestMethod>
        Public Sub testReorderColumnsTransform()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnDouble("col1").addColumnString("col2").addColumnInteger("col3").build()

            Dim transform As TransformT = New ReorderColumnsTransform("col3", "col2")
            transform.InputSchema = schema

            Dim [out] As SchemaT = transform.transform(schema)

            assertEquals(3, [out].numColumns())
            assertEquals(Arrays.asList("col3", "col2", "col1"), [out].getColumnNames())
            assertEquals(Arrays.asList(ColumnType.Integer, ColumnType.String, ColumnType.Double), [out].getColumnTypes())

            assertEquals(row(New IntWritable(1), New Text("one"), New DoubleWritable(1.1)), transform.map(row(New DoubleWritable(1.1), New Text("one"), New IntWritable(1))))

            assertEquals(row(New IntWritable(2), New Text("two"), New DoubleWritable(200.2)), transform.map(row(New DoubleWritable(200.2), New Text("two"), New IntWritable(2))))
        End Sub

        <TestMethod>
        Public Sub testConditionalReplaceValueTransform()
            Dim schema As SchemaT = getSchema(ColumnType.Integer)

            Dim condition As ConditionT = New IntegerColumnCondition("column", ConditionOp.LessThan, 0)
            condition.InputSchema = schema

            Dim transform As TransformT = New ConditionalReplaceValueTransform("column", New IntWritable(0), condition)
            transform.InputSchema = schema

            assertEquals(singleRow(New IntWritable(10)), transform.map(singleRow(New IntWritable(10))))
            assertEquals(singleRow(New IntWritable(1)), transform.map(singleRow(New IntWritable(1))))
            assertEquals(singleRow(New IntWritable(0)), transform.map(singleRow(New IntWritable(0))))
            assertEquals(singleRow(New IntWritable(0)), transform.map(singleRow(New IntWritable(-1))))
            assertEquals(singleRow(New IntWritable(0)), transform.map(singleRow(New IntWritable(-10))))
        End Sub

        <TestMethod>
        Public Sub testConditionalReplaceValueTransformWithDefault()
            Dim schema As SchemaT = getSchema(ColumnType.Integer)

            Dim condition As ConditionT = New IntegerColumnCondition("column", ConditionOp.LessThan, 0)
            condition.InputSchema = schema

            Dim transform As TransformT = New ConditionalReplaceValueTransformWithDefault("column", New IntWritable(0), New IntWritable(1), condition)
            transform.InputSchema = schema

            assertEquals(singleRow(New IntWritable(1)), transform.map(singleRow(New IntWritable(10))))
            assertEquals(singleRow(New IntWritable(1)), transform.map(singleRow(New IntWritable(1))))
            assertEquals(singleRow(New IntWritable(1)), transform.map(singleRow(New IntWritable(0))))
            assertEquals(singleRow(New IntWritable(0)), transform.map(singleRow(New IntWritable(-1))))
            assertEquals(singleRow(New IntWritable(0)), transform.map(singleRow(New IntWritable(-10))))
        End Sub

        <TestMethod>
        Public Sub testConditionalCopyValueTransform()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnsString("first", "second", "third").build()

            Dim condition As ConditionT = New StringColumnCondition("third", ConditionOp.Equal, "")
            Dim transform As TransformT = New ConditionalCopyValueTransform("third", "second", condition)
            transform.InputSchema = schema

            Dim list As IList(Of WritableT) = row(New Text("first"), New Text("second"), New Text("third"))
            assertEquals(list, transform.map(list))

            list = row(New Text("first"), New Text("second"), New Text(""))
            Dim exp As IList(Of WritableT) = row(New Text("first"), New Text("second"), New Text("second"))
            assertEquals(exp, transform.map(list))
        End Sub

        <TestMethod>
        Public Sub testSequenceDifferenceTransform()
            Dim schema As SchemaT = (New SequenceSchema.Builder()).addColumnString("firstCol").addColumnInteger("secondCol").addColumnDouble("thirdCol").build()

            Dim sequence As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            sequence.Add(row(New Text("val0"), New IntWritable(10), New DoubleWritable(10)))
            sequence.Add(row(New Text("val1"), New IntWritable(15), New DoubleWritable(15)))
            sequence.Add(row(New Text("val2"), New IntWritable(25), New DoubleWritable(25)))
            sequence.Add(row(New Text("val3"), New IntWritable(40), New DoubleWritable(40)))

            Dim t As TransformT = New SequenceDifferenceTransform("secondCol")
            t.InputSchema = schema

            Dim [out] As IList(Of IList(Of WritableT)) = t.mapSequence(sequence)

            Dim expected As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            expected.Add(row(New Text("val0"), New IntWritable(0), New DoubleWritable(10)))
            expected.Add(row(New Text("val1"), New IntWritable(15 - 10), New DoubleWritable(15)))
            expected.Add(row(New Text("val2"), New IntWritable(25 - 15), New DoubleWritable(25)))
            expected.Add(row(New Text("val3"), New IntWritable(40 - 25), New DoubleWritable(40)))

            assertEquals(expected, [out])

            t = New SequenceDifferenceTransform("thirdCol", "newThirdColName", 2, SequenceDifferenceTransform.FirstStepMode.SpecifiedValue, NullWritable.INSTANCE)
            Dim outputSchema As SchemaT = t.transform(schema)
            assertTrue(TypeOf outputSchema Is SequenceSchema)
            assertEquals(outputSchema.getColumnNames(), Arrays.asList("firstCol", "secondCol", "newThirdColName"))

            expected = New List(Of IList(Of WritableT))()
            expected.Add(row(New Text("val0"), New IntWritable(10), NullWritable.INSTANCE))
            expected.Add(row(New Text("val1"), New IntWritable(15), NullWritable.INSTANCE))
            expected.Add(row(New Text("val2"), New IntWritable(25), New DoubleWritable(25 - 10)))
            expected.Add(row(New Text("val3"), New IntWritable(40), New DoubleWritable(40 - 15)))

            ' Java built this expectation but never checked it (its implementation compared the column index, not the
            ' time step, with the lookback, so it could not pass); the port checks it.
            t.InputSchema = schema
            assertEquals(expected, t.mapSequence(sequence))
        End Sub

        <TestMethod>
        Public Sub testAddConstantColumnTransform()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnString("first").addColumnDouble("second").build()

            Dim transform As TransformT = New AddConstantColumnTransform("newCol", ColumnType.Integer, New IntWritable(10))
            transform.InputSchema = schema

            Dim [out] As SchemaT = transform.transform(schema)
            assertEquals(3, [out].numColumns())
            assertEquals(Arrays.asList("first", "second", "newCol"), [out].getColumnNames())
            assertEquals(Arrays.asList(ColumnType.String, ColumnType.Double, ColumnType.Integer), [out].getColumnTypes())

            assertEquals(row(New Text("something"), New DoubleWritable(1.0), New IntWritable(10)), transform.map(row(New Text("something"), New DoubleWritable(1.0))))
            assertEquals(row(New Text("something2"), New DoubleWritable(100.0), New IntWritable(10)), transform.map(row(New Text("something2"), New DoubleWritable(100.0))))
        End Sub

        <TestMethod>
        Public Sub testReplaceStringTransform()
            Dim schema As SchemaT = getSchema(ColumnType.String)

            ' Linked
            Dim map As IDictionary(Of String, String) = New LinkedHashMap(Of String, String)()
            map("mid") = "C2"
            map("\d") = "one"
            Dim transform As TransformT = New ReplaceStringTransform("column", map)
            transform.InputSchema = schema
            Dim [out] As SchemaT = transform.transform(schema)

            assertEquals(1, [out].getColumnMetaData().Count)
            assertEquals(ColumnType.String, [out].getMetaData(0).ColumnType)

            assertEquals(singleRow(New Text("BoneConeTone")), transform.map(singleRow(New Text("B1midT3"))))

            ' No link
            map = New Dictionary(Of String, String)()
            map("^\s+|\s+$") = ""
            transform = New ReplaceStringTransform("column", map)
            transform.InputSchema = schema
            [out] = transform.transform(schema)

            assertEquals(1, [out].getColumnMetaData().Count)
            assertEquals(ColumnType.String, [out].getMetaData(0).ColumnType)

            assertEquals(singleRow(New Text("4.25")), transform.map(singleRow(New Text("  4.25 "))))
        End Sub

        ' testReduceSequenceTransform: see transform/sequence/TestSequenceOperations.

        ' testTrimSequenceTransform: see transform/sequence/TestSequenceOperations.

        ' testSequenceTrimToLengthTransform, testSequenceTrimToLengthTransformTrimOrPad: see TestTransformProcessTransforms.

        <TestMethod>
        Public Sub testSequenceOffsetTransform()
            Dim seq As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                row(New DoubleWritable(0), New DoubleWritable(1), New DoubleWritable(2)),
                row(New DoubleWritable(3), New DoubleWritable(4), New DoubleWritable(5)),
                row(New DoubleWritable(6), New DoubleWritable(7), New DoubleWritable(8)),
                row(New DoubleWritable(9), New DoubleWritable(10), New DoubleWritable(11))}

            Dim schema As SchemaT = (New SequenceSchema.Builder()).addColumnsDouble("col%d", 0, 2).build()

            'First: test InPlace
            Dim exp1 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                row(New DoubleWritable(6), New DoubleWritable(1), New DoubleWritable(8)),
                row(New DoubleWritable(9), New DoubleWritable(4), New DoubleWritable(11))}

            Dim exp2 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                row(New DoubleWritable(0), New DoubleWritable(7), New DoubleWritable(2)),
                row(New DoubleWritable(3), New DoubleWritable(10), New DoubleWritable(5))}

            'In-place + trim
            Dim t_inplace_trim_p2 As New SequenceOffsetTransform(JavaCollections.singletonList("col1"), 2, SequenceOffsetTransform.OperationType.InPlace, SequenceOffsetTransform.EdgeHandling.TrimSequence, Nothing)
            Dim t_inplace_trim_m2 As New SequenceOffsetTransform(JavaCollections.singletonList("col1"), -2, SequenceOffsetTransform.OperationType.InPlace, SequenceOffsetTransform.EdgeHandling.TrimSequence, Nothing)
            t_inplace_trim_p2.InputSchema = schema
            t_inplace_trim_m2.InputSchema = schema

            assertEquals(exp1, t_inplace_trim_p2.mapSequence(seq))
            assertEquals(exp2, t_inplace_trim_m2.mapSequence(seq))

            'In-place + specified
            Dim t_inplace_specified_p2 As New SequenceOffsetTransform(JavaCollections.singletonList("col1"), 2, SequenceOffsetTransform.OperationType.InPlace, SequenceOffsetTransform.EdgeHandling.SpecifiedValue, NullWritable.INSTANCE)
            Dim t_inplace_specified_m2 As New SequenceOffsetTransform(JavaCollections.singletonList("col1"), -2, SequenceOffsetTransform.OperationType.InPlace, SequenceOffsetTransform.EdgeHandling.SpecifiedValue, NullWritable.INSTANCE)
            t_inplace_specified_p2.InputSchema = schema
            t_inplace_specified_m2.InputSchema = schema

            Dim exp3 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                row(New DoubleWritable(0), NullWritable.INSTANCE, New DoubleWritable(2)),
                row(New DoubleWritable(3), NullWritable.INSTANCE, New DoubleWritable(5)),
                row(New DoubleWritable(6), New DoubleWritable(1), New DoubleWritable(8)),
                row(New DoubleWritable(9), New DoubleWritable(4), New DoubleWritable(11))}
            Dim exp4 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                row(New DoubleWritable(0), New DoubleWritable(7), New DoubleWritable(2)),
                row(New DoubleWritable(3), New DoubleWritable(10), New DoubleWritable(5)),
                row(New DoubleWritable(6), NullWritable.INSTANCE, New DoubleWritable(8)),
                row(New DoubleWritable(9), NullWritable.INSTANCE, New DoubleWritable(11))}

            assertEquals(exp3, t_inplace_specified_p2.mapSequence(seq))
            assertEquals(exp4, t_inplace_specified_m2.mapSequence(seq))

            'Second: test NewColumn
            Dim exp1a As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                row(New DoubleWritable(6), New DoubleWritable(7), New DoubleWritable(1), New DoubleWritable(8)),
                row(New DoubleWritable(9), New DoubleWritable(10), New DoubleWritable(4), New DoubleWritable(11))}

            Dim exp2a As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                row(New DoubleWritable(0), New DoubleWritable(1), New DoubleWritable(7), New DoubleWritable(2)),
                row(New DoubleWritable(3), New DoubleWritable(4), New DoubleWritable(10), New DoubleWritable(5))}
            Dim t_newcol_trim_p2 As New SequenceOffsetTransform(JavaCollections.singletonList("col1"), 2, SequenceOffsetTransform.OperationType.NewColumn, SequenceOffsetTransform.EdgeHandling.TrimSequence, Nothing)
            Dim t_newcol_trim_m2 As New SequenceOffsetTransform(JavaCollections.singletonList("col1"), -2, SequenceOffsetTransform.OperationType.NewColumn, SequenceOffsetTransform.EdgeHandling.TrimSequence, Nothing)
            t_newcol_trim_p2.InputSchema = schema
            t_newcol_trim_m2.InputSchema = schema

            assertEquals(exp1a, t_newcol_trim_p2.mapSequence(seq))
            assertEquals(exp2a, t_newcol_trim_m2.mapSequence(seq))

            Dim exp3a As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                row(New DoubleWritable(0), New DoubleWritable(1), NullWritable.INSTANCE, New DoubleWritable(2)),
                row(New DoubleWritable(3), New DoubleWritable(4), NullWritable.INSTANCE, New DoubleWritable(5)),
                row(New DoubleWritable(6), New DoubleWritable(7), New DoubleWritable(1), New DoubleWritable(8)),
                row(New DoubleWritable(9), New DoubleWritable(10), New DoubleWritable(4), New DoubleWritable(11))}
            Dim exp4a As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                row(New DoubleWritable(0), New DoubleWritable(1), New DoubleWritable(7), New DoubleWritable(2)),
                row(New DoubleWritable(3), New DoubleWritable(4), New DoubleWritable(10), New DoubleWritable(5)),
                row(New DoubleWritable(6), New DoubleWritable(7), NullWritable.INSTANCE, New DoubleWritable(8)),
                row(New DoubleWritable(9), New DoubleWritable(10), NullWritable.INSTANCE, New DoubleWritable(11))}

            Dim t_newcol_specified_p2 As New SequenceOffsetTransform(JavaCollections.singletonList("col1"), 2, SequenceOffsetTransform.OperationType.NewColumn, SequenceOffsetTransform.EdgeHandling.SpecifiedValue, NullWritable.INSTANCE)
            Dim t_newcol_specified_m2 As New SequenceOffsetTransform(JavaCollections.singletonList("col1"), -2, SequenceOffsetTransform.OperationType.NewColumn, SequenceOffsetTransform.EdgeHandling.SpecifiedValue, NullWritable.INSTANCE)
            t_newcol_specified_p2.InputSchema = schema
            t_newcol_specified_m2.InputSchema = schema

            assertEquals(exp3a, t_newcol_specified_p2.mapSequence(seq))
            assertEquals(exp4a, t_newcol_specified_m2.mapSequence(seq))

            'Finally: check edge case
            assertEquals(New List(Of Object)(), t_inplace_trim_p2.mapSequence(exp1))
            assertEquals(New List(Of Object)(), t_inplace_trim_m2.mapSequence(exp1))
            assertEquals(New List(Of Object)(), t_newcol_trim_p2.mapSequence(exp1))
            assertEquals(New List(Of Object)(), t_newcol_trim_m2.mapSequence(exp1))
        End Sub

        ' testStringListToCountsNDArrayTransform, testStringListToIndicesNDArrayTransform: not ported (need INDArray).

        ' testFirstDigitTransform: see TestTransformProcessTransforms.

    End Class

End Namespace
