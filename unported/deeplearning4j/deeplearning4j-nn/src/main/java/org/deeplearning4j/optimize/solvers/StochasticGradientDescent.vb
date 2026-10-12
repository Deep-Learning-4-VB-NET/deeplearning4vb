Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Model = org.deeplearning4j.nn.api.Model
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports StepFunction = org.deeplearning4j.optimize.api.StepFunction
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives

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

Namespace org.deeplearning4j.optimize.solvers


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class StochasticGradientDescent extends BaseOptimizer
	<Serializable>
	Public Class StochasticGradientDescent
		Inherits BaseOptimizer


		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal stepFunction As StepFunction, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal model As Model)
			MyBase.New(conf, stepFunction, trainingListeners, model)
		End Sub


		Public Overrides Function optimize(ByVal workspaceMgr As LayerWorkspaceMgr) As Boolean
			If accumulator IsNot Nothing Then
				' before going FF, we're checking if there are any updates available
				If accumulator.hasAnything() Then
					log.info("Applying external updates before FF...")

					' we'll just fire off params update process
					accumulator.applyUpdate(stepFunction, model.params(), Nd4j.createUninitialized(model.params().shape(), model.params().ordering()), False)
				End If
			End If

			Dim pair As Pair(Of Gradient, Double) = gradientAndScore(workspaceMgr)

			Dim gradient As Gradient = pair.First

			Dim params As INDArray = model.params()

			' if optimizer has GradientsAccumulator defined - go for it
			If accumulator IsNot Nothing Then
				' we're propagating current update
				Dim epochNum As Integer = 0
				Dim iterationNum As Integer = 0

				If TypeOf model Is MultiLayerNetwork Then
					iterationNum = DirectCast(model, MultiLayerNetwork).IterationCount
					epochNum = DirectCast(model, MultiLayerNetwork).EpochCount
				ElseIf TypeOf model Is ComputationGraph Then
					iterationNum = DirectCast(model, ComputationGraph).IterationCount
					epochNum = DirectCast(model, ComputationGraph).EpochCount
				End If

				accumulator.storeUpdate(gradient.gradient(), iterationNum, epochNum)

				' and getting (possible) pending update from accumulator
				'INDArray pendingUpdate = accumulator.getUpdate();
				'stepFunction.step(params, pendingUpdate);
				accumulator.applyUpdate(stepFunction, params, gradient.gradient(), True)

				' if there's no update available - just go on then
			Else
				' if accumulator isn't used - we just to for direct updates application
				stepFunction.step(params, gradient.gradient())
			End If

			'Note: model.params() is always in-place for MultiLayerNetwork and ComputationGraph, hence no setParams is necessary there
			'However: for pretrain layers, params are NOT a view. Thus a setParams call is necessary
			'But setParams should be a no-op for MLN and CG
			model.Params = params

			Dim iterationCount As Integer = BaseOptimizer.getIterationCount(model)
			Dim epochCount As Integer = BaseOptimizer.getEpochCount(model)
			Using workspace As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
				For Each listener As TrainingListener In trainingListeners
					listener.iterationDone(model, iterationCount, epochCount)
				Next listener
			End Using

			BaseOptimizer.incrementIterationCount(model, 1)
			applyConstraints(model)
			Return True
		End Function

		Public Overrides Sub preProcessLine()
		End Sub
		Public Overrides Sub postStep(ByVal gradient As INDArray)
		End Sub
	End Class

End Namespace