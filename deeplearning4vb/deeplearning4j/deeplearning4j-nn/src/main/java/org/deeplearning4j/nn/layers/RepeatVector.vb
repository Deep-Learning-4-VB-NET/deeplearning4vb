Imports System
Imports DL4JInvalidInputException = org.deeplearning4j.exception.DL4JInvalidInputException
Imports CacheMode = org.deeplearning4j.nn.conf.CacheMode
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
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



	<Serializable>
	Public Class RepeatVector
		Inherits AbstractLayer(Of org.deeplearning4j.nn.conf.layers.misc.RepeatVector)

		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			MyBase.New(conf, dataType)
		End Sub

		Public Overrides Function calcRegularizationScore(ByVal backpropParamsOnly As Boolean) As Double
			Return 0
		End Function

		Public Overrides Function type() As Type
			Return Type.UPSAMPLING
		End Function


		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			assertInputSet(True)

			If epsilon.dataType() <> dataType Then
				epsilon = epsilon.castTo(dataType)
			End If

			Dim outEpsilon As INDArray
			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = workspaceMgr.notifyScopeBorrowed(org.deeplearning4j.nn.workspace.ArrayType.ACTIVATION_GRAD)
				If layerConf().getDataFormat() = RNNFormat.NCW Then
					outEpsilon = epsilon.sum(2)
				Else
					outEpsilon = epsilon.sum(1)
				End If
			End Using

			Dim gradient As Gradient = New DefaultGradient()
			Return New Pair(Of Gradient, INDArray)(gradient, outEpsilon)
		End Function

		Protected Friend Overridable ReadOnly Property N As Integer
			Get
				Return layerConf().getN()
			End Get
		End Property

		Protected Friend Overridable Function preOutput(ByVal training As Boolean, ByVal forBackprop As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			assertInputSet(False)
			applyDropOutIfNecessary(training, workspaceMgr)

			If input_Conflict.rank() <> 2 Then
				Throw New DL4JInvalidInputException("Got rank " & input_Conflict.rank() & " array as input to RepeatVector with shape " & Arrays.toString(input_Conflict.shape()) & ". Expected rank 2 array with shape [minibatchSize, size]. " & layerId())
			End If

			If preOutput IsNot Nothing AndAlso forBackprop Then
				Return preOutput
			End If

			Dim miniBatch As Long = input_Conflict.size(0)
			Dim size As Long = input_Conflict.size(1)
			If DataFormat = RNNFormat.NCW Then
				Dim output As INDArray = input_Conflict.reshape(ChrW(miniBatch), size, 1).castTo(dataType)
				Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = workspaceMgr.notifyScopeBorrowed(org.deeplearning4j.nn.workspace.ArrayType.ACTIVATIONS)
					Return output.repeat(2, CLng(N))
				End Using
			Else
				Dim output As INDArray = input_Conflict.reshape(ChrW(miniBatch), 1, size).castTo(dataType)
				Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = workspaceMgr.notifyScopeBorrowed(org.deeplearning4j.nn.workspace.ArrayType.ACTIVATIONS)
					Return output.repeat(1, CLng(N))
				End Using
			End If
		End Function

		Public Overridable ReadOnly Property DataFormat As RNNFormat
			Get
				Return layerConf().getDataFormat()
			End Get
		End Property

		Public Overrides Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			assertInputSet(False)

			If cacheMode_Conflict = Nothing Then
				cacheMode_Conflict = CacheMode.NONE
			End If

			Dim z As INDArray = preOutput(training, False, workspaceMgr)
			If training AndAlso cacheMode_Conflict <> CacheMode.NONE AndAlso workspaceMgr.hasConfiguration(ArrayType.FF_CACHE) AndAlso workspaceMgr.isWorkspaceOpen(ArrayType.FF_CACHE) Then
				Using wsB As org.nd4j.linalg.api.memory.MemoryWorkspace = workspaceMgr.notifyScopeBorrowed(org.deeplearning4j.nn.workspace.ArrayType.FF_CACHE)
					preOutput = z.unsafeDuplication()
				End Using
			End If
			Return z
		End Function

		Public Overrides ReadOnly Property PretrainLayer As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides Sub clearNoiseWeightParams()
			'No op
		End Sub

		Public Overrides Function gradient() As Gradient
			Throw New System.NotSupportedException("Not supported - no parameters")
		End Function

		Public Overrides Sub fit()

		End Sub

		Public Overrides Function numParams() As Long
			Return 0
		End Function

		Public Overrides Sub fit(ByVal input As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr)
			Throw New System.NotSupportedException("Not supported")
		End Sub

		Public Overrides Function score() As Double
			Return 0
		End Function

		Public Overrides Sub update(ByVal gradient As INDArray, ByVal paramType As String)

		End Sub

		Public Overrides Function params() As INDArray
			Return Nothing
		End Function

		Public Overrides Function getParam(ByVal param As String) As INDArray
			Return params()
		End Function

		Public Overrides WriteOnly Property Params As INDArray
			Set(ByVal params As INDArray)
    
			End Set
		End Property

	End Class

End Namespace