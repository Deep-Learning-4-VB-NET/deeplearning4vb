Imports val = lombok.val
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports InferenceMode = org.deeplearning4j.parallelism.inference.InferenceMode
Imports LoadBalanceMode = org.deeplearning4j.parallelism.inference.LoadBalanceMode
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.junit.jupiter.api.Assertions

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

Namespace org.deeplearning4j.parallelism

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @NativeTag @Tag(TagNames.LARGE_RESOURCES) @Tag(TagNames.LONG_TEST) public class InplaceParallelInferenceTest extends org.deeplearning4j.BaseDL4JTest
	Public Class InplaceParallelInferenceTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testUpdateModel()
		Public Overridable Sub testUpdateModel()
			Dim nIn As Integer = 5

			Dim conf As val = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").layer("out0", (New OutputLayer.Builder()).nIn(nIn).nOut(4).activation(Activation.SOFTMAX).build(), "in").layer("out1", (New OutputLayer.Builder()).nIn(nIn).nOut(6).activation(Activation.SOFTMAX).build(), "in").setOutputs("out0", "out1").build()

			Dim net As val = New ComputationGraph(conf)
			net.init()

			Dim pi As val = (New ParallelInference.Builder(net)).inferenceMode(InferenceMode.INPLACE).workers(2).build()
			Try

				assertTrue(TypeOf pi Is InplaceParallelInference)

				Dim models As val = pi.getCurrentModelsFromWorkers()

				assertTrue(models.length > 0)

				For Each m As val In models
					assertNotNull(m)
					assertEquals(net.params(), m.params())
				Next m

				Dim conf2 As val = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").layer("out0", (New OutputLayer.Builder()).nIn(nIn).nOut(4).activation(Activation.SOFTMAX).build(), "in").layer("out1", (New OutputLayer.Builder()).nIn(nIn).nOut(6).activation(Activation.SOFTMAX).build(), "in").layer("out2", (New OutputLayer.Builder()).nIn(nIn).nOut(8).activation(Activation.SOFTMAX).build(), "in").setOutputs("out0", "out1", "out2").build()

				Dim net2 As val = New ComputationGraph(conf2)
				net2.init()

				assertNotEquals(net.params(), net2.params())

				pi.updateModel(net2)

				Dim models2 As val = pi.getCurrentModelsFromWorkers()

				assertTrue(models2.length > 0)

				For Each m As val In models2
					assertNotNull(m)
					assertEquals(net2.params(), m.params())
				Next m
			Finally
				pi.shutdown()
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testOutput_RoundRobin_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testOutput_RoundRobin_1()
			Dim nIn As Integer = 5

			Dim conf As val = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").layer("out0", (New OutputLayer.Builder()).nIn(nIn).nOut(4).activation(Activation.SOFTMAX).build(), "in").layer("out1", (New OutputLayer.Builder()).nIn(nIn).nOut(6).activation(Activation.SOFTMAX).build(), "in").setOutputs("out0", "out1").build()

			Dim net As val = New ComputationGraph(conf)
			net.init()

			Dim pi As val = (New ParallelInference.Builder(net)).inferenceMode(InferenceMode.INPLACE).loadBalanceMode(LoadBalanceMode.ROUND_ROBIN).workers(2).build()

			Try

				Dim result0 As val = pi.output(New INDArray(){Nd4j.create(New Double(){1.0, 2.0, 3.0, 4.0, 5.0}, New Long(){1, 5})}, Nothing)(0)
				Dim result1 As val = pi.output(New INDArray(){Nd4j.create(New Double(){1.0, 2.0, 3.0, 4.0, 5.0}, New Long(){1, 5})}, Nothing)(0)

				assertNotNull(result0)
				assertEquals(result0, result1)
			Finally
				pi.shutdown()
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testOutput_FIFO_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testOutput_FIFO_1()
			Dim nIn As Integer = 5

			Dim conf As val = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").layer("out0", (New OutputLayer.Builder()).nIn(nIn).nOut(4).activation(Activation.SOFTMAX).build(), "in").layer("out1", (New OutputLayer.Builder()).nIn(nIn).nOut(6).activation(Activation.SOFTMAX).build(), "in").setOutputs("out0", "out1").build()

			Dim net As val = New ComputationGraph(conf)
			net.init()

			Dim pi As val = (New ParallelInference.Builder(net)).inferenceMode(InferenceMode.INPLACE).loadBalanceMode(LoadBalanceMode.FIFO).workers(2).build()

			Try

				Dim result0 As val = pi.output(New INDArray(){Nd4j.create(New Double(){1.0, 2.0, 3.0, 4.0, 5.0}, New Long(){1, 5})}, Nothing)(0)
				Dim result1 As val = pi.output(New INDArray(){Nd4j.create(New Double(){1.0, 2.0, 3.0, 4.0, 5.0}, New Long(){1, 5})}, Nothing)(0)

				assertNotNull(result0)
				assertEquals(result0, result1)
			Finally
				pi.shutdown()
			End Try
		End Sub
	End Class
End Namespace