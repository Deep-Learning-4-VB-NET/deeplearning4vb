Imports DQNDenseNetworkConfiguration = org.deeplearning4j.rl4j.network.configuration.DQNDenseNetworkConfiguration
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Test = org.junit.jupiter.api.Test
Imports RmsProp = org.nd4j.linalg.learning.config.RmsProp
import static org.junit.jupiter.api.Assertions.assertEquals

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

Namespace org.deeplearning4j.rl4j.network.dqn


	''' <summary>
	''' @author saudet
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled("File permissions") public class DQNTest
	Public Class DQNTest

		Private Shared NET_CONF As DQNDenseNetworkConfiguration = DQNDenseNetworkConfiguration.builder().numLayers(3).numHiddenNodes(16).l2(0.001).updater(New RmsProp(0.0005)).build()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testModelLoadSave() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testModelLoadSave()
			Dim dqn As DQN = (New DQNFactoryStdDense(NET_CONF)).buildDQN(New Integer(){42}, 13)

			Dim file As File = File.createTempFile("rl4j-dqn-", ".model")
			dqn.save(file.getAbsolutePath())

			Dim dqn2 As DQN = DQN.load(file.getAbsolutePath())

			assertEquals(dqn.mln, dqn2.mln)
		End Sub
	End Class

End Namespace