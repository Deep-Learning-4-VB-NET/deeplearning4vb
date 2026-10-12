Imports System.Collections
Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports NotImplementedException = org.apache.commons.lang3.NotImplementedException
Imports INDArrayHelper = org.deeplearning4j.rl4j.helper.INDArrayHelper
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor
Imports Maps = org.nd4j.shade.guava.collect.Maps
Imports org.datavec.api.transform

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
Namespace org.deeplearning4j.rl4j.observation.transform


	Public Class TransformProcess

		Private ReadOnly operations As IList(Of KeyValuePair(Of String, Object))
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final String[] channelNames;
		Private ReadOnly channelNames() As String
		Private ReadOnly operationsChannelNames As HashSet(Of String)

		Private Sub New(ByVal builder As Builder, ParamArray ByVal channelNames() As String)
			operations = builder.operations
			Me.channelNames = channelNames
			Me.operationsChannelNames = builder.requiredChannelNames
		End Sub

		''' <summary>
		''' This method will call reset() of all steps implementing <seealso cref="ResettableOperation ResettableOperation"/> in the transform process.
		''' </summary>
		Public Overridable Sub reset()
			For Each entry As KeyValuePair(Of String, Object) In operations
				If TypeOf entry.Value Is ResettableOperation Then
					DirectCast(entry.Value, ResettableOperation).reset()
				End If
			Next entry
		End Sub

		''' <summary>
		''' Transforms the channel data into an Observation or a skipped observation depending on the specific steps in the transform process.
		''' </summary>
		''' <param name="channelsData"> A Map that maps the channel name to its data. </param>
		''' <param name="currentObservationStep"> The observation's step number within the episode. </param>
		''' <param name="isFinalObservation"> True if the observation is the last of the episode. </param>
		''' <returns> An observation (may be a skipped observation) </returns>
		Public Overridable Function transform(ByVal channelsData As IDictionary(Of String, Object), ByVal currentObservationStep As Integer, ByVal isFinalObservation As Boolean) As Observation
			' null or empty channelData
			Preconditions.checkArgument(channelsData IsNot Nothing AndAlso channelsData.Count <> 0, "Error: channelsData not supplied.")

			' Check that all channels have data
			For Each channel As KeyValuePair(Of String, Object) In channelsData.SetOfKeyValuePairs()
				Preconditions.checkNotNull(channel.Value, "Error: data of channel '%s' is null", channel.Key)
			Next channel

			' Check that all required channels are present
			For Each channelName As String In operationsChannelNames
				Preconditions.checkArgument(channelsData.ContainsKey(channelName), "The channelsData map does not contain the channel '%s'", channelName)
			Next channelName

			For Each entry As KeyValuePair(Of String, Object) In operations

				' Filter
				If TypeOf entry.Value Is FilterOperation Then
					Dim filterOperation As FilterOperation = DirectCast(entry.Value, FilterOperation)
					If filterOperation.isSkipped(channelsData, currentObservationStep, isFinalObservation) Then
						Return Observation.SkippedObservation
					End If

				' Transform
				' null results are considered skipped observations
				ElseIf TypeOf entry.Value Is Operation Then
					Dim transformOperation As Operation = CType(entry.Value, Operation)
					Dim transformed As Object = transformOperation.transform(channelsData(entry.Key))
					If transformed Is Nothing Then
						Return Observation.SkippedObservation
					End If
					channelsData.replace(entry.Key, transformed)

				' PreProcess
				ElseIf TypeOf entry.Value Is DataSetPreProcessor Then
					Dim channelData As Object = channelsData(entry.Key)
					Dim dataSetPreProcessor As DataSetPreProcessor = DirectCast(entry.Value, DataSetPreProcessor)
					If Not (TypeOf channelData Is DataSet) Then
						Throw New System.ArgumentException("The channel data must be a DataSet to call preProcess")
					End If
					dataSetPreProcessor.preProcess(DirectCast(channelData, DataSet))

				Else
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					Throw New System.ArgumentException(String.Format("Unknown operation: '{0}'", entry.Value.GetType().FullName))
				End If
			Next entry

			' Check that all channels used to build the observation are instances of
			' INDArray or DataSet
			' TODO: Add support for an interface with a toINDArray() method
			For Each channelName As String In channelNames
				Dim channelData As Object = channelsData(channelName)

				Dim finalChannelData As INDArray
				If TypeOf channelData Is DataSet Then
					finalChannelData = DirectCast(channelData, DataSet).Features
				ElseIf TypeOf channelData Is INDArray Then
					finalChannelData = DirectCast(channelData, INDArray)
				Else
					Throw New System.InvalidOperationException("All channels used to build the observation must be instances of DataSet or INDArray")
				End If

				' The dimension 0 of all INDArrays must be 1 (batch count)
				channelsData.replace(channelName, INDArrayHelper.forceCorrectShape(finalChannelData))
			Next channelName

			Dim data(channelNames.Length - 1) As INDArray
			For i As Integer = 0 To channelNames.Length - 1
				data(i) = (DirectCast(channelsData(channelNames(i)), INDArray))
			Next i

			Return New Observation(data)
		End Function

		''' <returns> An instance of a builder </returns>
		Public Shared Function builder() As Builder
			Return New Builder()
		End Function

		Public Class Builder

			Friend ReadOnly operations As IList(Of KeyValuePair(Of String, Object)) = New List(Of KeyValuePair(Of String, Object))()
			Friend ReadOnly requiredChannelNames As New HashSet(Of String)()

			''' <summary>
			''' Add a filter to the transform process steps. Used to skip observations on certain conditions.
			''' See <seealso cref="FilterOperation FilterOperation"/> </summary>
			''' <param name="filterOperation"> An instance </param>
			Public Overridable Function filter(ByVal filterOperation As FilterOperation) As Builder
				Preconditions.checkNotNull(filterOperation, "The filterOperation must not be null")

				operations.Add(CType(Maps.immutableEntry(Nothing, filterOperation), DictionaryEntry))
				Return Me
			End Function

			''' <summary>
			''' Add a transform to the steps. The transform can change the data and / or change the type of the data
			''' (e.g. Byte[] to a ImageWritable)
			''' </summary>
			''' <param name="targetChannel"> The name of the channel to which the transform is applied. </param>
			''' <param name="transformOperation"> An instance of <seealso cref="Operation Operation"/> </param>
			Public Overridable Function transform(ByVal targetChannel As String, ByVal transformOperation As Operation) As Builder
				Preconditions.checkNotNull(targetChannel, "The targetChannel must not be null")
				Preconditions.checkNotNull(transformOperation, "The transformOperation must not be null")

				requiredChannelNames.Add(targetChannel)
				operations.Add(CType(Maps.immutableEntry(targetChannel, transformOperation), DictionaryEntry))
				Return Me
			End Function

			''' <summary>
			''' Add a DataSetPreProcessor to the steps. The channel must be a DataSet instance at this step. </summary>
			''' <param name="targetChannel"> The name of the channel to which the pre processor is applied. </param>
			''' <param name="dataSetPreProcessor"> </param>
			Public Overridable Function preProcess(ByVal targetChannel As String, ByVal dataSetPreProcessor As DataSetPreProcessor) As Builder
				Preconditions.checkNotNull(targetChannel, "The targetChannel must not be null")
				Preconditions.checkNotNull(dataSetPreProcessor, "The dataSetPreProcessor must not be null")

				requiredChannelNames.Add(targetChannel)
				operations.Add(CType(Maps.immutableEntry(targetChannel, dataSetPreProcessor), DictionaryEntry))
				Return Me
			End Function

			''' <summary>
			''' Builds the TransformProcess. </summary>
			''' <param name="channelNames"> A subset of channel names to be used to build the observation </param>
			''' <returns> An instance of TransformProcess </returns>
			Public Overridable Function build(ParamArray ByVal channelNames() As String) As TransformProcess
				If channelNames.Length = 0 Then
					Throw New System.ArgumentException("At least one channel must be supplied.")
				End If

				For Each channelName As String In channelNames
					Preconditions.checkNotNull(channelName, "Error: got a null channel name")
					requiredChannelNames.Add(channelName)
				Next channelName

				Return New TransformProcess(Me, channelNames)
			End Function
		End Class
	End Class

End Namespace