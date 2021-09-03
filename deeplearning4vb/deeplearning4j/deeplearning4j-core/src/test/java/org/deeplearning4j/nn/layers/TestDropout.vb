Imports System.Collections.Generic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.fail

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
'ORIGINAL LINE: @NativeTag @Tag(TagNames.DL4J_OLD_API) @Tag(TagNames.FILE_IO) @Tag(TagNames.RNG) public class TestDropout extends org.deeplearning4j.BaseDL4JTest
	Public Class TestDropout
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDropoutSimple() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testDropoutSimple()
			'Testing dropout with a single layer
			'Layer input: values should be set to either 0.0 or 2.0x original value

			Dim nIn As Integer = 8
			Dim nOut As Integer = 8

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Sgd()).dropOut(0.5).list().layer(0, (New OutputLayer.Builder()).activation(Activation.IDENTITY).lossFunction(LossFunctions.LossFunction.MSE).nIn(nIn).nOut(nOut).weightInit(WeightInit.XAVIER).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			net.getLayer(0).getParam("W").assign(Nd4j.eye(nIn))

			Dim nTests As Integer = 15

			Nd4j.Random.setSeed(12345)
			Dim noDropoutCount As Integer = 0
			For i As Integer = 0 To nTests - 1
				Dim [in] As INDArray = Nd4j.rand(1, nIn)
				Dim [out] As INDArray = Nd4j.rand(1, nOut)
				Dim inCopy As INDArray = [in].dup()

				Dim l As IList(Of INDArray) = net.feedForward([in], True)

				Dim postDropout As INDArray = l(l.Count - 1)
				'Dropout occurred. Expect inputs to be either scaled 2x original, or set to 0.0 (with dropout = 0.5)
				For j As Integer = 0 To inCopy.length() - 1
					Dim origValue As Double = inCopy.getDouble(j)
					Dim doValue As Double = postDropout.getDouble(j)
					If doValue > 0.0 Then
						'Input was kept -> should be scaled by factor of (1.0/0.5 = 2)
						assertEquals(origValue * 2.0, doValue, 0.0001)
					End If
				Next j

				'Do forward pass
				'(1) ensure dropout ISN'T being applied for forward pass at test time
				'(2) ensure dropout ISN'T being applied for test time scoring
				'If dropout is applied at test time: outputs + score will differ between passes
				Dim in2 As INDArray = Nd4j.rand(1, nIn)
				Dim out2 As INDArray = Nd4j.rand(1, nOut)
				Dim outTest1 As INDArray = net.output(in2, False)
				Dim outTest2 As INDArray = net.output(in2, False)
				Dim outTest3 As INDArray = net.output(in2, False)
				assertEquals(outTest1, outTest2)
				assertEquals(outTest1, outTest3)

				Dim score1 As Double = net.score(New DataSet(in2, out2), False)
				Dim score2 As Double = net.score(New DataSet(in2, out2), False)
				Dim score3 As Double = net.score(New DataSet(in2, out2), False)
				assertEquals(score1, score2, 0.0)
				assertEquals(score1, score3, 0.0)
			Next i

			If noDropoutCount >= nTests \ 3 Then
				'at 0.5 dropout ratio and more than a few inputs, expect only a very small number of instances where
				'no dropout occurs, just due to random chance
				fail("Too many instances of dropout not being applied")
			End If
		End Sub
	End Class

End Namespace