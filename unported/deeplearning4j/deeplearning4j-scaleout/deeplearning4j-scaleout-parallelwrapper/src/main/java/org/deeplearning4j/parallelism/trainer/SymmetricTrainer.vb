Imports System
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Model = org.deeplearning4j.nn.api.Model
Imports WorkspaceMode = org.deeplearning4j.nn.conf.WorkspaceMode
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports SharedGradient = org.deeplearning4j.optimize.listeners.SharedGradient
Imports GradientsAccumulator = org.deeplearning4j.optimize.solvers.accumulation.GradientsAccumulator
Imports ParallelWrapper = org.deeplearning4j.parallelism.ParallelWrapper

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

Namespace org.deeplearning4j.parallelism.trainer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class SymmetricTrainer extends DefaultTrainer implements CommunicativeTrainer
	Public Class SymmetricTrainer
		Inherits DefaultTrainer
		Implements CommunicativeTrainer

		Protected Friend accumulator As GradientsAccumulator

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SymmetricTrainer(@NonNull Model originalModel, String uuid, int threadIdx, @NonNull WorkspaceMode mode, @NonNull ParallelWrapper wrapper, boolean useMDS)
		Public Sub New(ByVal originalModel As Model, ByVal uuid As String, ByVal threadIdx As Integer, ByVal mode As WorkspaceMode, ByVal wrapper As ParallelWrapper, ByVal useMDS As Boolean)
			MyBase.New()
			Me.uuid = uuid & "_thread_" & threadIdx
			Me.useMDS = useMDS
			Me.originalModel = originalModel
			Me.threadId = threadIdx
			Me.workspaceMode = mode
			Me.parallelWrapper = wrapper
			Me.accumulator = wrapper.getGradientsAccumulator()

		End Sub

		' FIXME: delete this method, it's not needed anymore
		<Obsolete>
		Public Overridable Sub enqueueGradient(ByVal gradient As SharedGradient) Implements CommunicativeTrainer.enqueueGradient
			'
		End Sub


		Public Overrides Function averagingRequired() As Boolean Implements org.deeplearning4j.parallelism.trainer.Trainer.averagingRequired
			Return False
		End Function

		Protected Friend Overrides Sub postInit()
			MyBase.postInit()

			If accumulator Is Nothing Then
				log.warn("GradientsAccumulator is undefined, gradients sharing will be skipped")
				Return
			End If

			' just pass accumulator down the hill
			If TypeOf replicatedModel Is ComputationGraph Then
				DirectCast(replicatedModel, ComputationGraph).GradientsAccumulator = accumulator
			ElseIf TypeOf replicatedModel Is MultiLayerNetwork Then
				DirectCast(replicatedModel, MultiLayerNetwork).GradientsAccumulator = accumulator
			End If

			' need to attach this device id to accumulator's workspaces
			accumulator.touch()
		End Sub



	End Class

End Namespace