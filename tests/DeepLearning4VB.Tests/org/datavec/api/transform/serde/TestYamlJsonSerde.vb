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
Imports DeepLearning4VB.org.datavec.api.transform.condition.column
Imports DeepLearning4VB.org.datavec.api.transform.transforms.categorical
Imports DeepLearning4VB.org.datavec.api.transform.transforms.column
Imports DeepLearning4VB.org.datavec.api.transform.transforms.doubletransform
Imports DeepLearning4VB.org.datavec.api.transform.transforms.integer
Imports DeepLearning4VB.org.datavec.api.transform.transforms.string
Imports BooleanConditionT = DeepLearning4VB.org.datavec.api.transform.condition.BooleanCondition
Imports CalculateSortedRankT = DeepLearning4VB.org.datavec.api.transform.rank.CalculateSortedRank
Imports ConditionFilterT = DeepLearning4VB.org.datavec.api.transform.filter.ConditionFilter
Imports ConditionOpT = DeepLearning4VB.org.datavec.api.transform.condition.ConditionOp
Imports ConditionT = DeepLearning4VB.org.datavec.api.transform.condition.Condition
Imports ConditionalCopyValueTransformT = DeepLearning4VB.org.datavec.api.transform.transforms.condition.ConditionalCopyValueTransform
Imports ConvertFromSequenceT = DeepLearning4VB.org.datavec.api.transform.sequence.ConvertFromSequence
Imports ConvertToSequenceT = DeepLearning4VB.org.datavec.api.transform.sequence.ConvertToSequence
Imports DataActionT = DeepLearning4VB.org.datavec.api.transform.DataAction
Imports DateTimeFieldType = DeepLearning4VB.org.joda.time.DateTimeFieldType
Imports DateTimeZone = DeepLearning4VB.org.joda.time.DateTimeZone
Imports DeriveColumnsFromTimeTransformT = DeepLearning4VB.org.datavec.api.transform.transforms.time.DeriveColumnsFromTimeTransform
Imports DoubleWritableComparatorT = DeepLearning4VB.org.datavec.api.writable.comparator.DoubleWritableComparator
Imports FilterInvalidValuesT = DeepLearning4VB.org.datavec.api.transform.filter.FilterInvalidValues
Imports FilterT = DeepLearning4VB.org.datavec.api.transform.filter.Filter
Imports IAssociativeReducerT = DeepLearning4VB.org.datavec.api.transform.reduce.IAssociativeReducer
Imports JsonSerializerT = DeepLearning4VB.org.datavec.api.transform.serde.JsonSerializer
Imports LongColumnsMathOpTransformT = DeepLearning4VB.org.datavec.api.transform.transforms.longtransform.LongColumnsMathOpTransform
Imports LongMathOpTransformT = DeepLearning4VB.org.datavec.api.transform.transforms.longtransform.LongMathOpTransform
Imports MathOpT = DeepLearning4VB.org.datavec.api.transform.MathOp
Imports NumericalColumnComparatorT = DeepLearning4VB.org.datavec.api.transform.sequence.comparator.NumericalColumnComparator
Imports ReduceOpT = DeepLearning4VB.org.datavec.api.transform.ReduceOp
Imports ReducerT = DeepLearning4VB.org.datavec.api.transform.reduce.Reducer
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports SequenceComparatorT = DeepLearning4VB.org.datavec.api.transform.sequence.SequenceComparator
Imports SequenceSplitT = DeepLearning4VB.org.datavec.api.transform.sequence.SequenceSplit
Imports SequenceSplitTimeSeparationT = DeepLearning4VB.org.datavec.api.transform.sequence.split.SequenceSplitTimeSeparation
Imports SplitMaxLengthSequenceT = DeepLearning4VB.org.datavec.api.transform.sequence.split.SplitMaxLengthSequence
Imports StringComparatorT = DeepLearning4VB.org.datavec.api.transform.sequence.comparator.StringComparator
Imports StringRegexColumnConditionT = DeepLearning4VB.org.datavec.api.transform.condition.string.StringRegexColumnCondition
Imports StringToTimeTransformT = DeepLearning4VB.org.datavec.api.transform.transforms.time.StringToTimeTransform
Imports TimeMathOpTransformT = DeepLearning4VB.org.datavec.api.transform.transforms.time.TimeMathOpTransform
Imports TransformProcessT = DeepLearning4VB.org.datavec.api.transform.TransformProcess
Imports TransformT = DeepLearning4VB.org.datavec.api.transform.Transform

