Imports System
Imports System.Collections.Generic
Imports lombok
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports InputTypeUtil = org.deeplearning4j.nn.conf.layers.InputTypeUtil
Imports Layer = org.deeplearning4j.nn.conf.layers.Layer
Imports NoParamLayer = org.deeplearning4j.nn.conf.layers.NoParamLayer
Imports LayerMemoryReport = org.deeplearning4j.nn.conf.memory.LayerMemoryReport
Imports Cropping2DLayer = org.deeplearning4j.nn.layers.convolution.Cropping2DLayer
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

Namespace org.deeplearning4j.nn.conf.layers.convolutional


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @NoArgsConstructor @EqualsAndHashCode(callSuper = true) public class Cropping2D extends org.deeplearning4j.nn.conf.layers.NoParamLayer
	<Serializable>
	Public Class Cropping2D
		Inherits NoParamLayer

		Private cropping() As Integer
		Private dataFormat As CNN2DFormat = CNN2DFormat.NCHW

		''' <param name="cropTopBottom"> Amount of cropping to apply to both the top and the bottom of the input activations </param>
		''' <param name="cropLeftRight"> Amount of cropping to apply to both the left and the right of the input activations </param>
		Public Sub New(ByVal cropTopBottom As Integer, ByVal cropLeftRight As Integer)
			Me.New(cropTopBottom, cropTopBottom, cropLeftRight, cropLeftRight)
		End Sub

		Public Sub New(ByVal dataFormat As CNN2DFormat, ByVal cropTopBottom As Integer, ByVal cropLeftRight As Integer)
			Me.New(dataFormat, cropTopBottom, cropTopBottom, cropLeftRight, cropLeftRight)
		End Sub

		''' <param name="cropTop"> Amount of cropping to apply to the top of the input activations </param>
		''' <param name="cropBottom"> Amount of cropping to apply to the bottom of the input activations </param>
		''' <param name="cropLeft"> Amount of cropping to apply to the left of the input activations </param>
		''' <param name="cropRight"> Amount of cropping to apply to the right of the input activations </param>
		Public Sub New(ByVal cropTop As Integer, ByVal cropBottom As Integer, ByVal cropLeft As Integer, ByVal cropRight As Integer)
			Me.New(CNN2DFormat.NCHW, cropTop, cropBottom, cropLeft, cropRight)
		End Sub

		Public Sub New(ByVal format As CNN2DFormat, ByVal cropTop As Integer, ByVal cropBottom As Integer, ByVal cropLeft As Integer, ByVal cropRight As Integer)
			Me.New((New Builder(cropTop, cropBottom, cropLeft, cropRight)).dataFormat(format))
		End Sub

		''' <param name="cropping"> Cropping as either a length 2 array, with values {@code [cropTopBottom, cropLeftRight]}, or as a
		''' length 4 array, with values {@code [cropTop, cropBottom, cropLeft, cropRight]} </param>
		Public Sub New(ByVal cropping() As Integer)
			Me.New(New Builder(cropping))
		End Sub

		Protected Friend Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.cropping = builder.cropping_Conflict
			Me.dataFormat = builder.cnn2DFormat
		End Sub

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As org.deeplearning4j.nn.api.Layer
			Dim ret As New Cropping2DLayer(conf, networkDataType)
			ret.setListeners(trainingListeners)
			ret.Index = layerIndex
			Dim paramTable As IDictionary(Of String, INDArray) = initializer().init(conf, layerParamsView, initializeParams)
			ret.ParamTable = paramTable
			ret.Conf = conf
			Return ret
		End Function

		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			Dim hwd() As Integer = ConvolutionUtils.getHWDFromInputType(inputType)
			Dim outH As Integer = hwd(0) - cropping(0) - cropping(1)
			Dim outW As Integer = hwd(1) - cropping(2) - cropping(3)

			Dim c As InputType.InputTypeConvolutional = DirectCast(inputType, InputType.InputTypeConvolutional)

			Return InputType.convolutional(outH, outW, hwd(2), c.getFormat())
		End Function

		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			Preconditions.checkArgument(inputType IsNot Nothing, "Invalid input for Cropping2D layer (layer name=""" & LayerName & """): InputType is null")
			Return InputTypeUtil.getPreProcessorForInputTypeCnnLayers(inputType, LayerName)
		End Function

		Public Overrides Function getMemoryReport(ByVal inputType As InputType) As LayerMemoryReport
			Return Nothing
		End Function

		Public Overrides Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)
			Me.dataFormat = DirectCast(inputType, InputType.InputTypeConvolutional).getFormat()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter public static class Builder extends org.deeplearning4j.nn.conf.layers.Layer.Builder<Builder>
		Public Class Builder
			Inherits Layer.Builder(Of Builder)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter(AccessLevel.NONE) private int[] cropping = new int[] {0, 0, 0, 0};
'JAVA TO VB CONVERTER NOTE: The field cropping was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend cropping_Conflict() As Integer = {0, 0, 0, 0}

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

			''' <param name="cropping"> Cropping amount for top/bottom/left/right (in that order). Must be length 1, 2, or 4 array. </param>
			Public Overridable WriteOnly Property Cropping As Integer()
				Set(ByVal cropping() As Integer)
					Me.cropping_Conflict = ValidationUtils.validate4NonNegative(cropping, "cropping")
				End Set
			End Property

			Public Sub New()

			End Sub

			''' <param name="cropping"> Cropping amount for top/bottom/left/right (in that order). Must be length 4 array. </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder(@NonNull int[] cropping)
			Public Sub New(ByVal cropping() As Integer)
				Me.Cropping = cropping
			End Sub

			''' <param name="cropTopBottom"> Amount of cropping to apply to both the top and the bottom of the input activations </param>
			''' <param name="cropLeftRight"> Amount of cropping to apply to both the left and the right of the input activations </param>
			Public Sub New(ByVal cropTopBottom As Integer, ByVal cropLeftRight As Integer)
				Me.New(cropTopBottom, cropTopBottom, cropLeftRight, cropLeftRight)
			End Sub

			''' <param name="cropTop"> Amount of cropping to apply to the top of the input activations </param>
			''' <param name="cropBottom"> Amount of cropping to apply to the bottom of the input activations </param>
			''' <param name="cropLeft"> Amount of cropping to apply to the left of the input activations </param>
			''' <param name="cropRight"> Amount of cropping to apply to the right of the input activations </param>
			Public Sub New(ByVal cropTop As Integer, ByVal cropBottom As Integer, ByVal cropLeft As Integer, ByVal cropRight As Integer)
				Me.Cropping = New Integer() {cropTop, cropBottom, cropLeft, cropRight}
			End Sub

			Public Overrides Function build() As Cropping2D
				Return New Cropping2D(Me)
			End Function
		End Class
	End Class

End Namespace