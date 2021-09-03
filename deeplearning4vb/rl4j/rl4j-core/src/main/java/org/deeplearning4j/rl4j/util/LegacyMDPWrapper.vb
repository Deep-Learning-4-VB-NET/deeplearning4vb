Imports System.Collections.Generic
Imports AccessLevel = lombok.AccessLevel
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports ColorConversionTransform = org.datavec.image.transform.ColorConversionTransform
Imports CropImageTransform = org.datavec.image.transform.CropImageTransform
Imports MultiImageTransform = org.datavec.image.transform.MultiImageTransform
Imports ResizeImageTransform = org.datavec.image.transform.ResizeImageTransform
Imports org.deeplearning4j.gym
Imports IHistoryProcessor = org.deeplearning4j.rl4j.learning.IHistoryProcessor
Imports org.deeplearning4j.rl4j.mdp
Imports EncodableToINDArrayTransform = org.deeplearning4j.rl4j.observation.transform.EncodableToINDArrayTransform
Imports Encodable = org.deeplearning4j.rl4j.space.Encodable
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
Imports TransformProcess = org.deeplearning4j.rl4j.observation.transform.TransformProcess
Imports UniformSkippingFilter = org.deeplearning4j.rl4j.observation.transform.filter.UniformSkippingFilter
Imports EncodableToImageWritableTransform = org.deeplearning4j.rl4j.observation.transform.legacy.EncodableToImageWritableTransform
Imports ImageWritableToINDArrayTransform = org.deeplearning4j.rl4j.observation.transform.legacy.ImageWritableToINDArrayTransform
Imports HistoryMergeTransform = org.deeplearning4j.rl4j.observation.transform.operation.HistoryMergeTransform
Imports SimpleNormalizationTransform = org.deeplearning4j.rl4j.observation.transform.operation.SimpleNormalizationTransform
Imports org.deeplearning4j.rl4j.space
Imports org.deeplearning4j.rl4j.space
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
import static org.bytedeco.opencv.global.opencv_imgproc.COLOR_BGR2GRAY

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

Namespace org.deeplearning4j.rl4j.util


	Public Class LegacyMDPWrapper(Of OBSERVATION As org.deeplearning4j.rl4j.space.Encodable, A, [AS] As org.deeplearning4j.rl4j.space.ActionSpace(Of A))
		Implements MDP(Of Observation, A, [AS])

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final org.deeplearning4j.rl4j.mdp.MDP<OBSERVATION, A, @AS> wrappedMDP;
		Private ReadOnly wrappedMDP As MDP(Of OBSERVATION, A, [AS])
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final WrapperObservationSpace observationSpace;
		Private ReadOnly observationSpace As WrapperObservationSpace
		Private ReadOnly shape() As Integer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter private org.deeplearning4j.rl4j.observation.transform.TransformProcess transformProcess;
		Private transformProcess As TransformProcess

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter(lombok.AccessLevel.@PRIVATE) private org.deeplearning4j.rl4j.learning.IHistoryProcessor historyProcessor;
'JAVA TO VB CONVERTER NOTE: The field historyProcessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private historyProcessor_Conflict As IHistoryProcessor

		Private skipFrame As Integer = 1
		Private steps As Integer = 0


		Public Sub New(ByVal wrappedMDP As MDP(Of OBSERVATION, A, [AS]), ByVal historyProcessor As IHistoryProcessor)
			Me.wrappedMDP = wrappedMDP
			Me.shape = wrappedMDP.getObservationSpace().Shape
			Me.observationSpace = New WrapperObservationSpace(shape)
			Me.historyProcessor_Conflict = historyProcessor

			Me.HistoryProcessor = historyProcessor
		End Sub

		Public Overridable WriteOnly Property HistoryProcessor As IHistoryProcessor
			Set(ByVal historyProcessor As IHistoryProcessor)
				Me.historyProcessor_Conflict = historyProcessor
				createTransformProcess()
			End Set
		End Property

		'TODO: this transform process should be decoupled from history processor and configured seperately by the end-user
		Private Sub createTransformProcess()
			Dim historyProcessor As IHistoryProcessor = getHistoryProcessor()

			If historyProcessor IsNot Nothing AndAlso shape.Length = 3 Then
				Dim skipFrame As Integer = historyProcessor.Conf.getSkipFrame()
				Dim frameStackLength As Integer = historyProcessor.Conf.getHistoryLength()

				Dim height As Integer = shape(1)
				Dim width As Integer = shape(2)

				Dim cropBottom As Integer = height - historyProcessor.Conf.getCroppingHeight()
				Dim cropRight As Integer = width - historyProcessor.Conf.getCroppingWidth()

				transformProcess = TransformProcess.builder().filter(New UniformSkippingFilter(skipFrame)).transform("data", New EncodableToImageWritableTransform()).transform("data", New MultiImageTransform(New CropImageTransform(historyProcessor.Conf.getOffsetY(), historyProcessor.Conf.getOffsetX(), cropBottom, cropRight), New ResizeImageTransform(historyProcessor.Conf.getRescaledWidth(), historyProcessor.Conf.getRescaledHeight()), New ColorConversionTransform(COLOR_BGR2GRAY))).transform("data", New ImageWritableToINDArrayTransform()).transform("data", New SimpleNormalizationTransform(0.0, 255.0)).transform("data", HistoryMergeTransform.builder().isFirstDimenstionBatch(True).build(frameStackLength)).build("data")
			Else
				transformProcess = TransformProcess.builder().transform("data", New EncodableToINDArrayTransform()).build("data")
			End If
		End Sub

		Public Overridable ReadOnly Property ActionSpace As [AS]
			Get
				Return wrappedMDP.ActionSpace
			End Get
		End Property

		Public Overridable Function reset() As Observation
			transformProcess.reset()

			Dim rawResetResponse As OBSERVATION = wrappedMDP.reset()
			record(rawResetResponse)

			If historyProcessor_Conflict IsNot Nothing Then
				skipFrame = historyProcessor_Conflict.Conf.getSkipFrame()
			End If

			Dim channelsData As IDictionary(Of String, Object) = buildChannelsData(rawResetResponse)
			Return transformProcess.transform(channelsData, 0, False)
		End Function

		Public Overridable Function [step](ByVal a As A) As StepReply(Of Observation)
			Dim historyProcessor As IHistoryProcessor = getHistoryProcessor()

			Dim rawStepReply As StepReply(Of OBSERVATION) = wrappedMDP.step(a)
			Dim rawObservation As INDArray = getInput(rawStepReply.getObservation())

			If historyProcessor IsNot Nothing Then
				historyProcessor.record(rawObservation)
			End If