Namespace org.datavec.api.transform.serde

    ''' <summary>
    ''' Port of the DataVec <c>TestYamlJsonSerde</c> JUnit class: every transform, filter, condition, reducer,
    ''' sequence comparator, sorted rank, sequence split and data action survives a JSON round trip, alone and in lists.
    ''' YAML is not supported, so only the JSON halves are ported.
    ''' </summary>
    <TestClass>
    Public Class TestYamlJsonSerde

        Public Shared j As New JsonSerializerT()

        <TestMethod>
        Public Sub testTransforms()
            Dim map As IDictionary(Of String, String) = New Dictionary(Of String, String)()
            map("A") = "A1"
            map("B") = "B1"

            Dim transforms() As TransformT = {
                New CategoricalToIntegerTransform("ColName"),
                New CategoricalToOneHotTransform("ColName"),
                New IntegerToCategoricalTransform("ColName", New List(Of String) From {"State0", "State1"}),
                New StringToCategoricalTransform("ColName", New List(Of String) From {"State0", "State1"}),
                New DuplicateColumnsTransform(New List(Of String) From {"Dup1", "Dup2"}, New List(Of String) From {"NewName1", "NewName2"}),
                New RemoveColumnsTransform("R1", "R2"),
                New RenameColumnsTransform(New List(Of String) From {"N1", "N2"}, New List(Of String) From {"NewN1", "NewN2"}),
                New ReorderColumnsTransform("A", "B"),
                New DoubleColumnsMathOpTransform("NewName", MathOpT.Subtract, "A", "B"),
                New DoubleMathOpTransform("ColName", MathOpT.Multiply, 2.0),
                New Log2Normalizer("ColName", 1.0, 0.0, 2.0),
                New MinMaxNormalizer("ColName", 0, 100, -1, 1),
                New StandardizeNormalizer("ColName", 20, 5),
                New SubtractMeanNormalizer("ColName", 10),
                New IntegerColumnsMathOpTransform("NewName", MathOpT.Multiply, "A", "B"),
                New IntegerMathOpTransform("ColName", MathOpT.Add, 10),
                New ReplaceEmptyIntegerWithValueTransform("Col", 3),
                New ReplaceInvalidWithIntegerTransform("Col", 3),
                New LongColumnsMathOpTransformT("NewName", MathOpT.Multiply, "A", "B"),
                New LongMathOpTransformT("Col", MathOpT.ScalarMax, 5L),
                New MapAllStringsExceptListTransform("Col", "NewVal", New List(Of String) From {"E1", "E2"}),
                New RemoveWhiteSpaceTransform("Col"),
                New ReplaceEmptyStringTransform("Col", "WasEmpty"),
                New ReplaceStringTransform("Col_A", map),
                New StringListToCategoricalSetTransform("Col", New List(Of String) From {"A", "B"}, New List(Of String) From {"A", "B"}, ","),
                New StringMapTransform("Col", map),
                (New DeriveColumnsFromTimeTransformT.Builder("TimeColName")).addIntegerDerivedColumn("Hour", DateTimeFieldType.hourOfDay()).addStringDerivedColumn("DateTime", "YYYY-MM-dd hh:mm:ss", DateTimeZone.UTC).build(),
                New StringToTimeTransformT("TimeCol", "YYYY-MM-dd hh:mm:ss", DateTimeZone.UTC),
                New TimeMathOpTransformT("TimeCol", MathOpT.Add, 1, TimeUnit.HOURS)
            }

            For Each t As TransformT In transforms
                Dim json As String = j.serialize(t)
                Dim t3 As TransformT = j.deserializeTransform(json)
                assertEquals(t, t3)
            Next t

            Dim tArrAsJson As String = j.serialize(transforms)
            Dim tListAsJson As String = j.serializeTransformList(New List(Of TransformT)(transforms))

            Dim lFromJson As IList(Of TransformT) = j.deserializeTransformList(tListAsJson)

            assertEquals(New List(Of TransformT)(transforms), j.deserializeTransformList(tArrAsJson))
            assertEquals(New List(Of TransformT)(transforms), lFromJson)
        End Sub

        <TestMethod>
        Public Sub testTransformsRegexBrackets()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnString("someCol").addColumnString("otherCol").build()

            Dim transforms() As TransformT = {
                New ConditionalCopyValueTransformT("someCol", "otherCol", New StringRegexColumnConditionT("someCol", "\d")),
                New ConditionalCopyValueTransformT("someCol", "otherCol", New StringRegexColumnConditionT("someCol", "\D+")),
                New ConditionalCopyValueTransformT("someCol", "otherCol", New StringRegexColumnConditionT("someCol", """.*""")),
                New ConditionalCopyValueTransformT("someCol", "otherCol", New StringRegexColumnConditionT("someCol", "[]{}()][}{)("))
            }

            For Each t As TransformT In transforms
                Dim json As String = j.serialize(t)

                Dim t3 As TransformT = j.deserializeTransform(json)
                assertEquals(t, t3)

                Dim tp As TransformProcessT = (New TransformProcessT.Builder(schema)).transform(t).build()

                Dim tpJson As String = j.serialize(tp)

                Dim fromJson As TransformProcessT = TransformProcessT.fromJson(tpJson)

                assertEquals(tp, fromJson)
            Next t

            Dim tArrAsJson As String = j.serialize(transforms)
            Dim tListAsJson As String = j.serializeTransformList(New List(Of TransformT)(transforms))

            Dim lFromJson As IList(Of TransformT) = j.deserializeTransformList(tListAsJson)

            assertEquals(New List(Of TransformT)(transforms), j.deserializeTransformList(tArrAsJson))
            assertEquals(New List(Of TransformT)(transforms), lFromJson)
        End Sub

        <TestMethod>
        Public Sub testFilters()
            Dim filters() As FilterT = {
                New FilterInvalidValuesT("A", "B"),
                New ConditionFilterT(New DoubleColumnCondition("Col", ConditionOpT.GreaterOrEqual, 10.0))
            }

            For Each f As FilterT In filters
                Dim json As String = j.serialize(f)
                Dim t3 As FilterT = j.deserializeFilter(json)
                assertEquals(f, t3)
            Next f

            Dim arrAsJson As String = j.serialize(filters)
            Dim listAsJson As String = j.serializeFilterList(New List(Of FilterT)(filters))

            Dim lFromJson As IList(Of FilterT) = j.deserializeFilterList(listAsJson)

            assertEquals(New List(Of FilterT)(filters), j.deserializeFilterList(arrAsJson))
            assertEquals(New List(Of FilterT)(filters), lFromJson)
        End Sub

        <TestMethod>
        Public Sub testConditions()
            Dim setStr As ISet(Of String) = New HashSet(Of String)()
            setStr.Add("A")
            setStr.Add("B")

            Dim setD As ISet(Of Double) = New HashSet(Of Double)()
            setD.Add(1.0)
            setD.Add(2.0)

            Dim setI As ISet(Of Integer) = New HashSet(Of Integer)()
            setI.Add(1)
            setI.Add(2)

            Dim setL As ISet(Of Long) = New HashSet(Of Long)()
            setL.Add(1L)
            setL.Add(2L)

            Dim conditions() As ConditionT = {
                New CategoricalColumnCondition("Col", ConditionOpT.Equal, "A"),
                New CategoricalColumnCondition("Col", ConditionOpT.NotInSet, setStr),
                New DoubleColumnCondition("Col", ConditionOpT.Equal, 1.0),
                New DoubleColumnCondition("Col", ConditionOpT.InSet, setD),
                New IntegerColumnCondition("Col", ConditionOpT.Equal, 1),
                New IntegerColumnCondition("Col", ConditionOpT.InSet, setI),
                New LongColumnCondition("Col", ConditionOpT.Equal, 1),
                New LongColumnCondition("Col", ConditionOpT.InSet, setL),
                New NullWritableColumnCondition("Col"),
                New StringColumnCondition("Col", ConditionOpT.NotEqual, "A"),
                New StringColumnCondition("Col", ConditionOpT.InSet, setStr),
                New TimeColumnCondition("Col", ConditionOpT.Equal, 1L),
                New TimeColumnCondition("Col", ConditionOpT.InSet, setL),
                New StringRegexColumnConditionT("Col", "Regex"),
                BooleanConditionT.OR(BooleanConditionT.AND(New CategoricalColumnCondition("Col", ConditionOpT.Equal, "A"), New LongColumnCondition("Col2", ConditionOpT.Equal, 1)), BooleanConditionT.NOT(New TimeColumnCondition("Col3", ConditionOpT.Equal, 1L)))
            }

            For Each c As ConditionT In conditions
                Dim json As String = j.serialize(c)
                Dim t3 As ConditionT = j.deserializeCondition(json)
                assertEquals(c, t3)
            Next c

            Dim arrAsJson As String = j.serialize(conditions)
            Dim listAsJson As String = j.serializeConditionList(New List(Of ConditionT)(conditions))

            Dim lFromJson As IList(Of ConditionT) = j.deserializeConditionList(listAsJson)

            assertEquals(New List(Of ConditionT)(conditions), j.deserializeConditionList(arrAsJson))
            assertEquals(New List(Of ConditionT)(conditions), lFromJson)
        End Sub

        <TestMethod>
        Public Sub testReducer()
            Dim reducers() As IAssociativeReducerT = {(New ReducerT.Builder(ReduceOpT.TakeFirst)).keyColumns("KeyCol").stdevColumns("Stdev").minColumns("min").countUniqueColumns("B").build()}

            For Each r As IAssociativeReducerT In reducers
                Dim json As String = j.serialize(r)
                ' Java: @JsonTypeInfo(use = NAME, include = WRAPPER_OBJECT)
                assertTrue(json.StartsWith("{" & Environment.NewLine & "  ""Reducer"" : {"), json)
                Dim t3 As IAssociativeReducerT = j.deserializeReducer(json)
                assertEquals(r, t3)
            Next r

            Dim arrAsJson As String = j.serialize(reducers)
            Dim listAsJson As String = j.serializeReducerList(New List(Of IAssociativeReducerT)(reducers))

            Dim lFromJson As IList(Of IAssociativeReducerT) = j.deserializeReducerList(listAsJson)

            assertEquals(New List(Of IAssociativeReducerT)(reducers), j.deserializeReducerList(arrAsJson))
            assertEquals(New List(Of IAssociativeReducerT)(reducers), lFromJson)
        End Sub

        <TestMethod>
        Public Sub testSequenceComparator()
            Dim comparators() As SequenceComparatorT = {
                New NumericalColumnComparatorT("Col", True),
                New StringComparatorT("Col")
            }

            For Each f As SequenceComparatorT In comparators
                Dim json As String = j.serialize(f)
                Dim t3 As SequenceComparatorT = j.deserializeSequenceComparator(json)
                assertEquals(f, t3)
            Next f

            Dim arrAsJson As String = j.serialize(comparators)
            Dim listAsJson As String = j.serializeSequenceComparatorList(New List(Of SequenceComparatorT)(comparators))

            Dim lFromJson As IList(Of SequenceComparatorT) = j.deserializeSequenceComparatorList(listAsJson)

            assertEquals(New List(Of SequenceComparatorT)(comparators), j.deserializeSequenceComparatorList(arrAsJson))
            assertEquals(New List(Of SequenceComparatorT)(comparators), lFromJson)
        End Sub

        <TestMethod>
        Public Sub testCalculateSortedRank()
            Dim rank As New CalculateSortedRankT("RankCol", "SortOnCol", New DoubleWritableComparatorT())

            Dim asJson As String = j.serialize(rank)

            Dim jRank As CalculateSortedRankT = j.deserializeSortedRank(asJson)

            assertEquals(rank, jRank)
        End Sub

        <TestMethod>
        Public Sub testSequenceSplit()
            Dim splits() As SequenceSplitT = {
                New SequenceSplitTimeSeparationT("col", 1, TimeUnit.HOURS),
                New SplitMaxLengthSequenceT(100, False)
            }

            For Each f As SequenceSplitT In splits
                Dim json As String = j.serialize(f)
                Dim t3 As SequenceSplitT = j.deserializeSequenceSplit(json)
                assertEquals(f, t3)
            Next f
        End Sub

        <TestMethod>
        Public Sub testDataAction()
            Dim dataActions() As DataActionT = {
                New DataActionT(New CategoricalToIntegerTransform("Col")),
                New DataActionT(New ConditionFilterT(New DoubleColumnCondition("Col", ConditionOpT.Equal, 1))),
                New DataActionT(New ConvertToSequenceT("KeyCol", New NumericalColumnComparatorT("Col", True))),
                New DataActionT(New ConvertFromSequenceT()),
                New DataActionT(New SequenceSplitTimeSeparationT("TimeCol", 1, TimeUnit.HOURS)),
                New DataActionT((New ReducerT.Builder(ReduceOpT.TakeFirst)).build()),
                New DataActionT(New CalculateSortedRankT("NewCol", "SortCol", New DoubleWritableComparatorT()))
            }

            For Each f As DataActionT In dataActions
                Dim json As String = j.serialize(f)
                Dim t3 As DataActionT = j.deserializeDataAction(json)
                assertEquals(f, t3)
            Next f

            Dim arrAsJson As String = j.serialize(dataActions)
            Dim listAsJson As String = j.serializeDataActionList(New List(Of DataActionT)(dataActions))

            Dim lFromJson As IList(Of DataActionT) = j.deserializeDataActionList(listAsJson)

            assertEquals(New List(Of DataActionT)(dataActions), j.deserializeDataActionList(arrAsJson))
            assertEquals(New List(Of DataActionT)(dataActions), lFromJson)
        End Sub

    End Class

End Namespace
