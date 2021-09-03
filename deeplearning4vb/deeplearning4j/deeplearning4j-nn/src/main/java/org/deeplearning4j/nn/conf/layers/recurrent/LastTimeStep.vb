Imports System
Imports System.Collections.Generic
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports Layer = org.deeplearning4j.nn.conf.layers.Layer
Imports BaseWrapperLayer = org.deeplearning4j.nn.conf.layers.wrapper.BaseWrapperLayer
Imports LastTimeStepLayer = org.deeplearning4j.nn.layers.recurrent.LastTimeStepLayer
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

Namespace org.deeplearning4j.nn.conf.layers.recurrent


	<Serializable>
	Public Class LastTimeStep
		Inherits BaseWrapperLayer

		Private Sub New()
		End Sub

		Public Sub New(ByVal underlying As Layer)
			MyBase.New(underlying)
			Me.layerName = underlying.LayerName ' needed for keras import to match names
		End Sub

		Public Overridable ReadOnly Property Underlying As Layer
			Get
				Return underlying
			End Get
		End Property


		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As org.deeplearning4j.nn.api.Layer
			Dim conf2 As NeuralNetConfiguration = conf.clone()
			conf2.setLayer(CType(conf2.getLayer(), LastTimeStep).Underlying)
			Return New LastTimeStepLayer(underlying.instantiate(conf2, trainingListeners, layerIndex, layerParamsView, initializeParams, networkDataType))
		End Function

		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			If inputType.getType() <> InputType.Type.RNN Then
				Throw New System.ArgumentException("Require RNN input type - got " & inputType)
			End If
			Dim outType As InputType = underlying.getOutputType(layerIndex, inputType)
			Dim r As InputType.InputTypeRecurrent = DirectCast(outType, InputType.InputTypeRecurrent)
			Return InputType.feedForward(r.getSize())
		End Function
	End Class

End Namespace