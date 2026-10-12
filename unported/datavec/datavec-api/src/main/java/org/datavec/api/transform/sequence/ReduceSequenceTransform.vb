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

Namespace org.datavec.api.transform.sequence


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonIgnoreProperties({"inputSchema"}) @EqualsAndHashCode(exclude = {"inputSchema"}) @Data public class ReduceSequenceTransform implements org.datavec.api.transform.Transform
	<Serializable>
	Public Class ReduceSequenceTransform
		Implements Transform

		Private reducer As IAssociativeReducer
'JAVA TO VB CONVERTER NOTE: The field inputSchema was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private inputSchema_Conflict As Schema

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ReduceSequenceTransform(@JsonProperty("reducer") org.datavec.api.transform.reduce.IAssociativeReducer reducer)
		Public Sub New(ByVal reducer As IAssociativeReducer)
			Me.reducer = reducer
		End Sub


		Public Overrides Function transform(ByVal inputSchema As Schema) As Schema
			If inputSchema IsNot Nothing AndAlso Not (TypeOf inputSchema Is SequenceSchema) Then
				Throw New System.ArgumentException("Invalid input: input schema must be a SequenceSchema")
			End If

			'Approach here: The reducer gives us a schema for one time step -> simply convert this to a sequence schema...
			Dim oneStepSchema As Schema = reducer.transform(inputSchema)
			Dim meta As IList(Of ColumnMetaData) = oneStepSchema.getColumnMetaData()

			Return New SequenceSchema(meta)
		End Function

		Public Overridable Property InputSchema As Schema
			Set(ByVal inputSchema As Schema)
				Me.inputSchema_Conflict = inputSchema
				reducer.InputSchema = inputSchema
			End Set
			Get
				Return inputSchema_Conflict
			End Get
		End Property


		Public Overridable Function map(ByVal writables As IList(Of Writable)) As IList(Of Writable) Implements Transform.map
			Throw New System.NotSupportedException("ReduceSequenceTransform can only be applied on sequences")
		End Function

		Public Overridable Function mapSequence(ByVal sequence As IList(Of IList(Of Writable))) As IList(Of IList(Of Writable)) Implements Transform.mapSequence
			Dim accu As IAggregableReduceOp(Of IList(Of Writable), IList(Of Writable)) = reducer.aggregableReducer()
			For Each l As IList(Of Writable) In sequence
				accu.accept(l)
			Next l
			Return Collections.singletonList(accu.get())
		End Function

		''' <summary>
		''' Transform an object
		''' in to another object
		''' </summary>
		''' <param name="input"> the record to transform </param>
		''' <returns> the transformed writable </returns>
		Public Overridable Function map(ByVal input As Object) As Object Implements Transform.map
			Throw New System.NotSupportedException("ReduceSequenceTransform can only be applied on sequences")
		End Function

		''' <summary>
		''' Transform a sequence
		''' </summary>
		''' <param name="sequence"> </param>
		Public Overridable Function mapSequence(ByVal sequence As Object) As Object Implements Transform.mapSequence
			Throw New System.NotSupportedException("Needs to be implemented")
		End Function

		Public Overrides Function ToString() As String
			Return "ReduceSequenceTransform(reducer=" & reducer & ")"
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