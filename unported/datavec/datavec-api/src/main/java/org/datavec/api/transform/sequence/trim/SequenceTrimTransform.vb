Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Transform = org.datavec.api.transform.Transform
Imports Schema = org.datavec.api.transform.schema.Schema
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

Namespace org.datavec.api.transform.sequence.trim


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonIgnoreProperties({"schema"}) @EqualsAndHashCode(exclude = {"schema"}) @Data public class SequenceTrimTransform implements org.datavec.api.transform.Transform
	<Serializable>
	Public Class SequenceTrimTransform
		Implements Transform

		Private numStepsToTrim As Integer
		Private trimFromStart As Boolean
		Private schema As Schema

		''' 
		''' <param name="numStepsToTrim"> Number of time steps to trim from the sequence </param>
		''' <param name="trimFromStart">  If true: Trim values from the start of the sequence. If false: trim values from the end. </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SequenceTrimTransform(@JsonProperty("numStepsToTrim") int numStepsToTrim, @JsonProperty("trimFromStart") boolean trimFromStart)
		Public Sub New(ByVal numStepsToTrim As Integer, ByVal trimFromStart As Boolean)
			Me.numStepsToTrim = numStepsToTrim
			Me.trimFromStart = trimFromStart
		End Sub

		Public Overrides Function transform(ByVal inputSchema As Schema) As Schema
			Return inputSchema 'No op
		End Function

		Public Overridable Property InputSchema As Schema
			Set(ByVal inputSchema As Schema)
				Me.schema = inputSchema
			End Set
			Get
				Return schema
			End Get
		End Property


		Public Overridable Function outputColumnName() As String
			Return Nothing
		End Function

		Public Overridable Function outputColumnNames() As String()
			Return CType(schema.getColumnNames(), List(Of String)).ToArray()
		End Function

		Public Overridable Function columnNames() As String()
			Return outputColumnNames()
		End Function

		Public Overridable Function columnName() As String
			Return outputColumnName()
		End Function

		Public Overridable Function map(ByVal writables As IList(Of Writable)) As IList(Of Writable) Implements Transform.map
			Throw New System.NotSupportedException("SequenceTrimTransform cannot be applied to non-sequence values")
		End Function

		Public Overridable Function mapSequence(ByVal sequence As IList(Of IList(Of Writable))) As IList(Of IList(Of Writable)) Implements Transform.mapSequence
			Dim start As Integer = 0
			Dim [end] As Integer = sequence.Count
			If trimFromStart Then
				start += numStepsToTrim
			Else
				[end] -= numStepsToTrim
			End If

			If [end] < start Then
				Return Collections.emptyList()
			End If

			Dim [out] As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))([end] - start)

			For i As Integer = start To [end] - 1
				[out].Add(sequence(i))
			Next i

			Return [out]
		End Function

		Public Overridable Function map(ByVal input As Object) As Object Implements Transform.map
			Throw New System.NotSupportedException("SequenceTrimTransform cannot be applied to non-sequence values")
		End Function

		Public Overridable Function mapSequence(ByVal sequence As Object) As Object Implements Transform.mapSequence
			Throw New System.NotSupportedException("Not yet implemented")
		End Function
	End Class

End Namespace