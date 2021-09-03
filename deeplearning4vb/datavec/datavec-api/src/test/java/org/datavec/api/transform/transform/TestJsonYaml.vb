Imports System.Collections.Generic
Imports org.datavec.api.transform
Imports DataAnalysis = org.datavec.api.transform.analysis.DataAnalysis
Imports CategoricalAnalysis = org.datavec.api.transform.analysis.columns.CategoricalAnalysis
Imports DoubleAnalysis = org.datavec.api.transform.analysis.columns.DoubleAnalysis
Imports StringAnalysis = org.datavec.api.transform.analysis.columns.StringAnalysis
Imports ConditionOp = org.datavec.api.transform.condition.ConditionOp
Imports DoubleColumnCondition = org.datavec.api.transform.condition.column.DoubleColumnCondition
Imports NullWritableColumnCondition = org.datavec.api.transform.condition.column.NullWritableColumnCondition
Imports SequenceLengthCondition = org.datavec.api.transform.condition.sequence.SequenceLengthCondition
Imports ConditionFilter = org.datavec.api.transform.filter.ConditionFilter
Imports FilterInvalidValues = org.datavec.api.transform.filter.FilterInvalidValues
Imports Reducer = org.datavec.api.transform.reduce.Reducer
Imports Schema = org.datavec.api.transform.schema.Schema
Imports NumericalColumnComparator = org.datavec.api.transform.sequence.comparator.NumericalColumnComparator
Imports StringComparator = org.datavec.api.transform.sequence.comparator.StringComparator
Imports SequenceSplitTimeSeparation = org.datavec.api.transform.sequence.split.SequenceSplitTimeSeparation
Imports OverlappingTimeWindowFunction = org.datavec.api.transform.sequence.window.OverlappingTimeWindowFunction
Imports ReplaceEmptyIntegerWithValueTransform = org.datavec.api.transform.transform.integer.ReplaceEmptyIntegerWithValueTransform
Imports ReplaceInvalidWithIntegerTransform = org.datavec.api.transform.transform.integer.ReplaceInvalidWithIntegerTransform
Imports SequenceOffsetTransform = org.datavec.api.transform.transform.sequence.SequenceOffsetTransform
Imports MapAllStringsExceptListTransform = org.datavec.api.transform.transform.string.MapAllStringsExceptListTransform
Imports ReplaceEmptyStringTransform = org.datavec.api.transform.transform.string.ReplaceEmptyStringTransform
Imports StringListToCategoricalSetTransform = org.datavec.api.transform.transform.string.StringListToCategoricalSetTransform
Imports DeriveColumnsFromTimeTransform = org.datavec.api.transform.transform.time.DeriveColumnsFromTimeTransform
Imports DoubleWritable = org.datavec.api.writable.DoubleWritable
Imports IntWritable = org.datavec.api.writable.IntWritable
Imports Text = org.datavec.api.writable.Text
Imports LongWritableComparator = org.datavec.api.writable.comparator.LongWritableComparator
Imports DateTimeFieldType = org.joda.time.DateTimeFieldType
Imports DateTimeZone = org.joda.time.DateTimeZone
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
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

