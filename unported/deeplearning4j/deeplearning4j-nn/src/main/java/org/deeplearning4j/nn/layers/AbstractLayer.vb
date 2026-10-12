Imports System
Imports System.Collections.Generic
Imports AccessLevel = lombok.AccessLevel
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports Setter = lombok.Setter
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports TrainingConfig = org.deeplearning4j.nn.api.TrainingConfig
Imports LayerConstraint = org.deeplearning4j.nn.api.layers.LayerConstraint
Imports CacheMode = org.deeplearning4j.nn.conf.CacheMode
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports ConvexOptimizer = org.deeplearning4j.optimize.api.ConvexOptimizer
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports DataType = org.nd4j.linalg.api.buffer.DataType
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

Namespace org.deeplearning4j.nn.layers


	''' <summary>
	''' A layer with input and output, no parameters or gradients
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @NoArgsConstructor public abstract class AbstractLayer<LayerConfT extends org.deeplearning4j.nn.conf.layers.Layer> implements org.deeplearning4j.nn.api.Layer
	<Serializable>
	Public MustInherit Class AbstractLayer(Of LayerConfT As org.deeplearning4j.nn.conf.layers.Layer)
		Implements Layer

		Public MustOverride Sub clearNoiseWeightParams() Implements Layer.clearNoiseWeightParams
		Public MustOverride ReadOnly Property PretrainLayer As Boolean Implements Layer.isPretrainLayer
		Public MustOverride Property IterationCount As Integer
		Public MustOverride Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
		Public MustOverride Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray) Implements Layer.backpropGradient
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter(lombok.AccessLevel.NONE) protected org.nd4j.linalg.api.ndarray.INDArray input;
'JAVA TO VB CONVERTER NOTE: The field input was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend input_Conflict As INDArray
		Protected Friend preOutput As INDArray
'JAVA TO VB CONVERTER NOTE: The field conf was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend conf_Conflict As NeuralNetConfiguration
		Protected Friend dropoutApplied As Boolean = False
		Protected Friend trainingListeners As ICollection(Of TrainingListener) = New List(Of TrainingListener)()
'JAVA TO VB CONVERTER NOTE: The field index was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend index_Conflict As Integer = 0
'JAVA TO VB CONVERTER NOTE: The field maskArray was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend maskArray_Conflict As INDArray
		Protected Friend maskState As MaskState
'JAVA TO VB CONVERTER NOTE: The field cacheMode was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend cacheMode_Conflict As CacheMode = CacheMode.NONE
		Protected Friend inputModificationAllowed As Boolean = False
		Protected Friend dataType As DataType

		Protected Friend iterationCount As Integer
