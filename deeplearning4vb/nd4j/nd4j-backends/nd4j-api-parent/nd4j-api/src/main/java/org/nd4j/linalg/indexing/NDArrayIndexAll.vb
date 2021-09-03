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
	''' Mainly meant for internal use:
	''' represents all of the elements of a dimension
	''' 
	''' @author Adam Gibson
	''' </summary>
	Public Class NDArrayIndexAll
		Inherits IntervalIndex

		Public Sub New()
			MyBase.New(True, 1)
		End Sub


		Public Overrides Sub init(ByVal arr As INDArray, ByVal begin As Long, ByVal dimension As Integer)
			Me.begin = 0
			Me.end_Conflict = arr.size(dimension)
			Me.length_Conflict = (end_Conflict - begin)\stride + 1
		End Sub

		Public Overrides Function ToString() As String
			Return "all()"
		End Function

	End Class

End Namespace