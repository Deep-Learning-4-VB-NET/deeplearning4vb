Imports System
Imports System.Collections.Generic
Imports Preconditions = org.nd4j.shade.guava.base.Preconditions
Imports lombok
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports InvalidInputTypeException = org.deeplearning4j.nn.conf.inputs.InvalidInputTypeException
Imports SDVertexParams = org.deeplearning4j.nn.conf.layers.samediff.SDVertexParams
Imports SameDiffVertex = org.deeplearning4j.nn.conf.layers.samediff.SameDiffVertex
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports WeightInitUtil = org.deeplearning4j.nn.weights.WeightInitUtil
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
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
Namespace org.deeplearning4j.nn.conf.graph


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor @Data @EqualsAndHashCode(callSuper = true) @ToString public class AttentionVertex extends org.deeplearning4j.nn.conf.layers.samediff.SameDiffVertex
	<Serializable>
	Public Class AttentionVertex
		Inherits SameDiffVertex

		Private nInKeys As Long = 0
		Private nInValues As Long = 0
		Private nInQueries As Long = 0
		Private nOut As Long = 0
		Private headSize As Long = 0
		Private nHeads As Integer = 1
		Private projectInput As Boolean
		Protected Friend weightInit As WeightInit

		Private Const WEIGHT_KEY_QUERY_PROJECTION As String = "Wq"
		Private Const WEIGHT_KEY_KEY_PROJECTION As String = "Wk"
		Private Const WEIGHT_KEY_VALUE_PROJECTION As String = "Wv"
		Private Const WEIGHT_KEY_OUT_PROJECTION As String = "Wo"

		Protected Friend Sub New(ByVal builder As Builder)
			Me.nInKeys = builder.nInKeys_Conflict
			Me.nInValues = builder.nInValues_Conflict
			Me.nInQueries = builder.nInQueries_Conflict
			Me.nOut = builder.nOut_Conflict
			Me.headSize = builder.headSize_Conflict
			Me.projectInput = builder.projectInput_Conflict
			Me.nHeads = builder.nHeads_Conflict
			Me.weightInit = builder.weightInit_Conflict
		End Sub

		Public Overrides Function clone() As AttentionVertex
			Dim av As New AttentionVertex()
			av.nInKeys = nInKeys
			av.nInValues = nInValues
			av.nInQueries = nInQueries
			av.nOut = nOut
			av.headSize = headSize
			av.nHeads = nHeads
			av.projectInput = projectInput
			av.weightInit = weightInit
			Return av
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.nn.conf.inputs.InputType getOutputType(int layerIndex, org.deeplearning4j.nn.conf.inputs.InputType... vertexInputs) throws org.deeplearning4j.nn.conf.inputs.InvalidInputTypeException
		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ParamArray ByVal vertexInputs() As InputType) As InputType
			Dim queries As InputType.InputTypeRecurrent = DirectCast(vertexInputs(0), InputType.InputTypeRecurrent)

			If projectInput Then
				Return InputType.recurrent(nOut, queries.getTimeSeriesLength())
			Else
				Return InputType.recurrent(nInValues, queries.getTimeSeriesLength())
			End If
		End Function

		Public Overrides Sub defineParametersAndInputs(ByVal params As SDVertexParams)
			params.clear()

			params.defineInputs("queries", "keys", "values")

			If projectInput Then
				params.addWeightParam(WEIGHT_KEY_QUERY_PROJECTION, nHeads, headSize, nInQueries)
				params.addWeightParam(WEIGHT_KEY_KEY_PROJECTION, nHeads, headSize, nInKeys)
				params.addWeightParam(WEIGHT_KEY_VALUE_PROJECTION, nHeads, headSize, nInValues)
				params.addWeightParam(WEIGHT_KEY_OUT_PROJECTION, nHeads * headSize, nOut)
			End If
		End Sub

		Public Overrides Sub initializeParameters(ByVal params As IDictionary(Of String, INDArray))
			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.scopeOutOfWorkspaces()
				For Each e As KeyValuePair(Of String, INDArray) In params.SetOfKeyValuePairs()
					Select Case e.Key
						Case WEIGHT_KEY_QUERY_PROJECTION
							WeightInitUtil.initWeights(nInQueries, headSize, e.Value.shape(), weightInit, Nothing, "c"c, e.Value)
						Case WEIGHT_KEY_KEY_PROJECTION
							WeightInitUtil.initWeights(nInKeys, headSize, e.Value.shape(), weightInit, Nothing, "c"c, e.Value)
						Case WEIGHT_KEY_VALUE_PROJECTION
							WeightInitUtil.initWeights(nInValues, headSize, e.Value.shape(), weightInit, Nothing, "c"c, e.Value)
						Case WEIGHT_KEY_OUT_PROJECTION
							WeightInitUtil.initWeights(nHeads * headSize, nOut, e.Value.shape(), weightInit, Nothing, "c"c, e.Value)
					End Select
				Next e
			End Using
		End Sub

		Public Overrides Function feedForwardMaskArrays(ByVal maskArrays() As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState)
			If maskArrays IsNot Nothing Then
				If maskArrays(0) Is Nothing Then
					' Queries are unmasked, we don't need to pass on any mask
					Return Nothing
				Else
					' Queries are masked, keep the masking going
					Return Pair.of(maskArrays(0), currentMaskState)
				End If
			Else
				Return Pair.of(Nothing, currentMaskState)
			End If
		End Function

		Public Overrides Function defineVertex(ByVal sameDiff As SameDiff, ByVal layerInput As IDictionary(Of String, SDVariable), ByVal paramTable As IDictionary(Of String, SDVariable), ByVal maskVars As IDictionary(Of String, SDVariable)) As SDVariable
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.autodiff.samediff.SDVariable queries = layerInput.get("queries");
			Dim queries As SDVariable = layerInput("queries")
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.autodiff.samediff.SDVariable keys = layerInput.get("keys");
			Dim keys As SDVariable = layerInput("keys")
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.autodiff.samediff.SDVariable values = layerInput.get("values");
			Dim values As SDVariable = layerInput("values")
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.autodiff.samediff.SDVariable mask = maskVars != null ? sameDiff.min(maskVars.get("keys"), maskVars.get("values")): null;
			Dim mask As SDVariable = If(maskVars IsNot Nothing, sameDiff.min(maskVars("keys"), maskVars("values")), Nothing)

			Dim attention As SDVariable
			If projectInput Then
				Dim Wq As val = paramTable(WEIGHT_KEY_QUERY_PROJECTION)
				Dim Wk As val = paramTable(WEIGHT_KEY_KEY_PROJECTION)
				Dim Wv As val = paramTable(WEIGHT_KEY_VALUE_PROJECTION)
				Dim Wo As val = paramTable(WEIGHT_KEY_OUT_PROJECTION)

				attention = sameDiff.nn_Conflict.multiHeadDotProductAttention(LayerName, queries, keys, values, Wq, Wk, Wv, Wo, mask, True)
			Else
				attention = sameDiff.nn_Conflict.dotProductAttention(LayerName, queries, keys, values, mask, True)
			End If

			If maskVars IsNot Nothing Then
				Return attention.mul(sameDiff.expandDims(maskVars("queries"), 1))
			Else
				Return attention
			End If
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter public static class Builder
		Public Class Builder
			''' <summary>
			''' Size of Keys
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field nInKeys was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend nInKeys_Conflict As Long = 0

			''' <summary>
			''' Size of Values
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field nInValues was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend nInValues_Conflict As Long = 0

			''' <summary>
			''' Size of Queries
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field nInQueries was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend nInQueries_Conflict As Long = 0

			''' <summary>
			''' Output Size
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field nOut was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend nOut_Conflict As Long = 0

			''' <summary>
			''' Size of Attention Heads
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field headSize was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend headSize_Conflict As Long = 0

			''' <summary>
			''' Number of Attention Heads
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field nHeads was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend nHeads_Conflict As Integer = 1

			''' <summary>
			''' Toggle to enable / disable projection of inputs (key, values, queries).
			''' 
			''' Works only if input size is identical for all AND only one head is used AND output size is
			''' identical to input size
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field projectInput was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend projectInput_Conflict As Boolean

			''' <summary>
			''' Weight initialization scheme
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field weightInit was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend weightInit_Conflict As WeightInit


			''' <summary>
			''' Size of Keys
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter nInKeys was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function nInKeys(ByVal nInKeys_Conflict As Long) As Builder
				Me.nInKeys_Conflict = nInKeys_Conflict
				Return Me
			End Function

			''' <summary>
			''' Size of Queries
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter nInQueries was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function nInQueries(ByVal nInQueries_Conflict As Long) As Builder
				Me.nInQueries_Conflict = nInQueries_Conflict
				Return Me
			End Function

			''' <summary>
			''' Size of Values
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter nInValues was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function nInValues(ByVal nInValues_Conflict As Long) As Builder
				Me.nInValues_Conflict = nInValues_Conflict
				Return Me
			End Function

			''' <summary>
			''' Size of Attention Heads
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter headSize was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function headSize(ByVal headSize_Conflict As Long) As Builder
				Me.headSize_Conflict = headSize_Conflict
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
			''' Output Size
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter nOut was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function nOut(ByVal nOut_Conflict As Long) As Builder
				Me.nOut_Conflict = nOut_Conflict
				Return Me
			End Function

			''' <summary>
			''' Weight initialization scheme
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter weightInit was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function weightInit(ByVal weightInit_Conflict As WeightInit) As Builder
				Me.weightInit_Conflict = weightInit_Conflict
				Return Me
			End Function

			''' <summary>
			''' Toggle to enable / disable projection of inputs (key, values, queries).
			''' 
			''' Works only if input size is identical for all AND only one head is used AND output size is
			''' identical to input size
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter projectInput was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function projectInput(ByVal projectInput_Conflict As Boolean) As Builder
				Me.projectInput_Conflict = projectInput_Conflict
				Return Me
			End Function

			Public Overridable Function build() As AttentionVertex
				Me.nHeads_Conflict = If(nHeads_Conflict = 0, 1, nHeads_Conflict)
				Me.weightInit_Conflict = If(weightInit_Conflict = Nothing, WeightInit.XAVIER, weightInit_Conflict)
				Preconditions.checkArgument(nOut_Conflict > 0, "You have to set nOut")
				Preconditions.checkArgument(nInKeys_Conflict > 0, "You have to set nInKeys")
				Preconditions.checkArgument(nInQueries_Conflict > 0, "You have to set nInQueries")
				Preconditions.checkArgument(nInValues_Conflict > 0, "You have to set nInValues")
				Preconditions.checkArgument(headSize_Conflict > 0 OrElse nOut_Conflict Mod Me.nHeads_Conflict = 0, "You have to set a head size if nOut isn't cleanly divided by nHeads")
				Preconditions.checkArgument(projectInput_Conflict OrElse (nInQueries_Conflict = nInKeys_Conflict AndAlso nInKeys_Conflict = nInValues_Conflict AndAlso nInValues_Conflict = nOut_Conflict AndAlso nHeads_Conflict = 1), "You may only disable projectInput if all nIn* equal to nOut and you want to use only a single attention head")
				Me.headSize_Conflict = If(headSize_Conflict = 0, nOut_Conflict \ nHeads_Conflict, headSize_Conflict)

				Return New AttentionVertex(Me)
			End Function
		End Class
	End Class

End Namespace