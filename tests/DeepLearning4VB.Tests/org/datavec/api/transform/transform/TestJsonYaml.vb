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
Imports DeepLearning4VB.JavaCompat
Imports DeepLearning4VB.org.datavec.api.writable
Imports CategoricalAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.columns.CategoricalAnalysis
Imports ColumnAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.columns.ColumnAnalysis
Imports ColumnTypeT = DeepLearning4VB.org.datavec.api.transform.ColumnType
Imports ConditionOpT = DeepLearning4VB.org.datavec.api.transform.condition.ConditionOp
Imports ConditionFilterT = DeepLearning4VB.org.datavec.api.transform.filter.ConditionFilter
Imports DataActionT = DeepLearning4VB.org.datavec.api.transform.DataAction
Imports DataAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.DataAnalysis
Imports DateTimeFieldType = DeepLearning4VB.org.joda.time.DateTimeFieldType
Imports DateTimeZone = DeepLearning4VB.org.joda.time.DateTimeZone
Imports DeriveColumnsFromTimeTransformT = DeepLearning4VB.org.datavec.api.transform.transforms.time.DeriveColumnsFromTimeTransform
Imports DoubleAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.columns.DoubleAnalysis
Imports DoubleColumnConditionT = DeepLearning4VB.org.datavec.api.transform.condition.column.DoubleColumnCondition
Imports FilterInvalidValuesT = DeepLearning4VB.org.datavec.api.transform.filter.FilterInvalidValues
Imports LongWritableComparatorT = DeepLearning4VB.org.datavec.api.writable.comparator.LongWritableComparator
Imports MapAllStringsExceptListTransformT = DeepLearning4VB.org.datavec.api.transform.transforms.string.MapAllStringsExceptListTransform
Imports MathOpT = DeepLearning4VB.org.datavec.api.transform.MathOp
Imports NullWritableColumnConditionT = DeepLearning4VB.org.datavec.api.transform.condition.column.NullWritableColumnCondition
Imports NumericalColumnComparatorT = DeepLearning4VB.org.datavec.api.transform.sequence.comparator.NumericalColumnComparator
Imports OverlappingTimeWindowFunctionT = DeepLearning4VB.org.datavec.api.transform.sequence.window.OverlappingTimeWindowFunction
Imports ReduceOpT = DeepLearning4VB.org.datavec.api.transform.ReduceOp
Imports ReducerT = DeepLearning4VB.org.datavec.api.transform.reduce.Reducer
Imports ReplaceEmptyIntegerWithValueTransformT = DeepLearning4VB.org.datavec.api.transform.transforms.integer.ReplaceEmptyIntegerWithValueTransform
Imports ReplaceEmptyStringTransformT = DeepLearning4VB.org.datavec.api.transform.transforms.string.ReplaceEmptyStringTransform
Imports ReplaceInvalidWithIntegerTransformT = DeepLearning4VB.org.datavec.api.transform.transforms.integer.ReplaceInvalidWithIntegerTransform
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports SequenceDataAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.SequenceDataAnalysis
Imports SequenceLengthAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.sequence.SequenceLengthAnalysis
Imports SequenceLengthConditionT = DeepLearning4VB.org.datavec.api.transform.condition.sequence.SequenceLengthCondition
Imports SequenceOffsetTransformT = DeepLearning4VB.org.datavec.api.transform.transforms.sequence.SequenceOffsetTransform
Imports SequenceSplitTimeSeparationT = DeepLearning4VB.org.datavec.api.transform.sequence.split.SequenceSplitTimeSeparation
Imports StringAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.columns.StringAnalysis
Imports StringComparatorT = DeepLearning4VB.org.datavec.api.transform.sequence.comparator.StringComparator
Imports StringListToCategoricalSetTransformT = DeepLearning4VB.org.datavec.api.transform.transforms.string.StringListToCategoricalSetTransform
Imports TransformProcessT = DeepLearning4VB.org.datavec.api.transform.TransformProcess

