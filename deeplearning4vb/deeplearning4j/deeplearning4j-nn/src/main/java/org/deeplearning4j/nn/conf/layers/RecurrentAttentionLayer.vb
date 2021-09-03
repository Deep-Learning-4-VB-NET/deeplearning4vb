Imports System
Imports System.Collections.Generic
Imports lombok
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports SDLayerParams = org.deeplearning4j.nn.conf.layers.samediff.SDLayerParams
Imports SameDiffLayer = org.deeplearning4j.nn.conf.layers.samediff.SameDiffLayer
Imports SameDiffLayerUtils = org.deeplearning4j.nn.conf.layers.samediff.SameDiffLayerUtils
Imports SimpleRnnParamInitializer = org.deeplearning4j.nn.params.SimpleRnnParamInitializer
Imports WeightInitUtil = org.deeplearning4j.nn.weights.WeightInitUtil
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports Activation = org.nd4j.linalg.activations.Activation
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.deeplearning4j.nn.conf.layers


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @EqualsAndHashCode(callSuper = true) public class RecurrentAttentionLayer extends org.deeplearning4j.nn.conf.layers.samediff.SameDiffLayer
	<Serializable>
	Public Class RecurrentAttentionLayer
		Inherits SameDiffLayer

		Private nIn As Long
		Private nOut As Long
		Private nHeads As Integer
		Private headSize As Long
		Private projectInput As Boolean
		Private activation As Activation
		Private hasBias As Boolean

		Private Const WEIGHT_KEY_QUERY_PROJECTION As String = "Wq"
		Private Const WEIGHT_KEY_KEY_PROJECTION As String = "Wk"
		Private Const WEIGHT_KEY_VALUE_PROJECTION As String = "Wv"
		Private Const WEIGHT_KEY_OUT_PROJECTION As String = "Wo"
		Private Const WEIGHT_KEY As String = SimpleRnnParamInitializer.WEIGHT_KEY
		Private Const BIAS_KEY As String = SimpleRnnParamInitializer.BIAS_KEY
		Private Const RECURRENT_WEIGHT_KEY As String = SimpleRnnParamInitializer.RECURRENT_WEIGHT_KEY
		Private timeSteps As Integer

		Private Sub New()
		End Sub

		Protected Friend Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			nIn = builder.nIn_Conflict
			nOut = builder.nOut_Conflict
			nHeads = builder.nHeads_Conflict
			headSize = If(builder.headSize_Conflict = 0, nOut \ nHeads, builder.headSize_Conflict)
			projectInput = builder.projectInput_Conflict
			activation = builder.activation_Conflict
			hasBias = builder.hasBias_Conflict
		End Sub

		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			Return InputTypeUtil.getPreprocessorForInputTypeRnnLayers(inputType, RNNFormat.NCW, LayerName)
		End Function

		Public Overrides Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)
			If inputType Is Nothing OrElse inputType.getType() <> InputType.Type.RNN Then
				Throw New System.InvalidOperationException("Invalid input for Recurrent Attention layer (layer name = """ & LayerName & """): expect RNN input type with size > 0. Got: " & inputType)
			End If

			If nIn <= 0 OrElse override Then
				Dim r As InputType.InputTypeRecurrent = DirectCast(inputType, InputType.InputTypeRecurrent)
				Me.nIn = r.getSize()
			End If
		End Sub

		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			If inputType Is Nothing OrElse inputType.getType() <> InputType.Type.RNN Then
				Throw New System.InvalidOperationException("Invalid input for Recurrent Attention layer (layer index = " & layerIndex & ", layer name = """ & LayerName & """): expect RNN input type with size > 0. Got: " & inputType)
			End If

			Dim itr As InputType.InputTypeRecurrent = DirectCast(inputType, InputType.InputTypeRecurrent)


			Return InputType.recurrent(nOut, itr.getTimeSeriesLength())
		End Function

		Public Overrides Sub defineParameters(ByVal params As SDLayerParams)
			params.clear()

			params.addWeightParam(WEIGHT_KEY, nIn, nOut)
			params.addWeightParam(RECURRENT_WEIGHT_KEY, nOut, nOut)
			If hasBias Then
				params.addBiasParam(BIAS_KEY, nOut)
			End If

			If projectInput Then
				params.addWeightParam(WEIGHT_KEY_QUERY_PROJECTION, nHeads, headSize, nOut)
				params.addWeightParam(WEIGHT_KEY_KEY_PROJECTION, nHeads, headSize, nIn)
				params.addWeightParam(WEIGHT_KEY_VALUE_PROJECTION, nHeads, headSize, nIn)
				params.addWeightParam(WEIGHT_KEY_OUT_PROJECTION, nHeads * headSize, nOut)
			End If
		End Sub

		Public Overrides Sub initializeParameters(ByVal params As IDictionary(Of String, INDArray))
			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.scopeOutOfWorkspaces()
				For Each e As KeyValuePair(Of String, INDArray) In params.SetOfKeyValuePairs()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String keyName = e.getKey();
					Dim keyName As String = e.Key
					Select Case keyName
						Case WEIGHT_KEY
							WeightInitUtil.initWeights(nIn, nOut, e.Value.shape(), weightInit, Nothing, "c"c, e.Value)
						Case RECURRENT_WEIGHT_KEY
							WeightInitUtil.initWeights(nOut, nOut, e.Value.shape(), weightInit, Nothing, "c"c, e.Value)
						Case BIAS_KEY
							e.Value.assign(0)
						Case WEIGHT_KEY_OUT_PROJECTION
							WeightInitUtil.initWeights(nIn, headSize, e.Value.shape(), weightInit, Nothing, "c"c, e.Value)
						Case Else
							WeightInitUtil.initWeights(nHeads * headSize, nOut, e.Value.shape(), weightInit, Nothing, "c"c, e.Value)
					End Select
				Next e
			End Using
		End Sub

		Public Overrides Sub applyGlobalConfigToLayer(ByVal globalConfig As NeuralNetConfiguration.Builder)
			If activation = Nothing Then
				activation = SameDiffLayerUtils.fromIActivation(globalConfig.getActivationFn())
			End If
		End Sub

		Public Overrides Sub validateInput(ByVal input As INDArray)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final long inputLength = input.size(2);
			Dim inputLength As Long = input.size(2)
			Preconditions.checkArgument(inputLength = CLng(Me.timeSteps), "This layer only supports fixed length mini-batches. Expected %s time steps but got %s.", Me.timeSteps, inputLength)
		End Sub

		Public Overrides Function defineLayer(ByVal sameDiff As SameDiff, ByVal layerInput As SDVariable, ByVal paramTable As IDictionary(Of String, SDVariable), ByVal mask As SDVariable) As SDVariable
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final val W = paramTable.get(WEIGHT_KEY);
			Dim W As val = paramTable(WEIGHT_KEY)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final val R = paramTable.get(RECURRENT_WEIGHT_KEY);
			Dim R As val = paramTable(RECURRENT_WEIGHT_KEY)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final val b = paramTable.get(BIAS_KEY);
			Dim b As val = paramTable(BIAS_KEY)

			Dim shape() As Long = layerInput.Shape
			Preconditions.checkState(shape IsNot Nothing, "Null shape for input placeholder")
			Dim inputSlices() As SDVariable = sameDiff.unstack(layerInput, 2, CInt(shape(2)))
			Me.timeSteps = inputSlices.Length
			Dim outputSlices(timeSteps - 1) As SDVariable
			Dim prev As SDVariable = Nothing
			For i As Integer = 0 To timeSteps - 1
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final val x_i = inputSlices[i];
				Dim x_i As val = inputSlices(i)
				outputSlices(i) = x_i.mmul(W)
				If hasBias Then
					outputSlices(i) = outputSlices(i).add(b)
				End If

				If prev IsNot Nothing Then
					Dim attn As SDVariable
					If projectInput Then
						Dim Wq As val = paramTable(WEIGHT_KEY_QUERY_PROJECTION)
						Dim Wk As val = paramTable(WEIGHT_KEY_KEY_PROJECTION)
						Dim Wv As val = paramTable(WEIGHT_KEY_VALUE_PROJECTION)
						Dim Wo As val = paramTable(WEIGHT_KEY_OUT_PROJECTION)

						attn = sameDiff.nn_Conflict.multiHeadDotProductAttention(LayerName & "_attention_" & i, prev, layerInput, layerInput, Wq, Wk, Wv, Wo, mask, True)
					Else
						attn = sameDiff.nn_Conflict.dotProductAttention(LayerName & "_attention_" & i, prev, layerInput, layerInput, mask, True)
					End If

					attn = sameDiff.squeeze(attn, 2)

					outputSlices(i) = outputSlices(i).add(attn.mmul(R))
				End If

				outputSlices(i) = activation.asSameDiff(sameDiff, outputSlices(i))
				outputSlices(i) = sameDiff.expandDims(outputSlices(i), 2)
				prev = outputSlices(i)
			Next i
			Return sameDiff.concat(2, outputSlices)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter public static class Builder extends org.deeplearning4j.nn.conf.layers.samediff.SameDiffLayer.Builder<Builder>
		Public Class Builder
			Inherits SameDiffLayer.Builder(Of Builder)

			''' <summary>
			''' Number of inputs to the layer (input size)
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field nIn was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend nIn_Conflict As Integer

			''' <summary>
			''' Number of outputs (output size)
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field nOut was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend nOut_Conflict As Integer

			''' <summary>
			''' Number of Attention Heads
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field nHeads was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend nHeads_Conflict As Integer

			''' <summary>
			''' Size of attention heads
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field headSize was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend headSize_Conflict As Integer

			''' <summary>
			''' Project input before applying attention or not.
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field projectInput was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend projectInput_Conflict As Boolean = True

			''' <summary>
			''' If true (default is true) the layer will have a bias
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field hasBias was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend hasBias_Conflict As Boolean = True

			''' <summary>
			''' Activation function for the layer
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field activation was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend activation_Conflict As Activation = Activation.TANH

			''' <param name="nIn"> Number of inputs to the layer (input size) </param>
'JAVA TO VB CONVERTER NOTE: The parameter nIn was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function nIn(ByVal nIn_Conflict As Integer) As Builder
				Me.nIn_Conflict = nIn_Conflict
				Return Me
			End Function

			''' <param name="nOut"> Number of outputs (output size) </param>
'JAVA TO VB CONVERTER NOTE: The parameter nOut was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function nOut(ByVal nOut_Conflict As Integer) As Builder
				Me.nOut_Conflict = nOut_Conflict
				Return Me
			End Function

			''' <summary>
			''' Number of Attention Heads
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter nHeads was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function nHeads(ByVal nHeads_Conflict As Integer) As Builder
				Me.nHeads_Conflict = nHeads_Conflict
				Return Me
			End Function

			''' <summary>
			''' Size of attention heads
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter headSize was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function headSize(ByVal headSize_Conflict As Integer) As Builder
				Me.headSize_Conflict = headSize_Conflict
				Return Me
			End Function

			''' <summary>
			''' Project input before applying attention or not.
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter projectInput was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function projectInput(ByVal projectInput_Conflict As Boolean) As Builder
				Me.projectInput_Conflict = projectInput_Conflict
				Return Me
			End Function

			''' <param name="hasBias"> If true (default is true) the layer will have a bias </param>
'JAVA TO VB CONVERTER NOTE: The parameter hasBias was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function hasBias(ByVal hasBias_Conflict As Boolean) As Builder
				Me.hasBias_Conflict = hasBias_Conflict
				Return Me
			End Function

			''' <param name="activation"> Activation function for the layer </param>
'JAVA TO VB CONVERTER NOTE: The parameter activation was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function activation(ByVal activation_Conflict As Activation) As Builder
				Me.activation_Conflict = activation_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public RecurrentAttentionLayer build()
			Public Overrides Function build() As RecurrentAttentionLayer
				Preconditions.checkArgument(Me.projectInput_Conflict OrElse Me.nHeads_Conflict = 1, "projectInput must be true when nHeads != 1")
				Preconditions.checkArgument(Me.projectInput_Conflict OrElse nIn_Conflict = nOut_Conflict, "nIn must be equal to nOut when projectInput is false")
				Preconditions.checkArgument(Not Me.projectInput_Conflict OrElse nOut_Conflict <> 0, "nOut must be specified when projectInput is true")
				Preconditions.checkArgument(Me.nOut_Conflict Mod nHeads_Conflict = 0 OrElse headSize_Conflict > 0, "nOut isn't divided by nHeads cleanly. Specify the headSize manually.")
				Return New RecurrentAttentionLayer(Me)
			End Function
		End Class
	End Class

End Namespace