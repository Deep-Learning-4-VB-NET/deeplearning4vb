Imports System.Collections.Generic
Imports INDArrayHelper = org.deeplearning4j.rl4j.helper.INDArrayHelper
Imports IObservationSource = org.deeplearning4j.rl4j.observation.IObservationSource
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex

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
Namespace org.deeplearning4j.rl4j.agent.learning.update


	Public Class FeaturesBuilder
		Private ReadOnly isRecurrent As Boolean
		Private numChannels As Integer
		Private shapeByChannel()() As Long

		''' <param name="isRecurrent"> True if the network is a recurrent one. </param>
		Public Sub New(ByVal isRecurrent As Boolean)
			Me.isRecurrent = isRecurrent
		End Sub

		''' <summary>
		''' Build a <seealso cref="Features"/> instance </summary>
		''' <param name="trainingBatch"> A container of observation list (see <seealso cref="IObservationSource"/>)
		''' @return </param>
		Public Overridable Function build(Of T1 As IObservationSource)(ByVal trainingBatch As IList(Of T1)) As Features
			Return New Features(createFeatures(trainingBatch))
		End Function

		''' <summary>
		''' Build a <seealso cref="Features"/> instance </summary>
		''' <param name="trainingBatch"> An observation stream </param>
		''' <param name="size"> The total number of observations
		''' @return </param>
		Public Overridable Function build(ByVal trainingBatch As Stream(Of Observation), ByVal size As Integer) As Features
			Return New Features(createFeatures(trainingBatch, size))
		End Function

		Private Function createFeatures(Of T1 As IObservationSource)(ByVal trainingBatch As IList(Of T1)) As INDArray()
			Dim size As Integer = trainingBatch.Count

			If shapeByChannel Is Nothing Then
				Metadata = trainingBatch(0).getObservation()
			End If

			Dim features() As INDArray
			If isRecurrent Then
				features = recurrentCreateFeaturesArray(size)
				Dim arrayIndicesByChannel()() As INDArrayIndex = createChannelsArrayIndices(trainingBatch(0).getObservation())
				For observationIdx As Integer = 0 To size - 1
					Dim observation As Observation = trainingBatch(observationIdx).getObservation()
					recurrentAddObservation(features, observationIdx, observation, arrayIndicesByChannel)
				Next observationIdx
			Else
				features = nonRecurrentCreateFeaturesArray(size)
				For observationIdx As Integer = 0 To size - 1
					Dim observation As Observation = trainingBatch(observationIdx).getObservation()
					nonRecurrentAddObservation(features, observationIdx, observation)
				Next observationIdx
			End If

			Return features
		End Function

		Private Function createFeatures(ByVal trainingBatch As Stream(Of Observation), ByVal size As Integer) As INDArray()
			Dim features() As INDArray = Nothing
			If isRecurrent Then
				Dim it As IEnumerator(Of Observation) = trainingBatch.GetEnumerator()
				Dim observationIdx As Integer = 0
				Dim arrayIndicesByChannel()() As INDArrayIndex = Nothing
				Do While it.MoveNext()
					Dim observation As Observation = it.Current

					If shapeByChannel Is Nothing Then
						Metadata = observation
					End If

					If features Is Nothing Then
						features = recurrentCreateFeaturesArray(size)
						arrayIndicesByChannel = createChannelsArrayIndices(observation)
					End If

'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: recurrentAddObservation(features, observationIdx++, observation, arrayIndicesByChannel);
					recurrentAddObservation(features, observationIdx, observation, arrayIndicesByChannel)
						observationIdx += 1
				Loop
			Else
				Dim it As IEnumerator(Of Observation) = trainingBatch.GetEnumerator()
				Dim observationIdx As Integer = 0
				Do While it.MoveNext()
					Dim observation As Observation = it.Current

					If shapeByChannel Is Nothing Then
						Metadata = observation
					End If

					If features Is Nothing Then
						features = nonRecurrentCreateFeaturesArray(size)
					End If

'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: nonRecurrentAddObservation(features, observationIdx++, observation);
					nonRecurrentAddObservation(features, observationIdx, observation)
						observationIdx += 1
				Loop
			End If

			Return features
		End Function

		Private Sub nonRecurrentAddObservation(ByVal features() As INDArray, ByVal observationIdx As Integer, ByVal observation As Observation)
			For channelIdx As Integer = 0 To numChannels - 1
				features(channelIdx).putRow(observationIdx, observation.getChannelData(channelIdx))
			Next channelIdx
		End Sub

		Private Sub recurrentAddObservation(ByVal features() As INDArray, ByVal observationIdx As Integer, ByVal observation As Observation, ByVal arrayIndicesByChannel()() As INDArrayIndex)
			Dim arrayIndices() As INDArrayIndex

			For channelIdx As Integer = 0 To numChannels - 1
				Dim channelData As INDArray = observation.getChannelData(channelIdx)
				arrayIndices = arrayIndicesByChannel(channelIdx)
				arrayIndices(arrayIndices.Length - 1) = NDArrayIndex.point(observationIdx)

				features(channelIdx).get(arrayIndices).assign(channelData)
			Next channelIdx
		End Sub

		Private Function createChannelsArrayIndices(ByVal observation As Observation) As INDArrayIndex()()
			Dim result(numChannels - 1)() As INDArrayIndex
			For channelIdx As Integer = 0 To numChannels - 1
				Dim channelData As INDArray = observation.getChannelData(channelIdx)

				Dim arrayIndices((channelData.shape().Length) - 1) As INDArrayIndex
				arrayIndices(0) = NDArrayIndex.point(0)
				For i As Integer = 1 To arrayIndices.Length - 2
					arrayIndices(i) = NDArrayIndex.all()
				Next i

				result(channelIdx) = arrayIndices
			Next channelIdx

			Return result
		End Function

		Private WriteOnly Property Metadata As Observation
			Set(ByVal observation As Observation)
				Dim featuresData() As INDArray = observation.getChannelsData()
				numChannels = observation.numChannels()
				shapeByChannel = New Long(numChannels - 1)(){}
				For channelIdx As Integer = 0 To featuresData.Length - 1
					shapeByChannel(channelIdx) = featuresData(channelIdx).shape()
				Next channelIdx
			End Set
		End Property

		Private Function nonRecurrentCreateFeaturesArray(ByVal size As Integer) As INDArray()
			Dim features(numChannels - 1) As INDArray
			For channelIdx As Integer = 0 To numChannels - 1
				Dim observationShape() As Long = shapeByChannel(channelIdx)
				features(channelIdx) = nonRecurrentCreateFeatureArray(size, observationShape)
			Next channelIdx

			Return features
		End Function
		Protected Friend Overridable Function nonRecurrentCreateFeatureArray(ByVal size As Integer, ByVal observationShape() As Long) As INDArray
			Return INDArrayHelper.createBatchForShape(size, observationShape)
		End Function

		Private Function recurrentCreateFeaturesArray(ByVal size As Integer) As INDArray()
			Dim features(numChannels - 1) As INDArray
			For channelIdx As Integer = 0 To numChannels - 1
				Dim observationShape() As Long = shapeByChannel(channelIdx)
				features(channelIdx) = INDArrayHelper.createRnnBatchForShape(size, observationShape)
			Next channelIdx

			Return features
		End Function
	End Class

End Namespace