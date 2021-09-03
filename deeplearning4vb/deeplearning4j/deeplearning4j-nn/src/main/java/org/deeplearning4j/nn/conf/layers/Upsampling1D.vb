Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports ToString = lombok.ToString
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
'ORIGINAL LINE: @Data @NoArgsConstructor @ToString(callSuper = true) @EqualsAndHashCode(callSuper = true) public class Upsampling1D extends BaseUpsamplingLayer
	<Serializable>
	Public Class Upsampling1D
		Inherits BaseUpsamplingLayer

		Protected Friend Shadows size() As Integer

		Protected Friend Sub New(ByVal builder As UpsamplingBuilder)
			MyBase.New(builder)
			Me.size = builder.size
		End Sub

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As org.deeplearning4j.nn.api.Layer
			Dim ret As New org.deeplearning4j.nn.layers.convolution.upsampling.Upsampling1D(conf, networkDataType)
			ret.setListeners(trainingListeners)
			ret.Index = layerIndex
			ret.ParamsViewArray = layerParamsView
			Dim paramTable As IDictionary(Of String, INDArray) = initializer().init(conf, layerParamsView, initializeParams)
			ret.ParamTable = paramTable
			ret.Conf = conf
			Return ret
		End Function

		Public Overrides Function clone() As Upsampling1D
'JAVA TO VB CONVERTER NOTE: The local variable clone was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim clone_Conflict As Upsampling1D = CType(MyBase.clone(), Upsampling1D)
			Return clone_Conflict
		End Function

		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			If inputType Is Nothing OrElse inputType.getType() <> InputType.Type.RNN Then
				Throw New System.InvalidOperationException("Invalid input for 1D Upsampling layer (layer index = " & layerIndex & ", layer name = """ & LayerName & """): expect RNN input type with size > 0. Got: " & inputType)
			End If
			Dim recurrent As InputType.InputTypeRecurrent = DirectCast(inputType, InputType.InputTypeRecurrent)
			Dim outLength As Long = recurrent.getTimeSeriesLength()
			If outLength > 0 Then
				outLength *= size(0)
			End If
			Return InputType.recurrent(recurrent.getSize(), outLength)
		End Function

		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			If inputType Is Nothing Then
				Throw New System.InvalidOperationException("Invalid input for Upsampling layer (layer name=""" & LayerName & """): input is null")
			End If
			Return InputTypeUtil.getPreProcessorForInputTypeCnnLayers(inputType, LayerName)
		End Function

		Public Overrides Function getMemoryReport(ByVal inputType As InputType) As LayerMemoryReport
			Dim recurrent As InputType.InputTypeRecurrent = DirectCast(inputType, InputType.InputTypeRecurrent)
			Dim outputType As InputType.InputTypeRecurrent = DirectCast(getOutputType(-1, inputType), InputType.InputTypeRecurrent)

			Dim im2colSizePerEx As Long = recurrent.getSize() * outputType.getTimeSeriesLength() * size(0)
			Dim trainingWorkingSizePerEx As Long = im2colSizePerEx
			If getIDropout() IsNot Nothing Then
				trainingWorkingSizePerEx += inputType.arrayElementsPerExample()
			End If

			Return (New LayerMemoryReport.Builder(layerName, GetType(Upsampling1D), inputType, outputType)).standardMemory(0, 0).workingMemory(0, im2colSizePerEx, 0, trainingWorkingSizePerEx).cacheMemory(MemoryReport.CACHE_MODE_ALL_ZEROS, MemoryReport.CACHE_MODE_ALL_ZEROS).build()
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public static class Builder extends UpsamplingBuilder<Builder>
		Public Class Builder
			Inherits UpsamplingBuilder(Of Builder)

			Public Sub New(ByVal size As Integer)
				MyBase.New(New Integer() {size, size})
			End Sub

			''' <summary>
			''' Upsampling size
			''' </summary>
			''' <param name="size"> upsampling size in single spatial dimension of this 1D layer </param>
'JAVA TO VB CONVERTER NOTE: The parameter size was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function size(ByVal size_Conflict As Integer) As Builder

				Me.Size = New Integer() {size_Conflict}
				Return Me
			End Function

			''' <summary>
			''' Upsampling size int array with a single element. Array must be length 1
			''' </summary>
			''' <param name="size"> upsampling size in single spatial dimension of this 1D layer </param>
'JAVA TO VB CONVERTER NOTE: The parameter size was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function size(ByVal size_Conflict() As Integer) As Builder
				Me.Size = size_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public Upsampling1D build()
			Public Overrides Function build() As Upsampling1D
				Return New Upsampling1D(Me)
			End Function

			Public Overrides WriteOnly Property Size As Integer()
				Set(ByVal size() As Integer)
    
					If size.Length = 2 Then
						If size(0) = size(1) Then
							Me.Size = New Integer(){size(0)}
							Return
						Else
							Preconditions.checkArgument(False, "When given a length 2 array for size, " & "the values must be equal.  Got: " & Arrays.toString(size))
						End If
					End If
    
					Dim temp() As Integer = ValidationUtils.validate1NonNegative(size, "size")
					Me.size = New Integer(){temp(0), temp(0)}
				End Set
			End Property
		End Class

	End Class

End Namespace