Imports System
Imports ActorCriticDenseNetworkConfiguration = org.deeplearning4j.rl4j.network.configuration.ActorCriticDenseNetworkConfiguration
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Test = org.junit.jupiter.api.Test
Imports ActivationSoftmax = org.nd4j.linalg.activations.impl.ActivationSoftmax
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports RmsProp = org.nd4j.linalg.learning.config.RmsProp
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue

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

Namespace org.deeplearning4j.rl4j.network.ac


	''' <summary>
	''' @author saudet
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled("File permissions on CI") public class ActorCriticTest
	Public Class ActorCriticTest

		Public Shared NET_CONF As ActorCriticDenseNetworkConfiguration = ActorCriticDenseNetworkConfiguration.builder().numLayers(4).numHiddenNodes(32).l2(0.001).updater(New RmsProp(0.0005)).useLSTM(False).build()

		Public Shared NET_CONF_CG As ActorCriticDenseNetworkConfiguration = ActorCriticDenseNetworkConfiguration.builder().numLayers(2).numHiddenNodes(128).l2(0.00001).updater(New RmsProp(0.005)).useLSTM(True).build()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testModelLoadSave() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testModelLoadSave()
			Dim acs As ActorCriticSeparate = (New ActorCriticFactorySeparateStdDense(NET_CONF)).buildActorCritic(New Integer(){7}, 5)

			Dim fileValue As File = File.createTempFile("rl4j-value-", ".model")
			Dim filePolicy As File = File.createTempFile("rl4j-policy-", ".model")
			acs.save(fileValue.getAbsolutePath(), filePolicy.getAbsolutePath())

			Dim acs2 As ActorCriticSeparate = ActorCriticSeparate.load(fileValue.getAbsolutePath(), filePolicy.getAbsolutePath())

			assertEquals(acs.valueNet, acs2.valueNet)
			assertEquals(acs.policyNet, acs2.policyNet)

			Dim accg As ActorCriticCompGraph = (New ActorCriticFactoryCompGraphStdDense(NET_CONF_CG)).buildActorCritic(New Integer(){37}, 43)

			Dim file As File = File.createTempFile("rl4j-cg-", ".model")
			accg.save(file.getAbsolutePath())

			Dim accg2 As ActorCriticCompGraph = ActorCriticCompGraph.load(file.getAbsolutePath())

			assertEquals(accg.cg, accg2.cg)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testLoss()
		Public Overridable Sub testLoss()
			Dim activation As New ActivationSoftmax()
			Dim loss As New ActorCriticLoss()
			Dim n As Double = 10
			Dim eps As Double = 1e-5
			Dim maxRelError As Double = 1e-3

			For i As Double = eps To n - 1
				For j As Double = eps To n - 1
					Dim labels As INDArray = Nd4j.create(New Double(){i / n, 1 - i / n}, New Long(){1, 2})
					Dim output As INDArray = Nd4j.create(New Double(){j / n, 1 - j / n}, New Long(){1, 2})
					Dim gradient As INDArray = loss.computeGradient(labels, output, activation, Nothing)

					output = Nd4j.create(New Double(){j / n, 1 - j / n}, New Long(){1, 2})
					Dim score As Double = loss.computeScore(labels, output, activation, Nothing, False)
					Dim output1 As INDArray = Nd4j.create(New Double(){j / n + eps, 1 - j / n}, New Long(){1, 2})
					Dim score1 As Double = loss.computeScore(labels, output1, activation, Nothing, False)
					Dim output2 As INDArray = Nd4j.create(New Double(){j / n, 1 - j / n + eps}, New Long(){1, 2})
					Dim score2 As Double = loss.computeScore(labels, output2, activation, Nothing, False)

					Dim gradient1 As Double = (score1 - score) / eps
					Dim gradient2 As Double = (score2 - score) / eps
					Dim error1 As Double = gradient1 - gradient.getDouble(0)
					Dim error2 As Double = gradient2 - gradient.getDouble(1)
					Dim relError1 As Double = error1 / gradient.getDouble(0)
					Dim relError2 As Double = error2 / gradient.getDouble(1)
	'                System.out.println(gradient.getDouble(0) + "  " + gradient1 + " " + relError1);
	'                System.out.println(gradient.getDouble(1) + "  " + gradient2 + " " + relError2);
					assertTrue(gradient.getDouble(0) < maxRelError OrElse Math.Abs(relError1) < maxRelError)
					assertTrue(gradient.getDouble(1) < maxRelError OrElse Math.Abs(relError2) < maxRelError)
				Next j
			Next i
		End Sub
	End Class

End Namespace