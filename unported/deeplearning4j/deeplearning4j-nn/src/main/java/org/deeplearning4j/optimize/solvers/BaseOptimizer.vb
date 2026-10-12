Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports InvalidStepException = org.deeplearning4j.exception.InvalidStepException
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports Model = org.deeplearning4j.nn.api.Model
Imports Updater = org.deeplearning4j.nn.api.Updater
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports UpdaterCreator = org.deeplearning4j.nn.updater.UpdaterCreator
Imports ComputationGraphUpdater = org.deeplearning4j.nn.updater.graph.ComputationGraphUpdater
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports ConvexOptimizer = org.deeplearning4j.optimize.api.ConvexOptimizer
Imports StepFunction = org.deeplearning4j.optimize.api.StepFunction
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports GradientsAccumulator = org.deeplearning4j.optimize.solvers.accumulation.GradientsAccumulator
Imports NegativeDefaultStepFunction = org.deeplearning4j.optimize.stepfunctions.NegativeDefaultStepFunction
Imports NegativeGradientStepFunction = org.deeplearning4j.optimize.stepfunctions.NegativeGradientStepFunction
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory

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


	''' <summary>
	''' Base optimizer
	''' @author Adam Gibson
	''' </summary>
	<Serializable>
	Public MustInherit Class BaseOptimizer
		Implements ConvexOptimizer

		Public MustOverride ReadOnly Property StepFunction As StepFunction
		Public MustOverride WriteOnly Property Listeners As ICollection(Of TrainingListener)

'JAVA TO VB CONVERTER NOTE: The field conf was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend conf_Conflict As NeuralNetConfiguration
		Protected Friend Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(BaseOptimizer))
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected org.deeplearning4j.optimize.api.StepFunction stepFunction;
		Protected Friend stepFunction As StepFunction
		Protected Friend trainingListeners As ICollection(Of TrainingListener) = New List(Of TrainingListener)()
		Protected Friend model As Model
		Protected Friend lineMaximizer As BackTrackLineSearch
'JAVA TO VB CONVERTER NOTE: The field updater was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend updater_Conflict As Updater
'JAVA TO VB CONVERTER NOTE: The field computationGraphUpdater was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend computationGraphUpdater_Conflict As ComputationGraphUpdater
		Protected Friend [step] As Double
