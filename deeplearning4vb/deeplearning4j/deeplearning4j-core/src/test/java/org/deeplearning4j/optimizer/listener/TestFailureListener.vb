Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports IrisDataSetIterator = org.deeplearning4j.datasets.iterator.impl.IrisDataSetIterator
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports FailureTestingListener = org.deeplearning4j.optimize.listeners.FailureTestingListener
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
import static org.junit.jupiter.api.Assertions.assertFalse
import static org.junit.jupiter.api.Assertions.assertNotNull

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

Namespace org.deeplearning4j.optimizer.listener


	''' <summary>
	''' WARNING: DO NOT ENABLE (UN-IGNORE) THESE TESTS.
	''' They should be run manually, not as part of standard unit test run.
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled @NativeTag @Tag(TagNames.DL4J_OLD_API) @Tag(TagNames.MANUAL) public class TestFailureListener extends org.deeplearning4j.BaseDL4JTest
	Public Class TestFailureListener
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled @Test public void testFailureIter5() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testFailureIter5()

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Adam(1e-4)).list().layer(0, (New OutputLayer.Builder()).nIn(4).nOut(3).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()

			net.setListeners(New FailureTestingListener(FailureTestingListener.FailureMode.SYSTEM_EXIT_1, New FailureTestingListener.IterationEpochTrigger(False, 10)))

			Dim iter As DataSetIterator = New IrisDataSetIterator(5,150)

			net.fit(iter)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled @Test public void testFailureRandom_OR()
		Public Overridable Sub testFailureRandom_OR()

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Adam(1e-4)).list().layer(0, (New OutputLayer.Builder()).nIn(4).nOut(3).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()

			Dim username As String = System.getProperty("user.name")
			assertNotNull(username)
			assertFalse(username.Length = 0)

			net.setListeners(New FailureTestingListener(FailureTestingListener.FailureMode.SYSTEM_EXIT_1, New FailureTestingListener.Or(New FailureTestingListener.IterationEpochTrigger(False, 10000), New FailureTestingListener.RandomProb(FailureTestingListener.CallType.ANY, 0.02))))

			Dim iter As DataSetIterator = New IrisDataSetIterator(5,150)

			net.fit(iter)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled @Test public void testFailureRandom_AND() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testFailureRandom_AND()

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Adam(1e-4)).list().layer(0, (New OutputLayer.Builder()).nIn(4).nOut(3).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()

			Dim hostname As String = InetAddress.getLocalHost().getHostName()
			assertNotNull(hostname)
			assertFalse(hostname.Length = 0)

			net.setListeners(New FailureTestingListener(FailureTestingListener.FailureMode.ILLEGAL_STATE, New FailureTestingListener.And(New FailureTestingListener.HostNameTrigger(hostname), New FailureTestingListener.RandomProb(FailureTestingListener.CallType.ANY, 0.05))))

			Dim iter As DataSetIterator = New IrisDataSetIterator(5,150)

			net.fit(iter)
		End Sub

	End Class

End Namespace