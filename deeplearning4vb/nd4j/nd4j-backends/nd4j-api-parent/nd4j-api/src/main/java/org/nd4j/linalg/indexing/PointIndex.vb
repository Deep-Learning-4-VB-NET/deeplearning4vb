Imports Longs = org.nd4j.shade.guava.primitives.Longs
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
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
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode public class PointIndex implements INDArrayIndex
	Public Class PointIndex
		Implements INDArrayIndex

		Private point As Long

		''' 
		''' <param name="point"> </param>
		Public Sub New(ByVal point As Long)
			Me.point = point
		End Sub

		Public Overridable Function [end]() As Long Implements INDArrayIndex.end
			Return point
		End Function

		Public Overridable Function offset() As Long Implements INDArrayIndex.offset
			Return point
		End Function

		Public Overridable Function length() As Long Implements INDArrayIndex.length
			Return 1
		End Function

		Public Overridable Function stride() As Long Implements INDArrayIndex.stride
			Return 1
		End Function

		Public Overridable Sub reverse() Implements INDArrayIndex.reverse

		End Sub

		Public Overridable ReadOnly Property Interval As Boolean Implements INDArrayIndex.isInterval
			Get
				Return False
			End Get
		End Property

		Public Overridable Sub init(ByVal arr As INDArray, ByVal begin As Long, ByVal dimension As Integer) Implements INDArrayIndex.init

		End Sub

		Public Overridable Sub init(ByVal arr As INDArray, ByVal dimension As Integer) Implements INDArrayIndex.init

		End Sub

		Public Overridable Sub init(ByVal begin As Long, ByVal [end] As Long, ByVal max As Long) Implements INDArrayIndex.init

		End Sub

		Public Overridable Sub init(ByVal begin As Long, ByVal [end] As Long) Implements INDArrayIndex.init

		End Sub

		Public Overrides Function ToString() As String
			Return "Point(" & point & ")"
		End Function
	End Class

End Namespace