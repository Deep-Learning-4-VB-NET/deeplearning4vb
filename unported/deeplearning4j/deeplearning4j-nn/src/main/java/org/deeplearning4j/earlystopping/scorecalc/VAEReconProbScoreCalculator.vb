Imports org.deeplearning4j.earlystopping.scorecalc.base
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports Model = org.deeplearning4j.nn.api.Model
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports VariationalAutoencoder = org.deeplearning4j.nn.layers.variational.VariationalAutoencoder
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator

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

	Public Class VAEReconProbScoreCalculator
		Inherits BaseScoreCalculator(Of Model)

		Protected Friend ReadOnly reconstructionProbNumSamples As Integer
		Protected Friend ReadOnly logProb As Boolean
		Protected Friend ReadOnly average As Boolean

		''' <summary>
		''' Constructor for average reconstruction probability
		''' </summary>
		''' <param name="iterator"> Iterator </param>
		''' <param name="reconstructionProbNumSamples"> Number of samples. See <seealso cref="VariationalAutoencoder.reconstructionProbability(INDArray, Integer)"/>
		'''                                    for details </param>
		''' <param name="logProb"> If true: calculate (negative) log probability. False: probability </param>
		Public Sub New(ByVal iterator As DataSetIterator, ByVal reconstructionProbNumSamples As Integer, ByVal logProb As Boolean)
			Me.New(iterator, reconstructionProbNumSamples, logProb, True)
		End Sub

		''' <summary>
		''' Constructor for reconstruction probability
		''' </summary>
		''' <param name="iterator"> Iterator </param>
		''' <param name="reconstructionProbNumSamples"> Number of samples. See <seealso cref="VariationalAutoencoder.reconstructionProbability(INDArray, Integer)"/>
		'''                                    for details </param>
		''' <param name="logProb"> If true: calculate (negative) log probability. False: probability </param>
		''' <param name="average"> If true: return average (log) probability. False: sum of log probability.
		'''  </param>
		Public Sub New(ByVal iterator As DataSetIterator, ByVal reconstructionProbNumSamples As Integer, ByVal logProb As Boolean, ByVal average As Boolean)
			MyBase.New(iterator)
			Me.reconstructionProbNumSamples = reconstructionProbNumSamples
			Me.logProb = logProb
			Me.average = average
		End Sub

		Protected Friend Overrides Sub reset()
			scoreSum = 0
			minibatchCount = 0
			exampleCount = 0
		End Sub

		Protected Friend Overrides Function output(ByVal network As Model, ByVal input As INDArray, ByVal fMask As INDArray, ByVal lMask As INDArray) As INDArray
			Return Nothing 'Not used
		End Function

		Protected Friend Overrides Function output(ByVal network As Model, ByVal input() As INDArray, ByVal fMask() As INDArray, ByVal lMask() As INDArray) As INDArray()
			Return Nothing 'Not used
		End Function

		Protected Friend Overrides Function scoreMinibatch(ByVal net As Model, ByVal features As INDArray, ByVal labels As INDArray, ByVal fMask As INDArray, ByVal lMask As INDArray, ByVal output As INDArray) As Double
			Dim l As Layer
			If TypeOf net Is MultiLayerNetwork Then
				Dim network As MultiLayerNetwork = DirectCast(net, MultiLayerNetwork)
				l = network.getLayer(0)
			Else
				Dim network As ComputationGraph = DirectCast(net, ComputationGraph)
				l = network.getLayer(0)
			End If

			If Not (TypeOf l Is VariationalAutoencoder) Then
				Throw New System.NotSupportedException("Can only score networks with VariationalAutoencoder layers as first layer -" & " got " & l.GetType().Name)
			End If
			Dim vae As VariationalAutoencoder = DirectCast(l, VariationalAutoencoder)
			'Reconstruction prob
			If logProb Then
				Return -vae.reconstructionLogProbability(features, reconstructionProbNumSamples).sumNumber().doubleValue()
			Else
				Return vae.reconstructionProbability(features, reconstructionProbNumSamples).sumNumber().doubleValue()
			End If
		End Function

		Protected Friend Overrides Function scoreMinibatch(ByVal network As Model, ByVal features() As INDArray, ByVal labels() As INDArray, ByVal fMask() As INDArray, ByVal lMask() As INDArray, ByVal output() As INDArray) As Double
			Return 0
		End Function

		Protected Friend Overrides Function finalScore(ByVal scoreSum As Double, ByVal minibatchCount As Integer, ByVal exampleCount As Integer) As Double
			If average Then
				Return scoreSum / exampleCount
			Else
				Return scoreSum
			End If
		End Function

		Public Overrides Function minimizeScore() As Boolean
			Return False 'Maximize the reconstruction probability
		End Function
	End Class

End Namespace