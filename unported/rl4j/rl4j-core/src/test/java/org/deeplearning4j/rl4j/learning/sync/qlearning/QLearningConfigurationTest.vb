Imports ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper
Imports QLearningConfiguration = org.deeplearning4j.rl4j.learning.configuration.QLearningConfiguration
Imports Test = org.junit.jupiter.api.Test

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

Namespace org.deeplearning4j.rl4j.learning.sync.qlearning

	Public Class QLearningConfigurationTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void serialize() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub serialize()
			Dim mapper As New ObjectMapper()

			Dim qLearningConfiguration As QLearningConfiguration = QLearningConfiguration.builder().build()

			' Should not throw..
			Dim json As String = mapper.writeValueAsString(qLearningConfiguration)
			Dim cnf As QLearningConfiguration = mapper.readValue(json, GetType(QLearningConfiguration))
		End Sub
	End Class

End Namespace