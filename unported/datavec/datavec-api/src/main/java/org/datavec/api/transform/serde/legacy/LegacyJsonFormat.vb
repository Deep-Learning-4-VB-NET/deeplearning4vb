Imports AccessLevel = lombok.AccessLevel
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports Transform = org.datavec.api.transform.Transform
Imports org.datavec.api.transform.analysis.columns
Imports BooleanCondition = org.datavec.api.transform.condition.BooleanCondition
Imports Condition = org.datavec.api.transform.condition.Condition
Imports org.datavec.api.transform.condition.column
Imports SequenceLengthCondition = org.datavec.api.transform.condition.sequence.SequenceLengthCondition
Imports StringRegexColumnCondition = org.datavec.api.transform.condition.string.StringRegexColumnCondition
Imports ConditionFilter = org.datavec.api.transform.filter.ConditionFilter
Imports Filter = org.datavec.api.transform.filter.Filter
Imports FilterInvalidValues = org.datavec.api.transform.filter.FilterInvalidValues
Imports InvalidNumColumns = org.datavec.api.transform.filter.InvalidNumColumns
Imports org.datavec.api.transform.metadata
Imports NDArrayColumnsMathOpTransform = org.datavec.api.transform.ndarray.NDArrayColumnsMathOpTransform
Imports NDArrayDistanceTransform = org.datavec.api.transform.ndarray.NDArrayDistanceTransform
Imports NDArrayMathFunctionTransform = org.datavec.api.transform.ndarray.NDArrayMathFunctionTransform
Imports NDArrayScalarOpTransform = org.datavec.api.transform.ndarray.NDArrayScalarOpTransform
Imports CalculateSortedRank = org.datavec.api.transform.rank.CalculateSortedRank
Imports Schema = org.datavec.api.transform.schema.Schema
Imports SequenceSchema = org.datavec.api.transform.schema.SequenceSchema
Imports ReduceSequenceTransform = org.datavec.api.transform.sequence.ReduceSequenceTransform
Imports SequenceComparator = org.datavec.api.transform.sequence.SequenceComparator
Imports SequenceSplit = org.datavec.api.transform.sequence.SequenceSplit
Imports NumericalColumnComparator = org.datavec.api.transform.sequence.comparator.NumericalColumnComparator
Imports StringComparator = org.datavec.api.transform.sequence.comparator.StringComparator
Imports SequenceSplitTimeSeparation = org.datavec.api.transform.sequence.split.SequenceSplitTimeSeparation
Imports SplitMaxLengthSequence = org.datavec.api.transform.sequence.split.SplitMaxLengthSequence
Imports SequenceTrimTransform = org.datavec.api.transform.sequence.trim.SequenceTrimTransform
Imports OverlappingTimeWindowFunction = org.datavec.api.transform.sequence.window.OverlappingTimeWindowFunction
Imports ReduceSequenceByWindowTransform = org.datavec.api.transform.sequence.window.ReduceSequenceByWindowTransform
Imports TimeWindowFunction = org.datavec.api.transform.sequence.window.TimeWindowFunction
Imports WindowFunction = org.datavec.api.transform.sequence.window.WindowFunction
Imports IStringReducer = org.datavec.api.transform.stringreduce.IStringReducer
Imports StringReducer = org.datavec.api.transform.stringreduce.StringReducer
Imports org.datavec.api.transform.transform.categorical
Imports org.datavec.api.transform.transform.column
Imports ConditionalCopyValueTransform = org.datavec.api.transform.transform.condition.ConditionalCopyValueTransform
Imports ConditionalReplaceValueTransform = org.datavec.api.transform.transform.condition.ConditionalReplaceValueTransform
Imports ConditionalReplaceValueTransformWithDefault = org.datavec.api.transform.transform.condition.ConditionalReplaceValueTransformWithDefault
Imports org.datavec.api.transform.transform.doubletransform
Imports org.datavec.api.transform.transform.integer
Imports LongColumnsMathOpTransform = org.datavec.api.transform.transform.longtransform.LongColumnsMathOpTransform
Imports LongMathOpTransform = org.datavec.api.transform.transform.longtransform.LongMathOpTransform
Imports TextToCharacterIndexTransform = org.datavec.api.transform.transform.nlp.TextToCharacterIndexTransform
Imports ParseDoubleTransform = org.datavec.api.transform.transform.parse.ParseDoubleTransform
Imports SequenceDifferenceTransform = org.datavec.api.transform.transform.sequence.SequenceDifferenceTransform
Imports SequenceMovingWindowReduceTransform = org.datavec.api.transform.transform.sequence.SequenceMovingWindowReduceTransform
Imports SequenceOffsetTransform = org.datavec.api.transform.transform.sequence.SequenceOffsetTransform
Imports org.datavec.api.transform.transform.string
Imports DeriveColumnsFromTimeTransform = org.datavec.api.transform.transform.time.DeriveColumnsFromTimeTransform
Imports StringToTimeTransform = org.datavec.api.transform.transform.time.StringToTimeTransform
Imports TimeMathOpTransform = org.datavec.api.transform.transform.time.TimeMathOpTransform
Imports org.datavec.api.writable
Imports org.datavec.api.writable.comparator
Imports JsonInclude = org.nd4j.shade.jackson.annotation.JsonInclude
Imports JsonSubTypes = org.nd4j.shade.jackson.annotation.JsonSubTypes
Imports JsonTypeInfo = org.nd4j.shade.jackson.annotation.JsonTypeInfo
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper

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

