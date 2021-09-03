Imports System.Collections.Generic
Imports Model = org.deeplearning4j.nn.api.Model
Imports Updater = org.deeplearning4j.nn.api.Updater
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports ComputationGraphUpdater = org.deeplearning4j.nn.updater.graph.ComputationGraphUpdater
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports GradientsAccumulator = org.deeplearning4j.optimize.solvers.accumulation.GradientsAccumulator
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
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

Namespace org.deeplearning4j.optimize.api


	Public Interface ConvexOptimizer
		''' <summary>
		''' The score for the optimizer so far </summary>
		''' <returns> the score for this optimizer so far </returns>
		Function score() As Double

		Property Updater As Updater

		Function getUpdater(ByVal initializeIfReq As Boolean) As Updater

		ReadOnly Property ComputationGraphUpdater As ComputationGraphUpdater

		Function getComputationGraphUpdater(ByVal initializeIfReq As Boolean) As ComputationGraphUpdater


		WriteOnly Property UpdaterComputationGraph As ComputationGraphUpdater

		WriteOnly Property Listeners As ICollection(Of TrainingListener)

		''' <summary>
		''' This method specifies GradientsAccumulator instance to be used for updates sharing across multiple models
		''' </summary>
		''' <param name="accumulator"> </param>
		Property GradientsAccumulator As GradientsAccumulator

		''' <summary>
		''' This method returns StepFunction defined within this Optimizer instance
		''' @return
		''' </summary>
		ReadOnly Property StepFunction As StepFunction


		ReadOnly Property Conf As NeuralNetConfiguration

		''' <summary>
		''' The gradient and score for this optimizer </summary>
		''' <returns> the gradient and score for this optimizer </returns>
		Function gradientAndScore(ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, Double)

		''' <summary>
		''' Calls optimize </summary>
		''' <returns> whether the convex optimizer
		''' converted or not </returns>
		Function optimize(ByVal workspaceMgr As LayerWorkspaceMgr) As Boolean


		''' <summary>
		''' The batch size for the optimizer
		''' @return
		''' </summary>
		Function batchSize() As Integer

		''' <summary>
		''' Set the batch size for the optimizer </summary>
		''' <param name="batchSize"> </param>
		WriteOnly Property BatchSize As Integer

		''' <summary>
		''' Pre preProcess a line before an iteration
		''' </summary>
		Sub preProcessLine()

		''' <summary>
		''' After the step has been made, do an action </summary>
		''' <param name="line">
		'''  </param>
		Sub postStep(ByVal line As INDArray)

		''' <summary>
		''' Based on the gradient and score
		''' setup a search state </summary>
		''' <param name="pair"> the gradient and score </param>
		Sub setupSearchState(ByVal pair As Pair(Of Gradient, Double))

		''' <summary>
		''' Update the gradient according to the configuration such as adagrad, momentum, and sparsity </summary>
		''' <param name="gradient"> the gradient to modify </param>
		''' <param name="model"> the model with the parameters to update </param>
		''' <param name="batchSize"> batchSize for update
		''' @paramType paramType to update </param>
		Sub updateGradientAccordingToParams(ByVal gradient As Gradient, ByVal model As Model, ByVal batchSize As Integer, ByVal workspaceMgr As LayerWorkspaceMgr)

	End Interface

End Namespace