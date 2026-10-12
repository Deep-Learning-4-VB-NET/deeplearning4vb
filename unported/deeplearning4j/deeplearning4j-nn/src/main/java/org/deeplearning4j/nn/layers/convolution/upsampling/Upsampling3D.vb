Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports DL4JInvalidInputException = org.deeplearning4j.exception.DL4JInvalidInputException
Imports CacheMode = org.deeplearning4j.nn.conf.CacheMode
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports org.deeplearning4j.nn.layers
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports CustomOp = org.nd4j.linalg.api.ops.CustomOp
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.deeplearning4j.nn.layers.convolution.upsampling



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class Upsampling3D extends org.deeplearning4j.nn.layers.AbstractLayer<org.deeplearning4j.nn.conf.layers.Upsampling3D>
	<Serializable>
	Public Class Upsampling3D
		Inherits AbstractLayer(Of org.deeplearning4j.nn.conf.layers.Upsampling3D)


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

			Dim ncdhw As Boolean = layerConf().getDataFormat() = org.deeplearning4j.nn.conf.layers.Convolution3D.DataFormat.NCDHW
			' Assumes NCDHW order
			Dim miniBatch As Long = input_Conflict.size(0)
			Dim inChannels, inD, inH, inW As Long
			Dim intArgs() As Integer
			If ncdhw Then
				inChannels = input_Conflict.size(1)
				inD = input_Conflict.size(2)
				inH = input_Conflict.size(3)
				inW = input_Conflict.size(4)
				intArgs = New Integer() {1} ' 1 is channels first
			Else
				inD = input_Conflict.size(1)
				inH = input_Conflict.size(2)
				inW = input_Conflict.size(3)
				inChannels = input_Conflict.size(4)
				intArgs = New Integer() {0} ' 0 is channels last
			End If



			Dim epsOut As INDArray
			If ncdhw Then
				epsOut = workspaceMgr.createUninitialized(ArrayType.ACTIVATION_GRAD, epsilon.dataType(), New Long(){miniBatch, inChannels, inD, inH, inW}, "c"c)
			Else
				epsOut = workspaceMgr.createUninitialized(ArrayType.ACTIVATION_GRAD, epsilon.dataType(), New Long(){miniBatch, inD, inH, inW, inChannels}, "c"c)
			End If


			Dim gradient As Gradient = New DefaultGradient()

			Dim op As CustomOp = DynamicCustomOp.builder("upsampling3d_bp").addIntegerArguments(intArgs).addInputs(input_Conflict, epsilon).addOutputs(epsOut).callInplace(False).build()
			Nd4j.Executioner.exec(op)

			epsOut = backpropDropOutIfPresent(epsOut)
			Return New Pair(Of Gradient, INDArray)(gradient, epsOut)
		End Function

		Protected Friend Overridable ReadOnly Property Size As Integer()
			Get
				Return layerConf().getSize()
			End Get
		End Property

		Protected Friend Overridable Function preOutput(ByVal training As Boolean, ByVal forBackprop As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			assertInputSet(False)
			applyDropOutIfNecessary(training, workspaceMgr)

			If input_Conflict.rank() <> 5 Then
				Throw New DL4JInvalidInputException("Got rank " & input_Conflict.rank() & " array as input to Upsampling3DLayer with shape " & Arrays.toString(input_Conflict.shape()) & ". Expected rank 5 array with shape " & "[minibatchSize, channels, inputDepth, inputHeight, inputWidth]. " & layerId())
			End If

			If preOutput IsNot Nothing AndAlso forBackprop Then
				Return preOutput
			End If

			Dim ncdhw As Boolean = layerConf().getDataFormat() = org.deeplearning4j.nn.conf.layers.Convolution3D.DataFormat.NCDHW
			Dim miniBatch As Long = input_Conflict.size(0)
			Dim inChannels, inD, inH, inW As Long
			Dim intArgs() As Integer
			Dim size() As Integer = Me.Size
			If ncdhw Then
				inChannels = CInt(input_Conflict.size(1))
				inD = CInt(input_Conflict.size(2))
				inH = CInt(input_Conflict.size(3))
				inW = CInt(input_Conflict.size(4))
				intArgs = New Integer() {size(0), size(1), size(2), 1} ' 1 is channels first
			Else
				inD = CInt(input_Conflict.size(1))
				inH = CInt(input_Conflict.size(2))
				inW = CInt(input_Conflict.size(3))
				inChannels = CInt(input_Conflict.size(4))
				intArgs = New Integer() {size(0), size(1), size(2), 0} ' 0 is channels last
			End If


			Dim outD As Long = inD * size(0)
			Dim outH As Long = inH * size(1)
			Dim outW As Long = inW * size(2)

			Dim output As INDArray
			If ncdhw Then
				output = workspaceMgr.createUninitialized(ArrayType.ACTIVATIONS, input_Conflict.dataType(), New Long(){miniBatch, inChannels, outD, outH, outW}, "c"c)
			Else
				output = workspaceMgr.createUninitialized(ArrayType.ACTIVATIONS, input_Conflict.dataType(), New Long(){miniBatch, outD, outH, outW, inChannels}, "c"c)
			End If



			Dim upsampling As CustomOp = DynamicCustomOp.builder("upsampling3d").addIntegerArguments(intArgs).addInputs(input_Conflict).addOutputs(output).callInplace(False).build()
			Nd4j.Executioner.exec(upsampling)

			Return output
		End Function

		Public Overrides Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			assertInputSet(False)
			applyDropOutIfNecessary(training, workspaceMgr)

			If cacheMode_Conflict = Nothing Then
				cacheMode_Conflict = CacheMode.NONE
			End If

			Dim z As INDArray = preOutput(training, False, workspaceMgr)

			' we do cache only if cache workspace exists. Skip otherwise
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