Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Reflection
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports ToString = lombok.ToString
Imports IOUtils = org.apache.commons.compress.utils.IOUtils
Imports Histogram = org.deeplearning4j.ui.model.stats.api.Histogram
Imports StatsReport = org.deeplearning4j.ui.model.stats.api.StatsReport
Imports StatsType = org.deeplearning4j.ui.model.stats.api.StatsType
Imports SummaryType = org.deeplearning4j.ui.model.stats.api.SummaryType
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

Namespace org.deeplearning4j.ui.model.stats.impl.java


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode @ToString @Data public class JavaStatsReport implements org.deeplearning4j.ui.model.stats.api.StatsReport
	<Serializable>
	Public Class JavaStatsReport
		Implements StatsReport

		Private sessionID As String
		Private typeID As String
		Private workerID As String
		Private timeStamp As Long

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
			Me.sessionID = sessionID
			Me.typeID = typeID
			Me.workerID = workerID
			Me.timeStamp = timeStamp
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

		Public Overridable Sub reportLearningRates(ByVal learningRatesByParam As IDictionary(Of String, Double)) Implements StatsReport.reportLearningRates
			Me.learningRatesByParam = learningRatesByParam
		End Sub

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

		Public Overridable ReadOnly Property GarbageCollectionStats As IList(Of Pair(Of String, Integer())) Implements StatsReport.getGarbageCollectionStats
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

		Public Overridable Sub reportDataSetMetaData(ByVal dataSetMetaData As IList(Of Serializable), ByVal metaDataClass As Type) Implements StatsReport.reportDataSetMetaData
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			reportDataSetMetaData(dataSetMetaData, (If(metaDataClass Is Nothing, Nothing, metaDataClass.FullName)))
		End Sub

		Public Overridable Sub reportDataSetMetaData(ByVal dataSetMetaData As IList(Of Serializable), ByVal metaDataClass As String) Implements StatsReport.reportDataSetMetaData
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

		Public Overridable ReadOnly Property DataSetMetaData As IList(Of Serializable) Implements StatsReport.getDataSetMetaData
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

		Public Overridable ReadOnly Property LearningRates As IDictionary(Of String, Double) Implements StatsReport.getLearningRates
			Get
				Return Me.learningRatesByParam
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

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data private static class GCStats implements Serializable
		<Serializable>
		Private Class GCStats
			Friend gcName As String
			Friend deltaGCCount As Integer
			Friend deltaGCTime As Integer
		End Class

		Public Overridable Function encodingLengthBytes() As Integer
			'TODO - presumably a more efficient way to do this
			Dim encoded() As SByte = encode()
			Return encoded.Length
		End Function

		Public Overridable Function encode() As SByte()
			Dim baos As New MemoryStream()
			Try
					Using oos As New ObjectOutputStream(baos)
					oos.writeObject(Me)
					End Using
			Catch e As IOException
				Throw New Exception(e) 'Should never happen
			End Try
			Return baos.toByteArray()
		End Function

		Public Overridable Sub encode(ByVal buffer As ByteBuffer)
			buffer.put(encode())
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void encode(OutputStream outputStream) throws IOException
		Public Overridable Sub encode(ByVal outputStream As Stream)
			Using oos As New ObjectOutputStream(outputStream)
				oos.writeObject(Me)
			End Using
		End Sub

		Public Overridable Sub decode(ByVal decode() As SByte)
			Dim r As JavaStatsReport
			Try
					Using ois As New ObjectInputStream(New MemoryStream(decode))
					r = CType(ois.readObject(), JavaStatsReport)
					End Using
			Catch e As Exception When TypeOf e Is IOException OrElse TypeOf e Is ClassNotFoundException
				Throw New Exception(e) 'Should never happen
			End Try

			Dim fields() As System.Reflection.FieldInfo = GetType(JavaStatsReport).GetFields(BindingFlags.DeclaredOnly Or BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.Static Or BindingFlags.Instance)
			For Each f As System.Reflection.FieldInfo In fields
				f.setAccessible(True)
				Try
					f.set(Me, f.get(r))
				Catch e As IllegalAccessException
					Throw New Exception(e) 'Should never happen
				End Try
			Next f
		End Sub

		Public Overridable Sub decode(ByVal buffer As ByteBuffer)
			Dim bytes(buffer.remaining() - 1) As SByte
			buffer.get(bytes)
			decode(bytes)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void decode(InputStream inputStream) throws IOException
		Public Overridable Sub decode(ByVal inputStream As Stream)
			decode(IOUtils.toByteArray(inputStream))
		End Sub
	End Class

End Namespace