'JAVA TO VB CONVERTER NOTE: The field batchSize was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private batchSize_Conflict As Integer
'JAVA TO VB CONVERTER NOTE: The field score was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend score_Conflict, oldScore As Double
		Protected Friend stepMax As Double = Double.MaxValue
		Public Const GRADIENT_KEY As String = "g"
		Public Const SCORE_KEY As String = "score"
		Public Const PARAMS_KEY As String = "params"
		Public Const SEARCH_DIR As String = "searchDirection"
		Protected Friend searchState As IDictionary(Of String, Object) = New ConcurrentDictionary(Of String, Object)()


		Protected Friend accumulator As GradientsAccumulator


		''' 
		''' <param name="conf"> </param>
		''' <param name="stepFunction"> </param>
		''' <param name="trainingListeners"> </param>
		''' <param name="model"> </param>
		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal stepFunction As StepFunction, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal model As Model)
			Me.conf_Conflict = conf
			Me.stepFunction = (If(stepFunction IsNot Nothing, stepFunction, getDefaultStepFunctionForOptimizer(Me.GetType())))
			Me.trainingListeners = If(trainingListeners IsNot Nothing, trainingListeners, New List(Of TrainingListener)())
			Me.model = model
			lineMaximizer = New BackTrackLineSearch(model, Me.stepFunction, Me)
			lineMaximizer.StepMax = stepMax
			lineMaximizer.MaxIterations = conf.getMaxNumLineSearchIterations()
		End Sub

		Public Overridable Property GradientsAccumulator Implements ConvexOptimizer.setGradientsAccumulator As GradientsAccumulator
			Set(ByVal accumulator As GradientsAccumulator)
				Me.accumulator = accumulator
			End Set
			Get
				Return accumulator
			End Get
		End Property


		Public Overridable Function score() As Double Implements ConvexOptimizer.score
	'        model.computeGradientAndScore();
	'        return model.score();
			Throw New System.NotSupportedException("Not yet reimplemented")
		End Function

		Public Overridable Property Updater As Updater Implements ConvexOptimizer.getUpdater
			Get
				Return getUpdater(True)
			End Get
			Set(ByVal updater As Updater)
				Me.updater_Conflict = updater
			End Set
		End Property

		Public Overridable Function getUpdater(ByVal initializeIfReq As Boolean) As Updater Implements ConvexOptimizer.getUpdater
			If updater_Conflict Is Nothing AndAlso initializeIfReq Then
				updater_Conflict = UpdaterCreator.getUpdater(model)
			End If
			Return updater_Conflict
		End Function




		Public Overridable ReadOnly Property ComputationGraphUpdater As ComputationGraphUpdater Implements ConvexOptimizer.getComputationGraphUpdater
			Get
				Return getComputationGraphUpdater(True)
			End Get
		End Property

		Public Overridable Function getComputationGraphUpdater(ByVal initializIfReq As Boolean) As ComputationGraphUpdater Implements ConvexOptimizer.getComputationGraphUpdater
			If computationGraphUpdater_Conflict Is Nothing AndAlso TypeOf model Is ComputationGraph AndAlso initializIfReq Then
				computationGraphUpdater_Conflict = New ComputationGraphUpdater(DirectCast(model, ComputationGraph))
			End If
			Return computationGraphUpdater_Conflict
		End Function

		Public Overridable WriteOnly Property UpdaterComputationGraph Implements ConvexOptimizer.setUpdaterComputationGraph As ComputationGraphUpdater
			Set(ByVal updater As ComputationGraphUpdater)
				Me.computationGraphUpdater_Conflict = updater
			End Set
		End Property

		Public Overridable WriteOnly Property Listeners Implements ConvexOptimizer.setListeners As ICollection(Of TrainingListener)
			Set(ByVal listeners As ICollection(Of TrainingListener))
				If listeners Is Nothing Then
					Me.trainingListeners = Collections.emptyList()
				Else
					Me.trainingListeners = listeners
				End If
			End Set
		End Property

		Public Overridable ReadOnly Property Conf As NeuralNetConfiguration Implements ConvexOptimizer.getConf
			Get
				Return conf_Conflict
			End Get
		End Property

		Public Overridable Function gradientAndScore(ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, Double) Implements ConvexOptimizer.gradientAndScore
			oldScore = score_Conflict
			model.computeGradientAndScore(workspaceMgr)

			If trainingListeners IsNot Nothing AndAlso trainingListeners.Count > 0 Then
				Using workspace As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
					For Each l As TrainingListener In trainingListeners
						l.onGradientCalculation(model)
					Next l
				End Using
			End If

			Dim pair As Pair(Of Gradient, Double) = model.gradientAndScore()
			score_Conflict = pair.Second
			updateGradientAccordingToParams(pair.First, model, model.batchSize(), workspaceMgr)
			Return pair
		End Function

		''' <summary>
		''' Optimize call. This runs the optimizer. </summary>
		''' <returns> whether it converged or not </returns>
		' TODO add flag to allow retaining state between mini batches and when to apply updates
		Public Overridable Function optimize(ByVal workspaceMgr As LayerWorkspaceMgr) As Boolean Implements ConvexOptimizer.optimize
			'validate the input before training
			Dim gradient As INDArray
			Dim searchDirection As INDArray
			Dim parameters As INDArray
			Dim pair As Pair(Of Gradient, Double) = gradientAndScore(workspaceMgr)
			If searchState.Count = 0 Then
				searchState(GRADIENT_KEY) = pair.First.gradient()
				Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.scopeOutOfWorkspaces()
					setupSearchState(pair) 'Only do this once
				End Using
			Else
				searchState(GRADIENT_KEY) = pair.First.gradient()
			End If

			'calculate initial search direction
			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.scopeOutOfWorkspaces()
				preProcessLine()
			End Using

			gradient = DirectCast(searchState(GRADIENT_KEY), INDArray)
			searchDirection = DirectCast(searchState(SEARCH_DIR), INDArray)
			parameters = DirectCast(searchState(PARAMS_KEY), INDArray)

			'perform one line search optimization
			Try
				[step] = lineMaximizer.optimize(parameters, gradient, searchDirection, workspaceMgr)
			Catch e As InvalidStepException
				log.warn("Invalid step...continuing another iteration: {}", e.Message)
				[step] = 0.0
			End Try

			'Update parameters based on final/best step size returned by line search:
			If [step] <> 0.0 Then
				' TODO: inject accumulation use here
				stepFunction.step(parameters, searchDirection, [step]) 'Calculate params. given step size
				model.Params = parameters
			Else
				log.debug("Step size returned by line search is 0.0.")
			End If

			pair = gradientAndScore(workspaceMgr)

			'updates searchDirection
			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.scopeOutOfWorkspaces()
				postStep(pair.First.gradient())
			End Using

			'invoke listeners
			Dim iterationCount As Integer = BaseOptimizer.getIterationCount(model)
			Dim epochCount As Integer = BaseOptimizer.getEpochCount(model)
			Using workspace As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
				For Each listener As TrainingListener In trainingListeners
					listener.iterationDone(model, iterationCount, epochCount)
				Next listener
			End Using


			'check for termination conditions based on absolute change in score
			incrementIterationCount(model, 1)
			applyConstraints(model)
			Return True
		End Function

		Protected Friend Overridable Sub postFirstStep(ByVal gradient As INDArray)
			'no-op
		End Sub

		Public Overridable Function batchSize() As Integer Implements ConvexOptimizer.batchSize
			Return batchSize_Conflict
		End Function

		Public Overridable WriteOnly Property BatchSize Implements ConvexOptimizer.setBatchSize As Integer
			Set(ByVal batchSize As Integer)
				Me.batchSize_Conflict = batchSize
			End Set
		End Property


		''' <summary>
		''' Pre preProcess to setup initial searchDirection approximation
		''' </summary>
		Public Overridable Sub preProcessLine() Implements ConvexOptimizer.preProcessLine
			'no-op
		End Sub

		''' <summary>
		''' Post step to update searchDirection with new gradient and parameter information
		''' </summary>
		Public Overridable Sub postStep(ByVal gradient As INDArray) Implements ConvexOptimizer.postStep
			'no-op
		End Sub


		Public Overridable Sub updateGradientAccordingToParams(ByVal gradient As Gradient, ByVal model As Model, ByVal batchSize As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) Implements ConvexOptimizer.updateGradientAccordingToParams
			If TypeOf model Is ComputationGraph Then
				Dim graph As ComputationGraph = DirectCast(model, ComputationGraph)
				If computationGraphUpdater_Conflict Is Nothing Then
					Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
						computationGraphUpdater_Conflict = New ComputationGraphUpdater(graph)
					End Using
				End If
				computationGraphUpdater_Conflict.update(gradient, getIterationCount(model), getEpochCount(model), batchSize, workspaceMgr)
			Else
				If updater_Conflict Is Nothing Then
					Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
						updater_Conflict = UpdaterCreator.getUpdater(model)
					End Using
				End If
				Dim layer As Layer = DirectCast(model, Layer)

				updater_Conflict.update(layer, gradient, getIterationCount(model), getEpochCount(model), batchSize, workspaceMgr)
			End If
		End Sub

		''' <summary>
		''' Setup the initial search state </summary>
		''' <param name="pair"> </param>
		Public Overridable Sub setupSearchState(ByVal pair As Pair(Of Gradient, Double)) Implements ConvexOptimizer.setupSearchState
			Dim gradient As INDArray = pair.First.gradient(conf_Conflict.variables())
			Dim params As INDArray = model.params().dup() 'Need dup here: params returns an array that isn't a copy (hence changes to this are problematic for line search methods)
			searchState(GRADIENT_KEY) = gradient
			searchState(SCORE_KEY) = pair.Second
			searchState(PARAMS_KEY) = params
		End Sub


		Public Shared Function getDefaultStepFunctionForOptimizer(ByVal optimizerClass As Type) As StepFunction
			If optimizerClass = GetType(StochasticGradientDescent) Then
				Return New NegativeGradientStepFunction()
			Else
				Return New NegativeDefaultStepFunction()
			End If
		End Function

		Public Shared Function getIterationCount(ByVal model As Model) As Integer
			If TypeOf model Is MultiLayerNetwork Then
				Return DirectCast(model, MultiLayerNetwork).LayerWiseConfigurations.getIterationCount()
			ElseIf TypeOf model Is ComputationGraph Then
				Return DirectCast(model, ComputationGraph).Configuration.getIterationCount()
			Else
				Return model.conf().getIterationCount()
			End If
		End Function

		Public Shared Sub incrementIterationCount(ByVal model As Model, ByVal incrementBy As Integer)
			If TypeOf model Is MultiLayerNetwork Then
				Dim conf As MultiLayerConfiguration = DirectCast(model, MultiLayerNetwork).LayerWiseConfigurations
				conf.setIterationCount(conf.getIterationCount() + incrementBy)
			ElseIf TypeOf model Is ComputationGraph Then
				Dim conf As ComputationGraphConfiguration = DirectCast(model, ComputationGraph).Configuration
				conf.setIterationCount(conf.getIterationCount() + incrementBy)
			Else
				model.conf().setIterationCount(model.conf().getIterationCount() + incrementBy)
			End If
		End Sub

		Public Shared Function getEpochCount(ByVal model As Model) As Integer
			If TypeOf model Is MultiLayerNetwork Then
				Return DirectCast(model, MultiLayerNetwork).LayerWiseConfigurations.EpochCount
			ElseIf TypeOf model Is ComputationGraph Then
				Return DirectCast(model, ComputationGraph).Configuration.getEpochCount()
			Else
				Return model.conf().getEpochCount()
			End If
		End Function

		Public Shared Sub applyConstraints(ByVal model As Model)
			Dim iter As Integer = getIterationCount(model)
			Dim epoch As Integer = getEpochCount(model)
			model.applyConstraints(iter, epoch)
		End Sub

	End Class

End Namespace