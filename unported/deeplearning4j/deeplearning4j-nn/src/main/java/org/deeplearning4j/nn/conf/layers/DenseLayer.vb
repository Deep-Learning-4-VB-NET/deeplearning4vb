Imports System
Imports System.Collections.Generic
Imports lombok
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports LayerMemoryReport = org.deeplearning4j.nn.conf.memory.LayerMemoryReport
Imports MemoryReport = org.deeplearning4j.nn.conf.memory.MemoryReport
Imports DefaultParamInitializer = org.deeplearning4j.nn.params.DefaultParamInitializer
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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
'ORIGINAL LINE: @Data @NoArgsConstructor @ToString(callSuper = true) @EqualsAndHashCode(callSuper = true) public class DenseLayer extends FeedForwardLayer
	<Serializable>
	Public Class DenseLayer
		Inherits FeedForwardLayer

'JAVA TO VB CONVERTER NOTE: The field hasLayerNorm was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private hasLayerNorm_Conflict As Boolean = False
'JAVA TO VB CONVERTER NOTE: The field hasBias was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private hasBias_Conflict As Boolean = True

		Private Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.hasBias_Conflict = builder.hasBias_Conflict
			Me.hasLayerNorm_Conflict = builder.hasLayerNorm_Conflict

			initializeConstraints(builder)
		End Sub

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As Layer
			LayerValidation.assertNInNOutSet("DenseLayer", getLayerName(), layerIndex, getNIn(), getNOut())

			Dim ret As New org.deeplearning4j.nn.layers.feedforward.dense.DenseLayer(conf, networkDataType)
			ret.setListeners(trainingListeners)
			ret.Index = layerIndex
			ret.ParamsViewArray = layerParamsView
			Dim paramTable As IDictionary(Of String, INDArray) = initializer().init(conf, layerParamsView, initializeParams)
			ret.ParamTable = paramTable
			ret.Conf = conf
			Return ret
		End Function

		Public Overrides Function initializer() As ParamInitializer
			Return DefaultParamInitializer.Instance
		End Function

		Public Overrides Function getMemoryReport(ByVal inputType As InputType) As LayerMemoryReport
			Dim outputType As InputType = getOutputType(-1, inputType)

			Dim numParams As val = initializer().numParams(Me)
			Dim updaterStateSize As val = CInt(Math.Truncate(getIUpdater().stateSize(numParams)))

			Dim trainSizeFixed As Integer = 0
			Dim trainSizeVariable As Integer = 0
			If getIDropout() IsNot Nothing Then
				If False Then
					'TODO drop connect
					'Dup the weights... note that this does NOT depend on the minibatch size...
					trainSizeVariable += 0 'TODO
				Else
					'Assume we dup the input
					trainSizeVariable += inputType.arrayElementsPerExample()
				End If
			End If

			'Also, during backprop: we do a preOut call -> gives us activations size equal to the output size
			' which is modified in-place by activation function backprop
			' then we have 'epsilonNext' which is equivalent to input size
			trainSizeVariable += outputType.arrayElementsPerExample()

			Return (New LayerMemoryReport.Builder(layerName, GetType(DenseLayer), inputType, outputType)).standardMemory(numParams, updaterStateSize).workingMemory(0, 0, trainSizeFixed, trainSizeVariable).cacheMemory(MemoryReport.CACHE_MODE_ALL_ZEROS, MemoryReport.CACHE_MODE_ALL_ZEROS).build()
		End Function

		Public Overridable Function hasBias() As Boolean
			Return hasBias_Conflict
		End Function

		Public Overridable Function hasLayerNorm() As Boolean
			Return hasLayerNorm_Conflict
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor @Getter @Setter public static class Builder extends FeedForwardLayer.Builder<Builder>
		Public Class Builder
			Inherits FeedForwardLayer.Builder(Of Builder)

			''' <summary>
			''' If true (default): include bias parameters in the model. False: no bias.
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field hasBias was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend hasBias_Conflict As Boolean = True

			''' <summary>
			''' If true (default): include bias parameters in the model. False: no bias.
			''' </summary>
			''' <param name="hasBias"> If true: include bias parameters in this model </param>
'JAVA TO VB CONVERTER NOTE: The parameter hasBias was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function hasBias(ByVal hasBias_Conflict As Boolean) As Builder
				Me.setHasBias(hasBias_Conflict)
				Return Me
			End Function

			''' <summary>
			''' If true (default = false): enable layer normalization on this layer
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field hasLayerNorm was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend hasLayerNorm_Conflict As Boolean = False
'JAVA TO VB CONVERTER NOTE: The parameter hasLayerNorm was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function hasLayerNorm(ByVal hasLayerNorm_Conflict As Boolean) As Builder
				Me.hasLayerNorm_Conflict = hasLayerNorm_Conflict
				Return Me
			End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public DenseLayer build()
			Public Overrides Function build() As DenseLayer
				Return New DenseLayer(Me)
			End Function
		End Class

	End Class

End Namespace