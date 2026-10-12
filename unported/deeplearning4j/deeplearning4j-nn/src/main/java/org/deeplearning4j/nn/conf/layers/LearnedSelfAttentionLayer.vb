Imports System
Imports System.Collections.Generic
Imports lombok
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports SDLayerParams = org.deeplearning4j.nn.conf.layers.samediff.SDLayerParams
Imports SameDiffLayer = org.deeplearning4j.nn.conf.layers.samediff.SameDiffLayer
Imports WeightInitUtil = org.deeplearning4j.nn.weights.WeightInitUtil
Imports SDIndex = org.nd4j.autodiff.samediff.SDIndex
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
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

Namespace org.deeplearning4j.nn.conf.layers


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @EqualsAndHashCode(callSuper = true) public class LearnedSelfAttentionLayer extends org.deeplearning4j.nn.conf.layers.samediff.SameDiffLayer
	<Serializable>
	Public Class LearnedSelfAttentionLayer
		Inherits SameDiffLayer

		Private nIn As Long
		Private nOut As Long
		Private nHeads As Integer
		Private headSize As Long
		Private projectInput As Boolean
		Private nQueries As Integer

		Private Const WEIGHT_KEY_QUERY_PROJECTION As String = "Wq"
		Private Const WEIGHT_KEY_KEY_PROJECTION As String = "Wk"
		Private Const WEIGHT_KEY_VALUE_PROJECTION As String = "Wv"
		Private Const WEIGHT_KEY_OUT_PROJECTION As String = "Wo"
		Private Const WEIGHT_QUERIES As String = "Q"

		Private Sub New()
		End Sub

		Protected Friend Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			nIn = builder.nIn_Conflict
			nOut = builder.nOut_Conflict
			nHeads = builder.nHeads_Conflict
			headSize = If(builder.headSize_Conflict = 0, nOut \ nHeads, builder.headSize_Conflict)
			projectInput = builder.projectInput_Conflict
			nQueries = builder.nQueries_Conflict
		End Sub

		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			Return InputTypeUtil.getPreprocessorForInputTypeRnnLayers(inputType, RNNFormat.NCW,LayerName)
		End Function

		Public Overrides Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)
			If inputType Is Nothing OrElse inputType.getType() <> InputType.Type.RNN Then
				Throw New System.InvalidOperationException("Invalid input for Learned Self Attention layer (layer name = """ & LayerName & """): expect RNN input type with size > 0. Got: " & inputType)
			End If

			If nIn <= 0 OrElse override Then
				Dim r As InputType.InputTypeRecurrent = DirectCast(inputType, InputType.InputTypeRecurrent)
				Me.nIn = r.getSize()
			End If
		End Sub

		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			If inputType Is Nothing OrElse inputType.getType() <> InputType.Type.RNN Then
				Throw New System.InvalidOperationException("Invalid input for Learned Self Attention layer (layer index = " & layerIndex & ", layer name = """ & LayerName & """): expect RNN input type with size > 0. Got: " & inputType)
			End If

			If projectInput Then
				Return InputType.recurrent(nOut, nQueries)
			Else
				Return InputType.recurrent(nIn, nQueries)
			End If
		End Function

		Public Overrides Sub defineParameters(ByVal params As SDLayerParams)
			params.clear()

			params.addWeightParam(WEIGHT_QUERIES, 1, nIn, nQueries)

			If projectInput Then
				params.addWeightParam(WEIGHT_KEY_QUERY_PROJECTION, nHeads, headSize, nIn)
				params.addWeightParam(WEIGHT_KEY_KEY_PROJECTION, nHeads, headSize, nIn)
				params.addWeightParam(WEIGHT_KEY_VALUE_PROJECTION, nHeads, headSize, nIn)
				params.addWeightParam(WEIGHT_KEY_OUT_PROJECTION, nHeads * headSize, nOut)
			End If
		End Sub

		Public Overrides Sub initializeParameters(ByVal params As IDictionary(Of String, INDArray))
			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.scopeOutOfWorkspaces()
				For Each e As KeyValuePair(Of String, INDArray) In params.SetOfKeyValuePairs()
					If e.Key.Equals(WEIGHT_KEY_OUT_PROJECTION) Then
						WeightInitUtil.initWeights(nIn, headSize, e.Value.shape(), weightInit, Nothing, "c"c, e.Value)
					ElseIf e.Key.Equals(WEIGHT_QUERIES) Then
						WeightInitUtil.initWeights(nIn, nQueries, e.Value.shape(), weightInit, Nothing, "c"c, e.Value)
					Else
						WeightInitUtil.initWeights(nHeads * headSize, nOut, e.Value.shape(), weightInit, Nothing, "c"c, e.Value)
					End If
				Next e
			End Using
		End Sub


		Public Overrides Function defineLayer(ByVal sameDiff As SameDiff, ByVal layerInput As SDVariable, ByVal paramTable As IDictionary(Of String, SDVariable), ByVal mask As SDVariable) As SDVariable
			Dim baseQueries As val = paramTable(WEIGHT_QUERIES)
			Dim batchSize As val = layerInput.shape().get(SDIndex.point(0))
			Dim tileAxis As val = sameDiff.scatterUpdate(sameDiff.onesLike(layerInput.shape()), sameDiff.constant(0), batchSize)

			Dim queries As val = sameDiff.tile(baseQueries, tileAxis)

			If projectInput Then
				Dim Wq As val = paramTable(WEIGHT_KEY_QUERY_PROJECTION)
				Dim Wk As val = paramTable(WEIGHT_KEY_KEY_PROJECTION)
				Dim Wv As val = paramTable(WEIGHT_KEY_VALUE_PROJECTION)
				Dim Wo As val = paramTable(WEIGHT_KEY_OUT_PROJECTION)

				Return sameDiff.nn_Conflict.multiHeadDotProductAttention(LayerName, queries, layerInput, layerInput, Wq, Wk, Wv, Wo, mask, True)
			Else
				Return sameDiff.nn_Conflict.dotProductAttention(LayerName, queries, layerInput, layerInput, mask, True)
			End If
		End Function

		Public Overrides Function feedForwardMaskArray(ByVal maskArray As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState)
			' No further mask propagation here, as the results have taken any mask into account, like in a global pooling layer
			Return Nothing
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
			Friend projectInput_Conflict As Boolean


			''' <summary>
			''' Number of queries to learn
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field nQueries was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend nQueries_Conflict As Integer

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

			''' <summary>
			''' Number of queries to learn
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter nQueries was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function nQueries(ByVal nQueries_Conflict As Integer) As Builder
				Me.nQueries_Conflict = nQueries_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public LearnedSelfAttentionLayer build()
			Public Overrides Function build() As LearnedSelfAttentionLayer
				Preconditions.checkArgument(Me.projectInput_Conflict OrElse Me.nHeads_Conflict = 1, "projectInput must be true when nHeads != 1")
				Preconditions.checkArgument(Me.projectInput_Conflict OrElse nIn_Conflict = nOut_Conflict, "nIn must be equal to nOut when projectInput is false")
				Preconditions.checkArgument(Not Me.projectInput_Conflict OrElse nOut_Conflict <> 0, "nOut must be specified when projectInput is true")
				Preconditions.checkArgument(Me.nOut_Conflict Mod nHeads_Conflict = 0 OrElse headSize_Conflict > 0, "nOut isn't divided by nHeads cleanly. Specify the headSize manually.")
				Preconditions.checkArgument(Me.nQueries_Conflict > 0, "You must set numQueries.")

				Return New LearnedSelfAttentionLayer(Me)
			End Function
		End Class
	End Class

End Namespace