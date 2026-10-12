Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Data = lombok.Data
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports DataAnalysis = org.datavec.api.transform.analysis.DataAnalysis
Imports ColumnAnalysis = org.datavec.api.transform.analysis.columns.ColumnAnalysis
Imports NumericalColumnAnalysis = org.datavec.api.transform.analysis.columns.NumericalColumnAnalysis
Imports Condition = org.datavec.api.transform.condition.Condition
Imports ConditionFilter = org.datavec.api.transform.filter.ConditionFilter
Imports Filter = org.datavec.api.transform.filter.Filter
Imports NDArrayColumnsMathOpTransform = org.datavec.api.transform.ndarray.NDArrayColumnsMathOpTransform
Imports NDArrayDistanceTransform = org.datavec.api.transform.ndarray.NDArrayDistanceTransform
Imports NDArrayMathFunctionTransform = org.datavec.api.transform.ndarray.NDArrayMathFunctionTransform
Imports NDArrayScalarOpTransform = org.datavec.api.transform.ndarray.NDArrayScalarOpTransform
Imports CalculateSortedRank = org.datavec.api.transform.rank.CalculateSortedRank
Imports IAssociativeReducer = org.datavec.api.transform.reduce.IAssociativeReducer
Imports Schema = org.datavec.api.transform.schema.Schema
Imports SequenceSchema = org.datavec.api.transform.schema.SequenceSchema
Imports org.datavec.api.transform.sequence
Imports SequenceTrimToLengthTransform = org.datavec.api.transform.sequence.trim.SequenceTrimToLengthTransform
Imports SequenceTrimTransform = org.datavec.api.transform.sequence.trim.SequenceTrimTransform
Imports ReduceSequenceByWindowTransform = org.datavec.api.transform.sequence.window.ReduceSequenceByWindowTransform
Imports WindowFunction = org.datavec.api.transform.sequence.window.WindowFunction
Imports JsonMappers = org.datavec.api.transform.serde.JsonMappers
Imports org.datavec.api.transform.transform.categorical
Imports org.datavec.api.transform.transform.column
Imports ConditionalCopyValueTransform = org.datavec.api.transform.transform.condition.ConditionalCopyValueTransform
Imports ConditionalReplaceValueTransform = org.datavec.api.transform.transform.condition.ConditionalReplaceValueTransform
Imports ConditionalReplaceValueTransformWithDefault = org.datavec.api.transform.transform.condition.ConditionalReplaceValueTransformWithDefault
Imports org.datavec.api.transform.transform.doubletransform
Imports FloatColumnsMathOpTransform = org.datavec.api.transform.transform.floattransform.FloatColumnsMathOpTransform
Imports FloatMathFunctionTransform = org.datavec.api.transform.transform.floattransform.FloatMathFunctionTransform
Imports FloatMathOpTransform = org.datavec.api.transform.transform.floattransform.FloatMathOpTransform
Imports ConvertToInteger = org.datavec.api.transform.transform.integer.ConvertToInteger
Imports IntegerColumnsMathOpTransform = org.datavec.api.transform.transform.integer.IntegerColumnsMathOpTransform
Imports IntegerMathOpTransform = org.datavec.api.transform.transform.integer.IntegerMathOpTransform
Imports IntegerToOneHotTransform = org.datavec.api.transform.transform.integer.IntegerToOneHotTransform
Imports LongColumnsMathOpTransform = org.datavec.api.transform.transform.longtransform.LongColumnsMathOpTransform
Imports LongMathOpTransform = org.datavec.api.transform.transform.longtransform.LongMathOpTransform
Imports Normalize = org.datavec.api.transform.transform.normalize.Normalize
Imports SequenceMovingWindowReduceTransform = org.datavec.api.transform.transform.sequence.SequenceMovingWindowReduceTransform
Imports SequenceOffsetTransform = org.datavec.api.transform.transform.sequence.SequenceOffsetTransform
Imports org.datavec.api.transform.transform.string
Imports StringToTimeTransform = org.datavec.api.transform.transform.time.StringToTimeTransform
Imports TimeMathOpTransform = org.datavec.api.transform.transform.time.TimeMathOpTransform
Imports org.datavec.api.writable
Imports WritableComparator = org.datavec.api.writable.comparator.WritableComparator
Imports DateTimeZone = org.joda.time.DateTimeZone
Imports org.nd4j.common.primitives
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty
Imports JsonProcessingException = org.nd4j.shade.jackson.core.JsonProcessingException
Imports InvalidTypeIdException = org.nd4j.shade.jackson.databind.exc.InvalidTypeIdException

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

