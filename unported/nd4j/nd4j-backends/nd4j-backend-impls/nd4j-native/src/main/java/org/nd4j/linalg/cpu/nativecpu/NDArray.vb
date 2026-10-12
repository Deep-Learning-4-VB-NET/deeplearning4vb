Imports System
Imports System.Collections.Generic
Imports System.IO
Imports FlatBufferBuilder = com.google.flatbuffers.FlatBufferBuilder
Imports val = lombok.val
Imports BytePointer = org.bytedeco.javacpp.BytePointer
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports FlatArray = org.nd4j.graph.FlatArray
Imports org.nd4j.linalg.api.buffer
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports BaseNDArray = org.nd4j.linalg.api.ndarray.BaseNDArray
Imports BaseNDArrayProxy = org.nd4j.linalg.api.ndarray.BaseNDArrayProxy
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports JvmShapeInfo = org.nd4j.linalg.api.ndarray.JvmShapeInfo
Imports PerformanceTracker = org.nd4j.linalg.api.ops.performance.PerformanceTracker
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports DoubleBuffer = org.nd4j.linalg.cpu.nativecpu.buffer.DoubleBuffer
Imports FloatBuffer = org.nd4j.linalg.cpu.nativecpu.buffer.FloatBuffer
Imports LongBuffer = org.nd4j.linalg.cpu.nativecpu.buffer.LongBuffer
Imports Utf8Buffer = org.nd4j.linalg.cpu.nativecpu.buffer.Utf8Buffer
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports MemcpyDirection = org.nd4j.linalg.api.memory.MemcpyDirection
Imports WorkspaceUtils = org.nd4j.linalg.workspace.WorkspaceUtils

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

