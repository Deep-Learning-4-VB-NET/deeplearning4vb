Imports System
Imports JsonCreator = org.nd4j.shade.jackson.annotation.JsonCreator
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

Namespace org.deeplearning4j.nn.conf.distribution

	<Serializable>
	Public Class BinomialDistribution
		Inherits Distribution

		Private Const serialVersionUID As Long = 7407024251874318749L

'JAVA TO VB CONVERTER NOTE: The field numberOfTrials was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly numberOfTrials_Conflict As Integer
'JAVA TO VB CONVERTER NOTE: The field probabilityOfSuccess was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private probabilityOfSuccess_Conflict As Double

		''' <summary>
		''' Create a distribution
		''' </summary>
		''' <param name="numberOfTrials"> the number of trials </param>
		''' <param name="probabilityOfSuccess"> the probability of success </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonCreator public BinomialDistribution(@JsonProperty("numberOfTrials") int numberOfTrials, @JsonProperty("probabilityOfSuccess") double probabilityOfSuccess)
		Public Sub New(ByVal numberOfTrials As Integer, ByVal probabilityOfSuccess As Double)
			Me.numberOfTrials_Conflict = numberOfTrials
			Me.probabilityOfSuccess_Conflict = probabilityOfSuccess
		End Sub

		Public Overridable Property ProbabilityOfSuccess As Double
			Get
				Return probabilityOfSuccess_Conflict
			End Get
			Set(ByVal probabilityOfSuccess As Double)
				Me.probabilityOfSuccess_Conflict = probabilityOfSuccess
			End Set
		End Property


		Public Overridable ReadOnly Property NumberOfTrials As Integer
			Get
				Return numberOfTrials_Conflict
			End Get
		End Property

		Public Overrides Function GetHashCode() As Integer
			Const prime As Integer = 31
			Dim result As Integer = 1
			result = prime * result + numberOfTrials_Conflict
			Dim temp As Long
			temp = System.BitConverter.DoubleToInt64Bits(probabilityOfSuccess_Conflict)
			result = prime * result + CInt(temp Xor (CLng(CULng(temp) >> 32)))
			Return result
		End Function

		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If Me Is obj Then
				Return True
			End If
			If obj Is Nothing Then
				Return False
			End If
			If Me.GetType() <> obj.GetType() Then
				Return False
			End If
			Dim other As BinomialDistribution = DirectCast(obj, BinomialDistribution)
			If numberOfTrials_Conflict <> other.numberOfTrials_Conflict Then
				Return False
			End If
			If System.BitConverter.DoubleToInt64Bits(probabilityOfSuccess_Conflict) <> System.BitConverter.DoubleToInt64Bits(other.probabilityOfSuccess_Conflict) Then
				Return False
			End If
			Return True
		End Function

		Public Overrides Function ToString() As String
			Return "BinomialDistribution(" & "numberOfTrials=" & numberOfTrials_Conflict & ", probabilityOfSuccess=" & probabilityOfSuccess_Conflict & ")"c
		End Function
	End Class

End Namespace