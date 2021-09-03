Imports System.Collections.Generic
Imports Model = org.deeplearning4j.nn.api.Model
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports ConvexOptimizer = org.deeplearning4j.optimize.api.ConvexOptimizer
Imports StepFunction = org.deeplearning4j.optimize.api.StepFunction
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports ConjugateGradient = org.deeplearning4j.optimize.solvers.ConjugateGradient
Imports LBFGS = org.deeplearning4j.optimize.solvers.LBFGS
Imports LineGradientDescent = org.deeplearning4j.optimize.solvers.LineGradientDescent
Imports StochasticGradientDescent = org.deeplearning4j.optimize.solvers.StochasticGradientDescent
Imports StepFunctions = org.deeplearning4j.optimize.stepfunctions.StepFunctions
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.deeplearning4j.optimize


	Public Class Solver
		Private conf As NeuralNetConfiguration
'JAVA TO VB CONVERTER NOTE: The field listeners was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private listeners_Conflict As ICollection(Of TrainingListener)
		Private model As Model
'JAVA TO VB CONVERTER NOTE: The field optimizer was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private optimizer_Conflict As ConvexOptimizer
		Private stepFunction As StepFunction

		Public Overridable Sub optimize(ByVal workspaceMgr As LayerWorkspaceMgr)
			initOptimizer()

			optimizer_Conflict.optimize(workspaceMgr)
		End Sub

		Public Overridable Sub initOptimizer()
			If optimizer_Conflict Is Nothing Then
				Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
					optimizer_Conflict = Optimizer
				End Using
			End If
		End Sub

		Public Overridable ReadOnly Property Optimizer As ConvexOptimizer
			Get
				If optimizer_Conflict IsNot Nothing Then
					Return optimizer_Conflict
				End If
				Select Case conf.getOptimizationAlgo()
					Case LBFGS
						optimizer_Conflict = New LBFGS(conf, stepFunction, listeners_Conflict, model)
					Case LINE_GRADIENT_DESCENT
						optimizer_Conflict = New LineGradientDescent(conf, stepFunction, listeners_Conflict, model)
					Case CONJUGATE_GRADIENT
						optimizer_Conflict = New ConjugateGradient(conf, stepFunction, listeners_Conflict, model)
					Case STOCHASTIC_GRADIENT_DESCENT
						optimizer_Conflict = New StochasticGradientDescent(conf, stepFunction, listeners_Conflict, model)
					Case Else
						Throw New System.InvalidOperationException("No optimizer found")
				End Select
				Return optimizer_Conflict
			End Get
		End Property

		Public Overridable WriteOnly Property Listeners As ICollection(Of TrainingListener)
			Set(ByVal listeners As ICollection(Of TrainingListener))
				Me.listeners_Conflict = listeners
				If optimizer_Conflict IsNot Nothing Then
					optimizer_Conflict.Listeners = listeners
				End If
			End Set
		End Property

		Public Class Builder
			Friend conf As NeuralNetConfiguration
'JAVA TO VB CONVERTER NOTE: The field model was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend model_Conflict As Model
'JAVA TO VB CONVERTER NOTE: The field listeners was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend listeners_Conflict As IList(Of TrainingListener) = New List(Of TrainingListener)()

			Public Overridable Function configure(ByVal conf As NeuralNetConfiguration) As Builder
				Me.conf = conf
				Return Me
			End Function

			Public Overridable Function listener(ParamArray ByVal listeners() As TrainingListener) As Builder
				If listeners IsNot Nothing Then
					CType(Me.listeners_Conflict, List(Of TrainingListener)).AddRange(New List(Of TrainingListener) From {listeners})
				End If

				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter listeners was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function listeners(ByVal listeners_Conflict As ICollection(Of TrainingListener)) As Builder
				If listeners_Conflict IsNot Nothing Then
					CType(Me.listeners_Conflict, List(Of TrainingListener)).AddRange(listeners_Conflict)
				End If

				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter model was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function model(ByVal model_Conflict As Model) As Builder
				Me.model_Conflict = model_Conflict
				Return Me
			End Function

			Public Overridable Function build() As Solver
				Dim solver As New Solver()
				solver.conf = conf
				solver.stepFunction = StepFunctions.createStepFunction(conf.getStepFunction())
				solver.model = model_Conflict
				solver.listeners = listeners_Conflict
				Return solver
			End Function
		End Class


	End Class

End Namespace