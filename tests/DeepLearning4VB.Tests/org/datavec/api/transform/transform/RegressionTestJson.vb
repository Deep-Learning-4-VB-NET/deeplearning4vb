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
Imports System.IO
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.datavec.api.writable
Imports BooleanConditionT = DeepLearning4VB.org.datavec.api.transform.condition.BooleanCondition
Imports ClassPathResource = DeepLearning4VB.org.nd4vb.common.io.ClassPathResource
Imports ColumnTypeT = DeepLearning4VB.org.datavec.api.transform.ColumnType
Imports ConditionOpT = DeepLearning4VB.org.datavec.api.transform.condition.ConditionOp
Imports DateTimeFieldType = DeepLearning4VB.org.joda.time.DateTimeFieldType
Imports DateTimeZone = DeepLearning4VB.org.joda.time.DateTimeZone
Imports DeriveColumnsFromTimeTransformT = DeepLearning4VB.org.datavec.api.transform.transforms.time.DeriveColumnsFromTimeTransform
Imports DoubleColumnConditionT = DeepLearning4VB.org.datavec.api.transform.condition.column.DoubleColumnCondition
Imports DoubleWritableComparatorT = DeepLearning4VB.org.datavec.api.writable.comparator.DoubleWritableComparator
Imports IntegerColumnConditionT = DeepLearning4VB.org.datavec.api.transform.condition.column.IntegerColumnCondition
Imports LongColumnConditionT = DeepLearning4VB.org.datavec.api.transform.condition.column.LongColumnCondition
Imports MinMaxNormalizerT = DeepLearning4VB.org.datavec.api.transform.transforms.doubletransform.MinMaxNormalizer
Imports NumericalColumnComparatorT = DeepLearning4VB.org.datavec.api.transform.sequence.comparator.NumericalColumnComparator
Imports ReduceOpT = DeepLearning4VB.org.datavec.api.transform.ReduceOp
Imports ReducerT = DeepLearning4VB.org.datavec.api.transform.reduce.Reducer
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports SequenceLengthConditionT = DeepLearning4VB.org.datavec.api.transform.condition.sequence.SequenceLengthCondition
Imports SequenceMovingWindowReduceTransformT = DeepLearning4VB.org.datavec.api.transform.transforms.sequence.SequenceMovingWindowReduceTransform
Imports StringRegexColumnConditionT = DeepLearning4VB.org.datavec.api.transform.condition.string.StringRegexColumnCondition
Imports TransformProcessT = DeepLearning4VB.org.datavec.api.transform.TransformProcess
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms

    ''' <summary>
    ''' Port of the DataVec <c>RegressionTestJson</c> JUnit class, plus a test of JSON in the current Java format.
    ''' <para>The JSON files of the Java test resources are not part of this repository. They are rebuilt here:
    ''' <c>regression_test/100a/transformprocess_regression_100a.json</c> is the transform process of the Java test in
    ''' the 1.0.0-alpha wrapper-object format (type names as property names, as written with the mix-ins of
    ''' <c>LegacyJsonFormat</c>); <c>regression_test/java_format/transformprocess_java_format.json</c> is written by hand in
    ''' the format of Java DataVec 1.0.0-beta and later ("@class" names, Jackson layout, Java-only fields such as an
    ''' embedded input schema, a property from a newer version and an "@class" that is not the first property).</para>
    ''' </summary>
    <TestClass>
    Public Class RegressionTestJson

        Private Shared Function readResource(ByVal path As String) As String
            Return File.ReadAllText((New ClassPathResource(path)).File)
        End Function

        <TestMethod>
        Public Sub regressionTestJson100a()
            'JSON saved in 1.0.0-alpha, before JSON format change
            Dim s As String = readResource("datavec-api/regression_test/100a/transformprocess_regression_100a.json")

            Dim fromJson As TransformProcessT = TransformProcessT.fromJson(s)

            Dim expected As TransformProcessT = TestJsonYaml.buildTestProcess(False)

            assertEquals(expected, fromJson)
        End Sub

        <TestMethod>
        Public Sub regressionTestJavaFormat()
            Dim s As String = readResource("datavec-api/regression_test/java_format/transformprocess_java_format.json")

            Dim fromJson As TransformProcessT = TransformProcessT.fromJson(s)

            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnInteger("id", 0, 1000).addColumnCategorical("cat", "a", "b", "c").addColumnDouble("score", 0.0, 100.0, True, False).addColumnTime("when", DateTimeZone.forID("America/New_York")).addColumnString("text", "\w+", 1, 20).addColumnBoolean("flag").addColumnLong("amount", 0L, Nothing).addColumnFloat("weight", Nothing, 1.5E10F, False, False).build()

            Dim expected As TransformProcessT = (New TransformProcessT.Builder(schema)).categoricalToInteger("cat").transform(New MinMaxNormalizerT("score", 0.0, 100.0)).conditionalReplaceValueTransformWithDefault("amount", New LongWritable(0), New LongWritable(1), New LongColumnConditionT("amount", ConditionOpT.LessThan, 10)).addConstantColumn("source", ColumnTypeT.String, New Text("java")).transform((New DeriveColumnsFromTimeTransformT.Builder("when")).addIntegerDerivedColumn("hour", DateTimeFieldType.hourOfDay()).addStringDerivedColumn("day", "YYYY-MM-dd", DateTimeZone.UTC).build()).filter(BooleanConditionT.AND(New IntegerColumnConditionT("id", ConditionOpT.InSet, New HashSet(Of Integer) From {1, 2, 3}), BooleanConditionT.NOT(New StringRegexColumnConditionT("text", "x+")))).filter(New DoubleColumnConditionT("score", ConditionOpT.Equal, Double.NaN)).convertToSequence("id", New NumericalColumnComparatorT("when", True)).transform(New SequenceMovingWindowReduceTransformT("score", "avgScore", 3, ReduceOpT.Mean, SequenceMovingWindowReduceTransformT.EdgeCaseHandling.Default, Nothing)).filter(New SequenceLengthConditionT(ConditionOpT.LessThan, 2)).convertFromSequence().reduce((New ReducerT.Builder(ReduceOpT.TakeFirst)).keyColumns("id").meanColumns("score").sumColumns("amount").build()).calculateSortedRank("rank", "mean(score)", New DoubleWritableComparatorT(), False).build()

            assertEquals(expected.getActionList().Count, fromJson.getActionList().Count)
            For i As Integer = 0 To expected.getActionList().Count - 1
                assertEquals(expected.getActionList()(i), fromJson.getActionList()(i), "step " & i)
            Next i
            assertEquals(expected, fromJson)
            assertEquals(expected.FinalSchema, fromJson.FinalSchema)
            assertEquals("rank", fromJson.FinalSchema.getName(fromJson.FinalSchema.numColumns() - 1))

            ' the values read from Java JSON: Joda time zones and field types, Text bytes, NaN
            Dim derive As DeriveColumnsFromTimeTransformT = DirectCast(fromJson.getActionList()(4).getTransform(), DeriveColumnsFromTimeTransformT)
            assertEquals(DateTimeZone.forID("America/New_York"), derive.getInputTimeZone())
            assertEquals(DateTimeFieldType.hourOfDay(), derive.getDerivedColumns()(0).getFieldType())
            assertEquals(DateTimeZone.UTC, derive.getDerivedColumns()(1).getDateTimeZone())
            assertEquals(New List(Of WritableT) From {New Text("1"), New Text("b"), New DoubleWritable(0.5), New LongWritable(0L), New Text("t"), New BooleanWritable(True), New LongWritable(0L), New FloatWritable(1.0F), New Text("java")},
                         New List(Of WritableT)(fromJson.getActionList()(3).getTransform().map(New List(Of WritableT) From {New Text("1"), New Text("b"), New DoubleWritable(0.5), New LongWritable(0L), New Text("t"), New BooleanWritable(True), New LongWritable(0L), New FloatWritable(1.0F)})))

            ' written again, our JSON is read back to the same process
            assertEquals(expected, TransformProcessT.fromJson(fromJson.toJson()))
        End Sub

    End Class

End Namespace