Namespace org.datavec.api.transform.serde.legacy

	Public Class LegacyJsonFormat

		Private Sub New()
		End Sub

		''' <summary>
		''' Get a mapper (minus general config) suitable for loading old format JSON - 1.0.0-alpha and before </summary>
		''' <returns> Object mapper </returns>
		Public Shared Function legacyMapper() As ObjectMapper
			Dim om As New ObjectMapper()
			om.addMixIn(GetType(Schema), GetType(SchemaMixin))
			om.addMixIn(GetType(ColumnMetaData), GetType(ColumnMetaDataMixin))
			om.addMixIn(GetType(Transform), GetType(TransformMixin))
			om.addMixIn(GetType(Condition), GetType(ConditionMixin))
			om.addMixIn(GetType(Writable), GetType(WritableMixin))
			om.addMixIn(GetType(Filter), GetType(FilterMixin))
			om.addMixIn(GetType(SequenceComparator), GetType(SequenceComparatorMixin))
			om.addMixIn(GetType(SequenceSplit), GetType(SequenceSplitMixin))
			om.addMixIn(GetType(WindowFunction), GetType(WindowFunctionMixin))
			om.addMixIn(GetType(CalculateSortedRank), GetType(CalculateSortedRankMixin))
			om.addMixIn(GetType(WritableComparator), GetType(WritableComparatorMixin))
			om.addMixIn(GetType(ColumnAnalysis), GetType(ColumnAnalysisMixin))
			om.addMixIn(GetType(IStringReducer), GetType(IStringReducerMixin))
			Return om
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonTypeInfo(use = JsonTypeInfo.Id.NAME, include = JsonTypeInfo.@As.WRAPPER_OBJECT) @JsonSubTypes({@JsonSubTypes.Type(value = Schema.class, name = "Schema"), @JsonSubTypes.Type(value = SequenceSchema.class, name = "SequenceSchema")}) @NoArgsConstructor(access = AccessLevel.@PRIVATE) public static class SchemaMixin
		Public Class SchemaMixin
		End Class
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonTypeInfo(use = JsonTypeInfo.Id.NAME, include = JsonTypeInfo.@As.WRAPPER_OBJECT) @JsonSubTypes({@JsonSubTypes.Type(value = BinaryMetaData.class, name = "Binary"), @JsonSubTypes.Type(value = BooleanMetaData.class, name = "Boloean"), @JsonSubTypes.Type(value = CategoricalMetaData.class, name = "Categorical"), @JsonSubTypes.Type(value = DoubleMetaData.class, name = "Double"), @JsonSubTypes.Type(value = FloatMetaData.class, name = "Float"), @JsonSubTypes.Type(value = IntegerMetaData.class, name = "Integer"), @JsonSubTypes.Type(value = LongMetaData.class, name = "Long"), @JsonSubTypes.Type(value = NDArrayMetaData.class, name = "NDArray"), @JsonSubTypes.Type(value = StringMetaData.class, name = "String"), @JsonSubTypes.Type(value = TimeMetaData.class, name = "Time")}) @NoArgsConstructor(access = AccessLevel.@PRIVATE) public static class ColumnMetaDataMixin
		Public Class ColumnMetaDataMixin
		End Class
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonTypeInfo(use = JsonTypeInfo.Id.NAME, include = JsonTypeInfo.@As.WRAPPER_OBJECT) @JsonSubTypes(value = {@JsonSubTypes.Type(value = CalculateSortedRank.class, name = "CalculateSortedRank"), @JsonSubTypes.Type(value = CategoricalToIntegerTransform.class, name = "CategoricalToIntegerTransform"), @JsonSubTypes.Type(value = CategoricalToOneHotTransform.class, name = "CategoricalToOneHotTransform"), @JsonSubTypes.Type(value = IntegerToCategoricalTransform.class, name = "IntegerToCategoricalTransform"), @JsonSubTypes.Type(value = StringToCategoricalTransform.class, name = "StringToCategoricalTransform"), @JsonSubTypes.Type(value = DuplicateColumnsTransform.class, name = "DuplicateColumnsTransform"), @JsonSubTypes.Type(value = RemoveColumnsTransform.class, name = "RemoveColumnsTransform"), @JsonSubTypes.Type(value = RenameColumnsTransform.class, name = "RenameColumnsTransform"), @JsonSubTypes.Type(value = ReorderColumnsTransform.class, name = "ReorderColumnsTransform"), @JsonSubTypes.Type(value = ConditionalCopyValueTransform.class, name = "ConditionalCopyValueTransform"), @JsonSubTypes.Type(value = ConditionalReplaceValueTransform.class, name = "ConditionalReplaceValueTransform"), @JsonSubTypes.Type(value = ConditionalReplaceValueTransformWithDefault.class, name = "ConditionalReplaceValueTransformWithDefault"), @JsonSubTypes.Type(value = DoubleColumnsMathOpTransform.class, name = "DoubleColumnsMathOpTransform"), @JsonSubTypes.Type(value = DoubleMathOpTransform.class, name = "DoubleMathOpTransform"), @JsonSubTypes.Type(value = Log2Normalizer.class, name = "Log2Normalizer"), @JsonSubTypes.Type(value = MinMaxNormalizer.class, name = "MinMaxNormalizer"), @JsonSubTypes.Type(value = StandardizeNormalizer.class, name = "StandardizeNormalizer"), @JsonSubTypes.Type(value = SubtractMeanNormalizer.class, name = "SubtractMeanNormalizer"), @JsonSubTypes.Type(value = IntegerColumnsMathOpTransform.class, name = "IntegerColumnsMathOpTransform"), @JsonSubTypes.Type(value = IntegerMathOpTransform.class, name = "IntegerMathOpTransform"), @JsonSubTypes.Type(value = ReplaceEmptyIntegerWithValueTransform.class, name = "ReplaceEmptyIntegerWithValueTransform"), @JsonSubTypes.Type(value = ReplaceInvalidWithIntegerTransform.class, name = "ReplaceInvalidWithIntegerTransform"), @JsonSubTypes.Type(value = LongColumnsMathOpTransform.class, name = "LongColumnsMathOpTransform"), @JsonSubTypes.Type(value = LongMathOpTransform.class, name = "LongMathOpTransform"), @JsonSubTypes.Type(value = MapAllStringsExceptListTransform.class, name = "MapAllStringsExceptListTransform"), @JsonSubTypes.Type(value = RemoveWhiteSpaceTransform.class, name = "RemoveWhiteSpaceTransform"), @JsonSubTypes.Type(value = ReplaceEmptyStringTransform.class, name = "ReplaceEmptyStringTransform"), @JsonSubTypes.Type(value = ReplaceStringTransform.class, name = "ReplaceStringTransform"), @JsonSubTypes.Type(value = StringListToCategoricalSetTransform.class, name = "StringListToCategoricalSetTransform"), @JsonSubTypes.Type(value = StringMapTransform.class, name = "StringMapTransform"), @JsonSubTypes.Type(value = DeriveColumnsFromTimeTransform.class, name = "DeriveColumnsFromTimeTransform"), @JsonSubTypes.Type(value = StringToTimeTransform.class, name = "StringToTimeTransform"), @JsonSubTypes.Type(value = TimeMathOpTransform.class, name = "TimeMathOpTransform"), @JsonSubTypes.Type(value = ReduceSequenceByWindowTransform.class, name = "ReduceSequenceByWindowTransform"), @JsonSubTypes.Type(value = DoubleMathFunctionTransform.class, name = "DoubleMathFunctionTransform"), @JsonSubTypes.Type(value = AddConstantColumnTransform.class, name = "AddConstantColumnTransform"), @JsonSubTypes.Type(value = RemoveAllColumnsExceptForTransform.class, name = "RemoveAllColumnsExceptForTransform"), @JsonSubTypes.Type(value = ParseDoubleTransform.class, name = "ParseDoubleTransform"), @JsonSubTypes.Type(value = ConvertToString.class, name = "ConvertToStringTransform"), @JsonSubTypes.Type(value = AppendStringColumnTransform.class, name = "AppendStringColumnTransform"), @JsonSubTypes.Type(value = SequenceDifferenceTransform.class, name = "SequenceDifferenceTransform"), @JsonSubTypes.Type(value = ReduceSequenceTransform.class, name = "ReduceSequenceTransform"), @JsonSubTypes.Type(value = SequenceMovingWindowReduceTransform.class, name = "SequenceMovingWindowReduceTransform"), @JsonSubTypes.Type(value = IntegerToOneHotTransform.class, name = "IntegerToOneHotTransform"), @JsonSubTypes.Type(value = SequenceTrimTransform.class, name = "SequenceTrimTransform"), @JsonSubTypes.Type(value = SequenceOffsetTransform.class, name = "SequenceOffsetTransform"), @JsonSubTypes.Type(value = NDArrayColumnsMathOpTransform.class, name = "NDArrayColumnsMathOpTransform"), @JsonSubTypes.Type(value = NDArrayDistanceTransform.class, name = "NDArrayDistanceTransform"), @JsonSubTypes.Type(value = NDArrayMathFunctionTransform.class, name = "NDArrayMathFunctionTransform"), @JsonSubTypes.Type(value = NDArrayScalarOpTransform.class, name = "NDArrayScalarOpTransform"), @JsonSubTypes.Type(value = ChangeCaseStringTransform.class, name = "ChangeCaseStringTransform"), @JsonSubTypes.Type(value = ConcatenateStringColumns.class, name = "ConcatenateStringColumns"), @JsonSubTypes.Type(value = StringListToCountsNDArrayTransform.class, name = "StringListToCountsNDArrayTransform"), @JsonSubTypes.Type(value = StringListToIndicesNDArrayTransform.class, name = "StringListToIndicesNDArrayTransform"), @JsonSubTypes.Type(value = PivotTransform.class, name = "PivotTransform"), @JsonSubTypes.Type(value = TextToCharacterIndexTransform.class, name = "TextToCharacterIndexTransform")}) @NoArgsConstructor(access = AccessLevel.@PRIVATE) public static class TransformMixin
		Public Class TransformMixin
		End Class
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonTypeInfo(use = JsonTypeInfo.Id.NAME, include = JsonTypeInfo.@As.WRAPPER_OBJECT) @JsonSubTypes(value = {@JsonSubTypes.Type(value = TrivialColumnCondition.class, name = "TrivialColumnCondition"), @JsonSubTypes.Type(value = CategoricalColumnCondition.class, name = "CategoricalColumnCondition"), @JsonSubTypes.Type(value = DoubleColumnCondition.class, name = "DoubleColumnCondition"), @JsonSubTypes.Type(value = IntegerColumnCondition.class, name = "IntegerColumnCondition"), @JsonSubTypes.Type(value = LongColumnCondition.class, name = "LongColumnCondition"), @JsonSubTypes.Type(value = NullWritableColumnCondition.class, name = "NullWritableColumnCondition"), @JsonSubTypes.Type(value = StringColumnCondition.class, name = "StringColumnCondition"), @JsonSubTypes.Type(value = TimeColumnCondition.class, name = "TimeColumnCondition"), @JsonSubTypes.Type(value = StringRegexColumnCondition.class, name = "StringRegexColumnCondition"), @JsonSubTypes.Type(value = BooleanCondition.class, name = "BooleanCondition"), @JsonSubTypes.Type(value = NaNColumnCondition.class, name = "NaNColumnCondition"), @JsonSubTypes.Type(value = InfiniteColumnCondition.class, name = "InfiniteColumnCondition"), @JsonSubTypes.Type(value = SequenceLengthCondition.class, name = "SequenceLengthCondition")}) @NoArgsConstructor(access = AccessLevel.@PRIVATE) public static class ConditionMixin
		Public Class ConditionMixin
		End Class
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonTypeInfo(use = JsonTypeInfo.Id.NAME, include = JsonTypeInfo.@As.WRAPPER_OBJECT) @JsonSubTypes(value = {@JsonSubTypes.Type(value = ArrayWritable.class, name = "ArrayWritable"), @JsonSubTypes.Type(value = BooleanWritable.class, name = "BooleanWritable"), @JsonSubTypes.Type(value = ByteWritable.class, name = "ByteWritable"), @JsonSubTypes.Type(value = DoubleWritable.class, name = "DoubleWritable"), @JsonSubTypes.Type(value = FloatWritable.class, name = "FloatWritable"), @JsonSubTypes.Type(value = IntWritable.class, name = "IntWritable"), @JsonSubTypes.Type(value = LongWritable.class, name = "LongWritable"), @JsonSubTypes.Type(value = NullWritable.class, name = "NullWritable"), @JsonSubTypes.Type(value = Text.class, name = "Text"), @JsonSubTypes.Type(value = BytesWritable.class, name = "BytesWritable")}) @NoArgsConstructor(access = AccessLevel.@PRIVATE) public static class WritableMixin
		Public Class WritableMixin
		End Class
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonInclude(JsonInclude.Include.NON_NULL) @JsonTypeInfo(use = JsonTypeInfo.Id.NAME, include = JsonTypeInfo.@As.WRAPPER_OBJECT) @JsonSubTypes(value = {@JsonSubTypes.Type(value = ConditionFilter.class, name = "ConditionFilter"), @JsonSubTypes.Type(value = FilterInvalidValues.class, name = "FilterInvalidValues"), @JsonSubTypes.Type(value = InvalidNumColumns.class, name = "InvalidNumCols")}) @NoArgsConstructor(access = AccessLevel.@PRIVATE) public static class FilterMixin
		Public Class FilterMixin
		End Class
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonInclude(JsonInclude.Include.NON_NULL) @JsonTypeInfo(use = JsonTypeInfo.Id.NAME, include = JsonTypeInfo.@As.WRAPPER_OBJECT) @JsonSubTypes(value = {@JsonSubTypes.Type(value = NumericalColumnComparator.class, name = "NumericalColumnComparator"), @JsonSubTypes.Type(value = StringComparator.class, name = "StringComparator")}) @NoArgsConstructor(access = AccessLevel.@PRIVATE) public static class SequenceComparatorMixin
		Public Class SequenceComparatorMixin
		End Class
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonInclude(JsonInclude.Include.NON_NULL) @JsonTypeInfo(use = JsonTypeInfo.Id.NAME, include = JsonTypeInfo.@As.WRAPPER_OBJECT) @JsonSubTypes(value = {@JsonSubTypes.Type(value = SequenceSplitTimeSeparation.class, name = "SequenceSplitTimeSeparation"), @JsonSubTypes.Type(value = SplitMaxLengthSequence.class, name = "SplitMaxLengthSequence")}) @NoArgsConstructor(access = AccessLevel.@PRIVATE) public static class SequenceSplitMixin
		Public Class SequenceSplitMixin
		End Class
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonInclude(JsonInclude.Include.NON_NULL) @JsonTypeInfo(use = JsonTypeInfo.Id.NAME, include = JsonTypeInfo.@As.WRAPPER_OBJECT) @JsonSubTypes(value = {@JsonSubTypes.Type(value = TimeWindowFunction.class, name = "TimeWindowFunction"), @JsonSubTypes.Type(value = OverlappingTimeWindowFunction.class, name = "OverlappingTimeWindowFunction")}) @NoArgsConstructor(access = AccessLevel.@PRIVATE) public static class WindowFunctionMixin
		Public Class WindowFunctionMixin
		End Class
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonTypeInfo(use = JsonTypeInfo.Id.NAME, include = JsonTypeInfo.@As.WRAPPER_OBJECT) @JsonSubTypes(value = {@JsonSubTypes.Type(value = CalculateSortedRank.class, name = "CalculateSortedRank")}) @NoArgsConstructor(access = AccessLevel.@PRIVATE) public static class CalculateSortedRankMixin
		Public Class CalculateSortedRankMixin
		End Class
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonTypeInfo(use = JsonTypeInfo.Id.NAME, include = JsonTypeInfo.@As.WRAPPER_OBJECT) @JsonSubTypes(value = {@JsonSubTypes.Type(value = DoubleWritableComparator.class, name = "DoubleWritableComparator"), @JsonSubTypes.Type(value = FloatWritableComparator.class, name = "FloatWritableComparator"), @JsonSubTypes.Type(value = IntWritableComparator.class, name = "IntWritableComparator"), @JsonSubTypes.Type(value = LongWritableComparator.class, name = "LongWritableComparator"), @JsonSubTypes.Type(value = TextWritableComparator.class, name = "TextWritableComparator")}) @NoArgsConstructor(access = AccessLevel.@PRIVATE) public static class WritableComparatorMixin
		Public Class WritableComparatorMixin
		End Class
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonTypeInfo(use = JsonTypeInfo.Id.NAME, include = JsonTypeInfo.@As.WRAPPER_OBJECT) @JsonSubTypes(value = {@JsonSubTypes.Type(value = BytesAnalysis.class, name = "BytesAnalysis"), @JsonSubTypes.Type(value = CategoricalAnalysis.class, name = "CategoricalAnalysis"), @JsonSubTypes.Type(value = DoubleAnalysis.class, name = "DoubleAnalysis"), @JsonSubTypes.Type(value = IntegerAnalysis.class, name = "IntegerAnalysis"), @JsonSubTypes.Type(value = LongAnalysis.class, name = "LongAnalysis"), @JsonSubTypes.Type(value = StringAnalysis.class, name = "StringAnalysis"), @JsonSubTypes.Type(value = TimeAnalysis.class, name = "TimeAnalysis")}) @NoArgsConstructor(access = AccessLevel.@PRIVATE) public static class ColumnAnalysisMixin
		Public Class ColumnAnalysisMixin
		End Class
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonTypeInfo(use = JsonTypeInfo.Id.NAME, include = JsonTypeInfo.@As.WRAPPER_OBJECT) @JsonSubTypes(value = {@JsonSubTypes.Type(value = StringReducer.class, name = "StringReducer")}) @NoArgsConstructor(access = AccessLevel.@PRIVATE) public static class IStringReducerMixin
		Public Class IStringReducerMixin
		End Class
	End Class

End Namespace