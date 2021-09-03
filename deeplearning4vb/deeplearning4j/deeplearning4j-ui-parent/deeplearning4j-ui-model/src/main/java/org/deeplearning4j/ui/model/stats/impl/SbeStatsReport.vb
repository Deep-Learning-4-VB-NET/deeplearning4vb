Imports System
Imports System.Collections.Generic
Imports System.IO
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports ToString = lombok.ToString
Imports DirectBuffer = org.agrona.DirectBuffer
Imports MutableDirectBuffer = org.agrona.MutableDirectBuffer
Imports UnsafeBuffer = org.agrona.concurrent.UnsafeBuffer
Imports IOUtils = org.apache.commons.io.IOUtils
Imports Histogram = org.deeplearning4j.ui.model.stats.api.Histogram
Imports StatsReport = org.deeplearning4j.ui.model.stats.api.StatsReport
Imports StatsType = org.deeplearning4j.ui.model.stats.api.StatsType
Imports SummaryType = org.deeplearning4j.ui.model.stats.api.SummaryType
Imports org.deeplearning4j.ui.model.stats.sbe
Imports AgronaPersistable = org.deeplearning4j.ui.model.storage.AgronaPersistable
Imports org.nd4j.common.primitives

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

Namespace org.deeplearning4j.ui.model.stats.impl


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode @ToString @Data public class SbeStatsReport implements org.deeplearning4j.ui.model.stats.api.StatsReport, org.deeplearning4j.ui.model.storage.AgronaPersistable
	<Serializable>
	Public Class SbeStatsReport
		Implements StatsReport, AgronaPersistable

