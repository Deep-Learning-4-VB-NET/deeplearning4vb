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
Imports System.Linq
Imports System.Text.Json
Imports System.Text.Json.Nodes
Imports JsonMappers = DeepLearning4VB.org.datavec.api.transform.serde.JsonMappers
Imports TypeCatalog = DeepLearning4VB.org.datavec.api.transform.serde.TypeCatalog
Imports TransformT = DeepLearning4VB.org.datavec.api.transform.Transform
Imports ColumnAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.columns.ColumnAnalysis
Imports ConditionT = DeepLearning4VB.org.datavec.api.transform.condition.Condition
Imports FilterT = DeepLearning4VB.org.datavec.api.transform.filter.Filter
Imports ColumnMetaDataT = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports CalculateSortedRankT = DeepLearning4VB.org.datavec.api.transform.rank.CalculateSortedRank
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports SequenceComparatorT = DeepLearning4VB.org.datavec.api.transform.sequence.SequenceComparator
Imports SequenceSplitT = DeepLearning4VB.org.datavec.api.transform.sequence.SequenceSplit
Imports WindowFunctionT = DeepLearning4VB.org.datavec.api.transform.sequence.window.WindowFunction
Imports IStringReducerT = DeepLearning4VB.org.datavec.api.transform.stringreduce.IStringReducer
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable
Imports WritableComparatorT = DeepLearning4VB.org.datavec.api.writable.comparator.WritableComparator

