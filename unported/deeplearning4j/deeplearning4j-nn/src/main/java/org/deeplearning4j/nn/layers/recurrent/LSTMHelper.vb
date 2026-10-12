Imports System.Collections.Generic
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports LayerHelper = org.deeplearning4j.nn.layers.LayerHelper
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.common.primitives
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr

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

Namespace org.deeplearning4j.nn.layers.recurrent


	Public Interface LSTMHelper
		Inherits LayerHelper

		Function checkSupported(ByVal gateActivationFn As IActivation, ByVal activationFn As IActivation, ByVal hasPeepholeConnections As Boolean) As Boolean

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: org.nd4j.common.primitives.Pair<org.deeplearning4j.nn.gradient.Gradient, org.nd4j.linalg.api.ndarray.INDArray> backpropGradient(final org.deeplearning4j.nn.conf.NeuralNetConfiguration conf, final org.nd4j.linalg.activations.IActivation gateActivationFn, final org.nd4j.linalg.api.ndarray.INDArray input, final org.nd4j.linalg.api.ndarray.INDArray recurrentWeights, final org.nd4j.linalg.api.ndarray.INDArray inputWeights, final org.nd4j.linalg.api.ndarray.INDArray epsilon, final boolean truncatedBPTT, final int tbpttBackwardLength, final FwdPassReturn fwdPass, final boolean forwards, final String inputWeightKey, final String recurrentWeightKey, final String biasWeightKey, final java.util.Map<String, org.nd4j.linalg.api.ndarray.INDArray> gradientViews, org.nd4j.linalg.api.ndarray.INDArray maskArray, final boolean hasPeepholeConnections, final org.deeplearning4j.nn.workspace.LayerWorkspaceMgr workspaceMgr);
		Function backpropGradient(ByVal conf As NeuralNetConfiguration, ByVal gateActivationFn As IActivation, ByVal input As INDArray, ByVal recurrentWeights As INDArray, ByVal inputWeights As INDArray, ByVal epsilon As INDArray, ByVal truncatedBPTT As Boolean, ByVal tbpttBackwardLength As Integer, ByVal fwdPass As FwdPassReturn, ByVal forwards As Boolean, ByVal inputWeightKey As String, ByVal recurrentWeightKey As String, ByVal biasWeightKey As String, ByVal gradientViews As IDictionary(Of String, INDArray), ByVal maskArray As INDArray, ByVal hasPeepholeConnections As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: FwdPassReturn activate(final org.deeplearning4j.nn.api.Layer layer, final org.deeplearning4j.nn.conf.NeuralNetConfiguration conf, final org.nd4j.linalg.activations.IActivation gateActivationFn, final org.nd4j.linalg.api.ndarray.INDArray input, final org.nd4j.linalg.api.ndarray.INDArray recurrentWeights, final org.nd4j.linalg.api.ndarray.INDArray inputWeights, final org.nd4j.linalg.api.ndarray.INDArray biases, final boolean training, final org.nd4j.linalg.api.ndarray.INDArray prevOutputActivations, final org.nd4j.linalg.api.ndarray.INDArray prevMemCellState, boolean forBackprop, boolean forwards, final String inputWeightKey, org.nd4j.linalg.api.ndarray.INDArray maskArray, final boolean hasPeepholeConnections, final org.deeplearning4j.nn.workspace.LayerWorkspaceMgr workspaceMgr);
		Function activate(ByVal layer As Layer, ByVal conf As NeuralNetConfiguration, ByVal gateActivationFn As IActivation, ByVal input As INDArray, ByVal recurrentWeights As INDArray, ByVal inputWeights As INDArray, ByVal biases As INDArray, ByVal training As Boolean, ByVal prevOutputActivations As INDArray, ByVal prevMemCellState As INDArray, ByVal forBackprop As Boolean, ByVal forwards As Boolean, ByVal inputWeightKey As String, ByVal maskArray As INDArray, ByVal hasPeepholeConnections As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As FwdPassReturn
	End Interface

End Namespace