'JAVA TO VB CONVERTER NOTE: The field epochCount was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend epochCount_Conflict As Integer

		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			Me.conf_Conflict = conf
			If conf IsNot Nothing Then
				cacheMode_Conflict = conf.getCacheMode()
			End If
			Me.dataType = dataType
		End Sub

		Public Overridable WriteOnly Property CacheMode Implements Layer.setCacheMode As CacheMode
			Set(ByVal mode As CacheMode)
				If mode = Nothing Then
					mode = CacheMode.NONE
				End If
    
				Me.cacheMode_Conflict = mode
			End Set
		End Property

		Public Overridable Function layerConf() As LayerConfT
			Return CType(Me.conf_Conflict.getLayer(), LayerConfT)
		End Function

		Public Overridable ReadOnly Property Config As TrainingConfig
			Get
				Return conf_Conflict.getLayer()
			End Get
		End Property

		Protected Friend Overridable Function layerId() As String
			Dim name As String = Me.conf().getLayer().getLayerName()
			Return "(layer name: " & (If(name Is Nothing, """""", name)) & ", layer index: " & index_Conflict & ", layer type: " & Me.GetType().Name & ")"
		End Function

		Public Overridable ReadOnly Property Input As INDArray
			Get
				Return input_Conflict
			End Get
		End Property

		Public Overridable Property EpochCount As Integer Implements Layer.getEpochCount
			Get
				Return epochCount_Conflict
			End Get
			Set(ByVal epochCount As Integer)
				Me.epochCount_Conflict = epochCount
			End Set
		End Property


		''' <summary>
		''' Init the model
		''' </summary>
		Public Overridable Sub init()

		End Sub

		Public Overridable Sub setInput(ByVal input As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) Implements Layer.setInput
			Me.input_Conflict = workspaceMgr.leverageTo(ArrayType.INPUT, input)
			dropoutApplied = False
		End Sub

		Public Overridable Property Index As Integer Implements Layer.getIndex
			Get
				Return index_Conflict
			End Get
			Set(ByVal index As Integer)
				Me.index_Conflict = index
			End Set
		End Property



		Public Overridable Property Listeners As ICollection(Of TrainingListener)
			Get
				Return trainingListeners
			End Get
			Set(ByVal listeners As ICollection(Of TrainingListener))
				Me.trainingListeners = If(listeners IsNot Nothing, listeners, New List(Of TrainingListener)())
			End Set
		End Property


		''' <summary>
		''' This method ADDS additional TrainingListener to existing listeners
		''' </summary>
		''' <param name="listeners"> </param>
		Public Overridable Sub addListeners(ParamArray ByVal listeners() As TrainingListener)
			If Me.trainingListeners Is Nothing Then
				setListeners(listeners)
				Return
			End If

			Collections.addAll(trainingListeners, listeners)
		End Sub

		Public Overridable WriteOnly Property Listeners Implements Layer.setListeners As TrainingListener()
			Set(ByVal listeners() As TrainingListener)
				setListeners(java.util.Arrays.asList(listeners))
			End Set
		End Property

		Public Overridable Sub computeGradientAndScore(ByVal workspaceMgr As LayerWorkspaceMgr)
			Throw New System.NotSupportedException("Not supported")
		End Sub

		Public Overridable Sub update(ByVal gradient As Gradient)
			Throw New System.NotSupportedException()
		End Sub

		Public Overridable Sub update(ByVal gradient As INDArray, ByVal paramType As String)
			Throw New System.NotSupportedException()
		End Sub


		Public Overridable ReadOnly Property Optimizer As ConvexOptimizer
			Get
				Throw New System.NotSupportedException("Not supported")
			End Get
		End Property

		Public Overridable WriteOnly Property Conf As NeuralNetConfiguration
			Set(ByVal conf As NeuralNetConfiguration)
				Me.conf_Conflict = conf
			End Set
		End Property

		''' <summary>
		'''Returns the parameters of the neural network as a flattened row vector </summary>
		''' <returns> the parameters of the neural network </returns>
		Public Overridable Function params() As INDArray
			Return Nothing
		End Function

		Public Overridable Function getParam(ByVal param As String) As INDArray
			Throw New System.NotSupportedException("Not supported")
		End Function

		Public Overridable Sub setParam(ByVal key As String, ByVal val As INDArray)
			Throw New System.NotSupportedException("Not supported")
		End Sub

		Public Overridable WriteOnly Property Params As INDArray
			Set(ByVal params As INDArray)
				If params IsNot Nothing Then
					Throw New System.NotSupportedException("Not supported")
				End If
			End Set
		End Property

		Protected Friend Overridable Sub setParams(ByVal params As INDArray, ByVal order As Char)
			Throw New System.NotSupportedException("Not supported")
		End Sub

		Public Overridable WriteOnly Property ParamsViewArray As INDArray
			Set(ByVal params As INDArray)
				If params IsNot Nothing Then
					Throw New System.NotSupportedException("Not supported")
				End If
			End Set
		End Property

		Public Overridable ReadOnly Property GradientsViewArray As INDArray
			Get
				Return Nothing
			End Get
		End Property

		Public Overridable WriteOnly Property BackpropGradientsViewArray As INDArray
			Set(ByVal gradients As INDArray)
				If gradients IsNot Nothing Then
					Throw New System.NotSupportedException("Not supported")
				End If
			End Set
		End Property

		Public Overridable WriteOnly Property ParamTable As IDictionary(Of String, INDArray)
			Set(ByVal paramTable As IDictionary(Of String, INDArray))
				If paramTable IsNot Nothing AndAlso paramTable.Count > 0 Then
					Throw New System.NotSupportedException("Not supported")
				End If
			End Set
		End Property

		Public Overridable Function paramTable() As IDictionary(Of String, INDArray)
			Return paramTable(False)
		End Function

		Public Overridable Function paramTable(ByVal backpropParamsOnly As Boolean) As IDictionary(Of String, INDArray)
			Return java.util.Collections.emptyMap()
		End Function

		Protected Friend Overridable Sub applyMask(ByVal [to] As INDArray)
			[to].muliColumnVector(maskArray_Conflict.castTo([to].dataType()))
		End Sub

		Public Overridable Function activate(ByVal input As INDArray, ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements Layer.activate
			setInput(input, workspaceMgr)
			Return activate(training, workspaceMgr)
		End Function

		Public Overridable Function calcRegularizationScore(ByVal backpropParamsOnly As Boolean) As Double Implements Layer.calcRegularizationScore
			Return 0.0
		End Function

		Public Overridable Function batchSize() As Integer
			Return CInt(input_Conflict.size(0))
		End Function

		Public Overridable Function conf() As NeuralNetConfiguration
			Return conf_Conflict
		End Function


		Public Overridable Sub clear()
			input_Conflict = Nothing
			maskArray_Conflict = Nothing
			maskState = Nothing
			If layerConf().getIDropout() IsNot Nothing Then
				layerConf().getIDropout().clear()
			End If
		End Sub

		Protected Friend Overridable Sub applyDropOutIfNecessary(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr)
			If training AndAlso Not dropoutApplied AndAlso layerConf().getIDropout() IsNot Nothing Then
				Dim result As INDArray
				If inputModificationAllowed Then
					result = input_Conflict
				Else
					result = workspaceMgr.createUninitialized(ArrayType.INPUT, input_Conflict.dataType(), input_Conflict.shape(), input_Conflict.ordering())
				End If

				input_Conflict = layerConf().getIDropout().applyDropout(input_Conflict, result, IterationCount, EpochCount, workspaceMgr)
				dropoutApplied = True
			End If
		End Sub

		Protected Friend Overridable Function backpropDropOutIfPresent(ByVal epsilon As INDArray) As INDArray
			If layerConf().getIDropout() IsNot Nothing Then
				layerConf().getIDropout().backprop(epsilon, epsilon, IterationCount, EpochCount)
			End If
			Return epsilon
		End Function


		Public Overridable Function type() As Type Implements Layer.type
			Return Type.FEED_FORWARD
		End Function

		''' <summary>
		''' The number of parameters for the model
		''' </summary>
		''' <returns> the number of parameters for the model </returns>
		Public Overridable Function numParams() As Long
			Return 0
		End Function

		Public Overridable Function numParams(ByVal backwards As Boolean) As Long
			Return numParams()
		End Function

		Public Overridable Sub fit(ByVal input As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr)
			Throw New System.NotSupportedException("Not supported")
		End Sub


		Public Overridable Function gradientAndScore() As Pair(Of Gradient, Double)
			Return New Pair(Of Gradient, Double)(gradient(), score())
		End Function

		Public Overridable Function input() As INDArray
			Return input_Conflict
		End Function

		Public Overridable Property InputMiniBatchSize Implements Layer.setInputMiniBatchSize As Integer
			Set(ByVal size As Integer)
			End Set
			Get
				Return CInt(input_Conflict.size(0))
			End Get
		End Property

		Public Overridable Property MaskArray Implements Layer.setMaskArray As INDArray
			Set(ByVal maskArray As INDArray)
				Me.maskArray_Conflict = maskArray
			End Set
			Get
				Return maskArray_Conflict
			End Get
		End Property



		Public Overridable Function feedForwardMaskArray(ByVal maskArray As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState)
			'Most layers: CNN, dense, activation, etc - set mask array, mask state and then leave the mask unmodified

			Me.maskArray_Conflict = maskArray
			Me.maskState = currentMaskState

			Return New Pair(Of INDArray, MaskState)(maskArray, currentMaskState)
		End Function


		Public Overridable Function gradient() As Gradient
			Throw New System.NotSupportedException("Not supported for this layer, or should be overridden for layers requiring it")
		End Function

		Public Overridable Sub fit()
			Throw New System.NotSupportedException("Not supported for this layer, or should be overridden for layers requiring it")
		End Sub

		Public Overridable Function score() As Double
			Throw New System.NotSupportedException("Not supported for this layer, or should be overridden for layers requiring it")
		End Function


		Public Overridable Sub applyConstraints(ByVal iteration As Integer, ByVal epoch As Integer)
			If layerConf().getConstraints() IsNot Nothing Then
				For Each lc As LayerConstraint In layerConf().getConstraints()
					lc.applyConstraint(Me, iteration, epoch)
				Next lc
			End If
		End Sub

		Public Overridable Sub assertInputSet(ByVal backprop As Boolean)
			If input_Conflict Is Nothing Then
				If backprop Then
					Throw New System.InvalidOperationException("Cannot perform backprop in layer " & Me.GetType().Name & ": layer input field is not set")
				Else
					Throw New System.InvalidOperationException("Cannot perform forward pass in layer " & Me.GetType().Name & ": layer input field is not set")
				End If
			End If
		End Sub

		Public Overridable Sub allowInputModification(ByVal allow As Boolean) Implements Layer.allowInputModification
			inputModificationAllowed = allow
		End Sub

		Public Overridable ReadOnly Property Helper As LayerHelper
			Get
				'Layers with helpers should override this method!
				Return Nothing
			End Get
		End Property

		Public Overridable Function updaterDivideByMinibatch(ByVal paramName As String) As Boolean
			'Majority of params's gradients should be... Exception: batch norm mean/variance estimate
			Return True
		End Function

		Public Overridable Sub close()
			'No-op for individual layers
		End Sub
	End Class

End Namespace