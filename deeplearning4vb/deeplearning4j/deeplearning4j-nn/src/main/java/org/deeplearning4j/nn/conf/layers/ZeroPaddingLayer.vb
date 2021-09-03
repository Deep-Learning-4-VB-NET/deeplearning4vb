Imports System
Imports System.Collections.Generic
Imports lombok
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports LayerMemoryReport = org.deeplearning4j.nn.conf.memory.LayerMemoryReport
Imports MemoryReport = org.deeplearning4j.nn.conf.memory.MemoryReport
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports ConvolutionUtils = org.deeplearning4j.util.ConvolutionUtils
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
'ORIGINAL LINE: @Data @NoArgsConstructor @EqualsAndHashCode(callSuper = true) public class ZeroPaddingLayer extends NoParamLayer
	<Serializable>
	Public Class ZeroPaddingLayer
		Inherits NoParamLayer

		Private padding() As Integer
		Private dataFormat As CNN2DFormat = CNN2DFormat.NCHW

		Public Sub New(ByVal padTopBottom As Integer, ByVal padLeftRight As Integer)
			Me.New(New Builder(padTopBottom, padLeftRight))
		End Sub

		Public Sub New(ByVal padTop As Integer, ByVal padBottom As Integer, ByVal padLeft As Integer, ByVal padRight As Integer)
			Me.New(New Builder(padTop, padBottom, padLeft, padRight))
		End Sub

		Private Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			If builder.padding_Conflict Is Nothing OrElse builder.padding_Conflict.Length <> 4 Then
				Throw New System.ArgumentException("Invalid padding values: must have exactly 4 values [top, bottom, left, right]." & " Got: " & (If(builder.padding_Conflict Is Nothing, Nothing, Arrays.toString(builder.padding_Conflict))))
			End If

			Me.padding = builder.padding_Conflict
			Me.dataFormat = builder.cnn2DFormat
		End Sub

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As org.deeplearning4j.nn.api.Layer
			Dim ret As New org.deeplearning4j.nn.layers.convolution.ZeroPaddingLayer(conf, networkDataType)
			ret.setListeners(trainingListeners)
			ret.Index = layerIndex
			Dim paramTable As IDictionary(Of String, INDArray) = initializer().init(conf, layerParamsView, initializeParams)
			ret.ParamTable = paramTable
			ret.Conf = conf
			Return ret
		End Function

		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			Dim hwd() As Integer = ConvolutionUtils.getHWDFromInputType(inputType)
			Dim outH As Integer = hwd(0) + padding(0) + padding(1)
			Dim outW As Integer = hwd(1) + padding(2) + padding(3)

			Dim c As InputType.InputTypeConvolutional = DirectCast(inputType, InputType.InputTypeConvolutional)

			Return InputType.convolutional(outH, outW, hwd(2), c.getFormat())
		End Function

		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			Preconditions.checkArgument(inputType IsNot Nothing, "Invalid input for ZeroPaddingLayer layer (layer name=""" & LayerName & """): InputType is null")
			Return InputTypeUtil.getPreProcessorForInputTypeCnnLayers(inputType, LayerName)
		End Function

		Public Overrides Function getMemoryReport(ByVal inputType As InputType) As LayerMemoryReport
			Dim outputType As InputType = getOutputType(-1, inputType)

			Return (New LayerMemoryReport.Builder(layerName, GetType(ZeroPaddingLayer), inputType, outputType)).standardMemory(0, 0).workingMemory(0, 0, MemoryReport.CACHE_MODE_ALL_ZEROS, MemoryReport.CACHE_MODE_ALL_ZEROS).cacheMemory(MemoryReport.CACHE_MODE_ALL_ZEROS, MemoryReport.CACHE_MODE_ALL_ZEROS).build()
		End Function

		Public Overrides Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)
			Dim c As InputType.InputTypeConvolutional = DirectCast(inputType, InputType.InputTypeConvolutional)
			Me.dataFormat = c.getFormat()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter public static class Builder extends Layer.Builder<Builder>
		Public Class Builder
			Inherits Layer.Builder(Of Builder)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter(AccessLevel.NONE) private int[] padding = new int[] {0, 0, 0, 0};
'JAVA TO VB CONVERTER NOTE: The field padding was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend padding_Conflict() As Integer = {0, 0, 0, 0} 'Padding: top, bottom, left, right

			Friend cnn2DFormat As CNN2DFormat = CNN2DFormat.NCHW

			''' <summary>
			''' Set the data format for the CNN activations - NCHW (channels first) or NHWC (channels last).
			''' See <seealso cref="CNN2DFormat"/> for more details.<br>
			''' Default: NCHW </summary>
			''' <param name="format"> Format for activations (in and out) </param>
			Public Overridable Function dataFormat(ByVal format As CNN2DFormat) As Builder
				Me.cnn2DFormat = format
				Return Me
			End Function

			''' <param name="padding"> Padding value for top, bottom, left, and right. Must be length 4 array </param>
			Public Overridable WriteOnly Property Padding As Integer()
				Set(ByVal padding() As Integer)
					Me.padding_Conflict = ValidationUtils.validate4NonNegative(padding, "padding")
				End Set
			End Property

			''' <param name="padHeight"> Padding for both the top and bottom </param>
			''' <param name="padWidth"> Padding for both the left and right </param>
			Public Sub New(ByVal padHeight As Integer, ByVal padWidth As Integer)
				Me.New(padHeight, padHeight, padWidth, padWidth)
			End Sub

			''' <param name="padTop"> Top padding value </param>
			''' <param name="padBottom"> Bottom padding value </param>
			''' <param name="padLeft"> Left padding value </param>
			''' <param name="padRight"> Right padding value </param>
			Public Sub New(ByVal padTop As Integer, ByVal padBottom As Integer, ByVal padLeft As Integer, ByVal padRight As Integer)
				Me.New(New Integer() {padTop, padBottom, padLeft, padRight})
			End Sub

			''' <param name="padding"> Must be a length 1 array with values [paddingAll], a length 2 array with values
			''' [padTopBottom, padLeftRight], or a length 4 array with
			''' values [padTop, padBottom, padLeft, padRight] </param>
			Public Sub New(ByVal padding() As Integer)
				Me.Padding = padding
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public ZeroPaddingLayer build()
			Public Overrides Function build() As ZeroPaddingLayer
				For Each p As Integer In padding_Conflict
					If p < 0 Then
						Throw New System.InvalidOperationException("Invalid zero padding layer config: padding [top, bottom, left, right]" & " must be > 0 for all elements. Got: " & Arrays.toString(padding_Conflict))
					End If
				Next p

				Return New ZeroPaddingLayer(Me)
			End Function
		End Class
	End Class

End Namespace