Namespace org.nd4j.linalg.cpu.nativecpu




	<Serializable>
	Public Class NDArray
		Inherits BaseNDArray

		Shared Sub New()
			'invoke the override
			Nd4j.BlasWrapper
		End Sub

		Public Sub New()
			MyBase.New()
		End Sub


		Public Sub New(ByVal buffer As DataBuffer, ByVal shapeInfo As LongBuffer, ByVal javaShapeInfo() As Long)
			Me.jvmShapeInfo = New JvmShapeInfo(javaShapeInfo)
			Me.shapeInformation_Conflict = shapeInfo
			Me.data_Conflict = buffer
		End Sub

		Public Sub New(ByVal buffer As DataBuffer)
			MyBase.New(buffer)
		End Sub

		Public Sub New(ByVal buffer As DataBuffer, ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long, ByVal ordering As Char)
			MyBase.New(buffer, shape, stride, offset, ordering)
		End Sub

		Public Sub New(ByVal buffer As DataBuffer, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long, ByVal ordering As Char)
			MyBase.New(buffer, shape, stride, offset, ordering)
		End Sub

		Public Sub New(ByVal buffer As DataBuffer, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long, ByVal ews As Long, ByVal ordering As Char)
			MyBase.New(buffer, shape, stride, offset, ews, ordering)
		End Sub

		Public Sub New(ByVal buffer As DataBuffer, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long, ByVal ordering As Char, ByVal dataType As DataType)
			MyBase.New(buffer, shape, stride, offset, ordering, dataType)
		End Sub

		Public Sub New(ByVal data()() As Double)
			MyBase.New(data)
		End Sub

		Public Sub New(ByVal data()() As Double, ByVal ordering As Char)
			MyBase.New(data, ordering)
		End Sub

		''' <summary>
		''' Create this ndarray with the given data and shape and 0 offset
		''' </summary>
		''' <param name="data">     the data to use </param>
		''' <param name="shape">    the shape of the ndarray </param>
		''' <param name="ordering"> </param>
		Public Sub New(ByVal data() As Single, ByVal shape() As Integer, ByVal ordering As Char)
			MyBase.New(data, shape, ordering)
		End Sub

		''' <param name="data">     the data to use </param>
		''' <param name="shape">    the shape of the ndarray </param>
		''' <param name="offset">   the desired offset </param>
		''' <param name="ordering"> the ordering of the ndarray </param>
		Public Sub New(ByVal data() As Single, ByVal shape() As Integer, ByVal offset As Long, ByVal ordering As Char)
			MyBase.New(data, shape, offset, ordering)
		End Sub

		Public Sub New(ByVal data() As Single, ByVal shape() As Long, ByVal offset As Long, ByVal ordering As Char)
			MyBase.New(data, shape, offset, ordering)
		End Sub

		Public Sub New(ByVal data() As Single, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long, ByVal ordering As Char)
			MyBase.New(data, shape, stride, offset, ordering)
		End Sub

		Public Sub New(ByVal data() As Double, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long, ByVal ordering As Char)
			MyBase.New(data, shape, stride, offset, ordering)
		End Sub

		Public Sub New(ByVal data As DataBuffer, ByVal shape() As Long, ByVal stride() As Long, ByVal ordering As Char, ByVal type As DataType)
			MyBase.New(data, shape, stride, ordering, type)
		End Sub

		Public Sub New(ByVal data As DataBuffer, ByVal shape() As Long, ByVal stride() As Long, ByVal ordering As Char, ByVal type As DataType, ByVal workspace As MemoryWorkspace)
			MyBase.New(data, shape, stride, ordering, type, workspace)
		End Sub

		Public Sub New(ByVal data() As Double, ByVal shape() As Long, ByVal offset As Long, ByVal ordering As Char)
			MyBase.New(data, shape, offset, ordering)
		End Sub

		''' <summary>
		''' Construct an ndarray of the specified shape
		''' with an empty data array
		''' </summary>
		''' <param name="shape">    the shape of the ndarray </param>
		''' <param name="stride">   the stride of the ndarray </param>
		''' <param name="offset">   the desired offset </param>
		''' <param name="ordering"> the ordering of the ndarray </param>
		Public Sub New(ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long, ByVal ordering As Char)
			MyBase.New(shape, stride, offset, ordering)
		End Sub

		Public Sub New(ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long, ByVal ordering As Char)
			MyBase.New(shape, stride, offset, ordering)
		End Sub



		''' <summary>
		''' Construct an ndarray of the specified shape, with optional initialization
		''' </summary>
		''' <param name="shape">    the shape of the ndarray </param>
		''' <param name="stride">   the stride of the ndarray </param>
		''' <param name="offset">   the desired offset </param>
		''' <param name="ordering"> the ordering of the ndarray </param>
		''' <param name="initialize"> Whether to initialize the INDArray. If true: initialize. If false: don't. </param>
		Public Sub New(ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long, ByVal ordering As Char, ByVal initialize As Boolean)
			MyBase.New(shape, stride, offset, ordering, initialize)
		End Sub

		Public Sub New(ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long, ByVal ordering As Char, ByVal initialize As Boolean)
			MyBase.New(shape, stride, offset, ordering, initialize)
		End Sub

		Public Sub New(ByVal type As DataType, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long, ByVal ordering As Char)
			MyBase.New(type, shape, stride, offset, ordering, True)
		End Sub

		Public Sub New(ByVal type As DataType, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long, ByVal ordering As Char, ByVal workspace As MemoryWorkspace)
			MyBase.New(type, shape, stride, offset, ordering, True, workspace)
		End Sub

		Public Sub New(ByVal type As DataType, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long, ByVal ordering As Char, ByVal initialize As Boolean)
			MyBase.New(type, shape, stride, offset, ordering, initialize)
		End Sub

		Public Sub New(ByVal type As DataType, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long, ByVal ordering As Char, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace)
			MyBase.New(type, shape, stride, offset, ordering, initialize, workspace)
		End Sub

		''' <summary>
		''' Create the ndarray with
		''' the specified shape and stride and an offset of 0
		''' </summary>
		''' <param name="shape">    the shape of the ndarray </param>
		''' <param name="stride">   the stride of the ndarray </param>
		''' <param name="ordering"> the ordering of the ndarray </param>
		Public Sub New(ByVal shape() As Integer, ByVal stride() As Integer, ByVal ordering As Char)
			MyBase.New(shape, stride, ordering)
		End Sub

		Public Sub New(ByVal shape() As Integer, ByVal offset As Long, ByVal ordering As Char)
			MyBase.New(shape, offset, ordering)
		End Sub

		Public Sub New(ByVal shape() As Integer)
			MyBase.New(shape)
		End Sub

		''' <summary>
		''' Creates a new <i>n</i> times <i>m</i> <tt>DoubleMatrix</tt>.
		''' </summary>
		''' <param name="newRows">    the number of rows (<i>n</i>) of the new matrix. </param>
		''' <param name="newColumns"> the number of columns (<i>m</i>) of the new matrix. </param>
		''' <param name="ordering"> </param>
		Public Sub New(ByVal newRows As Integer, ByVal newColumns As Integer, ByVal ordering As Char)
			MyBase.New(newRows, newColumns, ordering)
		End Sub

		Public Sub New(ByVal newRows As Long, ByVal newColumns As Long, ByVal ordering As Char)
			MyBase.New(newRows, newColumns, ordering)
		End Sub

		''' <summary>
		''' Create an ndarray from the specified slices.
		''' This will go through and merge all of the
		''' data from each slice in to one ndarray
		''' which will then take the specified shape
		''' </summary>
		''' <param name="slices">   the slices to merge </param>
		''' <param name="shape">    the shape of the ndarray </param>
		''' <param name="ordering"> </param>
		Public Sub New(ByVal slices As IList(Of INDArray), ByVal shape() As Integer, ByVal ordering As Char)
			MyBase.New(slices, shape, ordering)
		End Sub

		Public Sub New(ByVal slices As IList(Of INDArray), ByVal shape() As Long, ByVal ordering As Char)
			MyBase.New(slices, shape, ordering)
		End Sub

		''' <summary>
		''' Create an ndarray from the specified slices.
		''' This will go through and merge all of the
		''' data from each slice in to one ndarray
		''' which will then take the specified shape
		''' </summary>
		''' <param name="slices">   the slices to merge </param>
		''' <param name="shape">    the shape of the ndarray </param>
		''' <param name="stride"> </param>
		''' <param name="ordering"> </param>
		Public Sub New(ByVal slices As IList(Of INDArray), ByVal shape() As Integer, ByVal stride() As Integer, ByVal ordering As Char)
			MyBase.New(slices, shape, stride, ordering)
		End Sub

		Public Sub New(ByVal data() As Single, ByVal shape() As Integer, ByVal stride() As Integer, ByVal ordering As Char)
			MyBase.New(data, shape, stride, ordering)
		End Sub

		Public Sub New(ByVal data() As Single, ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long, ByVal ordering As Char)
			MyBase.New(data, shape, stride, offset, ordering)
		End Sub

		''' <summary>
		''' Create this ndarray with the given data and shape and 0 offset
		''' </summary>
		''' <param name="data">  the data to use </param>
		''' <param name="shape"> the shape of the ndarray </param>
		Public Sub New(ByVal data() As Single, ByVal shape() As Integer)
			MyBase.New(data, shape)
		End Sub

		Public Sub New(ByVal data() As Single, ByVal shape() As Integer, ByVal offset As Long)
			MyBase.New(data, shape, offset)
		End Sub

		''' <summary>
		''' Construct an ndarray of the specified shape
		''' with an empty data array
		''' </summary>
		''' <param name="shape">  the shape of the ndarray </param>
		''' <param name="stride"> the stride of the ndarray </param>
		''' <param name="offset"> the desired offset </param>
		Public Sub New(ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long)
			MyBase.New(shape, stride, offset)
		End Sub

		Public Sub New(ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long)
			MyBase.New(shape, stride, offset)
		End Sub

		''' <summary>
		''' Create the ndarray with
		''' the specified shape and stride and an offset of 0
		''' </summary>
		''' <param name="shape">  the shape of the ndarray </param>
		''' <param name="stride"> the stride of the ndarray </param>
		Public Sub New(ByVal shape() As Integer, ByVal stride() As Integer)
			MyBase.New(shape, stride)
		End Sub

		Public Sub New(ByVal shape() As Integer, ByVal offset As Long)
			MyBase.New(shape, offset)
		End Sub

		Public Sub New(ByVal shape() As Integer, ByVal ordering As Char)
			MyBase.New(shape, ordering)
		End Sub

		''' <summary>
		''' Creates a new <i>n</i> times <i>m</i> <tt>DoubleMatrix</tt>.
		''' </summary>
		''' <param name="newRows">    the number of rows (<i>n</i>) of the new matrix. </param>
		''' <param name="newColumns"> the number of columns (<i>m</i>) of the new matrix. </param>
		Public Sub New(ByVal newRows As Integer, ByVal newColumns As Integer)
			MyBase.New(newRows, newColumns)
		End Sub

		Public Sub New(ByVal newRows As Long, ByVal newColumns As Long)
			MyBase.New(newRows, newColumns)
		End Sub

		''' <summary>
		''' Create an ndarray from the specified slices.
		''' This will go through and merge all of the
		''' data from each slice in to one ndarray
		''' which will then take the specified shape
		''' </summary>
		''' <param name="slices"> the slices to merge </param>
		''' <param name="shape">  the shape of the ndarray </param>
		Public Sub New(ByVal slices As IList(Of INDArray), ByVal shape() As Integer)
			MyBase.New(slices, shape)
		End Sub

		Public Sub New(ByVal slices As IList(Of INDArray), ByVal shape() As Long)
			MyBase.New(slices, shape)
		End Sub

		''' <summary>
		''' Create an ndarray from the specified slices.
		''' This will go through and merge all of the
		''' data from each slice in to one ndarray
		''' which will then take the specified shape
		''' </summary>
		''' <param name="slices"> the slices to merge </param>
		''' <param name="shape">  the shape of the ndarray </param>
		''' <param name="stride"> </param>
		Public Sub New(ByVal slices As IList(Of INDArray), ByVal shape() As Integer, ByVal stride() As Integer)
			MyBase.New(slices, shape, stride)
		End Sub

		Public Sub New(ByVal slices As IList(Of INDArray), ByVal shape() As Long, ByVal stride() As Long)
			MyBase.New(slices, shape, stride)
		End Sub

		Public Sub New(ByVal data() As Single, ByVal shape() As Integer, ByVal stride() As Integer)
			MyBase.New(data, shape, stride)
		End Sub


		Public Sub New(ByVal data() As Single, ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long)
			MyBase.New(data, shape, stride, offset)
		End Sub

		Public Sub New(ByVal data() As Single)
			MyBase.New(data)
		End Sub



		Public Sub New(ByVal data() As Double, ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long)
			MyBase.New(data, shape, stride, offset)
		End Sub

		Public Sub New(ByVal floats()() As Single)
			MyBase.New(floats)
		End Sub

		Public Sub New(ByVal data()() As Single, ByVal ordering As Char)
			MyBase.New(data, ordering)
		End Sub

		Public Sub New(ByVal data As DataBuffer, ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long)
			MyBase.New(data, shape, stride, offset)

		End Sub

		Public Sub New(ByVal data() As Integer, ByVal shape() As Integer, ByVal strides() As Integer)
			MyBase.New(data, shape, strides)
		End Sub

		Public Sub New(ByVal data As DataBuffer, ByVal shape() As Integer)
			MyBase.New(data, shape)
		End Sub

		Public Sub New(ByVal data As DataBuffer, ByVal shape() As Long)
			MyBase.New(data, shape)
		End Sub

		Public Sub New(ByVal buffer As DataBuffer, ByVal shape() As Integer, ByVal offset As Long)
			MyBase.New(buffer, shape, offset)
		End Sub

		Public Sub New(ByVal buffer As DataBuffer, ByVal shape() As Integer, ByVal ordering As Char)
			MyBase.New(buffer, shape, ordering)
		End Sub

		Public Sub New(ByVal data() As Double, ByVal shape() As Integer, ByVal ordering As Char)
			MyBase.New(data, shape, ordering)
		End Sub

		Public Sub New(ByVal data() As Double, ByVal shape() As Long, ByVal ordering As Char)
			MyBase.New(data, shape, ordering)
		End Sub

		Public Sub New(ByVal data() As Double, ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long, ByVal ordering As Char)
			MyBase.New(data, shape, stride, offset, ordering)
		End Sub

		Public Sub New(ByVal data() As Single, ByVal order As Char)
			MyBase.New(data, order)
		End Sub

		Public Sub New(ByVal floatBuffer As FloatBuffer, ByVal order As Char)
			MyBase.New(floatBuffer, order)
		End Sub

		Public Sub New(ByVal buffer As DataBuffer, ByVal shape() As Integer, ByVal strides() As Integer)
			MyBase.New(buffer, shape, strides)
		End Sub

		Public Sub New(ByVal buffer As DoubleBuffer, ByVal shape() As Integer, ByVal ordering As Char)
			MyBase.New(buffer, shape, 0, ordering)
		End Sub

		Public Sub New(ByVal buffer As DoubleBuffer, ByVal shape() As Integer, ByVal offset As Long)
			MyBase.New(buffer, shape, offset)
		End Sub

		Public Sub New(ByVal shape() As Integer, ByVal buffer As DataBuffer)
			MyBase.New(shape, buffer)
		End Sub

		Public Sub New(ByVal dataType As DataType, ByVal shape() As Long, ByVal paddings() As Long, ByVal paddingOffsets() As Long, ByVal ordering As Char, ByVal workspace As MemoryWorkspace)
			MyBase.New(dataType, shape, paddings, paddingOffsets, ordering, workspace)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private Object writeReplace() throws java.io.ObjectStreamException
		Private Function writeReplace() As Object
			Return New BaseNDArrayProxy(Me)
		End Function

		Public Overrides Function unsafeDuplication() As INDArray
			WorkspaceUtils.assertValidArray(Me, "Cannot duplicate array")
			If View Then
				Return Me.dup(Me.ordering())
			End If

			Dim rb As DataBuffer = If(Nd4j.MemoryManager.CurrentWorkspace Is Nothing, Nd4j.DataBufferFactory.createSame(Me.data_Conflict, False), Nd4j.DataBufferFactory.createSame(Me.data_Conflict, False, Nd4j.MemoryManager.CurrentWorkspace))

			Dim ret As INDArray = Nd4j.createArrayFromShapeBuffer(rb, Me.shapeInfoDataBuffer())

			Dim perfD As val = PerformanceTracker.Instance.helperStartTransaction()

			Pointer.memcpy(ret.data().addressPointer(), Me.data().addressPointer(), Me.data().length() * Me.data().ElementSize)

			PerformanceTracker.Instance.helperRegisterTransaction(0, perfD, Me.data().length() * Me.data().ElementSize, MemcpyDirection.HOST_TO_HOST)

			Return ret
		End Function

		Public Overrides Function unsafeDuplication(ByVal blocking As Boolean) As INDArray
			Return unsafeDuplication()
		End Function


		Public Overrides Function shapeDescriptor() As LongShapeDescriptor
			Return LongShapeDescriptor.fromShape(shape(), stride(), elementWiseStride(), ordering(), dataType(), Empty)
		End Function

		Protected Friend Overrides Function stringBuffer(ByVal builder As FlatBufferBuilder, ByVal buffer As DataBuffer) As Integer
			Preconditions.checkArgument(buffer.dataType() = DataType.UTF8, "This method can be called on UTF8 buffers only")
			Try
				Dim bos As New MemoryStream()
				Dim dos As New DataOutputStream(bos)

				Dim numWords As val = Me.length()
				Dim ub As val = DirectCast(buffer, Utf8Buffer)
				' writing length first
				Dim t As val = length()
				Dim ptr As val = CType(ub.pointer(), BytePointer)

				' now write all strings as bytes
				For i As Integer = 0 To ub.length() - 1
					dos.writeByte(ptr.get(i))
				Next i

				Dim bytes As val = bos.toByteArray()
				Return FlatArray.createBufferVector(builder, bytes)
			Catch e As IOException
				Throw New Exception(e)
			End Try
		End Function

		Public Overrides Function getString(ByVal index As Long) As String
			If Not S Then
				Throw New System.NotSupportedException("This method is usable only on String dataType, but got [" & Me.dataType() & "]")
			End If

			Return DirectCast(data_Conflict, Utf8Buffer).getString(index)
		End Function
	End Class

End Namespace