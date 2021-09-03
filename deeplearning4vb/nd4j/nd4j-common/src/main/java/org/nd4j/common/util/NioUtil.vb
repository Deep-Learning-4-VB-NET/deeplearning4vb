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

Namespace org.nd4j.common.util


	''' <summary>
	''' NioUtils for operations on
	''' nio buffers
	''' @author Adam Gibson
	''' </summary>
	Public Class NioUtil

		Private Sub New()
		End Sub

		Public Enum BufferType
			INT
			FLOAT
			[DOUBLE]
		End Enum

		''' <summary>
		''' Copy from the given from buffer
		''' to the to buffer at the specified
		''' offsets and strides </summary>
		''' <param name="n"> </param>
		''' <param name="bufferType"> </param>
		''' <param name="from"> the origin buffer </param>
		''' <param name="fromOffset"> the starting offset </param>
		''' <param name="fromStride"> the stride at which to copy from the origin </param>
		''' <param name="to"> the destination buffer </param>
		''' <param name="toOffset"> the starting point </param>
		''' <param name="toStride"> the to stride </param>
		Public Shared Sub copyAtStride(ByVal n As Integer, ByVal bufferType As BufferType, ByVal from As ByteBuffer, ByVal fromOffset As Integer, ByVal fromStride As Integer, ByVal [to] As ByteBuffer, ByVal toOffset As Integer, ByVal toStride As Integer)
			' TODO: implement shape copy for cases where stride == 1
			Dim fromView As ByteBuffer = from
			Dim toView As ByteBuffer = [to]
			fromView.order(ByteOrder.nativeOrder())
			toView.order(ByteOrder.nativeOrder())
			Select Case bufferType
				Case org.nd4j.common.util.NioUtil.BufferType.INT
					Dim fromInt As IntBuffer = fromView.asIntBuffer()
					Dim toInt As IntBuffer = toView.asIntBuffer()
					For i As Integer = 0 To n - 1
						Dim put As Integer = fromInt.get(fromOffset + i * fromStride)
						toInt.put(toOffset + i * toStride, put)
					Next i
				Case org.nd4j.common.util.NioUtil.BufferType.FLOAT
					Dim fromFloat As FloatBuffer = fromView.asFloatBuffer()
					Dim toFloat As FloatBuffer = toView.asFloatBuffer()
					For i As Integer = 0 To n - 1
						Dim put As Single = fromFloat.get(fromOffset + i * fromStride)
						toFloat.put(toOffset + i * toStride, put)
					Next i
				Case org.nd4j.common.util.NioUtil.BufferType.DOUBLE
					Dim fromDouble As DoubleBuffer = fromView.asDoubleBuffer()
					Dim toDouble As DoubleBuffer = toView.asDoubleBuffer()
					For i As Integer = 0 To n - 1
						toDouble.put(toOffset + i * toStride, fromDouble.get(fromOffset + i * fromStride))

					Next i
				Case Else
					Throw New System.ArgumentException("Only floats and double supported")

			End Select


		End Sub

	End Class

End Namespace