Imports Data = lombok.Data
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

Namespace org.nd4j.evaluation.curves

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class Histogram extends BaseHistogram
	Public Class Histogram
		Inherits BaseHistogram

		Private ReadOnly title As String
		Private ReadOnly lower As Double
		Private ReadOnly upper As Double
		Private ReadOnly binCounts() As Integer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Histogram(@JsonProperty("title") String title, @JsonProperty("lower") double lower, @JsonProperty("upper") double upper, @JsonProperty("binCounts") int[] binCounts)
		Public Sub New(ByVal title As String, ByVal lower As Double, ByVal upper As Double, ByVal binCounts() As Integer)
			Me.title = title
			Me.lower = lower
			Me.upper = upper
			Me.binCounts = binCounts
		End Sub

		Public Overrides Function numPoints() As Integer
			Return binCounts.Length
		End Function

		Public Overrides ReadOnly Property BinLowerBounds As Double()
			Get
				Dim [step] As Double = 1.0 / binCounts.Length
				Dim [out](binCounts.Length - 1) As Double
				For i As Integer = 0 To [out].Length - 1
					[out](i) = i * [step]
				Next i
				Return [out]
			End Get
		End Property

		Public Overrides ReadOnly Property BinUpperBounds As Double()
			Get
				Dim [step] As Double = 1.0 / binCounts.Length
				Dim [out](binCounts.Length - 1) As Double
				For i As Integer = 0 To [out].Length - 2
					[out](i) = (i + 1) * [step]
				Next i
				[out]([out].Length - 1) = 1.0
				Return [out]
			End Get
		End Property

		Public Overrides ReadOnly Property BinMidValues As Double()
			Get
				Dim [step] As Double = 1.0 / binCounts.Length
				Dim [out](binCounts.Length - 1) As Double
				For i As Integer = 0 To [out].Length - 1
					[out](i) = (i + 0.5) * [step]
				Next i
				Return [out]
			End Get
		End Property

		''' <param name="json">       JSON representation </param>
		''' <returns>           Instance of the histogram </returns>
		Public Shared Function fromJson(ByVal json As String) As Histogram
			Return BaseHistogram.fromJson(json, GetType(Histogram))
		End Function

		''' 
		''' <param name="yaml">       YAML representation </param>
		''' <returns>           Instance of the histogram </returns>
		Public Shared Function fromYaml(ByVal yaml As String) As Histogram
			Return BaseHistogram.fromYaml(yaml, GetType(Histogram))
		End Function
	End Class

End Namespace