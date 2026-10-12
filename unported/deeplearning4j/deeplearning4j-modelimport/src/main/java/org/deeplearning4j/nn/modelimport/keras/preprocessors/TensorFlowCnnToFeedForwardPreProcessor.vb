Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports CnnToFeedForwardPreProcessor = org.deeplearning4j.nn.conf.preprocessor.CnnToFeedForwardPreProcessor
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports JsonCreator = org.nd4j.shade.jackson.annotation.JsonCreator
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty

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

Namespace org.deeplearning4j.nn.modelimport.keras.preprocessors

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Deprecated public class TensorFlowCnnToFeedForwardPreProcessor extends org.deeplearning4j.nn.conf.preprocessor.CnnToFeedForwardPreProcessor
	<Obsolete, Serializable>
	Public Class TensorFlowCnnToFeedForwardPreProcessor
		Inherits CnnToFeedForwardPreProcessor

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonCreator @Deprecated public TensorFlowCnnToFeedForwardPreProcessor(@JsonProperty("inputHeight") long inputHeight, @JsonProperty("inputWidth") long inputWidth, @JsonProperty("numChannels") long numChannels)
		<Obsolete>
		Public Sub New(ByVal inputHeight As Long, ByVal inputWidth As Long, ByVal numChannels As Long)
			MyBase.New(inputHeight, inputWidth, numChannels)
		End Sub

		<Obsolete>
		Public Sub New(ByVal inputHeight As Long, ByVal inputWidth As Long)
			MyBase.New(inputHeight, inputWidth)
		End Sub

		<Obsolete>
		Public Sub New()
			MyBase.New()
		End Sub

		Public Overrides Function preProcess(ByVal input As INDArray, ByVal miniBatchSize As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			If input.rank() = 2 Then
				Return workspaceMgr.leverageTo(ArrayType.ACTIVATIONS, input) 'Should usually never happen
			End If
	'         DL4J convolutional input:       # channels, # rows, # cols
	'         * TensorFlow convolutional input: # rows, # cols, # channels
	'         * Theano convolutional input:     # channels, # rows, # cols
	'         
			Dim permuted As INDArray = workspaceMgr.dup(ArrayType.ACTIVATIONS, input.permute(0, 2, 3, 1), "c"c) 'To: [n, h, w, c]

			Dim inShape As val = input.shape() '[miniBatch,depthOut,outH,outW]
			Dim outShape As val = New Long(){inShape(0), inShape(1) * inShape(2) * inShape(3)}

			Return workspaceMgr.leverageTo(ArrayType.ACTIVATIONS, permuted.reshape("c"c, outShape))
		End Function

		Public Overrides Function backprop(ByVal epsilons As INDArray, ByVal miniBatchSize As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			If epsilons.ordering() <> "c"c OrElse Not Shape.hasDefaultStridesForShape(epsilons) Then
				epsilons = workspaceMgr.dup(ArrayType.ACTIVATION_GRAD, epsilons, "c"c)
			End If

			Dim epsilonsReshaped As INDArray = epsilons.reshape("c"c, epsilons.size(0), inputHeight, inputWidth, numChannels)

			Return workspaceMgr.leverageTo(ArrayType.ACTIVATION_GRAD, epsilonsReshaped.permute(0, 3, 1, 2)) 'To [n, c, h, w]
		End Function

		Public Overrides Function clone() As TensorFlowCnnToFeedForwardPreProcessor
			Return CType(MyBase.clone(), TensorFlowCnnToFeedForwardPreProcessor)
		End Function
	End Class
End Namespace