Namespace org.datavec.api.transform.serde.legacy

    ''' <summary>
    ''' Reading of the JSON format of DataVec 1.0.0-alpha and earlier, where polymorphic values were written as wrapper
    ''' objects named after the subtype (<c>{"Schema" : {"columns" : [ {"Double" : {"name" : "x"}} ]}}</c>) instead of
    ''' carrying an <c>"@class"</c> property. The Java class registered Jackson mix-ins with these names; here the
    ''' same name tables drive <see cref="convertLegacyJson"/>, which rewrites such wrapper objects into the current
    ''' format before the JSON is read. It walks the tree together with the declared types and only rewrites a
    ''' single-property object without <c>"@class"</c> at a position of a polymorphic type whose property name is a
    ''' legacy name of that type, so current-format JSON passes through unchanged. Names of classes this library does
    ''' not have (the NDArray transforms and metadata) are not resolved.
    ''' </summary>
    Public NotInheritable Class LegacyJsonFormat

        Private Sub New()
        End Sub

        Private Shared ReadOnly tablesLock As New Object()
        Private Shared tables As List(Of KeyValuePair(Of Type, Dictionary(Of String, String)))

        ''' <summary>The legacy names per polymorphic base type (Java: the mix-in classes), mapped to Java class names.</summary>
        Private Shared Function legacyTables() As List(Of KeyValuePair(Of Type, Dictionary(Of String, String)))
            SyncLock tablesLock
                If tables IsNot Nothing Then Return tables
                Const T As String = "org.datavec.api.transform."
                Dim list As New List(Of KeyValuePair(Of Type, Dictionary(Of String, String)))()
                list.Add(table(GetType(SchemaT),
                    "Schema", T & "schema.Schema",
                    "SequenceSchema", T & "schema.SequenceSchema"))
                list.Add(table(GetType(ColumnMetaDataT),
                    "Binary", T & "metadata.BinaryMetaData",
                    "Boloean", T & "metadata.BooleanMetaData",
                    "Categorical", T & "metadata.CategoricalMetaData",
                    "Double", T & "metadata.DoubleMetaData",
                    "Float", T & "metadata.FloatMetaData",
                    "Integer", T & "metadata.IntegerMetaData",
                    "Long", T & "metadata.LongMetaData",
                    "NDArray", T & "metadata.NDArrayMetaData",
                    "String", T & "metadata.StringMetaData",
                    "Time", T & "metadata.TimeMetaData"))
                list.Add(table(GetType(TransformT),
                    "CalculateSortedRank", T & "rank.CalculateSortedRank",
                    "CategoricalToIntegerTransform", T & "transform.categorical.CategoricalToIntegerTransform",
                    "CategoricalToOneHotTransform", T & "transform.categorical.CategoricalToOneHotTransform",
                    "IntegerToCategoricalTransform", T & "transform.categorical.IntegerToCategoricalTransform",
                    "StringToCategoricalTransform", T & "transform.categorical.StringToCategoricalTransform",
                    "DuplicateColumnsTransform", T & "transform.column.DuplicateColumnsTransform",
                    "RemoveColumnsTransform", T & "transform.column.RemoveColumnsTransform",
                    "RenameColumnsTransform", T & "transform.column.RenameColumnsTransform",
                    "ReorderColumnsTransform", T & "transform.column.ReorderColumnsTransform",
                    "ConditionalCopyValueTransform", T & "transform.condition.ConditionalCopyValueTransform",
                    "ConditionalReplaceValueTransform", T & "transform.condition.ConditionalReplaceValueTransform",
                    "ConditionalReplaceValueTransformWithDefault", T & "transform.condition.ConditionalReplaceValueTransformWithDefault",
                    "DoubleColumnsMathOpTransform", T & "transform.doubletransform.DoubleColumnsMathOpTransform",
                    "DoubleMathOpTransform", T & "transform.doubletransform.DoubleMathOpTransform",
                    "Log2Normalizer", T & "transform.doubletransform.Log2Normalizer",
                    "MinMaxNormalizer", T & "transform.doubletransform.MinMaxNormalizer",
                    "StandardizeNormalizer", T & "transform.doubletransform.StandardizeNormalizer",
                    "SubtractMeanNormalizer", T & "transform.doubletransform.SubtractMeanNormalizer",
                    "IntegerColumnsMathOpTransform", T & "transform.integer.IntegerColumnsMathOpTransform",
                    "IntegerMathOpTransform", T & "transform.integer.IntegerMathOpTransform",
                    "ReplaceEmptyIntegerWithValueTransform", T & "transform.integer.ReplaceEmptyIntegerWithValueTransform",
                    "ReplaceInvalidWithIntegerTransform", T & "transform.integer.ReplaceInvalidWithIntegerTransform",
                    "LongColumnsMathOpTransform", T & "transform.longtransform.LongColumnsMathOpTransform",
                    "LongMathOpTransform", T & "transform.longtransform.LongMathOpTransform",
                    "MapAllStringsExceptListTransform", T & "transform.string.MapAllStringsExceptListTransform",
                    "RemoveWhiteSpaceTransform", T & "transform.string.RemoveWhiteSpaceTransform",
                    "ReplaceEmptyStringTransform", T & "transform.string.ReplaceEmptyStringTransform",
                    "ReplaceStringTransform", T & "transform.string.ReplaceStringTransform",
                    "StringListToCategoricalSetTransform", T & "transform.string.StringListToCategoricalSetTransform",
                    "StringMapTransform", T & "transform.string.StringMapTransform",
                    "DeriveColumnsFromTimeTransform", T & "transform.time.DeriveColumnsFromTimeTransform",
                    "StringToTimeTransform", T & "transform.time.StringToTimeTransform",
                    "TimeMathOpTransform", T & "transform.time.TimeMathOpTransform",
                    "ReduceSequenceByWindowTransform", T & "sequence.window.ReduceSequenceByWindowTransform",
                    "DoubleMathFunctionTransform", T & "transform.doubletransform.DoubleMathFunctionTransform",
                    "AddConstantColumnTransform", T & "transform.column.AddConstantColumnTransform",
                    "RemoveAllColumnsExceptForTransform", T & "transform.column.RemoveAllColumnsExceptForTransform",
                    "ParseDoubleTransform", T & "transform.parse.ParseDoubleTransform",
                    "ConvertToStringTransform", T & "transform.string.ConvertToString",
                    "AppendStringColumnTransform", T & "transform.string.AppendStringColumnTransform",
                    "SequenceDifferenceTransform", T & "transform.sequence.SequenceDifferenceTransform",
                    "ReduceSequenceTransform", T & "sequence.ReduceSequenceTransform",
                    "SequenceMovingWindowReduceTransform", T & "transform.sequence.SequenceMovingWindowReduceTransform",
                    "IntegerToOneHotTransform", T & "transform.integer.IntegerToOneHotTransform",
                    "SequenceTrimTransform", T & "sequence.trim.SequenceTrimTransform",
                    "SequenceOffsetTransform", T & "transform.sequence.SequenceOffsetTransform",
                    "NDArrayColumnsMathOpTransform", T & "ndarray.NDArrayColumnsMathOpTransform",
                    "NDArrayDistanceTransform", T & "ndarray.NDArrayDistanceTransform",
                    "NDArrayMathFunctionTransform", T & "ndarray.NDArrayMathFunctionTransform",
                    "NDArrayScalarOpTransform", T & "ndarray.NDArrayScalarOpTransform",
                    "ChangeCaseStringTransform", T & "transform.string.ChangeCaseStringTransform",
                    "ConcatenateStringColumns", T & "transform.string.ConcatenateStringColumns",
                    "StringListToCountsNDArrayTransform", T & "transform.string.StringListToCountsNDArrayTransform",
                    "StringListToIndicesNDArrayTransform", T & "transform.string.StringListToIndicesNDArrayTransform",
                    "PivotTransform", T & "transform.categorical.PivotTransform",
                    "TextToCharacterIndexTransform", T & "transform.nlp.TextToCharacterIndexTransform"))
                list.Add(table(GetType(ConditionT),
                    "TrivialColumnCondition", T & "condition.column.TrivialColumnCondition",
                    "CategoricalColumnCondition", T & "condition.column.CategoricalColumnCondition",
                    "DoubleColumnCondition", T & "condition.column.DoubleColumnCondition",
                    "IntegerColumnCondition", T & "condition.column.IntegerColumnCondition",
                    "LongColumnCondition", T & "condition.column.LongColumnCondition",
                    "NullWritableColumnCondition", T & "condition.column.NullWritableColumnCondition",
                    "StringColumnCondition", T & "condition.column.StringColumnCondition",
                    "TimeColumnCondition", T & "condition.column.TimeColumnCondition",
                    "StringRegexColumnCondition", T & "condition.string.StringRegexColumnCondition",
                    "BooleanCondition", T & "condition.BooleanCondition",
                    "NaNColumnCondition", T & "condition.column.NaNColumnCondition",
                    "InfiniteColumnCondition", T & "condition.column.InfiniteColumnCondition",
                    "SequenceLengthCondition", T & "condition.sequence.SequenceLengthCondition"))
                list.Add(table(GetType(WritableT),
                    "ArrayWritable", "org.datavec.api.writable.ArrayWritable",
                    "BooleanWritable", "org.datavec.api.writable.BooleanWritable",
                    "ByteWritable", "org.datavec.api.writable.ByteWritable",
                    "DoubleWritable", "org.datavec.api.writable.DoubleWritable",
                    "FloatWritable", "org.datavec.api.writable.FloatWritable",
                    "IntWritable", "org.datavec.api.writable.IntWritable",
                    "LongWritable", "org.datavec.api.writable.LongWritable",
                    "NullWritable", "org.datavec.api.writable.NullWritable",
                    "Text", "org.datavec.api.writable.Text",
                    "BytesWritable", "org.datavec.api.writable.BytesWritable"))
                list.Add(table(GetType(FilterT),
                    "ConditionFilter", T & "filter.ConditionFilter",
                    "FilterInvalidValues", T & "filter.FilterInvalidValues",
                    "InvalidNumCols", T & "filter.InvalidNumColumns"))
                list.Add(table(GetType(SequenceComparatorT),
                    "NumericalColumnComparator", T & "sequence.comparator.NumericalColumnComparator",
                    "StringComparator", T & "sequence.comparator.StringComparator"))
                list.Add(table(GetType(SequenceSplitT),
                    "SequenceSplitTimeSeparation", T & "sequence.split.SequenceSplitTimeSeparation",
                    "SplitMaxLengthSequence", T & "sequence.split.SplitMaxLengthSequence"))
                list.Add(table(GetType(WindowFunctionT),
                    "TimeWindowFunction", T & "sequence.window.TimeWindowFunction",
                    "OverlappingTimeWindowFunction", T & "sequence.window.OverlappingTimeWindowFunction"))
                list.Add(table(GetType(CalculateSortedRankT),
                    "CalculateSortedRank", T & "rank.CalculateSortedRank"))
                list.Add(table(GetType(WritableComparatorT),
                    "DoubleWritableComparator", "org.datavec.api.writable.comparator.DoubleWritableComparator",
                    "FloatWritableComparator", "org.datavec.api.writable.comparator.FloatWritableComparator",
                    "IntWritableComparator", "org.datavec.api.writable.comparator.IntWritableComparator",
                    "LongWritableComparator", "org.datavec.api.writable.comparator.LongWritableComparator",
                    "TextWritableComparator", "org.datavec.api.writable.comparator.TextWritableComparator"))
                list.Add(table(GetType(ColumnAnalysisT),
                    "BytesAnalysis", T & "analysis.columns.BytesAnalysis",
                    "CategoricalAnalysis", T & "analysis.columns.CategoricalAnalysis",
                    "DoubleAnalysis", T & "analysis.columns.DoubleAnalysis",
                    "IntegerAnalysis", T & "analysis.columns.IntegerAnalysis",
                    "LongAnalysis", T & "analysis.columns.LongAnalysis",
                    "StringAnalysis", T & "analysis.columns.StringAnalysis",
                    "TimeAnalysis", T & "analysis.columns.TimeAnalysis"))
                list.Add(table(GetType(IStringReducerT),
                    "StringReducer", T & "stringreduce.StringReducer"))
                tables = list
                Return tables
            End SyncLock
        End Function

        Private Shared Function table(ByVal root As Type, ByVal ParamArray namesAndClasses() As String) As KeyValuePair(Of Type, Dictionary(Of String, String))
            Dim d As New Dictionary(Of String, String)(StringComparer.Ordinal)
            For i As Integer = 0 To namesAndClasses.Length - 2 Step 2
                d(namesAndClasses(i)) = namesAndClasses(i + 1)
            Next
            Return New KeyValuePair(Of Type, Dictionary(Of String, String))(root, d)
        End Function

        ''' <summary>
        ''' Rewrites the legacy wrapper objects of a JSON tree into the current <c>"@class"</c> format. Returns the
        ''' (possibly replaced) root; current-format JSON is returned unchanged.
        ''' </summary>
        ''' <param name="node">         The JSON tree (modified in place) </param>
        ''' <param name="declaredType"> The type the JSON is read as </param>
        ''' <param name="options">      The options of <see cref="JsonMappers"/> </param>
        Public Shared Function convertLegacyJson(ByVal node As JsonNode, ByVal declaredType As Type, ByVal options As JsonSerializerOptions) As JsonNode
            Return convert(node, If(declaredType, GetType(Object)), options, 0)
        End Function

        Private Shared Function convert(ByVal node As JsonNode, ByVal declared As Type, ByVal options As JsonSerializerOptions, ByVal depth As Integer) As JsonNode
            If node Is Nothing OrElse depth > 512 Then Return node
            Dim arr As JsonArray = TryCast(node, JsonArray)
            If arr IsNot Nothing Then
                Dim elementType As Type = JsonMappers.elementTypeOf(declared)
                For i As Integer = 0 To arr.Count - 1
                    Dim child As JsonNode = arr(i)
                    Dim converted As JsonNode = convert(child, elementType, options, depth + 1)
                    If converted IsNot child Then arr(i) = converted
                Next
                Return arr
            End If
            Dim obj As JsonObject = TryCast(node, JsonObject)
            If obj Is Nothing Then Return node

            Dim actual As Type = declared
            Dim unwrapped As JsonObject = unwrapLegacy(obj, declared, options, actual)
            If unwrapped IsNot Nothing Then
                obj = unwrapped
            Else
                actual = JsonMappers.runtimeTypeOf(obj, declared, options)
            End If

            Dim wrapped As Boolean = JsonMappers.isWrapperRoot(actual) AndAlso obj.Count = 1
            Dim catalog As TypeCatalog = JsonMappers.catalogOf(options)
            For Each key As String In obj.Select(Function(kv) kv.Key).ToList()
                Dim child As JsonNode = obj(key)
                Dim childType As Type
                If wrapped Then
                    childType = If(catalog.wrapperType(actual, key), GetType(Object))
                Else
                    childType = JsonMappers.propertyTypeOf(actual, key, options)
                End If
                Dim converted As JsonNode = convert(child, childType, options, depth + 1)
                If converted IsNot child Then obj(key) = converted
            Next
            Return obj
        End Function

        ''' <summary>
        ''' If the object is a legacy wrapper for the declared type, a new object with <c>"@class"</c> and the wrapped
        ''' fields (and the resolved type in <paramref name="actual"/>); else Nothing.
        ''' </summary>
        Private Shared Function unwrapLegacy(ByVal obj As JsonObject, ByVal declared As Type, ByVal options As JsonSerializerOptions, ByRef actual As Type) As JsonObject
            If obj.Count <> 1 OrElse declared Is Nothing OrElse declared Is GetType(Object) Then Return Nothing
            Dim kv As KeyValuePair(Of String, JsonNode) = obj.First()
            If kv.Key = "@class" Then Return Nothing
            Dim inner As JsonObject = TryCast(kv.Value, JsonObject)
            If inner Is Nothing Then Return Nothing
            Dim catalog As TypeCatalog = JsonMappers.catalogOf(options)
            For Each entry In legacyTables()
                If Not entry.Key.IsAssignableFrom(declared) Then Continue For
                Dim javaName As String = Nothing
                If Not entry.Value.TryGetValue(kv.Key, javaName) Then Continue For
                Dim t As Type = catalog.typeForName(javaName)
                If t Is Nothing OrElse Not declared.IsAssignableFrom(t) Then Continue For
                Dim result As New JsonObject()
                result.Add("@class", JsonValue.Create(catalog.javaName(t)))
                For Each p In inner.ToList()
                    inner.Remove(p.Key)
                    If p.Key <> "@class" Then result.Add(p.Key, p.Value)
                Next
                actual = t
                Return result
            Next
            Return Nothing
        End Function

    End Class

End Namespace
