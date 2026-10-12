Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
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

Namespace org.nd4j.linalg.api.ops


	Public Interface Op
		Friend Enum Type
			SCALAR
			SCALAR_BOOL
			TRANSFORM_SAME
			TRANSFORM_FLOAT
			TRANSFORM_ANY
			TRANSFORM_BOOL
			TRANSFORM_STRICT
			PAIRWISE
			PAIRWISE_BOOL
			SPECIAL
			BROADCAST
			BROADCAST_BOOL
			REDUCE_LONG
			REDUCE_SAME
			REDUCE_FLOAT
			REDUCE_BOOL
			INDEXREDUCE
			VARIANCE
			REDUCE3
			GRID
			META
			AGGREGATION
			CUSTOM
			GRADIENT
			CONDITIONAL
			[LOOP]
			LOOP_COND
			[RETURN]
			RANDOM
			SUMMARYSTATS
			LOGIC
		End Enum

		''' <summary>
		''' Returns the extra args as a data buffer
		''' @return
		''' </summary>
		Function extraArgsDataBuff(ByVal bufferType As DataType) As DataBuffer

		''' <summary>
		''' Returns a buffer of either float
		''' or double
		''' of the extra args for this buffer </summary>
		''' <returns>  a buffer of either opType float or double
		''' representing the extra args for this op </returns>
		Function extraArgsBuff() As Buffer

		''' <summary>
		''' An op number
		''' @return
		''' </summary>
		Function opNum() As Integer

		''' <summary>
		''' The opName of this operation
		''' </summary>
		''' <returns> the opName of this operation </returns>
		Function opName() As String

		''' <summary>
		''' The origin ndarray
		''' </summary>
		''' <returns> the origin ndarray </returns>
		Function x() As INDArray

		''' <summary>
		''' The pairwise op ndarray
		''' </summary>
		''' <returns> the pairwise op ndarray </returns>
		Function y() As INDArray

		''' <summary>
		''' The resulting ndarray
		''' </summary>
		''' <returns> the resulting ndarray </returns>
		Function z() As INDArray

		''' <summary>
		''' Extra arguments
		''' </summary>
		''' <returns> the extra arguments </returns>
		Function extraArgs() As Object()


		''' <summary>
		''' set x (the input ndarray) </summary>
		''' <param name="x"> </param>
		WriteOnly Property X As INDArray

		''' <summary>
		''' set z (the solution ndarray) </summary>
		''' <param name="z"> </param>
		WriteOnly Property Z As INDArray

		''' <summary>
		''' set y(the pairwise ndarray) </summary>
		''' <param name="y"> </param>
		WriteOnly Property Y As INDArray

		''' 
		''' <param name="extraArgs"> </param>
		WriteOnly Property ExtraArgs As Object()

		''' <summary>
		''' Converts this op to be a <seealso cref="CustomOp"/>
		''' A <seealso cref="CustomOp"/> is a more flexible op
		''' meant for multiple inputs and outputs.
		''' The default implementation in <seealso cref="BaseOp"/>
		''' converts a simple op to a multi input/output operation
		''' by mapping the x and y on to inputs , the op opName
		''' and the z on to outputs. </summary>
		''' <returns> the equivalent <seealso cref="CustomOp"/> </returns>
		Function toCustomOp() As CustomOp

		''' <summary>
		''' Clear the input and output INDArrays, if any are set
		''' </summary>
		Sub clearArrays()
	End Interface

End Namespace