Imports System
Imports System.Collections.Generic
Imports lombok
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports FeedForwardLayer = org.deeplearning4j.nn.conf.layers.FeedForwardLayer
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

Namespace org.deeplearning4j.nn.conf.layers.misc


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @NoArgsConstructor @ToString(callSuper = true) @EqualsAndHashCode(callSuper = true) public class RepeatVector extends org.deeplearning4j.nn.conf.layers.FeedForwardLayer
	<Serializable>
	Public Class RepeatVector
		Inherits FeedForwardLayer

		Private n As Integer = 1
		Private dataFormat As RNNFormat = RNNFormat.NCW

		Protected Friend Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.n = builder.n
			Me.dataFormat = builder.dataFormat
		End Sub

		Public Overrides Function clone() As RepeatVector
			Return CType(MyBase.clone(), RepeatVector)
		End Function

		Public Overrides Function initializer() As ParamInitializer
			Return EmptyParamInitializer.Instance
		End Function

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As org.deeplearning4j.nn.api.Layer
			Dim ret As New org.deeplearning4j.nn.layers.RepeatVector(conf, networkDataType)
			ret.setListeners(trainingListeners)
			ret.Index = layerIndex
			ret.ParamsViewArray = layerParamsView
			Dim paramTable As IDictionary(Of String, INDArray) = initializer().init(conf, layerParamsView, initializeParams)
			ret.ParamTable = paramTable
			ret.Conf = conf
			Return ret
		End Function

		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			If inputType Is Nothing OrElse inputType.getType() <> InputType.Type.FF Then
				Throw New System.InvalidOperationException("Invalid input for RepeatVector layer (layer name=""" & LayerName & """): Expected FF input, got " & inputType)
			End If
			Dim ffInput As InputType.InputTypeFeedForward = DirectCast(inputType, InputType.InputTypeFeedForward)
			Return InputType.recurrent(ffInput.getSize(), n, Me.dataFormat)
		End Function

		Public Overrides Function getMemoryReport(ByVal inputType As InputType) As LayerMemoryReport
			Dim outputType As InputType = getOutputType(-1, inputType)

			Return (New LayerMemoryReport.Builder(layerName, GetType(RepeatVector), inputType, outputType)).standardMemory(0, 0).workingMemory(0, 0, 0, 0).cacheMemory(MemoryReport.CACHE_MODE_ALL_ZEROS, MemoryReport.CACHE_MODE_ALL_ZEROS).build()
		End Function

		Public Overrides Function isPretrainParam(ByVal paramName As String) As Boolean
			Throw New System.NotSupportedException("UpsamplingLayer does not contain parameters")
		End Function



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor @Getter @Setter public static class Builder<T extends Builder<T>> extends org.deeplearning4j.nn.conf.layers.FeedForwardLayer.Builder<T>
		Public Class Builder(Of T As Builder(Of T))
			Inherits FeedForwardLayer.Builder(Of T)

			Friend n As Integer = 1 ' no repetition by default
'JAVA TO VB CONVERTER NOTE: The field dataFormat was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend dataFormat_Conflict As RNNFormat = RNNFormat.NCW
			''' <summary>
			''' Set repetition factor for RepeatVector layer
			''' </summary>
			Public Overridable Property RepetitionFactor As Integer
				Get
					Return n
				End Get
				Set(ByVal n As Integer)
					Me.setN(n)
				End Set
			End Property

			Public Overridable ReadOnly Property DataFormat As RNNFormat
				Get
					Return dataFormat_Conflict
				End Get
			End Property

'JAVA TO VB CONVERTER NOTE: The parameter dataFormat was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function dataFormat(ByVal dataFormat_Conflict As RNNFormat) As Builder
				Me.dataFormat_Conflict = dataFormat_Conflict
				Return Me
			End Function


			Public Sub New(ByVal n As Integer)
				Me.setN(n)
			End Sub

			''' <summary>
			''' Set repetition factor for RepeatVector layer
			''' </summary>
			''' <param name="n"> upsampling size in height and width dimensions </param>
			Public Overridable Function repetitionFactor(ByVal n As Integer) As Builder
				Me.setN(n)
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public RepeatVector build()
			Public Overrides Function build() As RepeatVector
				Return New RepeatVector(Me)
			End Function
		End Class
	End Class

End Namespace