Imports System
Imports AllArgsConstructor = lombok.AllArgsConstructor
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
'ORIGINAL LINE: @Data @EqualsAndHashCode(exclude = {"area"}, callSuper = false) public class PrecisionRecallCurve extends BaseCurve
	Public Class PrecisionRecallCurve
		Inherits BaseCurve

		Private threshold() As Double
		Private precision() As Double
		Private recall() As Double
		Private tpCount() As Integer
		Private fpCount() As Integer
		Private fnCount() As Integer
		Private totalCount As Integer

		Private area As Double?

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public PrecisionRecallCurve(@JsonProperty("threshold") double[] threshold, @JsonProperty("precision") double[] precision, @JsonProperty("recall") double[] recall, @JsonProperty("tpCount") int[] tpCount, @JsonProperty("fpCount") int[] fpCount, @JsonProperty("fnCount") int[] fnCount, @JsonProperty("totalCount") int totalCount)
		Public Sub New(ByVal threshold() As Double, ByVal precision() As Double, ByVal recall() As Double, ByVal tpCount() As Integer, ByVal fpCount() As Integer, ByVal fnCount() As Integer, ByVal totalCount As Integer)
			Me.threshold = threshold
			Me.precision = precision
			Me.recall = recall
			Me.tpCount = tpCount
			Me.fpCount = fpCount
			Me.fnCount = fnCount
			Me.totalCount = totalCount
		End Sub

		Public Overrides Function numPoints() As Integer
			Return threshold.Length
		End Function

		Public Overrides ReadOnly Property X As Double()
			Get
				Return recall
			End Get
		End Property

		Public Overrides ReadOnly Property Y As Double()
			Get
				Return precision
			End Get
		End Property

		Public Overrides ReadOnly Property Title As String
			Get
				Return "Precision-Recall Curve (Area=" & format(calculateAUPRC(), DEFAULT_FORMAT_PREC) & ")"
			End Get
		End Property

		''' <param name="i"> Point number, 0 to numPoints()-1 inclusive </param>
		''' <returns> Threshold of a given point </returns>
		Public Overridable Function getThreshold(ByVal i As Integer) As Double
			Preconditions.checkArgument(i >= 0 AndAlso i < threshold.Length, "Invalid index: " & i)
			Return threshold(i)
		End Function

		''' <param name="i"> Point number, 0 to numPoints()-1 inclusive </param>
		''' <returns> Precision of a given point </returns>
		Public Overridable Function getPrecision(ByVal i As Integer) As Double
			Preconditions.checkArgument(i >= 0 AndAlso i < precision.Length, "Invalid index: " & i)
			Return precision(i)
		End Function

		''' <param name="i"> Point number, 0 to numPoints()-1 inclusive </param>
		''' <returns> Recall of a given point </returns>
		Public Overridable Function getRecall(ByVal i As Integer) As Double
			Preconditions.checkArgument(i >= 0 AndAlso i < recall.Length, "Invalid index: " & i)
			Return recall(i)
		End Function

		''' <returns> The area under the precision recall curve </returns>
		Public Overridable Function calculateAUPRC() As Double
			If area IsNot Nothing Then
				Return area
			End If

			area = calculateArea()
			Return area
		End Function

		''' <summary>
		''' Get the point (index, threshold, precision, recall) at the given threshold.<br>
		''' Note that if the threshold is not found exactly, the next highest threshold exceeding the requested threshold
		''' is returned
		''' </summary>
		''' <param name="threshold"> Threshold to get the point for </param>
		''' <returns> point (index, threshold, precision, recall) at the given threshold </returns>
		Public Overridable Function getPointAtThreshold(ByVal threshold As Double) As Point

			'Return (closest) point number, precision, recall, whether it's interpolated or not

			'Binary search to find closest threshold

			Dim idx As Integer = Array.BinarySearch(Me.threshold, threshold)
			If idx < 0 Then
				'Not found (usual case). binarySearch javadoc:
	'            
	'            index of the search key, if it is contained in the array;
	'            otherwise, (-(insertion point) - 1).  The
	'            insertion point is defined as the point at which the
	'            key would be inserted into the array: the index of the first
	'            element greater than the key, or a.length if all
	'            elements in the array are less than the specified key.
	'            
				idx = -idx - 1
			End If

			'At this point: idx = exact, on the next highest
			Dim thr As Double = Me.threshold(idx)
			Dim pr As Double = precision(idx)
			Dim rec As Double = recall(idx)

			Return New Point(idx, thr, pr, rec)
		End Function

		''' <summary>
		''' Get the point (index, threshold, precision, recall) at the given precision.<br>
		''' Specifically, return the points at the lowest threshold that has precision equal to or greater than the
		''' requested precision.
		''' </summary>
		''' <param name="precision"> Precision to get the point for </param>
		''' <returns> point (index, threshold, precision, recall) at (or closest exceeding) the given precision </returns>
		Public Overridable Function getPointAtPrecision(ByVal precision As Double) As Point
			'Find the LOWEST threshold that gives the specified precision

			For i As Integer = 0 To Me.precision.Length - 1
				If Me.precision(i) >= precision Then
					Return New Point(i, threshold(i), Me.precision(i), recall(i))
				End If
			Next i

			'Not found, return last point. Should never happen though...
			Dim i As Integer = threshold.Length - 1
			Return New Point(i, threshold(i), Me.precision(i), Me.recall(i))
		End Function

		''' <summary>
		''' Get the point (index, threshold, precision, recall) at the given recall.<br>
		''' Specifically, return the points at the highest threshold that has recall equal to or greater than the
		''' requested recall.
		''' </summary>
		''' <param name="recall"> Recall to get the point for </param>
		''' <returns> point (index, threshold, precision, recall) at (or closest exceeding) the given recall </returns>
		Public Overridable Function getPointAtRecall(ByVal recall As Double) As Point
			Dim foundPoint As Point = Nothing
			'Find the HIGHEST threshold that gives the specified recall
			For i As Integer = Me.recall.Length - 1 To 0 Step -1
					If Me.recall(i) >= recall Then
							If foundPoint Is Nothing OrElse (Me.recall(i) = foundPoint.getRecall() AndAlso Me.precision(i) >= foundPoint.getPrecision()) Then
									foundPoint = New Point(i, threshold(i), precision(i), Me.recall(i))
							End If
					End If
			Next i
			If foundPoint Is Nothing Then
				'Not found - return first point. Should never happen...
				foundPoint = New Point(0, threshold(0), precision(0), Me.recall(0))
			End If
			Return foundPoint
		End Function

		''' <summary>
		''' Get the binary confusion matrix for the given threshold. As per <seealso cref="getPointAtThreshold(Double)"/>,
		''' if the threshold is not found exactly, the next highest threshold exceeding the requested threshold
		''' is returned
		''' </summary>
		''' <param name="threshold"> Threshold at which to get the confusion matrix </param>
		''' <returns> Binary confusion matrix </returns>
		Public Overridable Function getConfusionMatrixAtThreshold(ByVal threshold As Double) As Confusion
			Dim p As Point = getPointAtThreshold(threshold)
			Dim idx As Integer = p.idx
			Dim tn As Integer = totalCount - (tpCount(idx) + fpCount(idx) + fnCount(idx))
			Return New Confusion(p, tpCount(idx), fpCount(idx), fnCount(idx), tn)
		End Function

		''' <summary>
		''' Get the binary confusion matrix for the given position. As per <seealso cref="getPointAtThreshold(Double)"/>.
		''' </summary>
		''' <param name="point"> Position at which to get the binary confusion matrix </param>
		''' <returns> Binary confusion matrix </returns>
		Public Overridable Function getConfusionMatrixAtPoint(ByVal point As Integer) As Confusion
			Return getConfusionMatrixAtThreshold(threshold(point))
		End Function

		Public Shared Function fromJson(ByVal json As String) As PrecisionRecallCurve
			Return fromJson(json, GetType(PrecisionRecallCurve))
		End Function

		Public Shared Function fromYaml(ByVal yaml As String) As PrecisionRecallCurve
			Return fromYaml(yaml, GetType(PrecisionRecallCurve))
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data public static class Point
		Public Class Point
			Friend ReadOnly idx As Integer
			Friend ReadOnly threshold As Double
			Friend ReadOnly precision As Double
			Friend ReadOnly recall As Double
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data public static class Confusion
		Public Class Confusion
			Friend ReadOnly point As Point
			Friend ReadOnly tpCount As Integer
			Friend ReadOnly fpCount As Integer
			Friend ReadOnly fnCount As Integer
			Friend ReadOnly tnCount As Integer
		End Class
	End Class

End Namespace