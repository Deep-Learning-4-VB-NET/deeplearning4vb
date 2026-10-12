Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports At = org.nd4j.autodiff.listeners.At
Imports BaseListener = org.nd4j.autodiff.listeners.BaseListener
Imports ListenerResponse = org.nd4j.autodiff.listeners.ListenerResponse
Imports Loss = org.nd4j.autodiff.listeners.Loss
Imports LossCurve = org.nd4j.autodiff.listeners.records.LossCurve
Imports Operation = org.nd4j.autodiff.listeners.Operation
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet

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

Namespace org.nd4j.autodiff.listeners.impl


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class ScoreListener extends org.nd4j.autodiff.listeners.BaseListener
	Public Class ScoreListener
		Inherits BaseListener

		Private ReadOnly frequency As Integer
		Private ReadOnly reportEpochs As Boolean
		Private ReadOnly reportIterPerformance As Boolean

		Private epochExampleCount As Long
		Private epochBatchCount As Integer
		Private etlTotalTimeEpoch As Long

		Private lastIterTime As Long
		Private etlTimeSumSinceLastReport As Long
		Private iterTimeSumSinceLastReport As Long
		Private examplesSinceLastReportIter As Integer
		Private lastReportTime As Long = -1

		''' <summary>
		''' Create a ScoreListener reporting every 10 iterations, and at the end of each epoch
		''' </summary>
		Public Sub New()
			Me.New(10, True)
		End Sub

		''' <summary>
		''' Create a ScoreListener reporting every N iterations, and at the end of each epoch
		''' </summary>
		Public Sub New(ByVal frequency As Integer)
			Me.New(frequency, True)
		End Sub

		''' <summary>
		''' Create a ScoreListener reporting every N iterations, and optionally at the end of each epoch
		''' </summary>
		Public Sub New(ByVal frequency As Integer, ByVal reportEpochs As Boolean)
			Me.New(frequency, reportEpochs, True)
		End Sub

		Public Sub New(ByVal frequency As Integer, ByVal reportEpochs As Boolean, ByVal reportIterPerformance As Boolean)
			Preconditions.checkArgument(frequency > 0, "ScoreListener frequency must be > 0, got %s", frequency)
			Me.frequency = frequency
			Me.reportEpochs = reportEpochs
			Me.reportIterPerformance = reportIterPerformance
		End Sub


		Public Overrides Function isActive(ByVal operation As Operation) As Boolean
			Return operation = Operation.TRAINING
		End Function

		Public Overrides Sub epochStart(ByVal sd As SameDiff, ByVal at As At)
			If reportEpochs Then
				epochExampleCount = 0
				epochBatchCount = 0
				etlTotalTimeEpoch = 0
			End If
			lastReportTime = -1
			examplesSinceLastReportIter = 0
		End Sub

		Public Overrides Function epochEnd(ByVal sd As SameDiff, ByVal at As At, ByVal lossCurve As LossCurve, ByVal epochTimeMillis As Long) As ListenerResponse
			If reportEpochs Then
				Dim batchesPerSec As Double = epochBatchCount / (epochTimeMillis / 1000.0)
				Dim examplesPerSec As Double = epochExampleCount / (epochTimeMillis / 1000.0)
				Dim pcEtl As Double = 100.0 * etlTotalTimeEpoch / CDbl(epochTimeMillis)
				Dim etl As String = formatDurationMs(etlTotalTimeEpoch) & " ETL time" & (If(etlTotalTimeEpoch > 0, "(" & format2dp(pcEtl) & " %)", ""))
				log.info("Epoch {} complete on iteration {} - {} batches ({} examples) in {} - {} batches/sec, {} examples/sec, {}", at.epoch(), at.iteration(), epochBatchCount, epochExampleCount, formatDurationMs(epochTimeMillis), format2dp(batchesPerSec), format2dp(examplesPerSec), etl)
			End If

			Return ListenerResponse.CONTINUE
		End Function

		Public Overrides Sub iterationStart(ByVal sd As SameDiff, ByVal at As At, ByVal data As MultiDataSet, ByVal etlMs As Long)
			lastIterTime = DateTimeHelper.CurrentUnixTimeMillis()
			etlTimeSumSinceLastReport += etlMs
			etlTotalTimeEpoch += etlMs
		End Sub

		Public Overrides Sub iterationDone(ByVal sd As SameDiff, ByVal at As At, ByVal dataSet As MultiDataSet, ByVal loss As Loss)
			iterTimeSumSinceLastReport += DateTimeHelper.CurrentUnixTimeMillis() - lastIterTime
			epochBatchCount += 1
			If dataSet.numFeatureArrays() > 0 AndAlso dataSet.getFeatures(0) IsNot Nothing Then
				Dim n As Integer = CInt(dataSet.getFeatures(0).size(0))
				examplesSinceLastReportIter += n
				epochExampleCount += n
			End If

			If at.iteration() > 0 AndAlso at.iteration() Mod frequency = 0 Then
				Dim l As Double = loss.totalLoss()
				Dim etl As String = ""
				If etlTimeSumSinceLastReport > 0 Then
					etl = "(" & formatDurationMs(etlTimeSumSinceLastReport) & " ETL"
					If frequency = 1 Then
						etl &= ")"
					Else
						etl &= " in " & frequency & " iter)"
					End If
				End If

				If Not reportIterPerformance Then
					log.info("Loss at epoch {}, iteration {}: {}{}", at.epoch(), at.iteration(), format5dp(l), etl)
				Else
					Dim time As Long = DateTimeHelper.CurrentUnixTimeMillis()
					If lastReportTime > 0 Then
						Dim batchPerSec As Double = 1000 * frequency / CDbl(time - lastReportTime)
						Dim exPerSec As Double = 1000 * examplesSinceLastReportIter / CDbl(time - lastReportTime)
						log.info("Loss at epoch {}, iteration {}: {}{}, batches/sec: {}, examples/sec: {}", at.epoch(), at.iteration(), format5dp(l), etl, format5dp(batchPerSec), format5dp(exPerSec))
					Else
						log.info("Loss at epoch {}, iteration {}: {}{}", at.epoch(), at.iteration(), format5dp(l), etl)
					End If

					lastReportTime = time
				End If

				iterTimeSumSinceLastReport = 0
				etlTimeSumSinceLastReport = 0
				examplesSinceLastReportIter = 0
			End If
		End Sub

		Protected Friend Overridable Function formatDurationMs(ByVal ms As Long) As String
			If ms <= 100 Then
				Return ms & " ms"
			ElseIf ms <= 60000L Then
				Dim sec As Double = ms / 1000.0
				Return format2dp(sec) & " sec"
			ElseIf ms <= 60 * 60000L Then
				Dim min As Double = ms / 60_000.0
				Return format2dp(min) & " min"
			Else
				Dim hr As Double = ms / 360_000.0
				Return format2dp(hr) & " hr"
			End If
		End Function

		Protected Friend Shared ReadOnly DF_2DP As New ThreadLocal(Of DecimalFormat)()
		Protected Friend Shared ReadOnly DF_2DP_SCI As New ThreadLocal(Of DecimalFormat)()

		Protected Friend Overridable Function format2dp(ByVal d As Double) As String
			If d < 0.01 Then
				Dim f As DecimalFormat = DF_2DP_SCI.get()
				If f Is Nothing Then
					f = New DecimalFormat("0.00E0")
					DF_2DP.set(f)
				End If
				Return f.format(d)
			Else
				Dim f As DecimalFormat = DF_2DP.get()
				If f Is Nothing Then
					f = New DecimalFormat("#.00")
					DF_2DP.set(f)
				End If
				Return f.format(d)
			End If
		End Function

		Protected Friend Shared ReadOnly DF_5DP As New ThreadLocal(Of DecimalFormat)()
		Protected Friend Shared ReadOnly DF_5DP_SCI As New ThreadLocal(Of DecimalFormat)()

		Protected Friend Overridable Function format5dp(ByVal d As Double) As String

			If d < 1e-4 OrElse d > 1e4 Then
				'Use scientific
				Dim f As DecimalFormat = DF_5DP_SCI.get()
				If f Is Nothing Then
					f = New DecimalFormat("0.00000E0")
					DF_5DP_SCI.set(f)
				End If
				Return f.format(d)
			Else
				Dim f As DecimalFormat = DF_5DP.get()
				If f Is Nothing Then
					f = New DecimalFormat("0.00000")
					DF_5DP.set(f)
				End If
				Return f.format(d)
			End If
		End Function
	End Class

End Namespace