Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Schema = org.datavec.api.transform.schema.Schema
Imports SequenceSplit = org.datavec.api.transform.sequence.SequenceSplit
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

Namespace org.datavec.api.transform.sequence.split


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode(exclude = {"inputSchema"}) @JsonIgnoreProperties({"inputSchema"}) @Data public class SplitMaxLengthSequence implements org.datavec.api.transform.sequence.SequenceSplit
	<Serializable>
	Public Class SplitMaxLengthSequence
		Implements SequenceSplit

		Private ReadOnly maxSequenceLength As Integer
		Private ReadOnly equalSplits As Boolean
'JAVA TO VB CONVERTER NOTE: The field inputSchema was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private inputSchema_Conflict As Schema

		''' <param name="maxSequenceLength"> max length of sequences </param>
		''' <param name="equalSplits">       if true: split larger sequences into equal sized subsequences. If false: split into
		'''                          n maxSequenceLength sequences, and (if necessary) 1 with 1 <= length < maxSequenceLength </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SplitMaxLengthSequence(@JsonProperty("maxSequenceLength") int maxSequenceLength, @JsonProperty("equalSplits") boolean equalSplits)
		Public Sub New(ByVal maxSequenceLength As Integer, ByVal equalSplits As Boolean)
			Me.maxSequenceLength = maxSequenceLength
			Me.equalSplits = equalSplits
		End Sub

		Public Overridable Function split(ByVal sequence As IList(Of IList(Of Writable))) As IList(Of IList(Of IList(Of Writable))) Implements SequenceSplit.split
			Dim n As Integer = sequence.Count
			If n <= maxSequenceLength Then
				Return Collections.singletonList(sequence)
			End If
			Dim splitSize As Integer
			If equalSplits Then
				If n Mod maxSequenceLength = 0 Then
					splitSize = n \ maxSequenceLength
				Else
					splitSize = n \ maxSequenceLength + 1
				End If
			Else
				splitSize = maxSequenceLength
			End If

			Dim [out] As IList(Of IList(Of IList(Of Writable))) = New List(Of IList(Of IList(Of Writable)))()
			Dim current As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))(splitSize)
			For Each [step] As IList(Of Writable) In sequence
				If current.Count >= splitSize Then
					[out].Add(current)
					current = New List(Of IList(Of Writable))(splitSize)
				End If
				current.Add([step])
			Next [step]
			[out].Add(current)

			Return [out]
		End Function

		Public Overridable Property InputSchema Implements SequenceSplit.setInputSchema As Schema
			Set(ByVal inputSchema As Schema)
				Me.inputSchema_Conflict = inputSchema
			End Set
			Get
				Return inputSchema_Conflict
			End Get
		End Property


		Public Overrides Function ToString() As String
			Return "SplitMaxLengthSequence(maxSequenceLength=" & maxSequenceLength & ",equalSplits=" & equalSplits & ")"
		End Function
	End Class

End Namespace