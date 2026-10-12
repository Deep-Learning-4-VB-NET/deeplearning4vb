Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports DL4JInvalidInputException = org.deeplearning4j.exception.DL4JInvalidInputException
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports org.deeplearning4j.nn.layers
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports CustomOp = org.nd4j.linalg.api.ops.CustomOp
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType

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

Namespace org.deeplearning4j.nn.layers.convolution



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class SpaceToDepth extends org.deeplearning4j.nn.layers.AbstractLayer<org.deeplearning4j.nn.conf.layers.SpaceToDepthLayer>
	<Serializable>
	Public Class SpaceToDepth
		Inherits AbstractLayer(Of org.deeplearning4j.nn.conf.layers.SpaceToDepthLayer)

		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			MyBase.New(conf, dataType)
		End Sub

		Private ReadOnly Property BlockSize As Integer
			Get
				Return layerConf().getBlockSize()
			End Get
		End Property

		Public Overrides Function type() As Type
			Return Type.CONVOLUTIONAL
		End Function


		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			assertInputSet(True)

			Dim input As INDArray = Me.input_Conflict.castTo(epsilon.dataType())

			Dim nchw As Boolean = layerConf().getDataFormat() = CNN2DFormat.NCHW
			Dim miniBatch As Long = input.size(0)
			Dim inDepth As Long = input.size(If(nchw, 1, 3))
			Dim inH As Long = input.size(If(nchw, 2, 1))
			Dim inW As Long = input.size(If(nchw, 3, 2))

			Dim epsShape() As Long = If(nchw, New Long(){miniBatch, inDepth, inH, inW}, New Long()){miniBatch, inH, inW, inDepth}
			Dim outEpsilon As INDArray = workspaceMgr.create(ArrayType.ACTIVATION_GRAD, input.dataType(), epsShape, "c"c)

			Dim gradient As Gradient = New DefaultGradient()

			Dim blockSize As Integer = Me.BlockSize

			'Workaround for issue: https://github.com/eclipse/deeplearning4j/issues/8859
			If Not Shape.hasDefaultStridesForShape(epsilon) Then
				epsilon = epsilon.dup("c"c)
			End If

			Dim op As CustomOp = DynamicCustomOp.builder("depth_to_space").addInputs(epsilon).addIntegerArguments(blockSize,If(nchw, 0, 1)).addOutputs(outEpsilon).build()
			Nd4j.Executioner.exec(op)

			Return New Pair(Of Gradient, INDArray)(gradient, outEpsilon)
		End Function

		Protected Friend Overridable Function preOutput(ByVal training As Boolean, ByVal forBackprop As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			assertInputSet(False)
			applyDropOutIfNecessary(training, workspaceMgr)

			If input_Conflict.rank() <> 4 Then
				Throw New DL4JInvalidInputException("Got rank " & input_Conflict.rank() & " array as input to space to channels with shape " & Arrays.toString(input_Conflict.shape()) & ". Expected rank 4 array with shape " & layerConf().getDataFormat().dimensionNames() & ". " & layerId())
			End If

			If preOutput IsNot Nothing AndAlso forBackprop Then
				Return preOutput
			End If

			Dim nchw As Boolean = layerConf().getDataFormat() = CNN2DFormat.NCHW

			Dim miniBatch As Long = input_Conflict.size(0)
			Dim depth As Long = input_Conflict.size(If(nchw, 1, 3))
			Dim inH As Long = input_Conflict.size(If(nchw, 2, 1))
			Dim inW As Long = input_Conflict.size(If(nchw, 3, 2))

			Dim blockSize As Integer = Me.BlockSize

			Dim outH As Long = inH \ blockSize
			Dim outW As Long = inW \ blockSize
			Dim outDepth As Long = depth * blockSize * blockSize

			Dim outShape() As Long = If(nchw, New Long(){miniBatch, outDepth, outH, outW}, New Long()){miniBatch, outH, outW, outDepth}
			Dim [out] As INDArray = workspaceMgr.create(ArrayType.ACTIVATIONS, input_Conflict.dataType(), outShape, "c"c)

			'Workaround for issue: https://github.com/eclipse/deeplearning4j/issues/8859
			Dim input As INDArray = Me.input_Conflict
			If Not Shape.hasDefaultStridesForShape(input) Then
				input = input.dup("c"c)
			End If

			Dim op As CustomOp = DynamicCustomOp.builder("space_to_depth").addInputs(input).addIntegerArguments(blockSize,If(nchw, 0, 1)).addOutputs([out]).build()
			Nd4j.Executioner.exec(op)

			Return [out]
		End Function

		Public Overrides Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			Return preOutput(training, False, workspaceMgr)
		End Function


		Public Overrides Function calcRegularizationScore(ByVal backpropParamsOnly As Boolean) As Double
			Return 0
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

		Public Overrides Function numParams() As Long
			Return 0
		End Function

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