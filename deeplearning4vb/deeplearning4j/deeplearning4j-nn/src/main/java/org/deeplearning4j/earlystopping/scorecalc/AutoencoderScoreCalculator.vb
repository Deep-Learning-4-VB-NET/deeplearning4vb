Imports org.deeplearning4j.earlystopping.scorecalc.base
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports Model = org.deeplearning4j.nn.api.Model
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports AutoEncoder = org.deeplearning4j.nn.layers.feedforward.autoencoder.AutoEncoder
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports RegressionEvaluation = org.nd4j.evaluation.regression.RegressionEvaluation
Imports Metric = org.nd4j.evaluation.regression.RegressionEvaluation.Metric
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
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

Namespace org.deeplearning4j.earlystopping.scorecalc

	Public Class AutoencoderScoreCalculator
		Inherits BaseScoreCalculator(Of Model)

		Protected Friend ReadOnly metric As RegressionEvaluation.Metric
		Protected Friend evaluation As RegressionEvaluation

		Public Sub New(ByVal metric As RegressionEvaluation.Metric, ByVal iterator As DataSetIterator)
			MyBase.New(iterator)
			Me.metric = metric
		End Sub

		Protected Friend Overrides Sub reset()
			evaluation = New RegressionEvaluation()
		End Sub

		Protected Friend Overrides Function output(ByVal net As Model, ByVal input As INDArray, ByVal fMask As INDArray, ByVal lMask As INDArray) As INDArray

			Dim l As Layer
			If TypeOf net Is MultiLayerNetwork Then
				Dim network As MultiLayerNetwork = DirectCast(net, MultiLayerNetwork)
				l = network.getLayer(0)
			Else
				Dim network As ComputationGraph = DirectCast(net, ComputationGraph)
				l = network.getLayer(0)
			End If

			If Not (TypeOf l Is AutoEncoder) Then
				Throw New System.NotSupportedException("Can only score networks with autoencoder layers as first layer -" & " got " & l.GetType().Name)
			End If
			Dim ae As AutoEncoder = DirectCast(l, AutoEncoder)

			Dim workspaceMgr As LayerWorkspaceMgr = LayerWorkspaceMgr.noWorkspaces()
			Dim encode As INDArray = ae.encode(input, False, workspaceMgr)
			Return ae.decode(encode, workspaceMgr)
		End Function

		Protected Friend Overrides Function output(ByVal network As Model, ByVal input() As INDArray, ByVal fMask() As INDArray, ByVal lMask() As INDArray) As INDArray()
			Return New INDArray(){output(network, get0(input), get0(fMask), get0(lMask))}
		End Function

		Protected Friend Overrides Function scoreMinibatch(ByVal network As Model, ByVal features As INDArray, ByVal labels As INDArray, ByVal fMask As INDArray, ByVal lMask As INDArray, ByVal output As INDArray) As Double
			evaluation.eval(features, output)
			Return 0.0 'Not used
		End Function

		Protected Friend Overrides Function scoreMinibatch(ByVal network As Model, ByVal features() As INDArray, ByVal labels() As INDArray, ByVal fMask() As INDArray, ByVal lMask() As INDArray, ByVal output() As INDArray) As Double
			Return scoreMinibatch(network, get0(features), get0(labels), get0(fMask), get0(lMask), get0(output))
		End Function

		Protected Friend Overrides Function finalScore(ByVal scoreSum As Double, ByVal minibatchCount As Integer, ByVal exampleCount As Integer) As Double
			Return evaluation.scoreForMetric(metric)
		End Function

		Public Overrides Function minimizeScore() As Boolean
			Return metric.minimize()
		End Function
	End Class

End Namespace