'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: int stepOfObservation = steps++;
			Dim stepOfObservation As Integer = steps
				steps += 1

			Dim channelsData As IDictionary(Of String, Object) = buildChannelsData(rawStepReply.getObservation())
			Dim observation As Observation = transformProcess.transform(channelsData, stepOfObservation, rawStepReply.isDone())

			Return New StepReply(Of Observation)(observation, rawStepReply.getReward(), rawStepReply.isDone(), rawStepReply.getInfo())
		End Function

		Private Sub record(ByVal obs As OBSERVATION)
			Dim rawObservation As INDArray = getInput(obs)

			Dim historyProcessor As IHistoryProcessor = getHistoryProcessor()
			If historyProcessor IsNot Nothing Then
				historyProcessor.record(rawObservation)
			End If
		End Sub

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: private java.util.Map<String, Object> buildChannelsData(final OBSERVATION obs)
		Private Function buildChannelsData(ByVal obs As OBSERVATION) As IDictionary(Of String, Object)
			Return New HashMapAnonymousInnerClass(Me, obs)
		End Function

		Private Class HashMapAnonymousInnerClass
			Inherits Dictionary(Of String, Object)

			Private ReadOnly outerInstance As LegacyMDPWrapper(Of OBSERVATION, A, [AS])

			Private obs As Encodable

			Public Sub New(ByVal outerInstance As LegacyMDPWrapper(Of OBSERVATION, A, [AS]), ByVal obs As Encodable)
				Me.outerInstance = outerInstance
				Me.obs = obs

				Me.put("data", obs)
			End Sub

		End Class

		Public Overridable Sub close() Implements MDP(Of Observation, A, [AS]).close
			wrappedMDP.close()
		End Sub

		Public Overridable ReadOnly Property Done As Boolean Implements MDP(Of Observation, A, [AS]).isDone
			Get
				Return wrappedMDP.Done
			End Get
		End Property

		Public Overridable Function newInstance() As MDP(Of Observation, A, [AS])
			Return New LegacyMDPWrapper(Of Observation, A, [AS])(wrappedMDP.newInstance(), historyProcessor_Conflict)
		End Function

		Private Function getInput(ByVal obs As OBSERVATION) As INDArray
			Return obs.getData()
		End Function

		Public Class WrapperObservationSpace
			Implements ObservationSpace(Of Observation)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final int[] shape;
			Friend ReadOnly shape() As Integer

			Public Sub New(ByVal shape() As Integer)

				Me.shape = shape
			End Sub

			Public Overridable ReadOnly Property Name As String Implements ObservationSpace(Of Observation).getName
				Get
					Return Nothing
				End Get
			End Property

			Public Overridable ReadOnly Property Low As INDArray Implements ObservationSpace(Of Observation).getLow
				Get
					Return Nothing
				End Get
			End Property

			Public Overridable ReadOnly Property High As INDArray Implements ObservationSpace(Of Observation).getHigh
				Get
					Return Nothing
				End Get
			End Property
		End Class
	End Class

End Namespace