Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports org.nd4j.common.primitives
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports Shape = org.nd4j.linalg.api.shape.Shape
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

Namespace org.nd4j.linalg.api.ndarray


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public abstract class BaseShapeInfoProvider implements ShapeInfoProvider
	Public MustInherit Class BaseShapeInfoProvider
		Implements ShapeInfoProvider

		Public MustOverride Sub purgeCache() Implements ShapeInfoProvider.purgeCache
		Protected Friend bytes As New AtomicLong(0)

		''' <summary>
		''' This method creates shapeInformation buffer, based on shape being passed in
		''' </summary>
		''' <param name="shape">
		''' @return </param>
		Public Overridable Function createShapeInformation(ByVal shape() As Long, ByVal dataType As DataType) As Pair(Of DataBuffer, Long()) Implements ShapeInfoProvider.createShapeInformation
			Dim order As Char = Nd4j.order()

			Return createShapeInformation(shape, order, dataType)
		End Function

		''' <summary>
		''' This method creates shapeInformation buffer, based on shape & order being passed in
		''' </summary>
		''' <param name="shape"> </param>
		''' <param name="order">
		''' @return </param>
		Public Overridable Function createShapeInformation(ByVal shape() As Long, ByVal order As Char, ByVal dataType As DataType) As Pair(Of DataBuffer, Long()) Implements ShapeInfoProvider.createShapeInformation
			Dim stride() As Long = Nd4j.getStrides(shape, order)

			' this won't be view, so ews is 1
			Dim ews As Integer = 1

			Return createShapeInformation(shape, stride, ews, order, dataType, False)
		End Function

		Public Overridable Function createShapeInformation(ByVal shape() As Long, ByVal stride() As Long, ByVal elementWiseStride As Long, ByVal order As Char, ByVal dataType As DataType, ByVal empty As Boolean) As Pair(Of DataBuffer, Long()) Implements ShapeInfoProvider.createShapeInformation
			Dim buffer As DataBuffer = Shape.createShapeInformation(shape, stride, elementWiseStride, order, dataType, empty)
			buffer.Constant = True
			Return Pair.create(buffer, buffer.asLong())
		End Function

		Public Overridable Function createShapeInformation(ByVal shape() As Long, ByVal stride() As Long, ByVal elementWiseStride As Long, ByVal order As Char, ByVal extras As Long) As Pair(Of DataBuffer, Long()) Implements ShapeInfoProvider.createShapeInformation
			Dim buffer As DataBuffer = Shape.createShapeInformation(shape, stride, elementWiseStride, order, extras)
			buffer.Constant = True
			Return Pair.create(buffer, buffer.asLong())
		End Function


		Public Overridable ReadOnly Property CachedBytes As Long Implements ShapeInfoProvider.getCachedBytes
			Get
				Return bytes.get()
			End Get
		End Property
	End Class

End Namespace