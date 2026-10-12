Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Preconditions = org.nd4j.common.base.Preconditions
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
'ORIGINAL LINE: @Data @EqualsAndHashCode(exclude = {"auc"}, callSuper = false) public class RocCurve extends BaseCurve
	Public Class RocCurve
		Inherits BaseCurve

		Private threshold() As Double
		Private fpr() As Double
		Private tpr() As Double

		Private auc As Double?

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public RocCurve(@JsonProperty("threshold") double[] threshold, @JsonProperty("fpr") double[] fpr, @JsonProperty("tpr") double[] tpr)
		Public Sub New(ByVal threshold() As Double, ByVal fpr() As Double, ByVal tpr() As Double)
			Me.threshold = threshold
			Me.fpr = fpr
			Me.tpr = tpr
		End Sub


		Public Overrides Function numPoints() As Integer
			Return threshold.Length
		End Function

		Public Overrides ReadOnly Property X As Double()
			Get
				Return fpr
			End Get
		End Property

		Public Overrides ReadOnly Property Y As Double()
			Get
				Return tpr
			End Get
		End Property

		Public Overrides ReadOnly Property Title As String
			Get
				Return "ROC (Area=" & format(calculateAUC(), DEFAULT_FORMAT_PREC) & ")"
			End Get
		End Property

		''' <param name="i"> Point number, 0 to numPoints()-1 inclusive </param>
		''' <returns> Threshold of a given point </returns>
		Public Overridable Function getThreshold(ByVal i As Integer) As Double
			Preconditions.checkArgument(i >= 0 AndAlso i < threshold.Length, "Invalid index: " & i)
			Return threshold(i)
		End Function

		''' <param name="i"> Point number, 0 to numPoints()-1 inclusive </param>
		''' <returns> True positive rate of a given point </returns>
		Public Overridable Function getTruePositiveRate(ByVal i As Integer) As Double
			Preconditions.checkArgument(i >= 0 AndAlso i < tpr.Length, "Invalid index: " & i)
			Return tpr(i)
		End Function

		''' <param name="i"> Point number, 0 to numPoints()-1 inclusive </param>
		''' <returns> False positive rate of a given point </returns>
		Public Overridable Function getFalsePositiveRate(ByVal i As Integer) As Double
			Preconditions.checkArgument(i >= 0 AndAlso i < fpr.Length, "Invalid index: " & i)
			Return fpr(i)
		End Function

		''' <summary>
		''' Calculate and return the area under ROC curve
		''' </summary>
		Public Overridable Function calculateAUC() As Double
			If auc IsNot Nothing Then
				Return auc
			End If

			auc = calculateArea()
			Return auc
		End Function

		Public Shared Function fromJson(ByVal json As String) As RocCurve
			Return fromJson(json, GetType(RocCurve))
		End Function

		Public Shared Function fromYaml(ByVal yaml As String) As RocCurve
			Return fromYaml(yaml, GetType(RocCurve))
		End Function

	End Class

End Namespace