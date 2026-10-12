Imports Longs = org.nd4j.shade.guava.primitives.Longs
Imports Getter = lombok.Getter
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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
	''' And indexing representing
	''' an interval
	''' 
	''' @author Adam Gibson
	''' </summary>
	Public Class IntervalIndex
		Implements INDArrayIndex

'JAVA TO VB CONVERTER NOTE: The field end was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend begin, end_Conflict As Long
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected boolean inclusive;
		Protected Friend inclusive As Boolean
'JAVA TO VB CONVERTER NOTE: The field stride was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend stride_Conflict As Long = 1
		Protected Friend index As Long = 0
'JAVA TO VB CONVERTER NOTE: The field length was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend length_Conflict As Long = 0

		''' 
		''' <param name="inclusive"> whether to include the last number </param>
		''' <param name="stride"> the stride for the interval </param>
		Public Sub New(ByVal inclusive As Boolean, ByVal stride As Long)
			Me.inclusive = inclusive
			Me.stride_Conflict = stride
		End Sub

		Public Overridable Function [end]() As Long Implements INDArrayIndex.end
			Return end_Conflict
		End Function

		Public Overridable Function offset() As Long Implements INDArrayIndex.offset
			Return begin
		End Function

		Public Overridable Function length() As Long Implements INDArrayIndex.length
			Return length_Conflict
		End Function

		Public Overridable Function stride() As Long Implements INDArrayIndex.stride
			Return stride_Conflict
		End Function

		Public Overridable Sub reverse() Implements INDArrayIndex.reverse
			Dim oldEnd As Long = end_Conflict
			Dim oldBegin As Long = begin
			Me.end_Conflict = oldBegin
			Me.begin = oldEnd
		End Sub

		Public Overridable ReadOnly Property Interval As Boolean Implements INDArrayIndex.isInterval
			Get
				Return True
			End Get
		End Property

		Public Overridable Sub init(ByVal arr As INDArray, ByVal begin As Long, ByVal dimension As Integer) Implements INDArrayIndex.init
			If begin < 0 Then
				begin += arr.size(dimension)
			End If

			Me.begin = begin
			Me.index = begin
			Me.end_Conflict = If(inclusive, arr.size(dimension) + 1, arr.size(dimension))

			'Calculation of length: (endInclusive - begin)/stride + 1
			Dim endInc As Long = arr.size(dimension) - (If(inclusive, 0, 1))
			Me.length_Conflict = (endInc - begin)\stride + 1

			Preconditions.checkState(endInc < arr.size(dimension), "Invalid interval: %s on array with shape %ndShape", Me, arr)
		End Sub

		Public Overridable Sub init(ByVal arr As INDArray, ByVal dimension As Integer) Implements INDArrayIndex.init
			init(arr, 0, dimension)
		End Sub


		Public Overridable Sub init(ByVal begin As Long, ByVal [end] As Long, ByVal max As Long) Implements INDArrayIndex.init
			If begin < 0 Then
				begin += max
			End If

			If [end] < 0 Then
				[end] += max
			End If
			Me.begin = begin
			Me.index = begin
			Me.end_Conflict = [end]

			Dim endInc As Long = [end] - (If(inclusive, 0, 1))
			Me.length_Conflict = (endInc - begin)\stride + 1
		End Sub

		Public Overridable Sub init(ByVal begin As Long, ByVal [end] As Long) Implements INDArrayIndex.init
			If begin < 0 OrElse [end] < 0 Then
				Throw New System.ArgumentException("Please pass in an array for negative indices. Unable to determine size for dimension otherwise")
			End If
			Me.begin = begin
			Me.index = begin
			Me.end_Conflict = [end]

			Dim endInc As Long = [end] - (If(inclusive, 0, 1))
			Me.length_Conflict = (endInc - begin)\stride + 1
		End Sub

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Me Is o Then
				Return True
			End If
			If Not (TypeOf o Is IntervalIndex) Then
				Return False
			End If

			Dim that As IntervalIndex = DirectCast(o, IntervalIndex)

			If begin <> that.begin Then
				Return False
			End If
			If end_Conflict <> that.end_Conflict Then
				Return False
			End If
			If inclusive <> that.inclusive Then
				Return False
			End If
			If stride_Conflict <> that.stride_Conflict Then
				Return False
			End If
			Return index = that.index

		End Function

		Public Overrides Function GetHashCode() As Integer
			Dim result As Integer = Longs.hashCode(begin)
			result = 31 * result + Longs.hashCode(end_Conflict)
			result = 31 * result + (If(inclusive, 1, 0))
			result = 31 * result + Longs.hashCode(stride_Conflict)
			result = 31 * result + Longs.hashCode(index)
			Return result
		End Function

		Public Overrides Function ToString() As String
			Return "Interval(b=" & begin & ",e=" & end_Conflict & ",s=" & stride_Conflict + (If(inclusive, ",inclusive", "")) & ")"
		End Function
	End Class

End Namespace