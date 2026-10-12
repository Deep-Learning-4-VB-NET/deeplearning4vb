Imports System.Collections.Generic
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports Singular = lombok.Singular
Imports SuperBuilder = lombok.experimental.SuperBuilder
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports IUpdater = org.nd4j.linalg.learning.config.IUpdater

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

Namespace org.deeplearning4j.rl4j.network.configuration



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @SuperBuilder @NoArgsConstructor public class NetworkConfiguration
	Public Class NetworkConfiguration
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private double learningRate = 0.01;
		Private learningRate As Double = 0.01

		''' <summary>
		''' L2 regularization on the network
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private double l2 = 0.0;
		Private l2 As Double = 0.0

		''' <summary>
		''' The network's gradient update algorithm
		''' </summary>
		Private updater As IUpdater

		''' <summary>
		''' Training listeners attached to the network
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Singular private java.util.List<org.deeplearning4j.optimize.api.TrainingListener> listeners;
		Private listeners As IList(Of TrainingListener)

	End Class

End Namespace