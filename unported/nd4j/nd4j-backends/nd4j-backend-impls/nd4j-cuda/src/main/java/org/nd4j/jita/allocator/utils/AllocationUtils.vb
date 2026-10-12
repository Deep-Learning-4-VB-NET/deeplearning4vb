Imports NonNull = lombok.NonNull
Imports LongPointer = org.bytedeco.javacpp.LongPointer
Imports AllocationShape = org.nd4j.jita.allocator.impl.AllocationShape
Imports AtomicAllocator = org.nd4j.jita.allocator.impl.AtomicAllocator
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports CudaDoubleDataBuffer = org.nd4j.linalg.jcublas.buffer.CudaDoubleDataBuffer
Imports JCudaBuffer = org.nd4j.linalg.jcublas.buffer.JCudaBuffer

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

Namespace org.nd4j.jita.allocator.utils


	''' <summary>
	''' @author raver119@gmail.com
	''' </summary>
	Public Class AllocationUtils

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static long getRequiredMemory(@NonNull AllocationShape shape)
		Public Shared Function getRequiredMemory(ByVal shape As AllocationShape) As Long
			Return shape.getLength() * getElementSize(shape)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static int getElementSize(@NonNull AllocationShape shape)
		Public Shared Function getElementSize(ByVal shape As AllocationShape) As Integer
			If shape.getElementSize() > 0 Then
				Return shape.getElementSize()
			Else
				Return Nd4j.sizeOfDataType(shape.getDataType())
			End If
		End Function

		''' <summary>
		''' This method returns AllocationShape for specific array, that takes in account its real shape: offset, length, etc
		''' </summary>
		''' <param name="array">
		''' @return </param>
		Public Shared Function buildAllocationShape(ByVal array As INDArray) As AllocationShape
			Dim shape As New AllocationShape()
			shape.setDataType(array.data().dataType())
			shape.setLength(array.length())
			shape.setDataType(array.data().dataType())

			Return shape
		End Function

		''' <summary>
		''' This method returns AllocationShape for the whole DataBuffer.
		''' </summary>
		''' <param name="buffer">
		''' @return </param>
		Public Shared Function buildAllocationShape(ByVal buffer As DataBuffer) As AllocationShape
			Dim shape As New AllocationShape()
			shape.setDataType(buffer.dataType())
			shape.setLength(buffer.length())

			Return shape
		End Function

		''' <summary>
		''' This method returns AllocationShape for specific buffer, that takes in account its real shape: offset, length, etc
		''' </summary>
		''' <param name="buffer">
		''' @return </param>
		Public Shared Function buildAllocationShape(ByVal buffer As JCudaBuffer) As AllocationShape
			Dim shape As New AllocationShape()
			shape.setDataType(buffer.dataType())
			shape.setLength(buffer.length())

			Return shape
		End Function

		Public Shared Function getPointersBuffer(ByVal pointers() As Long) As DataBuffer
			Dim tempX As New CudaDoubleDataBuffer(pointers.Length)
			AtomicAllocator.Instance.memcpyBlocking(tempX, New LongPointer(pointers), pointers.Length * 8, 0)
			Return tempX
		End Function
	End Class

End Namespace