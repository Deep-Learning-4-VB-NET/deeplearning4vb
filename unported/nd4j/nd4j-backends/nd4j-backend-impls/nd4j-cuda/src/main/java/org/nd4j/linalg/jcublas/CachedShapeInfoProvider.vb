Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports org.nd4j.common.primitives
Imports ProtectedCudaShapeInfoProvider = org.nd4j.jita.constant.ProtectedCudaShapeInfoProvider
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports BaseShapeInfoProvider = org.nd4j.linalg.api.ndarray.BaseShapeInfoProvider
Imports ShapeInfoProvider = org.nd4j.linalg.api.ndarray.ShapeInfoProvider
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory

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

Namespace org.nd4j.linalg.jcublas

	''' <summary>
	''' @author raver119@gmail.com
	''' </summary>
	Public Class CachedShapeInfoProvider
		Inherits BaseShapeInfoProvider

		Private Shared logger As Logger = LoggerFactory.getLogger(GetType(CachedShapeInfoProvider))

		Protected Friend provider As ShapeInfoProvider = ProtectedCudaShapeInfoProvider.Instance

		Public Sub New()

		End Sub

		Public Overrides Function createShapeInformation(ByVal shape() As Long, ByVal stride() As Long, ByVal elementWiseStride As Long, ByVal order As Char, ByVal type As DataType, ByVal empty As Boolean) As Pair(Of DataBuffer, Long())
			Return provider.createShapeInformation(shape, stride, elementWiseStride, order, type, empty)
		End Function


		Public Overrides Function createShapeInformation(ByVal shape() As Long, ByVal stride() As Long, ByVal elementWiseStride As Long, ByVal order As Char, ByVal extras As Long) As Pair(Of DataBuffer, Long())
			Return provider.createShapeInformation(shape, stride, elementWiseStride, order, extras)
		End Function

		''' <summary>
		''' This method forces cache purge, if cache is available for specific implementation
		''' </summary>
		Public Overrides Sub purgeCache()
			provider.purgeCache()
		End Sub
	End Class

End Namespace