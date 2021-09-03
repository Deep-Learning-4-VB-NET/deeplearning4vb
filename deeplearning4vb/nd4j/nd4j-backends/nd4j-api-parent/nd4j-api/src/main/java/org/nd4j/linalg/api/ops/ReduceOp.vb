Imports System
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

	Public Interface ReduceOp
		Inherits Op

		''' <summary>
		''' Returns the no op version
		''' of the input
		''' Basically when a reduce can't happen (eg: sum(0) on a row vector)
		''' you have a no op state for a given reduction.
		''' For most accumulations, this should return x
		''' but certain transformations should return say: the absolute value
		''' 
		''' </summary>
		''' <returns> the no op version of the input </returns>
		Function noOp() As INDArray

		''' <summary>
		''' This method returns dimensions for this op
		''' @return
		''' </summary>
		Function dimensions() As INDArray

		<Obsolete>
		ReadOnly Property ComplexAccumulation As Boolean

		ReadOnly Property OpType As Type

		''' <summary>
		''' This method returns TRUE if we're going to keep axis, FALSE otherwise
		''' 
		''' @return
		''' </summary>
		ReadOnly Property KeepDims As Boolean

		''' <summary>
		''' This method returns datatype for result array wrt given inputs
		''' @return
		''' </summary>
		Function resultType() As DataType

		Function resultType(ByVal oc As OpContext) As DataType

		Function validateDataTypes(ByVal oc As OpContext) As Boolean

		ReadOnly Property FinalResult As Number

		WriteOnly Property Dimensions As Integer()
	End Interface

End Namespace