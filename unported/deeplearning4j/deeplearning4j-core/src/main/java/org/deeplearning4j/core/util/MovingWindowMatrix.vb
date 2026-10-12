Imports System.Collections.Generic
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.deeplearning4j.core.util



	''' 
	''' <summary>
	''' Moving window on a matrix (usually used for images)
	''' 
	''' Given a:          This is a list of flattened arrays:
	''' 1 1 1 1          1 1 2 2
	''' 2 2 2 2 ---->    1 1 2 2
	''' 3 3 3 3          3 3 4 4
	''' 4 4 4 4          3 3 4 4
	''' 
	''' @author Adam Gibson
	''' </summary>
	Public Class MovingWindowMatrix

		Private windowRowSize As Integer = 28
		Private windowColumnSize As Integer = 28
		Private toSlice As INDArray
		Private addRotate As Boolean = False


		''' 
		''' <param name="toSlice"> matrix to slice </param>
		''' <param name="windowRowSize"> the number of rows in each window </param>
		''' <param name="windowColumnSize"> the number of columns in each window </param>
		''' <param name="addRotate"> whether to add the possible rotations of each moving window </param>
		Public Sub New(ByVal toSlice As INDArray, ByVal windowRowSize As Integer, ByVal windowColumnSize As Integer, ByVal addRotate As Boolean)
			Me.toSlice = toSlice
			Me.windowRowSize = windowRowSize
			Me.windowColumnSize = windowColumnSize
			Me.addRotate = addRotate
		End Sub


		''' <summary>
		''' Same as calling new MovingWindowMatrix(toSlice,windowRowSize,windowColumnSize,false) </summary>
		''' <param name="toSlice"> </param>
		''' <param name="windowRowSize"> </param>
		''' <param name="windowColumnSize"> </param>
		Public Sub New(ByVal toSlice As INDArray, ByVal windowRowSize As Integer, ByVal windowColumnSize As Integer)
			Me.New(toSlice, windowRowSize, windowColumnSize, False)
		End Sub



		''' <summary>
		''' Returns a list of non flattened moving window matrices </summary>
		''' <returns> the list of matrices </returns>
		Public Overridable Function windows() As IList(Of INDArray)
			Return windows(False)
		End Function

		''' <summary>
		''' Moving window, capture a row x column moving window of
		''' a given matrix </summary>
		''' <param name="flattened"> whether the arrays should be flattened or not </param>
		''' <returns> the list of moving windows </returns>
		Public Overridable Function windows(ByVal flattened As Boolean) As IList(Of INDArray)
			Dim ret As IList(Of INDArray) = New List(Of INDArray)()
			Dim window As Integer = 0

			For i As Integer = 0 To toSlice.length() - 1
				If window >= toSlice.length() Then
					Exit For
				End If
				Dim w((Me.windowRowSize * Me.windowColumnSize) - 1) As Double
				Dim count As Integer = 0
				Do While count < Me.windowRowSize * Me.windowColumnSize
					w(count) = toSlice.getDouble(count + window)
					count += 1
				Loop
				Dim add As INDArray = Nd4j.create(w)
				If flattened Then
					add = add.ravel()
				Else
					add = add.reshape(ChrW(windowRowSize), windowColumnSize)
				End If
				If addRotate Then
					Dim currRotation As INDArray = add.dup()
					'3 different orientations besides the original
					For rotation As Integer = 0 To 2
						Nd4j.rot90(currRotation)
						ret.Add(currRotation.dup())
					Next rotation

				End If

				window += Me.windowRowSize * Me.windowColumnSize
				ret.Add(add)
			Next i


			Return ret
		End Function
	End Class

End Namespace