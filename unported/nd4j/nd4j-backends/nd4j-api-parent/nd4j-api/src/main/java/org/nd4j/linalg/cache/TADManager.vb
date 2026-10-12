Imports org.nd4j.common.primitives
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
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

Namespace org.nd4j.linalg.cache

	Public Interface TADManager

		''' <summary>
		''' This method returns TAD shapeInfo and all offsets
		''' for specified tensor and dimensions.
		''' </summary>
		''' <param name="array"> Tensor for TAD precalculation </param>
		''' <param name="dimension">
		''' @return </param>
		Function getTADOnlyShapeInfo(ByVal array As INDArray, ParamArray ByVal dimension() As Integer) As Pair(Of DataBuffer, DataBuffer)

		''' <summary>
		''' This method removes all cached shape buffers
		''' </summary>
		Sub purgeBuffers()

		''' <summary>
		''' This method returns memory used for cache, in bytes
		''' 
		''' @return
		''' </summary>
		ReadOnly Property CachedBytes As Long
	End Interface

End Namespace