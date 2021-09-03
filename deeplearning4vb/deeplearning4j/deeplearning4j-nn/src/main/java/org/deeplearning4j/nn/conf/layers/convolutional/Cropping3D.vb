Imports System
Imports System.Collections.Generic
Imports lombok
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports InputTypeUtil = org.deeplearning4j.nn.conf.layers.InputTypeUtil
Imports Layer = org.deeplearning4j.nn.conf.layers.Layer
Imports NoParamLayer = org.deeplearning4j.nn.conf.layers.NoParamLayer
Imports LayerMemoryReport = org.deeplearning4j.nn.conf.memory.LayerMemoryReport
Imports Cropping3DLayer = org.deeplearning4j.nn.layers.convolution.Cropping3DLayer
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

Namespace org.deeplearning4j.nn.conf.layers.convolutional


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @NoArgsConstructor @EqualsAndHashCode(callSuper = true) public class Cropping3D extends org.deeplearning4j.nn.conf.layers.NoParamLayer
	<Serializable>
	Public Class Cropping3D
		Inherits NoParamLayer

		Private cropping() As Integer

		''' <param name="cropDepth"> Amount of cropping to apply to both depth boundaries of the input activations </param>
		''' <param name="cropHeight"> Amount of cropping to apply to both height boundaries of the input activations </param>
		''' <param name="cropWidth"> Amount of cropping to apply to both width boundaries of the input activations </param>
		Public Sub New(ByVal cropDepth As Integer, ByVal cropHeight As Integer, ByVal cropWidth As Integer)
			Me.New(cropDepth, cropDepth, cropHeight, cropHeight, cropWidth, cropWidth)
		End Sub

		''' <param name="cropLeftD"> Amount of cropping to apply to the left of the depth dimension </param>
		''' <param name="cropRightD"> Amount of cropping to apply to the right of the depth dimension </param>
		''' <param name="cropLeftH"> Amount of cropping to apply to the left of the height dimension </param>
		''' <param name="cropRightH"> Amount of cropping to apply to the right of the height dimension </param>
		''' <param name="cropLeftW"> Amount of cropping to apply to the left of the width dimension </param>
		''' <param name="cropRightW"> Amount of cropping to apply to the right of the width dimension </param>
		Public Sub New(ByVal cropLeftD As Integer, ByVal cropRightD As Integer, ByVal cropLeftH As Integer, ByVal cropRightH As Integer, ByVal cropLeftW As Integer, ByVal cropRightW As Integer)
			Me.New(New Builder(cropLeftD, cropRightD, cropLeftH, cropRightH, cropLeftW, cropRightW))
		End Sub

		''' <param name="cropping"> Cropping as either a length 3 array, with values {@code [cropDepth, cropHeight, cropWidth]}, or
		''' as a length 4 array, with values {@code [cropLeftDepth, cropRightDepth, cropLeftHeight, cropRightHeight,
		''' cropLeftWidth, cropRightWidth]} </param>
		Public Sub New(ByVal cropping() As Integer)
			Me.New(New Builder(cropping))
		End Sub

		Protected Friend Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.cropping = builder.cropping_Conflict
		End Sub

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal iterationListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As org.deeplearning4j.nn.api.Layer
			Dim ret As New Cropping3DLayer(conf, networkDataType)
			ret.setListeners(iterationListeners)
			ret.Index = layerIndex
			Dim paramTable As IDictionary(Of String, INDArray) = initializer().init(conf, layerParamsView, initializeParams)
			ret.ParamTable = paramTable
			ret.Conf = conf
			Return ret
		End Function

		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			If inputType Is Nothing OrElse inputType.getType() <> InputType.Type.CNN3D Then
				Throw New System.InvalidOperationException("Invalid input for 3D cropping layer (layer index = " & layerIndex & ", layer name = """ & LayerName & """): expect CNN3D input type with size > 0. Got: " & inputType)
			End If
			Dim c As InputType.InputTypeConvolutional3D = DirectCast(inputType, InputType.InputTypeConvolutional3D)
			Return InputType.convolutional3D(c.getDepth() - cropping(0) - cropping(1), c.getHeight() - cropping(2) - cropping(3), c.getWidth() - cropping(4) - cropping(5), c.getChannels())
		End Function

		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			Preconditions.checkArgument(inputType IsNot Nothing, "Invalid input for Cropping3D " & "layer (layer name=""" & LayerName & """): InputType is null")
			Return InputTypeUtil.getPreProcessorForInputTypeCnn3DLayers(inputType, LayerName)
		End Function

		Public Overrides Function getMemoryReport(ByVal inputType As InputType) As LayerMemoryReport
			Return Nothing
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter public static class Builder extends org.deeplearning4j.nn.conf.layers.Layer.Builder<Builder>
		Public Class Builder
			Inherits Layer.Builder(Of Builder)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter(AccessLevel.NONE) private int[] cropping = new int[] {0, 0, 0, 0, 0, 0};
'JAVA TO VB CONVERTER NOTE: The field cropping was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend cropping_Conflict() As Integer = {0, 0, 0, 0, 0, 0}

			''' <param name="cropping"> Cropping amount, must be length 1, 3, or 6 array, i.e. either all values, crop depth, crop height, crop width
			''' or crop left depth, crop right depth, crop left height, crop right height, crop left width, crop right width </param>
			Public Overridable WriteOnly Property Cropping As Integer()
				Set(ByVal cropping() As Integer)
					Me.cropping_Conflict = ValidationUtils.validate6NonNegative(cropping, "cropping")
				End Set
			End Property

			Public Sub New()

			End Sub

			''' <param name="cropping"> Cropping amount, must be length 3 or 6 array, i.e. either crop depth, crop height, crop width
			''' or crop left depth, crop right depth, crop left height, crop right height, crop left width, crop right width </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder(@NonNull int[] cropping)
			Public Sub New(ByVal cropping() As Integer)
				Me.Cropping = cropping
			End Sub

			''' <param name="cropDepth"> Amount of cropping to apply to both depth boundaries of the input activations </param>
			''' <param name="cropHeight"> Amount of cropping to apply to both height boundaries of the input activations </param>
			''' <param name="cropWidth"> Amount of cropping to apply to both width boundaries of the input activations </param>
			Public Sub New(ByVal cropDepth As Integer, ByVal cropHeight As Integer, ByVal cropWidth As Integer)
				Me.New(cropDepth, cropDepth, cropHeight, cropHeight, cropWidth, cropWidth)
			End Sub

			''' <param name="cropLeftD"> Amount of cropping to apply to the left of the depth dimension </param>
			''' <param name="cropRightD"> Amount of cropping to apply to the right of the depth dimension </param>
			''' <param name="cropLeftH"> Amount of cropping to apply to the left of the height dimension </param>
			''' <param name="cropRightH"> Amount of cropping to apply to the right of the height dimension </param>
			''' <param name="cropLeftW"> Amount of cropping to apply to the left of the width dimension </param>
			''' <param name="cropRightW"> Amount of cropping to apply to the right of the width dimension </param>
			Public Sub New(ByVal cropLeftD As Integer, ByVal cropRightD As Integer, ByVal cropLeftH As Integer, ByVal cropRightH As Integer, ByVal cropLeftW As Integer, ByVal cropRightW As Integer)
				Me.Cropping = New Integer() {cropLeftD, cropRightD, cropLeftH, cropRightH, cropLeftW, cropRightW}
			End Sub

			Public Overrides Function build() As Cropping3D
				Return New Cropping3D(Me)
			End Function
		End Class
	End Class

End Namespace