Imports System
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
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
Namespace org.deeplearning4j.rl4j.helper

	''' <summary>
	''' INDArray helper methods used by RL4J
	''' 
	''' @author Alexandre Boulanger
	''' </summary>
	Public Class INDArrayHelper
		''' <summary>
		''' Force the input source to have the correct shape:
		'''  <para><ul>
		'''      <li>DL4J requires it to be at least 2D</li>
		'''      <li>RL4J has a convention to have the batch size on dimension 0 to all INDArrays</li>
		'''  </ul></para> </summary>
		''' <param name="source"> The <seealso cref="INDArray"/> to be corrected. </param>
		''' <returns> The corrected INDArray </returns>
		Public Shared Function forceCorrectShape(ByVal source As INDArray) As INDArray

			Return If(source.shape()(0) = 1 AndAlso source.rank() > 1, source, Nd4j.expandDims(source, 0))

		End Function

		''' <summary>
		''' This will create a INDArray with <i>batchSize</i> as dimension 0 and <i>shape</i> as other dimensions.
		''' For example, if <i>batchSize</i> is 10 and shape is { 1, 3, 4 }, the resulting INDArray shape will be { 10, 3, 4 } </summary>
		''' <param name="batchSize"> The size of the batch to create </param>
		''' <param name="shape"> The shape of individual elements.
		'''              Note: all shapes in RL4J should have a batch size as dimension 0; in this case the batch size should be 1. </param>
		''' <returns> A INDArray </returns>
		Public Shared Function createBatchForShape(ByVal batchSize As Long, ParamArray ByVal shape() As Long) As INDArray
			Dim batchShape() As Long

			batchShape = New Long(shape.Length - 1){}
			Array.Copy(shape, 0, batchShape, 0, shape.Length)

			batchShape(0) = batchSize
			Return Nd4j.create(batchShape)
		End Function

		''' <summary>
		''' This will create a INDArray to be used with RNNs. Dimension 0 is set to 1, <i>batchSize</i> will be used as the
		''' time-step dimension (last dimension), and <i>shape</i> as other dimensions.
		''' For example, if <i>batchSize</i> is 5 and shape is { 1, 3, 1 }, the resulting INDArray shape will be { 1, 3, 5 } </summary>
		''' <param name="batchSize"> The size of the batch to create </param>
		''' <param name="shape"> The shape of individual elements.
		'''              Note: all shapes in RL4J should have a batch size as dimension 0; in this case the batch size should be 1.
		'''                    And recurrent INDArrays should have their time-serie dimension as the last. </param>
		''' <returns> A INDArray </returns>
		Public Shared Function createRnnBatchForShape(ByVal batchSize As Long, ParamArray ByVal shape() As Long) As INDArray
			Dim batchShape() As Long

			batchShape = New Long(shape.Length - 1){}
			Array.Copy(shape, 0, batchShape, 0, shape.Length)

			batchShape(0) = 1
			batchShape(shape.Length - 1) = batchSize
			Return Nd4j.create(batchShape)
		End Function

	End Class

End Namespace