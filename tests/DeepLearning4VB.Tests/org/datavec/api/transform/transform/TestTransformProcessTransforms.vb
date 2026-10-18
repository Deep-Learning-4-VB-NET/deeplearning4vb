'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
'

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.datavec.api.writable
Imports ChangeCaseStringTransformT = DeepLearning4VB.org.datavec.api.transform.transforms.string.ChangeCaseStringTransform
Imports ColumnTypeT = DeepLearning4VB.org.datavec.api.transform.ColumnType
Imports ConcatenateStringColumnsT = DeepLearning4VB.org.datavec.api.transform.transforms.string.ConcatenateStringColumns
Imports FirstDigitTransformT = DeepLearning4VB.org.datavec.api.transform.transforms.categorical.FirstDigitTransform
Imports IntegerMetaDataT = DeepLearning4VB.org.datavec.api.transform.metadata.IntegerMetaData
Imports JsonMappersT = DeepLearning4VB.org.datavec.api.transform.serde.JsonMappers
Imports ReduceOpT = DeepLearning4VB.org.datavec.api.transform.ReduceOp
Imports ReducerT = DeepLearning4VB.org.datavec.api.transform.reduce.Reducer
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports SequenceMovingWindowReduceTransformT = DeepLearning4VB.org.datavec.api.transform.transforms.sequence.SequenceMovingWindowReduceTransform
Imports SequenceSchemaT = DeepLearning4VB.org.datavec.api.transform.schema.SequenceSchema
Imports TextToCharacterIndexTransformT = DeepLearning4VB.org.datavec.api.transform.transforms.nlp.TextToCharacterIndexTransform
Imports TextToTermIndexSequenceTransformT = DeepLearning4VB.org.datavec.api.transform.transforms.nlp.TextToTermIndexSequenceTransform
Imports TransformProcessT = DeepLearning4VB.org.datavec.api.transform.TransformProcess
Imports TransformT = DeepLearning4VB.org.datavec.api.transform.Transform
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms

    ''' <summary>
    ''' The parts of the DataVec <c>TestTransforms</c> JUnit class that need <see cref="TransformProcessT"/>, the JSON
    ''' serde or the transforms ported with them (<c>testSequenceMovingWindowReduceTransform</c>,
    ''' <c>testSequenceTrimToLengthTransform</c>, <c>testSequenceTrimToLengthTransformTrimOrPad</c>,
    ''' <c>testTextToCharacterIndexTransform</c>, <c>testTextToTermIndexSequenceTransform</c>,
    ''' <c>testFirstDigitTransform</c> and the JSON round trips of <c>testConcatenateStringColumnsTransform</c> and
    ''' <c>testChangeCaseStringTransform</c>); the rest is in <see cref="TestTransforms"/>. Plus tests of the other
    ''' members of the three new transforms.
    ''' </summary>
    <TestClass>
    Public Class TestTransformProcessTransforms

        Private Shared Function row(ByVal ParamArray values() As WritableT) As IList(Of WritableT)
            Return New List(Of WritableT)(values)
        End Function

        Private Shared Function doubles(ByVal ParamArray values() As Double) As IList(Of WritableT)
            Dim l As New List(Of WritableT)()
            For Each v As Double In values
                l.Add(New DoubleWritable(v))
            Next
            Return l
        End Function

        <TestMethod>
        Public Sub testConcatenateStringColumnsJson()
            Dim transform As TransformT = New ConcatenateStringColumnsT("NewColumn", " ", New List(Of String) From {"ConcatenatedColumn1", "ConcatenatedColumn2", "ConcatenatedColumn3"})
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnsString("ConcatenatedColumn1", "OtherColumn4", "ConcatenatedColumn2", "OtherColumn5", "ConcatenatedColumn3", "OtherColumn6").build()
            transform.InputSchema = schema

            Dim s As String = JsonMappersT.writeValueAsString(transform)
            Dim transform2 As TransformT = DirectCast(JsonMappersT.readValue(s, GetType(ConcatenateStringColumnsT)), TransformT)
            assertEquals(transform, transform2)
        End Sub

        <TestMethod>
        Public Sub testChangeCaseStringJson()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnsString("StringColumn", "OtherColumn").build()
            Dim transform As TransformT = New ChangeCaseStringTransformT("StringColumn", ChangeCaseStringTransformT.CaseType.UPPER)
            transform.InputSchema = schema

            Dim s As String = JsonMappersT.writeValueAsString(transform)
            Dim transform2 As TransformT = DirectCast(JsonMappersT.readValue(s, GetType(ChangeCaseStringTransformT)), TransformT)
            assertEquals(transform, transform2)
            transform2.InputSchema = schema
            assertEquals(row(New Text("ABC"), New Text("def")), transform2.map(row(New Text("aBc"), New Text("def"))))
        End Sub

        <TestMethod>
        Public Sub testSequenceMovingWindowReduceTransform()
            Dim seq As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {doubles(0, 1, 2), doubles(3, 4, 5), doubles(6, 7, 8), doubles(9, 10, 11)}

            Dim exp1 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {doubles(0, 1, 2, 2), doubles(3, 4, 5, (2 + 5) / 2.0), doubles(6, 7, 8, (2 + 5 + 8) / 3.0), doubles(9, 10, 11, (5 + 8 + 11) / 3.0)}

            Dim exp2 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                row(New DoubleWritable(0), New DoubleWritable(1), New DoubleWritable(2), NullWritable.INSTANCE),
                row(New DoubleWritable(3), New DoubleWritable(4), New DoubleWritable(5), NullWritable.INSTANCE),
                doubles(6, 7, 8, (2 + 5 + 8) / 3.0),
                doubles(9, 10, 11, (5 + 8 + 11) / 3.0)}

            Dim schema As SchemaT = (New SequenceSchemaT.Builder()).addColumnsDouble("col%d", 0, 2).build()
            Dim expOutSchema1 As SchemaT = (New SequenceSchemaT.Builder()).addColumnsDouble("col%d", 0, 2).addColumnDouble("mean(3,col2)").build()
            Dim expOutSchema2 As SchemaT = (New SequenceSchemaT.Builder()).addColumnsDouble("col%d", 0, 2).addColumnDouble("newCol").build()

            Dim t1 As New SequenceMovingWindowReduceTransformT("col2", 3, ReduceOpT.Mean)
            Dim t2 As New SequenceMovingWindowReduceTransformT("col2", "newCol", 3, ReduceOpT.Mean, SequenceMovingWindowReduceTransformT.EdgeCaseHandling.SpecifiedValue, NullWritable.INSTANCE)

            t1.InputSchema = schema
            assertEquals(expOutSchema1, t1.transform(schema))

            t2.InputSchema = schema
            assertEquals(expOutSchema2, t2.transform(schema))

            Dim act1 As IList(Of IList(Of WritableT)) = t1.mapSequence(seq)
            Dim act2 As IList(Of IList(Of WritableT)) = t2.mapSequence(seq)

            assertEquals(exp1, act1)
            assertEquals(exp2, act2)
        End Sub

        <TestMethod>
        Public Sub testSequenceMovingWindowReduceTransformOtherMembers()
            Dim schema As SchemaT = (New SequenceSchemaT.Builder()).addColumnInteger("i").addColumnsDouble("d").build()
            Dim tp As TransformProcessT = (New TransformProcessT.Builder(schema)).sequenceMovingWindowReduce("i", 2, ReduceOpT.Max).sequenceMovingWindowReduce("d", 2, ReduceOpT.Count).build()
            ' Max keeps the column type, Count gives an integer column
            assertEquals(New List(Of String) From {"i", "d", "max(2,i)", "count(2,d)"}, tp.FinalSchema.getColumnNames())
            assertEquals(New List(Of ColumnTypeT) From {ColumnTypeT.Integer, ColumnTypeT.Double, ColumnTypeT.Integer, ColumnTypeT.Integer}, tp.FinalSchema.getColumnTypes())
            assertTrue(TypeOf tp.FinalSchema Is SequenceSchemaT)

            Dim seq As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {row(New IntWritable(5), New DoubleWritable(1)), row(New IntWritable(3), New DoubleWritable(2)), row(New IntWritable(4), New DoubleWritable(3))}
            Dim out1 As IList(Of IList(Of WritableT)) = tp.executeSequence(seq)
            assertEquals(row(New IntWritable(5), New DoubleWritable(1), New IntWritable(5), New IntWritable(1)), out1(0))
            assertEquals(row(New IntWritable(3), New DoubleWritable(2), New IntWritable(5), New IntWritable(2)), out1(1))
            assertEquals(row(New IntWritable(4), New DoubleWritable(3), New IntWritable(4), New IntWritable(2)), out1(2))

            Dim tp2 As TransformProcessT = TransformProcessT.fromJson(tp.toJson())
            assertEquals(tp, tp2)
            assertEquals(out1, tp2.executeSequence(seq))

            Dim t As SequenceMovingWindowReduceTransformT = DirectCast(tp.getActionList()(0).getTransform(), SequenceMovingWindowReduceTransformT)
            assertEquals("SequenceMovingWindowReduceTransform(columnName=""i"",newColumnName=""max(2,i)"",lookback=2,op=Max,edgeCaseHandling=Default)", t.ToString())
            assertEquals(New String() {"i", "d"}, t.columnNames())
            assertEquals("i", t.outputColumnName())
            assertThrows(Of NotSupportedException)(Sub() t.map(row(New IntWritable(1))))
            assertThrows(Of NotSupportedException)(Sub() t.map(CObj(1)))
        End Sub

        <TestMethod>
        Public Sub testSequenceTrimToLengthTransform()
            Dim seq As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {doubles(0, 1, 2), doubles(3, 4, 5), doubles(6, 7, 8), doubles(9, 10, 11)}
            Dim expTrimLength3 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {doubles(0, 1, 2), doubles(3, 4, 5), doubles(6, 7, 8)}

            Dim s As SchemaT = (New SchemaT.Builder()).addColumnsDouble("first", "second", "third").build()

            Dim p As TransformProcessT = (New TransformProcessT.Builder(s)).trimSequenceToLength(3).build()

            Dim [out] As IList(Of IList(Of WritableT)) = p.executeSequence(seq)
            assertEquals(expTrimLength3, [out])

            Dim seq2 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {doubles(0, 1, 2), doubles(3, 4, 5)}

            [out] = p.executeSequence(seq2)
            assertEquals(seq2, [out])

            Dim json As String = p.toJson()
            Dim tp2 As TransformProcessT = TransformProcessT.fromJson(json)
            assertEquals(expTrimLength3, tp2.executeSequence(seq))
            assertEquals(seq2, tp2.executeSequence(seq2))
        End Sub

        <TestMethod>
        Public Sub testSequenceTrimToLengthTransformTrimOrPad()
            Dim seq As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {doubles(0, 1, 2), doubles(3, 4, 5), doubles(6, 7, 8), doubles(9, 10, 11), doubles(12, 13, 14)}
            Dim seq2 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {doubles(0, 1, 2), doubles(3, 4, 5)}
            Dim expTrimLength4 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {doubles(0, 1, 2), doubles(3, 4, 5), doubles(6, 7, 8), doubles(9, 10, 11)}

            Dim s As SchemaT = (New SchemaT.Builder()).addColumnsDouble("first", "second", "third").build()

            Dim p As TransformProcessT = (New TransformProcessT.Builder(s)).trimOrPadSequenceToLength(4, doubles(900, 901, 902)).build()

            Dim [out] As IList(Of IList(Of WritableT)) = p.executeSequence(seq)
            assertEquals(expTrimLength4, [out])

            Dim exp2 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {doubles(0, 1, 2), doubles(3, 4, 5), doubles(900, 901, 902), doubles(900, 901, 902)}

            [out] = p.executeSequence(seq2)
            assertEquals(exp2, [out])

            Dim json As String = p.toJson()
            Dim tp2 As TransformProcessT = TransformProcessT.fromJson(json)
            assertEquals(expTrimLength4, tp2.executeSequence(seq))
            assertEquals(exp2, tp2.executeSequence(seq2))
            assertThrows(Of ArgumentNullException)(Sub() Call (New TransformProcessT.Builder(s)).trimOrPadSequenceToLength(4, Nothing))
        End Sub

        <TestMethod>
        Public Sub testTextToCharacterIndexTransform()

            Dim s As SchemaT = (New SchemaT.Builder()).addColumnString("col").addColumnDouble("d").build()

            Dim inSeq As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {row(New Text("text"), New DoubleWritable(1.0)), row(New Text("ab"), New DoubleWritable(2.0))}

            Dim map As IDictionary(Of Char, Integer) = New Dictionary(Of Char, Integer)()
            map("a"c) = 0
            map("b"c) = 1
            map("e"c) = 2
            map("t"c) = 3
            map("x"c) = 4

            Dim exp As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {row(New IntWritable(3), New DoubleWritable(1.0)), row(New IntWritable(2), New DoubleWritable(1.0)), row(New IntWritable(4), New DoubleWritable(1.0)), row(New IntWritable(3), New DoubleWritable(1.0)), row(New IntWritable(0), New DoubleWritable(2.0)), row(New IntWritable(1), New DoubleWritable(2.0))}

            Dim t As TransformT = New TextToCharacterIndexTransformT("col", "newName", map, False)
            t.InputSchema = s

            Dim outputSchema As SchemaT = t.transform(s)
            assertEquals(2, outputSchema.getColumnNames().Count)
            assertEquals(ColumnTypeT.Integer, outputSchema.getType(0))
            assertEquals(ColumnTypeT.Double, outputSchema.getType(1))

            Dim intMetadata As IntegerMetaDataT = DirectCast(outputSchema.getMetaData(0), IntegerMetaDataT)
            assertEquals(0, intMetadata.getMinAllowedValue().Value)
            assertEquals(4, intMetadata.getMaxAllowedValue().Value)

            Dim [out] As IList(Of IList(Of WritableT)) = t.mapSequence(inSeq)
            assertEquals(exp, [out])
        End Sub

        <TestMethod>
        Public Sub testTextToCharacterIndexTransformOtherMembers()
            Dim s As SchemaT = (New SchemaT.Builder()).addColumnString("col").build()
            Dim map As IDictionary(Of Char, Integer) = New Dictionary(Of Char, Integer) From {{"a"c, 0}, {"b"c, 1}}

            ' unknown characters: skipped, or an exception
            Dim skip As New TextToCharacterIndexTransformT("col", "idx", map, False)
            skip.InputSchema = s
            assertEquals(New List(Of IList(Of WritableT)) From {row(New IntWritable(1)), row(New IntWritable(0))}, skip.mapSequence(New List(Of IList(Of WritableT)) From {row(New Text("bza"))}))
            Dim strict As New TextToCharacterIndexTransformT("col", "idx", map, True)
            strict.InputSchema = s
            assertThrows(Of InvalidOperationException)(Sub() strict.mapSequence(New List(Of IList(Of WritableT)) From {row(New Text("bza"))}))

            ' JSON: the base class fields carry the column names (as in Java, "columnName" is only a creator property)
            Dim tp As TransformProcessT = (New TransformProcessT.Builder(s)).convertToSequence().transform(strict).build()
            Dim json As String = tp.toJson()
            assertTrue(json.Contains("""requiredColumns"" : [ ""col"" ]"), json)
            assertTrue(json.Contains("""characterIndexMap"" : {"), json)
            Dim tp2 As TransformProcessT = TransformProcessT.fromJson(json)
            assertEquals(tp, tp2)
            Dim strict2 As TextToCharacterIndexTransformT = DirectCast(tp2.getActionList()(1).getTransform(), TextToCharacterIndexTransformT)
            assertTrue(strict2.isExceptionOnUnknown())
            assertEquals(New List(Of String) From {"idx"}, strict2.getExpandedColumnNames())
            assertEquals(New List(Of IList(Of WritableT)) From {row(New IntWritable(0)), row(New IntWritable(1))}, tp2.executeToSequence(row(New Text("ab"))))

            assertNotEquals(skip, strict)
            assertEquals(strict, New TextToCharacterIndexTransformT("col", "idx", New Dictionary(Of Char, Integer) From {{"b"c, 1}, {"a"c, 0}}, True))
            assertEquals(strict.GetHashCode(), New TextToCharacterIndexTransformT("col", "idx", New Dictionary(Of Char, Integer) From {{"b"c, 1}, {"a"c, 0}}, True).GetHashCode())
        End Sub

        <TestMethod>
        Public Sub testTextToTermIndexSequenceTransform()

            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnString("ID").addColumnString("TEXT").addColumnDouble("FEATURE").build()
            Dim vocab As IList(Of String) = New List(Of String) From {"zero", "one", "two", "three"}
            Dim inSeq As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {row(New Text("a"), New Text("zero four two"), New DoubleWritable(4.2)), row(New Text("b"), New Text("six one two four three five"), New DoubleWritable(87.9))}

            Dim expSchema As SchemaT = (New SchemaT.Builder()).addColumnString("ID").addColumnInteger("INDEXSEQ", 0, 3).addColumnDouble("FEATURE").build()
            Dim exp As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {row(New Text("a"), New IntWritable(0), New DoubleWritable(4.2)), row(New Text("a"), New IntWritable(2), New DoubleWritable(4.2)), row(New Text("b"), New IntWritable(1), New DoubleWritable(87.9)), row(New Text("b"), New IntWritable(2), New DoubleWritable(87.9)), row(New Text("b"), New IntWritable(3), New DoubleWritable(87.9))}

            Dim t As TransformT = New TextToTermIndexSequenceTransformT("TEXT", "INDEXSEQ", vocab, " ", False)
            t.InputSchema = schema

            Dim outputSchema As SchemaT = t.transform(schema)
            assertEquals(expSchema.getColumnNames(), outputSchema.getColumnNames())
            assertEquals(expSchema.getColumnTypes(), outputSchema.getColumnTypes())
            assertEquals(expSchema, outputSchema)

            assertEquals(3, outputSchema.getColumnNames().Count)
            assertEquals(ColumnTypeT.String, outputSchema.getType(0))
            assertEquals(ColumnTypeT.Integer, outputSchema.getType(1))
            assertEquals(ColumnTypeT.Double, outputSchema.getType(2))

            Dim intMetadata As IntegerMetaDataT = DirectCast(outputSchema.getMetaData(1), IntegerMetaDataT)
            assertEquals(0, intMetadata.getMinAllowedValue().Value)
            assertEquals(3, intMetadata.getMaxAllowedValue().Value)

            Dim [out] As IList(Of IList(Of WritableT)) = t.mapSequence(inSeq)
            assertEquals(exp, [out])

            Dim tp As TransformProcessT = (New TransformProcessT.Builder(schema)).transform(t).build()
            Dim json As String = tp.toJson()
            Dim tp2 As TransformProcessT = TransformProcessT.fromJson(json)
            assertEquals(tp, tp2)
        End Sub

        <TestMethod>
        Public Sub testTextToTermIndexSequenceTransformOtherMembers()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnString("TEXT").build()
            Dim t As New TextToTermIndexSequenceTransformT("TEXT", "IDX", New Dictionary(Of String, Integer) From {{"a", 0}, {"b", 1}}, ",\s*", True)
            t.InputSchema = schema
            ' the delimiter is a regular expression, as in Java's String.split
            assertEquals(New List(Of IList(Of WritableT)) From {row(New IntWritable(1)), row(New IntWritable(0)), row(New IntWritable(1))}, t.mapSequence(New List(Of IList(Of WritableT)) From {row(New Text("b, a,b"))}))
            assertThrows(Of InvalidOperationException)(Sub() t.mapSequence(New List(Of IList(Of WritableT)) From {row(New Text("a,c"))}))
            assertEquals(",\s*", t.getDelimiter())
            assertEquals(2, t.getWordIndexMap().Count)

            ' Java: the class's own @JsonIgnoreProperties({"writableMap"}) replaces the base class list, so the input
            ' schema is written as well
            Dim json As String = JsonMappersT.writeValueAsString(t)
            assertTrue(json.Contains("""inputSchema"" : {"), json)
            assertTrue(json.Contains("""wordIndexMap"" : {"), json)
            Dim back As TextToTermIndexSequenceTransformT = DirectCast(JsonMappersT.readValue(json, GetType(TransformT)), TextToTermIndexSequenceTransformT)
            assertEquals(t, back)
            assertEquals(schema, back.InputSchema)
        End Sub

        <TestMethod>
        Public Sub testFirstDigitTransform()
            Dim s As SchemaT = (New SchemaT.Builder()).addColumnString("data").addColumnDouble("double").addColumnString("stringNumber").build()

            Dim tp As TransformProcessT = (New TransformProcessT.Builder(s)).firstDigitTransform("double", "fdDouble", FirstDigitTransformT.Mode.EXCEPTION_ON_INVALID).firstDigitTransform("stringNumber", "stringNumber", FirstDigitTransformT.Mode.INCLUDE_OTHER_CATEGORY).build()

            Dim s2 As SchemaT = tp.FinalSchema
            assertEquals(New List(Of String) From {"data", "double", "fdDouble", "stringNumber"}, s2.getColumnNames())

            assertEquals(New List(Of ColumnTypeT) From {ColumnTypeT.String, ColumnTypeT.Double, ColumnTypeT.Categorical, ColumnTypeT.Categorical}, s2.getColumnTypes())

            Dim [in] As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {row(New Text("a"), New DoubleWritable(3.14159), New Text("8e-4")), row(New Text("b"), New DoubleWritable(2.71828), New Text("7e2")), row(New Text("c"), New DoubleWritable(1.61803), New Text("6e8")), row(New Text("c"), New DoubleWritable(-2), New Text("non numerical"))}

            Dim expected As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {row(New Text("a"), New DoubleWritable(3.14159), New Text("3"), New Text("8")), row(New Text("b"), New DoubleWritable(2.71828), New Text("2"), New Text("7")), row(New Text("c"), New DoubleWritable(1.61803), New Text("1"), New Text("6")), row(New Text("c"), New DoubleWritable(-2), New Text("2"), New Text("Other"))}

            Dim [out] As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            For Each i As IList(Of WritableT) In [in]
                [out].Add(tp.execute(i))
            Next i
            assertEquals(expected, [out])

            'Test Benfords law use case:
            Dim tp2 As TransformProcessT = (New TransformProcessT.Builder(s)).firstDigitTransform("double", "fdDouble", FirstDigitTransformT.Mode.EXCEPTION_ON_INVALID).firstDigitTransform("stringNumber", "stringNumber", FirstDigitTransformT.Mode.INCLUDE_OTHER_CATEGORY).removeColumns("data", "double").categoricalToOneHot("fdDouble", "stringNumber").reduce((New ReducerT.Builder(ReduceOpT.Sum)).build()).build()
            assertEquals(21, tp2.FinalSchema.numColumns())
            assertEquals(tp2, TransformProcessT.fromJson(tp2.toJson()))
        End Sub

    End Class

End Namespace
