Imports System
Imports System.Collections.Generic
Imports INDArrayDataSetIterator = org.deeplearning4j.datasets.iterator.INDArrayDataSetIterator
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports org.deeplearning4j.nn.conf.layers
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports Test = org.junit.jupiter.api.Test
Imports org.nd4j.common.primitives
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nesterovs = org.nd4j.linalg.learning.config.Nesterovs
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions

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
Namespace org.deeplearning4j.nn.graph


	Public Class TestCompGraphWorkSpaces
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testWorkspaces()
		Public Overridable Sub testWorkspaces()

			Try
				Dim computationGraphConf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(123).updater(New Nesterovs(0.1, 0.9)).graphBuilder().addInputs("input").appendLayer("L1", (New ConvolutionLayer.Builder(New Integer(){3, 3}, New Integer(){1, 1}, New Integer(){1, 1})).nIn(1).nOut(1).hasBias(False).build()).appendLayer("out", (New CnnLossLayer.Builder()).activation(Activation.SIGMOID).lossFunction(LossFunctions.LossFunction.XENT).build()).setOutputs("out").build()

				Dim graph As New ComputationGraph(computationGraphConf)

				Dim data1 As INDArray = Nd4j.create(1, 1, 256, 256)
				Dim data2 As INDArray = Nd4j.create(1, 1, 256, 256)
				Dim label1 As INDArray = Nd4j.create(1, 1, 256, 256)
				Dim label2 As INDArray = Nd4j.create(1, 1, 256, 256)
				Dim trainData As IList(Of Pair(Of INDArray, INDArray)) = Collections.singletonList(New Pair(Of Pair(Of INDArray, INDArray))(data1, label1))
				Dim testData As IList(Of Pair(Of INDArray, INDArray)) = Collections.singletonList(New Pair(Of Pair(Of INDArray, INDArray))(data2, label2))
				Dim trainIter As DataSetIterator = New INDArrayDataSetIterator(trainData, 1)
				Dim testIter As DataSetIterator = New INDArrayDataSetIterator(testData, 1)

				graph.fit(trainIter)

				Do While testIter.MoveNext()
					graph.score(testIter.Current)
				Loop

			Catch e As Exception
				Console.WriteLine(e.ToString())
				Console.Write(e.StackTrace)
			End Try
		End Sub

	End Class

End Namespace