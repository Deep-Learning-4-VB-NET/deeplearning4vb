Imports System
Imports Data = lombok.Data
Imports Filter = org.datavec.api.transform.filter.Filter
Imports CalculateSortedRank = org.datavec.api.transform.rank.CalculateSortedRank
Imports IAssociativeReducer = org.datavec.api.transform.reduce.IAssociativeReducer
Imports Schema = org.datavec.api.transform.schema.Schema
Imports ConvertFromSequence = org.datavec.api.transform.sequence.ConvertFromSequence
Imports ConvertToSequence = org.datavec.api.transform.sequence.ConvertToSequence
Imports SequenceSplit = org.datavec.api.transform.sequence.SequenceSplit
Imports JsonInclude = org.nd4j.shade.jackson.annotation.JsonInclude

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
'ORIGINAL LINE: @Data @JsonInclude(JsonInclude.Include.NON_NULL) public class DataAction implements java.io.Serializable
	<Serializable>
	Public Class DataAction

		Private transform As Transform
		Private filter As Filter
		Private convertToSequence As ConvertToSequence
		Private convertFromSequence As ConvertFromSequence
		Private sequenceSplit As SequenceSplit
		Private reducer As IAssociativeReducer
		Private calculateSortedRank As CalculateSortedRank

		Public Sub New()
			'No-arg constructor for Jackson
		End Sub

		Public Sub New(ByVal transform As Transform)
			Me.transform = transform
		End Sub

		Public Sub New(ByVal filter As Filter)
			Me.filter = filter
		End Sub

		Public Sub New(ByVal convertToSequence As ConvertToSequence)
			Me.convertToSequence = convertToSequence
		End Sub

		Public Sub New(ByVal convertFromSequence As ConvertFromSequence)
			Me.convertFromSequence = convertFromSequence
		End Sub

		Public Sub New(ByVal sequenceSplit As SequenceSplit)
			Me.sequenceSplit = sequenceSplit
		End Sub

		Public Sub New(ByVal reducer As IAssociativeReducer)
			Me.reducer = reducer
		End Sub

		Public Sub New(ByVal calculateSortedRank As CalculateSortedRank)
			Me.calculateSortedRank = calculateSortedRank
		End Sub

		Public Overrides Function ToString() As String
			Dim str As String
			If transform IsNot Nothing Then
				str = transform.ToString()
			ElseIf filter IsNot Nothing Then
				str = filter.ToString()
			ElseIf convertToSequence IsNot Nothing Then
				str = convertToSequence.ToString()
			ElseIf convertFromSequence IsNot Nothing Then
				str = convertFromSequence.ToString()
			ElseIf sequenceSplit IsNot Nothing Then
				str = sequenceSplit.ToString()
			ElseIf reducer IsNot Nothing Then
				str = reducer.ToString()
			ElseIf calculateSortedRank IsNot Nothing Then
				str = calculateSortedRank.ToString()
			Else
				Throw New System.InvalidOperationException("Invalid DataAction: does not contain any operation to perform (all fields are null)")
			End If
			Return "DataAction(" & str & ")"
		End Function

		Public Overridable ReadOnly Property Schema As Schema
			Get
				If transform IsNot Nothing Then
					Return transform.InputSchema
				ElseIf filter IsNot Nothing Then
					Return filter.InputSchema
				ElseIf convertToSequence IsNot Nothing Then
					Return convertToSequence.getInputSchema()
				ElseIf convertFromSequence IsNot Nothing Then
					Return convertFromSequence.getInputSchema()
				ElseIf sequenceSplit IsNot Nothing Then
					Return sequenceSplit.InputSchema
				ElseIf reducer IsNot Nothing Then
					Return reducer.InputSchema
				ElseIf calculateSortedRank IsNot Nothing Then
					Return calculateSortedRank.InputSchema
				Else
					Throw New System.InvalidOperationException("Invalid DataAction: does not contain any operation to perform (all fields are null)")
				End If
			End Get
		End Property

	End Class

End Namespace