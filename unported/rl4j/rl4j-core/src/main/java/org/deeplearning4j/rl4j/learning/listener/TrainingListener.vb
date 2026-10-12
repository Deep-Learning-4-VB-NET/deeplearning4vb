Imports IEpochTrainer = org.deeplearning4j.rl4j.learning.IEpochTrainer
Imports org.deeplearning4j.rl4j.learning
Imports IDataManager = org.deeplearning4j.rl4j.util.IDataManager

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
Namespace org.deeplearning4j.rl4j.learning.listener

	''' <summary>
	''' The base definition of all training event listeners
	''' 
	''' @author Alexandre Boulanger
	''' </summary>
	Public Interface TrainingListener
		Friend Enum ListenerResponse
			''' <summary>
			''' Tell the learning process to continue calling the listeners and the training.
			''' </summary>
			[CONTINUE]

			''' <summary>
			''' Tell the learning process to stop calling the listeners and terminate the training.
			''' </summary>
			[STOP]
		End Enum

		''' <summary>
		''' Called once when the training starts. </summary>
		''' <returns> A ListenerResponse telling the source of the event if it should go on or cancel the training. </returns>
		Function onTrainingStart() As ListenerResponse

		''' <summary>
		''' Called once when the training has finished. This method is called even when the training has been aborted.
		''' </summary>
		Sub onTrainingEnd()

		''' <summary>
		''' Called before the start of every epoch. </summary>
		''' <param name="trainer"> A <seealso cref="IEpochTrainer"/> </param>
		''' <returns> A ListenerResponse telling the source of the event if it should continue or stop the training. </returns>
		Function onNewEpoch(ByVal trainer As IEpochTrainer) As ListenerResponse

		''' <summary>
		''' Called when an epoch has been completed </summary>
		''' <param name="trainer"> A <seealso cref="IEpochTrainer"/> </param>
		''' <param name="statEntry"> A <seealso cref="org.deeplearning4j.rl4j.util.IDataManager.StatEntry"/> </param>
		''' <returns> A ListenerResponse telling the source of the event if it should continue or stop the training. </returns>
		Function onEpochTrainingResult(ByVal trainer As IEpochTrainer, ByVal statEntry As IDataManager.StatEntry) As ListenerResponse

		''' <summary>
		''' Called regularly to monitor the training progress. </summary>
		''' <param name="learning"> A <seealso cref="ILearning"/> </param>
		''' <returns> A ListenerResponse telling the source of the event if it should continue or stop the training. </returns>
		Function onTrainingProgress(ByVal learning As ILearning) As ListenerResponse
	End Interface

End Namespace