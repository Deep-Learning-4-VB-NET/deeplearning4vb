Imports System
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.common.primitives
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports JsonTypeInfo = org.nd4j.shade.jackson.annotation.JsonTypeInfo

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

Namespace org.deeplearning4j.nn.conf



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonTypeInfo(use = JsonTypeInfo.Id.@CLASS, include = JsonTypeInfo.@As.@PROPERTY, property = "@class") public interface InputPreProcessor extends java.io.Serializable, Cloneable
	Public Interface InputPreProcessor
		Inherits ICloneable

		''' <summary>
		''' Pre preProcess input/activations for a multi layer network </summary>
		''' <param name="input"> the input to pre preProcess </param>
		''' <param name="miniBatchSize"> Minibatch size </param>
		''' <param name="workspaceMgr"> Workspace manager </param>
		''' <returns> the processed input. Note that the returned array should be placed in the
		'''         <seealso cref="org.deeplearning4j.nn.workspace.ArrayType.ACTIVATIONS"/> workspace via the workspace manager </returns>
		Function preProcess(ByVal input As INDArray, ByVal miniBatchSize As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray

		''' <summary>
		'''Reverse the preProcess during backprop. Process Gradient/epsilons before
		''' passing them to the layer below. </summary>
		''' <param name="output"> which is a pair of the gradient and epsilon </param>
		''' <param name="miniBatchSize"> Minibatch size </param>
		''' <param name="workspaceMgr"> Workspace manager </param>
		''' <returns> the reverse of the pre preProcess step (if any). Note that the returned array should be
		'''         placed in <seealso cref="org.deeplearning4j.nn.workspace.ArrayType.ACTIVATION_GRAD"/> workspace via the
		'''         workspace manager </returns>
		Function backprop(ByVal output As INDArray, ByVal miniBatchSize As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray

		Function clone() As InputPreProcessor

		''' <summary>
		''' For a given type of input to this preprocessor, what is the type of the output?
		''' </summary>
		''' <param name="inputType">    Type of input for the preprocessor </param>
		''' <returns>             Type of input after applying the preprocessor </returns>
		Function getOutputType(ByVal inputType As InputType) As InputType


		Function feedForwardMaskArray(ByVal maskArray As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState)
	End Interface

End Namespace