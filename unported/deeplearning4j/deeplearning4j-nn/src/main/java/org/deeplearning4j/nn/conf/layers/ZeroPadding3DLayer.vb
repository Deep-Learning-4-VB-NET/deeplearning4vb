Imports System
Imports System.Collections.Generic
Imports lombok
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports LayerMemoryReport = org.deeplearning4j.nn.conf.memory.LayerMemoryReport
Imports MemoryReport = org.deeplearning4j.nn.conf.memory.MemoryReport
Imports EmptyParamInitializer = org.deeplearning4j.nn.params.EmptyParamInitializer
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports ValidationUtils = org.deeplearning4j.util.ValidationUtils
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
'ORIGINAL LINE: @Data @NoArgsConstructor @EqualsAndHashCode(callSuper = true) public class ZeroPadding3DLayer extends NoParamLayer
	<Serializable>
	Public Class ZeroPadding3DLayer
		Inherits NoParamLayer

		Private padding() As Integer ' [padLeftD, padRightD, padLeftH, padRightH, padLeftW, padRightW]

		Private Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.padding = builder.padding_Conflict
		End Sub

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal iterationListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As org.deeplearning4j.nn.api.Layer
			Dim ret As New org.deeplearning4j.nn.layers.convolution.ZeroPadding3DLayer(conf, networkDataType)
			ret.setListeners(iterationListeners)
			ret.Index = layerIndex
			Dim paramTable As IDictionary(Of String, INDArray) = initializer().init(conf, layerParamsView, initializeParams)
			ret.ParamTable = paramTable
			ret.Conf = conf
			Return ret
		End Function

		Public Overrides Function initializer() As ParamInitializer
			Return EmptyParamInitializer.Instance
		End Function

		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			If inputType Is Nothing OrElse inputType.getType() <> InputType.Type.CNN3D Then
				Throw New System.InvalidOperationException("Invalid input for 3D CNN layer (layer index = " & layerIndex & ", layer name = """ & LayerName & """): expect CNN3D input type with size > 0. Got: " & inputType)
			End If
			Dim c As InputType.InputTypeConvolutional3D = DirectCast(inputType, InputType.InputTypeConvolutional3D)
			Return InputType.convolutional3D(c.getDepth() + padding(0) + padding(1), c.getHeight() + padding(2) + padding(3), c.getWidth() + padding(4) + padding(5), c.getChannels())
		End Function

		Public Overrides Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)
			'No op
		End Sub

		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			If inputType Is Nothing Then
				Throw New System.InvalidOperationException("Invalid input for ZeroPadding3DLayer layer (layer name=""" & LayerName & """): input is null")
			End If

			Return InputTypeUtil.getPreProcessorForInputTypeCnn3DLayers(inputType, LayerName)
		End Function

		Public Overrides Function isPretrainParam(ByVal paramName As String) As Boolean
			Throw New System.NotSupportedException("ZeroPadding3DLayer does not contain parameters")
		End Function

		Public Overrides Function getMemoryReport(ByVal inputType As InputType) As LayerMemoryReport
			Dim outputType As InputType = getOutputType(-1, inputType)

			Return (New LayerMemoryReport.Builder(layerName, GetType(ZeroPadding3DLayer), inputType, outputType)).standardMemory(0, 0).workingMemory(0, 0, MemoryReport.CACHE_MODE_ALL_ZEROS, MemoryReport.CACHE_MODE_ALL_ZEROS).cacheMemory(MemoryReport.CACHE_MODE_ALL_ZEROS, MemoryReport.CACHE_MODE_ALL_ZEROS).build()
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter public static class Builder extends Layer.Builder<Builder>
		Public Class Builder
			Inherits Layer.Builder(Of Builder)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter(AccessLevel.NONE) private int[] padding = new int[] {0, 0, 0, 0, 0, 0};
'JAVA TO VB CONVERTER NOTE: The field padding was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend padding_Conflict() As Integer = {0, 0, 0, 0, 0, 0}

			''' <summary>
			''' [padLeftD, padRightD, padLeftH, padRightH, padLeftW, padRightW]
			''' </summary>
			Public Overridable WriteOnly Property Padding As Integer()
				Set(ByVal padding() As Integer)
					Me.padding_Conflict = ValidationUtils.validate6NonNegative(padding, "padding")
				End Set
			End Property

			''' <param name="padding"> Padding for both the left and right in all three spatial dimensions </param>
			Public Sub New(ByVal padding As Integer)
				Me.New(padding, padding, padding, padding, padding, padding)
			End Sub


			''' <summary>
			''' Use same padding for left and right boundaries in depth, height and width.
			''' </summary>
			''' <param name="padDepth"> padding used for both depth boundaries </param>
			''' <param name="padHeight"> padding used for both height boundaries </param>
			''' <param name="padWidth"> padding used for both width boudaries </param>
			Public Sub New(ByVal padDepth As Integer, ByVal padHeight As Integer, ByVal padWidth As Integer)
				Me.New(padDepth, padDepth, padHeight, padHeight, padWidth, padWidth)
			End Sub

			''' <summary>
			''' Explicit padding of left and right boundaries in depth, height and width dimensions
			''' </summary>
			''' <param name="padLeftD"> Depth padding left </param>
			''' <param name="padRightD"> Depth padding right </param>
			''' <param name="padLeftH"> Height padding left </param>
			''' <param name="padRightH"> Height padding right </param>
			''' <param name="padLeftW"> Width padding left </param>
			''' <param name="padRightW"> Width padding right </param>
			Public Sub New(ByVal padLeftD As Integer, ByVal padRightD As Integer, ByVal padLeftH As Integer, ByVal padRightH As Integer, ByVal padLeftW As Integer, ByVal padRightW As Integer)
				Me.New(New Integer() {padLeftD, padRightD, padLeftH, padRightH, padLeftW, padRightW})
			End Sub

			Public Sub New(ByVal padding() As Integer)
				Me.Padding = padding
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public ZeroPadding3DLayer build()
			Public Overrides Function build() As ZeroPadding3DLayer
				For Each p As Integer In padding_Conflict
					If p < 0 Then
						Throw New System.InvalidOperationException("Invalid zero padding layer config: padding [left, right]" & " must be > 0 for all elements. Got: " & Arrays.toString(padding_Conflict))
					End If
				Next p
				Return New ZeroPadding3DLayer(Me)
			End Function
		End Class
	End Class

End Namespace