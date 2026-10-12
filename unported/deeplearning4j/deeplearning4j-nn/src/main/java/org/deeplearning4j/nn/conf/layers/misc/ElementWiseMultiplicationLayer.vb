Imports System
Imports System.Collections.Generic
Imports lombok
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports FeedForwardLayer = org.deeplearning4j.nn.conf.layers.FeedForwardLayer
Imports LayerMemoryReport = org.deeplearning4j.nn.conf.memory.LayerMemoryReport
Imports MemoryReport = org.deeplearning4j.nn.conf.memory.MemoryReport
Imports ElementWiseParamInitializer = org.deeplearning4j.nn.params.ElementWiseParamInitializer
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

Namespace org.deeplearning4j.nn.conf.layers.misc



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @ToString(callSuper = true) @EqualsAndHashCode(callSuper = true) public class ElementWiseMultiplicationLayer extends org.deeplearning4j.nn.conf.layers.FeedForwardLayer
	<Serializable>
	Public Class ElementWiseMultiplicationLayer
		Inherits FeedForwardLayer

		'  We have to add an empty constructor for custom layers otherwise we will have errors when loading the model
		Protected Friend Sub New()
		End Sub

		Protected Friend Sub New(ByVal builder As Builder)
			MyBase.New(builder)
		End Sub

		Public Overrides Function clone() As ElementWiseMultiplicationLayer
'JAVA TO VB CONVERTER NOTE: The local variable clone was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim clone_Conflict As ElementWiseMultiplicationLayer = CType(MyBase.clone(), ElementWiseMultiplicationLayer)
			Return clone_Conflict
		End Function

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As Layer
			If Me.nIn <> Me.nOut Then
				Throw New System.InvalidOperationException("Element wise layer must have the same input and output size. Got nIn=" & nIn & ", nOut=" & nOut)
			End If
			Dim ret As New org.deeplearning4j.nn.layers.feedforward.elementwise.ElementWiseMultiplicationLayer(conf, networkDataType)
			ret.setListeners(trainingListeners)
			ret.Index = layerIndex
			ret.ParamsViewArray = layerParamsView
			Dim paramTable As IDictionary(Of String, INDArray) = initializer().init(conf, layerParamsView, initializeParams)
			ret.ParamTable = paramTable
			ret.Conf = conf

			Return ret
		End Function

		Public Overrides Function initializer() As ParamInitializer
			Return ElementWiseParamInitializer.Instance
		End Function

		''' <summary>
		''' This is a report of the estimated memory consumption for the given layer
		''' </summary>
		''' <param name="inputType"> Input type to the layer. Memory consumption is often a function of the input type </param>
		''' <returns> Memory report for the layer </returns>
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

			Return (New LayerMemoryReport.Builder(layerName, GetType(ElementWiseMultiplicationLayer), inputType, outputType)).standardMemory(numParams, updaterStateSize).workingMemory(0, 0, trainSizeFixed, trainSizeVariable).cacheMemory(MemoryReport.CACHE_MODE_ALL_ZEROS, MemoryReport.CACHE_MODE_ALL_ZEROS).build()
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public static class Builder extends org.deeplearning4j.nn.conf.layers.FeedForwardLayer.Builder<Builder>
		Public Class Builder
			Inherits FeedForwardLayer.Builder(Of Builder)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public ElementWiseMultiplicationLayer build()
			Public Overrides Function build() As ElementWiseMultiplicationLayer
				Return New ElementWiseMultiplicationLayer(Me)
			End Function
		End Class
	End Class

End Namespace