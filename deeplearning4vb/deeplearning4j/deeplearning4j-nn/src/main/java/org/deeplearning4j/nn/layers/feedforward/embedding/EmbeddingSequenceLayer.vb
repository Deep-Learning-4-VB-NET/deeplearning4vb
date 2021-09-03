Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports DL4JInvalidInputException = org.deeplearning4j.exception.DL4JInvalidInputException
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports org.deeplearning4j.nn.layers
Imports DefaultParamInitializer = org.deeplearning4j.nn.params.DefaultParamInitializer
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Broadcast = org.nd4j.linalg.factory.Broadcast
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives
import static org.nd4j.linalg.api.shape.Shape.hasDefaultStridesForShape

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

Namespace org.deeplearning4j.nn.layers.feedforward.embedding


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class EmbeddingSequenceLayer extends org.deeplearning4j.nn.layers.BaseLayer<org.deeplearning4j.nn.conf.layers.EmbeddingSequenceLayer>
	<Serializable>
	Public Class EmbeddingSequenceLayer
		Inherits BaseLayer(Of org.deeplearning4j.nn.conf.layers.EmbeddingSequenceLayer)

		Private Shared ReadOnly WEIGHT_DIM() As Integer = {1}

		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			MyBase.New(conf, dataType)
		End Sub

		Private indexes() As Integer

		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			assertInputSet(True)
			Dim z As INDArray = preOutput(True, workspaceMgr)
			Dim delta As INDArray = layerConf().getActivationFn().backprop(z, epsilon).getFirst() 'Shape: [mb, vector, seqLength]

			Dim ncw As Boolean = layerConf().getOutputFormat() = RNNFormat.NCW

			If maskArray_Conflict IsNot Nothing Then
				If ncw Then
					delta = Broadcast.mul(delta, maskArray_Conflict, delta, 0, 2)
				Else
					delta = Broadcast.mul(delta, maskArray_Conflict, delta, 0, 1)
				End If
			End If

			Dim inputLength As Integer = layerConf().getInputLength()
			Dim numSamples As Long = input_Conflict.size(0)
			Dim nOut As val = layerConf().getNOut()

			If delta.ordering() <> "c"c OrElse delta.View OrElse Not hasDefaultStridesForShape(delta) Then
				delta = delta.dup("c"c)
			End If

			If ncw Then
				delta = delta.permute(0, 2, 1) 'From [minibatch, nOut, length] to [minibatch, length, nOut]
			End If

			delta = delta.reshape("c"c,inputLength * numSamples, nOut)

			Dim weightGradients As INDArray = gradientViews(DefaultParamInitializer.WEIGHT_KEY)
			weightGradients.assign(0)

			If Not hasDefaultStridesForShape(input_Conflict) Then
				input_Conflict = workspaceMgr.dup(ArrayType.ACTIVATIONS, input_Conflict, "f"c)
			End If

			Dim indices As INDArray = Nd4j.createFromArray(indexes)
			Nd4j.scatterUpdate(org.nd4j.linalg.api.ops.impl.scatter.ScatterUpdate.UpdateOp.ADD, weightGradients, indices, delta, WEIGHT_DIM)

			Dim ret As Gradient = New DefaultGradient()
			ret.gradientForVariable()(DefaultParamInitializer.WEIGHT_KEY) = weightGradients

			If hasBias() Then
				Dim biasGradientsView As INDArray = gradientViews(DefaultParamInitializer.BIAS_KEY)
				delta.sum(biasGradientsView, 0) 'biasGradientView is initialized/zeroed first in sum op
				ret.gradientForVariable()(DefaultParamInitializer.BIAS_KEY) = biasGradientsView
			End If

			Return New Pair(Of Gradient, INDArray)(ret, Nothing)
		End Function

		Protected Friend Overrides Function preOutput(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			assertInputSet(False)
			If input_Conflict.rank() = 1 Then
				input_Conflict = input_Conflict.reshape(ChrW(input_Conflict.length()), 1, 1)
			End If

			If (input_Conflict.rank() = 3 AndAlso input_Conflict.size(1) <> 1) OrElse (input_Conflict.rank() <> 2 AndAlso input_Conflict.rank() <> 3) Then
				Throw New System.InvalidOperationException("Invalid input: EmbeddingSequenceLayer expects either rank 2 input of shape " & "[minibatch,seqLength] or rank 3 input of shape [minibatch,1,seqLength]. Got rank " & input_Conflict.rank() & " input of shape " & Arrays.toString(input_Conflict.shape()))
			End If

			Dim [in] As INDArray = input_Conflict
			If input_Conflict.rank() = 3 Then
				'From: [mb,1,tsLength] to [mb,tsLength]
				[in] = input_Conflict.reshape(input_Conflict.ordering(), input_Conflict.size(0), input_Conflict.size(2))
			End If

			' if inference is true, override input length config with input data columns
			Dim inferInputLength As Boolean = layerConf().isInferInputLength()
			If inferInputLength Then
				layerConf().setInputLength([in].columns())
			End If

			If [in].columns() <> layerConf().getInputLength() Then
				'Assume shape is [numExamples, inputLength], and each entry is an integer index
				Throw New DL4JInvalidInputException("Sequence length of embedding input has to be equal to the specified " & "input length: " & layerConf().getInputLength() & " i.e. we expect input shape [numExamples, inputLength] (or [numExamples, 1, inputLength] with each entry being an integer index, " & " got " & Arrays.toString(input_Conflict.shape()) & " instead, for layer with id: " & layerId())
			End If

			Dim nIn As val = layerConf().getNIn()
			Dim minibatch As val = [in].rows()
			Dim inputLength As val = layerConf().getInputLength()
			If [in].ordering() <> "c"c OrElse [in].View OrElse Not hasDefaultStridesForShape([in]) Then
				[in] = workspaceMgr.dup(ArrayType.INPUT, [in], "c"c)
			End If

			indexes = [in].data().asInt() 'C order: minibatch dimension changes least rapidly when iterating over buffer

			For i As Integer = 0 To indexes.Length - 1
				If indexes(i) < 0 OrElse indexes(i) >= nIn Then
					Throw New DL4JInvalidInputException("Invalid index for embedding layer: got index " & indexes(i) & " for entry " & i & " in minibatch; indexes must be between 0 and nIn-1 inclusive (0 to " & (nIn - 1) & ")")
				End If
			Next i

			Dim weights As INDArray = getParam(DefaultParamInitializer.WEIGHT_KEY)

			Dim nOut As val = layerConf().getNOut()
			Dim destination As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATIONS, weights.dataType(), New Long(){minibatch * inputLength, nOut}, "c"c)
			Dim rows As INDArray = Nd4j.pullRows(weights, destination, 1, indexes)

			If hasBias() Then
				Dim bias As INDArray = getParam(DefaultParamInitializer.BIAS_KEY)
				rows.addiRowVector(bias)
			End If

			Dim shape As val = New Long(){minibatch, inputLength, nOut}
			Dim ret As INDArray = rows.reshape("c"c, shape)
			If layerConf().getOutputFormat() = RNNFormat.NCW Then
				ret = ret.permute(0, 2, 1) '[minibatch, seqLen, nOut] -> [minibatch, nOut, seqLen] i.e., NWC -> NCW
			End If
			Return workspaceMgr.leverageTo(ArrayType.ACTIVATIONS, ret)
		End Function

		Public Overrides Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			Dim rows As INDArray = preOutput(training, workspaceMgr)

			Dim ret As INDArray = layerConf().getActivationFn().getActivation(rows, training)
			If maskArray_Conflict IsNot Nothing Then
				If maskArray_Conflict.rank() <> 2 OrElse (input_Conflict.rank() = 2 AndAlso Not maskArray_Conflict.equalShapes(input_Conflict)) OrElse (input_Conflict.rank() = 3 AndAlso (input_Conflict.size(0) <> maskArray_Conflict.size(0) OrElse input_Conflict.size(2) <> maskArray_Conflict.size(1))) Then
					Throw New System.InvalidOperationException("Mask array for EmbeddingSequenceLayer (when defined) must be rank 2 and" & "have shape equal to input shape (when input is rank 2, shape [mb,tsLength]) or equal to input dimensions 0 and" & " 2 (when input is rank 3, shape [mb,1,tsLength]). Input shape: " & Arrays.toString(input_Conflict.shape()) & ", mask shape: " & Arrays.toString(maskArray_Conflict.shape()))
				End If
				Dim ncw As Boolean = layerConf().getOutputFormat() = RNNFormat.NCW
				If ncw Then
					'Returned array: rank 3, shape [mb, vector, seqLength]. mask shape: [mb, seqLength]
					Broadcast.mul(ret, maskArray_Conflict.castTo(ret.dataType()), ret, 0, 2)
				Else
					'Returned array: rank 3, shape [mb, seqLength, vector]. mask shape: [mb, seqLength]
					Broadcast.mul(ret, maskArray_Conflict.castTo(ret.dataType()), ret, 0, 1)
				End If
			End If
			Return ret
		End Function

		Public Overrides Function hasBias() As Boolean
			Return layerConf().hasBias()
		End Function

		Public Overrides ReadOnly Property PretrainLayer As Boolean
			Get
				Return False
			End Get
		End Property

		Protected Friend Overrides Sub applyDropOutIfNecessary(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr)
			Throw New System.NotSupportedException("Dropout not supported with EmbeddingLayer " & layerId())
		End Sub


		Public Overrides Function type() As Type
			Return Type.RECURRENT
		End Function

		Public Overrides Sub clear()
			MyBase.clear()
			indexes = Nothing
		End Sub
	End Class

End Namespace