Imports System
Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports NonNull = lombok.NonNull
Imports CacheMode = org.deeplearning4j.nn.conf.CacheMode
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType

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

Namespace org.deeplearning4j.nn.conf.memory


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @AllArgsConstructor @NoArgsConstructor public class LayerMemoryReport extends MemoryReport
	Public Class LayerMemoryReport
		Inherits MemoryReport

		Private layerName As String
		Private layerType As Type
		Private inputType As InputType
		Private outputType As InputType

		'Standard memory (in terms of total ND4J array length)
		Private parameterSize As Long
		Private updaterStateSize As Long

		'Working memory (in ND4J array length)
		'Note that *working* memory may be reduced by caching (which is only used during train mode)
		Private workingMemoryFixedInference As Long
		Private workingMemoryVariableInference As Long
		Private workingMemoryFixedTrain As IDictionary(Of CacheMode, Long)
		Private workingMemoryVariableTrain As IDictionary(Of CacheMode, Long)

		'Cache memory, by cache mode:
		Friend cacheModeMemFixed As IDictionary(Of CacheMode, Long)
		Friend cacheModeMemVariablePerEx As IDictionary(Of CacheMode, Long)

		Protected Friend Sub New(ByVal b As Builder)
			Me.layerName = b.layerName
			Me.layerType = b.layerType
			Me.inputType = b.inputType
			Me.outputType = b.outputType

			Me.parameterSize = b.parameterSize
			Me.updaterStateSize = b.updaterStateSize

			Me.workingMemoryFixedInference = b.workingMemoryFixedInference
			Me.workingMemoryVariableInference = b.workingMemoryVariableInference
			Me.workingMemoryFixedTrain = b.workingMemoryFixedTrain
			Me.workingMemoryVariableTrain = b.workingMemoryVariableTrain

			Me.cacheModeMemFixed = b.cacheModeMemFixed
			Me.cacheModeMemVariablePerEx = b.cacheModeMemVariablePerEx
		End Sub

		Public Overrides ReadOnly Property ReportClass As Type
			Get
				Return layerType
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return layerName
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public long getTotalMemoryBytes(int minibatchSize, @NonNull MemoryUseMode memoryUseMode, @NonNull CacheMode cacheMode, @NonNull DataType dataType)
		Public Overrides Function getTotalMemoryBytes(ByVal minibatchSize As Integer, ByVal memoryUseMode As MemoryUseMode, ByVal cacheMode As CacheMode, ByVal dataType As DataType) As Long
			Dim total As Long = 0
			For Each mt As MemoryType In MemoryType.values()
				total += getMemoryBytes(mt, minibatchSize, memoryUseMode, cacheMode, dataType)
			Next mt
			Return total
		End Function

		Public Overrides Function getMemoryBytes(ByVal memoryType As MemoryType, ByVal minibatchSize As Integer, ByVal memoryUseMode As MemoryUseMode, ByVal cacheMode As CacheMode, ByVal dataType As DataType) As Long
			Dim bytesPerElement As Integer = getBytesPerElement(dataType)
			Select Case memoryType.innerEnumValue
				Case org.deeplearning4j.nn.conf.memory.MemoryType.InnerEnum.PARAMETERS
					Return parameterSize * bytesPerElement
				Case org.deeplearning4j.nn.conf.memory.MemoryType.InnerEnum.PARAMATER_GRADIENTS
					If memoryUseMode = MemoryUseMode.INFERENCE Then
						Return 0
					End If
					Return parameterSize * bytesPerElement
				Case org.deeplearning4j.nn.conf.memory.MemoryType.InnerEnum.ACTIVATIONS
					Return minibatchSize * outputType.arrayElementsPerExample() * bytesPerElement
				Case org.deeplearning4j.nn.conf.memory.MemoryType.InnerEnum.ACTIVATION_GRADIENTS
					If memoryUseMode = MemoryUseMode.INFERENCE Then
						Return 0
					End If
					'Activation gradients produced by this layer: epsilons to layer below -> equal to input size
					Return minibatchSize * inputType.arrayElementsPerExample() * bytesPerElement
				Case org.deeplearning4j.nn.conf.memory.MemoryType.InnerEnum.UPDATER_STATE
					If memoryUseMode = MemoryUseMode.INFERENCE Then
						Return 0
					End If
					Return updaterStateSize * bytesPerElement
				Case org.deeplearning4j.nn.conf.memory.MemoryType.InnerEnum.WORKING_MEMORY_FIXED
					If memoryUseMode = MemoryUseMode.INFERENCE Then
						Return workingMemoryFixedInference * bytesPerElement
					Else
						Return workingMemoryFixedTrain(cacheMode) * bytesPerElement
					End If
				Case org.deeplearning4j.nn.conf.memory.MemoryType.InnerEnum.WORKING_MEMORY_VARIABLE
					If memoryUseMode = MemoryUseMode.INFERENCE Then
						Return workingMemoryVariableInference * bytesPerElement
					Else
						Return minibatchSize * workingMemoryVariableTrain(cacheMode) * bytesPerElement
					End If
				Case org.deeplearning4j.nn.conf.memory.MemoryType.InnerEnum.CACHED_MEMORY_FIXED
					If memoryUseMode = MemoryUseMode.INFERENCE Then
						Return 0
					Else
						Return cacheModeMemFixed(cacheMode) * bytesPerElement
					End If
				Case org.deeplearning4j.nn.conf.memory.MemoryType.InnerEnum.CACHED_MEMORY_VARIABLE
					If memoryUseMode = MemoryUseMode.INFERENCE Then
						Return 0
					Else
						Return minibatchSize * cacheModeMemVariablePerEx(cacheMode) * bytesPerElement
					End If
				Case Else
					Throw New System.InvalidOperationException("Unknown memory type: " & memoryType)
			End Select
		End Function

		Public Overrides Function ToString() As String
			Return "LayerMemoryReport(layerName=" & layerName & ",layerType=" & layerType.Name & ")"
		End Function

		''' <summary>
		''' Multiply all memory usage by the specified scaling factor
		''' </summary>
		''' <param name="scale"> Scale factor to multiply all memory usage by </param>
		Public Overridable Sub scale(ByVal scale As Integer)
			parameterSize *= scale
			updaterStateSize *= scale
			workingMemoryFixedInference *= scale
			workingMemoryVariableInference *= scale
			cacheModeMemFixed = scaleEntries(cacheModeMemFixed, scale)
			cacheModeMemVariablePerEx = scaleEntries(cacheModeMemVariablePerEx, scale)
		End Sub

		Private Shared Function scaleEntries(ByVal [in] As IDictionary(Of CacheMode, Long), ByVal scale As Integer) As IDictionary(Of CacheMode, Long)
			If [in] Is Nothing Then
				Return Nothing
			End If

			Dim [out] As IDictionary(Of CacheMode, Long) = New Dictionary(Of CacheMode, Long)()
			For Each e As KeyValuePair(Of CacheMode, Long) In [in].SetOfKeyValuePairs()
				[out](e.Key) = scale * e.Value
			Next e

			Return [out]
		End Function

		Public Class Builder

			Friend layerName As String
			Friend layerType As Type
			Friend inputType As InputType
			Friend outputType As InputType

			'Standard memory (in terms of total ND4J array length)
			Friend parameterSize As Long
			Friend updaterStateSize As Long

			'Working memory (in ND4J array length)
			'Note that *working* memory may be reduced by caching (which is only used during train mode)
			Friend workingMemoryFixedInference As Long
			Friend workingMemoryVariableInference As Long
			Friend workingMemoryFixedTrain As IDictionary(Of CacheMode, Long)
			Friend workingMemoryVariableTrain As IDictionary(Of CacheMode, Long)

			'Cache memory, by cache mode:
			Friend cacheModeMemFixed As IDictionary(Of CacheMode, Long)
			Friend cacheModeMemVariablePerEx As IDictionary(Of CacheMode, Long)

			''' 
			''' <param name="layerName">  Name of the layer or graph vertex </param>
			''' <param name="layerType">  Type of the layer or graph vertex </param>
			''' <param name="inputType">  Input type to the layer/vertex </param>
			''' <param name="outputType"> Output type from the layer/vertex </param>
			Public Sub New(ByVal layerName As String, ByVal layerType As Type, ByVal inputType As InputType, ByVal outputType As InputType)
				Me.layerName = layerName
				Me.layerType = layerType
				Me.inputType = inputType
				Me.outputType = outputType
			End Sub

			''' <summary>
			''' Report the standard memory
			''' </summary>
			''' <param name="parameterSize">    Number of parameters </param>
			''' <param name="updaterStateSize"> Size for the updater array </param>
			Public Overridable Function standardMemory(ByVal parameterSize As Long, ByVal updaterStateSize As Long) As Builder
				Me.parameterSize = parameterSize
				Me.updaterStateSize = updaterStateSize
				Return Me
			End Function

			''' <summary>
			''' Report the working memory size, for both inference and training
			''' </summary>
			''' <param name="fixedInference">         Number of elements used for inference ( independent of minibatch size) </param>
			''' <param name="variableInferencePerEx"> Number of elements used for inference, for each example </param>
			''' <param name="fixedTrain">             Number of elements used for training (independent of minibatch size) </param>
			''' <param name="variableTrainPerEx">     Number of elements used for training, for each example </param>
			Public Overridable Function workingMemory(ByVal fixedInference As Long, ByVal variableInferencePerEx As Long, ByVal fixedTrain As Long, ByVal variableTrainPerEx As Long) As Builder
				Return workingMemory(fixedInference, variableInferencePerEx, MemoryReport.cacheModeMapFor(fixedTrain), MemoryReport.cacheModeMapFor(variableTrainPerEx))
			End Function

			''' <summary>
			''' Report the working memory requirements, for both inference and training. As noted in <seealso cref="MemoryReport"/>
			''' Working memory is memory That will be allocated in a ND4J workspace, or can be garbage collected at any
			''' points after the method returns.
			''' </summary>
			''' <param name="fixedInference">         Number of elements of working memory used for inference (independent of minibatch size) </param>
			''' <param name="variableInferencePerEx"> Number of elements of working memory used for inference, for each example </param>
			''' <param name="fixedTrain">             Number of elements of working memory used for training (independent of
			'''                               minibatch size), for each cache mode </param>
			''' <param name="variableTrainPerEx">     Number of elements of working memory used for training, for each example, for
			'''                               each cache mode </param>
			Public Overridable Function workingMemory(ByVal fixedInference As Long, ByVal variableInferencePerEx As Long, ByVal fixedTrain As IDictionary(Of CacheMode, Long), ByVal variableTrainPerEx As IDictionary(Of CacheMode, Long)) As Builder
				Me.workingMemoryFixedInference = fixedInference
				Me.workingMemoryVariableInference = variableInferencePerEx
				Me.workingMemoryFixedTrain = fixedTrain
				Me.workingMemoryVariableTrain = variableTrainPerEx
				Return Me
			End Function

			''' <summary>
			''' Reports the cached/cacheable memory requirements. This method assumes the caseload memory is the same for
			''' all cases, i.e., typically used with zeros (Layers that do not use caching)
			''' 
			''' </summary>
			''' <param name="cacheModeMemoryFixed">         Number of elements of cache memory, independent of the mini batch size </param>
			''' <param name="cacheModeMemoryVariablePerEx"> Number of elements of cache memory, for each example </param>
			Public Overridable Function cacheMemory(ByVal cacheModeMemoryFixed As Long, ByVal cacheModeMemoryVariablePerEx As Long) As Builder
				Return cacheMemory(MemoryReport.cacheModeMapFor(cacheModeMemoryFixed), MemoryReport.cacheModeMapFor(cacheModeMemoryVariablePerEx))
			End Function

			''' <summary>
			''' Reports the cached/cacheable memory requirements.
			''' </summary>
			''' <param name="cacheModeMemoryFixed">         Number of elements of cache memory, independent of the mini batch size </param>
			''' <param name="cacheModeMemoryVariablePerEx"> Number of elements of cache memory, for each example </param>
			Public Overridable Function cacheMemory(ByVal cacheModeMemoryFixed As IDictionary(Of CacheMode, Long), ByVal cacheModeMemoryVariablePerEx As IDictionary(Of CacheMode, Long)) As Builder
				Me.cacheModeMemFixed = cacheModeMemoryFixed
				Me.cacheModeMemVariablePerEx = cacheModeMemoryVariablePerEx
				Return Me
			End Function

			Public Overridable Function build() As LayerMemoryReport
				Return New LayerMemoryReport(Me)
			End Function
		End Class
	End Class

End Namespace