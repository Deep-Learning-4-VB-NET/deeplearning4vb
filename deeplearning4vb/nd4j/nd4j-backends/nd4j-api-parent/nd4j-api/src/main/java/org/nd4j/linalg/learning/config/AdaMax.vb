﻿Imports System
Imports System.Collections.Generic
Imports lombok
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports AdaMaxUpdater = org.nd4j.linalg.learning.AdaMaxUpdater
Imports org.nd4j.linalg.learning
Imports ISchedule = org.nd4j.linalg.schedule.ISchedule
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty

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

Namespace org.nd4j.linalg.learning.config


	''' <summary>
	''' The AdaMax updater, a variant of Adam.
	''' https://arxiv.org/abs/1412.6980
	''' 
	''' @author Justin Long
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @Builder(builderClassName = "Builder") public class AdaMax implements IUpdater
	<Serializable>
	Public Class AdaMax
		Implements IUpdater

		Public Const DEFAULT_ADAMAX_LEARNING_RATE As Double = 1e-3
		Public Const DEFAULT_ADAMAX_EPSILON As Double = 1e-8
		Public Const DEFAULT_ADAMAX_BETA1_MEAN_DECAY As Double = 0.9
		Public Const DEFAULT_ADAMAX_BETA2_VAR_DECAY As Double = 0.999

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @lombok.Builder.@Default private double learningRate = DEFAULT_ADAMAX_LEARNING_RATE;
		Private learningRate As Double = DEFAULT_ADAMAX_LEARNING_RATE ' learning rate
		Private learningRateSchedule As ISchedule
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @lombok.Builder.@Default private double beta1 = DEFAULT_ADAMAX_BETA1_MEAN_DECAY;
		Private beta1 As Double = DEFAULT_ADAMAX_BETA1_MEAN_DECAY ' gradient moving avg decay rate
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @lombok.Builder.@Default private double beta2 = DEFAULT_ADAMAX_BETA2_VAR_DECAY;
		Private beta2 As Double = DEFAULT_ADAMAX_BETA2_VAR_DECAY ' gradient sqrd decay rate
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @lombok.Builder.@Default private double epsilon = DEFAULT_ADAMAX_EPSILON;
		Private epsilon As Double = DEFAULT_ADAMAX_EPSILON

		Public Sub New()
			Me.New(DEFAULT_ADAMAX_LEARNING_RATE)
		End Sub

		Public Sub New(ByVal learningRate As Double)
			Me.New(learningRate, Nothing, DEFAULT_ADAMAX_BETA1_MEAN_DECAY, DEFAULT_ADAMAX_BETA2_VAR_DECAY, DEFAULT_ADAMAX_EPSILON)
		End Sub

		Public Sub New(ByVal learningRateSchedule As ISchedule)
			Me.New(Double.NaN, learningRateSchedule, DEFAULT_ADAMAX_BETA1_MEAN_DECAY, DEFAULT_ADAMAX_BETA2_VAR_DECAY, DEFAULT_ADAMAX_EPSILON)
		End Sub

		Public Sub New(ByVal learningRate As Double, ByVal beta1 As Double, ByVal beta2 As Double, ByVal epsilon As Double)
			Me.New(learningRate, Nothing, beta1, beta2, epsilon)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: private AdaMax(@JsonProperty("learningRate") double learningRate, @JsonProperty("learningRateSchedule") org.nd4j.linalg.schedule.ISchedule learningRateSchedule, @JsonProperty("beta1") double beta1, @JsonProperty("beta2") double beta2, @JsonProperty("epsilon") double epsilon)
		Private Sub New(ByVal learningRate As Double, ByVal learningRateSchedule As ISchedule, ByVal beta1 As Double, ByVal beta2 As Double, ByVal epsilon As Double)
			Me.learningRate = learningRate
			Me.learningRateSchedule = learningRateSchedule
			Me.beta1 = beta1
			Me.beta2 = beta2
			Me.epsilon = epsilon
		End Sub

		Public Overridable Function stateSize(ByVal numParams As Long) As Long Implements IUpdater.stateSize
			Return 2 * numParams
		End Function

		Public Overridable Function instantiate(ByVal viewArray As INDArray, ByVal initializeViewArray As Boolean) As GradientUpdater Implements IUpdater.instantiate
			Dim a As New AdaMaxUpdater(Me)
			Dim gradientShape() As Long = viewArray.shape()
			gradientShape = Arrays.CopyOf(gradientShape, gradientShape.Length)
			gradientShape(1) \= 2
			a.setStateViewArray(viewArray, gradientShape, viewArray.ordering(), initializeViewArray)
			Return a
		End Function

		Public Overridable Function instantiate(ByVal updaterState As IDictionary(Of String, INDArray), ByVal initializeStateArrays As Boolean) As GradientUpdater Implements IUpdater.instantiate
			Dim u As New AdaMaxUpdater(Me)
			u.setState(updaterState, initializeStateArrays)
			Return u
		End Function

		Public Overridable Function clone() As IUpdater Implements IUpdater.clone
			Return New AdaMax(learningRate, learningRateSchedule, beta1, beta2, epsilon)
		End Function

		Public Overridable Function getLearningRate(ByVal iteration As Integer, ByVal epoch As Integer) As Double Implements IUpdater.getLearningRate
			If learningRateSchedule IsNot Nothing Then
				Return learningRateSchedule.valueAt(iteration, epoch)
			End If
			Return learningRate
		End Function

		Public Overridable Function hasLearningRate() As Boolean Implements IUpdater.hasLearningRate
			Return True
		End Function

		Public Overridable Sub setLrAndSchedule(ByVal lr As Double, ByVal lrSchedule As ISchedule) Implements IUpdater.setLrAndSchedule
			Me.learningRate = lr
			Me.learningRateSchedule = lrSchedule
		End Sub

		'Partial builder implementation to give public no-arg constructor
		Public Class Builder
			Public Sub New()
			End Sub
		End Class
	End Class

End Namespace