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
'ORIGINAL LINE: @Data @NoArgsConstructor @ToString(callSuper = true) @EqualsAndHashCode(callSuper = true) public class SpaceToDepthLayer extends NoParamLayer
	<Serializable>
	Public Class SpaceToDepthLayer
		Inherits NoParamLayer

		<Obsolete("Use <seealso cref=""CNN2DFormat""/> instead")>
		Public NotInheritable Class DataFormat
			Public Shared ReadOnly NCHW As New DataFormat("NCHW", InnerEnum.NCHW)
			Public Shared ReadOnly NHWC As New DataFormat("NHWC", InnerEnum.NHWC)

			Private Shared ReadOnly valueList As New List(Of DataFormat)()

			Shared Sub New()
				valueList.Add(NCHW)
				valueList.Add(NHWC)
			End Sub

			Public Enum InnerEnum
				NCHW
				NHWC
			End Enum

			Public ReadOnly innerEnumValue As InnerEnum
			Private ReadOnly nameValue As String
			Private ReadOnly ordinalValue As Integer
			Private Shared nextOrdinal As Integer = 0

			Private Sub New(ByVal name As String, ByVal thisInnerEnumValue As InnerEnum)
				nameValue = name
				ordinalValue = nextOrdinal
				nextOrdinal += 1
				innerEnumValue = thisInnerEnumValue
			End Sub

			Public Function toFormat() As org.deeplearning4j.nn.conf.CNN2DFormat
				Return If(Me = NCHW, CNN2DFormat.NCHW, CNN2DFormat.NHWC)
			End Function

			Public Shared Function values() As DataFormat()
				Return valueList.ToArray()
			End Function

			Public Function ordinal() As Integer
				Return ordinalValue
			End Function

			Public Overrides Function ToString() As String
				Return nameValue
			End Function

			Public Shared Operator =(ByVal one As DataFormat, ByVal two As DataFormat) As Boolean
				Return one.innerEnumValue = two.innerEnumValue
			End Operator

			Public Shared Operator <>(ByVal one As DataFormat, ByVal two As DataFormat) As Boolean
				Return one.innerEnumValue <> two.innerEnumValue
			End Operator

			Public Shared Function valueOf(ByVal name As String) As DataFormat
				For Each enumInstance As DataFormat In DataFormat.valueList
					If enumInstance.nameValue = name Then
						Return enumInstance
					End If
				Next
				Throw New System.ArgumentException(name)
			End Function
		End Class

		Protected Friend blockSize As Integer
		Protected Friend dataFormat As CNN2DFormat


		Protected Friend Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.setBlockSize(builder.blockSize)
			Me.setDataFormat(builder.dataFormat)
		End Sub

		Public Overrides Function clone() As SpaceToDepthLayer
			Return CType(MyBase.clone(), SpaceToDepthLayer)
		End Function

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As org.deeplearning4j.nn.api.Layer
			Dim ret As New org.deeplearning4j.nn.layers.convolution.SpaceToDepth(conf, networkDataType)
			ret.setListeners(trainingListeners)
			ret.Index = layerIndex
			ret.ParamsViewArray = layerParamsView
			Dim paramTable As IDictionary(Of String, INDArray) = initializer().init(conf, layerParamsView, initializeParams)
			ret.ParamTable = paramTable
			ret.Conf = conf
			Return ret
		End Function

		Public Overrides Function getMemoryReport(ByVal inputType As InputType) As LayerMemoryReport
			Dim outputType As InputType.InputTypeConvolutional = DirectCast(getOutputType(-1, inputType), InputType.InputTypeConvolutional)

			Return (New LayerMemoryReport.Builder(layerName, GetType(SpaceToDepthLayer), inputType, outputType)).standardMemory(0, 0).cacheMemory(MemoryReport.CACHE_MODE_ALL_ZEROS, MemoryReport.CACHE_MODE_ALL_ZEROS).build()
		End Function

		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			If inputType Is Nothing OrElse inputType.getType() <> InputType.Type.CNN Then
				Throw New System.InvalidOperationException("Invalid input for space to channels layer (layer name=""" & LayerName & """): Expected CNN input, got " & inputType)
			End If
			Dim i As InputType.InputTypeConvolutional = DirectCast(inputType, InputType.InputTypeConvolutional)
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Return InputType.convolutional(i.getHeight() / blockSize, i.getWidth() / blockSize, i.getChannels() * blockSize * blockSize, i.getFormat())
		End Function

		Public Overrides Function initializer() As ParamInitializer
			Return EmptyParamInitializer.Instance
		End Function


		Public Overrides Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)
			Me.dataFormat = DirectCast(inputType, InputType.InputTypeConvolutional).getFormat()
		End Sub

		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			If inputType Is Nothing Then
				Throw New System.InvalidOperationException("Invalid input for space to channels layer (layer name=""" & LayerName & """): input is null")
			End If
			Return InputTypeUtil.getPreProcessorForInputTypeCnnLayers(inputType, LayerName)
		End Function

		Public Overrides Function isPretrainParam(ByVal paramName As String) As Boolean
			Throw New System.NotSupportedException("SpaceToDepthLayer does not contain parameters")
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor @Getter @Setter public static class Builder<T extends Builder<T>> extends Layer.Builder<T>
		Public Class Builder(Of T As Builder(Of T))
			Inherits Layer.Builder(Of T)

			Protected Friend blockSize As Integer

			''' <summary>
			''' Data format for input activations. Note DL4J uses NCHW in most cases
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field dataFormat was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend dataFormat_Conflict As CNN2DFormat = CNN2DFormat.NCHW

			''' <param name="blockSize"> Block size </param>
			Public Sub New(ByVal blockSize As Integer)
				Me.setBlockSize(blockSize)
			End Sub

			''' <param name="blockSize"> Block size </param>
			''' <param name="dataFormat"> Data format for input activations. Note DL4J uses NCHW in most cases </param>
			<Obsolete>
			Public Sub New(ByVal blockSize As Integer, ByVal dataFormat As DataFormat)
				Me.New(blockSize, dataFormat.toFormat())
			End Sub

			Public Sub New(ByVal blockSize As Integer, ByVal dataFormat As CNN2DFormat)
				Me.setBlockSize(blockSize)
				Me.setDataFormat(dataFormat)
			End Sub

			''' <param name="blockSize"> Block size </param>
			Public Overridable Function blocks(ByVal blockSize As Integer) As T
				Me.setBlockSize(blockSize)
				Return CType(Me, T)
			End Function

			''' <param name="dataFormat"> Data format for input activations. Note DL4J uses NCHW in most cases </param>
			''' @deprecated Use <seealso cref="dataFormat(CNN2DFormat)"/> 
'JAVA TO VB CONVERTER NOTE: The parameter dataFormat was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			<Obsolete("Use <seealso cref=""dataFormat(CNN2DFormat)""/>")>
			Public Overridable Function dataFormat(ByVal dataFormat_Conflict As DataFormat) As T
				Return dataFormat(dataFormat_Conflict.toFormat())
			End Function

			''' <summary>
			''' Set the data format for the CNN activations - NCHW (channels first) or NHWC (channels last).
			''' See <seealso cref="CNN2DFormat"/> for more details.<br>
			''' Default: NCHW </summary>
			''' <param name="dataFormat"> Format for activations (in and out) </param>
'JAVA TO VB CONVERTER NOTE: The parameter dataFormat was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function dataFormat(ByVal dataFormat_Conflict As CNN2DFormat) As T
				Me.setDataFormat(dataFormat_Conflict)
				Return CType(Me, T)
			End Function

			Public Overrides Function name(ByVal layerName As String) As T
				Me.setLayerName(layerName)
				Return CType(Me, T)
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public SpaceToDepthLayer build()
			Public Overrides Function build() As SpaceToDepthLayer
				Return New SpaceToDepthLayer(Me)
			End Function
		End Class

	End Class

End Namespace