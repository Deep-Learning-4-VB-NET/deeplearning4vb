Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports SuperBuilder = lombok.experimental.SuperBuilder

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

Namespace org.deeplearning4j.rl4j.learning.configuration

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @SuperBuilder @NoArgsConstructor @EqualsAndHashCode(callSuper = false) public class QLearningConfiguration extends LearningConfiguration
	Public Class QLearningConfiguration
		Inherits LearningConfiguration

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int expRepMaxSize = 150000;
		Private expRepMaxSize As Integer = 150000

		''' <summary>
		''' The batch size of experience for each training iteration
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int batchSize = 32;
		Private batchSize As Integer = 32

		''' <summary>
		''' How many steps between target network updates
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int targetDqnUpdateFreq = 100;
		Private targetDqnUpdateFreq As Integer = 100

		''' <summary>
		''' The number of steps to initially wait for until samplling batches from experience replay buffer
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int updateStart = 10;
		Private updateStart As Integer = 10

		''' <summary>
		''' Prevent the new Q-Value from being farther than <i>errorClamp</i> away from the previous value. Double.NaN will result in no clamping
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private double errorClamp = 1.0;
		Private errorClamp As Double = 1.0

		''' <summary>
		''' The minimum probability for random exploration action during episilon-greedy annealing
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private double minEpsilon = 0.1f;
		Private minEpsilon As Double = 0.1f

		''' <summary>
		''' The number of steps to anneal epsilon to its minimum value.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int epsilonNbStep = 10000;
		Private epsilonNbStep As Integer = 10000

		''' <summary>
		''' Whether to use the double DQN algorithm
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private boolean doubleDQN = false;
		Private doubleDQN As Boolean = False

	End Class

End Namespace