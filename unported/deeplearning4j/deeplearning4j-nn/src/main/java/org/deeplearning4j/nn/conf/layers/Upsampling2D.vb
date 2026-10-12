Imports System
Imports System.Collections.Generic
Imports lombok
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports LayerMemoryReport = org.deeplearning4j.nn.conf.memory.LayerMemoryReport
Imports MemoryReport = org.deeplearning4j.nn.conf.memory.MemoryReport
Imports LegacyIntArrayDeserializer = org.deeplearning4j.nn.conf.serde.legacy.LegacyIntArrayDeserializer
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports ValidationUtils = org.deeplearning4j.util.ValidationUtils
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports JsonDeserialize = org.nd4j.shade.jackson.databind.annotation.JsonDeserialize

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
'ORIGINAL LINE: @Data @NoArgsConstructor @ToString(callSuper = true) @EqualsAndHashCode(callSuper = true) public class Upsampling2D extends BaseUpsamplingLayer
	<Serializable>
	Public Class Upsampling2D
		Inherits BaseUpsamplingLayer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonDeserialize(using = org.deeplearning4j.nn.conf.serde.legacy.LegacyIntArrayDeserializer.class) protected int[] size;
		Protected Friend Shadows size() As Integer
		Protected Friend format As CNN2DFormat = CNN2DFormat.NCHW

		Protected Friend Sub New(ByVal builder As UpsamplingBuilder)
			MyBase.New(builder)
			Me.size = builder.size
			Me.format = CType(builder, Builder).format
		End Sub

		Public Overrides Function clone() As Upsampling2D
'JAVA TO VB CONVERTER NOTE: The local variable clone was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim clone_Conflict As Upsampling2D = CType(MyBase.clone(), Upsampling2D)
			Return clone_Conflict
		End Function

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As org.deeplearning4j.nn.api.Layer
			Dim ret As New org.deeplearning4j.nn.layers.convolution.upsampling.Upsampling2D(conf, networkDataType)
			ret.setListeners(trainingListeners)
			ret.Index = layerIndex
			ret.ParamsViewArray = layerParamsView
			Dim paramTable As IDictionary(Of String, INDArray) = initializer().init(conf, layerParamsView, initializeParams)
			ret.ParamTable = paramTable
			ret.Conf = conf
			Return ret
		End Function

		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			If inputType Is Nothing OrElse inputType.getType() <> InputType.Type.CNN Then
				Throw New System.InvalidOperationException("Invalid input for Upsampling 2D layer (layer name=""" & LayerName & """): Expected CNN input, got " & inputType)
			End If
			Dim i As InputType.InputTypeConvolutional = DirectCast(inputType, InputType.InputTypeConvolutional)
			Dim inHeight As val = i.getHeight()
			Dim inWidth As val = i.getWidth()
			Dim inDepth As val = i.getChannels()

			Return InputType.convolutional(size(0) * inHeight, size(1) * inWidth, inDepth, i.getFormat())
		End Function

		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			If inputType Is Nothing Then
				Throw New System.InvalidOperationException("Invalid input for Upsampling 2D layer (layer name=""" & LayerName & """): input is null")
			End If
			Return InputTypeUtil.getPreProcessorForInputTypeCnnLayers(inputType, LayerName)
		End Function

		Public Overrides Function getMemoryReport(ByVal inputType As InputType) As LayerMemoryReport
			Dim c As InputType.InputTypeConvolutional = DirectCast(inputType, InputType.InputTypeConvolutional)
			Dim outputType As InputType.InputTypeConvolutional = DirectCast(getOutputType(-1, inputType), InputType.InputTypeConvolutional)

			' During forward pass: im2col array + reduce. Reduce is counted as activations, so only im2col is working mem
			Dim im2colSizePerEx As val = c.getChannels() * outputType.getHeight() * outputType.getWidth() * size(0) * size(1)

			' Current implementation does NOT cache im2col etc... which means: it's recalculated on each backward pass
			Dim trainingWorkingSizePerEx As Long = im2colSizePerEx
			If getIDropout() IsNot Nothing Then
				'Dup on the input before dropout, but only for training
				trainingWorkingSizePerEx += inputType.arrayElementsPerExample()
			End If

			Return (New LayerMemoryReport.Builder(layerName, GetType(Upsampling2D), inputType, outputType)).standardMemory(0, 0).workingMemory(0, im2colSizePerEx, 0, trainingWorkingSizePerEx).cacheMemory(MemoryReport.CACHE_MODE_ALL_ZEROS, MemoryReport.CACHE_MODE_ALL_ZEROS).build()
		End Function

		Public Overrides Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)
			If inputType Is Nothing OrElse inputType.getType() <> InputType.Type.CNN Then
				Throw New System.InvalidOperationException("Invalid input for Upsampling 2D layer (layer name=""" & LayerName & """): Expected CNN input, got " & inputType)
			End If
			Me.format = DirectCast(inputType, InputType.InputTypeConvolutional).getFormat()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public static class Builder extends UpsamplingBuilder<Builder>
		Public Class Builder
			Inherits UpsamplingBuilder(Of Builder)

			Protected Friend format As CNN2DFormat = CNN2DFormat.NCHW

			Public Sub New(ByVal size As Integer)
				MyBase.New(New Integer() {size, size})
			End Sub

			''' <summary>
			''' Set the data format for the CNN activations - NCHW (channels first) or NHWC (channels last).
			''' See <seealso cref="CNN2DFormat"/> for more details.<br>
			''' Default: NCHW </summary>
			''' <param name="format"> Format for activations (in and out) </param>
			Public Overridable Function dataFormat(ByVal format As CNN2DFormat) As Builder
				Me.format = format
				Return Me
			End Function

			''' <summary>
			''' Upsampling size int, used for both height and width
			''' </summary>
			''' <param name="size"> upsampling size in height and width dimensions </param>
'JAVA TO VB CONVERTER NOTE: The parameter size was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function size(ByVal size_Conflict As Integer) As Builder

				Me.setSize(size_Conflict, size_Conflict)
				Return Me
			End Function


			''' <summary>
			''' Upsampling size array
			''' </summary>
			''' <param name="size"> upsampling size in height and width dimensions </param>
'JAVA TO VB CONVERTER NOTE: The parameter size was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function size(ByVal size_Conflict() As Integer) As Builder
				Me.Size = size_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public Upsampling2D build()
			Public Overrides Function build() As Upsampling2D
				Return New Upsampling2D(Me)
			End Function

			Public Overrides WriteOnly Property Size As Integer()
				Set(ByVal size() As Integer)
					Me.size = ValidationUtils.validate2NonNegative(size, False, "size")
				End Set
			End Property
		End Class

	End Class

End Namespace