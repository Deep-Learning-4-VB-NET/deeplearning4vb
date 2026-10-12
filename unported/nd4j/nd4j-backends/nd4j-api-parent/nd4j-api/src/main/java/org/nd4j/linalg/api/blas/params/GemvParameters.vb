Imports Data = lombok.Data
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ND4JArraySizeException = org.nd4j.linalg.exception.ND4JArraySizeException

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

Namespace org.nd4j.linalg.api.blas.params

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public @Data class GemvParameters
	Public Class GemvParameters
		Private m, n, lda, incx, incy As Integer
		Private a, x, y As INDArray
		Private aOrdering As Char = "N"c

		Public Sub New(ByVal a As INDArray, ByVal x As INDArray, ByVal y As INDArray)
			a = copyIfNecessary(a)
			x = copyIfNecessaryVector(x)
			Me.a = a
			Me.x = x
			Me.y = y

			If a.columns() > Integer.MaxValue OrElse a.rows() > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If

			If x.columns() > Integer.MaxValue OrElse x.rows() > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If


			If a.ordering() = "f"c AndAlso a.Matrix Then
				Me.m = CInt(a.rows())
				Me.n = CInt(a.columns())
				Me.lda = CInt(a.rows())
			ElseIf a.ordering() = "c"c AndAlso a.Matrix Then
				Me.m = CInt(a.columns())
				Me.n = CInt(a.rows())
				Me.lda = CInt(a.columns())
				aOrdering = "T"c

			Else
				Me.m = CInt(a.rows())
				Me.n = CInt(a.columns())
				Me.lda = CInt(a.size(0))
			End If


			If x.rank() = 1 Then
				incx = 1
			ElseIf x.ColumnVector Then
				incx = x.stride(0)
			Else
				incx = x.stride(1)
			End If

			Me.incy = y.elementWiseStride()

		End Sub

		Private Function copyIfNecessary(ByVal arr As INDArray) As INDArray
			'See also: Shape.toMmulCompatible - want same conditions here and there
			'Check if matrix values are contiguous in memory. If not: dup
			'Contiguous for c if: stride[0] == shape[1] and stride[1] = 1
			'Contiguous for f if: stride[0] == 1 and stride[1] == shape[0]
			If arr.ordering() = "c"c AndAlso (arr.stride(0) <> arr.size(1) OrElse arr.stride(1) <> 1) Then
				Return arr.dup()
			ElseIf arr.ordering() = "f"c AndAlso (arr.stride(0) <> 1 OrElse arr.stride(1) <> arr.size(0)) Then
				Return arr.dup()
			ElseIf arr.elementWiseStride() < 1 Then
				Return arr.dup()
			End If
			Return arr
		End Function

		Private Function copyIfNecessaryVector(ByVal vec As INDArray) As INDArray
			If vec.offset() <> 0 Then
				Return vec.dup()
			End If
			Return vec
		End Function

	End Class

End Namespace