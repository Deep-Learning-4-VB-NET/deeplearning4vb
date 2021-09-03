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
Imports Cropping1DLayer = org.deeplearning4j.nn.layers.convolution.Cropping1DLayer
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
'ORIGINAL LINE: @Data @NoArgsConstructor @EqualsAndHashCode(callSuper = true) public class Cropping1D extends org.deeplearning4j.nn.conf.layers.NoParamLayer
	<Serializable>
	Public Class Cropping1D
		Inherits NoParamLayer

		Private cropping() As Integer

		''' <param name="cropTopBottom"> Amount of cropping to apply to both the top and the bottom of the input activations </param>
		Public Sub New(ByVal cropTopBottom As Integer)
			Me.New(cropTopBottom, cropTopBottom)
		End Sub

		''' <param name="cropTop"> Amount of cropping to apply to the top of the input activations </param>
		''' <param name="cropBottom"> Amount of cropping to apply to the bottom of the input activations </param>
		Public Sub New(ByVal cropTop As Integer, ByVal cropBottom As Integer)
			Me.New(New Builder(cropTop, cropBottom))
		End Sub

		''' <param name="cropping"> Cropping as a length 2 array, with values {@code [cropTop, cropBottom]} </param>
		Public Sub New(ByVal cropping() As Integer)
			Me.New(New Builder(cropping))
		End Sub

		Protected Friend Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.cropping = builder.cropping_Conflict
		End Sub

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As org.deeplearning4j.nn.api.Layer
			Dim ret As New Cropping1DLayer(conf, networkDataType)
			ret.setListeners(trainingListeners)
			ret.Index = layerIndex
			Dim paramTable As IDictionary(Of String, INDArray) = initializer().init(conf, layerParamsView, initializeParams)
			ret.ParamTable = paramTable
			ret.Conf = conf
			Return ret
		End Function

		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			If inputType Is Nothing OrElse inputType.getType() <> InputType.Type.RNN Then
				Throw New System.InvalidOperationException("Invalid input for 1D Cropping layer (layer index = " & layerIndex & ", layer name = """ & LayerName & """): expect RNN input type with size > 0. Got: " & inputType)
			End If
			Dim cnn1d As InputType.InputTypeRecurrent = DirectCast(inputType, InputType.InputTypeRecurrent)
			Dim length As val = cnn1d.getTimeSeriesLength()
			Dim outLength As val = length - cropping(0) - cropping(1)
			Return InputType.recurrent(cnn1d.getSize(), outLength)
		End Function

		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			Preconditions.checkArgument(inputType IsNot Nothing, "Invalid input for Cropping1D layer (layer name=""" & LayerName & """): InputType is null")
			Return InputTypeUtil.getPreProcessorForInputTypeCnnLayers(inputType, LayerName)
		End Function

		Public Overrides Function getMemoryReport(ByVal inputType As InputType) As LayerMemoryReport
			Return Nothing
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter public static class Builder extends org.deeplearning4j.nn.conf.layers.Layer.Builder<Builder>
		Public Class Builder
			Inherits Layer.Builder(Of Builder)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter(AccessLevel.NONE) private int[] cropping = new int[] {0, 0};
'JAVA TO VB CONVERTER NOTE: The field cropping was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend cropping_Conflict() As Integer = {0, 0}

			''' <param name="cropping"> Cropping amount for top/bottom (in that order). Must be length 1 or 2 array. </param>
			Public Overridable WriteOnly Property Cropping As Integer()
				Set(ByVal cropping() As Integer)
					Me.cropping_Conflict = ValidationUtils.validate2NonNegative(cropping, True,"cropping")
				End Set
			End Property

			Public Sub New()

			End Sub

			''' <param name="cropping"> Cropping amount for top/bottom (in that order). Must be length 1 or 2 array. </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder(@NonNull int[] cropping)
			Public Sub New(ByVal cropping() As Integer)
				Me.Cropping = cropping
			End Sub

			''' <param name="cropTopBottom"> Amount of cropping to apply to both the top and the bottom of the input activations </param>
			Public Sub New(ByVal cropTopBottom As Integer)
				Me.New(cropTopBottom, cropTopBottom)
			End Sub

			''' <param name="cropTop"> Amount of cropping to apply to the top of the input activations </param>
			''' <param name="cropBottom"> Amount of cropping to apply to the bottom of the input activations </param>
			Public Sub New(ByVal cropTop As Integer, ByVal cropBottom As Integer)
				Me.Cropping = New Integer(){cropTop, cropBottom}
			End Sub

			Public Overrides Function build() As Cropping1D
				Return New Cropping1D(Me)
			End Function
		End Class
	End Class

End Namespace