Namespace org.datavec.api.transform


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @Slf4j public class TransformProcess implements java.io.Serializable
	<Serializable>
	Public Class TransformProcess

		Private ReadOnly initialSchema As Schema
		Private actionList As IList(Of DataAction)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public TransformProcess(@JsonProperty("initialSchema") org.datavec.api.transform.schema.Schema initialSchema, @JsonProperty("actionList") List<DataAction> actionList)
		Public Sub New(ByVal initialSchema As Schema, ByVal actionList As IList(Of DataAction))
			Me.initialSchema = initialSchema
			Me.actionList = actionList

			'Calculate and set the schemas for each tranformation:
			Dim currInputSchema As Schema = initialSchema
			For Each d As DataAction In actionList
				If d.getTransform() IsNot Nothing Then
					Dim t As Transform = d.getTransform()
					t.InputSchema = currInputSchema
					currInputSchema = t.transform(currInputSchema)
				ElseIf d.getFilter() IsNot Nothing Then
					'Filter -> doesn't change schema. But we DO need to set the schema in the filter...
					d.getFilter().setInputSchema(currInputSchema)
				ElseIf d.getConvertToSequence() IsNot Nothing Then
					If TypeOf currInputSchema Is SequenceSchema Then
						Throw New Exception("Cannot convert to sequence: schema is already a sequence schema: " & currInputSchema)
					End If
					Dim cts As ConvertToSequence = d.getConvertToSequence()
					cts.InputSchema = currInputSchema
					currInputSchema = cts.transform(currInputSchema)
				ElseIf d.getConvertFromSequence() IsNot Nothing Then
					Dim cfs As ConvertFromSequence = d.getConvertFromSequence()
					If Not (TypeOf currInputSchema Is SequenceSchema) Then
						Throw New Exception("Cannot convert from sequence: schema is not a sequence schema: " & currInputSchema)
					End If
					cfs.setInputSchema(DirectCast(currInputSchema, SequenceSchema))
					currInputSchema = cfs.transform(DirectCast(currInputSchema, SequenceSchema))
				ElseIf d.getSequenceSplit() IsNot Nothing Then
					d.getSequenceSplit().setInputSchema(currInputSchema)
					Continue For 'no change to sequence schema
				ElseIf d.getReducer() IsNot Nothing Then
					Dim reducer As IAssociativeReducer = d.getReducer()
					reducer.InputSchema = currInputSchema
					currInputSchema = reducer.transform(currInputSchema)
				ElseIf d.getCalculateSortedRank() IsNot Nothing Then
					Dim csr As CalculateSortedRank = d.getCalculateSortedRank()
					csr.InputSchema = currInputSchema
					currInputSchema = csr.transform(currInputSchema)
				Else
					Throw New Exception("Unknown action: " & d)
				End If
			Next d
		End Sub

		Private Sub New(ByVal builder As Builder)
			Me.New(builder.initialSchema, builder.actionList)
		End Sub

		''' <summary>
		''' Get the action list that this transform process
		''' will execute
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property ActionList As IList(Of DataAction)
			Get
				Return actionList
			End Get
		End Property

		''' <summary>
		''' Get the Schema of the output data, after executing the process
		''' </summary>
		''' <returns> Schema of the output data </returns>
		Public Overridable ReadOnly Property FinalSchema As Schema
			Get
				Return getSchemaAfterStep(actionList.Count)
			End Get
		End Property

		''' <summary>
		''' Return the schema after executing all steps up to and including the specified step.
		''' Steps are indexed from 0: so getSchemaAfterStep(0) is after one transform has been executed.
		''' </summary>
		''' <param name="step"> Index of the step </param>
		''' <returns> Schema of the data, after that (and all prior) steps have been executed </returns>
		Public Overridable Function getSchemaAfterStep(ByVal [step] As Integer) As Schema
			Dim currInputSchema As Schema = initialSchema
			Dim i As Integer = 0
			For Each d As DataAction In actionList
				If d.getTransform() IsNot Nothing Then
					Dim t As Transform = d.getTransform()
					currInputSchema = t.transform(currInputSchema)
				ElseIf d.getFilter() IsNot Nothing Then
					i += 1
					Continue For 'Filter -> doesn't change schema
				ElseIf d.getConvertToSequence() IsNot Nothing Then
					If TypeOf currInputSchema Is SequenceSchema Then
						Throw New Exception("Cannot convert to sequence: schema is already a sequence schema: " & currInputSchema)
					End If
					Dim cts As ConvertToSequence = d.getConvertToSequence()
					currInputSchema = cts.transform(currInputSchema)
				ElseIf d.getConvertFromSequence() IsNot Nothing Then
					Dim cfs As ConvertFromSequence = d.getConvertFromSequence()
					If Not (TypeOf currInputSchema Is SequenceSchema) Then
						Throw New Exception("Cannot convert from sequence: schema is not a sequence schema: " & currInputSchema)
					End If
					currInputSchema = cfs.transform(DirectCast(currInputSchema, SequenceSchema))
				ElseIf d.getSequenceSplit() IsNot Nothing Then
					Continue For 'Sequence split -> no change to schema
				ElseIf d.getReducer() IsNot Nothing Then
					Dim reducer As IAssociativeReducer = d.getReducer()
					currInputSchema = reducer.transform(currInputSchema)
				ElseIf d.getCalculateSortedRank() IsNot Nothing Then
					Dim csr As CalculateSortedRank = d.getCalculateSortedRank()
					currInputSchema = csr.transform(currInputSchema)
				Else
					Throw New Exception("Unknown action: " & d)
				End If
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: if (i++ == step)
				If i = [step] Then
						i += 1
					Return currInputSchema
					Else
						i += 1
					End If
			Next d
			Return currInputSchema
		End Function


		''' <summary>
		''' Execute the full sequence of transformations for a single example. May return null if example is filtered
		''' <b>NOTE:</b> Some TransformProcess operations cannot be done on examples individually. Most notably, ConvertToSequence
		''' and ConvertFromSequence operations require the full data set to be processed at once
		''' </summary>
		''' <param name="input">
		''' @return </param>
		Public Overridable Function execute(ByVal input As IList(Of Writable)) As IList(Of Writable)
			Dim currValues As IList(Of Writable) = input

			For Each d As DataAction In actionList
				If d.getTransform() IsNot Nothing Then
					Dim t As Transform = d.getTransform()
					currValues = t.map(currValues)

				ElseIf d.getFilter() IsNot Nothing Then
					Dim f As Filter = d.getFilter()
					If f.removeExample(currValues) Then
						Return Nothing
					End If
				ElseIf d.getConvertToSequence() IsNot Nothing Then
					Throw New Exception("Cannot execute examples individually: TransformProcess contains a ConvertToSequence operation")
				ElseIf d.getConvertFromSequence() IsNot Nothing Then
					Throw New Exception("Unexpected operation: TransformProcess contains a ConvertFromSequence operation")
				ElseIf d.getSequenceSplit() IsNot Nothing Then
					Throw New Exception("Cannot execute examples individually: TransformProcess contains a SequenceSplit operation")
				Else
					Throw New Exception("Unknown action: " & d)
				End If
			Next d

			Return currValues
		End Function

		''' 
		''' <param name="input">
		''' @return </param>
		Public Overridable Function executeSequenceToSequence(ByVal input As IList(Of IList(Of Writable))) As IList(Of IList(Of Writable))
			Dim currValues As IList(Of IList(Of Writable)) = input

			For Each d As DataAction In actionList
				If d.getTransform() IsNot Nothing Then
					Dim t As Transform = d.getTransform()
					currValues = t.mapSequence(currValues)

				ElseIf d.getFilter() IsNot Nothing Then
					If d.getFilter().removeSequence(currValues) Then
						Return Nothing
					End If
				ElseIf d.getConvertToSequence() IsNot Nothing Then
					Throw New Exception("Cannot execute examples individually: TransformProcess contains a ConvertToSequence operation")
				ElseIf d.getConvertFromSequence() IsNot Nothing Then
					Throw New Exception("Unexpected operation: TransformProcess contains a ConvertFromSequence operation")
				ElseIf d.getSequenceSplit() IsNot Nothing Then
					Throw New Exception("Cannot execute examples individually: TransformProcess contains a SequenceSplit operation")
				Else
					Throw New Exception("Unknown or not supported action: " & d)
				End If
			Next d

			Return currValues
		End Function

		''' <summary>
		''' Execute the full sequence of transformations for a single time series (sequence). May return null if example is filtered
		''' </summary>
		Public Overridable Function executeSequence(ByVal inputSequence As IList(Of IList(Of Writable))) As IList(Of IList(Of Writable))
			Return executeSequenceToSequence(inputSequence)
		End Function


		''' <summary>
		''' Execute a TransformProcess that starts with a single (non-sequence) record,
		''' and converts it to a sequence record.
		''' <b>NOTE</b>: This method has the following significant limitation:
		''' if it contains a ConvertToSequence op,
		''' it MUST be using singleStepSequencesMode - see <seealso cref="ConvertToSequence"/> for details.<br>
		''' This restriction is necessary, as ConvertToSequence.singleStepSequencesMode is false, this requires a group by
		''' operation - i.e., we need to group multiple independent records together by key(s) - this isn't possible here,
		''' when providing a single example as input
		''' </summary>
		''' <param name="inputExample"> Input example </param>
		''' <returns> Sequence, after processing (or null, if it was filtered out) </returns>
		Public Overridable Function executeToSequenceBatch(ByVal inputExample As IList(Of IList(Of Writable))) As IList(Of IList(Of IList(Of Writable)))
			Dim ret As IList(Of IList(Of IList(Of Writable))) = New List(Of IList(Of IList(Of Writable)))()
			For Each record As IList(Of Writable) In inputExample
				ret.Add(execute(record, Nothing).Right)
			Next record
			Return ret
		End Function

		''' <summary>
		''' Execute a TransformProcess that starts with a single (non-sequence) record,
		''' and converts it to a sequence record.
		''' <b>NOTE</b>: This method has the following significant limitation:
		''' if it contains a ConvertToSequence op,
		''' it MUST be using singleStepSequencesMode - see <seealso cref="ConvertToSequence"/> for details.<br>
		''' This restriction is necessary, as ConvertToSequence.singleStepSequencesMode is false, this requires a group by
		''' operation - i.e., we need to group multiple independent records together by key(s) - this isn't possible here,
		''' when providing a single example as input
		''' </summary>
		''' <param name="inputExample"> Input example </param>
		''' <returns> Sequence, after processing (or null, if it was filtered out) </returns>
		Public Overridable Function executeToSequence(ByVal inputExample As IList(Of Writable)) As IList(Of IList(Of Writable))
			Return execute(inputExample, Nothing).Right
		End Function

		''' <summary>
		''' Execute a TransformProcess that starts with a sequence
		''' record, and converts it to a single (non-sequence) record
		''' </summary>
		''' <param name="inputSequence"> Input sequence </param>
		''' <returns> Record after processing (or null if filtered out) </returns>
		Public Overridable Function executeSequenceToSingle(ByVal inputSequence As IList(Of IList(Of Writable))) As IList(Of Writable)
			Return execute(Nothing, inputSequence).Left
		End Function

		Private Function execute(ByVal currEx As IList(Of Writable), ByVal currSeq As IList(Of IList(Of Writable))) As Pair(Of IList(Of Writable), IList(Of IList(Of Writable)))
			For Each d As DataAction In actionList
				If d.getTransform() IsNot Nothing Then
					Dim t As Transform = d.getTransform()

					If currEx IsNot Nothing Then
						currEx = t.map(currEx)
						currSeq = Nothing
					Else
						currEx = Nothing
						currSeq = t.mapSequence(currSeq)
					End If
				ElseIf d.getFilter() IsNot Nothing Then
					If (currEx IsNot Nothing AndAlso d.getFilter().removeExample(currEx)) OrElse d.getFilter().removeSequence(currEx) Then
						Return New Pair(Of IList(Of Writable), IList(Of IList(Of Writable)))(Nothing, Nothing)
					End If
				ElseIf d.getConvertToSequence() IsNot Nothing Then

					If d.getConvertToSequence().isSingleStepSequencesMode() Then
						If currSeq IsNot Nothing Then
							Throw New Exception("Cannot execute ConvertToSequence op: current records are already a sequence")
						Else
							currSeq = Collections.singletonList(currEx)
							currEx = Nothing
						End If
					Else
						'Can't execute this - would require a group-by operation, and we only have 1 example!
						Throw New Exception("Cannot execute examples individually: TransformProcess contains a" & " ConvertToSequence operation, with singleStepSequnceeMode == false. Only " & " ConvertToSequence operations with singleStepSequnceeMode == true can be executed individually " & "as other types require a groupBy operation (which cannot be executed when only a sinlge record) " & "is provided as input")
					End If
				ElseIf d.getConvertFromSequence() IsNot Nothing Then
					Throw New Exception("Unexpected operation: TransformProcess contains a ConvertFromSequence" & " operation. This would produce multiple output records, which cannot be executed using this method")
				ElseIf d.getSequenceSplit() IsNot Nothing Then
					Throw New Exception("Cannot execute examples individually: TransformProcess contains a" & " SequenceSplit operation. This would produce multiple output records, which cannot be executed" & " using this method")
				Else
					Throw New Exception("Unknown or not supported action: " & d)
				End If
			Next d

			Return New Pair(Of IList(Of Writable), IList(Of IList(Of Writable)))(currEx, currSeq)
		End Function

		''' <summary>
		''' Convert the TransformProcess to a JSON string
		''' </summary>
		''' <returns> TransformProcess, as JSON </returns>
		Public Overridable Function toJson() As String
			Try
				Return JsonMappers.Mapper.writeValueAsString(Me)
			Catch e As JsonProcessingException
				'TODO proper exception message
				Throw New Exception(e)
			End Try
		End Function

		''' <summary>
		''' Convert the TransformProcess to a YAML string
		''' </summary>
		''' <returns> TransformProcess, as YAML </returns>
		Public Overridable Function toYaml() As String
			Try
				Return JsonMappers.Mapper.writeValueAsString(Me)
			Catch e As JsonProcessingException
				'TODO proper exception message
				Throw New Exception(e)
			End Try
		End Function

		''' <summary>
		''' Deserialize a JSON String (created by <seealso cref="toJson()"/>) to a TransformProcess
		''' </summary>
		''' <returns> TransformProcess, from JSON </returns>
		Public Shared Function fromJson(ByVal json As String) As TransformProcess
			Try
				Return JsonMappers.Mapper.readValue(json, GetType(TransformProcess))
			Catch e As InvalidTypeIdException
				If e.Message.contains("@class") Then
					'JSON may be legacy (1.0.0-alpha or earlier), attempt to load it using old format
					Try
						Return JsonMappers.LegacyMapper.readValue(json, GetType(TransformProcess))
					Catch e2 As IOException
						Throw New Exception(e2)
					End Try
				End If
				Throw New Exception(e)
			Catch e As IOException
				'TODO proper exception message
				Throw New Exception(e)
			End Try
		End Function

		''' <summary>
		''' Deserialize a JSON String (created by <seealso cref="toJson()"/>) to a TransformProcess
		''' </summary>
		''' <returns> TransformProcess, from JSON </returns>
		Public Shared Function fromYaml(ByVal yaml As String) As TransformProcess
			Try
				Return JsonMappers.Mapper.readValue(yaml, GetType(TransformProcess))
			Catch e As IOException
				'TODO proper exception message
				Throw New Exception(e)
			End Try
		End Function


		''' <summary>
		''' Infer the categories for the given record reader for a particular column
		'''  Note that each "column index" is a column in the context of:
		''' List<Writable> record = ...;
		''' record.get(columnIndex);
		''' 
		'''  Note that anything passed in as a column will be automatically converted to a
		'''  string for categorical purposes.
		''' 
		'''  The *expected* input is strings or numbers (which have sensible toString() representations)
		''' 
		'''  Note that the returned categories will be sorted alphabetically
		''' </summary>
		''' <param name="recordReader"> the record reader to iterate through </param>
		''' <param name="columnIndex"> te column index to get categories for
		''' @return </param>
		Public Shared Function inferCategories(ByVal recordReader As RecordReader, ByVal columnIndex As Integer) As IList(Of String)
			Dim categories As ISet(Of String) = New HashSet(Of String)()
			Do While recordReader.hasNext()
				Dim [next] As IList(Of Writable) = recordReader.next()
				categories.Add([next](columnIndex).ToString())
			Loop

			'Sort categories alphabetically - HashSet and RecordReader orders are not deterministic in general
			Dim ret As IList(Of String) = New List(Of String)(categories)
			ret.Sort()
			Return ret
		End Function

		''' <summary>
		''' Infer the categories for the given record reader for
		''' a particular set of columns (this is more efficient than
		''' <seealso cref="inferCategories(RecordReader, Integer)"/>
		''' if you have more than one column you plan on inferring categories for)
		''' 
		''' Note that each "column index" is a column in the context of:
		''' List<Writable> record = ...;
		''' record.get(columnIndex);
		''' 
		''' 
		'''  Note that anything passed in as a column will be automatically converted to a
		'''  string for categorical purposes. Results may vary depending on what's passed in.
		'''  The *expected* input is strings or numbers (which have sensible toString() representations)
		''' 
		''' Note that the returned categories will be sorted alphabetically, for each column
		''' </summary>
		''' <param name="recordReader"> the record reader to scan </param>
		''' <param name="columnIndices"> the column indices the get </param>
		''' <returns> the inferred categories </returns>
		Public Shared Function inferCategories(ByVal recordReader As RecordReader, ByVal columnIndices() As Integer) As IDictionary(Of Integer, IList(Of String))
			If columnIndices Is Nothing OrElse columnIndices.Length < 1 Then
				Return java.util.Collections.emptyMap()
			End If

			Dim categoryMap As IDictionary(Of Integer, IList(Of String)) = New Dictionary(Of Integer, IList(Of String))()
			Dim categories As IDictionary(Of Integer, ISet(Of String)) = New Dictionary(Of Integer, ISet(Of String))()
			For i As Integer = 0 To columnIndices.Length - 1
				categoryMap(columnIndices(i)) = New List(Of String)()
				categories(columnIndices(i)) = New HashSet(Of String)()
			Next i
			Do While recordReader.hasNext()
				Dim [next] As IList(Of Writable) = recordReader.next()
				For i As Integer = 0 To columnIndices.Length - 1
					If columnIndices(i) >= [next].Count Then
						log.warn("Filtering out example: Invalid length of columns")
						Continue For
					End If

					categories(columnIndices(i)).Add([next](columnIndices(i)).ToString())
				Next i

			Loop

			For i As Integer = 0 To columnIndices.Length - 1
				CType(categoryMap(columnIndices(i)), List(Of String)).AddRange(categories(columnIndices(i)))

				'Sort categories alphabetically - HashSet and RecordReader orders are not deterministic in general
				categoryMap(columnIndices(i)).Sort()
			Next i

			Return categoryMap
		End Function

		''' <summary>
		''' Transforms a sequence
		''' of strings in to a sequence of writables
		''' (very similar to <seealso cref="transformRawStringsToInput(String...)"/>
		''' for sequences </summary>
		''' <param name="sequence"> the sequence to transform </param>
		''' <returns> the transformed input </returns>
		Public Overridable Function transformRawStringsToInputSequence(ByVal sequence As IList(Of IList(Of String))) As IList(Of IList(Of Writable))
			Dim ret As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			For Each input As IList(Of String) In sequence
				ret.Add(transformRawStringsToInputList(input))
			Next input
			Return ret
		End Function


		''' <summary>
		''' Based on the input schema,
		''' map raw string values to the appropriate
		''' writable </summary>
		''' <param name="values"> the values to convert </param>
		''' <returns> the transformed values based on the schema </returns>
		Public Overridable Function transformRawStringsToInputList(ByVal values As IList(Of String)) As IList(Of Writable)
			Dim ret As IList(Of Writable) = New List(Of Writable)()
			If values.Count <> initialSchema.numColumns() Then
				Throw New System.ArgumentException(String.Format("Number of values {0:D} does not match the number of input columns {1:D} for schema", values.Count, initialSchema.numColumns()))
			End If
			For i As Integer = 0 To values.Count - 1
				Select Case initialSchema.getType(i)
					Case String
						ret.Add(New Text(values(i)))
					Case Integer?
						ret.Add(New IntWritable(Integer.Parse(values(i))))
					Case Double?
						ret.Add(New DoubleWritable(Double.Parse(values(i))))
					Case Single?
						ret.Add(New FloatWritable(Single.Parse(values(i))))
					Case Categorical
						ret.Add(New Text(values(i)))
					Case Boolean?
						ret.Add(New BooleanWritable(Boolean.Parse(values(i))))
					Case Time

					Case Long?
						ret.Add(New LongWritable(Long.Parse(values(i))))
				End Select
			Next i
			Return ret
		End Function


		''' <summary>
		''' Based on the input schema,
		''' map raw string values to the appropriate
		''' writable </summary>
		''' <param name="values"> the values to convert </param>
		''' <returns> the transformed values based on the schema </returns>
		Public Overridable Function transformRawStringsToInput(ParamArray ByVal values() As String) As IList(Of Writable)
			Return transformRawStringsToInputList(New List(Of String) From {values})
		End Function

		''' <summary>
		''' Builder class for constructing a TransformProcess
		''' </summary>
		Public Class Builder

			Friend actionList As IList(Of DataAction) = New List(Of DataAction)()
			Friend initialSchema As Schema

			Public Sub New(ByVal initialSchema As Schema)
				Me.initialSchema = initialSchema
			End Sub

			''' <summary>
			''' Add a transformation to be executed after the previously-added operations have been executed
			''' </summary>
			''' <param name="transform"> Transform to execute </param>
'JAVA TO VB CONVERTER NOTE: The parameter transform was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function transform(ByVal transform_Conflict As Transform) As Builder
				actionList.Add(New DataAction(transform_Conflict))
				Return Me
			End Function

			''' <summary>
			''' Add a filter operation to be executed after the previously-added operations have been executed
			''' </summary>
			''' <param name="filter"> Filter operation to execute </param>
'JAVA TO VB CONVERTER NOTE: The parameter filter was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function filter(ByVal filter_Conflict As Filter) As Builder
				actionList.Add(New DataAction(filter_Conflict))
				Return Me
			End Function

			''' <summary>
			''' Add a filter operation, based on the specified condition.
			''' 
			''' If condition is satisfied (returns true): remove the example or sequence<br>
			''' If condition is not satisfied (returns false): keep the example or sequence
			''' </summary>
			''' <param name="condition"> Condition to filter on </param>
			Public Overridable Function filter(ByVal condition As Condition) As Builder
				Return filter(New ConditionFilter(condition))
			End Function

			''' <summary>
			''' Remove all of the specified columns, by name
			''' </summary>
			''' <param name="columnNames"> Names of the columns to remove </param>
			Public Overridable Function removeColumns(ParamArray ByVal columnNames() As String) As Builder
				Return transform(New RemoveColumnsTransform(columnNames))
			End Function

			''' <summary>
			''' Remove all of the specified columns, by name
			''' </summary>
			''' <param name="columnNames"> Names of the columns to remove </param>
			Public Overridable Function removeColumns(ByVal columnNames As ICollection(Of String)) As Builder
				Return transform(New RemoveColumnsTransform(columnNames.ToArray()))
			End Function

			''' <summary>
			''' Remove all columns, except for those that are specified here </summary>
			''' <param name="columnNames">    Names of the columns to keep </param>
			Public Overridable Function removeAllColumnsExceptFor(ParamArray ByVal columnNames() As String) As Builder
				Return transform(New RemoveAllColumnsExceptForTransform(columnNames))
			End Function

			''' <summary>
			''' Remove all columns, except for those that are specified here </summary>
			''' <param name="columnNames">    Names of the columns to keep </param>
			Public Overridable Function removeAllColumnsExceptFor(ByVal columnNames As ICollection(Of String)) As Builder
				Return removeAllColumnsExceptFor(columnNames.ToArray())
			End Function

			''' <summary>
			''' Rename a single column
			''' </summary>
			''' <param name="oldName"> Original column name </param>
			''' <param name="newName"> New column name </param>
			Public Overridable Function renameColumn(ByVal oldName As String, ByVal newName As String) As Builder
				Return transform(New RenameColumnsTransform(oldName, newName))
			End Function

			''' <summary>
			''' Rename multiple columns
			''' </summary>
			''' <param name="oldNames"> List of original column names </param>
			''' <param name="newNames"> List of new column names </param>
			Public Overridable Function renameColumns(ByVal oldNames As IList(Of String), ByVal newNames As IList(Of String)) As Builder
				Return transform(New RenameColumnsTransform(oldNames, newNames))
			End Function

			''' <summary>
			''' Reorder the columns using a partial or complete new ordering.
			''' If only some of the column names are specified for the new order, the remaining columns will be placed at
			''' the end, according to their current relative ordering
			''' </summary>
			''' <param name="newOrder"> Names of the columns, in the order they will appear in the output </param>
			Public Overridable Function reorderColumns(ParamArray ByVal newOrder() As String) As Builder
				Return transform(New ReorderColumnsTransform(newOrder))
			End Function

			''' <summary>
			''' Duplicate a single column
			''' </summary>
			''' <param name="column"> Name of the column to duplicate </param>
			''' <param name="newName">    Name of the new (duplicate) column </param>
			Public Overridable Function duplicateColumn(ByVal column As String, ByVal newName As String) As Builder
				Return transform(New DuplicateColumnsTransform(Collections.singletonList(column), Collections.singletonList(newName)))
			End Function


			''' <summary>
			''' Duplicate a set of columns
			''' </summary>
			''' <param name="columnNames"> Names of the columns to duplicate </param>
			''' <param name="newNames">    Names of the new (duplicated) columns </param>
			Public Overridable Function duplicateColumns(ByVal columnNames As IList(Of String), ByVal newNames As IList(Of String)) As Builder
				Return transform(New DuplicateColumnsTransform(columnNames, newNames))
			End Function

			''' <summary>
			''' Perform a mathematical operation (add, subtract, scalar max etc) on the specified integer column, with a scalar
			''' </summary>
			''' <param name="column"> The integer column to perform the operation on </param>
			''' <param name="mathOp">     The mathematical operation </param>
			''' <param name="scalar">     The scalar value to use in the mathematical operation </param>
			Public Overridable Function integerMathOp(ByVal column As String, ByVal mathOp As MathOp, ByVal scalar As Integer) As Builder
				Return transform(New IntegerMathOpTransform(column, mathOp, scalar))
			End Function

			''' <summary>
			''' Calculate and add a new integer column by performing a mathematical operation on a number of existing columns.
			''' New column is added to the end.
			''' </summary>
			''' <param name="newColumnName"> Name of the new/derived column </param>
			''' <param name="mathOp">        Mathematical operation to execute on the columns </param>
			''' <param name="columnNames">   Names of the columns to use in the mathematical operation </param>
			Public Overridable Function integerColumnsMathOp(ByVal newColumnName As String, ByVal mathOp As MathOp, ParamArray ByVal columnNames() As String) As Builder
				Return transform(New IntegerColumnsMathOpTransform(newColumnName, mathOp, columnNames))
			End Function

			''' <summary>
			''' Perform a mathematical operation (add, subtract, scalar max etc) on the specified long column, with a scalar
			''' </summary>
			''' <param name="columnName"> The long column to perform the operation on </param>
			''' <param name="mathOp">     The mathematical operation </param>
			''' <param name="scalar">     The scalar value to use in the mathematical operation </param>
			Public Overridable Function longMathOp(ByVal columnName As String, ByVal mathOp As MathOp, ByVal scalar As Long) As Builder
				Return transform(New LongMathOpTransform(columnName, mathOp, scalar))
			End Function

			''' <summary>
			''' Calculate and add a new long column by performing a mathematical operation on a number of existing columns.
			''' New column is added to the end.
			''' </summary>
			''' <param name="newColumnName"> Name of the new/derived column </param>
			''' <param name="mathOp">        Mathematical operation to execute on the columns </param>
			''' <param name="columnNames">   Names of the columns to use in the mathematical operation </param>
			Public Overridable Function longColumnsMathOp(ByVal newColumnName As String, ByVal mathOp As MathOp, ParamArray ByVal columnNames() As String) As Builder
				Return transform(New LongColumnsMathOpTransform(newColumnName, mathOp, columnNames))
			End Function

			''' <summary>
			''' Perform a mathematical operation (add, subtract, scalar max etc) on the specified double column, with a scalar
			''' </summary>
			''' <param name="columnName"> The float column to perform the operation on </param>
			''' <param name="mathOp">     The mathematical operation </param>
			''' <param name="scalar">     The scalar value to use in the mathematical operation </param>
			Public Overridable Function floatMathOp(ByVal columnName As String, ByVal mathOp As MathOp, ByVal scalar As Single) As Builder
				Return transform(New FloatMathOpTransform(columnName, mathOp, scalar))
			End Function

			''' <summary>
			''' Calculate and add a new float column by performing a mathematical operation on a number of existing columns.
			''' New column is added to the end.
			''' </summary>
			''' <param name="newColumnName"> Name of the new/derived column </param>
			''' <param name="mathOp">        Mathematical operation to execute on the columns </param>
			''' <param name="columnNames">   Names of the columns to use in the mathematical operation </param>
			Public Overridable Function floatColumnsMathOp(ByVal newColumnName As String, ByVal mathOp As MathOp, ParamArray ByVal columnNames() As String) As Builder
				Return transform(New FloatColumnsMathOpTransform(newColumnName, mathOp, columnNames))
			End Function

			''' <summary>
			''' Perform a mathematical operation (such as sin(x), ceil(x), exp(x) etc) on a column
			''' </summary>
			''' <param name="columnName">   Column name to operate on </param>
			''' <param name="mathFunction"> MathFunction to apply to the column </param>
			Public Overridable Function floatMathFunction(ByVal columnName As String, ByVal mathFunction As MathFunction) As Builder
				Return transform(New FloatMathFunctionTransform(columnName, mathFunction))
			End Function

			''' <summary>
			''' Perform a mathematical operation (add, subtract, scalar max etc) on the specified double column, with a scalar
			''' </summary>
			''' <param name="columnName"> The double column to perform the operation on </param>
			''' <param name="mathOp">     The mathematical operation </param>
			''' <param name="scalar">     The scalar value to use in the mathematical operation </param>
			Public Overridable Function doubleMathOp(ByVal columnName As String, ByVal mathOp As MathOp, ByVal scalar As Double) As Builder
				Return transform(New DoubleMathOpTransform(columnName, mathOp, scalar))
			End Function

			''' <summary>
			''' Calculate and add a new double column by performing a mathematical operation on a number of existing columns.
			''' New column is added to the end.
			''' </summary>
			''' <param name="newColumnName"> Name of the new/derived column </param>
			''' <param name="mathOp">        Mathematical operation to execute on the columns </param>
			''' <param name="columnNames">   Names of the columns to use in the mathematical operation </param>
			Public Overridable Function doubleColumnsMathOp(ByVal newColumnName As String, ByVal mathOp As MathOp, ParamArray ByVal columnNames() As String) As Builder
				Return transform(New DoubleColumnsMathOpTransform(newColumnName, mathOp, columnNames))
			End Function

			''' <summary>
			''' Perform a mathematical operation (such as sin(x), ceil(x), exp(x) etc) on a column
			''' </summary>
			''' <param name="columnName">   Column name to operate on </param>
			''' <param name="mathFunction"> MathFunction to apply to the column </param>
			Public Overridable Function doubleMathFunction(ByVal columnName As String, ByVal mathFunction As MathFunction) As Builder
				Return transform(New DoubleMathFunctionTransform(columnName, mathFunction))
			End Function

			''' <summary>
			''' Perform a mathematical operation (add, subtract, scalar min/max only) on the specified time column
			''' </summary>
			''' <param name="columnName">   The integer column to perform the operation on </param>
			''' <param name="mathOp">       The mathematical operation </param>
			''' <param name="timeQuantity"> The quantity used in the mathematical op </param>
			''' <param name="timeUnit">     The unit that timeQuantity is specified in </param>
			Public Overridable Function timeMathOp(ByVal columnName As String, ByVal mathOp As MathOp, ByVal timeQuantity As Long, ByVal timeUnit As TimeUnit) As Builder
				Return transform(New TimeMathOpTransform(columnName, mathOp, timeQuantity, timeUnit))
			End Function


			''' <summary>
			''' Convert the specified column(s) from a categorical representation to a one-hot representation.
			''' This involves the creation of multiple new columns each.
			''' </summary>
			''' <param name="columnNames"> Names of the categorical column(s) to convert to a one-hot representation </param>
			Public Overridable Function categoricalToOneHot(ParamArray ByVal columnNames() As String) As Builder
				For Each s As String In columnNames
					transform(New CategoricalToOneHotTransform(s))
				Next s
				Return Me
			End Function

			''' <summary>
			''' Convert the specified column(s) from a categorical representation to an integer representation.
			''' This will replace the specified categorical column(s) with an integer repreesentation, where
			''' each integer has the value 0 to numCategories-1.
			''' </summary>
			''' <param name="columnNames"> Name of the categorical column(s) to convert to an integer representation </param>
			Public Overridable Function categoricalToInteger(ParamArray ByVal columnNames() As String) As Builder
				For Each s As String In columnNames
					transform(New CategoricalToIntegerTransform(s))
				Next s
				Return Me
			End Function

			''' <summary>
			''' Convert the specified column from an integer representation (assume values 0 to numCategories-1) to
			''' a categorical representation, given the specified state names
			''' </summary>
			''' <param name="columnName">         Name of the column to convert </param>
			''' <param name="categoryStateNames"> Names of the states for the categorical column </param>
			Public Overridable Function integerToCategorical(ByVal columnName As String, ByVal categoryStateNames As IList(Of String)) As Builder
				Return transform(New IntegerToCategoricalTransform(columnName, categoryStateNames))
			End Function

			''' <summary>
			''' Convert the specified column from an integer representation to a categorical representation, given the specified
			''' mapping between integer indexes and state names
			''' </summary>
			''' <param name="columnName">           Name of the column to convert </param>
			''' <param name="categoryIndexNameMap"> Names of the states for the categorical column </param>
			Public Overridable Function integerToCategorical(ByVal columnName As String, ByVal categoryIndexNameMap As IDictionary(Of Integer, String)) As Builder
				Return transform(New IntegerToCategoricalTransform(columnName, categoryIndexNameMap))
			End Function

			''' <summary>
			''' Convert an integer column to a set of 1 hot columns, based on the value in integer column
			''' </summary>
			''' <param name="columnName"> Name of the integer column </param>
			''' <param name="minValue">   Minimum value possible for the integer column (inclusive) </param>
			''' <param name="maxValue">   Maximum value possible for the integer column (inclusive) </param>
			Public Overridable Function integerToOneHot(ByVal columnName As String, ByVal minValue As Integer, ByVal maxValue As Integer) As Builder
				Return transform(New IntegerToOneHotTransform(columnName, minValue, maxValue))
			End Function

			''' <summary>
			''' Add a new column, where all values in the column are identical and as specified.
			''' </summary>
			''' <param name="newColumnName"> Name of the new column </param>
			''' <param name="newColumnType"> Type of the new column </param>
			''' <param name="fixedValue">    Value in the new column for all records </param>
			Public Overridable Function addConstantColumn(ByVal newColumnName As String, ByVal newColumnType As ColumnType, ByVal fixedValue As Writable) As Builder
				Return transform(New AddConstantColumnTransform(newColumnName, newColumnType, fixedValue))
			End Function

			''' <summary>
			''' Add a new double column, where the value for that column (for all records) are identical
			''' </summary>
			''' <param name="newColumnName"> Name of the new column </param>
			''' <param name="value">         Value in the new column for all records </param>
			Public Overridable Function addConstantDoubleColumn(ByVal newColumnName As String, ByVal value As Double) As Builder
				Return addConstantColumn(newColumnName, ColumnType.Double, New DoubleWritable(value))
			End Function

			''' <summary>
			''' Add a new integer column, where th
			''' e value for that column (for all records) are identical
			''' </summary>
			''' <param name="newColumnName"> Name of the new column </param>
			''' <param name="value">         Value of the new column for all records </param>
			Public Overridable Function addConstantIntegerColumn(ByVal newColumnName As String, ByVal value As Integer) As Builder
				Return addConstantColumn(newColumnName, ColumnType.Integer, New IntWritable(value))
			End Function

			''' <summary>
			''' Add a new integer column, where the value for that column (for all records) are identical
			''' </summary>
			''' <param name="newColumnName"> Name of the new column </param>
			''' <param name="value">         Value in the new column for all records </param>
			Public Overridable Function addConstantLongColumn(ByVal newColumnName As String, ByVal value As Long) As Builder
				Return addConstantColumn(newColumnName, ColumnType.Long, New LongWritable(value))
			End Function


			''' <summary>
			''' Convert the specified column to a string. </summary>
			''' <param name="inputColumn"> the input column to convert </param>
			''' <returns> builder pattern </returns>
			Public Overridable Function convertToString(ByVal inputColumn As String) As Builder
				Return transform(New ConvertToString(inputColumn))
			End Function


			''' <summary>
			''' Convert the specified column to a double. </summary>
			''' <param name="inputColumn"> the input column to convert </param>
			''' <returns> builder pattern </returns>
			Public Overridable Function convertToDouble(ByVal inputColumn As String) As Builder
				Return transform(New ConvertToDouble(inputColumn))
			End Function


			''' <summary>
			''' Convert the specified column to an integer. </summary>
			''' <param name="inputColumn"> the input column to convert </param>
			''' <returns> builder pattern </returns>
			Public Overridable Function convertToInteger(ByVal inputColumn As String) As Builder
				Return transform(New ConvertToInteger(inputColumn))
			End Function

			''' <summary>
			''' Normalize the specified column with a given type of normalization
			''' </summary>
			''' <param name="column"> Column to normalize </param>
			''' <param name="type">   Type of normalization to apply </param>
			''' <param name="da">     DataAnalysis object </param>
			Public Overridable Function normalize(ByVal column As String, ByVal type As Normalize, ByVal da As DataAnalysis) As Builder

				Dim ca As ColumnAnalysis = da.getColumnAnalysis(column)
				If Not (TypeOf ca Is NumericalColumnAnalysis) Then
					Throw New System.InvalidOperationException("Column """ & column & """ analysis is not numerical. " & "Column is not numerical?")
				End If

				Dim nca As NumericalColumnAnalysis = DirectCast(ca, NumericalColumnAnalysis)
				Dim min As Double = nca.MinDouble
				Dim max As Double = nca.MaxDouble
				Dim mean As Double = nca.getMean()
				Dim sigma As Double = nca.getSampleStdev()

				Select Case type
					Case Normalize.MinMax
						Return transform(New MinMaxNormalizer(column, min, max))
					Case Normalize.MinMax2
						Return transform(New MinMaxNormalizer(column, min, max, -1, 1))
					Case Normalize.Standardize
						Return transform(New StandardizeNormalizer(column, mean, sigma))
					Case Normalize.SubtractMean
						Return transform(New SubtractMeanNormalizer(column, mean))
					Case Normalize.Log2Mean
						Return transform(New Log2Normalizer(column, mean, min, 0.5))
					Case Normalize.Log2MeanExcludingMin
						Dim countMin As Long = nca.getCountMinValue()

						'mean including min value: (sum/totalCount)
						'mean excluding min value: (sum - countMin*min)/(totalCount - countMin)
						Dim meanExMin As Double
						If ca.CountTotal - countMin = 0 Then
							If ca.CountTotal = 0 Then
								log.warn("Normalizing with Log2MeanExcludingMin but 0 records present in analysis")
							Else
								log.warn("Normalizing with Log2MeanExcludingMin but all records are the same value")
							End If
							meanExMin = mean
						Else
							meanExMin = (mean * ca.CountTotal - countMin * min) / (ca.CountTotal - countMin)
						End If
						Return transform(New Log2Normalizer(column, meanExMin, min, 0.5))
					Case Else
						Throw New Exception("Unknown/not implemented normalization type: " & type)
				End Select
			End Function

			''' <summary>
			''' Convert a set of independent records/examples into a sequence, according to some key.
			''' Within each sequence, values are ordered using the provided <seealso cref="SequenceComparator"/>
			''' </summary>
			''' <param name="keyColumn">  Column to use as a key (values with the same key will be combined into sequences) </param>
			''' <param name="comparator"> A SequenceComparator to order the values within each sequence (for example, by time or String order) </param>
			Public Overridable Function convertToSequence(ByVal keyColumn As String, ByVal comparator As SequenceComparator) As Builder
				actionList.Add(New DataAction(New ConvertToSequence(keyColumn, comparator)))
				Return Me
			End Function

			''' <summary>
			''' Convert a set of independent records/examples into a sequence; each example is simply treated as a sequence
			''' of length 1, without any join/group operations. Note that more commonly, joining/grouping is required;
			''' use <seealso cref="convertToSequence(List, SequenceComparator)"/> for this functionality
			''' 
			''' </summary>
			Public Overridable Function convertToSequence() As Builder
				actionList.Add(New DataAction(New ConvertToSequence(True, Nothing, Nothing)))
				Return Me
			End Function

			''' <summary>
			''' Convert a set of independent records/examples into a sequence, where each sequence is grouped according to
			''' one or more key values (i.e., the values in one or more columns)
			''' Within each sequence, values are ordered using the provided <seealso cref="SequenceComparator"/>
			''' </summary>
			''' <param name="keyColumns">  Column to use as a key (values with the same key will be combined into sequences) </param>
			''' <param name="comparator"> A SequenceComparator to order the values within each sequence (for example, by time or String order) </param>
			Public Overridable Function convertToSequence(ByVal keyColumns As IList(Of String), ByVal comparator As SequenceComparator) As Builder
				actionList.Add(New DataAction(New ConvertToSequence(keyColumns, comparator)))
				Return Me
			End Function


			''' <summary>
			''' Convert a sequence to a set of individual values (by treating each value in each sequence as a separate example)
			''' </summary>
			Public Overridable Function convertFromSequence() As Builder
				actionList.Add(New DataAction(New ConvertFromSequence()))
				Return Me
			End Function

			''' <summary>
			''' Split sequences into 1 or more other sequences. Used for example to split large sequences into a set of smaller sequences
			''' </summary>
			''' <param name="split"> SequenceSplit that defines how splits will occur </param>
			Public Overridable Function splitSequence(ByVal split As SequenceSplit) As Builder
				actionList.Add(New DataAction(split))
				Return Me
			End Function

			''' <summary>
			''' SequenceTrimTranform removes the first or last N values in a sequence. Note that the resulting sequence
			''' may be of length 0, if the input sequence is less than or equal to N.
			''' </summary>
			''' <param name="numStepsToTrim"> Number of time steps to trim from the sequence </param>
			''' <param name="trimFromStart">  If true: Trim values from the start of the sequence. If false: trim values from the end. </param>
			Public Overridable Function trimSequence(ByVal numStepsToTrim As Integer, ByVal trimFromStart As Boolean) As Builder
				actionList.Add(New DataAction(New org.datavec.api.transform.sequence.Trim.SequenceTrimTransform(numStepsToTrim, trimFromStart)))
				Return Me
			End Function

			''' <summary>
			''' Trim the sequence to the specified length (number of sequence steps).<br>
			''' Sequences longer than the specified maximum will be trimmed to exactly the maximum. Shorter sequences will not be modified.
			''' </summary>
			''' <param name="maxLength"> Maximum sequence length (number of time steps) </param>
			Public Overridable Function trimSequenceToLength(ByVal maxLength As Integer) As Builder
				actionList.Add(New DataAction(New org.datavec.api.transform.sequence.Trim.SequenceTrimToLengthTransform(maxLength, org.datavec.api.transform.sequence.Trim.SequenceTrimToLengthTransform.Mode.TRIM, Nothing)))
				Return Me
			End Function

			''' <summary>
			''' Trim or pad the sequence to the specified length (number of sequence steps).<br>
			''' Sequences longer than the specified maximum will be trimmed to exactly the maximum. Shorter sequences will be
			''' padded with as many copies of the "pad" array to make the sequence length equal the specified maximum.<br>
			''' Note that the 'pad' list (i.e., values to pad with) must be equal in length to the number of columns (values per time step)
			''' </summary>
			''' <param name="length"> Required length - trim sequences longer than this, pad sequences shorter than this </param>
			''' <param name="pad">    Values to pad at the end of the sequence </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder trimOrPadSequenceToLength(int length, @NonNull List<Writable> pad)
			Public Overridable Function trimOrPadSequenceToLength(ByVal length As Integer, ByVal pad As IList(Of Writable)) As Builder
				actionList.Add(New DataAction(New org.datavec.api.transform.sequence.Trim.SequenceTrimToLengthTransform(length, org.datavec.api.transform.sequence.Trim.SequenceTrimToLengthTransform.Mode.TRIM_OR_PAD, pad)))
				Return Me
			End Function

			''' <summary>
			''' Perform a sequence of operation on the specified columns. Note that this also truncates sequences by the
			''' specified offset amount by default. Use {@code transform(new SequenceOffsetTransform(...)} to change this.
			''' See <seealso cref="SequenceOffsetTransform"/> for details on exactly what this operation does and how.
			''' </summary>
			''' <param name="columnsToOffset"> Columns to offset </param>
			''' <param name="offsetAmount">    Amount to offset the specified columns by (positive offset: 'columnsToOffset' are
			'''                        moved to later time steps) </param>
			''' <param name="operationType">   Whether the offset should be done in-place or by adding a new column </param>
			Public Overridable Function offsetSequence(ByVal columnsToOffset As IList(Of String), ByVal offsetAmount As Integer, ByVal operationType As SequenceOffsetTransform.OperationType) As Builder
				Return transform(New SequenceOffsetTransform(columnsToOffset, offsetAmount, operationType, SequenceOffsetTransform.EdgeHandling.TrimSequence, Nothing))
			End Function


			''' <summary>
			''' Reduce (i.e., aggregate/combine) a set of examples (typically by key).
			''' <b>Note</b>: In the current implementation, reduction operations can be performed only on standard (i.e., non-sequence) data
			''' </summary>
			''' <param name="reducer"> Reducer to use </param>
			Public Overridable Function reduce(ByVal reducer As IAssociativeReducer) As Builder
				actionList.Add(New DataAction(reducer))
				Return Me
			End Function

			''' <summary>
			''' Reduce (i.e., aggregate/combine) a set of sequence examples - for each sequence individually.
			''' <b>Note</b>: This method results in non-sequence data. If you would instead prefer sequences of length 1
			''' after the reduction, use {@code transform(new ReduceSequenceTransform(reducer))}.
			''' </summary>
			''' <param name="reducer">        Reducer to use to reduce each window </param>
			Public Overridable Function reduceSequence(ByVal reducer As IAssociativeReducer) As Builder
				actionList.Add(New DataAction(New ReduceSequenceTransform(reducer)))
				convertFromSequence()
				Return Me
			End Function

			''' <summary>
			''' Reduce (i.e., aggregate/combine) a set of sequence examples - for each sequence individually - using a window function.
			''' For example, take all records/examples in each 24-hour period (i.e., using window function), and convert them into
			''' a singe value (using the reducer). In this example, the output is a sequence, with time period of 24 hours.
			''' </summary>
			''' <param name="reducer">        Reducer to use to reduce each window </param>
			''' <param name="windowFunction"> Window function to find apply on each sequence individually </param>
			Public Overridable Function reduceSequenceByWindow(ByVal reducer As IAssociativeReducer, ByVal windowFunction As WindowFunction) As Builder
				actionList.Add(New DataAction(New ReduceSequenceByWindowTransform(reducer, windowFunction)))
				Return Me
			End Function

			''' <summary>
			''' SequenceMovingWindowReduceTransform: Adds a new column, where the value is derived by:<br>
			''' (a) using a window of the last N values in a single column,<br>
			''' (b) Apply a reduction op on the window to calculate a new value<br>
			''' for example, this transformer can be used to implement a simple moving average of the last N values,
			''' or determine the minimum or maximum values in the last N time steps.
			''' <para>
			''' For example, for a simple moving average, length 20: {@code new SequenceMovingWindowReduceTransform("myCol", 20, ReduceOp.Mean)}
			''' 
			''' </para>
			''' </summary>
			''' <param name="columnName"> Column name to perform windowing on </param>
			''' <param name="lookback">   Look back period for windowing </param>
			''' <param name="op">         Reduction operation to perform on each window </param>
			Public Overridable Function sequenceMovingWindowReduce(ByVal columnName As String, ByVal lookback As Integer, ByVal op As ReduceOp) As Builder
				actionList.Add(New DataAction(New SequenceMovingWindowReduceTransform(columnName, lookback, op)))
				Return Me
			End Function

			''' <summary>
			''' CalculateSortedRank: calculate the rank of each example, after sorting example.
			''' For example, we might have some numerical "score" column, and we want to know for the rank (sort order) for each
			''' example, according to that column.<br>
			''' The rank of each example (after sorting) will be added in a new Long column. Indexing is done from 0; examples will have
			''' values 0 to dataSetSize-1.<br>
			''' <para>
			''' Currently, CalculateSortedRank can only be applied on standard (i.e., non-sequence) data
			''' Furthermore, the current implementation can only sort on one column
			''' 
			''' </para>
			''' </summary>
			''' <param name="newColumnName"> Name of the new column (will contain the rank for each example) </param>
			''' <param name="sortOnColumn">  Column to sort on </param>
			''' <param name="comparator">    Comparator used to sort examples </param>
			Public Overridable Function calculateSortedRank(ByVal newColumnName As String, ByVal sortOnColumn As String, ByVal comparator As WritableComparator) As Builder
				actionList.Add(New DataAction(New CalculateSortedRank(newColumnName, sortOnColumn, comparator)))
				Return Me
			End Function

			''' <summary>
			''' CalculateSortedRank: calculate the rank of each example, after sorting example.
			''' For example, we might have some numerical "score" column, and we want to know for the rank (sort order) for each
			''' example, according to that column.<br>
			''' The rank of each example (after sorting) will be added in a new Long column. Indexing is done from 0; examples will have
			''' values 0 to dataSetSize-1.<br>
			''' <para>
			''' Currently, CalculateSortedRank can only be applied on standard (i.e., non-sequence) data
			''' Furthermore, the current implementation can only sort on one column
			''' 
			''' </para>
			''' </summary>
			''' <param name="newColumnName"> Name of the new column (will contain the rank for each example) </param>
			''' <param name="sortOnColumn">  Column to sort on </param>
			''' <param name="comparator">    Comparator used to sort examples </param>
			''' <param name="ascending">     If true: sort ascending. False: descending </param>
			Public Overridable Function calculateSortedRank(ByVal newColumnName As String, ByVal sortOnColumn As String, ByVal comparator As WritableComparator, ByVal ascending As Boolean) As Builder
				actionList.Add(New DataAction(New CalculateSortedRank(newColumnName, sortOnColumn, comparator, ascending)))
				Return Me
			End Function

			''' <summary>
			''' Convert the specified String column to a categorical column. The state names must be provided.
			''' </summary>
			''' <param name="columnName"> Name of the String column to convert to categorical </param>
			''' <param name="stateNames"> State names of the category </param>
			Public Overridable Function stringToCategorical(ByVal columnName As String, ByVal stateNames As IList(Of String)) As Builder
				Return transform(New StringToCategoricalTransform(columnName, stateNames))
			End Function

			''' <summary>
			''' Remove all whitespace characters from the values in the specified String column
			''' </summary>
			''' <param name="columnName"> Name of the column to remove whitespace from </param>
			Public Overridable Function stringRemoveWhitespaceTransform(ByVal columnName As String) As Builder
				Return transform(New RemoveWhiteSpaceTransform(columnName))
			End Function

			''' <summary>
			''' Replace one or more String values in the specified column with new values.
			''' <para>
			''' Keys in the map are the original values; the Values in the map are their replacements.
			''' If a String appears in the data but does not appear in the provided map (as a key), that String values will
			''' not be modified.
			''' 
			''' </para>
			''' </summary>
			''' <param name="columnName"> Name of the column in which to do replacement </param>
			''' <param name="mapping">    Map of oldValues -> newValues </param>
			Public Overridable Function stringMapTransform(ByVal columnName As String, ByVal mapping As IDictionary(Of String, String)) As Builder
				Return transform(New StringMapTransform(columnName, mapping))
			End Function

			''' <summary>
			''' Convert a String column (containing a date/time String) to a time column (by parsing the date/time String)
			''' </summary>
			''' <param name="column">       String column containing the date/time Strings </param>
			''' <param name="format">       Format of the strings. Time format is specified as per http://www.joda.org/joda-time/apidocs/org/joda/time/format/DateTimeFormat.html </param>
			''' <param name="dateTimeZone"> Timezone of the column </param>
			Public Overridable Function stringToTimeTransform(ByVal column As String, ByVal format As String, ByVal dateTimeZone As DateTimeZone) As Builder
				Return transform(New StringToTimeTransform(column, format, dateTimeZone))
			End Function


			''' <summary>
			''' Convert a String column (containing a date/time String) to a time column (by parsing the date/time String)
			''' </summary>
			''' <param name="column">       String column containing the date/time Strings </param>
			''' <param name="format">       Format of the strings. Time format is specified as per http://www.joda.org/joda-time/apidocs/org/joda/time/format/DateTimeFormat.html </param>
			''' <param name="dateTimeZone"> Timezone of the column </param>
			''' <param name="locale">       Locale of the column </param>
			Public Overridable Function stringToTimeTransform(ByVal column As String, ByVal format As String, ByVal dateTimeZone As DateTimeZone, ByVal locale As Locale) As Builder
				Return transform(New StringToTimeTransform(column, format, dateTimeZone, locale))
			End Function

			''' <summary>
			''' Append a String to a specified column
			''' </summary>
			''' <param name="column">      Column to append the value to </param>
			''' <param name="toAppend">    String to append to the end of each writable </param>
			Public Overridable Function appendStringColumnTransform(ByVal column As String, ByVal toAppend As String) As Builder
				Return transform(New AppendStringColumnTransform(column, toAppend))
			End Function

			''' <summary>
			''' Replace the values in a specified column with a specified new value, if some condition holds.
			''' If the condition does not hold, the original values are not modified.
			''' </summary>
			''' <param name="column">    Column to operate on </param>
			''' <param name="newValue">  Value to use as replacement, if condition is satisfied </param>
			''' <param name="condition"> Condition that must be satisfied for replacement </param>
			Public Overridable Function conditionalReplaceValueTransform(ByVal column As String, ByVal newValue As Writable, ByVal condition As Condition) As Builder
				Return transform(New ConditionalReplaceValueTransform(column, newValue, condition))
			End Function

			''' <summary>
			''' Replace the values in a specified column with a specified "yes" value, if some condition holds.
			''' Replace it with a "no" value, otherwise.
			''' </summary>
			''' <param name="column">    Column to operate on </param>
			''' <param name="yesVal">  Value to use as replacement, if condition is satisfied </param>
			''' <param name="noVal">  Value to use as replacement, if condition is not satisfied </param>
			''' <param name="condition"> Condition that must be satisfied for replacement </param>
			Public Overridable Function conditionalReplaceValueTransformWithDefault(ByVal column As String, ByVal yesVal As Writable, ByVal noVal As Writable, ByVal condition As Condition) As Builder
				Return transform(New ConditionalReplaceValueTransformWithDefault(column, yesVal, noVal, condition))
			End Function

			''' <summary>
			''' Replace the value in a specified column with a new value taken from another column, if a condition is satisfied/true.<br>
			''' Note that the condition can be any generic condition, including on other column(s), different to the column
			''' that will be modified if the condition is satisfied/true.<br>
			''' </summary>
			''' <param name="columnToReplace">    Name of the column in which values will be replaced (if condition is satisfied) </param>
			''' <param name="sourceColumn">       Name of the column from which the new values will be </param>
			''' <param name="condition">          Condition to use </param>
			Public Overridable Function conditionalCopyValueTransform(ByVal columnToReplace As String, ByVal sourceColumn As String, ByVal condition As Condition) As Builder
				Return transform(New ConditionalCopyValueTransform(columnToReplace, sourceColumn, condition))
			End Function

			''' <summary>
			''' Replace one or more String values in the specified column that match regular expressions.
			''' <para>
			''' Keys in the map are the regular expressions; the Values in the map are their String replacements.
			''' For example:
			''' <blockquote>
			''' <table cellpadding="2">
			''' <tr>
			'''      <th>Original</th>
			'''      <th>Regex</th>
			'''      <th>Replacement</th>
			'''      <th>Result</th>
			''' </tr>
			''' <tr>
			'''      <td>Data_Vec</td>
			'''      <td>_</td>
			'''      <td></td>
			'''      <td>DataVec</td>
			''' </tr>
			''' <tr>
			'''      <td>B1C2T3</td>
			'''      <td>\\d</td>
			'''      <td>one</td>
			'''      <td>BoneConeTone</td>
			''' </tr>
			''' <tr>
			'''      <td>'&nbsp&nbsp4.25&nbsp'</td>
			'''      <td>^\\s+|\\s+$</td>
			'''      <td></td>
			'''      <td>'4.25'</td>
			''' </tr>
			''' </table>
			''' </blockquote>
			''' 
			''' </para>
			''' </summary>
			''' <param name="columnName"> Name of the column in which to do replacement </param>
			''' <param name="mapping">    Map of old values or regular expression to new values </param>
			Public Overridable Function replaceStringTransform(ByVal columnName As String, ByVal mapping As IDictionary(Of String, String)) As Builder
				Return transform(New ReplaceStringTransform(columnName, mapping))
			End Function

			''' <summary>
			''' Element-wise NDArray math operation (add, subtract, etc) on an NDArray column
			''' </summary>
			''' <param name="columnName"> Name of the NDArray column to perform the operation on </param>
			''' <param name="op">         Operation to perform </param>
			''' <param name="value">      Value for the operation </param>
			Public Overridable Function ndArrayScalarOpTransform(ByVal columnName As String, ByVal op As MathOp, ByVal value As Double) As Builder
				Return transform(New NDArrayScalarOpTransform(columnName, op, value))
			End Function

			''' <summary>
			''' Perform an element wise mathematical operation (such as add, subtract, multiply) on NDArray columns.
			''' The existing columns are unchanged, a new NDArray column is added
			''' </summary>
			''' <param name="newColumnName"> Name of the new NDArray column </param>
			''' <param name="mathOp">        Operation to perform </param>
			''' <param name="columnNames">   Name of the columns used as input to the operation </param>
			Public Overridable Function ndArrayColumnsMathOpTransform(ByVal newColumnName As String, ByVal mathOp As MathOp, ParamArray ByVal columnNames() As String) As Builder
				Return transform(New NDArrayColumnsMathOpTransform(newColumnName, mathOp, columnNames))
			End Function

			''' <summary>
			''' Apply an element wise mathematical function (sin, tanh, abs etc) to an NDArray column. This operation is
			''' performed in place.
			''' </summary>
			''' <param name="columnName">   Name of the column to perform the operation on </param>
			''' <param name="mathFunction"> Mathematical function to apply </param>
			Public Overridable Function ndArrayMathFunctionTransform(ByVal columnName As String, ByVal mathFunction As MathFunction) As Builder
				Return transform(New NDArrayMathFunctionTransform(columnName, mathFunction))
			End Function

			''' <summary>
			''' Calculate a distance (cosine similarity, Euclidean, Manhattan) on two equal-sized NDArray columns. This
			''' operation adds a new Double column (with the specified name) with the result.
			''' </summary>
			''' <param name="newColumnName"> Name of the new column (result) to add </param>
			''' <param name="distance">      Distance to apply </param>
			''' <param name="firstCol">      first column to use in the distance calculation </param>
			''' <param name="secondCol">     second column to use in the distance calculation </param>
			Public Overridable Function ndArrayDistanceTransform(ByVal newColumnName As String, ByVal distance As Distance, ByVal firstCol As String, ByVal secondCol As String) As Builder
				Return transform(New NDArrayDistanceTransform(newColumnName, distance, firstCol, secondCol))
			End Function

			''' <summary>
			''' FirstDigitTransform converts a column to a categorical column, with values being the first digit of the number.<br>
			''' For example, "3.1415" becomes "3" and "2.0" becomes "2".<br>
			''' Negative numbers ignore the sign: "-7.123" becomes "7".<br>
			''' Note that two <seealso cref="FirstDigitTransform.Mode"/>s are supported, which determines how non-numerical entries should be handled:<br>
			''' EXCEPTION_ON_INVALID: output has 10 category values ("0", ..., "9"), and any non-numerical values result in an exception<br>
			''' INCLUDE_OTHER_CATEGORY: output has 11 category values ("0", ..., "9", "Other"), all non-numerical values are mapped to "Other"<br>
			''' <br>
			''' FirstDigitTransform is useful (combined with <seealso cref="CategoricalToOneHotTransform"/> and Reductions) to implement
			''' <a href="https://en.wikipedia.org/wiki/Benford%27s_law">Benford's law</a>.
			''' </summary>
			''' <param name="inputColumn">  Input column name </param>
			''' <param name="outputColumn"> Output column name. If same as input, input column is replaced </param>
			Public Overridable Function firstDigitTransform(ByVal inputColumn As String, ByVal outputColumn As String) As Builder
				Return firstDigitTransform(inputColumn, outputColumn, FirstDigitTransform.Mode.INCLUDE_OTHER_CATEGORY)
			End Function

			''' <summary>
			''' FirstDigitTransform converts a column to a categorical column, with values being the first digit of the number.<br>
			''' For example, "3.1415" becomes "3" and "2.0" becomes "2".<br>
			''' Negative numbers ignore the sign: "-7.123" becomes "7".<br>
			''' Note that two <seealso cref="FirstDigitTransform.Mode"/>s are supported, which determines how non-numerical entries should be handled:<br>
			''' EXCEPTION_ON_INVALID: output has 10 category values ("0", ..., "9"), and any non-numerical values result in an exception<br>
			''' INCLUDE_OTHER_CATEGORY: output has 11 category values ("0", ..., "9", "Other"), all non-numerical values are mapped to "Other"<br>
			''' <br>
			''' FirstDigitTransform is useful (combined with <seealso cref="CategoricalToOneHotTransform"/> and Reductions) to implement
			''' <a href="https://en.wikipedia.org/wiki/Benford%27s_law">Benford's law</a>.
			''' </summary>
			''' <param name="inputColumn">  Input column name </param>
			''' <param name="outputColumn"> Output column name. If same as input, input column is replaced </param>
			''' <param name="mode"> See <seealso cref="FirstDigitTransform.Mode"/> </param>
			Public Overridable Function firstDigitTransform(ByVal inputColumn As String, ByVal outputColumn As String, ByVal mode As FirstDigitTransform.Mode) As Builder
				Return transform(New FirstDigitTransform(inputColumn, outputColumn, mode))
			End Function

			''' <summary>
			''' Create the TransformProcess object
			''' </summary>
			Public Overridable Function build() As TransformProcess
				Return New TransformProcess(Me)
			End Function


		End Class


	End Class

End Namespace