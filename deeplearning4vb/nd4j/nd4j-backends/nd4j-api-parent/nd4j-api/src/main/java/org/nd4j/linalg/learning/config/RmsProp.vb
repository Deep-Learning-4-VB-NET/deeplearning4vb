﻿Imports System
Imports System.Collections.Generic
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.linalg.learning
Imports RmsPropUpdater = org.nd4j.linalg.learning.RmsPropUpdater
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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @Builder(builderClassName = "Builder") public class RmsProp implements IUpdater
	<Serializable>
	Public Class RmsProp
		Implements IUpdater

		Public Const DEFAULT_RMSPROP_LEARNING_RATE As Double = 1e-1
		Public Const DEFAULT_RMSPROP_EPSILON As Double = 1e-8
		Public Const DEFAULT_RMSPROP_RMSDECAY As Double = 0.95

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @lombok.Builder.@Default private double learningRate = DEFAULT_RMSPROP_LEARNING_RATE;
		Private learningRate As Double = DEFAULT_RMSPROP_LEARNING_RATE
		Private learningRateSchedule As ISchedule
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @lombok.Builder.@Default private double rmsDecay = DEFAULT_RMSPROP_RMSDECAY;
		Private rmsDecay As Double = DEFAULT_RMSPROP_RMSDECAY
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @lombok.Builder.@Default private double epsilon = DEFAULT_RMSPROP_EPSILON;
		Private epsilon As Double = DEFAULT_RMSPROP_EPSILON

		Public Sub New()
			Me.New(DEFAULT_RMSPROP_LEARNING_RATE, Nothing, DEFAULT_RMSPROP_RMSDECAY, DEFAULT_RMSPROP_EPSILON)
		End Sub

		Public Sub New(ByVal learningRate As Double)
			Me.New(learningRate, Nothing, DEFAULT_RMSPROP_RMSDECAY, DEFAULT_RMSPROP_EPSILON)
		End Sub

		Public Sub New(ByVal learningRateSchedule As ISchedule)
			Me.New(Double.NaN, learningRateSchedule, DEFAULT_RMSPROP_RMSDECAY, DEFAULT_RMSPROP_EPSILON)
		End Sub

		Public Sub New(ByVal learningRate As Double, ByVal rmsDecay As Double, ByVal epsilon As Double)
			Me.New(learningRate, Nothing, rmsDecay, epsilon)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: private RmsProp(@JsonProperty("learningRate") double learningRate, @JsonProperty("learningRateSchedule") org.nd4j.linalg.schedule.ISchedule learningRateSchedule, @JsonProperty("rmsDecay") double rmsDecay, @JsonProperty("epsilon") double epsilon)
		Private Sub New(ByVal learningRate As Double, ByVal learningRateSchedule As ISchedule, ByVal rmsDecay As Double, ByVal epsilon As Double)
			Me.learningRate = learningRate
			Me.learningRateSchedule = learningRateSchedule
			Me.rmsDecay = rmsDecay
			Me.epsilon = epsilon
		End Sub

		Public Overridable Function stateSize(ByVal numParams As Long) As Long Implements IUpdater.stateSize
			Return numParams
		End Function

		Public Overridable Function instantiate(ByVal viewArray As INDArray, ByVal initializeViewArray As Boolean) As GradientUpdater Implements IUpdater.instantiate
			Dim u As New RmsPropUpdater(Me)
			u.setStateViewArray(viewArray, viewArray.shape(), viewArray.ordering(), initializeViewArray)
			Return u
		End Function

		Public Overridable Function instantiate(ByVal updaterState As IDictionary(Of String, INDArray), ByVal initializeStateArrays As Boolean) As GradientUpdater Implements IUpdater.instantiate
			Dim u As New RmsPropUpdater(Me)
			u.setState(updaterState, initializeStateArrays)
			Return u
		End Function

		Public Overridable Function clone() As RmsProp
			Return New RmsProp(learningRate, learningRateSchedule, rmsDecay, epsilon)
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