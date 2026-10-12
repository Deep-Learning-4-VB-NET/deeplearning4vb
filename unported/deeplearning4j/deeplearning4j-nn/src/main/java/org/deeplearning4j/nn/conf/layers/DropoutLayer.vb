Imports System
Imports System.Collections.Generic
Imports lombok
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports Dropout = org.deeplearning4j.nn.conf.dropout.Dropout
Imports IDropout = org.deeplearning4j.nn.conf.dropout.IDropout
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports LayerMemoryReport = org.deeplearning4j.nn.conf.memory.LayerMemoryReport
Imports MemoryReport = org.deeplearning4j.nn.conf.memory.MemoryReport
Imports EmptyParamInitializer = org.deeplearning4j.nn.params.EmptyParamInitializer
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Regularization = org.nd4j.linalg.learning.regularization.Regularization

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
'ORIGINAL LINE: @Data @NoArgsConstructor @ToString(callSuper = true) @EqualsAndHashCode(callSuper = true) public class DropoutLayer extends FeedForwardLayer
	<Serializable>
	Public Class DropoutLayer
		Inherits FeedForwardLayer

		Private Sub New(ByVal builder As Builder)
			MyBase.New(builder)
		End Sub

		Public Sub New(ByVal activationRetainProb As Double)
			Me.New((New Builder()).dropOut(activationRetainProb))
		End Sub

		Public Sub New(ByVal dropout As IDropout)
			Me.New((New Builder()).dropOut(dropout))
		End Sub

		Public Overrides Function clone() As DropoutLayer
			Return CType(MyBase.clone(), DropoutLayer)
		End Function

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As org.deeplearning4j.nn.api.Layer
			Dim ret As New org.deeplearning4j.nn.layers.DropoutLayer(conf, networkDataType)
			ret.setListeners(trainingListeners)
			ret.Index = layerIndex
			ret.ParamsViewArray = layerParamsView
			Dim paramTable As IDictionary(Of String, INDArray) = initializer().init(conf, layerParamsView, initializeParams)
			ret.ParamTable = paramTable
			ret.Conf = conf
			Return ret
		End Function

		Public Overrides Function initializer() As ParamInitializer
			Return EmptyParamInitializer.Instance
		End Function

		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			If inputType Is Nothing Then
				Throw New System.InvalidOperationException("Invalid input type: null for layer name """ & getLayerName() & """")
			End If
			Return inputType
		End Function

		Public Overrides Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)
			'No op: dropout layer doesn't have a fixed nIn value
		End Sub

		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			'No input preprocessor required; dropout applies to any input type
			Return Nothing
		End Function

		Public Overrides Function getRegularizationByParam(ByVal paramName As String) As IList(Of Regularization)
			'Not applicable
			Return Nothing
		End Function

		Public Overrides Function isPretrainParam(ByVal paramName As String) As Boolean
			Throw New System.NotSupportedException("Dropout layer does not contain parameters")
		End Function

		Public Overrides Function getMemoryReport(ByVal inputType As InputType) As LayerMemoryReport
			Dim actElementsPerEx As val = inputType.arrayElementsPerExample()
			'During inference: not applied. During  backprop: dup the input, in case it's used elsewhere
			'But: this will be counted in the activations
			'(technically inference memory is over-estimated as a result)

			Return (New LayerMemoryReport.Builder(layerName, GetType(DropoutLayer), inputType, inputType)).standardMemory(0, 0).workingMemory(0, 0, 0, 0).cacheMemory(MemoryReport.CACHE_MODE_ALL_ZEROS, MemoryReport.CACHE_MODE_ALL_ZEROS).build()
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public static class Builder extends FeedForwardLayer.Builder<Builder>
		Public Class Builder
			Inherits FeedForwardLayer.Builder(Of Builder)

			''' <summary>
			''' Create a dropout layer with standard <seealso cref="Dropout"/>, with the specified probability of retaining the input
			''' activation. See <seealso cref="Dropout"/> for the full details
			''' </summary>
			''' <param name="dropout"> Activation retain probability. </param>
			Public Sub New(ByVal dropout As Double)
				Me.dropOut(New Dropout(dropout))
			End Sub

			''' <param name="dropout"> Specified <seealso cref="IDropout"/> instance for the dropout layer </param>
			Public Sub New(ByVal dropout As IDropout)
				Me.dropOut(dropout)
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public DropoutLayer build()
			Public Overrides Function build() As DropoutLayer

				Return New DropoutLayer(Me)
			End Function
		End Class


	End Class

End Namespace