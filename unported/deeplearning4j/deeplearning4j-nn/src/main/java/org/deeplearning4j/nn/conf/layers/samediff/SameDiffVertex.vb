Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports TrainingConfig = org.deeplearning4j.nn.api.TrainingConfig
Imports GradientNormalization = org.deeplearning4j.nn.conf.GradientNormalization
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports GraphVertex = org.deeplearning4j.nn.conf.graph.GraphVertex
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports InvalidInputTypeException = org.deeplearning4j.nn.conf.inputs.InvalidInputTypeException
Imports MemoryReport = org.deeplearning4j.nn.conf.memory.MemoryReport
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports SameDiffGraphVertex = org.deeplearning4j.nn.layers.samediff.SameDiffGraphVertex
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports IUpdater = org.nd4j.linalg.learning.config.IUpdater
Imports Regularization = org.nd4j.linalg.learning.regularization.Regularization
Imports org.nd4j.common.primitives
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil

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

Namespace org.deeplearning4j.nn.conf.layers.samediff


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public abstract class SameDiffVertex extends org.deeplearning4j.nn.conf.graph.GraphVertex implements org.deeplearning4j.nn.api.TrainingConfig
	<Serializable>
	Public MustInherit Class SameDiffVertex
		Inherits GraphVertex
		Implements TrainingConfig

'JAVA TO VB CONVERTER NOTE: The field vertexParams was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private vertexParams_Conflict As SDVertexParams
		Private name As String

		Protected Friend regularization As IList(Of Regularization)
		Protected Friend regularizationBias As IList(Of Regularization)
		Protected Friend updater As IUpdater
		Protected Friend biasUpdater As IUpdater
