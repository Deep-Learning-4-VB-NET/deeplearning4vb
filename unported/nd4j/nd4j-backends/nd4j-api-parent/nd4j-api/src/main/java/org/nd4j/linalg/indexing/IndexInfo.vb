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

Namespace org.nd4j.linalg.indexing

	''' <summary>
	''' @author Adam Gibson
	''' </summary>
	Public Class IndexInfo
		Private indexes() As INDArrayIndex
		Private point() As Boolean
		Private newAxis() As Boolean
'JAVA TO VB CONVERTER NOTE: The field numNewAxes was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private numNewAxes_Conflict As Integer = 0
'JAVA TO VB CONVERTER NOTE: The field numPoints was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private numPoints_Conflict As Integer = 0

		Public Sub New(ParamArray ByVal indexes() As INDArrayIndex)
			Me.indexes = indexes
			For i As Integer = 0 To indexes.Length - 1
				If TypeOf indexes(i) Is PointIndex Then
					numPoints_Conflict += 1
				End If
				If TypeOf indexes(i) Is IntervalIndex Then

				End If
				If TypeOf indexes(i) Is NewAxis Then
					numNewAxes_Conflict += 1
				End If
			Next i

		End Sub

		Public Overridable ReadOnly Property NumNewAxes As Integer
			Get
				Return numNewAxes_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property NumPoints As Integer
			Get
				Return numPoints_Conflict
			End Get
		End Property
	End Class

End Namespace