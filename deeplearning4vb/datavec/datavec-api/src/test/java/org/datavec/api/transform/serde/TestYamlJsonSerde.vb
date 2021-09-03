Imports System.Collections.Generic
Imports org.datavec.api.transform
Imports BooleanCondition = org.datavec.api.transform.condition.BooleanCondition
Imports Condition = org.datavec.api.transform.condition.Condition
Imports ConditionOp = org.datavec.api.transform.condition.ConditionOp
Imports org.datavec.api.transform.condition.column
Imports StringRegexColumnCondition = org.datavec.api.transform.condition.string.StringRegexColumnCondition
Imports ConditionFilter = org.datavec.api.transform.filter.ConditionFilter
Imports Filter = org.datavec.api.transform.filter.Filter
Imports FilterInvalidValues = org.datavec.api.transform.filter.FilterInvalidValues
Imports CalculateSortedRank = org.datavec.api.transform.rank.CalculateSortedRank
Imports IAssociativeReducer = org.datavec.api.transform.reduce.IAssociativeReducer
Imports Reducer = org.datavec.api.transform.reduce.Reducer
Imports Schema = org.datavec.api.transform.schema.Schema
Imports ConvertFromSequence = org.datavec.api.transform.sequence.ConvertFromSequence
Imports ConvertToSequence = org.datavec.api.transform.sequence.ConvertToSequence
Imports SequenceComparator = org.datavec.api.transform.sequence.SequenceComparator
Imports SequenceSplit = org.datavec.api.transform.sequence.SequenceSplit
Imports NumericalColumnComparator = org.datavec.api.transform.sequence.comparator.NumericalColumnComparator
Imports StringComparator = org.datavec.api.transform.sequence.comparator.StringComparator
Imports SequenceSplitTimeSeparation = org.datavec.api.transform.sequence.split.SequenceSplitTimeSeparation
Imports SplitMaxLengthSequence = org.datavec.api.transform.sequence.split.SplitMaxLengthSequence
Imports CategoricalToIntegerTransform = org.datavec.api.transform.transform.categorical.CategoricalToIntegerTransform
Imports CategoricalToOneHotTransform = org.datavec.api.transform.transform.categorical.CategoricalToOneHotTransform
Imports IntegerToCategoricalTransform = org.datavec.api.transform.transform.categorical.IntegerToCategoricalTransform
Imports StringToCategoricalTransform = org.datavec.api.transform.transform.categorical.StringToCategoricalTransform
Imports DuplicateColumnsTransform = org.datavec.api.transform.transform.column.DuplicateColumnsTransform
Imports RemoveColumnsTransform = org.datavec.api.transform.transform.column.RemoveColumnsTransform
Imports RenameColumnsTransform = org.datavec.api.transform.transform.column.RenameColumnsTransform
Imports ReorderColumnsTransform = org.datavec.api.transform.transform.column.ReorderColumnsTransform
Imports ConditionalCopyValueTransform = org.datavec.api.transform.transform.condition.ConditionalCopyValueTransform
Imports org.datavec.api.transform.transform.doubletransform
Imports IntegerColumnsMathOpTransform = org.datavec.api.transform.transform.integer.IntegerColumnsMathOpTransform
Imports IntegerMathOpTransform = org.datavec.api.transform.transform.integer.IntegerMathOpTransform
Imports ReplaceEmptyIntegerWithValueTransform = org.datavec.api.transform.transform.integer.ReplaceEmptyIntegerWithValueTransform
Imports ReplaceInvalidWithIntegerTransform = org.datavec.api.transform.transform.integer.ReplaceInvalidWithIntegerTransform
Imports LongColumnsMathOpTransform = org.datavec.api.transform.transform.longtransform.LongColumnsMathOpTransform
Imports LongMathOpTransform = org.datavec.api.transform.transform.longtransform.LongMathOpTransform
Imports org.datavec.api.transform.transform.string
Imports DeriveColumnsFromTimeTransform = org.datavec.api.transform.transform.time.DeriveColumnsFromTimeTransform
Imports StringToTimeTransform = org.datavec.api.transform.transform.time.StringToTimeTransform
Imports TimeMathOpTransform = org.datavec.api.transform.transform.time.TimeMathOpTransform
Imports DoubleWritableComparator = org.datavec.api.writable.comparator.DoubleWritableComparator
Imports DateTimeFieldType = org.joda.time.DateTimeFieldType
Imports DateTimeZone = org.joda.time.DateTimeZone
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports TagNames = org.nd4j.common.tests.tags.TagNames
import static org.junit.jupiter.api.Assertions.assertEquals

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

