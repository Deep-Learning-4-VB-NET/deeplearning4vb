Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Transform = org.datavec.api.transform.Transform
Imports Schema = org.datavec.api.transform.schema.Schema
Imports Writable = org.datavec.api.writable.Writable
Imports Preconditions = org.nd4j.common.base.Preconditions
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
'ORIGINAL LINE: @JsonIgnoreProperties({"schema"}) @EqualsAndHashCode(exclude = {"schema"}) @Data public class SequenceTrimToLengthTransform implements org.datavec.api.transform.Transform
	<Serializable>
	Public Class SequenceTrimToLengthTransform
		Implements Transform

		''' <summary>
		''' Mode. See <seealso cref="SequenceTrimToLengthTransform"/>
		''' </summary>
		Public Enum Mode
			TRIM
			TRIM_OR_PAD

		End Enum
		Private maxLength As Integer
		Private mode As Mode
		Private pad As IList(Of Writable)

		Private schema As Schema

		''' <param name="maxLength"> maximum sequence length. Must be positive. </param>
		''' <param name="mode">      Mode - trim or trim/pad </param>
		''' <param name="pad">       Padding value. Only used with Mode.TRIM_OR_PAD. Must be equal in length to the number of columns (values per time step) </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SequenceTrimToLengthTransform(@JsonProperty("maxLength") int maxLength, @JsonProperty("mode") Mode mode, @JsonProperty("pad") java.util.List<org.datavec.api.writable.Writable> pad)
		Public Sub New(ByVal maxLength As Integer, ByVal mode As Mode, ByVal pad As IList(Of Writable))
			Preconditions.checkState(maxLength > 0, "Maximum length must be > 0, got %s", maxLength)
			Preconditions.checkState(mode = Mode.TRIM OrElse pad IsNot Nothing, "If mode == Mode.TRIM_OR_PAD ")
			Me.maxLength = maxLength
			Me.mode = mode
			Me.pad = pad
		End Sub

		Public Overridable Function map(ByVal writables As IList(Of Writable)) As IList(Of Writable) Implements Transform.map
			Throw New System.NotSupportedException("SequenceTrimToLengthTransform cannot be applied to non-sequence values")
		End Function

		Public Overridable Function mapSequence(ByVal sequence As IList(Of IList(Of Writable))) As IList(Of IList(Of Writable)) Implements Transform.mapSequence
			If mode = Mode.TRIM Then
				If sequence.Count <= maxLength Then
					Return sequence
				End If
				Return New List(Of IList(Of Writable))(sequence.subList(0, maxLength))
			Else
				'Trim or pad
				If sequence.Count = maxLength Then
					Return sequence
				ElseIf sequence.Count > maxLength Then
					Return New List(Of IList(Of Writable))(sequence.subList(0, maxLength))
				Else
					'Need to pad
					Preconditions.checkState(sequence.Count = 0 OrElse sequence(0).Count = pad.Count, "Invalid padding values: %s padding " & "values were provided, but data has %s values per time step (columns)", pad.Count, sequence(0).Count)

					Dim [out] As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))(maxLength)
					CType([out], List(Of IList(Of Writable))).AddRange(sequence)
					Do While [out].Count < maxLength
						[out].Add(pad)
					Loop
					Return [out]
				End If
			End If
		End Function

		Public Overridable Function map(ByVal input As Object) As Object Implements Transform.map
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function mapSequence(ByVal sequence As Object) As Object Implements Transform.mapSequence
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Function transform(ByVal inputSchema As Schema) As Schema
			Return inputSchema
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
			Return Nothing
		End Function
	End Class

End Namespace