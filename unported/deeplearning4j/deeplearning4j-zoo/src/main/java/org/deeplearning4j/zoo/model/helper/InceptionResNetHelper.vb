Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports ElementWiseVertex = org.deeplearning4j.nn.conf.graph.ElementWiseVertex
Imports MergeVertex = org.deeplearning4j.nn.conf.graph.MergeVertex
Imports ScaleVertex = org.deeplearning4j.nn.conf.graph.ScaleVertex
Imports ActivationLayer = org.deeplearning4j.nn.conf.layers.ActivationLayer
Imports BatchNormalization = org.deeplearning4j.nn.conf.layers.BatchNormalization
Imports ConvolutionLayer = org.deeplearning4j.nn.conf.layers.ConvolutionLayer
Imports Activation = org.nd4j.linalg.activations.Activation

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

Namespace org.deeplearning4j.zoo.model.helper

	Public Class InceptionResNetHelper

		Public Shared Function nameLayer(ByVal blockName As String, ByVal layerName As String, ByVal i As Integer) As String
			Return blockName & "-" & layerName & "-" & i
		End Function

		''' <summary>
		''' Append Inception-ResNet A to a computation graph. </summary>
		''' <param name="graph"> </param>
		''' <param name="blockName"> </param>
		''' <param name="scale"> </param>
		''' <param name="activationScale"> </param>
		''' <param name="input">
		''' @return </param>
		Public Shared Function inceptionV1ResA(ByVal graph As ComputationGraphConfiguration.GraphBuilder, ByVal blockName As String, ByVal scale As Integer, ByVal activationScale As Double, ByVal input As String) As ComputationGraphConfiguration.GraphBuilder
			'        // first add the RELU activation layer
			'        graph.addLayer(nameLayer(blockName,"activation1",0), new ActivationLayer.Builder().activation(Activation.TANH).build(), input);

			' loop and add each subsequent resnet blocks
			Dim previousBlock As String = input
			For i As Integer = 1 To scale
				graph.addLayer(nameLayer(blockName, "cnn1", i), (New ConvolutionLayer.Builder(New Integer() {1, 1})).convolutionMode(ConvolutionMode.Same).nIn(192).nOut(32).cudnnAlgoMode(ConvolutionLayer.AlgoMode.NO_WORKSPACE).build(), previousBlock).addLayer(nameLayer(blockName, "batch1", i), (New BatchNormalization.Builder(False)).decay(0.995).eps(0.001).nIn(32).nOut(32).build(), nameLayer(blockName, "cnn1", i)).addLayer(nameLayer(blockName, "cnn2", i), (New ConvolutionLayer.Builder(New Integer() {1, 1})).convolutionMode(ConvolutionMode.Same).nIn(192).nOut(32).cudnnAlgoMode(ConvolutionLayer.AlgoMode.NO_WORKSPACE).build(), previousBlock).addLayer(nameLayer(blockName, "batch2", i), (New BatchNormalization.Builder(False)).decay(0.995).eps(0.001).nIn(32).nOut(32).build(), nameLayer(blockName, "cnn2", i)).addLayer(nameLayer(blockName, "cnn3", i), (New ConvolutionLayer.Builder(New Integer() {3, 3})).convolutionMode(ConvolutionMode.Same).nIn(32).nOut(32).cudnnAlgoMode(ConvolutionLayer.AlgoMode.NO_WORKSPACE).build(), nameLayer(blockName, "batch2", i)).addLayer(nameLayer(blockName, "batch3", i), (New BatchNormalization.Builder(False)).decay(0.995).eps(0.001).nIn(32).nOut(32).build(), nameLayer(blockName, "cnn3", i)).addLayer(nameLayer(blockName, "cnn4", i), (New ConvolutionLayer.Builder(New Integer() {1, 1})).convolutionMode(ConvolutionMode.Same).nIn(192).nOut(32).cudnnAlgoMode(ConvolutionLayer.AlgoMode.NO_WORKSPACE).build(), previousBlock).addLayer(nameLayer(blockName, "batch4", i), (New BatchNormalization.Builder(False)).decay(0.995).eps(0.001).nIn(32).nOut(32).build(), nameLayer(blockName, "cnn4", i)).addLayer(nameLayer(blockName, "cnn5", i), (New ConvolutionLayer.Builder(New Integer() {3, 3})).convolutionMode(ConvolutionMode.Same).nIn(32).nOut(32).cudnnAlgoMode(ConvolutionLayer.AlgoMode.NO_WORKSPACE).build(), nameLayer(blockName, "batch4", i)).addLayer(nameLayer(blockName, "batch5", i), (New BatchNormalization.Builder(False)).decay(0.995).eps(0.001).nIn(32).nOut(32).build(), nameLayer(blockName, "cnn5", i)).addLayer(nameLayer(blockName, "cnn6", i), (New ConvolutionLayer.Builder(New Integer() {3, 3})).convolutionMode(ConvolutionMode.Same).nIn(32).nOut(32).cudnnAlgoMode(ConvolutionLayer.AlgoMode.NO_WORKSPACE).build(), nameLayer(blockName, "batch5", i)).addLayer(nameLayer(blockName, "batch6", i), (New BatchNormalization.Builder(False)).decay(0.995).eps(0.001).nIn(32).nOut(32).build(), nameLayer(blockName, "cnn6", i)).addVertex(nameLayer(blockName, "merge1", i), New MergeVertex(), nameLayer(blockName, "batch1", i), nameLayer(blockName, "batch3", i), nameLayer(blockName, "batch6", i)).addLayer(nameLayer(blockName, "cnn7", i), (New ConvolutionLayer.Builder(New Integer() {3, 3})).convolutionMode(ConvolutionMode.Same).nIn(96).nOut(192).cudnnAlgoMode(ConvolutionLayer.AlgoMode.NO_WORKSPACE).build(), nameLayer(blockName, "merge1", i)).addLayer(nameLayer(blockName, "batch7", i), (New BatchNormalization.Builder(False)).decay(0.995).eps(0.001).nIn(192).nOut(192).build(), nameLayer(blockName, "cnn7", i)).addVertex(nameLayer(blockName, "scaling", i), New ScaleVertex(activationScale), nameLayer(blockName, "batch7", i)).addLayer(nameLayer(blockName, "shortcut-identity", i), (New ActivationLayer.Builder()).activation(Activation.IDENTITY).build(), previousBlock).addVertex(nameLayer(blockName, "shortcut", i), New ElementWiseVertex(ElementWiseVertex.Op.Add), nameLayer(blockName, "scaling", i), nameLayer(blockName, "shortcut-identity", i))

				' leave the last vertex as the block name for convenience
				If i = scale Then
					graph.addLayer(blockName, (New ActivationLayer.Builder()).activation(Activation.TANH).build(), nameLayer(blockName, "shortcut", i))
				Else
					graph.addLayer(nameLayer(blockName, "activation", i), (New ActivationLayer.Builder()).activation(Activation.TANH).build(), nameLayer(blockName, "shortcut", i))
				End If

				previousBlock = nameLayer(blockName, "activation", i)
			Next i
			Return graph
		End Function

		''' <summary>
		''' Append Inception-ResNet B to a computation graph. </summary>
		''' <param name="graph"> </param>
		''' <param name="blockName"> </param>
		''' <param name="scale"> </param>
		''' <param name="activationScale"> </param>
		''' <param name="input">
		''' @return </param>
		Public Shared Function inceptionV1ResB(ByVal graph As ComputationGraphConfiguration.GraphBuilder, ByVal blockName As String, ByVal scale As Integer, ByVal activationScale As Double, ByVal input As String) As ComputationGraphConfiguration.GraphBuilder
			' first add the RELU activation layer
			graph.addLayer(nameLayer(blockName, "activation1", 0), (New ActivationLayer.Builder()).activation(Activation.TANH).build(), input)

			' loop and add each subsequent resnet blocks
			Dim previousBlock As String = nameLayer(blockName, "activation1", 0)
			For i As Integer = 1 To scale
				graph.addLayer(nameLayer(blockName, "cnn1", i), (New ConvolutionLayer.Builder(New Integer() {1, 1})).convolutionMode(ConvolutionMode.Same).nIn(576).nOut(128).cudnnAlgoMode(ConvolutionLayer.AlgoMode.NO_WORKSPACE).build(), previousBlock).addLayer(nameLayer(blockName, "batch1", i), (New BatchNormalization.Builder(False)).decay(0.995).eps(0.001).nIn(128).nOut(128).build(), nameLayer(blockName, "cnn1", i)).addLayer(nameLayer(blockName, "cnn2", i), (New ConvolutionLayer.Builder(New Integer() {1, 1})).convolutionMode(ConvolutionMode.Same).nIn(576).nOut(128).cudnnAlgoMode(ConvolutionLayer.AlgoMode.NO_WORKSPACE).build(), previousBlock).addLayer(nameLayer(blockName, "batch2", i), (New BatchNormalization.Builder(False)).decay(0.995).eps(0.001).nIn(128).nOut(128).build(), nameLayer(blockName, "cnn2", i)).addLayer(nameLayer(blockName, "cnn3", i), (New ConvolutionLayer.Builder(New Integer() {1, 3})).convolutionMode(ConvolutionMode.Same).nIn(128).nOut(128).cudnnAlgoMode(ConvolutionLayer.AlgoMode.NO_WORKSPACE).build(), nameLayer(blockName, "batch2", i)).addLayer(nameLayer(blockName, "batch3", i), (New BatchNormalization.Builder(False)).decay(0.995).eps(0.001).nIn(128).nOut(128).build(), nameLayer(blockName, "cnn3", i)).addLayer(nameLayer(blockName, "cnn4", i), (New ConvolutionLayer.Builder(New Integer() {3, 1})).convolutionMode(ConvolutionMode.Same).nIn(128).nOut(128).cudnnAlgoMode(ConvolutionLayer.AlgoMode.NO_WORKSPACE).build(), nameLayer(blockName, "batch3", i)).addLayer(nameLayer(blockName, "batch4", i), (New BatchNormalization.Builder(False)).decay(0.995).eps(0.001).nIn(128).nOut(128).build(), nameLayer(blockName, "cnn4", i)).addVertex(nameLayer(blockName, "merge1", i), New MergeVertex(), nameLayer(blockName, "batch1", i), nameLayer(blockName, "batch4", i)).addLayer(nameLayer(blockName, "cnn5", i), (New ConvolutionLayer.Builder(New Integer() {1, 1})).convolutionMode(ConvolutionMode.Same).nIn(256).nOut(576).cudnnAlgoMode(ConvolutionLayer.AlgoMode.NO_WORKSPACE).build(), nameLayer(blockName, "merge1", i)).addLayer(nameLayer(blockName, "batch5", i), (New BatchNormalization.Builder(False)).decay(0.995).eps(0.001).nIn(576).nOut(576).build(), nameLayer(blockName, "cnn5", i)).addVertex(nameLayer(blockName, "scaling", i), New ScaleVertex(activationScale), nameLayer(blockName, "batch5", i)).addLayer(nameLayer(blockName, "shortcut-identity", i), (New ActivationLayer.Builder()).activation(Activation.IDENTITY).build(), previousBlock).addVertex(nameLayer(blockName, "shortcut", i), New ElementWiseVertex(ElementWiseVertex.Op.Add), nameLayer(blockName, "scaling", i), nameLayer(blockName, "shortcut-identity", i))

				' leave the last vertex as the block name for convenience
				If i = scale Then
					graph.addLayer(blockName, (New ActivationLayer.Builder()).activation(Activation.TANH).build(), nameLayer(blockName, "shortcut", i))
				Else
					graph.addLayer(nameLayer(blockName, "activation", i), (New ActivationLayer.Builder()).activation(Activation.TANH).build(), nameLayer(blockName, "shortcut", i))
				End If

				previousBlock = nameLayer(blockName, "activation", i)
			Next i
			Return graph
		End Function

		''' <summary>
		''' Append Inception-ResNet C to a computation graph. </summary>
		''' <param name="graph"> </param>
		''' <param name="blockName"> </param>
		''' <param name="scale"> </param>
		''' <param name="activationScale"> </param>
		''' <param name="input">
		''' @return </param>
		Public Shared Function inceptionV1ResC(ByVal graph As ComputationGraphConfiguration.GraphBuilder, ByVal blockName As String, ByVal scale As Integer, ByVal activationScale As Double, ByVal input As String) As ComputationGraphConfiguration.GraphBuilder
			' loop and add each subsequent resnet blocks
			Dim previousBlock As String = input
			For i As Integer = 1 To scale
				graph.addLayer(nameLayer(blockName, "cnn1", i), (New ConvolutionLayer.Builder(New Integer() {1, 1})).convolutionMode(ConvolutionMode.Same).nIn(1344).nOut(192).cudnnAlgoMode(ConvolutionLayer.AlgoMode.NO_WORKSPACE).build(), previousBlock).addLayer(nameLayer(blockName, "batch1", i), (New BatchNormalization.Builder(False)).decay(0.995).eps(0.001).nIn(192).nOut(192).build(), nameLayer(blockName, "cnn1", i)).addLayer(nameLayer(blockName, "cnn2", i), (New ConvolutionLayer.Builder(New Integer() {1, 1})).convolutionMode(ConvolutionMode.Same).nIn(1344).nOut(192).cudnnAlgoMode(ConvolutionLayer.AlgoMode.NO_WORKSPACE).build(), previousBlock).addLayer(nameLayer(blockName, "batch2", i), (New BatchNormalization.Builder(False)).decay(0.995).eps(0.001).nIn(192).nOut(192).build(), nameLayer(blockName, "cnn2", i)).addLayer(nameLayer(blockName, "cnn3", i), (New ConvolutionLayer.Builder(New Integer() {1, 3})).convolutionMode(ConvolutionMode.Same).nIn(192).nOut(192).cudnnAlgoMode(ConvolutionLayer.AlgoMode.NO_WORKSPACE).build(), nameLayer(blockName, "batch2", i)).addLayer(nameLayer(blockName, "batch3", i), (New BatchNormalization.Builder(False)).decay(0.995).eps(0.001).nIn(192).nOut(192).build(), nameLayer(blockName, "cnn3", i)).addLayer(nameLayer(blockName, "cnn4", i), (New ConvolutionLayer.Builder(New Integer() {3, 1})).convolutionMode(ConvolutionMode.Same).nIn(192).nOut(192).cudnnAlgoMode(ConvolutionLayer.AlgoMode.NO_WORKSPACE).build(), nameLayer(blockName, "batch3", i)).addLayer(nameLayer(blockName, "batch4", i), (New BatchNormalization.Builder(False)).decay(0.995).eps(0.001).activation(Activation.TANH).nIn(192).nOut(192).build(), nameLayer(blockName, "cnn4", i)).addVertex(nameLayer(blockName, "merge1", i), New MergeVertex(), nameLayer(blockName, "batch1", i), nameLayer(blockName, "batch4", i)).addLayer(nameLayer(blockName, "cnn5", i), (New ConvolutionLayer.Builder(New Integer() {1, 1})).convolutionMode(ConvolutionMode.Same).nIn(384).nOut(1344).cudnnAlgoMode(ConvolutionLayer.AlgoMode.NO_WORKSPACE).build(), nameLayer(blockName, "merge1", i)).addLayer(nameLayer(blockName, "batch5", i), (New BatchNormalization.Builder(False)).decay(0.995).eps(0.001).activation(Activation.TANH).nIn(1344).nOut(1344).build(), nameLayer(blockName, "cnn5", i)).addVertex(nameLayer(blockName, "scaling", i), New ScaleVertex(activationScale), nameLayer(blockName, "batch5", i)).addLayer(nameLayer(blockName, "shortcut-identity", i), (New ActivationLayer.Builder()).activation(Activation.IDENTITY).build(), previousBlock).addVertex(nameLayer(blockName, "shortcut", i), New ElementWiseVertex(ElementWiseVertex.Op.Add), nameLayer(blockName, "scaling", i), nameLayer(blockName, "shortcut-identity", i))

				' leave the last vertex as the block name for convenience
				If i = scale Then
					graph.addLayer(blockName, (New ActivationLayer.Builder()).activation(Activation.TANH).build(), nameLayer(blockName, "shortcut", i))
				Else
					graph.addLayer(nameLayer(blockName, "activation", i), (New ActivationLayer.Builder()).activation(Activation.TANH).build(), nameLayer(blockName, "shortcut", i))
				End If

				previousBlock = nameLayer(blockName, "activation", i)
			Next i
			Return graph
		End Function

	End Class

End Namespace