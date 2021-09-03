Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports SingletonMultiDataSetIterator = org.deeplearning4j.datasets.iterator.impl.SingletonMultiDataSetIterator
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports MergeVertex = org.deeplearning4j.nn.conf.graph.MergeVertex
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports AutoEncoder = org.deeplearning4j.nn.conf.layers.AutoEncoder
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith

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
Namespace org.deeplearning4j.nn.layers

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Auto Encoder Test") @NativeTag @Tag(TagNames.CUSTOM_FUNCTIONALITY) @Tag(TagNames.DL4J_OLD_API) class AutoEncoderTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class AutoEncoderTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Sanity Check Issue 5662") void sanityCheckIssue5662()
		Friend Overridable Sub sanityCheckIssue5662()
			Dim mergeSize As Integer = 50
			Dim encdecSize As Integer = 25
			Dim in1Size As Integer = 20
			Dim in2Size As Integer = 15
			Dim hiddenSize As Integer = 10
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).weightInit(WeightInit.XAVIER).graphBuilder().addInputs("in1", "in2").addLayer("1", (New DenseLayer.Builder()).nOut(mergeSize).build(), "in1").addLayer("2", (New DenseLayer.Builder()).nOut(mergeSize).build(), "in2").addVertex("merge", New MergeVertex(), "1", "2").addLayer("e", (New AutoEncoder.Builder()).nOut(encdecSize).corruptionLevel(0.2).build(), "merge").addLayer("hidden", (New AutoEncoder.Builder()).nOut(hiddenSize).build(), "e").addLayer("decoder", (New AutoEncoder.Builder()).nOut(encdecSize).corruptionLevel(0.2).build(), "hidden").addLayer("L4", (New DenseLayer.Builder()).nOut(mergeSize).build(), "decoder").addLayer("out1", (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nOut(in1Size).build(), "L4").addLayer("out2", (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nOut(in2Size).build(), "L4").setOutputs("out1", "out2").setInputTypes(InputType.feedForward(in1Size), InputType.feedForward(in2Size)).build()
			Dim net As New ComputationGraph(conf)
			net.init()
			Dim mds As MultiDataSet = New org.nd4j.linalg.dataset.MultiDataSet(New INDArray() { Nd4j.create(1, in1Size), Nd4j.create(1, in2Size) }, New INDArray() { Nd4j.create(1, in1Size), Nd4j.create(1, in2Size) })
			net.summary(InputType.feedForward(in1Size), InputType.feedForward(in2Size))
			net.fit(New SingletonMultiDataSetIterator(mds))
		End Sub
	End Class

End Namespace