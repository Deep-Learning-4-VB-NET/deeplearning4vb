Imports System
Imports System.Collections.Generic
Imports lombok
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports LayerMemoryReport = org.deeplearning4j.nn.conf.memory.LayerMemoryReport
Imports MemoryReport = org.deeplearning4j.nn.conf.memory.MemoryReport
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
'ORIGINAL LINE: @Data @NoArgsConstructor @ToString(callSuper = true) @EqualsAndHashCode(callSuper = true) public class Upsampling3D extends BaseUpsamplingLayer
	<Serializable>
	Public Class Upsampling3D
		Inherits BaseUpsamplingLayer

		Protected Friend Shadows size() As Integer
		Protected Friend dataFormat As Convolution3D.DataFormat = Convolution3D.DataFormat.NCDHW 'Default to NCDHW for 1.0.0-beta4 and earlier, when no config existed (NCDHW only)



		Protected Friend Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.size = builder.size
			Me.dataFormat = builder.dataFormat_Conflict
		End Sub

		Public Overrides Function clone() As Upsampling3D
			Return CType(MyBase.clone(), Upsampling3D)
		End Function

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal iterationListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As org.deeplearning4j.nn.api.Layer
			Dim ret As New org.deeplearning4j.nn.layers.convolution.upsampling.Upsampling3D(conf, networkDataType)
			ret.setListeners(iterationListeners)
			ret.Index = layerIndex
			ret.ParamsViewArray = layerParamsView
			Dim paramTable As IDictionary(Of String, INDArray) = initializer().init(conf, layerParamsView, initializeParams)
			ret.ParamTable = paramTable
			ret.Conf = conf
			Return ret
		End Function

		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			If inputType Is Nothing OrElse inputType.getType() <> InputType.Type.CNN3D Then
				Throw New System.InvalidOperationException("Invalid input for Upsampling 3D layer (layer name=""" & LayerName & """): Expected CNN3D input, got " & inputType)
			End If
			Dim i As InputType.InputTypeConvolutional3D = DirectCast(inputType, InputType.InputTypeConvolutional3D)

			Dim inHeight As Long = CInt(Math.Truncate(i.getHeight()))
			Dim inWidth As Long = CInt(Math.Truncate(i.getWidth()))
			Dim inDepth As Long = CInt(Math.Truncate(i.getDepth()))
			Dim inChannels As Long = CInt(Math.Truncate(i.getChannels()))

			Return InputType.convolutional3D(size(0) * inDepth, size(1) * inHeight, size(2) * inWidth, inChannels)
		End Function

		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			If inputType Is Nothing Then
				Throw New System.InvalidOperationException("Invalid input for Upsampling 3D layer (layer name=""" & LayerName & """): input is null")
			End If
			Return InputTypeUtil.getPreProcessorForInputTypeCnn3DLayers(inputType, LayerName)
		End Function

		Public Overrides Function getMemoryReport(ByVal inputType As InputType) As LayerMemoryReport
			Dim c As InputType.InputTypeConvolutional3D = DirectCast(inputType, InputType.InputTypeConvolutional3D)
			Dim outputType As InputType.InputTypeConvolutional3D = DirectCast(getOutputType(-1, inputType), InputType.InputTypeConvolutional3D)

			' During forward pass: im2col array + reduce. Reduce is counted as activations, so only im2col is working mem
			Dim im2colSizePerEx As val = c.getChannels() And outputType.getDepth() * outputType.getHeight() * outputType.getWidth() * size(0) * size(1) * size(2)

			' Current implementation does NOT cache im2col etc... which means: it's recalculated on each backward pass
			Dim trainingWorkingSizePerEx As Long = im2colSizePerEx
			If getIDropout() IsNot Nothing Then
				'Dup on the input before dropout, but only for training
				trainingWorkingSizePerEx += inputType.arrayElementsPerExample()
			End If

			Return (New LayerMemoryReport.Builder(layerName, GetType(Upsampling3D), inputType, outputType)).standardMemory(0, 0).workingMemory(0, im2colSizePerEx, 0, trainingWorkingSizePerEx).cacheMemory(MemoryReport.CACHE_MODE_ALL_ZEROS, MemoryReport.CACHE_MODE_ALL_ZEROS).build()
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public static class Builder extends UpsamplingBuilder<Builder>
		Public Class Builder
			Inherits UpsamplingBuilder(Of Builder)

'JAVA TO VB CONVERTER NOTE: The field dataFormat was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend dataFormat_Conflict As Convolution3D.DataFormat = Convolution3D.DataFormat.NCDHW

			''' <param name="size"> Upsampling layer size (most common value: 2) </param>
			Public Sub New(ByVal size As Integer)
				MyBase.New(New Integer() {size, size, size})
			End Sub

			''' <param name="dataFormat"> Data format - see <seealso cref="Convolution3D.DataFormat"/> for more details </param>
			''' <param name="size"> Upsampling layer size (most common value: 2) </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder(@NonNull Convolution3D.DataFormat dataFormat, int size)
			Public Sub New(ByVal dataFormat As Convolution3D.DataFormat, ByVal size As Integer)
				MyBase.New(New Integer(){size, size, size})
				Me.dataFormat_Conflict = dataFormat
			End Sub

			''' <summary>
			''' Sets the DataFormat. See <seealso cref="Convolution3D.DataFormat"/> for more details
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder dataFormat(@NonNull Convolution3D.DataFormat dataFormat)
'JAVA TO VB CONVERTER NOTE: The parameter dataFormat was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function dataFormat(ByVal dataFormat_Conflict As Convolution3D.DataFormat) As Builder
				Me.dataFormat_Conflict = dataFormat_Conflict
				Return Me
			End Function

			''' <summary>
			''' Upsampling size as int, so same upsampling size is used for depth, width and height
			''' </summary>
			''' <param name="size"> upsampling size in height, width and depth dimensions </param>
'JAVA TO VB CONVERTER NOTE: The parameter size was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function size(ByVal size_Conflict As Integer) As Builder

				Me.Size = New Integer() {size_Conflict, size_Conflict, size_Conflict}
				Return Me
			End Function

			''' <summary>
			''' Upsampling size as int, so same upsampling size is used for depth, width and height
			''' </summary>
			''' <param name="size"> upsampling size in height, width and depth dimensions </param>
'JAVA TO VB CONVERTER NOTE: The parameter size was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function size(ByVal size_Conflict() As Integer) As Builder
				Preconditions.checkArgument(size_Conflict.Length = 3)
				Me.Size = size_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public Upsampling3D build()
			Public Overrides Function build() As Upsampling3D
				Return New Upsampling3D(Me)
			End Function

			Public Overrides WriteOnly Property Size As Integer()
				Set(ByVal size() As Integer)
					Me.size = ValidationUtils.validate3NonNegative(size, "size")
				End Set
			End Property
		End Class

	End Class

End Namespace