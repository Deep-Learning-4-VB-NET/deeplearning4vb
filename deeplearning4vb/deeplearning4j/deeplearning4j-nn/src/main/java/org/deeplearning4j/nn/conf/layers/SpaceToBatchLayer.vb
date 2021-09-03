Imports System
Imports System.Collections.Generic
Imports lombok
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports LayerMemoryReport = org.deeplearning4j.nn.conf.memory.LayerMemoryReport
Imports MemoryReport = org.deeplearning4j.nn.conf.memory.MemoryReport
Imports EmptyParamInitializer = org.deeplearning4j.nn.params.EmptyParamInitializer
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports ValidationUtils = org.deeplearning4j.util.ValidationUtils
Imports Preconditions = org.nd4j.common.base.Preconditions
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
'ORIGINAL LINE: @Data @NoArgsConstructor @ToString(callSuper = true) @EqualsAndHashCode(callSuper = true) public class SpaceToBatchLayer extends NoParamLayer
	<Serializable>
	Public Class SpaceToBatchLayer
		Inherits NoParamLayer

		' TODO: throw error when block and padding dims don't match

		Protected Friend blocks() As Integer
		Protected Friend padding()() As Integer
		Protected Friend format As CNN2DFormat = CNN2DFormat.NCHW


		Protected Friend Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.blocks = builder.blocks
			Me.padding = builder.padding
			Me.format = builder.format
		End Sub

		Public Overrides Function clone() As SpaceToBatchLayer
			Return CType(MyBase.clone(), SpaceToBatchLayer)
		End Function

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As org.deeplearning4j.nn.api.Layer
			Dim ret As New org.deeplearning4j.nn.layers.convolution.SpaceToBatch(conf, networkDataType)
			ret.setListeners(trainingListeners)
			ret.Index = layerIndex
			ret.ParamsViewArray = layerParamsView
			Dim paramTable As IDictionary(Of String, INDArray) = initializer().init(conf, layerParamsView, initializeParams)
			ret.ParamTable = paramTable
			ret.Conf = conf
			Return ret
		End Function

		Public Overrides Function getMemoryReport(ByVal inputType As InputType) As LayerMemoryReport
			Dim c As InputType.InputTypeConvolutional = DirectCast(inputType, InputType.InputTypeConvolutional)
			Dim outputType As InputType.InputTypeConvolutional = DirectCast(getOutputType(-1, inputType), InputType.InputTypeConvolutional)

			Return (New LayerMemoryReport.Builder(layerName, GetType(SpaceToBatchLayer), inputType, outputType)).standardMemory(0, 0).cacheMemory(MemoryReport.CACHE_MODE_ALL_ZEROS, MemoryReport.CACHE_MODE_ALL_ZEROS).build()
		End Function

		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			If inputType Is Nothing OrElse inputType.getType() <> InputType.Type.CNN Then
				Throw New System.InvalidOperationException("Invalid input for Subsampling layer (layer name=""" & LayerName & """): Expected CNN input, got " & inputType)
			End If
			Dim i As InputType.InputTypeConvolutional = DirectCast(inputType, InputType.InputTypeConvolutional)
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Return InputType.convolutional((i.getHeight() + padding(0)(0) + padding(0)(1)) / blocks(0), (i.getWidth() + padding(1)(0) + padding(1)(1)) / blocks(1), i.getChannels(), i.getFormat())
		End Function

		Public Overrides Function initializer() As ParamInitializer
			Return EmptyParamInitializer.Instance
		End Function


		Public Overrides Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)
			Preconditions.checkState(inputType.getType() = InputType.Type.CNN, "Only CNN input types can be used with SpaceToBatchLayer, got %s", inputType)
			Me.format = DirectCast(inputType, InputType.InputTypeConvolutional).getFormat()
		End Sub

		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			If inputType Is Nothing Then
				Throw New System.InvalidOperationException("Invalid input for space to batch layer (layer name=""" & LayerName & """): input is null")
			End If
			Return InputTypeUtil.getPreProcessorForInputTypeCnnLayers(inputType, LayerName)
		End Function

		Public Overrides Function isPretrainParam(ByVal paramName As String) As Boolean
			Throw New System.NotSupportedException("SpaceToBatchLayer does not contain parameters")
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor @Getter @Setter public static class Builder<T extends Builder<T>> extends Layer.Builder<T>
		Public Class Builder(Of T As Builder(Of T))
			Inherits Layer.Builder(Of T)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter(AccessLevel.NONE) protected int[] blocks;
'JAVA TO VB CONVERTER NOTE: The field blocks was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend blocks_Conflict() As Integer

			''' <summary>
			''' A 2d array, with format [[padTop, padBottom], [padLeft, padRight]]
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field padding was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend padding_Conflict()() As Integer

			Protected Friend format As CNN2DFormat = CNN2DFormat.NCHW

			''' <param name="blocks"> Block size for SpaceToBatch layer. Should be a length 2 array for the height and width
			''' dimensions </param>
			Public Overridable WriteOnly Property Blocks As Integer()
				Set(ByVal blocks() As Integer)
					Me.blocks_Conflict = ValidationUtils.validate2NonNegative(blocks, False, "blocks")
				End Set
			End Property

			''' <param name="padding"> Padding - should be a 2d array, with format [[padTop, padBottom], [padLeft, padRight]] </param>
			Public Overridable WriteOnly Property Padding As Integer()()
				Set(ByVal padding()() As Integer)
					Me.padding_Conflict = ValidationUtils.validate2x2NonNegative(padding, "padding")
				End Set
			End Property


			''' <param name="blocks"> Block size for SpaceToBatch layer. Should be a length 2 array for the height and width
			''' dimensions </param>
			Public Sub New(ByVal blocks() As Integer)
				Me.Blocks = blocks
				Me.Padding = New Integer()() {
					New Integer() {0, 0},
					New Integer() {0, 0}
				}
			End Sub

			''' <param name="blocks"> Block size for SpaceToBatch layer. Should be a length 2 array for the height and width
			''' dimensions </param>
			''' <param name="padding"> Padding - should be a 2d array, with format [[padTop, padBottom], [padLeft, padRight]] </param>
			Public Sub New(ByVal blocks() As Integer, ByVal padding()() As Integer)
				Me.Blocks = blocks
				Me.Padding = padding
			End Sub

			''' <summary>
			''' Set the data format for the CNN activations - NCHW (channels first) or NHWC (channels last).
			''' See <seealso cref="CNN2DFormat"/> for more details.<br>
			''' Default: NCHW </summary>
			''' <param name="format"> Format for activations (in and out) </param>
			Public Overridable Function dataFormat(ByVal format As CNN2DFormat) As T
				Me.format = format
				Return CType(Me, T)
			End Function

			''' <param name="blocks"> Block size for SpaceToBatch layer. Should be a length 2 array for the height and width
			''' dimensions </param>
'JAVA TO VB CONVERTER NOTE: The parameter blocks was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function blocks(ParamArray ByVal blocks_Conflict() As Integer) As T
				Me.Blocks = blocks_Conflict
				Return CType(Me, T)
			End Function

			''' <param name="padding"> Padding - should be a 2d array, with format [[padTop, padBottom], [padLeft, padRight]] </param>
'JAVA TO VB CONVERTER NOTE: The parameter padding was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function padding(ByVal padding_Conflict()() As Integer) As T
				Me.Padding = padding_Conflict
				Return CType(Me, T)
			End Function

			Public Overrides Function name(ByVal layerName As String) As T
				Me.setLayerName(layerName)
				Return CType(Me, T)
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public SpaceToBatchLayer build()
			Public Overrides Function build() As SpaceToBatchLayer
				If padding_Conflict Is Nothing Then
					Padding = New Integer()() {
						New Integer() {0, 0},
						New Integer() {0, 0}
					}
				End If
				Return New SpaceToBatchLayer(Me)
			End Function
		End Class

	End Class

End Namespace