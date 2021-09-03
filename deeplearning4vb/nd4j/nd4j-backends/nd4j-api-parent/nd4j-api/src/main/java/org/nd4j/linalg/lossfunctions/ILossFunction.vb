Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.common.primitives
Imports LegacyILossFunctionDeserializerHelper = org.nd4j.serde.json.LegacyILossFunctionDeserializerHelper
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

Namespace org.nd4j.linalg.lossfunctions



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonTypeInfo(use = JsonTypeInfo.Id.@CLASS, include = JsonTypeInfo.@As.@PROPERTY, property = "@class", defaultImpl = LegacyILossFunctionDeserializerHelper.class) public interface ILossFunction extends java.io.Serializable
	Public Interface ILossFunction

		''' <summary>
		''' Compute the score (loss function value) for the given inputs. </summary>
		'''  <param name="labels">       Label/expected preOutput </param>
		''' <param name="preOutput">    Output of the model (neural network) </param>
		''' <param name="activationFn"> Activation function that should be applied to preOutput </param>
		''' <param name="mask">         Mask array; may be null </param> </param>
		''' <param name="average">      Whether the score should be averaged (divided by number of rows in labels/preOutput) or not   <returns> Loss function value </returns>
		Function computeScore(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray, ByVal average As Boolean) As Double

		''' <summary>
		''' Compute the score (loss function value) for each example individually.
		''' For input [numExamples,nOut] returns scores as a column vector: [numExamples,1] </summary>
		'''  <param name="labels">       Labels/expected output </param>
		''' <param name="preOutput">    Output of the model (neural network) </param>
		''' <param name="activationFn"> Activation function that should be applied to preOutput </param> </param>
		''' <param name="mask">         <returns> Loss function value for each example; column vector </returns>
		Function computeScoreArray(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray) As INDArray

		''' <summary>
		''' Compute the gradient of the loss function with respect to the inputs: dL/dOutput
		''' </summary>
		''' <param name="labels">       Label/expected output </param>
		''' <param name="preOutput">    Output of the model (neural network), before the activation function is applied </param>
		''' <param name="activationFn"> Activation function that should be applied to preOutput </param>
		''' <param name="mask">         Mask array; may be null </param>
		''' <returns> Gradient dL/dPreOut </returns>
		Function computeGradient(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray) As INDArray

		''' <summary>
		''' Compute both the score (loss function value) and gradient. This is equivalent to calling <seealso cref="computeScore(INDArray, INDArray, IActivation, INDArray, Boolean)"/>
		''' and <seealso cref="computeGradient(INDArray, INDArray, IActivation, INDArray)"/> individually
		''' </summary>
		''' <param name="labels">       Label/expected output </param>
		''' <param name="preOutput">    Output of the model (neural network) </param>
		''' <param name="activationFn"> Activation function that should be applied to preOutput </param>
		''' <param name="mask">         Mask array; may be null </param>
		''' <param name="average">      Whether the score should be averaged (divided by number of rows in labels/output) or not </param>
		''' <returns> The score (loss function value) and gradient </returns>
		'TODO: do we want to use the apache commons pair here?
		Function computeGradientAndScore(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray, ByVal average As Boolean) As Pair(Of Double, INDArray)

		''' <summary>
		''' The opName of this function
		''' @return
		''' </summary>
		Function name() As String

	End Interface

End Namespace