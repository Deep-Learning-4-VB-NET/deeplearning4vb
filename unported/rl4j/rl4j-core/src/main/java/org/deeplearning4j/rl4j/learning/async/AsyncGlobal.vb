Imports Getter = lombok.Getter
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports IAsyncLearningConfiguration = org.deeplearning4j.rl4j.learning.configuration.IAsyncLearningConfiguration
Imports org.deeplearning4j.rl4j.network

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

Namespace org.deeplearning4j.rl4j.learning.async


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class AsyncGlobal<NN extends org.deeplearning4j.rl4j.network.NeuralNet> implements IAsyncGlobal<NN>
	Public Class AsyncGlobal(Of NN As org.deeplearning4j.rl4j.network.NeuralNet)
		Implements IAsyncGlobal(Of NN)

		Private ReadOnly current As NN

'JAVA TO VB CONVERTER NOTE: The field target was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private target_Conflict As NN

		Private ReadOnly configuration As IAsyncLearningConfiguration

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final java.util.concurrent.locks.Lock updateLock;
		Private ReadOnly updateLock As Lock

		''' <summary>
		''' The number of times the gradient has been updated by worker threads
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private int workerUpdateCount;
		Private workerUpdateCount As Integer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private int stepCount;
		Private stepCount As Integer

		Public Sub New(ByVal initial As NN, ByVal configuration As IAsyncLearningConfiguration)
			Me.current = initial
			target_Conflict = CType(initial.clone(), NN)
			Me.configuration = configuration

			' This is used to sync between
			updateLock = New ReentrantLock()
		End Sub

		Public Overridable ReadOnly Property TrainingComplete As Boolean Implements IAsyncGlobal(Of NN).isTrainingComplete
			Get
				Return stepCount >= configuration.MaxStep
			End Get
		End Property

		Public Overridable Sub applyGradient(ByVal gradient() As Gradient, ByVal nstep As Integer) Implements IAsyncGlobal(Of NN).applyGradient

			If TrainingComplete Then
				Return
			End If

			Try
				updateLock.lock()

				current.applyGradient(gradient, nstep)

				stepCount += nstep
				workerUpdateCount += 1

				Dim targetUpdateFrequency As Integer = configuration.LearnerUpdateFrequency

				' If we have a target update frequency, this means we only want to update the workers after a certain number of async updates
				' This can lead to more stable training
				If targetUpdateFrequency <> -1 AndAlso workerUpdateCount Mod targetUpdateFrequency = 0 Then
					log.info("Updating target network at updates={} steps={}", workerUpdateCount, stepCount)
				Else
					target_Conflict.copyFrom(current)
				End If
			Finally
				updateLock.unlock()
			End Try

		End Sub

		Public Overridable ReadOnly Property Target As NN Implements IAsyncGlobal(Of NN).getTarget
			Get
				Try
					updateLock.lock()
					Return target_Conflict
				Finally
					updateLock.unlock()
				End Try
			End Get
		End Property

	End Class

End Namespace