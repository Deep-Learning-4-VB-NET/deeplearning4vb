Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports NonNull = lombok.NonNull
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports TrainingConfig = org.deeplearning4j.nn.api.TrainingConfig
Imports CacheMode = org.deeplearning4j.nn.conf.CacheMode
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports LayerHelper = org.deeplearning4j.nn.layers.LayerHelper
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports ConvexOptimizer = org.deeplearning4j.optimize.api.ConvexOptimizer
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
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

Namespace org.deeplearning4j.nn.layers.wrapper


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public abstract class BaseWrapperLayer implements org.deeplearning4j.nn.api.Layer
	<Serializable>
	Public MustInherit Class BaseWrapperLayer
		Implements Layer

		Protected Friend underlying As Layer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BaseWrapperLayer(@NonNull Layer underlying)
		Public Sub New(ByVal underlying As Layer)
			Me.underlying = underlying
		End Sub

		Public Overridable WriteOnly Property CacheMode Implements Layer.setCacheMode As CacheMode
			Set(ByVal mode As CacheMode)
				underlying.CacheMode = mode
			End Set
		End Property

		Public Overridable Function calcRegularizationScore(ByVal backpropParamsOnly As Boolean) As Double Implements Layer.calcRegularizationScore
			Return underlying.calcRegularizationScore(backpropParamsOnly)
		End Function

		Public Overridable Function type() As Type Implements Layer.type
			Return underlying.type()
		End Function

		Public Overridable Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray) Implements Layer.backpropGradient
			Return underlying.backpropGradient(epsilon, workspaceMgr)
		End Function

		Public Overridable Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements Layer.activate
			Return underlying.activate(training, workspaceMgr)
		End Function

		Public Overridable Function activate(ByVal input As INDArray, ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements Layer.activate
			Return underlying.activate(input, training, workspaceMgr)
		End Function

		Public Overridable Property Listeners As ICollection(Of TrainingListener) Implements Layer.getListeners
			Get
				Return underlying.getListeners()
			End Get
			Set(ByVal listeners() As TrainingListener)
				underlying.setListeners(listeners)
			End Set
		End Property


		Public Overridable Sub addListeners(ParamArray ByVal listener() As TrainingListener)
			underlying.addListeners(listener)
		End Sub

		Public Overridable Sub fit()
			underlying.fit()
		End Sub

		Public Overridable Sub update(ByVal gradient As Gradient)
			underlying.update(gradient)
		End Sub

		Public Overridable Sub update(ByVal gradient As INDArray, ByVal paramType As String)
			underlying.update(gradient, paramType)
		End Sub

		Public Overridable Function score() As Double
			Return underlying.score()
		End Function

		Public Overridable Sub computeGradientAndScore(ByVal workspaceMgr As LayerWorkspaceMgr)
			underlying.computeGradientAndScore(workspaceMgr)
		End Sub

		Public Overridable Function params() As INDArray
			Return underlying.params()
		End Function

		Public Overridable Function numParams() As Long
			Return underlying.numParams()
		End Function

		Public Overridable Function numParams(ByVal backwards As Boolean) As Long
			Return underlying.numParams()
		End Function

		Public Overridable WriteOnly Property Params As INDArray
			Set(ByVal params As INDArray)
				underlying.Params = params
			End Set
		End Property

		Public Overridable WriteOnly Property ParamsViewArray As INDArray
			Set(ByVal params As INDArray)
				underlying.ParamsViewArray = params
			End Set
		End Property

		Public Overridable ReadOnly Property GradientsViewArray As INDArray
			Get
				Return underlying.GradientsViewArray
			End Get
		End Property

		Public Overridable WriteOnly Property BackpropGradientsViewArray As INDArray
			Set(ByVal gradients As INDArray)
				underlying.BackpropGradientsViewArray = gradients
			End Set
		End Property

		Public Overridable Sub fit(ByVal data As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr)
			underlying.fit(data, workspaceMgr)
		End Sub

		Public Overridable Function gradient() As Gradient
			Return underlying.gradient()
		End Function

		Public Overridable Function gradientAndScore() As Pair(Of Gradient, Double)
			Return underlying.gradientAndScore()
		End Function

		Public Overridable Function batchSize() As Integer
			Return underlying.batchSize()
		End Function

		Public Overridable Function conf() As NeuralNetConfiguration
			Return underlying.conf()
		End Function

		Public Overridable WriteOnly Property Conf As NeuralNetConfiguration
			Set(ByVal conf As NeuralNetConfiguration)
				underlying.Conf = conf
			End Set
		End Property

		Public Overridable Function input() As INDArray
			Return underlying.input()
		End Function

		Public Overridable ReadOnly Property Optimizer As ConvexOptimizer
			Get
				Return underlying.Optimizer
			End Get
		End Property

		Public Overridable Function getParam(ByVal param As String) As INDArray
			Return underlying.getParam(param)
		End Function

		Public Overridable Function paramTable() As IDictionary(Of String, INDArray)
			Return underlying.paramTable()
		End Function

		Public Overridable Function paramTable(ByVal backpropParamsOnly As Boolean) As IDictionary(Of String, INDArray)
			Return underlying.paramTable(backpropParamsOnly)
		End Function

		Public Overridable WriteOnly Property ParamTable As IDictionary(Of String, INDArray)
			Set(ByVal paramTable As IDictionary(Of String, INDArray))
				underlying.ParamTable = paramTable
			End Set
		End Property

		Public Overridable Sub setParam(ByVal key As String, ByVal val As INDArray)
			underlying.setParam(key, val)
		End Sub

		Public Overridable Sub clear()
			underlying.clear()
		End Sub

		Public Overridable Sub applyConstraints(ByVal iteration As Integer, ByVal epoch As Integer)
			underlying.applyConstraints(iteration, epoch)
		End Sub

		Public Overridable Sub init()
			underlying.init()
		End Sub

		Public Overridable WriteOnly Property Listeners Implements Layer.setListeners As ICollection(Of TrainingListener)
			Set(ByVal listeners As ICollection(Of TrainingListener))
				underlying.setListeners(listeners)
			End Set
		End Property

		Public Overridable Property Index Implements Layer.setIndex As Integer
			Set(ByVal index As Integer)
				underlying.Index = index
			End Set
			Get
				Return underlying.Index
			End Get
		End Property


		Public Overridable Property IterationCount As Integer Implements Layer.getIterationCount
			Get
				Return underlying.IterationCount
			End Get
			Set(ByVal iterationCount As Integer)
				underlying.IterationCount = iterationCount
			End Set
		End Property

		Public Overridable Property EpochCount As Integer Implements Layer.getEpochCount
			Get
				Return underlying.EpochCount
			End Get
			Set(ByVal epochCount As Integer)
				underlying.EpochCount = epochCount
			End Set
		End Property



		Public Overridable Sub setInput(ByVal input As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) Implements Layer.setInput
			underlying.setInput(input, workspaceMgr)
		End Sub

		Public Overridable Property InputMiniBatchSize Implements Layer.setInputMiniBatchSize As Integer
			Set(ByVal size As Integer)
				underlying.InputMiniBatchSize = size
			End Set
			Get
				Return underlying.InputMiniBatchSize
			End Get
		End Property


		Public Overridable Property MaskArray Implements Layer.setMaskArray As INDArray
			Set(ByVal maskArray As INDArray)
				underlying.MaskArray = maskArray
			End Set
			Get
				Return underlying.MaskArray
			End Get
		End Property


		Public Overridable ReadOnly Property PretrainLayer As Boolean Implements Layer.isPretrainLayer
			Get
				Return underlying.PretrainLayer
			End Get
		End Property

		Public Overridable Sub clearNoiseWeightParams() Implements Layer.clearNoiseWeightParams
			underlying.clearNoiseWeightParams()
		End Sub

		Public Overridable Function feedForwardMaskArray(ByVal maskArray As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState)
			Return underlying.feedForwardMaskArray(maskArray, currentMaskState, minibatchSize)
		End Function

		Public Overridable Sub allowInputModification(ByVal allow As Boolean) Implements Layer.allowInputModification
			underlying.allowInputModification(allow)
		End Sub

		Public Overridable ReadOnly Property Helper As LayerHelper Implements Layer.getHelper
			Get
				Return underlying.Helper
			End Get
		End Property

		Public Overridable ReadOnly Property Config As TrainingConfig
			Get
				Return underlying.Config
			End Get
		End Property

		Public Overridable Function updaterDivideByMinibatch(ByVal paramName As String) As Boolean
			Return underlying.updaterDivideByMinibatch(paramName)
		End Function

		Public Overridable Sub close()
			'No-op for individual layers
		End Sub
	End Class

End Namespace