﻿Imports Model = org.deeplearning4j.nn.api.Model
Imports WorkspaceMode = org.deeplearning4j.nn.conf.WorkspaceMode
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports ParallelWrapper = org.deeplearning4j.parallelism.ParallelWrapper
Imports DefaultTrainer = org.deeplearning4j.parallelism.trainer.DefaultTrainer
Imports Trainer = org.deeplearning4j.parallelism.trainer.Trainer

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

Namespace org.deeplearning4j.parallelism.factory

	Public Class DefaultTrainerContext
		Implements TrainerContext

		''' <summary>
		''' Initialize the context
		''' </summary>
		''' <param name="model"> </param>
		''' <param name="args"> the arguments to initialize with (maybe null) </param>
		Public Overridable Sub init(ByVal model As Model, ParamArray ByVal args() As Object) Implements TrainerContext.init

		End Sub

		''' <summary>
		''' Create a <seealso cref="Trainer"/>
		''' based on the given parameters
		''' </summary>
		''' <param name="threadId">   the thread id to use for this worker </param>
		''' <param name="model">      the model to start the trainer with </param>
		''' <param name="rootDevice"> the root device id </param>
		''' <param name="useMDS">     whether to use MultiDataSet or DataSet
		'''                   or not </param>
		''' <param name="wrapper">    the wrapper instance to use with this trainer (this refernece is needed
		'''                   for coordination with the <seealso cref="ParallelWrapper"/> 's <seealso cref="TrainingListener"/> </param>
		''' <returns> the created training instance </returns>
		Public Overridable Function create(ByVal uuid As String, ByVal threadId As Integer, ByVal model As Model, ByVal rootDevice As Integer, ByVal useMDS As Boolean, ByVal wrapper As ParallelWrapper, ByVal mode As WorkspaceMode, ByVal averagingFrequency As Integer) As Trainer Implements TrainerContext.create

			Dim trainer As DefaultTrainer = DefaultTrainer.builder().originalModel(model).replicatedModel(model).threadId(threadId).parallelWrapper(wrapper).workspaceMode(mode).useMDS(useMDS).uuid(uuid & "_thread_" & threadId).averagingFrequency(averagingFrequency).build()

			trainer.setName("DefaultTrainer thread " & threadId)
			trainer.setDaemon(True)

			Return trainer
		End Function

		Public Overridable Sub finalizeRound(ByVal originalModel As Model, ParamArray ByVal models() As Model) Implements TrainerContext.finalizeRound
			' apply averaging

			' TODO: move averaging here
		End Sub

		Public Overridable Sub finalizeTraining(ByVal originalModel As Model, ParamArray ByVal models() As Model) Implements TrainerContext.finalizeTraining
			finalizeRound(originalModel, models)
		End Sub
	End Class

End Namespace