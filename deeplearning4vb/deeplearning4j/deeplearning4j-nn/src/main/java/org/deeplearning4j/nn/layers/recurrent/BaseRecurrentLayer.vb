Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports RecurrentLayer = org.deeplearning4j.nn.api.layers.RecurrentLayer
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.layers
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
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

Namespace org.deeplearning4j.nn.layers.recurrent


	<Serializable>
	Public MustInherit Class BaseRecurrentLayer(Of LayerConfT As org.deeplearning4j.nn.conf.layers.BaseRecurrentLayer)
		Inherits BaseLayer(Of LayerConfT)
		Implements RecurrentLayer

		Public MustOverride Function tbpttBackpropGradient(ByVal epsilon As INDArray, ByVal tbpttBackLength As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) As org.nd4j.common.primitives.Pair(Of org.deeplearning4j.nn.gradient.Gradient, INDArray)
		Public MustOverride Function rnnActivateUsingStoredState(ByVal input As INDArray, ByVal training As Boolean, ByVal storeLastForTBPTT As Boolean, ByVal workspaceMg As LayerWorkspaceMgr) As INDArray
		Public MustOverride Function rnnTimeStep(ByVal input As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements RecurrentLayer.rnnTimeStep
		Public Overrides MustOverride ReadOnly Property PretrainLayer As Boolean
		Public Overrides MustOverride Property IterationCount As Integer

		''' <summary>
		''' stateMap stores the INDArrays needed to do rnnTimeStep() forward pass.
		''' </summary>
		Protected Friend stateMap As IDictionary(Of String, INDArray) = New ConcurrentDictionary(Of String, INDArray)()

		''' <summary>
		''' State map for use specifically in truncated BPTT training. Whereas stateMap contains the
		''' state from which forward pass is initialized, the tBpttStateMap contains the state at the
		''' end of the last truncated bptt
		''' </summary>
		Protected Friend tBpttStateMap As IDictionary(Of String, INDArray) = New ConcurrentDictionary(Of String, INDArray)()

		Protected Friend helperCountFail As Integer = 0

		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			MyBase.New(conf, dataType)
		End Sub

		''' <summary>
		''' Returns a shallow copy of the stateMap
		''' </summary>
		Public Overridable Function rnnGetPreviousState() As IDictionary(Of String, INDArray) Implements RecurrentLayer.rnnGetPreviousState
			Return New Dictionary(Of String, INDArray)(stateMap)
		End Function

		''' <summary>
		''' Set the state map. Values set using this method will be used
		''' in next call to rnnTimeStep()
		''' </summary>
		Public Overridable Sub rnnSetPreviousState(ByVal stateMap As IDictionary(Of String, INDArray)) Implements RecurrentLayer.rnnSetPreviousState
			Me.stateMap.Clear()
			Me.stateMap.PutAll(stateMap)
		End Sub

		''' <summary>
		''' Reset/clear the stateMap for rnnTimeStep() and tBpttStateMap for rnnActivateUsingStoredState()
		''' </summary>
		Public Overridable Sub rnnClearPreviousState() Implements RecurrentLayer.rnnClearPreviousState
			stateMap.Clear()
			tBpttStateMap.Clear()
		End Sub

		Public Overridable Function rnnGetTBPTTState() As IDictionary(Of String, INDArray) Implements RecurrentLayer.rnnGetTBPTTState
			Return New Dictionary(Of String, INDArray)(tBpttStateMap)
		End Function

		Public Overridable Sub rnnSetTBPTTState(ByVal state As IDictionary(Of String, INDArray)) Implements RecurrentLayer.rnnSetTBPTTState
			tBpttStateMap.Clear()
			tBpttStateMap.PutAll(state)
		End Sub

		Public Overridable ReadOnly Property DataFormat As RNNFormat
			Get
				Return layerConf().getRnnDataFormat()
			End Get
		End Property

		Protected Friend Overridable Function permuteIfNWC(ByVal arr As INDArray) As INDArray
			If arr Is Nothing Then
				Return Nothing
			End If
			If DataFormat = RNNFormat.NWC Then
				Return arr.permute(0, 2, 1)
			End If
			Return arr
		End Function


	End Class

End Namespace