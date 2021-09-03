Imports Data = lombok.Data
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ND4JArraySizeException = org.nd4j.linalg.exception.ND4JArraySizeException
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

Namespace org.nd4j.linalg.api.blas.params


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public @Data class GemmParams
	Public Class GemmParams
		Private lda, ldb, ldc, m, n, k As Integer
		Private a, b, c As INDArray
		Private transA As Char = "N"c
		Private transB As Char = "N"c
		Private ordering As Char = "f"c


		''' 
		''' <param name="a"> </param>
		''' <param name="b"> </param>
		''' <param name="c"> </param>
		Public Sub New(ByVal a As INDArray, ByVal b As INDArray, ByVal c As INDArray)
			If a.columns() <> b.rows() Then
				Throw New System.ArgumentException("A columns must equal B rows. MMul attempt: " & Arrays.toString(a.shape()) & "x" & Arrays.toString(b.shape()))
			End If
			If b.columns() <> c.columns() Then
				Throw New System.ArgumentException("B columns must match C columns. MMul attempt: " & Arrays.toString(a.shape()) & "x" & Arrays.toString(b.shape()) & "; result array provided: " & Arrays.toString(c.shape()))
			End If
			If a.rows() <> c.rows() Then
				Throw New System.ArgumentException("A rows must equal C rows. MMul attempt: " & Arrays.toString(a.shape()) & "x" & Arrays.toString(b.shape()) & "; result array provided: " & Arrays.toString(c.shape()))
			End If

			If a.columns() > Integer.MaxValue OrElse a.rows() > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If

			If b.columns() > Integer.MaxValue OrElse b.rows() > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If

			If c.columns() > Integer.MaxValue OrElse c.rows() > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If


			If Nd4j.allowsSpecifyOrdering() Then
				If a.ordering() = b.ordering() Then
					'both will be same ordering for cblas
					Me.ordering = a.ordering()
					'automatically assume fortran ordering
					'multiple backends force us to be
					'in fortran ordering only
					Me.a = copyIfNeccessary(a)
					Me.b = copyIfNeccessary(b)
					Me.c = c
					If ordering = "c"c Then
						Me.m = CInt(c.columns())
						Me.n = CInt(c.rows())
						Me.k = CInt(a.columns())
					Else
						Me.m = CInt(c.rows())
						Me.n = CInt(c.columns())
						Me.k = CInt(b.columns())
					End If

					Me.lda = CInt(a.rows())
					Me.ldb = CInt(b.rows())
					Me.ldc = CInt(c.rows())

					Me.transA = "N"c
					Me.transB = "N"c
				Else
					'automatically assume fortran ordering
					'multiple backends force us to be
					'in fortran ordering only
					Me.a = copyIfNeccessary(a)
					Me.b = b.dup(a.ordering())
					Me.c = c

					Me.m = CInt(c.rows())
					Me.n = CInt(c.columns())
					Me.k = CInt(a.columns())

					Me.ordering = a.ordering()

					Me.lda = CInt(a.rows())
					Me.ldb = CInt(b.rows())
					Me.ldc = CInt(c.rows())

					Me.transA = "N"c
					Me.transB = "N"c
				End If


			Else
				'automatically assume fortran ordering
				'multiple backends force us to be
				'in fortran ordering only
				Me.a = copyIfNeccessary(a)
				Me.b = copyIfNeccessary(b)
				Me.c = c

				Me.m = CInt(c.rows())
				Me.n = CInt(c.columns())
				Me.k = CInt(a.columns())

				'always fortran ordering
				Me.lda = CInt(If(Me.a.ordering() = "f"c, Me.a.rows(), Me.a.columns())) 'Leading dimension of a, as declared. But swap if 'c' order
				Me.ldb = CInt(If(Me.b.ordering() = "f"c, Me.b.rows(), Me.b.columns())) 'Leading dimension of b, as declared. But swap if 'c' order
				Me.ldc = CInt(c.rows())

				Me.transA = (If(Me.a.ordering() = "c"c, "T"c, "N"c))
				Me.transB = (If(Me.b.ordering() = "c"c, "T"c, "N"c))

			End If

			'/validate();
		End Sub

		Public Sub New(ByVal a As INDArray, ByVal b As INDArray, ByVal c As INDArray, ByVal transposeA As Boolean, ByVal transposeB As Boolean)
			Me.New(If(transposeA, a.transpose(), a),If(transposeB, b.transpose(), b), c)
		End Sub



		Private Function copyIfNeccessary(ByVal arr As INDArray) As INDArray
			'See also: Shape.toMmulCompatible - want same conditions here and there
			'Check if matrix values are contiguous in memory. If not: dup
			'Contiguous for c if: stride[0] == shape[1] and stride[1] = 1
			'Contiguous for f if: stride[0] == 1 and stride[1] == shape[0]
			If Not Nd4j.allowsSpecifyOrdering() AndAlso arr.ordering() = "c"c AndAlso (arr.stride(0) <> arr.size(1) OrElse arr.stride(1) <> 1) Then
				Return arr.dup()
			ElseIf arr.ordering() = "f"c AndAlso (arr.stride(0) <> 1 OrElse arr.stride(1) <> arr.size(0)) Then
				Return arr.dup()
			ElseIf arr.elementWiseStride() < 0 Then
				Return arr.dup()
			End If
			Return arr
		End Function
	End Class

End Namespace