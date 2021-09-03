Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports ToString = lombok.ToString
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports LayerMemoryReport = org.deeplearning4j.nn.conf.memory.LayerMemoryReport
Imports MemoryReport = org.deeplearning4j.nn.conf.memory.MemoryReport
Imports EmptyParamInitializer = org.deeplearning4j.nn.params.EmptyParamInitializer
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ILossFunction = org.nd4j.linalg.lossfunctions.ILossFunction
Imports LossFunction = org.nd4j.linalg.lossfunctions.LossFunctions.LossFunction

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
'ORIGINAL LINE: @Data @NoArgsConstructor @ToString(callSuper = true) @EqualsAndHashCode(callSuper = true) public class CnnLossLayer extends FeedForwardLayer
	<Serializable>
	Public Class CnnLossLayer
		Inherits FeedForwardLayer

		Protected Friend lossFn As ILossFunction
		Protected Friend format As CNN2DFormat = CNN2DFormat.NCHW

		Private Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.lossFn = builder.lossFn
			Me.format = builder.format_Conflict
		End Sub

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As Layer
			Dim ret As New org.deeplearning4j.nn.layers.convolution.CnnLossLayer(conf, networkDataType)
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
			If inputType Is Nothing OrElse (inputType.getType() <> InputType.Type.CNN AndAlso inputType.getType() <> InputType.Type.CNNFlat) Then
				Throw New System.InvalidOperationException("Invalid input type for CnnLossLayer (layer index = " & layerIndex & ", layer name=""" & getLayerName() & """): Expected CNN or CNNFlat input, got " & inputType)
			End If
			Return inputType
		End Function

		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			Return InputTypeUtil.getPreProcessorForInputTypeCnnLayers(inputType, getLayerName())
		End Function

		Public Overrides Function getMemoryReport(ByVal inputType As InputType) As LayerMemoryReport
			'During inference and training: dup the input array. But, this counts as *activations* not working memory
			Return (New LayerMemoryReport.Builder(layerName, Me.GetType(), inputType, inputType)).standardMemory(0, 0).workingMemory(0, 0, 0, 0).cacheMemory(MemoryReport.CACHE_MODE_ALL_ZEROS, MemoryReport.CACHE_MODE_ALL_ZEROS).build()
		End Function

		Public Overrides Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)
			If TypeOf inputType Is InputType.InputTypeConvolutional Then
				Me.format = DirectCast(inputType, InputType.InputTypeConvolutional).getFormat()
			End If
		End Sub


		Public Class Builder
			Inherits BaseOutputLayer.Builder(Of Builder)

'JAVA TO VB CONVERTER NOTE: The field format was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend format_Conflict As CNN2DFormat = CNN2DFormat.NCHW

			Public Sub New()
				Me.activationFn = Activation.IDENTITY.getActivationFunction()
			End Sub

			Public Sub New(ByVal lossFunction As LossFunction)
				Me.lossFunction(lossFunction)
			End Sub

			Public Sub New(ByVal lossFunction As ILossFunction)
				Me.lossFn = lossFunction
			End Sub

'JAVA TO VB CONVERTER NOTE: The parameter format was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function format(ByVal format_Conflict As CNN2DFormat) As Builder
				Me.format_Conflict = format_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public Builder nIn(int nIn)
'JAVA TO VB CONVERTER NOTE: The parameter nIn was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function nIn(ByVal nIn_Conflict As Integer) As Builder
				Throw New System.NotSupportedException("Ths layer has no parameters, thus nIn will always equal nOut.")
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public Builder nOut(int nOut)
'JAVA TO VB CONVERTER NOTE: The parameter nOut was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function nOut(ByVal nOut_Conflict As Integer) As Builder
				Throw New System.NotSupportedException("Ths layer has no parameters, thus nIn will always equal nOut.")
			End Function

			Public Overrides WriteOnly Property NIn As Long
				Set(ByVal nIn As Long)
					Throw New System.NotSupportedException("This layer has no parameters, thus nIn will always equal nOut.")
				End Set
			End Property

			Public Overrides WriteOnly Property NOut As Long
				Set(ByVal nOut As Long)
					Throw New System.NotSupportedException("This layer has no parameters, thus nIn will always equal nOut.")
				End Set
			End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public CnnLossLayer build()
			Public Overrides Function build() As CnnLossLayer
				Return New CnnLossLayer(Me)
			End Function
		End Class
	End Class

End Namespace