Imports System
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports NDArrayFactory = org.nd4j.linalg.factory.NDArrayFactory

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

Namespace org.nd4j.linalg.api.blas

	''' <summary>
	''' Blas buffer util for interopping with the underlying buffers
	''' and the given ndarrays
	''' 
	''' @author Adam Gibson
	''' </summary>
	Public Class BlasBufferUtil
		''' <summary>
		''' Get blas stride for the
		''' given array </summary>
		''' <param name="arr"> the array </param>
		''' <returns> the blas stride </returns>
		Public Shared Function getBlasOffset(ByVal arr As INDArray) As Long
			Return arr.offset()
		End Function

		''' <summary>
		''' Get blas stride for the
		''' given array </summary>
		''' <param name="arr"> the array </param>
		''' <returns> the blas stride </returns>
		Public Shared Function getBlasStride(ByVal arr As INDArray) As Integer
			Return arr.elementWiseStride()
		End Function

		''' <summary>
		''' Returns the float data
		''' for this ndarray.
		''' If possible (the offset is 0 representing the whole buffer)
		''' it will return a direct reference to the underlying array </summary>
		''' <param name="buf"> the ndarray to get the data for </param>
		''' <returns> the float data for this ndarray </returns>
		Public Shared Function getFloatData(ByVal buf As INDArray) As Single()
			If buf.data().dataType() <> DataType.FLOAT Then
				Throw New System.ArgumentException("Float data must be obtained from a float buffer")
			End If

			If buf.data().allocationMode() = DataBuffer.AllocationMode.HEAP Then
				Return buf.data().asFloat()
			Else
				Dim ret(CInt(buf.length()) - 1) As Single
				Dim linear As INDArray = buf.reshape(ChrW(-1))

				For i As Integer = 0 To buf.length() - 1
					ret(i) = linear.getFloat(i)
				Next i
				Return ret
			End If
		End Function

		''' <summary>
		''' Returns the double data
		''' for this ndarray.
		''' If possible (the offset is 0 representing the whole buffer)
		''' it will return a direct reference to the underlying array </summary>
		''' <param name="buf"> the ndarray to get the data for </param>
		''' <returns> the double data for this ndarray </returns>
		Public Shared Function getDoubleData(ByVal buf As INDArray) As Double()
			If buf.data().dataType() <> DataType.DOUBLE Then
				Throw New System.ArgumentException("Double data must be obtained from a double buffer")
			End If

			If buf.data().allocationMode() = DataBuffer.AllocationMode.HEAP Then
				Return buf.data().asDouble()

			Else
				Dim ret(CInt(buf.length()) - 1) As Double
				Dim linear As INDArray = buf.reshape(ChrW(-1))
				For i As Integer = 0 To buf.length() - 1
					ret(i) = linear.getDouble(i)
				Next i
				Return ret

			End If
		End Function


		''' <summary>
		''' Returns the proper character for
		''' how to interpret a buffer (fortran being N C being T) </summary>
		''' <param name="arr"> the array to get the transpose for </param>
		''' <returns> the character for transpose of a particular
		''' array </returns>
		Public Shared Function getCharForTranspose(ByVal arr As INDArray) As Char
			Return "n"c
		End Function

		''' <summary>
		''' Return the proper stride
		''' through a vector
		''' relative to the ordering of the array
		''' This is for incX/incY parameters in BLAS.
		''' </summary>
		''' <param name="arr"> the array to get the stride for </param>
		''' <returns> the stride wrt the ordering
		''' for the given array </returns>
		Public Shared Function getStrideForOrdering(ByVal arr As INDArray) As Integer
			If arr.ordering() = NDArrayFactory.FORTRAN Then
				Return getBlasStride(arr)
			Else
				Return arr.stride(1)
			End If
		End Function

		''' <summary>
		''' Get the dimension associated with
		''' the given ordering.
		''' 
		''' When working with blas routines, they typically assume
		''' c ordering, instead you can invert the rows/columns
		''' which enable you to do no copy blas operations.
		''' 
		''' 
		''' </summary>
		''' <param name="arr"> </param>
		''' <param name="defaultRows">
		''' @return </param>
		Public Shared Function getDimension(ByVal arr As INDArray, ByVal defaultRows As Boolean) As Long

			'ignore ordering for vectors
			If arr.Vector Then
				Return If(defaultRows, arr.rows(), arr.columns())
			End If
			If arr.ordering() = NDArrayFactory.C Then
				Return If(defaultRows, arr.columns(), arr.rows())
			End If
			Return If(defaultRows, arr.rows(), arr.columns())
		End Function


		''' <summary>
		''' Get the leading dimension
		''' for a blas invocation.
		''' 
		''' The lead dimension is usually
		''' arr.size(0) (this is only for fortran ordering though).
		''' It can be size(1) (assuming matrix) for C ordering though. </summary>
		''' <param name="arr"> the array to </param>
		''' <returns> the leading dimension wrt the ordering of the array
		'''  </returns>
		Public Shared Function getLd(ByVal arr As INDArray) As Integer
			'ignore ordering for vectors
			If arr.Vector Then
				Return CInt(arr.length())
			End If

			Return If(arr.ordering() = NDArrayFactory.C, CInt(arr.size(1)), CInt(arr.size(0)))
		End Function


		''' <summary>
		''' Returns the float data
		''' for this buffer.
		''' If possible (the offset is 0 representing the whole buffer)
		''' it will return a direct reference to the underlying array </summary>
		''' <param name="buf"> the ndarray to get the data for </param>
		''' <returns> the double data for this ndarray </returns>
		Public Shared Function getFloatData(ByVal buf As DataBuffer) As Single()
			If buf.allocationMode() = DataBuffer.AllocationMode.HEAP Then
				Return buf.asFloat()
			Else
				Dim ret(CInt(buf.length()) - 1) As Single
				For i As Integer = 0 To buf.length() - 1
					ret(i) = buf.getFloat(i)
				Next i
				Return ret
			End If
		End Function

		''' <summary>
		''' Returns the double data
		''' for this buffer.
		''' If possible (the offset is 0 representing the whole buffer)
		''' it will return a direct reference to the underlying array </summary>
		''' <param name="buf"> the ndarray to get the data for </param>
		''' <returns> the double data for this buffer </returns>
		Public Shared Function getDoubleData(ByVal buf As DataBuffer) As Double()
			If buf.allocationMode() = DataBuffer.AllocationMode.HEAP Then
				Return buf.asDouble()
			Else
				Dim ret(CInt(buf.length()) - 1) As Double
				For i As Integer = 0 To buf.length() - 1
					ret(i) = buf.getDouble(i)
				Next i
				Return ret

			End If
		End Function


		''' <summary>
		''' Set the data for the underlying array.
		''' If the underlying buffer's array is equivalent to this array
		''' it returns (avoiding an unneccessary copy)
		''' 
		''' If the underlying storage mechanism isn't heap (no arrays)
		''' it just copied the data over (strided access with offsets where neccessary)
		''' 
		''' This is meant to be used with blas operations where the underlying blas implementation
		''' takes an array but the data buffer being used might not be an array.
		''' 
		''' This is also for situations where there is strided access and it's not
		''' optimal to want to use the whole data buffer but just the subset of the
		''' buffer needed for calculations.
		''' 
		''' </summary>
		''' <param name="data"> the data to set </param>
		''' <param name="toSet"> the array to set the data to </param>
		Public Shared Sub setData(ByVal data() As Single, ByVal toSet As INDArray)
			If toSet.data().dataType() <> DataType.FLOAT Then
				Throw New System.ArgumentException("Unable to set double data for opType " & toSet.data().dataType())
			End If

			If toSet.data().allocationMode() = DataBuffer.AllocationMode.HEAP Then
				Dim array As Object = toSet.data().array()
				'data is assumed to have already been updated
				If array Is data Then
					Return
				Else
					'copy the data over directly to the underlying array
					Dim d() As Single = DirectCast(array, Single())

					If toSet.offset() = 0 AndAlso toSet.length() = data.Length Then
						Array.Copy(data, 0, d, 0, d.Length)
					Else
						Dim count As Integer = 0
						'need to do strided access with offset
						For i As Integer = 0 To data.Length - 1
							' FIXME: LONG
							Dim dIndex As Integer = CInt(toSet.offset()) + (i * toSet.stride(-1))
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: d[dIndex] = data[count++];
							d(dIndex) = data(count)
								count += 1
						Next i
					End If
				End If
			Else
				'assumes the underlying data is in the right order
				Dim underlyingData As DataBuffer = toSet.data()
				If data.Length = toSet.length() AndAlso toSet.offset() = 0 Then
					For i As Integer = 0 To toSet.length() - 1
						underlyingData.put(i, data(i))
					Next i
				Else
					Dim count As Integer = 0
					'need to do strided access with offset
					For i As Integer = 0 To data.Length - 1
						' FIXME: LONG
						Dim dIndex As Integer = CInt(toSet.offset()) + (i * toSet.stride(-1))
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: underlyingData.put(dIndex, data[count++]);
						underlyingData.put(dIndex, data(count))
							count += 1
					Next i
				End If
			End If

		End Sub

		''' <summary>
		''' Set the data for the underlying array.
		''' If the underlying buffer's array is equivalent to this array
		''' it returns (avoiding an unneccessary copy)
		''' 
		''' If the underlying storage mechanism isn't heap (no arrays)
		''' it just copied the data over (strided access with offsets where neccessary)
		''' 
		''' This is meant to be used with blas operations where the underlying blas implementation
		''' takes an array but the data buffer being used might not be an array.
		''' 
		''' This is also for situations where there is strided access and it's not
		''' optimal to want to use the whole data buffer but just the subset of the
		''' buffer needed for calculations.
		''' 
		''' </summary>
		''' <param name="data"> the data to set </param>
		''' <param name="toSet"> the array to set the data to </param>
		Public Shared Sub setData(ByVal data() As Double, ByVal toSet As INDArray)
			If toSet.data().dataType() <> DataType.DOUBLE Then
				Throw New System.ArgumentException("Unable to set double data for opType " & toSet.data().dataType())
			End If

			If toSet.data().allocationMode() = DataBuffer.AllocationMode.HEAP Then
				Dim array As Object = toSet.data().array()
				'data is assumed to have already been updated
				If array Is data Then
					Return
				Else
					'copy the data over directly to the underlying array
					Dim d() As Double = DirectCast(array, Double())

					If toSet.offset() = 0 AndAlso toSet.length() = data.Length Then
						Array.Copy(data, 0, d, 0, d.Length)
					Else
						Dim count As Integer = 0
						'need to do strided access with offset
						For i As Integer = 0 To data.Length - 1
							' FIXME: LONG
							Dim dIndex As Integer = CInt(toSet.offset()) + (i * toSet.stride(-1))
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: d[dIndex] = data[count++];
							d(dIndex) = data(count)
								count += 1
						Next i
					End If
				End If
			Else
				'assumes the underlying data is in the right order
				Dim underlyingData As DataBuffer = toSet.data()
				If data.Length = toSet.length() AndAlso toSet.offset() = 0 Then
					For i As Integer = 0 To toSet.length() - 1
						underlyingData.put(i, data(i))
					Next i
				Else
					Dim count As Integer = 0
					'need to do strided access with offset
					For i As Integer = 0 To data.Length - 1
						' FIXME: LONG
						Dim dIndex As Integer = CInt(toSet.offset()) + (i * toSet.stride(-1))
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: underlyingData.put(dIndex, data[count++]);
						underlyingData.put(dIndex, data(count))
							count += 1
					Next i
				End If
			End If


		End Sub


	End Class

End Namespace