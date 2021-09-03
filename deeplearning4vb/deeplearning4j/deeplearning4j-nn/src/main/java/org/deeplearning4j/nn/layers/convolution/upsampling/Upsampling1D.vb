Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports DL4JInvalidInputException = org.deeplearning4j.exception.DL4JInvalidInputException
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports BaseUpsamplingLayer = org.deeplearning4j.nn.conf.layers.BaseUpsamplingLayer
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports CustomOp = org.nd4j.linalg.api.ops.CustomOp
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr

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
'ORIGINAL LINE: @Slf4j public class Upsampling1D extends Upsampling2D
	<Serializable>
	Public Class Upsampling1D
		Inherits Upsampling2D


		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			MyBase.New(conf, dataType)
		End Sub

		Protected Friend Overrides ReadOnly Property Format As CNN2DFormat
			Get
				Return CNN2DFormat.NCHW
			End Get
		End Property

		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			assertInputSet(True)

			Dim size() As Integer = CType(layerConf(), BaseUpsamplingLayer).getSize()
			epsilon = epsilon.reshape(ChrW(epsilon.size(0)), epsilon.size(1), epsilon.size(2), 1)
			' we replicate the error term times "size" so that backprop works properly on it
			epsilon = epsilon.repeat(3, size(0))

			Dim originalInput As INDArray = input
			input = input.castTo(dataType).reshape(input.size(0), input.size(1), input.size(2), 1)

			Dim miniBatch As Long = input.size(0)
			Dim inDepth As Long = input.size(1)
			Dim inH As Long = input.size(2)
			Dim inW As Long = input.size(3)


			Dim outEpsilon As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATION_GRAD, input.dataType(), miniBatch * inDepth * inH * inW)
			Dim reshapedEpsilon As INDArray = outEpsilon.reshape("c"c, miniBatch, inDepth, inH, inW)

			Dim intArgs() As Integer = {1} ' 1 is for NCHW

			Dim op As CustomOp = DynamicCustomOp.builder("upsampling_bp").addIntegerArguments(intArgs).addInputs(input, epsilon).addOutputs(reshapedEpsilon).callInplace(False).build()
			Nd4j.Executioner.exec(op)

			Dim gradient As Gradient = New DefaultGradient()

			reshapedEpsilon = reshapedEpsilon.slice(0, 3)
			input = originalInput

			' Since we aggregate the gradient across "size" slices, we need to normalize afterwards.
			Return New Pair(Of Gradient, INDArray)(gradient, reshapedEpsilon.divi(size(0)))
		End Function

		Protected Friend Overrides ReadOnly Property Size As Integer()
			Get
				Return CType(conf.getLayer(), org.deeplearning4j.nn.conf.layers.Upsampling1D).getSize()
			End Get
		End Property

		Public Overrides Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			assertInputSet(False)
			If input.rank() <> 3 Then
				Throw New DL4JInvalidInputException("Got rank " & input.rank() & " array as input to Subsampling1DLayer with shape " & Arrays.toString(input.shape()) & ". Expected rank 3 array with shape [minibatchSize, features, length]. " & layerId())
			End If

			' add singleton fourth dimension to input
			Dim origInput As INDArray = input
			input = input.castTo(dataType).reshape(input.size(0), input.size(1), input.size(2), 1)

			' call 2D SubsamplingLayer's activate method
			Dim acts As INDArray = MyBase.activate(training, workspaceMgr)

			' remove singleton fourth dimension from input and output activations
			input = origInput
			acts = acts.reshape(ChrW(acts.size(0)), acts.size(1), acts.size(2))

			Return acts
		End Function

		Protected Friend Overrides Function preOutput(ByVal training As Boolean, ByVal forBackprop As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			Dim originalInput As INDArray = input
			input = input.reshape(input.size(0), input.size(1), input.size(2), 1)

'JAVA TO VB CONVERTER NOTE: The local variable preOutput was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim preOutput_Conflict As INDArray = MyBase.preOutput(training, forBackprop, workspaceMgr)

			input = originalInput
			preOutput_Conflict = preOutput_Conflict.slice(0, 3)

			Return preOutput_Conflict
		End Function


	End Class

End Namespace