Namespace org.datavec.api.transform.transforms

    ''' <summary>
    ''' Port of the DataVec <c>transform.TestJsonYaml</c> JUnit class. YAML is not supported, so only the JSON halves
    ''' are ported.
    ''' </summary>
    <TestClass>
    Public Class TestJsonYaml

        ''' <summary>The transform process of the Java test (shared with <see cref="RegressionTestJson"/>).</summary>
        Friend Shared Function buildTestProcess(ByVal withFirstDigit As Boolean) As TransformProcessT
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnCategorical("Cat", "State1", "State2").addColumnCategorical("Cat2", "State1", "State2").addColumnDouble("Dbl").addColumnDouble("Dbl2", Nothing, 100.0, True, False).addColumnInteger("Int").addColumnInteger("Int2", 0, 10).addColumnLong("Long").addColumnLong("Long2", -100L, Nothing).addColumnString("Str").addColumnString("Str2", "someregexhere", 1, Nothing).addColumnString("Str3").addColumnTime("TimeCol", DateTimeZone.UTC).addColumnTime("TimeCol2", DateTimeZone.UTC, Nothing, 1000L).build()

            Dim map As IDictionary(Of String, String) = New Dictionary(Of String, String)()
            map("from") = "to"
            map("anotherFrom") = "anotherTo"

            Dim b As TransformProcessT.Builder = (New TransformProcessT.Builder(schema)).categoricalToInteger("Cat").categoricalToOneHot("Cat2").appendStringColumnTransform("Str3", "ToAppend").integerToCategorical("Cat", New List(Of String) From {"State1", "State2"}).stringToCategorical("Str", New List(Of String) From {"State1", "State2"}).duplicateColumn("Str", "Str2a").removeColumns("Str2a").renameColumn("Str2", "Str2a").reorderColumns("Cat", "Dbl").conditionalCopyValueTransform("Dbl", "Dbl2", New DoubleColumnConditionT("Dbl", ConditionOpT.Equal, 0.0)).conditionalReplaceValueTransform("Dbl", New DoubleWritable(1.0), New DoubleColumnConditionT("Dbl", ConditionOpT.Equal, 1.0)).doubleColumnsMathOp("NewDouble", MathOpT.Add, "Dbl", "Dbl2").doubleMathOp("Dbl", MathOpT.Add, 1.0).integerColumnsMathOp("NewInt", MathOpT.Subtract, "Int", "Int2").integerMathOp("Int", MathOpT.Multiply, 2).transform(New ReplaceEmptyIntegerWithValueTransformT("Int", 1)).transform(New ReplaceInvalidWithIntegerTransformT("Int", 1)).longColumnsMathOp("Long", MathOpT.Multiply, "Long", "Long2").longMathOp("Long", MathOpT.ScalarMax, 0).transform(New MapAllStringsExceptListTransformT("Str", "Other", New List(Of String) From {"Ok", "SomeVal"})).stringRemoveWhitespaceTransform("Str").transform(New ReplaceEmptyStringTransformT("Str", "WasEmpty")).replaceStringTransform("Str", map).transform(New StringListToCategoricalSetTransformT("Str", New List(Of String) From {"StrA", "StrB"}, New List(Of String) From {"StrA", "StrB"}, ",")).stringMapTransform("Str2a", map).transform((New DeriveColumnsFromTimeTransformT.Builder("TimeCol")).addIntegerDerivedColumn("Hour", DateTimeFieldType.hourOfDay()).addStringDerivedColumn("Date", "YYYY-MM-dd", DateTimeZone.UTC).build()).stringToTimeTransform("Str2a", "YYYY-MM-dd hh:mm:ss", DateTimeZone.UTC).timeMathOp("TimeCol2", MathOpT.Add, 1, TimeUnit.HOURS).filter(New FilterInvalidValuesT("Cat", "Str2a")).filter(New ConditionFilterT(New NullWritableColumnConditionT("Long"))).convertToSequence("Int", New NumericalColumnComparatorT("TimeCol2")).convertFromSequence().convertToSequence("Int", New StringComparatorT("Str2a")).splitSequence(New SequenceSplitTimeSeparationT("TimeCol2", 1, TimeUnit.HOURS)).reduce((New ReducerT.Builder(ReduceOpT.TakeFirst)).keyColumns("TimeCol2").countColumns("Cat").sumColumns("Dbl").build()).reduceSequenceByWindow((New ReducerT.Builder(ReduceOpT.TakeFirst)).countColumns("Cat2").stdevColumns("Dbl2").build(), (New OverlappingTimeWindowFunctionT.Builder()).timeColumn("TimeCol2").addWindowStartTimeColumn(True).addWindowEndTimeColumn(True).windowSize(1, TimeUnit.HOURS).offset(5, TimeUnit.MINUTES).windowSeparation(15, TimeUnit.MINUTES).excludeEmptyWindows(True).build()).convertFromSequence().calculateSortedRank("rankColName", "TimeCol2", New LongWritableComparatorT()).sequenceMovingWindowReduce("rankColName", 20, ReduceOpT.Mean).addConstantColumn("someIntColumn", ColumnTypeT.Integer, New IntWritable(0)).integerToOneHot("someIntColumn", 0, 3).filter(New SequenceLengthConditionT(ConditionOpT.LessThan, 1)).addConstantColumn("testColSeq", ColumnTypeT.Integer, New DoubleWritable(0)).offsetSequence(New List(Of String) From {"testColSeq"}, 1, SequenceOffsetTransformT.OperationType.InPlace).addConstantColumn("someTextCol", ColumnTypeT.String, New Text("some values"))
            If withFirstDigit Then
                b = b.addConstantColumn("testFirstDigit", ColumnTypeT.Double, New DoubleWritable(0)).firstDigitTransform("testFirstDigit", "testFirstDigitOut")
            End If
            Return b.build()
        End Function

        <TestMethod>
        Public Sub testToFromJson()
            Dim tp As TransformProcessT = buildTestProcess(True)

            Dim asJson As String = tp.toJson()

            Dim tpFromJson As TransformProcessT = TransformProcessT.fromJson(asJson)

            Dim daList As IList(Of DataActionT) = tp.getActionList()
            Dim daListJson As IList(Of DataActionT) = tpFromJson.getActionList()
            assertEquals(daList.Count, daListJson.Count)

            For i As Integer = 0 To daList.Count - 1
                Dim da1 As DataActionT = daList(i)
                Dim da2 As DataActionT = daListJson(i)

                assertEquals(da1, da2, i & vbTab & da1.ToString())
            Next i

            assertEquals(tp, tpFromJson)
            assertEquals(tp.FinalSchema, tpFromJson.FinalSchema)
            ' written again, the JSON is the same
            assertEquals(asJson, tpFromJson.toJson())
        End Sub

        <TestMethod>
        Public Sub testJsonAnalysis()
            Dim s As SchemaT = (New SchemaT.Builder()).addColumnsDouble("first", "second").addColumnString("third").addColumnCategorical("fourth", "cat0", "cat1").build()

            Dim d1 As DoubleAnalysisT = (New DoubleAnalysisT.Builder()).max(-1).max(1).countPositive(10).mean(3.0).build()
            Dim d2 As DoubleAnalysisT = (New DoubleAnalysisT.Builder()).max(-5).max(5).countPositive(4).mean(2.0).build()
            Dim sa As StringAnalysisT = (New StringAnalysisT.Builder()).minLength(0).maxLength(10).build()
            Dim countMap As IDictionary(Of String, Long) = New Dictionary(Of String, Long)()
            countMap("cat0") = 100L
            countMap("cat1") = 200L
            Dim ca As New CategoricalAnalysisT(countMap)

            Dim da As New DataAnalysisT(s, New List(Of ColumnAnalysisT) From {d1, d2, sa, ca})

            Dim strJson As String = da.toJson()
            assertTrue(strJson.Contains("""@class"" : ""org.datavec.api.transform.analysis.DataAnalysis"""), strJson)
            assertTrue(strJson.Contains("""countTotal"""), strJson)

            Dim daFromJson As DataAnalysisT = DataAnalysisT.fromJson(strJson)

            assertEquals(da.getColumnAnalysis(), daFromJson.getColumnAnalysis())
            assertEquals(da, daFromJson)
        End Sub

        <TestMethod>
        Public Sub testJsonSequenceAnalysis()
            Dim s As SchemaT = (New SchemaT.Builder()).addColumnsDouble("first").build()
            Dim d1 As DoubleAnalysisT = (New DoubleAnalysisT.Builder()).min(-1).max(1).countPositive(10).mean(3.0).build()
            Dim sla As SequenceLengthAnalysisT = SequenceLengthAnalysisT.builder().totalNumSequences(3).minSeqLength(1).maxSeqLength(4).meanLength(2.5).build()
            Dim da As New SequenceDataAnalysisT(s, New List(Of ColumnAnalysisT) From {d1}, sla)

            Dim json As String = da.toJson()
            assertTrue(json.Contains("""@class"" : ""org.datavec.api.transform.analysis.SequenceDataAnalysis"""), json)
            Dim back As SequenceDataAnalysisT = SequenceDataAnalysisT.fromJson(json)
            assertEquals(da, back)
            assertEquals(4, back.getSequenceLengthAnalysis().getMaxSeqLength())
            ' read as the base type, the runtime type comes from "@class"
            assertTrue(TypeOf DataAnalysisT.fromJson(json) Is SequenceDataAnalysisT)
        End Sub

    End Class

End Namespace
