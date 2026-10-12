Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports org.nd4j.common.primitives
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer

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

	Public Interface ShapeInfoProvider
		''' <summary>
		''' This method creates long shapeInformation buffer, based on shape being passed in </summary>
		''' <param name="shape">
		''' @return </param>
		Function createShapeInformation(ByVal shape() As Long, ByVal dataType As DataType) As Pair(Of DataBuffer, Long())

		''' <summary>
		''' This method creates long shapeInformation buffer, based on shape & order being passed in </summary>
		''' <param name="shape">
		''' @return </param>
		Function createShapeInformation(ByVal shape() As Long, ByVal order As Char, ByVal dataType As DataType) As Pair(Of DataBuffer, Long())

		''' <summary>
		''' This method creates long shapeInformation buffer, based on detailed shape info being passed in </summary>
		''' <param name="shape">
		''' @return </param>
		Function createShapeInformation(ByVal shape() As Long, ByVal stride() As Long, ByVal elementWiseStride As Long, ByVal order As Char, ByVal dataType As DataType, ByVal empty As Boolean) As Pair(Of DataBuffer, Long())


		Function createShapeInformation(ByVal shape() As Long, ByVal stride() As Long, ByVal elementWiseStride As Long, ByVal order As Char, ByVal extras As Long) As Pair(Of DataBuffer, Long())

		''' <summary>
		''' This method forces cache purge, if cache is available for specific implementation
		''' </summary>
		Sub purgeCache()

		''' <summary>
		''' This method returns memory used for cache, in bytes
		''' @return
		''' </summary>
		ReadOnly Property CachedBytes As Long
	End Interface

End Namespace