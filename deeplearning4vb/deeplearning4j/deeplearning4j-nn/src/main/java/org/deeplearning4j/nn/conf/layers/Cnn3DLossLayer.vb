Imports System
Imports System.Collections.Generic
Imports lombok
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
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
'ORIGINAL LINE: @Data @NoArgsConstructor @ToString(callSuper = true) @EqualsAndHashCode(callSuper = true) public class Cnn3DLossLayer extends FeedForwardLayer
	<Serializable>
	Public Class Cnn3DLossLayer
		Inherits FeedForwardLayer

		Protected Friend lossFn As ILossFunction
		Protected Friend dataFormat As Convolution3D.DataFormat

		Private Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.lossFn = builder.lossFn
			Me.dataFormat = builder.dataFormat
		End Sub

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As Layer
			Dim ret As New org.deeplearning4j.nn.layers.convolution.Cnn3DLossLayer(conf, networkDataType)
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
			If inputType Is Nothing OrElse (inputType.getType() <> InputType.Type.CNN3D AndAlso inputType.getType() <> InputType.Type.CNNFlat) Then
				Throw New System.InvalidOperationException("Invalid input type for CnnLossLayer (layer index = " & layerIndex & ", layer name=""" & getLayerName() & """): Expected CNN3D or CNNFlat input, got " & inputType)
			End If
			Return inputType
		End Function

		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			Return InputTypeUtil.getPreProcessorForInputTypeCnn3DLayers(inputType, getLayerName())
		End Function

		Public Overrides Function getMemoryReport(ByVal inputType As InputType) As LayerMemoryReport
			'During inference and training: dup the input array. But, this counts as *activations* not working memory
			Return (New LayerMemoryReport.Builder(layerName, Me.GetType(), inputType, inputType)).standardMemory(0, 0).workingMemory(0, 0, 0, 0).cacheMemory(MemoryReport.CACHE_MODE_ALL_ZEROS, MemoryReport.CACHE_MODE_ALL_ZEROS).build()
		End Function

		Public Overrides Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)
			'No op
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter public static class Builder extends BaseOutputLayer.Builder<Builder>
		Public Class Builder
			Inherits BaseOutputLayer.Builder(Of Builder)

			''' <summary>
			''' Format of the input/output data. See <seealso cref="Convolution3D.DataFormat"/> for details
			''' </summary>
			Protected Friend dataFormat As Convolution3D.DataFormat

			''' <param name="format"> Format of the input/output data. See <seealso cref="Convolution3D.DataFormat"/> for details </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder(@NonNull Convolution3D.DataFormat format)
			Public Sub New(ByVal format As Convolution3D.DataFormat)
				Me.setDataFormat(format)
				Me.setActivationFn(Activation.IDENTITY.getActivationFunction())
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public Builder nIn(int nIn)
'JAVA TO VB CONVERTER NOTE: The parameter nIn was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function nIn(ByVal nIn_Conflict As Integer) As Builder
				Throw New System.NotSupportedException("Cnn3DLossLayer has no parameters, thus nIn will always equal nOut.")
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public Builder nOut(int nOut)
'JAVA TO VB CONVERTER NOTE: The parameter nOut was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function nOut(ByVal nOut_Conflict As Integer) As Builder
				Throw New System.NotSupportedException("Cnn3DLossLayer has no parameters, thus nIn will always equal nOut.")
			End Function

			Public Overrides WriteOnly Property NIn As Long
				Set(ByVal nIn As Long)
					Throw New System.NotSupportedException("Cnn3DLossLayer has no parameters, thus nIn will always equal nOut.")
				End Set
			End Property

			Public Overrides WriteOnly Property NOut As Long
				Set(ByVal nOut As Long)
					Throw New System.NotSupportedException("Cnn3DLossLayer has no parameters, thus nIn will always equal nOut.")
				End Set
			End Property


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public Cnn3DLossLayer build()
			Public Overrides Function build() As Cnn3DLossLayer
				Return New Cnn3DLossLayer(Me)
			End Function
		End Class
	End Class

End Namespace