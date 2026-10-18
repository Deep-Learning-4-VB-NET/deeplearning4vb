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
Imports ColumnTypeT = DeepLearning4VB.org.datavec.api.transform.ColumnType
Imports ConditionOpT = DeepLearning4VB.org.datavec.api.transform.condition.ConditionOp
Imports DataActionT = DeepLearning4VB.org.datavec.api.transform.DataAction
Imports DoubleAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.columns.DoubleAnalysis
Imports ColumnAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.columns.ColumnAnalysis
Imports DataAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.DataAnalysis
Imports IntegerColumnConditionT = DeepLearning4VB.org.datavec.api.transform.condition.column.IntegerColumnCondition
Imports ListStringRecordReaderT = DeepLearning4VB.org.datavec.api.records.reader.impl.collection.ListStringRecordReader
Imports ListStringSplitT = DeepLearning4VB.org.datavec.api.split.ListStringSplit
Imports MathOpT = DeepLearning4VB.org.datavec.api.transform.MathOp
Imports NumericalColumnComparatorT = DeepLearning4VB.org.datavec.api.transform.sequence.comparator.NumericalColumnComparator
Imports MinMaxNormalizerT = DeepLearning4VB.org.datavec.api.transform.transforms.doubletransform.MinMaxNormalizer
Imports NormalizeT = DeepLearning4VB.org.datavec.api.transform.transforms.normalize.Normalize
Imports RecordReaderT = DeepLearning4VB.org.datavec.api.records.reader.RecordReader
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports SequenceSchemaT = DeepLearning4VB.org.datavec.api.transform.schema.SequenceSchema
Imports StandardizeNormalizerT = DeepLearning4VB.org.datavec.api.transform.transforms.doubletransform.StandardizeNormalizer
Imports TextToCharacterIndexTransformT = DeepLearning4VB.org.datavec.api.transform.transforms.nlp.TextToCharacterIndexTransform
Imports TransformProcessT = DeepLearning4VB.org.datavec.api.transform.TransformProcess
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform

    ''' <summary>
    ''' Port of the DataVec <c>TestTransformProcess</c> JUnit class, plus tests of the other
    ''' <see cref="TransformProcessT"/> members (schemas after each step, raw string input, normalization from an
    ''' analysis, sequence to single record, equality).
    ''' </summary>
    <TestClass>
    Public Class TestTransformProcess

        <TestMethod>
        Public Sub testExecution()

            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnsString("col").addColumnsDouble("col2").build()

            Dim m As IDictionary(Of Char, Integer) = defaultCharIndex()
            Dim transformProcess As TransformProcessT = (New TransformProcessT.Builder(schema)).doubleMathOp("col2", MathOpT.Add, 1.0).build()

            Dim [in] As IList(Of WritableT) = New List(Of WritableT) From {New Text("Text"), New DoubleWritable(2.0)}
            Dim exp As IList(Of WritableT) = New List(Of WritableT) From {New Text("Text"), New DoubleWritable(3.0)}

            Dim [out] As IList(Of WritableT) = transformProcess.execute([in])
            assertEquals(exp, [out])
        End Sub

        <TestMethod>
        Public Sub testExecuteToSequence()

            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnsString("action").build()

            Dim m As IDictionary(Of Char, Integer) = defaultCharIndex()
            Dim transformProcess As TransformProcessT = (New TransformProcessT.Builder(schema)).removeAllColumnsExceptFor("action").convertToSequence().transform(New TextToCharacterIndexTransformT("action", "action_sequence", m, True)).build()

            Dim s As String = "in text"
            Dim input As IList(Of WritableT) = New List(Of WritableT) From {New Text(s)}

            Dim expSeq As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))(s.Length)
            For i As Integer = 0 To s.Length - 1
                expSeq.Add(New List(Of WritableT) From {New IntWritable(m(s.Chars(i)))})
            Next i

            Dim [out] As IList(Of IList(Of WritableT)) = transformProcess.executeToSequence(input)

            assertEquals(expSeq, [out])

            ' the batch version: one sequence per record
            Dim batch As IList(Of IList(Of IList(Of WritableT))) = transformProcess.executeToSequenceBatch(New List(Of IList(Of WritableT)) From {input, input})
            assertEquals(2, batch.Count)
            assertEquals(expSeq, batch(1))
        End Sub

        <TestMethod>
        Public Sub testInferColumns()
            Dim categories As IList(Of IList(Of String)) = New List(Of IList(Of String)) From {
                New List(Of String) From {"a", "d"},
                New List(Of String) From {"b", "e"},
                New List(Of String) From {"c", "f"}
            }

            Dim listReader As RecordReaderT = New ListStringRecordReaderT()
            listReader.initialize(New ListStringSplitT(categories))
            Dim inferredSingle As IList(Of String) = TransformProcessT.inferCategories(listReader, 0)
            assertEquals(3, inferredSingle.Count)
            listReader.initialize(New ListStringSplitT(categories))
            Dim integerListMap As IDictionary(Of Integer, IList(Of String)) = TransformProcessT.inferCategories(listReader, New Integer() {0, 1})
            For i As Integer = 0 To 1
                assertEquals(3, integerListMap(i).Count)
            Next i
        End Sub

        <TestMethod>
        Public Sub testInferCategoriesSortedOrdinal()
            Dim data As IList(Of IList(Of String)) = New List(Of IList(Of String)) From {
                New List(Of String) From {"b"},
                New List(Of String) From {"a"},
                New List(Of String) From {"B"},
                New List(Of String) From {"b"}
            }
            Dim reader As RecordReaderT = New ListStringRecordReaderT()
            reader.initialize(New ListStringSplitT(data))
            ' Java String.compareTo order: upper case before lower case
            assertEquals(New List(Of String) From {"B", "a", "b"}, TransformProcessT.inferCategories(reader, 0))
            reader.initialize(New ListStringSplitT(data))
            assertEquals(0, TransformProcessT.inferCategories(reader, New Integer() {}).Count)
        End Sub

        <TestMethod>
        Public Sub testSchemasAfterSteps()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnInteger("a").addColumnDouble("b").addColumnString("c").build()
            Dim tp As TransformProcessT = (New TransformProcessT.Builder(schema)).removeColumns("c").filter(New IntegerColumnConditionT("a", ConditionOpT.LessThan, 0)).renameColumn("b", "bb").integerMathOp("a", MathOpT.Add, 1).build()

            assertEquals(schema, tp.getInitialSchema())
            assertEquals(4, tp.getActionList().Count)
            assertEquals(New List(Of String) From {"a", "b"}, tp.getSchemaAfterStep(0).getColumnNames())
            ' as in Java, a filter is a step of its own
            assertEquals(New List(Of String) From {"a", "bb"}, tp.getSchemaAfterStep(2).getColumnNames())
            assertEquals(New List(Of String) From {"a", "bb"}, tp.FinalSchema.getColumnNames())
            assertEquals(tp.FinalSchema, tp.getFinalSchema())
            ' the input schema of every step was set
            assertEquals(New List(Of String) From {"a", "b"}, tp.getActionList()(1).getSchema().getColumnNames())

            Assert.IsNull(tp.execute(New List(Of WritableT) From {New IntWritable(-1), New DoubleWritable(1), New Text("x")}))
            assertEquals(New List(Of WritableT) From {New IntWritable(2), New DoubleWritable(1)}, tp.execute(New List(Of WritableT) From {New IntWritable(1), New DoubleWritable(1), New Text("x")}))
        End Sub

        <TestMethod>
        Public Sub testTransformRawStringsToInput()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnString("s").addColumnInteger("i").addColumnDouble("d").addColumnFloat("f").addColumnCategorical("c", "x", "y").addColumnBoolean("b").addColumnLong("l").build()
            Dim tp As TransformProcessT = (New TransformProcessT.Builder(schema)).build()

            Dim expected As IList(Of WritableT) = New List(Of WritableT) From {New Text("abc"), New IntWritable(5), New DoubleWritable(1.5), New FloatWritable(2.5F), New Text("y"), New BooleanWritable(True), New LongWritable(7L)}
            assertEquals(expected, tp.transformRawStringsToInput("abc", "5", "1.5", "2.5", "y", "TRUE", "7"))
            assertEquals(New List(Of IList(Of WritableT)) From {expected}, tp.transformRawStringsToInputSequence(New List(Of IList(Of String)) From {New List(Of String) From {"abc", "5", "1.5", "2.5", "y", "true", "7"}}))
            assertThrows(Of ArgumentException)(Sub() tp.transformRawStringsToInput("too", "few"))
        End Sub

        <TestMethod>
        Public Sub testNormalizeFromAnalysis()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnDouble("d").build()
            Dim da As New DataAnalysisT(schema, New List(Of ColumnAnalysisT) From {(New DoubleAnalysisT.Builder()).min(1.0).max(5.0).mean(3.0).sampleStdev(2.0).build()})

            Dim tp As TransformProcessT = (New TransformProcessT.Builder(schema)).normalize("d", NormalizeT.MinMax, da).normalize("d", NormalizeT.Standardize, da).build()
            assertEquals(New MinMaxNormalizerT("d", 1.0, 5.0), tp.getActionList()(0).getTransform())
            assertEquals(New StandardizeNormalizerT("d", 3.0, 2.0), tp.getActionList()(1).getTransform())
            ' min-max: (3 - 1) / (5 - 1) = 0.5; standardize: (0.5 - 3) / 2 = -1.25
            assertEquals(New List(Of WritableT) From {New DoubleWritable(-1.25)}, tp.execute(New List(Of WritableT) From {New DoubleWritable(3.0)}))
        End Sub

        <TestMethod>
        Public Sub testExecuteSequence()
            Dim schema As SchemaT = (New SequenceSchemaT.Builder()).addColumnInteger("a").addColumnDouble("b").build()
            Dim tp As TransformProcessT = (New TransformProcessT.Builder(schema)).integerMathOp("a", MathOpT.Multiply, 10).build()
            Dim seq As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                New List(Of WritableT) From {New IntWritable(1), New DoubleWritable(0.5)},
                New List(Of WritableT) From {New IntWritable(2), New DoubleWritable(1.5)}
            }
            Dim exp As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                New List(Of WritableT) From {New IntWritable(10), New DoubleWritable(0.5)},
                New List(Of WritableT) From {New IntWritable(20), New DoubleWritable(1.5)}
            }
            assertEquals(exp, tp.executeSequence(seq))
            assertEquals(exp, tp.executeSequenceToSequence(seq))
        End Sub

        <TestMethod>
        Public Sub testUnsupportedSingleRecordOperations()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnInteger("a").build()
            Dim tp As TransformProcessT = (New TransformProcessT.Builder(schema)).convertToSequence("a", New NumericalColumnComparatorT("a")).build()
            assertThrows(Of InvalidOperationException)(Sub() tp.execute(New List(Of WritableT) From {New IntWritable(1)}))
            assertThrows(Of InvalidOperationException)(Sub() tp.executeToSequence(New List(Of WritableT) From {New IntWritable(1)}))
            ' converting from a non-sequence schema fails when the process is built
            assertThrows(Of InvalidOperationException)(Sub() Call (New TransformProcessT.Builder(schema)).convertFromSequence().build())
        End Sub

        <TestMethod>
        Public Sub testEqualsHashCodeToString()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnInteger("a").build()
            Dim tp1 As TransformProcessT = (New TransformProcessT.Builder(schema)).integerMathOp("a", MathOpT.Add, 1).build()
            Dim tp2 As TransformProcessT = (New TransformProcessT.Builder(schema)).integerMathOp("a", MathOpT.Add, 1).build()
            Dim tp3 As TransformProcessT = (New TransformProcessT.Builder(schema)).integerMathOp("a", MathOpT.Add, 2).build()
            assertEquals(tp1, tp2)
            assertEquals(tp1.GetHashCode(), tp2.GetHashCode())
            assertNotEquals(tp1, tp3)
            assertTrue(tp1.ToString().StartsWith("TransformProcess(initialSchema="), tp1.ToString())
            assertTrue(tp1.ToString().Contains("actionList=[DataAction("), tp1.ToString())

            Dim da As DataActionT = tp1.getActionList()(0)
            assertEquals(New DataActionT(da.getTransform()), da)
            assertThrows(Of InvalidOperationException)(Sub() Call New DataActionT().ToString())
        End Sub

        Public Shared Function defaultCharIndex() As IDictionary(Of Char, Integer)
            Dim ret As IDictionary(Of Char, Integer) = New SortedDictionary(Of Char, Integer)()

            ret("a"c) = 0
            ret("b"c) = 1
            ret("c"c) = 2
            ret("d"c) = 3
            ret("e"c) = 4
            ret("f"c) = 5
            ret("g"c) = 6
            ret("h"c) = 7
            ret("i"c) = 8
            ret("j"c) = 9
            ret("k"c) = 10
            ret("l"c) = 11
            ret("m"c) = 12
            ret("n"c) = 13
            ret("o"c) = 14
            ret("p"c) = 15
            ret("q"c) = 16
            ret("r"c) = 17
            ret("s"c) = 18
            ret("t"c) = 19
            ret("u"c) = 20
            ret("v"c) = 21
            ret("w"c) = 22
            ret("x"c) = 23
            ret("y"c) = 24
            ret("z"c) = 25
            ret("/"c) = 26
            ret(" "c) = 27
            ret("("c) = 28
            ret(")"c) = 29

            Return ret
        End Function

    End Class

End Namespace
