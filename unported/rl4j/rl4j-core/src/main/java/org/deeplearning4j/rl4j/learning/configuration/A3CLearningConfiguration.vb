Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
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
'ORIGINAL LINE: @Data @SuperBuilder @EqualsAndHashCode(callSuper = true) public class A3CLearningConfiguration extends LearningConfiguration implements IAsyncLearningConfiguration
	Public Class A3CLearningConfiguration
		Inherits LearningConfiguration
		Implements IAsyncLearningConfiguration

		''' <summary>
		''' The number of asynchronous threads to use to generate gradients
		''' </summary>
		Private ReadOnly numThreads As Integer

		''' <summary>
		''' The number of steps to calculate gradients over
		''' </summary>
		Private ReadOnly nStep As Integer

		''' <summary>
		''' The frequency of async training iterations to update the target network.
		''' 
		''' If this is set to -1 then the target network is updated after every training iteration
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int learnerUpdateFrequency = -1;
		Private learnerUpdateFrequency As Integer = -1
	End Class

End Namespace