'JAVA TO VB CONVERTER NOTE: The field gradientNormalization was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend gradientNormalization_Conflict As GradientNormalization
'JAVA TO VB CONVERTER NOTE: The field gradientNormalizationThreshold was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend gradientNormalizationThreshold_Conflict As Double = Double.NaN
'JAVA TO VB CONVERTER NOTE: The field dataType was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend dataType_Conflict As DataType

		''' <summary>
		''' Define the vertex </summary>
		''' <param name="sameDiff">   SameDiff instance </param>
		''' <param name="layerInput"> Input to the layer - keys as defined by <seealso cref="defineParametersAndInputs(SDVertexParams)"/> </param>
		''' <param name="paramTable"> Parameter table - keys as defined by <seealso cref="defineParametersAndInputs(SDVertexParams)"/> </param>
		''' <param name="maskVars">  Masks of input, if available - keys as defined by <seealso cref="defineParametersAndInputs(SDVertexParams)"/> </param>
		''' <returns> The final layer variable corresponding to the activations/output from the forward pass </returns>
		Public MustOverride Function defineVertex(ByVal sameDiff As SameDiff, ByVal layerInput As IDictionary(Of String, SDVariable), ByVal paramTable As IDictionary(Of String, SDVariable), ByVal maskVars As IDictionary(Of String, SDVariable)) As SDVariable

		''' <summary>
		''' Define the parameters - and inputs - for the network.
		''' Use <seealso cref="SDVertexParams.addWeightParam(String, Long...)"/> and
		''' <seealso cref="SDVertexParams.addBiasParam(String, Long...)"/>.
		''' Note also you must define (and optionally name) the inputs to the vertex. This is required so that
		''' DL4J knows how many inputs exists for the vertex. </summary>
		''' <param name="params"> Object used to set parameters for this layer </param>
		Public MustOverride Sub defineParametersAndInputs(ByVal params As SDVertexParams)

		''' <summary>
		''' Set the initial parameter values for this layer, if required </summary>
		''' <param name="params"> Parameter arrays that may be initialized </param>
		Public MustOverride Sub initializeParameters(ByVal params As IDictionary(Of String, INDArray))

		Public Overridable ReadOnly Property VertexParams As SDVertexParams
			Get
				If vertexParams_Conflict Is Nothing Then
					vertexParams_Conflict = New SDVertexParams()
					defineParametersAndInputs(vertexParams_Conflict)
				End If
				Return vertexParams_Conflict
			End Get
		End Property

		Public Overrides Function numParams(ByVal backprop As Boolean) As Long
			Dim params As SDLayerParams = VertexParams
			Dim count As Long = 0
			For Each l As Long() In params.getParamShapes().Values
				count += ArrayUtil.prodLong(l)
			Next l
			Return CInt(count)
		End Function

		Public Overrides Function minVertexInputs() As Integer
			Return 1
		End Function

		Public Overrides Function maxVertexInputs() As Integer
			Return -1
		End Function

		Public Overrides Function instantiate(ByVal graph As ComputationGraph, ByVal name As String, ByVal idx As Integer, ByVal paramsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDatatype As DataType) As org.deeplearning4j.nn.graph.vertex.GraphVertex
			Me.name = name
			Return New SameDiffGraphVertex(Me, graph, name, idx, paramsView, initializeParams, networkDatatype)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.nn.conf.inputs.InputType getOutputType(int layerIndex, org.deeplearning4j.nn.conf.inputs.InputType... vertexInputs) throws org.deeplearning4j.nn.conf.inputs.InvalidInputTypeException
		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ParamArray ByVal vertexInputs() As InputType) As InputType
			Throw New System.NotSupportedException("Not yet implemented")
		End Function

		Public Overridable Function feedForwardMaskArrays(ByVal maskArrays() As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState)
			Throw New System.NotSupportedException("Not yet supported")
		End Function

		''' <summary>
		''' Validate input arrays to confirm that they fulfill the assumptions of the layer. If they don't, throw an exception. </summary>
		''' <param name="input"> inputs to the layer </param>
		Public Overridable Sub validateInput(ByVal input() As INDArray)
		End Sub

		Public Overrides Function getMemoryReport(ParamArray ByVal inputTypes() As InputType) As MemoryReport
			Return Nothing
		End Function


		Public Overridable Function paramReshapeOrder(ByVal paramName As String) As Char
			Return "c"c
		End Function


		Public Overridable Sub applyGlobalConfig(ByVal b As NeuralNetConfiguration.Builder)
			If regularization Is Nothing OrElse regularization.Count = 0 Then
				regularization = b.getRegularization()
			End If
			If regularizationBias Is Nothing OrElse regularizationBias.Count = 0 Then
				regularizationBias = b.getRegularizationBias()
			End If
			If updater Is Nothing Then
				updater = b.getIUpdater()
			End If
			If biasUpdater Is Nothing Then
				biasUpdater = b.getBiasUpdater()
			End If
			If gradientNormalization_Conflict = Nothing Then
				gradientNormalization_Conflict = b.getGradientNormalization()
			End If
			If Double.IsNaN(gradientNormalizationThreshold_Conflict) Then
				gradientNormalizationThreshold_Conflict = b.getGradientNormalizationThreshold()
			End If

			applyGlobalConfigToLayer(b)
		End Sub

		Public Overridable Sub applyGlobalConfigToLayer(ByVal globalConfig As NeuralNetConfiguration.Builder)
			'Default implementation: no op
		End Sub

		Public Overridable ReadOnly Property LayerName As String Implements TrainingConfig.getLayerName
			Get
				Return name
			End Get
		End Property

		Public Overridable Function getRegularizationByParam(ByVal paramName As String) As IList(Of Regularization) Implements TrainingConfig.getRegularizationByParam
			If (regularization Is Nothing OrElse regularization.Count = 0) AndAlso (regularizationBias Is Nothing OrElse regularizationBias.Count = 0) Then
				Return Nothing
			End If
			If VertexParams.isWeightParam(paramName) Then
				Return regularization
			End If
			If VertexParams.isBiasParam(paramName) Then
				Return regularizationBias
			End If
			Throw New System.InvalidOperationException("Unknown parameter name: " & paramName & " - not in weights (" & VertexParams.getWeightParameterKeys() & ") or biases (" & VertexParams.getBiasParameterKeys() & ")")
		End Function

		Public Overridable Function isPretrainParam(ByVal paramName As String) As Boolean Implements TrainingConfig.isPretrainParam
			Return False
		End Function

		Public Overridable Function getUpdaterByParam(ByVal paramName As String) As IUpdater Implements TrainingConfig.getUpdaterByParam
			If VertexParams.isWeightParam(paramName) Then
				Return updater
			End If
			If VertexParams.isBiasParam(paramName) Then
				If biasUpdater Is Nothing Then
					Return updater
				End If
				Return biasUpdater
			End If
			Throw New System.InvalidOperationException("Unknown parameter name: " & paramName & " - not in weights (" & VertexParams.getWeightParameterKeys() & ") or biases (" & VertexParams.getBiasParameterKeys() & ")")
		End Function

		Public Overridable ReadOnly Property GradientNormalization As GradientNormalization Implements TrainingConfig.getGradientNormalization
			Get
				Return gradientNormalization_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property GradientNormalizationThreshold As Double Implements TrainingConfig.getGradientNormalizationThreshold
			Get
				Return gradientNormalizationThreshold_Conflict
			End Get
		End Property

		Public Overrides WriteOnly Property DataType Implements TrainingConfig.setDataType As DataType
			Set(ByVal dataType As DataType)
				Me.dataType_Conflict = dataType
			End Set
		End Property
	End Class

End Namespace