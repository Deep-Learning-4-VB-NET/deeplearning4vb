Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
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

Namespace org.nd4j.linalg.util

	''' <summary>
	''' @author Adam Gibson
	''' </summary>
	Public Class NDArrayMath


		Private Sub New()
		End Sub

		''' <summary>
		''' Compute the offset for a given slice </summary>
		''' <param name="arr"> the array to compute
		'''            the offset frm </param>
		''' <param name="slice"> the slice to compute the offset for </param>
		''' <returns> the offset for a given slice </returns>
		Public Shared Function offsetForSlice(ByVal arr As INDArray, ByVal slice As Integer) As Long
			Return slice * lengthPerSlice(arr)
		End Function

		''' <summary>
		''' The number of elements in a slice
		''' along a set of dimensions </summary>
		''' <param name="arr"> the array
		'''            to calculate the length per slice for </param>
		''' <param name="dimension"> the dimensions to do the calculations along </param>
		''' <returns> the number of elements in a slice along
		''' arbitrary dimensions </returns>
		Public Shared Function lengthPerSlice(ByVal arr As INDArray, ParamArray ByVal dimension() As Integer) As Long
			Dim remove() As Long = ArrayUtil.removeIndex(arr.shape(), dimension)
			Return ArrayUtil.prodLong(remove)
		End Function

		''' <summary>
		''' Return the length of a slice </summary>
		''' <param name="arr"> the array to get the length of a slice for </param>
		''' <returns> the number of elements per slice in an array </returns>
		Public Shared Function lengthPerSlice(ByVal arr As INDArray) As Long
			Return lengthPerSlice(arr, 0)
		End Function


		''' <summary>
		''' Return the number of vectors for an array
		''' the number of vectors for an array </summary>
		''' <param name="arr"> the array to calculate the number of vectors for </param>
		''' <returns> the number of vectors for the given array </returns>
		Public Shared Function numVectors(ByVal arr As INDArray) As Long
			If arr.rank() = 1 Then
				Return 1
			ElseIf arr.rank() = 2 Then
				Return arr.size(0)
			Else
				Dim prod As Integer = 1
				Dim i As Integer = 0
				Do While i < arr.rank() - 1
					prod *= arr.size(i)
					i += 1
				Loop

				Return prod
			End If
		End Function


		''' <summary>
		''' The number of vectors
		''' in each slice of an ndarray. </summary>
		''' <param name="arr"> the array to
		'''            get the number
		'''            of vectors per slice for </param>
		''' <returns> the number of vectors per slice </returns>
		Public Shared Function vectorsPerSlice(ByVal arr As INDArray) As Long
			If arr.rank() > 2 Then
				Return ArrayUtil.prodLong(New Long() {arr.size(-1), arr.size(-2)})
			End If

			Return arr.slices()
		End Function


		''' <summary>
		''' Computes the tensors per slice
		''' given a tensor shape and array </summary>
		''' <param name="arr"> the array to get the tensors per slice for </param>
		''' <param name="tensorShape"> the desired tensor shape </param>
		''' <returns> the tensors per slice of an ndarray </returns>
		Public Shared Function tensorsPerSlice(ByVal arr As INDArray, ByVal tensorShape() As Integer) As Long
			Return lengthPerSlice(arr) \ ArrayUtil.prod(tensorShape)
		End Function

		''' <summary>
		''' The number of vectors
		''' in each slice of an ndarray. </summary>
		''' <param name="arr"> the array to
		'''            get the number
		'''            of vectors per slice for </param>
		''' <returns> the number of vectors per slice </returns>
		Public Shared Function matricesPerSlice(ByVal arr As INDArray) As Long
			If arr.rank() = 3 Then
				Return 1
			ElseIf arr.rank() > 3 Then
				Dim ret As Integer = 1
				Dim i As Integer = 1
				Do While i < arr.rank() - 2
					ret *= arr.size(i)
					i += 1
				Loop
				Return ret
			End If
			Return arr.size(-2)
		End Function

		''' <summary>
		''' The number of vectors
		''' in each slice of an ndarray. </summary>
		''' <param name="arr"> the array to
		'''            get the number
		'''            of vectors per slice for </param>
		''' <param name="rank"> the dimensions to get the number of vectors per slice for </param>
		''' <returns> the number of vectors per slice </returns>
		Public Shared Function vectorsPerSlice(ByVal arr As INDArray, ParamArray ByVal rank() As Integer) As Long
			If arr.rank() > 2 Then
				Return arr.size(-2) * arr.size(-1)
			End If

			Return arr.size(-1)

		End Function

		''' <summary>
		''' calculates the offset for a tensor </summary>
		''' <param name="index"> </param>
		''' <param name="arr"> </param>
		''' <param name="tensorShape">
		''' @return </param>
		Public Shared Function sliceOffsetForTensor(ByVal index As Integer, ByVal arr As INDArray, ByVal tensorShape() As Integer) As Long
			Dim tensorLength As Long = ArrayUtil.prodLong(tensorShape)
			Dim lengthPerSlice As Long = NDArrayMath.lengthPerSlice(arr)
			Dim offset As Long = index * tensorLength \ lengthPerSlice
			Return offset
		End Function

		Public Shared Function sliceOffsetForTensor(ByVal index As Integer, ByVal arr As INDArray, ByVal tensorShape() As Long) As Long
			Dim tensorLength As Long = ArrayUtil.prodLong(tensorShape)
			Dim lengthPerSlice As Long = NDArrayMath.lengthPerSlice(arr)
			Dim offset As Long = index * tensorLength \ lengthPerSlice
			Return offset
		End Function


		''' <summary>
		''' This maps an index of a vector
		''' on to a vector in the matrix that can be used
		''' for indexing in to a tensor </summary>
		''' <param name="index"> the index to map </param>
		''' <param name="arr"> the array to use
		'''            for indexing </param>
		''' <param name="rank"> the dimensions to compute a slice for </param>
		''' <returns> the mapped index </returns>
		Public Shared Function mapIndexOntoTensor(ByVal index As Integer, ByVal arr As INDArray, ParamArray ByVal rank() As Integer) As Integer
			Dim ret As Integer = index * ArrayUtil.prod(ArrayUtil.removeIndex(arr.shape(), rank))
			Return ret
		End Function


		''' <summary>
		''' This maps an index of a vector
		''' on to a vector in the matrix that can be used
		''' for indexing in to a tensor </summary>
		''' <param name="index"> the index to map </param>
		''' <param name="arr"> the array to use
		'''            for indexing </param>
		''' <returns> the mapped index </returns>
		Public Shared Function mapIndexOntoVector(ByVal index As Integer, ByVal arr As INDArray) As Long
			Dim ret As Long = index * arr.size(-1)
			Return ret
		End Function



	End Class

End Namespace