Imports Builder = lombok.Builder
Imports Data = lombok.Data
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
'ORIGINAL LINE: @Data @SuperBuilder @NoArgsConstructor public class LearningConfiguration implements ILearningConfiguration
	Public Class LearningConfiguration
		Implements ILearningConfiguration

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private System.Nullable<Long> seed = System.currentTimeMillis();
		Private seed As Long? = DateTimeHelper.CurrentUnixTimeMillis()

		''' <summary>
		''' The maximum number of steps in each episode
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int maxEpochStep = 200;
		Private maxEpochStep As Integer = 200

		''' <summary>
		''' The maximum number of steps to train for
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int maxStep = 150000;
		Private maxStep As Integer = 150000

		''' <summary>
		''' Gamma parameter used for discounted rewards
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private double gamma = 0.99;
		Private gamma As Double = 0.99

		''' <summary>
		''' Scaling parameter for rewards
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private double rewardFactor = 1.0;
		Private rewardFactor As Double = 1.0

	End Class

End Namespace