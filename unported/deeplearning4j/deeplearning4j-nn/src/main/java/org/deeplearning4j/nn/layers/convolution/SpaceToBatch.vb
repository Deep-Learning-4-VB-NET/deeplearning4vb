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
'ORIGINAL LINE: @Slf4j public class SpaceToBatch extends org.deeplearning4j.nn.layers.AbstractLayer<org.deeplearning4j.nn.conf.layers.SpaceToBatchLayer>
	<Serializable>
	Public Class SpaceToBatch
		Inherits AbstractLayer(Of org.deeplearning4j.nn.conf.layers.SpaceToBatchLayer)

		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			MyBase.New(conf, dataType)
		End Sub

		Private ReadOnly Property Blocks As Integer()
			Get
				Return layerConf().getBlocks()
			End Get
		End Property

		Private ReadOnly Property Padding As Integer()()
			Get
				Return layerConf().getPadding()
			End Get
		End Property

		Private ReadOnly Property BlocksArray As INDArray
			Get
				Dim intBlocks() As Integer = layerConf().getBlocks()
				Return Nd4j.createFromArray(intBlocks)
			End Get
		End Property

		Private ReadOnly Property PaddingArray As INDArray
			Get
				Dim intPad()() As Integer = layerConf().getPadding()
				Return Nd4j.createFromArray(intPad)
			End Get
		End Property


		Public Overrides Function type() As Type
			Return Type.CONVOLUTIONAL
		End Function


		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			assertInputSet(True)

			Dim input As INDArray = Me.input_Conflict.castTo(dataType) 'Cast to network dtype if required (no-op if already correct type)

			Dim nchw As Boolean = layerConf().getFormat() = CNN2DFormat.NCHW

			Dim outEpsilon As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATION_GRAD, input.dataType(), input.shape(), "c"c)

			Dim gradient As Gradient = New DefaultGradient()

			Dim epsilonNHWC As INDArray = If(nchw, epsilon.permute(0, 2, 3, 1), epsilon)
			Dim outEpsilonNHWC As INDArray = If(nchw, outEpsilon.permute(0, 2, 3, 1), outEpsilon)

			Dim op As CustomOp = DynamicCustomOp.builder("batch_to_space_nd").addInputs(epsilonNHWC, BlocksArray, PaddingArray).addOutputs(outEpsilonNHWC).callInplace(False).build()
			Nd4j.exec(op)

			outEpsilon = backpropDropOutIfPresent(outEpsilon)
			Return New Pair(Of Gradient, INDArray)(gradient, outEpsilon)
		End Function

		Protected Friend Overridable Function preOutput(ByVal training As Boolean, ByVal forBackprop As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			assertInputSet(False)
			applyDropOutIfNecessary(training, Nothing)

			If input_Conflict.rank() <> 4 Then
				Throw New DL4JInvalidInputException("Got rank " & input_Conflict.rank() & " array as input to space to batch with shape " & Arrays.toString(input_Conflict.shape()) & ". Expected rank 4 array with shape " & layerConf().getFormat().dimensionNames() & ". " & layerId())
			End If

			If preOutput IsNot Nothing AndAlso forBackprop Then
				Return preOutput
			End If

			Dim nchw As Boolean = layerConf().getFormat() = CNN2DFormat.NCHW

			Dim inMiniBatch As Long = input_Conflict.size(0)
			Dim depth As Long = input_Conflict.size(If(nchw, 1, 3))
			Dim inH As Long = input_Conflict.size(If(nchw, 2, 1))
			Dim inW As Long = input_Conflict.size(If(nchw, 3, 2))

			Dim blocks() As Integer = Me.Blocks
			Dim padding()() As Integer = Me.Padding

			Dim paddedH As Long = inH + padding(0)(0) + padding(0)(1)
			Dim paddedW As Long = inW + padding(1)(0) + padding(1)(1)

			Dim outH As Long = paddedH \ blocks(0)
			Dim outW As Long = paddedW \ blocks(1)
			Dim outMiniBatch As Long = inMiniBatch * blocks(0) * blocks(1)

			Dim outShape() As Long = If(nchw, New Long(){outMiniBatch, depth, outH, outW}, New Long()){outMiniBatch, outH, outW, depth}

			Dim [out] As INDArray = workspaceMgr.create(ArrayType.ACTIVATIONS, input_Conflict.dataType(), outShape, "c"c)

			Dim inNHWC As INDArray = If(nchw, input_Conflict.permute(0, 2, 3, 1), input_Conflict)
			Dim outNHWC As INDArray = If(nchw, [out].permute(0, 2, 3, 1), [out])

			Dim op As CustomOp = DynamicCustomOp.builder("space_to_batch_nd").addInputs(inNHWC, BlocksArray, PaddingArray).addOutputs(outNHWC).build()
			Nd4j.exec(op)

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