'JAVA TO VB CONVERTER NOTE: The field sessionID was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private sessionID_Conflict As String
'JAVA TO VB CONVERTER NOTE: The field typeID was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private typeID_Conflict As String
'JAVA TO VB CONVERTER NOTE: The field workerID was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private workerID_Conflict As String
'JAVA TO VB CONVERTER NOTE: The field timeStamp was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private timeStamp_Conflict As Long

		Private iterationCount As Integer
		Private statsCollectionDurationMs As Integer
		Private score As Double

		Private jvmCurrentBytes As Long
		Private jvmMaxBytes As Long
		Private offHeapCurrentBytes As Long
		Private offHeapMaxBytes As Long
		Private deviceCurrentBytes() As Long
		Private deviceMaxBytes() As Long

		Private totalRuntimeMs As Long
		Private totalExamples As Long
		Private totalMinibatches As Long
		Private examplesPerSecond As Double
		Private minibatchesPerSecond As Double

		Private gcStats As IList(Of GCStats)

		Private learningRatesByParam As IDictionary(Of String, Double)
		Private histograms As IDictionary(Of StatsType, IDictionary(Of String, Histogram))
		Private meanValues As IDictionary(Of StatsType, IDictionary(Of String, Double))
		Private stdevValues As IDictionary(Of StatsType, IDictionary(Of String, Double))
		Private meanMagnitudeValues As IDictionary(Of StatsType, IDictionary(Of String, Double))

		Private metaDataClassName As String
		'Store in serialized form; deserialize iff required. Might save us some class not found (or, version) errors, if
		' metadata is saved but is never used
		Private dataSetMetaData As IList(Of SByte())

		Private scorePresent As Boolean
		Private memoryUsePresent As Boolean
		Private performanceStatsPresent As Boolean

		Public Sub New()
			'No-Arg constructor only for deserialization
		End Sub

		Public Overridable Sub reportIDs(ByVal sessionID As String, ByVal typeID As String, ByVal workerID As String, ByVal timeStamp As Long) Implements StatsReport.reportIDs
			Me.sessionID_Conflict = sessionID
			Me.typeID_Conflict = typeID
			Me.workerID_Conflict = workerID
			Me.timeStamp_Conflict = timeStamp
		End Sub

		Public Overridable Sub reportIterationCount(ByVal iterationCount As Integer) Implements StatsReport.reportIterationCount
			Me.iterationCount = iterationCount
		End Sub


		Public Overridable Sub reportStatsCollectionDurationMS(ByVal statsCollectionDurationMS As Integer) Implements StatsReport.reportStatsCollectionDurationMS
			Me.statsCollectionDurationMs = statsCollectionDurationMS
		End Sub

		Public Overridable Sub reportScore(ByVal currentScore As Double) Implements StatsReport.reportScore
			Me.score = currentScore
			Me.scorePresent = True
		End Sub

		Public Overridable Sub reportLearningRates(ByVal learningRatesByParam As IDictionary(Of String, Double))
			Me.learningRatesByParam = learningRatesByParam
		End Sub

		Public Overridable ReadOnly Property LearningRates As IDictionary(Of String, Double)
			Get
				Return Me.learningRatesByParam
			End Get
		End Property

		Public Overridable Sub reportMemoryUse(ByVal jvmCurrentBytes As Long, ByVal jvmMaxBytes As Long, ByVal offHeapCurrentBytes As Long, ByVal offHeapMaxBytes As Long, ByVal deviceCurrentBytes() As Long, ByVal deviceMaxBytes() As Long) Implements StatsReport.reportMemoryUse
			Me.jvmCurrentBytes = jvmCurrentBytes
			Me.jvmMaxBytes = jvmMaxBytes
			Me.offHeapCurrentBytes = offHeapCurrentBytes
			Me.offHeapMaxBytes = offHeapMaxBytes
			Me.deviceCurrentBytes = deviceCurrentBytes
			Me.deviceMaxBytes = deviceMaxBytes
			Me.memoryUsePresent = True
		End Sub

		Public Overridable Sub reportPerformance(ByVal totalRuntimeMs As Long, ByVal totalExamples As Long, ByVal totalMinibatches As Long, ByVal examplesPerSecond As Double, ByVal minibatchesPerSecond As Double) Implements StatsReport.reportPerformance
			Me.totalRuntimeMs = totalRuntimeMs
			Me.totalExamples = totalExamples
			Me.totalMinibatches = totalMinibatches
			Me.examplesPerSecond = examplesPerSecond
			Me.minibatchesPerSecond = minibatchesPerSecond
			Me.performanceStatsPresent = True
		End Sub

		Public Overridable Sub reportGarbageCollection(ByVal gcName As String, ByVal deltaGCCount As Integer, ByVal deltaGCTime As Integer) Implements StatsReport.reportGarbageCollection
			If gcStats Is Nothing Then
				gcStats = New List(Of GCStats)()
			End If
			gcStats.Add(New GCStats(gcName, deltaGCCount, deltaGCTime))
		End Sub

		Public Overridable ReadOnly Property GarbageCollectionStats As IList(Of Pair(Of String, Integer()))
			Get
				If gcStats Is Nothing Then
					Return Nothing
				End If
				Dim temp As IList(Of Pair(Of String, Integer())) = New List(Of Pair(Of String, Integer()))()
				For Each g As GCStats In gcStats
					temp.Add(New Pair(Of String, Integer())(g.gcName, New Integer() {g.getDeltaGCCount(), g.getDeltaGCTime()}))
				Next g
				Return temp
			End Get
		End Property

		Public Overridable Sub reportHistograms(ByVal statsType As StatsType, ByVal histogram As IDictionary(Of String, Histogram))
			If Me.histograms Is Nothing Then
				Me.histograms = New Dictionary(Of StatsType, IDictionary(Of String, Histogram))()
			End If
			Me.histograms(statsType) = histogram
		End Sub

		Public Overridable Function getHistograms(ByVal statsType As StatsType) As IDictionary(Of String, Histogram)
			If histograms Is Nothing Then
				Return Nothing
			End If
			Return histograms(statsType)
		End Function

		Public Overridable Sub reportMean(ByVal statsType As StatsType, ByVal mean As IDictionary(Of String, Double))
			If Me.meanValues Is Nothing Then
				Me.meanValues = New Dictionary(Of StatsType, IDictionary(Of String, Double))()
			End If
			Me.meanValues(statsType) = mean
		End Sub

		Public Overridable Function getMean(ByVal statsType As StatsType) As IDictionary(Of String, Double)
			If Me.meanValues Is Nothing Then
				Return Nothing
			End If
			Return meanValues(statsType)
		End Function

		Public Overridable Sub reportStdev(ByVal statsType As StatsType, ByVal stdev As IDictionary(Of String, Double))
			If Me.stdevValues Is Nothing Then
				Me.stdevValues = New Dictionary(Of StatsType, IDictionary(Of String, Double))()
			End If
			Me.stdevValues(statsType) = stdev
		End Sub

		Public Overridable Function getStdev(ByVal statsType As StatsType) As IDictionary(Of String, Double)
			If Me.stdevValues Is Nothing Then
				Return Nothing
			End If
			Return stdevValues(statsType)
		End Function

		Public Overridable Sub reportMeanMagnitudes(ByVal statsType As StatsType, ByVal meanMagnitudes As IDictionary(Of String, Double))
			If Me.meanMagnitudeValues Is Nothing Then
				Me.meanMagnitudeValues = New Dictionary(Of StatsType, IDictionary(Of String, Double))()
			End If
			Me.meanMagnitudeValues(statsType) = meanMagnitudes
		End Sub

		Public Overridable Sub reportDataSetMetaData(ByVal dataSetMetaData As IList(Of Serializable), ByVal metaDataClass As Type)
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			reportDataSetMetaData(dataSetMetaData, (If(metaDataClass Is Nothing, Nothing, metaDataClass.FullName)))
		End Sub

		Public Overridable Sub reportDataSetMetaData(ByVal dataSetMetaData As IList(Of Serializable), ByVal metaDataClass As String)
			If dataSetMetaData IsNot Nothing Then
				Me.dataSetMetaData = New List(Of SByte())()
				For Each s As Serializable In dataSetMetaData
					Dim baos As New MemoryStream()
					Try
							Using oos As New ObjectOutputStream(baos)
							oos.writeObject(s)
							oos.flush()
							oos.close()
							End Using
					Catch e As IOException
						Throw New Exception("Unexpected IOException from ByteArrayOutputStream", e)
					End Try
					Dim b() As SByte = baos.toByteArray()
					Me.dataSetMetaData.Add(b)
				Next s
			Else
				Me.dataSetMetaData = Nothing
			End If
			Me.metaDataClassName = metaDataClass
		End Sub

		Public Overridable Function getMeanMagnitudes(ByVal statsType As StatsType) As IDictionary(Of String, Double)
			If Me.meanMagnitudeValues Is Nothing Then
				Return Nothing
			End If
			Return Me.meanMagnitudeValues(statsType)
		End Function

		Public Overridable ReadOnly Property DataSetMetaData As IList(Of Serializable)
			Get
				If dataSetMetaData Is Nothing OrElse dataSetMetaData.Count = 0 Then
					Return Nothing
				End If
    
				Dim l As IList(Of Serializable) = New List(Of Serializable)()
				For Each b As SByte() In dataSetMetaData
					Try
							Using ois As New ObjectInputStream(New MemoryStream(b))
							l.Add(CType(ois.readObject(), Serializable))
							End Using
					Catch e As Exception When TypeOf e Is IOException OrElse TypeOf e Is ClassNotFoundException
						Throw New Exception(e)
					End Try
				Next b
				Return l
			End Get
		End Property

		Public Overridable ReadOnly Property DataSetMetaDataClassName As String Implements StatsReport.getDataSetMetaDataClassName
			Get
				Return metaDataClassName
			End Get
		End Property

		Public Overridable Function hasScore() As Boolean Implements StatsReport.hasScore
			Return scorePresent
		End Function

		Public Overridable Function hasLearningRates() As Boolean Implements StatsReport.hasLearningRates
			Return learningRatesByParam IsNot Nothing
		End Function

		Public Overridable Function hasMemoryUse() As Boolean Implements StatsReport.hasMemoryUse
			Return memoryUsePresent
		End Function

		Public Overridable Function hasPerformance() As Boolean Implements StatsReport.hasPerformance
			Return performanceStatsPresent
		End Function

		Public Overridable Function hasGarbageCollection() As Boolean Implements StatsReport.hasGarbageCollection
			Return gcStats IsNot Nothing AndAlso gcStats.Count > 0
		End Function

		Public Overridable Function hasHistograms(ByVal statsType As StatsType) As Boolean
			If histograms Is Nothing Then
				Return False
			End If
			Return histograms.ContainsKey(statsType)
		End Function

		Public Overridable Function hasSummaryStats(ByVal statsType As StatsType, ByVal summaryType As SummaryType) As Boolean
			Select Case summaryType
				Case SummaryType.Mean
					Return meanValues IsNot Nothing AndAlso meanValues.ContainsKey(statsType)
				Case SummaryType.Stdev
					Return stdevValues IsNot Nothing AndAlso stdevValues.ContainsKey(statsType)
				Case SummaryType.MeanMagnitudes
					Return meanMagnitudeValues IsNot Nothing AndAlso meanMagnitudeValues.ContainsKey(statsType)
			End Select
			Return False
		End Function

		Public Overridable Function hasDataSetMetaData() As Boolean Implements StatsReport.hasDataSetMetaData
			Return dataSetMetaData IsNot Nothing OrElse metaDataClassName IsNot Nothing
		End Function

		Private Function mapForTypes(ByVal statsType As StatsType, ByVal summaryType As SummaryType) As IDictionary(Of String, Double)
			Select Case summaryType
				Case SummaryType.Mean
					If meanValues Is Nothing Then
						Return Nothing
					End If
					Return meanValues(statsType)
				Case SummaryType.Stdev
					If stdevValues Is Nothing Then
						Return Nothing
					End If
					Return stdevValues(statsType)
				Case SummaryType.MeanMagnitudes
					If meanMagnitudeValues Is Nothing Then
						Return Nothing
					End If
					Return meanMagnitudeValues(statsType)
			End Select
			Return Nothing
		End Function

		Private Shared Sub appendOrDefault(ByVal sse As UpdateEncoder.PerParameterStatsEncoder.SummaryStatEncoder, ByVal param As String, ByVal statsType As StatsType, ByVal summaryType As SummaryType, ByVal map As IDictionary(Of String, Double), ByVal defaultValue As Double)
			Dim d As Double? = map(param)
			If d Is Nothing Then
				d = defaultValue
			End If

			Dim st As org.deeplearning4j.ui.model.stats.sbe.StatsType
			Select Case statsType
				Case StatsType.Parameters
					st = org.deeplearning4j.ui.model.stats.sbe.StatsType.Parameters
				Case StatsType.Gradients
					st = org.deeplearning4j.ui.model.stats.sbe.StatsType.Gradients
				Case StatsType.Updates
					st = org.deeplearning4j.ui.model.stats.sbe.StatsType.Updates
				Case StatsType.Activations
					st = org.deeplearning4j.ui.model.stats.sbe.StatsType.Activations
				Case Else
					Throw New Exception("Unknown stats type: " & statsType)
			End Select
			Dim summaryT As org.deeplearning4j.ui.model.stats.sbe.SummaryType
			Select Case summaryType
				Case SummaryType.Mean
					summaryT = org.deeplearning4j.ui.model.stats.sbe.SummaryType.Mean
				Case SummaryType.Stdev
					summaryT = org.deeplearning4j.ui.model.stats.sbe.SummaryType.Stdev
				Case SummaryType.MeanMagnitudes
					summaryT = org.deeplearning4j.ui.model.stats.sbe.SummaryType.MeanMagnitude
				Case Else
					Throw New Exception("Unknown summary type: " & summaryType)
			End Select
			sse.next().statType(st).summaryType(summaryT).value(d)
		End Sub

		Private Shared Function translate(ByVal statsType As org.deeplearning4j.ui.model.stats.sbe.StatsType) As StatsType
			Select Case statsType.innerEnumValue
				Case org.deeplearning4j.ui.model.stats.sbe.StatsType.InnerEnum.Parameters
					Return StatsType.Parameters
				Case org.deeplearning4j.ui.model.stats.sbe.StatsType.InnerEnum.Gradients
					Return StatsType.Gradients
				Case org.deeplearning4j.ui.model.stats.sbe.StatsType.InnerEnum.Updates
					Return StatsType.Updates
				Case org.deeplearning4j.ui.model.stats.sbe.StatsType.InnerEnum.Activations
					Return StatsType.Activations
				Case Else
					Throw New Exception("Unknown stats type: " & statsType)
			End Select
		End Function

		Private Shared Function translate(ByVal statsType As StatsType) As org.deeplearning4j.ui.model.stats.sbe.StatsType
			Select Case statsType
				Case StatsType.Parameters
					Return org.deeplearning4j.ui.model.stats.sbe.StatsType.Parameters
				Case StatsType.Gradients
					Return org.deeplearning4j.ui.model.stats.sbe.StatsType.Gradients
				Case StatsType.Updates
					Return org.deeplearning4j.ui.model.stats.sbe.StatsType.Updates
				Case StatsType.Activations
					Return org.deeplearning4j.ui.model.stats.sbe.StatsType.Activations
				Case Else
					Throw New Exception("Unknown stats type: " & statsType)
			End Select
		End Function

		Private Shared Function translate(ByVal summaryType As org.deeplearning4j.ui.model.stats.sbe.SummaryType) As SummaryType
			Select Case summaryType.innerEnumValue
				Case org.deeplearning4j.ui.model.stats.sbe.SummaryType.InnerEnum.Mean
					Return SummaryType.Mean
				Case org.deeplearning4j.ui.model.stats.sbe.SummaryType.InnerEnum.Stdev
					Return SummaryType.Stdev
				Case org.deeplearning4j.ui.model.stats.sbe.SummaryType.InnerEnum.MeanMagnitude
					Return SummaryType.MeanMagnitudes
				Case Else
					Throw New Exception("Unknown summary type: " & summaryType)
			End Select
		End Function

		Public Overridable ReadOnly Property SessionID As String
			Get
				Return sessionID_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property TypeID As String
			Get
				Return typeID_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property WorkerID As String
			Get
				Return workerID_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property TimeStamp As Long
			Get
				Return timeStamp_Conflict
			End Get
		End Property


		'================ Ser/de methods =================

		Public Overridable Function encodingLengthBytes() As Integer
			'TODO convert Strings to byte[] only once

			'First: determine buffer size.
			'(a) Header: 8 bytes (4x uint16 = 8 bytes)
			'(b) Fixed length entries length (sie.BlockLength())
			'(c) Group 1: Memory use.
			'(d) Group 2: Performance stats
			'(e) Group 3: GC stats
			'(f) Group 4: param names (variable length strings)
			'(g) Group 5: layer names (variable length strings)
			'(g) Group 6: Per parameter performance stats
			'Variable length String fields: 4 - session/type/worker IDs and metadata -> 4*4=16 bytes header, plus content

			Dim ue As New UpdateEncoder()
			Dim bufferSize As Integer = 8 + ue.sbeBlockLength() + 16

			'Memory use group length...
			Dim memoryUseCount As Integer
			If Not memoryUsePresent Then
				memoryUseCount = 0
			Else
				memoryUseCount = 4 + (If(deviceCurrentBytes Is Nothing, 0, deviceCurrentBytes.Length)) + (If(deviceMaxBytes Is Nothing, 0, deviceMaxBytes.Length))
			End If
			bufferSize += 4 + 9 * memoryUseCount 'Group header: 4 bytes (always present); Each entry in group - 1x MemoryType (uint8) + 1x int64 -> 1+8 = 9 bytes

			'Performance group length
			bufferSize += 4 + (If(performanceStatsPresent, 32, 0)) 'Group header: 4 bytes (always present); Only 1 group: 3xint64 + 2xfloat = 32 bytes

			'GC stats group length
			bufferSize += 4 'Group header: always present
			Dim gcStatsLabelBytes As IList(Of SByte()) = Nothing
			If gcStats IsNot Nothing AndAlso gcStats.Count > 0 Then
				gcStatsLabelBytes = New List(Of SByte())()
				For i As Integer = 0 To gcStats.Count - 1
					Dim stats As GCStats = gcStats(i)
					bufferSize += 12 'Fixed per group entry: 2x int32 -> 8 bytes PLUS the header for the variable length GC name: another 4 bytes
					Dim nameAsBytes() As SByte = SbeUtil.toBytes(True, stats.gcName)
					bufferSize += nameAsBytes.Length
					gcStatsLabelBytes.Add(nameAsBytes)
				Next i
			End If

			'Param names group
			bufferSize += 4 'Header; always present
			Dim paramNames As IList(Of String) = getParamNames()
			For Each s As String In paramNames
				bufferSize += 4 'header for each entry
				bufferSize += SbeUtil.toBytes(True, s).Length 'Content
			Next s

			'Layer names group
			bufferSize += 4 'Header; always present
			Dim layerNames As IList(Of String) = getlayerNames()
			For Each s As String In layerNames
				bufferSize += 4
				bufferSize += SbeUtil.toBytes(True, s).Length 'Content
			Next s

			'Per parameter and per layer (activations) stats group length
			bufferSize += 4 'Per parameter/layer stats group header: always present
			Dim nEntries As Integer = paramNames.Count + layerNames.Count
			bufferSize += nEntries * 12 'Each parameter/layer entry: has learning rate -> float -> 4 bytes PLUS headers for 2 nested groups: 2*4 = 8 each -> 12 bytes total
			bufferSize += entrySize(paramNames, StatsType.Parameters, StatsType.Gradients, StatsType.Updates)
			bufferSize += entrySize(layerNames, StatsType.Activations)

			'Metadata group:
			bufferSize += 4 'Metadata group header: always present
			If dataSetMetaData IsNot Nothing AndAlso dataSetMetaData.Count > 0 Then
				For Each b As SByte() In dataSetMetaData
					bufferSize += 4 + b.Length '4 bytes header + content
				Next b
			End If

			'Session/worker IDs
			Dim bSessionID() As SByte = SbeUtil.toBytes(True, sessionID_Conflict)
			Dim bTypeID() As SByte = SbeUtil.toBytes(True, typeID_Conflict)
			Dim bWorkerID() As SByte = SbeUtil.toBytes(True, workerID_Conflict)
			bufferSize += bSessionID.Length + bTypeID.Length + bWorkerID.Length

			'Metadata class name:
			Dim metaDataClassNameBytes() As SByte = SbeUtil.toBytes(True, metaDataClassName)
			bufferSize += metaDataClassNameBytes.Length

			Return bufferSize
		End Function

		Private Function entrySize(ByVal entryNames As IList(Of String), ParamArray ByVal statsTypes() As StatsType) As Integer
			Dim bufferSize As Integer = 0
			For Each s As String In entryNames
				'For each parameter: MAY also have a number of summary stats (mean, stdev etc), and histograms (both as nested groups)
				Dim summaryStatsCount As Integer = 0
				For Each statsType As StatsType In statsTypes 'Parameters, Gradients, updates, activations
					For Each summaryType As SummaryType In System.Enum.GetValues(GetType(SummaryType)) 'Mean, stdev, MM
						Dim map As IDictionary(Of String, Double) = mapForTypes(statsType, summaryType)
						If map Is Nothing Then
							Continue For
						End If
						summaryStatsCount += 1
					Next summaryType
				Next statsType
				'Each summary stat value: StatsType (uint8), SummaryType (uint8), value (double) -> 1+1+8 = 10 bytes
				bufferSize += summaryStatsCount * 10

				'Histograms for this parameter
				Dim nHistogramsThisParam As Integer = 0
				If histograms IsNot Nothing AndAlso histograms.Count > 0 Then
					For Each map As IDictionary(Of String, Histogram) In histograms.Values
						If map IsNot Nothing AndAlso map.ContainsKey(s) Then
							nHistogramsThisParam += 1
						End If
					Next map
				End If
				'For each histogram: StatsType (uint8) + 2x double + int32 -> 1 + 2*8 + 4 = 21 bytes PLUS counts group header (4 bytes) -> 25 bytes fixed per histogram
				bufferSize += 25 * nHistogramsThisParam
				'PLUS, the number of count values, given by nBins...
				Dim nBinCountEntries As Integer = 0
				For Each statsType As StatsType In statsTypes
					If histograms Is Nothing OrElse Not histograms.ContainsKey(statsType) Then
						Continue For
					End If
					Dim map As IDictionary(Of String, Histogram) = histograms(statsType)
					If map IsNot Nothing AndAlso map.ContainsKey(s) Then 'If it doesn't: assume 0 count...
						nBinCountEntries += map(s).getNBins()
					End If
				Next statsType
				bufferSize += 4 * nBinCountEntries 'Each entry: uint32 -> 4 bytes
			Next s
			Return bufferSize
		End Function

		Private ReadOnly Property ParamNames As IList(Of String)
			Get
				Dim paramNames As ISet(Of String) = New LinkedHashSet(Of String)()
				If learningRatesByParam IsNot Nothing Then
					paramNames.addAll(learningRatesByParam.Keys)
				End If
				If histograms IsNot Nothing Then
					addToSet(paramNames, histograms(StatsType.Parameters))
					addToSet(paramNames, histograms(StatsType.Gradients))
					addToSet(paramNames, histograms(StatsType.Updates))
				End If
				If meanValues IsNot Nothing Then
					addToSet(paramNames, meanValues(StatsType.Parameters))
					addToSet(paramNames, meanValues(StatsType.Gradients))
					addToSet(paramNames, meanValues(StatsType.Updates))
				End If
				If stdevValues IsNot Nothing Then
					addToSet(paramNames, stdevValues(StatsType.Parameters))
					addToSet(paramNames, stdevValues(StatsType.Gradients))
					addToSet(paramNames, stdevValues(StatsType.Updates))
				End If
				If meanMagnitudeValues IsNot Nothing Then
					addToSet(paramNames, meanMagnitudeValues(StatsType.Parameters))
					addToSet(paramNames, meanMagnitudeValues(StatsType.Gradients))
					addToSet(paramNames, meanMagnitudeValues(StatsType.Updates))
				End If
				Return New List(Of String)(paramNames)
			End Get
		End Property

		Private Function getlayerNames() As IList(Of String)
			Dim layerNames As ISet(Of String) = New LinkedHashSet(Of String)()
			If histograms IsNot Nothing Then
				addToSet(layerNames, histograms(StatsType.Activations))
			End If
			If meanValues IsNot Nothing Then
				addToSet(layerNames, meanValues(StatsType.Activations))
			End If
			If stdevValues IsNot Nothing Then
				addToSet(layerNames, stdevValues(StatsType.Activations))
			End If
			If meanMagnitudeValues IsNot Nothing Then
				addToSet(layerNames, meanMagnitudeValues(StatsType.Activations))
			End If
			Return New List(Of String)(layerNames)
		End Function

		Private Sub addToSet(Of T1)(ByVal set As ISet(Of String), ByVal map As IDictionary(Of T1))
			If map Is Nothing Then
				Return
			End If
			set.addAll(map.Keys)
		End Sub

		Public Overridable Function encode() As SByte()
			Dim bytes(encodingLengthBytes() - 1) As SByte
			Dim buffer As MutableDirectBuffer = New UnsafeBuffer(bytes)
			encode(buffer)
			Return bytes
		End Function

		Public Overridable Sub encode(ByVal buffer As ByteBuffer)
			encode(New UnsafeBuffer(buffer))
		End Sub

		Public Overridable Sub encode(ByVal buffer As MutableDirectBuffer) Implements AgronaPersistable.encode
			Dim enc As New MessageHeaderEncoder()
			Dim ue As New UpdateEncoder()

			enc.wrap(buffer, 0).blockLength(ue.sbeBlockLength()).templateId(ue.sbeTemplateId()).schemaId(ue.sbeSchemaId()).version(ue.sbeSchemaVersion())

			Dim offset As Integer = enc.encodedLength() 'Expect 8 bytes
			ue.wrap(buffer, offset)

			'Fixed length fields: always encoded
			ue.time(timeStamp_Conflict).deltaTime(0).iterationCount(iterationCount).fieldsPresent().score(scorePresent).memoryUse(memoryUsePresent).performance(performanceStatsPresent).garbageCollection(gcStats IsNot Nothing AndAlso gcStats.Count > 0).histogramParameters(histograms IsNot Nothing AndAlso histograms.ContainsKey(StatsType.Parameters)).histogramActivations(histograms IsNot Nothing AndAlso histograms.ContainsKey(StatsType.Gradients)).histogramUpdates(histograms IsNot Nothing AndAlso histograms.ContainsKey(StatsType.Updates)).histogramActivations(histograms IsNot Nothing AndAlso histograms.ContainsKey(StatsType.Activations)).meanParameters(meanValues IsNot Nothing AndAlso meanValues.ContainsKey(StatsType.Parameters)).meanGradients(meanValues IsNot Nothing AndAlso meanValues.ContainsKey(StatsType.Gradients)).meanUpdates(meanValues IsNot Nothing AndAlso meanValues.ContainsKey(StatsType.Updates)).meanActivations(meanValues IsNot Nothing AndAlso meanValues.ContainsKey(StatsType.Activations)).meanMagnitudeParameters(meanMagnitudeValues IsNot Nothing AndAlso meanMagnitudeValues.ContainsKey(StatsType.Parameters)).meanMagnitudeGradients(meanMagnitudeValues IsNot Nothing AndAlso meanMagnitudeValues.ContainsKey(StatsType.Gradients)).meanMagnitudeUpdates(meanMagnitudeValues IsNot Nothing AndAlso meanMagnitudeValues.ContainsKey(StatsType.Updates)).meanMagnitudeActivations(meanMagnitudeValues IsNot Nothing AndAlso meanMagnitudeValues.ContainsKey(StatsType.Activations)).learningRatesPresent(learningRatesByParam IsNot Nothing).dataSetMetaDataPresent(hasDataSetMetaData())

			ue.statsCollectionDuration(statsCollectionDurationMs).score(score)

			Dim memoryUseCount As Integer
			If Not memoryUsePresent Then
				memoryUseCount = 0
			Else
				memoryUseCount = 4 + (If(deviceCurrentBytes Is Nothing, 0, deviceCurrentBytes.Length)) + (If(deviceMaxBytes Is Nothing, 0, deviceMaxBytes.Length))
			End If

			Dim mue As UpdateEncoder.MemoryUseEncoder = ue.memoryUseCount(memoryUseCount)
			If memoryUsePresent Then
				mue.next().memoryType(MemoryType.JvmCurrent).memoryBytes(jvmCurrentBytes).next().memoryType(MemoryType.JvmMax).memoryBytes(jvmMaxBytes).next().memoryType(MemoryType.OffHeapCurrent).memoryBytes(offHeapCurrentBytes).next().memoryType(MemoryType.OffHeapMax).memoryBytes(offHeapMaxBytes)
				If deviceCurrentBytes IsNot Nothing Then
					For i As Integer = 0 To deviceCurrentBytes.Length - 1
						mue.next().memoryType(MemoryType.DeviceCurrent).memoryBytes(deviceCurrentBytes(i))
					Next i
				End If
				If deviceMaxBytes IsNot Nothing Then
					For i As Integer = 0 To deviceMaxBytes.Length - 1
						mue.next().memoryType(MemoryType.DeviceMax).memoryBytes(deviceMaxBytes(i))
					Next i
				End If
			End If

			Dim pe As UpdateEncoder.PerformanceEncoder = ue.performanceCount(If(performanceStatsPresent, 1, 0))
			If performanceStatsPresent Then
				pe.next().totalRuntimeMs(totalRuntimeMs).totalExamples(totalExamples).totalMinibatches(totalMinibatches).examplesPerSecond(CSng(examplesPerSecond)).minibatchesPerSecond(CSng(minibatchesPerSecond))
			End If

			Dim gce As UpdateEncoder.GcStatsEncoder = ue.gcStatsCount(If(gcStats Is Nothing OrElse gcStats.Count = 0, 0, gcStats.Count))
			Dim gcStatsLabelBytes As IList(Of SByte()) = Nothing
			If gcStats IsNot Nothing AndAlso gcStats.Count > 0 Then
				gcStatsLabelBytes = New List(Of SByte())()
				For Each stats As GCStats In gcStats
					Dim nameAsBytes() As SByte = SbeUtil.toBytes(True, stats.gcName)
					gcStatsLabelBytes.Add(nameAsBytes)
				Next stats
			End If
			If gcStats IsNot Nothing AndAlso gcStats.Count > 0 Then
				Dim i As Integer = 0
				For Each g As GCStats In gcStats
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: byte[] gcLabelBytes = gcStatsLabelBytes.get(i++);
					Dim gcLabelBytes() As SByte = gcStatsLabelBytes(i)
						i += 1
					gce.next().deltaGCCount(g.deltaGCCount).deltaGCTimeMs(g.deltaGCTime).putGcName(gcLabelBytes, 0, gcLabelBytes.Length)
				Next g
			End If

			'Param names
			Dim paramNames As IList(Of String) = getParamNames()
			Dim pne As UpdateEncoder.ParamNamesEncoder = ue.paramNamesCount(paramNames.Count)
			For Each s As String In paramNames
				pne.next().paramName(s)
			Next s

			'Layer names
			Dim layerNames As IList(Of String) = getlayerNames()
			Dim lne As UpdateEncoder.LayerNamesEncoder = ue.layerNamesCount(layerNames.Count)
			For Each s As String In layerNames
				lne.next().layerName(s)
			Next s

			' +++++ Per Parameter Stats +++++
			Dim ppe As UpdateEncoder.PerParameterStatsEncoder = ue.perParameterStatsCount(paramNames.Count + layerNames.Count)
			Dim st() As StatsType = {StatsType.Parameters, StatsType.Gradients, StatsType.Updates}
			For Each s As String In paramNames
				ppe = ppe.next()
				Dim lr As Single = 0.0f
				If learningRatesByParam IsNot Nothing AndAlso learningRatesByParam.ContainsKey(s) Then
					lr = learningRatesByParam(s).floatValue()
				End If
				ppe.learningRate(lr)

				Dim summaryStatsCount As Integer = 0
				For Each statsType As StatsType In st 'Parameters, updates
					For Each summaryType As SummaryType In System.Enum.GetValues(GetType(SummaryType)) 'Mean, stdev, MM
						Dim map As IDictionary(Of String, Double) = mapForTypes(statsType, summaryType)
						If map Is Nothing OrElse map.Count = 0 Then
							Continue For
						End If
						summaryStatsCount += 1
					Next summaryType
				Next statsType

				Dim sse As UpdateEncoder.PerParameterStatsEncoder.SummaryStatEncoder = ppe.summaryStatCount(summaryStatsCount)

				'Summary stats
				For Each statsType As StatsType In st 'Parameters, updates
					For Each summaryType As SummaryType In System.Enum.GetValues(GetType(SummaryType)) 'Mean, stdev, MM
						Dim map As IDictionary(Of String, Double) = mapForTypes(statsType, summaryType)
						If map Is Nothing OrElse map.Count = 0 Then
							Continue For
						End If
						appendOrDefault(sse, s, statsType, summaryType, map, Double.NaN)
					Next summaryType
				Next statsType

				Dim nHistogramsThisParam As Integer = 0
				If histograms IsNot Nothing AndAlso histograms.Count > 0 Then
					For Each statsType As StatsType In st 'Parameters, updates
						Dim map As IDictionary(Of String, Histogram) = histograms(statsType)
						If map Is Nothing Then
							Continue For
						End If
						If map.ContainsKey(s) Then
							nHistogramsThisParam += 1
						End If
					Next statsType
				End If



				'Histograms
				Dim sshe As UpdateEncoder.PerParameterStatsEncoder.HistogramsEncoder = ppe.histogramsCount(nHistogramsThisParam)
				If nHistogramsThisParam > 0 Then
					For Each statsType As StatsType In st
						Dim map As IDictionary(Of String, Histogram) = histograms(statsType)
						If map Is Nothing OrElse Not map.ContainsKey(s) Then
							Continue For
						End If
						Dim h As Histogram = map(s) 'Histogram for StatsType for this parameter
						Dim min As Double
						Dim max As Double
						Dim nBins As Integer
						Dim binCounts() As Integer
						If h Is Nothing Then
							min = 0.0
							max = 0.0
							nBins = 0
							binCounts = Nothing
						Else
							min = h.getMin()
							max = h.getMax()
							nBins = h.getNBins()
							binCounts = h.getBinCounts()
						End If

						sshe = sshe.next().statType(translate(statsType)).minValue(min).maxValue(max).nBins(nBins)
						Dim histCountsEncoder As UpdateEncoder.PerParameterStatsEncoder.HistogramsEncoder.HistogramCountsEncoder = sshe.histogramCountsCount(nBins)
						For i As Integer = 0 To nBins - 1
							Dim count As Integer = (If(binCounts Is Nothing OrElse binCounts.Length <= i, 0, binCounts(i)))
							histCountsEncoder.next().binCount(count)
						Next i
					Next statsType
				End If
			Next s

			For Each s As String In layerNames
				ppe = ppe.next()
				ppe.learningRate(0.0f) 'Not applicable

				Dim summaryStatsCount As Integer = 0
				For Each summaryType As SummaryType In System.Enum.GetValues(GetType(SummaryType)) 'Mean, stdev, MM
					Dim map As IDictionary(Of String, Double) = mapForTypes(StatsType.Activations, summaryType)
					If map Is Nothing OrElse map.Count = 0 Then
						Continue For
					End If
					If map.ContainsKey(s) Then
						summaryStatsCount += 1
					End If
				Next summaryType

				Dim sse As UpdateEncoder.PerParameterStatsEncoder.SummaryStatEncoder = ppe.summaryStatCount(summaryStatsCount)

				'Summary stats
				For Each summaryType As SummaryType In System.Enum.GetValues(GetType(SummaryType)) 'Mean, stdev, MM
					Dim map As IDictionary(Of String, Double) = mapForTypes(StatsType.Activations, summaryType)
					If map Is Nothing OrElse map.Count = 0 Then
						Continue For
					End If
					appendOrDefault(sse, s, StatsType.Activations, summaryType, map, Double.NaN)
				Next summaryType

				Dim nHistogramsThisLayer As Integer = 0
				If histograms IsNot Nothing AndAlso histograms.Count > 0 Then
					For Each map As IDictionary(Of String, Histogram) In histograms.Values
						If map IsNot Nothing AndAlso map.ContainsKey(s) Then
							nHistogramsThisLayer += 1
						End If
					Next map
				End If

				'Histograms
				Dim sshe As UpdateEncoder.PerParameterStatsEncoder.HistogramsEncoder = ppe.histogramsCount(nHistogramsThisLayer)
				If nHistogramsThisLayer > 0 Then
					Dim map As IDictionary(Of String, Histogram) = histograms(StatsType.Activations)
					If map Is Nothing OrElse Not map.ContainsKey(s) Then
						Continue For
					End If
					Dim h As Histogram = map(s) 'Histogram for StatsType for this parameter
					Dim min As Double
					Dim max As Double
					Dim nBins As Integer
					Dim binCounts() As Integer
					If h Is Nothing Then
						min = 0.0
						max = 0.0
						nBins = 0
						binCounts = Nothing
					Else
						min = h.getMin()
						max = h.getMax()
						nBins = h.getNBins()
						binCounts = h.getBinCounts()
					End If

					sshe = sshe.next().statType(translate(StatsType.Activations)).minValue(min).maxValue(max).nBins(nBins)
					Dim histCountsEncoder As UpdateEncoder.PerParameterStatsEncoder.HistogramsEncoder.HistogramCountsEncoder = sshe.histogramCountsCount(nBins)
					For i As Integer = 0 To nBins - 1
						Dim count As Integer = (If(binCounts Is Nothing OrElse binCounts.Length <= i, 0, binCounts(i)))
						histCountsEncoder.next().binCount(count)
					Next i
				End If
			Next s

			' +++ DataSet MetaData +++
			Dim metaEnc As UpdateEncoder.DataSetMetaDataBytesEncoder = ue.dataSetMetaDataBytesCount(If(dataSetMetaData IsNot Nothing, dataSetMetaData.Count, 0))
			If dataSetMetaData IsNot Nothing AndAlso dataSetMetaData.Count > 0 Then
				For Each b As SByte() In dataSetMetaData
					metaEnc = metaEnc.next()
					Dim mdbe As UpdateEncoder.DataSetMetaDataBytesEncoder.MetaDataBytesEncoder = metaEnc.metaDataBytesCount(b.Length)
					For Each bb As SByte In b
						mdbe.next().bytes(bb)
					Next bb
				Next b
			End If

			'Session/worker IDs
			Dim bSessionID() As SByte = SbeUtil.toBytes(True, sessionID_Conflict)
			Dim bTypeID() As SByte = SbeUtil.toBytes(True, typeID_Conflict)
			Dim bWorkerID() As SByte = SbeUtil.toBytes(True, workerID_Conflict)
			ue.putSessionID(bSessionID, 0, bSessionID.Length)
			ue.putTypeID(bTypeID, 0, bTypeID.Length)
			ue.putWorkerID(bWorkerID, 0, bWorkerID.Length)

			'Class name for DataSet metadata
			Dim metaDataClassNameBytes() As SByte = SbeUtil.toBytes(True, metaDataClassName)
			ue.putDataSetMetaDataClassName(metaDataClassNameBytes, 0, metaDataClassNameBytes.Length)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void encode(OutputStream outputStream) throws IOException
		Public Overridable Sub encode(ByVal outputStream As Stream)
			'TODO there may be more efficient way of doing this
			outputStream.Write(encode(), 0, encode().Length)
		End Sub

		Public Overridable Sub decode(ByVal decode() As SByte)
			Dim buffer As MutableDirectBuffer = New UnsafeBuffer(decode)
			decode(buffer)
		End Sub

		Public Overridable Sub decode(ByVal buffer As ByteBuffer)
			decode(New UnsafeBuffer(buffer))
		End Sub

		Public Overridable Sub decode(ByVal buffer As DirectBuffer) Implements AgronaPersistable.decode
			'TODO we could do this more efficiently, with buffer re-use, etc.
			Dim dec As New MessageHeaderDecoder()
			Dim ud As New UpdateDecoder()
			dec.wrap(buffer, 0)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int blockLength = dec.blockLength();
			Dim blockLength As Integer = dec.blockLength()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int version = dec.version();
			Dim version As Integer = dec.version()

			Dim headerLength As Integer = dec.encodedLength()
			'TODO: in general, we'd check the header, version, schema etc.

			ud.wrap(buffer, headerLength, blockLength, version)

			'TODO iteration count
			timeStamp_Conflict = ud.time()
			Dim deltaTime As Long = ud.deltaTime() 'TODO
			iterationCount = ud.iterationCount()

			Dim fpd As UpdateFieldsPresentDecoder = ud.fieldsPresent()
			scorePresent = fpd.score()
			memoryUsePresent = fpd.memoryUse()
			performanceStatsPresent = fpd.performance()
			Dim gc As Boolean = fpd.garbageCollection()
			Dim histogramParameters As Boolean = fpd.histogramParameters()
			Dim histogramUpdates As Boolean = fpd.histogramUpdates()
			Dim histogramActivations As Boolean = fpd.histogramActivations()
			Dim meanParameters As Boolean = fpd.meanParameters()
			Dim meanUpdates As Boolean = fpd.meanUpdates()
			Dim meanActivations As Boolean = fpd.meanActivations()
			Dim meanMagParams As Boolean = fpd.meanMagnitudeParameters()
			Dim meanMagUpdates As Boolean = fpd.meanMagnitudeUpdates()
			Dim meanMagAct As Boolean = fpd.meanMagnitudeActivations()
			Dim learningRatesPresent As Boolean = fpd.learningRatesPresent()
			Dim metaDataPresent As Boolean = fpd.dataSetMetaDataPresent()

			statsCollectionDurationMs = ud.statsCollectionDuration()
			score = ud.score()

			'First group: memory use
			Dim mud As UpdateDecoder.MemoryUseDecoder = ud.memoryUse()
			Dim dcMem As IList(Of Long) = Nothing 'TODO avoid
			Dim dmMem As IList(Of Long) = Nothing
			For Each m As UpdateDecoder.MemoryUseDecoder In mud
				Dim type As MemoryType = m.memoryType()
				Dim memBytes As Long = m.memoryBytes()
				Select Case type.innerEnumValue
					Case org.deeplearning4j.ui.model.stats.sbe.MemoryType.InnerEnum.JvmCurrent
						jvmCurrentBytes = memBytes
					Case org.deeplearning4j.ui.model.stats.sbe.MemoryType.InnerEnum.JvmMax
						jvmMaxBytes = memBytes
					Case org.deeplearning4j.ui.model.stats.sbe.MemoryType.InnerEnum.OffHeapCurrent
						offHeapCurrentBytes = memBytes
					Case org.deeplearning4j.ui.model.stats.sbe.MemoryType.InnerEnum.OffHeapMax
						offHeapMaxBytes = memBytes
					Case org.deeplearning4j.ui.model.stats.sbe.MemoryType.InnerEnum.DeviceCurrent
						If dcMem Is Nothing Then
							dcMem = New List(Of Long)()
						End If
						dcMem.Add(memBytes)
					Case org.deeplearning4j.ui.model.stats.sbe.MemoryType.InnerEnum.DeviceMax
						If dmMem Is Nothing Then
							dmMem = New List(Of Long)()
						End If
						dmMem.Add(memBytes)
					Case org.deeplearning4j.ui.model.stats.sbe.MemoryType.InnerEnum.NULL_VAL
				End Select
			Next m
			If dcMem IsNot Nothing Then
				Dim a(dcMem.Count - 1) As Long
				Dim i As Integer = 0
				For Each l As Long? In dcMem
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: a[i++] = l;
					a(i) = l
						i += 1
				Next l
				deviceCurrentBytes = a
			End If
			If dmMem IsNot Nothing Then
				Dim a(dmMem.Count - 1) As Long
				Dim i As Integer = 0
				For Each l As Long? In dmMem
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: a[i++] = l;
					a(i) = l
						i += 1
				Next l
				deviceMaxBytes = a
			End If

			'Second group: performance stats (0 or 1 entries only)
			For Each pd As UpdateDecoder.PerformanceDecoder In ud.performance()
				totalRuntimeMs = pd.totalRuntimeMs()
				totalExamples = pd.totalExamples()
				totalMinibatches = pd.totalMinibatches()
				examplesPerSecond = pd.examplesPerSecond()
				minibatchesPerSecond = pd.minibatchesPerSecond()
			Next pd

			'Third group: GC stats
			For Each gcsd As UpdateDecoder.GcStatsDecoder In ud.gcStats()
				If gcStats Is Nothing Then
					gcStats = New List(Of GCStats)()
				End If
				Dim deltaGCCount As Integer = gcsd.deltaGCCount()
				Dim deltaGCTimeMs As Integer = gcsd.deltaGCTimeMs()
				Dim gcName As String = gcsd.gcName()
				Dim s As New GCStats(gcName, deltaGCCount, deltaGCTimeMs) 'TODO delta time...
				gcStats.Add(s)
			Next gcsd

			'Fourth group: param names
			Dim pnd As UpdateDecoder.ParamNamesDecoder = ud.paramNames()
			Dim nParams As Integer = pnd.count()
			Dim paramNames As IList(Of String) = Nothing
			If nParams > 0 Then
				paramNames = New List(Of String)(nParams)
			End If
			For Each pndec As UpdateDecoder.ParamNamesDecoder In pnd
				paramNames.Add(pndec.paramName())
			Next pndec

			'Fifth group: layer names
			Dim lnd As UpdateDecoder.LayerNamesDecoder = ud.layerNames()
			Dim nLayers As Integer = lnd.count()
			Dim layerNames As IList(Of String) = Nothing
			If nLayers > 0 Then
				layerNames = New List(Of String)(nLayers)
			End If
			For Each l As UpdateDecoder.LayerNamesDecoder In lnd
				layerNames.Add(l.layerName())
			Next l


			'Sixth group: Per parameter stats (and histograms, etc) AND per layer stats
			Dim entryNum As Integer = 0
			For Each ppsd As UpdateDecoder.PerParameterStatsDecoder In ud.perParameterStats()
				Dim isParam As Boolean = entryNum < nParams
				Dim name As String = (If(isParam, paramNames(entryNum), layerNames(entryNum - nParams)))
				entryNum += 1

				Dim lr As Single = ppsd.learningRate()

				If learningRatesPresent AndAlso isParam Then
					If learningRatesByParam Is Nothing Then
						learningRatesByParam = New Dictionary(Of String, Double)()
					End If
					learningRatesByParam(name) = CDbl(lr)
				End If

				'Summary stats (mean/stdev/mean magnitude)
				For Each ssd As UpdateDecoder.PerParameterStatsDecoder.SummaryStatDecoder In ppsd.summaryStat()
					Dim st As StatsType = translate(ssd.statType())
					Dim summaryType As SummaryType = translate(ssd.summaryType())
					Dim value As Double = ssd.value()

					Select Case summaryType
						Case SummaryType.Mean
							If meanValues Is Nothing Then
								meanValues = New Dictionary(Of StatsType, IDictionary(Of String, Double))()
							End If
							Dim map As IDictionary(Of String, Double) = meanValues(st)
							If map Is Nothing Then
								map = New Dictionary(Of String, Double)()
								meanValues(st) = map
							End If
							map(name) = value
						Case SummaryType.Stdev
							If stdevValues Is Nothing Then
								stdevValues = New Dictionary(Of StatsType, IDictionary(Of String, Double))()
							End If
							Dim map2 As IDictionary(Of String, Double) = stdevValues(st)
							If map2 Is Nothing Then
								map2 = New Dictionary(Of String, Double)()
								stdevValues(st) = map2
							End If
							map2(name) = value
						Case SummaryType.MeanMagnitudes
							If meanMagnitudeValues Is Nothing Then
								meanMagnitudeValues = New Dictionary(Of StatsType, IDictionary(Of String, Double))()
							End If
							Dim map3 As IDictionary(Of String, Double) = meanMagnitudeValues(st)
							If map3 Is Nothing Then
								map3 = New Dictionary(Of String, Double)()
								meanMagnitudeValues(st) = map3
							End If
							map3(name) = value
					End Select
				Next ssd

				'Histograms
				For Each hd As UpdateDecoder.PerParameterStatsDecoder.HistogramsDecoder In ppsd.histograms()
					Dim st As StatsType = translate(hd.statType())
					Dim min As Double = hd.minValue()
					Dim max As Double = hd.maxValue()
					Dim nBins As Integer = hd.nBins()
					Dim binCounts(nBins - 1) As Integer
					Dim i As Integer = 0
					For Each hcd As UpdateDecoder.PerParameterStatsDecoder.HistogramsDecoder.HistogramCountsDecoder In hd.histogramCounts()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: binCounts[i++] = (int) hcd.binCount();
						binCounts(i) = CInt(hcd.binCount())
							i += 1
					Next hcd

					Dim h As New Histogram(min, max, nBins, binCounts)
					If histograms Is Nothing Then
						histograms = New Dictionary(Of StatsType, IDictionary(Of String, Histogram))()
					End If
					Dim map As IDictionary(Of String, Histogram) = histograms(st)
					If map Is Nothing Then
						map = New Dictionary(Of String, Histogram)()
						histograms(st) = map
					End If
					map(name) = h
				Next hd
			Next ppsd

			'Final group: DataSet metadata
			For Each metaDec As UpdateDecoder.DataSetMetaDataBytesDecoder In ud.dataSetMetaDataBytes()
				If Me.dataSetMetaData Is Nothing Then
					Me.dataSetMetaData = New List(Of SByte())()
				End If
				Dim mdbd As UpdateDecoder.DataSetMetaDataBytesDecoder.MetaDataBytesDecoder = metaDec.metaDataBytes()
				Dim length As Integer = mdbd.count()
				Dim b(length - 1) As SByte
				Dim i As Integer = 0
				For Each mdbd2 As UpdateDecoder.DataSetMetaDataBytesDecoder.MetaDataBytesDecoder In mdbd
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: b[i++] = mdbd2.bytes();
					b(i) = mdbd2.bytes()
						i += 1
				Next mdbd2
				Me.dataSetMetaData.Add(b)
			Next metaDec

			'IDs
			Me.sessionID_Conflict = ud.sessionID()
			Me.typeID_Conflict = ud.typeID()
			Me.workerID_Conflict = ud.workerID()

			'Variable length: DataSet metadata class name
			Me.metaDataClassName = ud.dataSetMetaDataClassName()
			If Not metaDataPresent Then
				Me.metaDataClassName = Nothing
			End If
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void decode(InputStream inputStream) throws IOException
		Public Overridable Sub decode(ByVal inputStream As Stream)
			Dim bytes() As SByte = IOUtils.toByteArray(inputStream)
			decode(bytes)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data private static class GCStats implements Serializable
		<Serializable>
		Private Class GCStats
			Friend gcName As String
			Friend deltaGCCount As Integer
			Friend deltaGCTime As Integer
		End Class
	End Class

End Namespace