Namespace org.datavec.api.transform.serde


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) @Tag(TagNames.JACKSON_SERDE) public class TestYamlJsonSerde extends org.nd4j.common.tests.BaseND4JTest
	Public Class TestYamlJsonSerde
		Inherits BaseND4JTest

		Public Shared y As New YamlSerializer()
		Public Shared j As New JsonSerializer()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testTransforms()
		Public Overridable Sub testTransforms()

			Dim map As IDictionary(Of String, String) = New Dictionary(Of String, String)()
			map("A") = "A1"
			map("B") = "B1"

			Dim transforms() As Transform = {
				New CategoricalToIntegerTransform("ColName"),
				New CategoricalToOneHotTransform("ColName"),
				New IntegerToCategoricalTransform("ColName", java.util.Arrays.asList("State0", "State1")),
				New StringToCategoricalTransform("ColName", java.util.Arrays.asList("State0", "State1")),
				New DuplicateColumnsTransform(New List(Of String) From {"Dup1", "Dup2"}, New List(Of String) From {"NewName1", "NewName2"}),
				New RemoveColumnsTransform("R1", "R2"),
				New RenameColumnsTransform(java.util.Arrays.asList("N1", "N2"), java.util.Arrays.asList("NewN1", "NewN2")),
				New ReorderColumnsTransform("A", "B"),
				New DoubleColumnsMathOpTransform("NewName", MathOp.Subtract, "A", "B"),
				New DoubleMathOpTransform("ColName", MathOp.Multiply, 2.0),
				New Log2Normalizer("ColName", 1.0, 0.0, 2.0),
				New MinMaxNormalizer("ColName", 0, 100, -1, 1),
				New StandardizeNormalizer("ColName", 20, 5),
				New SubtractMeanNormalizer("ColName", 10),
				New IntegerColumnsMathOpTransform("NewName", MathOp.Multiply, "A", "B"),
				New IntegerMathOpTransform("ColName", MathOp.Add, 10),
				New ReplaceEmptyIntegerWithValueTransform("Col", 3),
				New ReplaceInvalidWithIntegerTransform("Col", 3),
				New LongColumnsMathOpTransform("NewName", MathOp.Multiply, "A", "B"),
				New LongMathOpTransform("Col", MathOp.ScalarMax, 5L),
				New MapAllStringsExceptListTransform("Col", "NewVal", New List(Of String) From {"E1", "E2"}),
				New RemoveWhiteSpaceTransform("Col"),
				New ReplaceEmptyStringTransform("Col", "WasEmpty"),
				New ReplaceStringTransform("Col_A", map),
				New StringListToCategoricalSetTransform("Col", New List(Of String) From {"A", "B"}, New List(Of String) From {"A", "B"}, ","),
				New StringMapTransform("Col", map),
				(New DeriveColumnsFromTimeTransform.Builder("TimeColName")).addIntegerDerivedColumn("Hour", DateTimeFieldType.hourOfDay()).addStringDerivedColumn("DateTime", "YYYY-MM-dd hh:mm:ss", DateTimeZone.UTC).build(),
				New StringToTimeTransform("TimeCol", "YYYY-MM-dd hh:mm:ss", DateTimeZone.UTC),
				New TimeMathOpTransform("TimeCol", MathOp.Add, 1, TimeUnit.HOURS)
			}

			For Each t As Transform In transforms
				Dim yaml As String = y.serialize(t)
				Dim json As String = j.serialize(t)

				'            System.out.println(yaml);
	'                        System.out.println(json);
				'            System.out.println();

	'            Transform t2 = y.deserializeTransform(yaml);
				Dim t3 As Transform = j.deserializeTransform(json)
	'            assertEquals(t, t2);
				assertEquals(t, t3)
			Next t


			Dim tArrAsYaml As String = y.serialize(transforms)
			Dim tArrAsJson As String = j.serialize(transforms)
			Dim tListAsYaml As String = y.serializeTransformList(New List(Of org.datavec.api.transform.Transform) From {transforms})
			Dim tListAsJson As String = j.serializeTransformList(New List(Of org.datavec.api.transform.Transform) From {transforms})

			'        System.out.println("\n\n\n\n");
			'        System.out.println(tListAsYaml);

			Dim lFromYaml As IList(Of Transform) = y.deserializeTransformList(tListAsYaml)
			Dim lFromJson As IList(Of Transform) = j.deserializeTransformList(tListAsJson)

			assertEquals(java.util.Arrays.asList(transforms), y.deserializeTransformList(tArrAsYaml))
			assertEquals(java.util.Arrays.asList(transforms), j.deserializeTransformList(tArrAsJson))
			assertEquals(java.util.Arrays.asList(transforms), lFromYaml)
			assertEquals(java.util.Arrays.asList(transforms), lFromJson)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testTransformsRegexBrackets()
		Public Overridable Sub testTransformsRegexBrackets()

			Dim schema As Schema = (New Schema.Builder()).addColumnString("someCol").addColumnString("otherCol").build()

			Dim transforms() As Transform = {
				New ConditionalCopyValueTransform("someCol", "otherCol", New StringRegexColumnCondition("someCol", "\d")),
				New ConditionalCopyValueTransform("someCol", "otherCol", New StringRegexColumnCondition("someCol", "\D+")),
				New ConditionalCopyValueTransform("someCol", "otherCol", New StringRegexColumnCondition("someCol", """.*""")),
				New ConditionalCopyValueTransform("someCol", "otherCol", New StringRegexColumnCondition("someCol", "[]{}()][}{)("))
			}

			For Each t As Transform In transforms
				Dim json As String = j.serialize(t)

				Dim t3 As Transform = j.deserializeTransform(json)
				assertEquals(t, t3)

				Dim tp As TransformProcess = (New TransformProcess.Builder(schema)).transform(t).build()

				Dim tpJson As String = j.serialize(tp)

				Dim fromJson As TransformProcess = TransformProcess.fromJson(tpJson)

				assertEquals(tp, fromJson)
			Next t


			Dim tArrAsYaml As String = y.serialize(transforms)
			Dim tArrAsJson As String = j.serialize(transforms)
			Dim tListAsYaml As String = y.serializeTransformList(New List(Of org.datavec.api.transform.Transform) From {transforms})
			Dim tListAsJson As String = j.serializeTransformList(New List(Of org.datavec.api.transform.Transform) From {transforms})

			Dim lFromYaml As IList(Of Transform) = y.deserializeTransformList(tListAsYaml)
			Dim lFromJson As IList(Of Transform) = j.deserializeTransformList(tListAsJson)

			assertEquals(java.util.Arrays.asList(transforms), y.deserializeTransformList(tArrAsYaml))
			assertEquals(java.util.Arrays.asList(transforms), j.deserializeTransformList(tArrAsJson))
			assertEquals(java.util.Arrays.asList(transforms), lFromYaml)
			assertEquals(java.util.Arrays.asList(transforms), lFromJson)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testFilters()
		Public Overridable Sub testFilters()
			Dim filters() As Filter = {
				New FilterInvalidValues("A", "B"),
				New ConditionFilter(New DoubleColumnCondition("Col", ConditionOp.GreaterOrEqual, 10.0))
			}

			For Each f As Filter In filters
				Dim yaml As String = y.serialize(f)
				Dim json As String = j.serialize(f)

				'            System.out.println(yaml);
				'            System.out.println(json);
				'            System.out.println();

				Dim t2 As Filter = y.deserializeFilter(yaml)
				Dim t3 As Filter = j.deserializeFilter(json)
				assertEquals(f, t2)
				assertEquals(f, t3)
			Next f

			Dim arrAsYaml As String = y.serialize(filters)
			Dim arrAsJson As String = j.serialize(filters)
			Dim listAsYaml As String = y.serializeFilterList(New List(Of Filter) From {filters})
			Dim listAsJson As String = j.serializeFilterList(New List(Of Filter) From {filters})

			'        System.out.println("\n\n\n\n");
			'        System.out.println(listAsYaml);

			Dim lFromYaml As IList(Of Filter) = y.deserializeFilterList(listAsYaml)
			Dim lFromJson As IList(Of Filter) = j.deserializeFilterList(listAsJson)

			assertEquals(java.util.Arrays.asList(filters), y.deserializeFilterList(arrAsYaml))
			assertEquals(java.util.Arrays.asList(filters), j.deserializeFilterList(arrAsJson))
			assertEquals(java.util.Arrays.asList(filters), lFromYaml)
			assertEquals(java.util.Arrays.asList(filters), lFromJson)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testConditions()
		Public Overridable Sub testConditions()
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

			Dim conditions() As Condition = {
				New CategoricalColumnCondition("Col", ConditionOp.Equal, "A"),
				New CategoricalColumnCondition("Col", ConditionOp.NotInSet, setStr),
				New DoubleColumnCondition("Col", ConditionOp.Equal, 1.0),
				New DoubleColumnCondition("Col", ConditionOp.InSet, setD),
				New IntegerColumnCondition("Col", ConditionOp.Equal, 1),
				New IntegerColumnCondition("Col", ConditionOp.InSet, setI),
				New LongColumnCondition("Col", ConditionOp.Equal, 1),
				New LongColumnCondition("Col", ConditionOp.InSet, setL),
				New NullWritableColumnCondition("Col"),
				New StringColumnCondition("Col", ConditionOp.NotEqual, "A"),
				New StringColumnCondition("Col", ConditionOp.InSet, setStr),
				New TimeColumnCondition("Col", ConditionOp.Equal, 1L),
				New TimeColumnCondition("Col", ConditionOp.InSet, setL),
				New StringRegexColumnCondition("Col", "Regex"),
				BooleanCondition.OR(BooleanCondition.AND(New CategoricalColumnCondition("Col", ConditionOp.Equal, "A"), New LongColumnCondition("Col2", ConditionOp.Equal, 1)), BooleanCondition.NOT(New TimeColumnCondition("Col3", ConditionOp.Equal, 1L)))
			}

			For Each c As Condition In conditions
				Dim yaml As String = y.serialize(c)
				Dim json As String = j.serialize(c)

	'                        System.out.println(yaml);
	'                        System.out.println(json);
				'            System.out.println();

				Dim t2 As Condition = y.deserializeCondition(yaml)
				Dim t3 As Condition = j.deserializeCondition(json)
				assertEquals(c, t2)
				assertEquals(c, t3)
			Next c

			Dim arrAsYaml As String = y.serialize(conditions)
			Dim arrAsJson As String = j.serialize(conditions)
			Dim listAsYaml As String = y.serializeConditionList(New List(Of Condition) From {conditions})
			Dim listAsJson As String = j.serializeConditionList(New List(Of Condition) From {conditions})

			'        System.out.println("\n\n\n\n");
			'        System.out.println(listAsYaml);

			Dim lFromYaml As IList(Of Condition) = y.deserializeConditionList(listAsYaml)
			Dim lFromJson As IList(Of Condition) = j.deserializeConditionList(listAsJson)

			assertEquals(java.util.Arrays.asList(conditions), y.deserializeConditionList(arrAsYaml))
			assertEquals(java.util.Arrays.asList(conditions), j.deserializeConditionList(arrAsJson))
			assertEquals(java.util.Arrays.asList(conditions), lFromYaml)
			assertEquals(java.util.Arrays.asList(conditions), lFromJson)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testReducer()
		Public Overridable Sub testReducer()
			Dim reducers() As IAssociativeReducer = {New Reducer.Builder(ReduceOp.TakeFirst)
							.keyColumns("KeyCol").stdevColumns("Stdev").minColumns("min").countUniqueColumns("B").build()
		End Sub

			Function for(ByVal r As IAssociativeReducer) As Friend Overridable
				Dim yaml As String = y.serialize(r)
				Dim json As String = j.serialize(r)

				'            System.out.println(yaml);
				'            System.out.println(json);
				'            System.out.println();

				Dim t2 As IAssociativeReducer = y.deserializeReducer(yaml)
				Dim t3 As IAssociativeReducer = j.deserializeReducer(json)
				assertEquals(r, t2)
				assertEquals(r, t3)
			End Function

			Friend arrAsYaml As String = y.serialize(reducers)
			Friend arrAsJson As String = j.serialize(reducers)
			Friend listAsYaml As String = y.serializeReducerList(New List(Of IAssociativeReducer) From {reducers})
			Friend listAsJson As String = j.serializeReducerList(New List(Of IAssociativeReducer) From {reducers})

			'        System.out.println("\n\n\n\n");
			'        System.out.println(listAsYaml);

			Friend lFromYaml As IList(Of IAssociativeReducer) = y.deserializeReducerList(listAsYaml)
			Friend lFromJson As IList(Of IAssociativeReducer) = j.deserializeReducerList(listAsJson)

			assertEquals(java.util.Arrays.asList(reducers), y.deserializeReducerList(arrAsYaml))
			assertEquals(java.util.Arrays.asList(reducers), j.deserializeReducerList(arrAsJson))
			assertEquals(java.util.Arrays.asList(reducers), lFromYaml)
			assertEquals(java.util.Arrays.asList(reducers), lFromJson)
	End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSequenceComparator()
		public void testSequenceComparator()
		If True Then
			Dim comparators() As SequenceComparator = {
				New NumericalColumnComparator("Col", True),
				New StringComparator("Col")
			}

			For Each f As SequenceComparator In comparators
				Dim yaml As String = y.serialize(f)
				Dim json As String = j.serialize(f)

				'            System.out.println(yaml);
				'            System.out.println(json);
				'            System.out.println();

				Dim t2 As SequenceComparator = y.deserializeSequenceComparator(yaml)
				Dim t3 As SequenceComparator = j.deserializeSequenceComparator(json)
				assertEquals(f, t2)
				assertEquals(f, t3)
			Next f

			Dim arrAsYaml As String = y.serialize(comparators)
			Dim arrAsJson As String = j.serialize(comparators)
			Dim listAsYaml As String = y.serializeSequenceComparatorList(java.util.Arrays.asList(comparators))
			Dim listAsJson As String = j.serializeSequenceComparatorList(java.util.Arrays.asList(comparators))

			'        System.out.println("\n\n\n\n");
			'        System.out.println(listAsYaml);

			Dim lFromYaml As IList(Of SequenceComparator) = y.deserializeSequenceComparatorList(listAsYaml)
			Dim lFromJson As IList(Of SequenceComparator) = j.deserializeSequenceComparatorList(listAsJson)

			assertEquals(java.util.Arrays.asList(comparators), y.deserializeSequenceComparatorList(arrAsYaml))
			assertEquals(java.util.Arrays.asList(comparators), j.deserializeSequenceComparatorList(arrAsJson))
			assertEquals(java.util.Arrays.asList(comparators), lFromYaml)
			assertEquals(java.util.Arrays.asList(comparators), lFromJson)
		End If

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCalculateSortedRank()
		public void testCalculateSortedRank()
		If True Then
			Dim rank As New CalculateSortedRank("RankCol", "SortOnCol", New DoubleWritableComparator())

			Dim asYaml As String = y.serialize(rank)
			Dim asJson As String = j.serialize(rank)

			Dim yRank As CalculateSortedRank = y.deserializeSortedRank(asYaml)
			Dim jRank As CalculateSortedRank = j.deserializeSortedRank(asJson)

			assertEquals(rank, yRank)
			assertEquals(rank, jRank)
		End If

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSequenceSplit()
		public void testSequenceSplit()
		If True Then
			Dim splits() As SequenceSplit = {
				New org.datavec.api.transform.sequence.Split.SequenceSplitTimeSeparation("col", 1, TimeUnit.HOURS),
				New org.datavec.api.transform.sequence.Split.SplitMaxLengthSequence(100, False)
			}

			For Each f As SequenceSplit In splits
				Dim yaml As String = y.serialize(f)
				Dim json As String = j.serialize(f)

				'            System.out.println(yaml);
				'            System.out.println(json);
				'            System.out.println();

				Dim t2 As SequenceSplit = y.deserializeSequenceSplit(yaml)
				Dim t3 As SequenceSplit = j.deserializeSequenceSplit(json)
				assertEquals(f, t2)
				assertEquals(f, t3)
			Next f
		End If


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDataAction()
		public void testDataAction()
		If True Then
			Dim dataActions() As DataAction = {
				New DataAction(New CategoricalToIntegerTransform("Col")),
				New DataAction(New ConditionFilter(New DoubleColumnCondition("Col", ConditionOp.Equal, 1))),
				New DataAction(New ConvertToSequence("KeyCol", New NumericalColumnComparator("Col", True))),
				New DataAction(New ConvertFromSequence()),
				New DataAction(New org.datavec.api.transform.sequence.Split.SequenceSplitTimeSeparation("TimeCol", 1, TimeUnit.HOURS)),
				New DataAction((New Reducer.Builder(ReduceOp.TakeFirst)).build()),
				New DataAction(New CalculateSortedRank("NewCol", "SortCol", New DoubleWritableComparator()))
			}

			For Each f As DataAction In dataActions
				Dim yaml As String = y.serialize(f)
				Dim json As String = j.serialize(f)

				'            System.out.println(yaml);
				'            System.out.println(json);
				'            System.out.println();

				Dim t2 As DataAction = y.deserializeDataAction(yaml)
				Dim t3 As DataAction = j.deserializeDataAction(json)
				assertEquals(f, t2)
				assertEquals(f, t3)
			Next f

			Dim arrAsYaml As String = y.serialize(dataActions)
			Dim arrAsJson As String = j.serialize(dataActions)
			Dim listAsYaml As String = y.serializeDataActionList(java.util.Arrays.asList(dataActions))
			Dim listAsJson As String = j.serializeDataActionList(java.util.Arrays.asList(dataActions))

			'        System.out.println("\n\n\n\n");
			'        System.out.println(listAsYaml);

			Dim lFromYaml As IList(Of DataAction) = y.deserializeDataActionList(listAsYaml)
			Dim lFromJson As IList(Of DataAction) = j.deserializeDataActionList(listAsJson)

			assertEquals(java.util.Arrays.asList(dataActions), y.deserializeDataActionList(arrAsYaml))
			assertEquals(java.util.Arrays.asList(dataActions), j.deserializeDataActionList(arrAsJson))
			assertEquals(java.util.Arrays.asList(dataActions), lFromYaml)
			assertEquals(java.util.Arrays.asList(dataActions), lFromJson)
		End If
	}

End Namespace