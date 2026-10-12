Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports BaseWrapperLayer = org.deeplearning4j.nn.layers.wrapper.BaseWrapperLayer
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.common.primitives
Imports OneTimeLogger = org.nd4j.common.util.OneTimeLogger

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
	''' Frozen layer freezes parameters of the layer it wraps, but allows the backpropagation to continue.
	''' 
	''' @author Ugljesa Jovanovic (ugljesa.jovanovic@ionspin.com)
	''' </summary>

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class FrozenLayerWithBackprop extends org.deeplearning4j.nn.layers.wrapper.BaseWrapperLayer
	<Serializable>
	Public Class FrozenLayerWithBackprop
		Inherits BaseWrapperLayer

		Private logUpdate As Boolean = False
		Private logFit As Boolean = False
'JAVA TO VB CONVERTER NOTE: The field logTestMode was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private logTestMode_Conflict As Boolean = False
		Private logGradient As Boolean = False

		Private zeroGradient As Gradient

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public FrozenLayerWithBackprop(final org.deeplearning4j.nn.api.Layer insideLayer)
		Public Sub New(ByVal insideLayer As Layer)
			MyBase.New(insideLayer)
			Me.zeroGradient = New DefaultGradient(insideLayer.params())
		End Sub

		Protected Friend Overridable Function layerId() As String
			Dim name As String = underlying.conf().getLayer().getLayerName()
			Return "(layer name: " & (If(name Is Nothing, """""", name)) & ", layer index: " & underlying.Index & ")"
		End Function

		Public Overrides Function calcRegularizationScore(ByVal backpropParamsOnly As Boolean) As Double
			Return 0.0
		End Function

		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			Dim backpropEpsilon As INDArray = underlying.backpropGradient(epsilon, workspaceMgr).Second
			'backprop might have already changed the gradient view (like BaseLayer and BaseOutputLayer do)
			'so we want to put it back to zeroes
			Dim gradientView As INDArray = underlying.GradientsViewArray
			If gradientView IsNot Nothing Then
				gradientView.assign(0)
			End If
			Return New Pair(Of Gradient, INDArray)(zeroGradient, backpropEpsilon)
		End Function
		Public Overrides Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			logTestMode(training)
			Return underlying.activate(False, workspaceMgr)
		End Function

		Public Overrides Function activate(ByVal input As INDArray, ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			logTestMode(training)
			Return underlying.activate(input, False, workspaceMgr)
		End Function

		Public Overrides Sub fit()
			If Not logFit Then
				OneTimeLogger.info(log, "Frozen layers cannot be fit. Warning will be issued only once per instance")
				logFit = True
			End If
			'no op
		End Sub

		Public Overrides Sub update(ByVal gradient As Gradient)
			If Not logUpdate Then
				OneTimeLogger.info(log, "Frozen layers will not be updated. Warning will be issued only once per instance")
				logUpdate = True
			End If
			'no op
		End Sub

		Public Overrides Sub update(ByVal gradient As INDArray, ByVal paramType As String)
			If Not logUpdate Then
				OneTimeLogger.info(log, "Frozen layers will not be updated. Warning will be issued only once per instance")
				logUpdate = True
			End If
			'no op
		End Sub

		Public Overrides Sub computeGradientAndScore(ByVal workspaceMgr As LayerWorkspaceMgr)
			If Not logGradient Then
				OneTimeLogger.info(log, "Gradients for the frozen layer are not set and will therefore will not be updated.Warning will be issued only once per instance")
				logGradient = True
			End If
			underlying.score()
			'no op
		End Sub

		Public Overrides WriteOnly Property BackpropGradientsViewArray As INDArray
			Set(ByVal gradients As INDArray)
				underlying.BackpropGradientsViewArray = gradients
				If Not logGradient Then
					OneTimeLogger.info(log, "Gradients for the frozen layer are not set and will therefore will not be updated.Warning will be issued only once per instance")
					logGradient = True
				End If
				'no-op
			End Set
		End Property

		Public Overrides Sub fit(ByVal data As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr)
			If Not logFit Then
				OneTimeLogger.info(log, "Frozen layers cannot be fit, but backpropagation will continue.Warning will be issued only once per instance")
				logFit = True
			End If
		End Sub

		Public Overrides Sub applyConstraints(ByVal iteration As Integer, ByVal epoch As Integer)
			'No-op
		End Sub

		Public Overridable Sub logTestMode(ByVal training As Boolean)
			If Not training Then
				Return
			End If
			If logTestMode_Conflict Then
				Return
			Else
				OneTimeLogger.info(log, "Frozen layer instance found! Frozen layers are treated as always in test mode. Warning will only be issued once per instance")
				logTestMode_Conflict = True
			End If
		End Sub

		Public Overridable Sub logTestMode(ByVal training As TrainingMode)
			If training.Equals(TrainingMode.TEST) Then
				Return
			End If
			If logTestMode_Conflict Then
				Return
			Else
				OneTimeLogger.info(log, "Frozen layer instance found! Frozen layers are treated as always in test mode. Warning will only be issued once per instance")
				logTestMode_Conflict = True
			End If
		End Sub

		Public Overridable ReadOnly Property InsideLayer As Layer
			Get
				Return underlying
			End Get
		End Property
	End Class



End Namespace