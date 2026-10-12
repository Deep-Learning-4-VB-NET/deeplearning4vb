Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports val = lombok.val
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ConstantHandler = org.nd4j.linalg.cache.ConstantHandler
Imports TADManager = org.nd4j.linalg.cache.TADManager
Imports TadDescriptor = org.nd4j.linalg.cache.TadDescriptor
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives
Imports NativeOps = org.nd4j.nativeblas.NativeOps

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


	Public Class CpuTADManager
		Implements TADManager

		Private cache As IDictionary(Of TadDescriptor, Pair(Of DataBuffer, DataBuffer)) = New ConcurrentDictionary(Of TadDescriptor, Pair(Of DataBuffer, DataBuffer))()
		Private nativeOps As NativeOps
		Private constantHandler As ConstantHandler
		Private bytes As New AtomicLong(0)
		Private counter As New AtomicInteger(0)
		Private Const MAX_ENTRIES As Integer = 100

		Public Sub New()
			'
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void init(@NonNull NativeOps nativeOps, @NonNull ConstantHandler constantHandler)
		Public Overridable Sub init(ByVal nativeOps As NativeOps, ByVal constantHandler As ConstantHandler)
			Me.nativeOps = nativeOps
			Me.constantHandler = constantHandler
		End Sub

		''' <summary>
		''' This method removes all cached shape buffers
		''' </summary>
		Public Overridable Sub purgeBuffers() Implements TADManager.purgeBuffers
			cache = New ConcurrentDictionary(Of TadDescriptor, Pair(Of DataBuffer, DataBuffer))()
		End Sub

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

	'        
	'        if (dimension != null && dimension.length > 1)
	'            Arrays.sort(dimension);
	'
	'        if (dimension == null || dimension.length >= 1 && dimension[0] == Integer.MAX_VALUE) {
	'            return new Pair<>(array.shapeInfoDataBuffer(), null);
	'        } else {
	'            TadDescriptor descriptor = new TadDescriptor(array, dimension);
	'
	'            if (!cache.containsKey(descriptor)) {
	'                int dimensionLength = dimension.length;
	'
	'                // FIXME: this is fast triage, remove it later
	'                int targetRank = array.rank(); //dimensionLength <= 1 ? 2 : dimensionLength;
	'                long offsetLength;
	'                long tadLength = 1;
	'                for (int i = 0; i < dimensionLength; i++) {
	'                    tadLength *= array.shape()[dimension[i]];
	'                }
	'
	'                offsetLength = array.lengthLong() / tadLength;
	'
	'                DataBuffer outputBuffer = new LongBuffer(targetRank * 2 + 4);
	'                DataBuffer offsetsBuffer = new LongBuffer(offsetLength);
	'
	'                DataBuffer dimensionBuffer = constantHandler.getConstantBuffer(dimension, DataType.INT);
	'                Pointer dimensionPointer = dimensionBuffer.addressPointer();
	'
	'                Pointer xShapeInfo = array.shapeInfoDataBuffer().addressPointer();
	'                Pointer targetPointer = outputBuffer.addressPointer();
	'                Pointer offsetsPointer = offsetsBuffer.addressPointer();
	'
	'                nativeOps.tadOnlyShapeInfo((LongPointer) xShapeInfo, (IntPointer) dimensionPointer, dimension.length);
	'                if (1 > 0)
	'                    throw new RuntimeException();
	'
	'
	'                // If the line below will be uncommented, shapes from JVM will be used on native side
	'                //outputBuffer = array.tensorAlongDimension(0, dimension).shapeInfoDataBuffer();
	'                Pair<DataBuffer, DataBuffer> pair = new Pair<>(outputBuffer, offsetsBuffer);
	'                if (counter.get() < MAX_ENTRIES) {
	'                    counter.incrementAndGet();
	'                    cache.put(descriptor, pair);
	'
	'                    bytes.addAndGet((outputBuffer.length() * 4) + (offsetsBuffer.length() * 8));
	'                }
	'                return pair;
	'            }
	'
	'            return cache.get(descriptor);
	'        }
	'        
		End Function

		Public Overridable ReadOnly Property CachedBytes As Long Implements TADManager.getCachedBytes
			Get
				Return bytes.get()
			End Get
		End Property
	End Class

End Namespace