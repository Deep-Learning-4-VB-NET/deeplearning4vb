Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
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
'ORIGINAL LINE: @Getter public class ReliabilityDiagram extends BaseCurve
	Public Class ReliabilityDiagram
		Inherits BaseCurve

'JAVA TO VB CONVERTER NOTE: The field title was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly title_Conflict As String
		Private ReadOnly meanPredictedValueX() As Double
		Private ReadOnly fractionPositivesY() As Double


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ReliabilityDiagram(@JsonProperty("title") String title, @NonNull @JsonProperty("meanPredictedValueX") double[] meanPredictedValueX, @NonNull @JsonProperty("fractionPositivesY") double[] fractionPositivesY)
		Public Sub New(ByVal title As String, ByVal meanPredictedValueX() As Double, ByVal fractionPositivesY() As Double)
			Me.title_Conflict = title
			Me.meanPredictedValueX = meanPredictedValueX
			Me.fractionPositivesY = fractionPositivesY
		End Sub

		Public Overrides Function numPoints() As Integer
			Return meanPredictedValueX.Length
		End Function

		Public Overrides ReadOnly Property X As Double()
			Get
				Return getMeanPredictedValueX()
			End Get
		End Property

		Public Overrides ReadOnly Property Y As Double()
			Get
				Return getFractionPositivesY()
			End Get
		End Property

		Public Overrides ReadOnly Property Title As String
			Get
				If title_Conflict Is Nothing Then
					Return "Reliability Diagram"
				End If
				Return title_Conflict
			End Get
		End Property
	End Class

End Namespace