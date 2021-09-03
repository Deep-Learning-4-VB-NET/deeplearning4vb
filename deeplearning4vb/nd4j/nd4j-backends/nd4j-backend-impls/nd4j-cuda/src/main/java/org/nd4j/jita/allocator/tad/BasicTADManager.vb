Imports System
Imports val = lombok.val
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports TADManager = org.nd4j.linalg.cache.TADManager
Imports NativeOps = org.nd4j.nativeblas.NativeOps
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder
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

Namespace org.nd4j.jita.allocator.tad


	''' <summary>
	''' @author raver119@gmail.com
	''' </summary>
	Public Class BasicTADManager
		Implements TADManager

		Protected Friend nativeOps As NativeOps = NativeOpsHolder.Instance.getDeviceNativeOps()
		Private Shared logger As Logger = LoggerFactory.getLogger(GetType(BasicTADManager))
		Protected Friend bytes As New AtomicLong(0)

		Public Overridable Function getTADOnlyShapeInfo(ByVal array As INDArray, ByVal dimension() As Integer) As Pair(Of DataBuffer, DataBuffer) Implements TADManager.getTADOnlyShapeInfo
			If dimension IsNot Nothing AndAlso dimension.Length > 1 Then
				Array.Sort(dimension)
			End If

			If dimension Is Nothing Then
				dimension = New Integer() {Integer.MaxValue}
			End If

			Dim pack As val = Nd4j.Executioner.tadShapeInfoAndOffsets(array, dimension)

			'   logger.info("TAD shapeInfo after construction: {}", Arrays.toString(TadDescriptor.dataBufferToArray(outputBuffer)));
			' now we need to copy this buffer to either device global memory or device cache

			Return New Pair(Of DataBuffer, DataBuffer)(pack.getTadShapeInfo(), pack.getTadOffsets())
		End Function

		''' <summary>
		''' This method removes all cached shape buffers
		''' </summary>
		Public Overridable Sub purgeBuffers() Implements TADManager.purgeBuffers
			' no-op
		End Sub

		Public Overridable ReadOnly Property CachedBytes As Long Implements TADManager.getCachedBytes
			Get
				Return bytes.get()
			End Get
		End Property
	End Class

End Namespace