Namespace org.datavec.api.transform.transform


	Public Class TestJsonYaml
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testToFromJsonYaml()
		Public Overridable Sub testToFromJsonYaml()

			Dim schema As Schema = (New Schema.Builder()).addColumnCategorical("Cat", "State1", "State2").addColumnCategorical("Cat2", "State1", "State2").addColumnDouble("Dbl").addColumnDouble("Dbl2", Nothing, 100.0, True, False).addColumnInteger("Int").addColumnInteger("Int2", 0, 10).addColumnLong("Long").addColumnLong("Long2", -100L, Nothing).addColumnString("Str").addColumnString("Str2", "someregexhere", 1, Nothing).addColumnString("Str3").addColumnTime("TimeCol", DateTimeZone.UTC).addColumnTime("TimeCol2", DateTimeZone.UTC, Nothing, 1000L).build()

			Dim map As IDictionary(Of String, String) = New Dictionary(Of String, String)()
			map("from") = "to"
			map("anotherFrom") = "anotherTo"

			Dim tp As TransformProcess = (New TransformProcess.Builder(schema)).categoricalToInteger("Cat").categoricalToOneHot("Cat2").appendStringColumnTransform("Str3", "ToAppend").integerToCategorical("Cat", java.util.Arrays.asList("State1", "State2")).stringToCategorical("Str", java.util.Arrays.asList("State1", "State2")).duplicateColumn("Str", "Str2a").removeColumns("Str2a").renameColumn("Str2", "Str2a").reorderColumns("Cat", "Dbl").conditionalCopyValueTransform("Dbl", "Dbl2", New DoubleColumnCondition("Dbl", ConditionOp.Equal, 0.0)).conditionalReplaceValueTransform("Dbl", New DoubleWritable(1.0), New DoubleColumnCondition("Dbl", ConditionOp.Equal, 1.0)).doubleColumnsMathOp("NewDouble", MathOp.Add, "Dbl", "Dbl2").doubleMathOp("Dbl", MathOp.Add, 1.0).integerColumnsMathOp("NewInt", MathOp.Subtract, "Int", "Int2").integerMathOp("Int", MathOp.Multiply, 2).transform(New ReplaceEmptyIntegerWithValueTransform("Int", 1)).transform(New ReplaceInvalidWithIntegerTransform("Int", 1)).longColumnsMathOp("Long", MathOp.Multiply, "Long", "Long2").longMathOp("Long", MathOp.ScalarMax, 0).transform(New MapAllStringsExceptListTransform("Str", "Other", New List(Of String) From {"Ok", "SomeVal"})).stringRemoveWhitespaceTransform("Str").transform(New ReplaceEmptyStringTransform("Str", "WasEmpty")).replaceStringTransform("Str", map).transform(New StringListToCategoricalSetTransform("Str", New List(Of String) From {"StrA", "StrB"}, New List(Of String) From {"StrA", "StrB"}, ",")).stringMapTransform("Str2a", map).transform((New DeriveColumnsFromTimeTransform.Builder("TimeCol")).addIntegerDerivedColumn("Hour", DateTimeFieldType.hourOfDay()).addStringDerivedColumn("Date", "YYYY-MM-dd", DateTimeZone.UTC).build()).stringToTimeTransform("Str2a", "YYYY-MM-dd hh:mm:ss", DateTimeZone.UTC).timeMathOp("TimeCol2", MathOp.Add, 1, TimeUnit.HOURS).filter(New FilterInvalidValues("Cat", "Str2a")).filter(New ConditionFilter(New NullWritableColumnCondition("Long"))).convertToSequence("Int", New NumericalColumnComparator("TimeCol2")).convertFromSequence().convertToSequence("Int", New StringComparator("Str2a")).splitSequence(New org.datavec.api.transform.sequence.Split.SequenceSplitTimeSeparation("TimeCol2", 1, TimeUnit.HOURS)).reduce((New Reducer.Builder(ReduceOp.TakeFirst)).keyColumns("TimeCol2").countColumns("Cat").sumColumns("Dbl").build()).reduceSequenceByWindow((New Reducer.Builder(ReduceOp.TakeFirst)).countColumns("Cat2").stdevColumns("Dbl2").build(), (New OverlappingTimeWindowFunction.Builder()).timeColumn("TimeCol2").addWindowStartTimeColumn(True).addWindowEndTimeColumn(True).windowSize(1, TimeUnit.HOURS).offset(5, TimeUnit.MINUTES).windowSeparation(15, TimeUnit.MINUTES).excludeEmptyWindows(True).build()).convertFromSequence().calculateSortedRank("rankColName", "TimeCol2", New LongWritableComparator()).sequenceMovingWindowReduce("rankColName", 20, ReduceOp.Mean).addConstantColumn("someIntColumn", ColumnType.Integer, New IntWritable(0)).integerToOneHot("someIntColumn", 0, 3).filter(New SequenceLengthCondition(ConditionOp.LessThan, 1)).addConstantColumn("testColSeq", ColumnType.Integer, New DoubleWritable(0)).offsetSequence(Collections.singletonList("testColSeq"), 1, SequenceOffsetTransform.OperationType.InPlace).addConstantColumn("someTextCol", ColumnType.String, New Text("some values")).addConstantColumn("testFirstDigit", ColumnType.Double, New DoubleWritable(0)).firstDigitTransform("testFirstDigit", "testFirstDigitOut").build()

			Dim asJson As String = tp.toJson()
			Dim asYaml As String = tp.toYaml()

	'                System.out.println(asJson);
			'        System.out.println("\n\n\n");
			'        System.out.println(asYaml);


			Dim tpFromJson As TransformProcess = TransformProcess.fromJson(asJson)
			Dim tpFromYaml As TransformProcess = TransformProcess.fromYaml(asYaml)

			Dim daList As IList(Of DataAction) = tp.getActionList()
			Dim daListJson As IList(Of DataAction) = tpFromJson.getActionList()
			Dim daListYaml As IList(Of DataAction) = tpFromYaml.getActionList()

			For i As Integer = 0 To daList.Count - 1
				Dim da1 As DataAction = daList(i)
				Dim da2 As DataAction = daListJson(i)
				Dim da3 As DataAction = daListYaml(i)

	'            System.out.println(i + "\t" + da1);

				assertEquals(da1, da2)
				assertEquals(da1, da3)
			Next i

			assertEquals(tp, tpFromJson)
			assertEquals(tp, tpFromYaml)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testJsonYamlAnalysis() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testJsonYamlAnalysis()
			Dim s As Schema = (New Schema.Builder()).addColumnsDouble("first", "second").addColumnString("third").addColumnCategorical("fourth", "cat0", "cat1").build()

			Dim d1 As DoubleAnalysis = (New DoubleAnalysis.Builder()).max(-1).max(1).countPositive(10).mean(3.0).build()
			Dim d2 As DoubleAnalysis = (New DoubleAnalysis.Builder()).max(-5).max(5).countPositive(4).mean(2.0).build()
			Dim sa As StringAnalysis = (New StringAnalysis.Builder()).minLength(0).maxLength(10).build()
			Dim countMap As IDictionary(Of String, Long) = New Dictionary(Of String, Long)()
			countMap("cat0") = 100L
			countMap("cat1") = 200L
			Dim ca As New CategoricalAnalysis(countMap)

			Dim da As New DataAnalysis(s, java.util.Arrays.asList(d1, d2, sa, ca))

			Dim strJson As String = da.toJson()
			Dim strYaml As String = da.toYaml()
			'        System.out.println(str);

			Dim daFromJson As DataAnalysis = DataAnalysis.fromJson(strJson)
			Dim daFromYaml As DataAnalysis = DataAnalysis.fromYaml(strYaml)
			'        System.out.println(da2);

			assertEquals(da.getColumnAnalysis(), daFromJson.getColumnAnalysis())
			assertEquals(da.getColumnAnalysis(), daFromYaml.getColumnAnalysis())
		End Sub

	End Class

End Namespace