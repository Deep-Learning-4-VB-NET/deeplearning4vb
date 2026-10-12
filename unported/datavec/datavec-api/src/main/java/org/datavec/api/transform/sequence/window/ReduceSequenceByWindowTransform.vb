Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Transform = org.datavec.api.transform.Transform
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports org.datavec.api.transform.ops
Imports IAssociativeReducer = org.datavec.api.transform.reduce.IAssociativeReducer
Imports Schema = org.datavec.api.transform.schema.Schema
Imports SequenceSchema = org.datavec.api.transform.schema.SequenceSchema
Imports Writable = org.datavec.api.writable.Writable
Imports JsonIgnoreProperties = org.nd4j.shade.jackson.annotation.JsonIgnoreProperties
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty

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

Namespace org.datavec.api.transform.sequence.window


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonIgnoreProperties({"inputSchema"}) @EqualsAndHashCode(exclude = {"inputSchema"}) @Data public class ReduceSequenceByWindowTransform implements org.datavec.api.transform.Transform
	<Serializable>
	Public Class ReduceSequenceByWindowTransform
		Implements Transform

		Private reducer As IAssociativeReducer
		Private windowFunction As WindowFunction
'JAVA TO VB CONVERTER NOTE: The field inputSchema was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private inputSchema_Conflict As Schema

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ReduceSequenceByWindowTransform(@JsonProperty("reducer") org.datavec.api.transform.reduce.IAssociativeReducer reducer, @JsonProperty("windowFunction") WindowFunction windowFunction)
		Public Sub New(ByVal reducer As IAssociativeReducer, ByVal windowFunction As WindowFunction)
			Me.reducer = reducer
			Me.windowFunction = windowFunction
		End Sub


		Public Overrides Function transform(ByVal inputSchema As Schema) As Schema
			If inputSchema IsNot Nothing AndAlso Not (TypeOf inputSchema Is SequenceSchema) Then
				Throw New System.ArgumentException("Invalid input: input schema must be a SequenceSchema")
			End If

			'Some window functions may make changes to the schema (adding window start/end times, for example)
			inputSchema = windowFunction.transform(inputSchema)

			'Approach here: The reducer gives us a schema for one time step -> simply convert this to a sequence schema...
			Dim oneStepSchema As Schema = reducer.transform(inputSchema)
			Dim meta As IList(Of ColumnMetaData) = oneStepSchema.getColumnMetaData()

			Return New SequenceSchema(meta)
		End Function

		Public Overridable Property InputSchema As Schema
			Set(ByVal inputSchema As Schema)
				Me.inputSchema_Conflict = inputSchema
				Me.windowFunction.InputSchema = inputSchema
				reducer.InputSchema = windowFunction.transform(inputSchema)
			End Set
			Get
				Return inputSchema_Conflict
			End Get
		End Property


		Public Overridable Function map(ByVal writables As IList(Of Writable)) As IList(Of Writable) Implements Transform.map
			Throw New System.NotSupportedException("ReduceSequenceByWindownTransform can only be applied on sequences")
		End Function

		Public Overridable Function mapSequence(ByVal sequence As IList(Of IList(Of Writable))) As IList(Of IList(Of Writable)) Implements Transform.mapSequence
			'List of windows, which are all small sequences...
			Dim sequenceAsWindows As IList(Of IList(Of IList(Of Writable))) = windowFunction.applyToSequence(sequence)

			Dim [out] As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()

			For Each window As IList(Of IList(Of Writable)) In sequenceAsWindows
				Dim accu As IAggregableReduceOp(Of IList(Of Writable), IList(Of Writable)) = reducer.aggregableReducer()
				For Each l As IList(Of Writable) In window
					accu.accept(l)
				Next l
				[out].Add(accu.get())
			Next window

			Return [out]
		End Function

		''' <summary>
		''' Transform an object
		''' in to another object
		''' </summary>
		''' <param name="input"> the record to transform </param>
		''' <returns> the transformed writable </returns>
		Public Overridable Function map(ByVal input As Object) As Object Implements Transform.map
			Throw New System.NotSupportedException("ReduceSequenceByWindownTransform can only be applied on sequences")
		End Function

		''' <summary>
		''' Transform a sequence
		''' </summary>
		''' <param name="sequence"> </param>
		Public Overridable Function mapSequence(ByVal sequence As Object) As Object Implements Transform.mapSequence
			Throw New System.NotSupportedException("Needs to be implemented")
		End Function

		Public Overrides Function ToString() As String
			Return "ReduceSequenceByWindowTransform(reducer=" & reducer & ",windowFunction=" & windowFunction & ")"
		End Function

		''' <summary>
		''' The output column name
		''' after the operation has been applied
		''' </summary>
		''' <returns> the output column name </returns>
		Public Overridable Function outputColumnName() As String
			Return outputColumnNames()(0)
		End Function

		''' <summary>
		''' The output column names
		''' This will often be the same as the input
		''' </summary>
		''' <returns> the output column names </returns>
		Public Overridable Function outputColumnNames() As String()
			Return columnNames()
		End Function

		''' <summary>
		''' Returns column names
		''' this op is meant to run on
		''' 
		''' @return
		''' </summary>
		Public Overridable Function columnNames() As String()
			Return InputSchema.getColumnNames().toArray(New String(InputSchema.numColumns() - 1){})
		End Function

		''' <summary>
		''' Returns a singular column name
		''' this op is meant to run on
		''' 
		''' @return
		''' </summary>
		Public Overridable Function columnName() As String
			Return columnNames()(0)
		End Function
	End Class

End Namespace