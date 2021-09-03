Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports AlgoMode = org.deeplearning4j.nn.conf.layers.ConvolutionLayer.AlgoMode
Imports BwdDataAlgo = org.deeplearning4j.nn.conf.layers.ConvolutionLayer.BwdDataAlgo
Imports BwdFilterAlgo = org.deeplearning4j.nn.conf.layers.ConvolutionLayer.BwdFilterAlgo
Imports FwdAlgo = org.deeplearning4j.nn.conf.layers.ConvolutionLayer.FwdAlgo
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

Namespace org.deeplearning4j.nn.layers.convolution

	Public Interface ConvolutionHelper
		Inherits LayerHelper

		Function checkSupported() As Boolean

		Function backpropGradient(ByVal input As INDArray, ByVal weights As INDArray, ByVal bias As INDArray, ByVal delta As INDArray, ByVal kernel() As Integer, ByVal strides() As Integer, ByVal pad() As Integer, ByVal biasGradView As INDArray, ByVal weightGradView As INDArray, ByVal afn As IActivation, ByVal mode As AlgoMode, ByVal bwdFilterAlgo As BwdFilterAlgo, ByVal bwdDataAlgo As BwdDataAlgo, ByVal convolutionMode As ConvolutionMode, ByVal dilation() As Integer, ByVal format As CNN2DFormat, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)

		Function preOutput(ByVal input As INDArray, ByVal weights As INDArray, ByVal bias As INDArray, ByVal kernel() As Integer, ByVal strides() As Integer, ByVal pad() As Integer, ByVal mode As AlgoMode, ByVal fwdAlgo As FwdAlgo, ByVal convolutionMode As ConvolutionMode, ByVal dilation() As Integer, ByVal format As CNN2DFormat, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray

		Function activate(ByVal z As INDArray, ByVal afn As IActivation, ByVal training As Boolean) As INDArray
	End Interface

End Namespace