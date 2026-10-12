Imports System
Imports System.Collections.Generic
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports NonNull = lombok.NonNull
Imports CacheMode = org.deeplearning4j.nn.conf.CacheMode
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports DataTypeUtil = org.nd4j.linalg.api.buffer.util.DataTypeUtil
Imports JsonTypeInfo = org.nd4j.shade.jackson.annotation.JsonTypeInfo
Imports JsonProcessingException = org.nd4j.shade.jackson.core.JsonProcessingException

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
'ORIGINAL LINE: @JsonTypeInfo(use = JsonTypeInfo.Id.@CLASS, include = JsonTypeInfo.@As.@PROPERTY) @EqualsAndHashCode public abstract class MemoryReport
	Public MustInherit Class MemoryReport

		''' <summary>
		''' A simple Map containing all zeros for each CacheMode key
		''' </summary>
		Public Shared ReadOnly CACHE_MODE_ALL_ZEROS As IDictionary(Of CacheMode, Long) = getAllZerosMap()

		Private Shared ReadOnly Property AllZerosMap As IDictionary(Of CacheMode, Long)
			Get
				Dim map As IDictionary(Of CacheMode, Long) = New Dictionary(Of CacheMode, Long)()
				For Each c As CacheMode In System.Enum.GetValues(GetType(CacheMode))
					map(c) = 0L
				Next c
    
				Return Collections.unmodifiableMap(map)
			End Get
		End Property

		''' <returns> Class that the memory report was generated for </returns>
		Public MustOverride ReadOnly Property ReportClass As Type

		''' <summary>
		''' Name of the object that the memory report was generated for
		''' </summary>
		''' <returns> Name of the object </returns>
		Public MustOverride ReadOnly Property Name As String

		''' <summary>
		''' Get the total memory use in bytes for the given configuration (using the current ND4J data type)
		''' </summary>
		''' <param name="minibatchSize"> Mini batch size to estimate the memory for </param>
		''' <param name="memoryUseMode"> The memory use mode (training or inference) </param>
		''' <param name="cacheMode">     The CacheMode to use </param>
		''' <returns> The estimated total memory consumption in bytes </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public long getTotalMemoryBytes(int minibatchSize, @NonNull MemoryUseMode memoryUseMode, @NonNull CacheMode cacheMode)
		Public Overridable Function getTotalMemoryBytes(ByVal minibatchSize As Integer, ByVal memoryUseMode As MemoryUseMode, ByVal cacheMode As CacheMode) As Long
			Return getTotalMemoryBytes(minibatchSize, memoryUseMode, cacheMode, DataTypeUtil.DtypeFromContext)
		End Function

		''' <summary>
		''' Get the total memory use in bytes for the given configuration
		''' </summary>
		''' <param name="minibatchSize"> Mini batch size to estimate the memory for </param>
		''' <param name="memoryUseMode"> The memory use mode (training or inference) </param>
		''' <param name="cacheMode">     The CacheMode to use </param>
		''' <param name="dataType">      Nd4j datatype </param>
		''' <returns> The estimated total memory consumption in bytes </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public abstract long getTotalMemoryBytes(int minibatchSize, @NonNull MemoryUseMode memoryUseMode, @NonNull CacheMode cacheMode, @NonNull DataType dataType);
		Public MustOverride Function getTotalMemoryBytes(ByVal minibatchSize As Integer, ByVal memoryUseMode As MemoryUseMode, ByVal cacheMode As CacheMode, ByVal dataType As DataType) As Long

		''' <summary>
		''' Get the memory estimate (in bytes) for the specified type of memory, using the current ND4J data type
		''' </summary>
		''' <param name="memoryType">    Type of memory to get the estimate for invites </param>
		''' <param name="minibatchSize"> Mini batch size to estimate the memory for </param>
		''' <param name="memoryUseMode"> The memory use mode (training or inference) </param>
		''' <param name="cacheMode">     The CacheMode to use </param>
		''' <returns>              Estimated memory use for the given memory type </returns>
		Public Overridable Function getMemoryBytes(ByVal memoryType As MemoryType, ByVal minibatchSize As Integer, ByVal memoryUseMode As MemoryUseMode, ByVal cacheMode As CacheMode) As Long
			Return getMemoryBytes(memoryType, minibatchSize, memoryUseMode, cacheMode, DataTypeUtil.DtypeFromContext)
		End Function

		''' <summary>
		''' Get the memory estimate (in bytes) for the specified type of memory
		''' </summary>
		''' <param name="memoryType">    Type of memory to get the estimate for invites </param>
		''' <param name="minibatchSize"> Mini batch size to estimate the memory for </param>
		''' <param name="memoryUseMode"> The memory use mode (training or inference) </param>
		''' <param name="cacheMode">     The CacheMode to use </param>
		''' <param name="dataType">      Nd4j datatype </param>
		''' <returns>              Estimated memory use for the given memory type </returns>
		Public MustOverride Function getMemoryBytes(ByVal memoryType As MemoryType, ByVal minibatchSize As Integer, ByVal memoryUseMode As MemoryUseMode, ByVal cacheMode As CacheMode, ByVal dataType As DataType) As Long

		Public Overrides MustOverride Function ToString() As String

		Protected Friend Overridable Function getBytesPerElement(ByVal dataType As DataType) As Integer
			Select Case dataType.innerEnumValue
				Case DataType.InnerEnum.DOUBLE
					Return 8
				Case DataType.InnerEnum.FLOAT
					Return 4
				Case DataType.InnerEnum.HALF
					Return 2
				Case Else
					Throw New System.NotSupportedException("Data type not supported: " & dataType)
			End Select
		End Function

		''' <summary>
		''' Get a map of CacheMode with all keys associated with the specified value
		''' </summary>
		''' <param name="value"> Value for all keys </param>
		''' <returns> Map </returns>
		Public Shared Function cacheModeMapFor(ByVal value As Long) As IDictionary(Of CacheMode, Long)
			If value = 0 Then
				Return CACHE_MODE_ALL_ZEROS
			End If
			Dim m As IDictionary(Of CacheMode, Long) = New Dictionary(Of CacheMode, Long)()
			For Each cm As CacheMode In System.Enum.GetValues(GetType(CacheMode))
				m(cm) = value
			Next cm
			Return m
		End Function

		Public Overridable Function toJson() As String
			Try
				Return NeuralNetConfiguration.mapper().writeValueAsString(Me)
			Catch e As JsonProcessingException
				Throw New Exception(e)
			End Try
		End Function

		Public Overridable Function toYaml() As String
			Try
				Return NeuralNetConfiguration.mapperYaml().writeValueAsString(Me)
			Catch e As JsonProcessingException
				Throw New Exception(e)
			End Try
		End Function

		Public Shared Function fromJson(ByVal json As String) As MemoryReport
			Try
				Return NeuralNetConfiguration.mapper().readValue(json, GetType(MemoryReport))
			Catch e As IOException
				Throw New Exception(e)
			End Try
		End Function

		Public Shared Function fromYaml(ByVal yaml As String) As MemoryReport
			Try
				Return NeuralNetConfiguration.mapperYaml().readValue(yaml, GetType(MemoryReport))
			Catch e As IOException
				Throw New Exception(e)
			End Try
		End Function

	End Class

End Namespace