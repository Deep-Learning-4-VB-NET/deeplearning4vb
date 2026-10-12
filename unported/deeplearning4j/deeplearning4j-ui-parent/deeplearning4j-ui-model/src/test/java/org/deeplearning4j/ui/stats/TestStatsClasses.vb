Imports System
Imports System.Collections.Generic
Imports System.IO
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports org.deeplearning4j.ui.model.stats.api
Imports SbeStatsInitializationReport = org.deeplearning4j.ui.model.stats.impl.SbeStatsInitializationReport
Imports SbeStatsReport = org.deeplearning4j.ui.model.stats.impl.SbeStatsReport
Imports JavaStatsInitializationReport = org.deeplearning4j.ui.model.stats.impl.java.JavaStatsInitializationReport
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports org.nd4j.common.primitives
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports org.junit.jupiter.api.Assertions

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

Namespace org.deeplearning4j.ui.stats


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.UI) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class TestStatsClasses extends org.deeplearning4j.BaseDL4JTest
	Public Class TestStatsClasses
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testStatsInitializationReport() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testStatsInitializationReport()

			Dim tf() As Boolean = {True, False}

			For Each useJ7 As Boolean In New Boolean() {False, True}

				'IDs
				Dim sessionID As String = "sid"
				Dim typeID As String = "tid"
				Dim workerID As String = "wid"
				Dim timestamp As Long = -1

				'Hardware info
				Dim jvmAvailableProcessors As Integer = 1
				Dim numDevices As Integer = 2
				Dim jvmMaxMemory As Long = 3
				Dim offHeapMaxMemory As Long = 4
				Dim deviceTotalMemory() As Long = {5, 6}
				Dim deviceDescription() As String = {"7", "8"}
				Dim hwUID As String = "8a"

				'Software info
				Dim arch As String = "9"
				Dim osName As String = "10"
				Dim jvmName As String = "11"
				Dim jvmVersion As String = "12"
				Dim jvmSpecVersion As String = "13"
				Dim nd4jBackendClass As String = "14"
				Dim nd4jDataTypeName As String = "15"
				Dim hostname As String = "15a"
				Dim jvmUID As String = "15b"
				Dim swEnvInfo As IDictionary(Of String, String) = New Dictionary(Of String, String)()
				swEnvInfo("env15c-1") = "SomeData"
				swEnvInfo("env15c-2") = "OtherData"
				swEnvInfo("env15c-3") = "EvenMoreData"

				'Model info
				Dim modelClassName As String = "16"
				Dim modelConfigJson As String = "17"
				Dim modelparamNames() As String = {"18", "19", "20", "21"}
				Dim numLayers As Integer = 22
				Dim numParams As Long = 23


				For Each hasHardwareInfo As Boolean In tf
					For Each hasSoftwareInfo As Boolean In tf
						For Each hasModelInfo As Boolean In tf

							Dim report As StatsInitializationReport
							If useJ7 Then
								report = New JavaStatsInitializationReport()
							Else
								report = New SbeStatsInitializationReport()
							End If

							report.reportIDs(sessionID, typeID, workerID, timestamp)

							If hasHardwareInfo Then
								report.reportHardwareInfo(jvmAvailableProcessors, numDevices, jvmMaxMemory, offHeapMaxMemory, deviceTotalMemory, deviceDescription, hwUID)
							End If

							If hasSoftwareInfo Then
								report.reportSoftwareInfo(arch, osName, jvmName, jvmVersion, jvmSpecVersion, nd4jBackendClass, nd4jDataTypeName, hostname, jvmUID, swEnvInfo)
							End If

							If hasModelInfo Then
								report.reportModelInfo(modelClassName, modelConfigJson, modelparamNames, numLayers, numParams)
							End If

							Dim asBytes() As SByte = report.encode()

							Dim report2 As StatsInitializationReport ' = new SbeStatsInitializationReport();
							If useJ7 Then
								report2 = New JavaStatsInitializationReport()
							Else
								report2 = New SbeStatsInitializationReport()
							End If
							report2.decode(asBytes)

							assertEquals(report, report2)

							assertEquals(sessionID, report2.SessionID)
							assertEquals(typeID, report2.TypeID)
							assertEquals(workerID, report2.WorkerID)
							assertEquals(timestamp, report2.TimeStamp)

							If hasHardwareInfo Then
								assertEquals(jvmAvailableProcessors, report2.HwJvmAvailableProcessors)
								assertEquals(numDevices, report2.HwNumDevices)
								assertEquals(jvmMaxMemory, report2.HwJvmMaxMemory)
								assertEquals(offHeapMaxMemory, report2.HwOffHeapMaxMemory)
								assertArrayEquals(deviceTotalMemory, report2.HwDeviceTotalMemory)
								assertArrayEquals(deviceDescription, report2.HwDeviceDescription)
								assertEquals(hwUID, report2.HwHardwareUID)
								assertTrue(report2.hasHardwareInfo())
							Else
								assertFalse(report2.hasHardwareInfo())
							End If

							If hasSoftwareInfo Then
								assertEquals(arch, report2.SwArch)
								assertEquals(osName, report2.SwOsName)
								assertEquals(jvmName, report2.SwJvmName)
								assertEquals(jvmVersion, report2.SwJvmVersion)
								assertEquals(jvmSpecVersion, report2.SwJvmSpecVersion)
								assertEquals(nd4jBackendClass, report2.SwNd4jBackendClass)
								assertEquals(nd4jDataTypeName, report2.SwNd4jDataTypeName)
								assertEquals(jvmUID, report2.SwJvmUID)
								assertEquals(hostname, report2.SwHostName)
								assertEquals(swEnvInfo, report2.getSwEnvironmentInfo())
								assertTrue(report2.hasSoftwareInfo())
							Else
								assertFalse(report2.hasSoftwareInfo())
							End If

							If hasModelInfo Then
								assertEquals(modelClassName, report2.ModelClassName)
								assertEquals(modelConfigJson, report2.ModelConfigJson)
								assertArrayEquals(modelparamNames, report2.ModelParamNames)
								assertEquals(numLayers, report2.ModelNumLayers)
								assertEquals(numParams, report2.ModelNumParams)
								assertTrue(report2.hasModelInfo())
							Else
								assertFalse(report2.hasModelInfo())
							End If


							'Check standard Java serialization
							Dim baos As New MemoryStream()
							Dim oos As New ObjectOutputStream(baos)
							oos.writeObject(report)
							oos.close()

							Dim javaBytes() As SByte = baos.toByteArray()
							Dim ois As New ObjectInputStream(New MemoryStream(javaBytes))
							Dim report3 As StatsInitializationReport = DirectCast(ois.readObject(), StatsInitializationReport)

							assertEquals(report, report3)
						Next hasModelInfo
					Next hasSoftwareInfo
				Next hasHardwareInfo
			Next useJ7
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testStatsInitializationReportNullValues() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testStatsInitializationReportNullValues()
			'Sanity check: shouldn't have any issues with encoding/decoding null values...
			Dim tf() As Boolean = {True, False}

			For Each useJ7 As Boolean In New Boolean() {False, True}

				'Hardware info
				Dim jvmAvailableProcessors As Integer = 1
				Dim numDevices As Integer = 2
				Dim jvmMaxMemory As Long = 3
				Dim offHeapMaxMemory As Long = 4
				Dim deviceTotalMemory() As Long = Nothing
				Dim deviceDescription() As String = Nothing
				Dim hwUID As String = Nothing

				'Software info
				Dim arch As String = Nothing
				Dim osName As String = Nothing
				Dim jvmName As String = Nothing
				Dim jvmVersion As String = Nothing
				Dim jvmSpecVersion As String = Nothing
				Dim nd4jBackendClass As String = Nothing
				Dim nd4jDataTypeName As String = Nothing
				Dim hostname As String = Nothing
				Dim jvmUID As String = Nothing
				Dim swEnvInfo As IDictionary(Of String, String) = Nothing

				'Model info
				Dim modelClassName As String = Nothing
				Dim modelConfigJson As String = Nothing
				Dim modelparamNames() As String = Nothing
				Dim numLayers As Integer = 22
				Dim numParams As Long = 23


				For Each hasHardwareInfo As Boolean In tf
					For Each hasSoftwareInfo As Boolean In tf
						For Each hasModelInfo As Boolean In tf

							'System.out.println(hasHardwareInfo + "\t" + hasSoftwareInfo + "\t" + hasModelInfo);

							Dim report As StatsInitializationReport
							If useJ7 Then
								report = New JavaStatsInitializationReport()
							Else
								report = New SbeStatsInitializationReport()
							End If
							report.reportIDs(Nothing, Nothing, Nothing, -1)

							If hasHardwareInfo Then
								report.reportHardwareInfo(jvmAvailableProcessors, numDevices, jvmMaxMemory, offHeapMaxMemory, deviceTotalMemory, deviceDescription, hwUID)
							End If

							If hasSoftwareInfo Then
								report.reportSoftwareInfo(arch, osName, jvmName, jvmVersion, jvmSpecVersion, nd4jBackendClass, nd4jDataTypeName, hostname, jvmUID, swEnvInfo)
							End If

							If hasModelInfo Then
								report.reportModelInfo(modelClassName, modelConfigJson, modelparamNames, numLayers, numParams)
							End If

							Dim asBytes() As SByte = report.encode()

							Dim report2 As StatsInitializationReport
							If useJ7 Then
								report2 = New JavaStatsInitializationReport()
							Else
								report2 = New SbeStatsInitializationReport()
							End If
							report2.decode(asBytes)

							If hasHardwareInfo Then
								assertEquals(jvmAvailableProcessors, report2.HwJvmAvailableProcessors)
								assertEquals(numDevices, report2.HwNumDevices)
								assertEquals(jvmMaxMemory, report2.HwJvmMaxMemory)
								assertEquals(offHeapMaxMemory, report2.HwOffHeapMaxMemory)
								If useJ7 Then
									assertArrayEquals(Nothing, report2.HwDeviceTotalMemory)
									assertArrayEquals(Nothing, report2.HwDeviceDescription)
								Else
									assertArrayEquals(New Long() {0, 0}, report2.HwDeviceTotalMemory) 'Edge case: nDevices = 2, but missing mem data -> expect long[] of 0s out, due to fixed encoding
									assertArrayEquals(New String() {"", ""}, report2.HwDeviceDescription) 'As above
								End If
								assertNullOrZeroLength(report2.HwHardwareUID)
								assertTrue(report2.hasHardwareInfo())
							Else
								assertFalse(report2.hasHardwareInfo())
							End If

							If hasSoftwareInfo Then
								assertNullOrZeroLength(report2.SwArch)
								assertNullOrZeroLength(report2.SwOsName)
								assertNullOrZeroLength(report2.SwJvmName)
								assertNullOrZeroLength(report2.SwJvmVersion)
								assertNullOrZeroLength(report2.SwJvmSpecVersion)
								assertNullOrZeroLength(report2.SwNd4jBackendClass)
								assertNullOrZeroLength(report2.SwNd4jDataTypeName)
								assertNullOrZeroLength(report2.SwJvmUID)
								assertNull(report2.getSwEnvironmentInfo())
								assertTrue(report2.hasSoftwareInfo())
							Else
								assertFalse(report2.hasSoftwareInfo())
							End If

							If hasModelInfo Then
								assertNullOrZeroLength(report2.ModelClassName)
								assertNullOrZeroLength(report2.ModelConfigJson)
								assertNullOrZeroLengthArray(report2.ModelParamNames)
								assertEquals(numLayers, report2.ModelNumLayers)
								assertEquals(numParams, report2.ModelNumParams)
								assertTrue(report2.hasModelInfo())
							Else
								assertFalse(report2.hasModelInfo())
							End If

							'Check standard Java serialization
							Dim baos As New MemoryStream()
							Dim oos As New ObjectOutputStream(baos)
							oos.writeObject(report)
							oos.close()

							Dim javaBytes() As SByte = baos.toByteArray()
							Dim ois As New ObjectInputStream(New MemoryStream(javaBytes))
							Dim report3 As StatsInitializationReport = DirectCast(ois.readObject(), StatsInitializationReport)

							assertEquals(report, report3)
						Next hasModelInfo
					Next hasSoftwareInfo
				Next hasHardwareInfo
			Next useJ7
		End Sub

		Private Shared Sub assertNullOrZeroLength(ByVal str As String)
			assertTrue(str Is Nothing OrElse str.Length = 0)
		End Sub

		Private Shared Sub assertNullOrZeroLengthArray(ByVal str() As String)
			assertTrue(str Is Nothing OrElse str.Length = 0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSbeStatsUpdate() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSbeStatsUpdate()

			Dim paramNames() As String = {"param0", "param1"}

			Dim layerNames() As String = {"layer0", "layer1"}

			'IDs
			Dim sessionID As String = "sid"
			Dim typeID As String = "tid"
			Dim workerID As String = "wid"
			Dim timestamp As Long = -1

			Dim time As Long = DateTimeHelper.CurrentUnixTimeMillis()
			Dim duration As Integer = 123456
			Dim iterCount As Integer = 123

			Dim perfRuntime As Long = 1
			Dim perfTotalEx As Long = 2
			Dim perfTotalMB As Long = 3
			Dim perfEPS As Double = 4.0
			Dim perfMBPS As Double = 5.0

			Dim memJC As Long = 6
			Dim memJM As Long = 7
			Dim memOC As Long = 8
			Dim memOM As Long = 9
			Dim memDC() As Long = {10, 11}
			Dim memDM() As Long = {12, 13}

			Dim gc1Name As String = "14"
			Dim gcdc1 As Integer = 16
			Dim gcdt1 As Integer = 17
			Dim gc2Name As String = "18"
			Dim gcdc2 As Integer = 20
			Dim gcdt2 As Integer = 21

			Dim score As Double = 22.0

			Dim lrByParam As IDictionary(Of String, Double) = New Dictionary(Of String, Double)()
			lrByParam(paramNames(0)) = 22.5
			lrByParam(paramNames(1)) = 22.75

			Dim pHist As IDictionary(Of String, Histogram) = New Dictionary(Of String, Histogram)()
			pHist(paramNames(0)) = New Histogram(23, 24, 2, New Integer() {25, 26})
			pHist(paramNames(1)) = New Histogram(27, 28, 3, New Integer() {29, 30, 31})
			Dim gHist As IDictionary(Of String, Histogram) = New Dictionary(Of String, Histogram)()
			gHist(paramNames(0)) = New Histogram(230, 240, 2, New Integer() {250, 260})
			gHist(paramNames(1)) = New Histogram(270, 280, 3, New Integer() {290, 300, 310})
			Dim uHist As IDictionary(Of String, Histogram) = New Dictionary(Of String, Histogram)()
			uHist(paramNames(0)) = New Histogram(32, 33, 2, New Integer() {34, 35})
			uHist(paramNames(1)) = New Histogram(36, 37, 3, New Integer() {38, 39, 40})
			Dim aHist As IDictionary(Of String, Histogram) = New Dictionary(Of String, Histogram)()
			aHist(layerNames(0)) = New Histogram(41, 42, 2, New Integer() {43, 44})
			aHist(layerNames(1)) = New Histogram(45, 46, 3, New Integer() {47, 48, 47})

			Dim pMean As IDictionary(Of String, Double) = New Dictionary(Of String, Double)()
			pMean(paramNames(0)) = 49.0
			pMean(paramNames(1)) = 50.0
			Dim gMean As IDictionary(Of String, Double) = New Dictionary(Of String, Double)()
			gMean(paramNames(0)) = 49.1
			gMean(paramNames(1)) = 50.1
			Dim uMean As IDictionary(Of String, Double) = New Dictionary(Of String, Double)()
			uMean(paramNames(0)) = 51.0
			uMean(paramNames(1)) = 52.0
			Dim aMean As IDictionary(Of String, Double) = New Dictionary(Of String, Double)()
			aMean(layerNames(0)) = 53.0
			aMean(layerNames(1)) = 54.0

			Dim pStd As IDictionary(Of String, Double) = New Dictionary(Of String, Double)()
			pStd(paramNames(0)) = 55.0
			pStd(paramNames(1)) = 56.0
			Dim gStd As IDictionary(Of String, Double) = New Dictionary(Of String, Double)()
			gStd(paramNames(0)) = 55.1
			gStd(paramNames(1)) = 56.1
			Dim uStd As IDictionary(Of String, Double) = New Dictionary(Of String, Double)()
			uStd(paramNames(0)) = 57.0
			uStd(paramNames(1)) = 58.0
			Dim aStd As IDictionary(Of String, Double) = New Dictionary(Of String, Double)()
			aStd(layerNames(0)) = 59.0
			aStd(layerNames(1)) = 60.0

			Dim pMM As IDictionary(Of String, Double) = New Dictionary(Of String, Double)()
			pMM(paramNames(0)) = 61.0
			pMM(paramNames(1)) = 62.0
			Dim gMM As IDictionary(Of String, Double) = New Dictionary(Of String, Double)()
			gMM(paramNames(0)) = 61.1
			gMM(paramNames(1)) = 62.1
			Dim uMM As IDictionary(Of String, Double) = New Dictionary(Of String, Double)()
			uMM(paramNames(0)) = 63.0
			uMM(paramNames(1)) = 64.0
			Dim aMM As IDictionary(Of String, Double) = New Dictionary(Of String, Double)()
			aMM(layerNames(0)) = 65.0
			aMM(layerNames(1)) = 66.0

			Dim metaDataList As IList(Of Serializable) = New List(Of Serializable)()
			metaDataList.Add("meta1")
			metaDataList.Add("meta2")
			metaDataList.Add("meta3")
			Dim metaDataClass As Type = GetType(String)


			Dim tf() As Boolean = {True, False}

			Dim tf4()() As Boolean = {
				New Boolean() {False, False, False, False},
				New Boolean() {True, False, False, False},
				New Boolean() {False, True, False, False},
				New Boolean() {False, False, True, False},
				New Boolean() {False, False, False, True},
				New Boolean() {True, True, True, True}
			}

			'Total tests: 2^6 x 6^3 = 13,824 separate tests
			Dim testCount As Integer = 0
			For Each collectPerformanceStats As Boolean In tf
				For Each collectMemoryStats As Boolean In tf
					For Each collectGCStats As Boolean In tf
						For Each collectScore As Boolean In tf
							For Each collectLearningRates As Boolean In tf
								For Each collectMetaData As Boolean In tf
									For Each collectHistograms As Boolean() In tf4
										For Each collectMeanStdev As Boolean() In tf4
											For Each collectMM As Boolean() In tf4

												Dim report As New SbeStatsReport()
												report.reportIDs(sessionID, typeID, workerID, time)
												report.reportStatsCollectionDurationMS(duration)
												report.reportIterationCount(iterCount)
												If collectPerformanceStats Then
													report.reportPerformance(perfRuntime, perfTotalEx, perfTotalMB, perfEPS, perfMBPS)
												End If

												If collectMemoryStats Then
													report.reportMemoryUse(memJC, memJM, memOC, memOM, memDC, memDM)
												End If

												If collectGCStats Then
													report.reportGarbageCollection(gc1Name, gcdc1, gcdt1)
													report.reportGarbageCollection(gc2Name, gcdc2, gcdt2)
												End If

												If collectScore Then
													report.reportScore(score)
												End If

												If collectLearningRates Then
													report.reportLearningRates(lrByParam)
												End If

												If collectMetaData Then
													report.reportDataSetMetaData(metaDataList, metaDataClass)
												End If

												If collectHistograms(0) Then 'Param hist
													report.reportHistograms(StatsType.Parameters, pHist)
												End If
												If collectHistograms(1) Then 'Grad hist
													report.reportHistograms(StatsType.Gradients, gHist)
												End If
												If collectHistograms(2) Then 'Update hist
													report.reportHistograms(StatsType.Updates, uHist)
												End If
												If collectHistograms(3) Then 'Act hist
													report.reportHistograms(StatsType.Activations, aHist)
												End If

												If collectMeanStdev(0) Then 'Param mean/stdev
													report.reportMean(StatsType.Parameters, pMean)
													report.reportStdev(StatsType.Parameters, pStd)
												End If
												If collectMeanStdev(1) Then 'Gradient mean/stdev
													report.reportMean(StatsType.Gradients, gMean)
													report.reportStdev(StatsType.Gradients, gStd)
												End If
												If collectMeanStdev(2) Then 'Update mean/stdev
													report.reportMean(StatsType.Updates, uMean)
													report.reportStdev(StatsType.Updates, uStd)
												End If
												If collectMeanStdev(3) Then 'Act mean/stdev
													report.reportMean(StatsType.Activations, aMean)
													report.reportStdev(StatsType.Activations, aStd)
												End If

												If collectMM(0) Then 'Param mean mag
													report.reportMeanMagnitudes(StatsType.Parameters, pMM)
												End If
												If collectMM(1) Then 'Gradient mean mag
													report.reportMeanMagnitudes(StatsType.Gradients, gMM)
												End If
												If collectMM(2) Then 'Update mm
													report.reportMeanMagnitudes(StatsType.Updates, uMM)
												End If
												If collectMM(3) Then 'Act mm
													report.reportMeanMagnitudes(StatsType.Activations, aMM)
												End If

												Dim bytes() As SByte = report.encode()

												Dim report2 As StatsReport = New SbeStatsReport()
												report2.decode(bytes)

												assertEquals(report, report2)

												assertEquals(sessionID, report2.SessionID)
												assertEquals(typeID, report2.TypeID)
												assertEquals(workerID, report2.WorkerID)
												assertEquals(time, report2.TimeStamp)


												assertEquals(time, report2.TimeStamp)
												assertEquals(duration, report2.StatsCollectionDurationMs)
												assertEquals(iterCount, report2.IterationCount)
												If collectPerformanceStats Then
													assertEquals(perfRuntime, report2.TotalRuntimeMs)
													assertEquals(perfTotalEx, report2.TotalExamples)
													assertEquals(perfTotalMB, report2.TotalMinibatches)
													assertEquals(perfEPS, report2.ExamplesPerSecond, 0.0)
													assertEquals(perfMBPS, report2.MinibatchesPerSecond, 0.0)
													assertTrue(report2.hasPerformance())
												Else
													assertFalse(report2.hasPerformance())
												End If

												If collectMemoryStats Then
													assertEquals(memJC, report2.JvmCurrentBytes)
													assertEquals(memJM, report2.JvmMaxBytes)
													assertEquals(memOC, report2.OffHeapCurrentBytes)
													assertEquals(memOM, report2.OffHeapMaxBytes)
													assertArrayEquals(memDC, report2.DeviceCurrentBytes)
													assertArrayEquals(memDM, report2.DeviceMaxBytes)

													assertTrue(report2.hasMemoryUse())
												Else
													assertFalse(report2.hasMemoryUse())
												End If

												If collectGCStats Then
													Dim gcs As IList(Of Pair(Of String, Integer())) = report2.getGarbageCollectionStats()
													assertEquals(2, gcs.Count)
													assertEquals(gc1Name, gcs(0).getFirst())
													assertArrayEquals(New Integer() {gcdc1, gcdt1}, gcs(0).getSecond())
													assertEquals(gc2Name, gcs(1).getFirst())
													assertArrayEquals(New Integer() {gcdc2, gcdt2}, gcs(1).getSecond())
													assertTrue(report2.hasGarbageCollection())
												Else
													assertFalse(report2.hasGarbageCollection())
												End If

												If collectScore Then
													assertEquals(score, report2.Score, 0.0)
													assertTrue(report2.hasScore())
												Else
													assertFalse(report2.hasScore())
												End If

												If collectLearningRates Then
													assertEquals(lrByParam.Keys, report2.getLearningRates().Keys)
													For Each s As String In lrByParam.Keys
														assertEquals(lrByParam(s), report2.getLearningRates()(s), 1e-6)
													Next s
													assertTrue(report2.hasLearningRates())
												Else
													assertFalse(report2.hasLearningRates())
												End If

												If collectMetaData Then
													assertNotNull(report2.getDataSetMetaData())
													assertEquals(metaDataList, report2.getDataSetMetaData())
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
													assertEquals(metaDataClass.FullName, report2.DataSetMetaDataClassName)
													assertTrue(report2.hasDataSetMetaData())
												Else
													assertFalse(report2.hasDataSetMetaData())
												End If

												If collectHistograms(0) Then
													assertEquals(pHist, report2.getHistograms(StatsType.Parameters))
													assertTrue(report2.hasHistograms(StatsType.Parameters))
												Else
													assertFalse(report2.hasHistograms(StatsType.Parameters))
												End If
												If collectHistograms(1) Then
													assertEquals(gHist, report2.getHistograms(StatsType.Gradients))
													assertTrue(report2.hasHistograms(StatsType.Gradients))
												Else
													assertFalse(report2.hasHistograms(StatsType.Gradients))
												End If
												If collectHistograms(2) Then
													assertEquals(uHist, report2.getHistograms(StatsType.Updates))
													assertTrue(report2.hasHistograms(StatsType.Updates))
												Else
													assertFalse(report2.hasHistograms(StatsType.Updates))
												End If
												If collectHistograms(3) Then
													assertEquals(aHist, report2.getHistograms(StatsType.Activations))
													assertTrue(report2.hasHistograms(StatsType.Activations))
												Else
													assertFalse(report2.hasHistograms(StatsType.Activations))
												End If

												If collectMeanStdev(0) Then
													assertEquals(pMean, report2.getMean(StatsType.Parameters))
													assertEquals(pStd, report2.getStdev(StatsType.Parameters))
													assertTrue(report2.hasSummaryStats(StatsType.Parameters, SummaryType.Mean))
													assertTrue(report2.hasSummaryStats(StatsType.Parameters, SummaryType.Stdev))
												Else
													assertFalse(report2.hasSummaryStats(StatsType.Parameters, SummaryType.Mean))
													assertFalse(report2.hasSummaryStats(StatsType.Parameters, SummaryType.Stdev))
												End If
												If collectMeanStdev(1) Then
													assertEquals(gMean, report2.getMean(StatsType.Gradients))
													assertEquals(gStd, report2.getStdev(StatsType.Gradients))
													assertTrue(report2.hasSummaryStats(StatsType.Gradients, SummaryType.Mean))
													assertTrue(report2.hasSummaryStats(StatsType.Gradients, SummaryType.Stdev))
												Else
													assertFalse(report2.hasSummaryStats(StatsType.Gradients, SummaryType.Mean))
													assertFalse(report2.hasSummaryStats(StatsType.Gradients, SummaryType.Stdev))
												End If
												If collectMeanStdev(2) Then
													assertEquals(uMean, report2.getMean(StatsType.Updates))
													assertEquals(uStd, report2.getStdev(StatsType.Updates))
													assertTrue(report2.hasSummaryStats(StatsType.Updates, SummaryType.Mean))
													assertTrue(report2.hasSummaryStats(StatsType.Updates, SummaryType.Stdev))
												Else
													assertFalse(report2.hasSummaryStats(StatsType.Updates, SummaryType.Mean))
													assertFalse(report2.hasSummaryStats(StatsType.Updates, SummaryType.Stdev))
												End If
												If collectMeanStdev(3) Then
													assertEquals(aMean, report2.getMean(StatsType.Activations))
													assertEquals(aStd, report2.getStdev(StatsType.Activations))
													assertTrue(report2.hasSummaryStats(StatsType.Activations, SummaryType.Mean))
													assertTrue(report2.hasSummaryStats(StatsType.Activations, SummaryType.Stdev))
												Else
													assertFalse(report2.hasSummaryStats(StatsType.Activations, SummaryType.Mean))
													assertFalse(report2.hasSummaryStats(StatsType.Activations, SummaryType.Stdev))
												End If

												If collectMM(0) Then
													assertEquals(pMM, report2.getMeanMagnitudes(StatsType.Parameters))
													assertTrue(report2.hasSummaryStats(StatsType.Parameters, SummaryType.MeanMagnitudes))
												Else
													assertFalse(report2.hasSummaryStats(StatsType.Parameters, SummaryType.MeanMagnitudes))
												End If
												If collectMM(1) Then
													assertEquals(gMM, report2.getMeanMagnitudes(StatsType.Gradients))
													assertTrue(report2.hasSummaryStats(StatsType.Gradients, SummaryType.MeanMagnitudes))
												Else
													assertFalse(report2.hasSummaryStats(StatsType.Gradients, SummaryType.MeanMagnitudes))
												End If
												If collectMM(2) Then
													assertEquals(uMM, report2.getMeanMagnitudes(StatsType.Updates))
													assertTrue(report2.hasSummaryStats(StatsType.Updates, SummaryType.MeanMagnitudes))
												Else
													assertFalse(report2.hasSummaryStats(StatsType.Updates, SummaryType.MeanMagnitudes))
												End If
												If collectMM(3) Then
													assertEquals(aMM, report2.getMeanMagnitudes(StatsType.Activations))
													assertTrue(report2.hasSummaryStats(StatsType.Activations, SummaryType.MeanMagnitudes))
												Else
													assertFalse(report2.hasSummaryStats(StatsType.Activations, SummaryType.MeanMagnitudes))
												End If

												'Check standard Java serialization
												Dim baos As New MemoryStream()
												Dim oos As New ObjectOutputStream(baos)
												oos.writeObject(report)
												oos.close()

												Dim javaBytes() As SByte = baos.toByteArray()
												Dim ois As New ObjectInputStream(New MemoryStream(javaBytes))
												Dim report3 As SbeStatsReport = CType(ois.readObject(), SbeStatsReport)

												assertEquals(report, report3)

												testCount += 1
											Next collectMM
										Next collectMeanStdev
									Next collectHistograms
								Next collectMetaData
							Next collectLearningRates
						Next collectScore
					Next collectGCStats
				Next collectMemoryStats
			Next collectPerformanceStats

			assertEquals(13824, testCount)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSbeStatsUpdateNullValues() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSbeStatsUpdateNullValues()

			Dim paramNames() As String = Nothing 'new String[]{"param0", "param1"};

			Dim time As Long = DateTimeHelper.CurrentUnixTimeMillis()
			Dim duration As Integer = 123456
			Dim iterCount As Integer = 123

			Dim perfRuntime As Long = 1
			Dim perfTotalEx As Long = 2
			Dim perfTotalMB As Long = 3
			Dim perfEPS As Double = 4.0
			Dim perfMBPS As Double = 5.0

			Dim memJC As Long = 6
			Dim memJM As Long = 7
			Dim memOC As Long = 8
			Dim memOM As Long = 9
			Dim memDC() As Long = Nothing
			Dim memDM() As Long = Nothing

			Dim gc1Name As String = Nothing
			Dim gcdc1 As Integer = 16
			Dim gcdt1 As Integer = 17
			Dim gc2Name As String = Nothing
			Dim gcdc2 As Integer = 20
			Dim gcdt2 As Integer = 21

			Dim score As Double = 22.0

			Dim lrByParam As IDictionary(Of String, Double) = Nothing

			Dim pHist As IDictionary(Of String, Histogram) = Nothing
			Dim gHist As IDictionary(Of String, Histogram) = Nothing
			Dim uHist As IDictionary(Of String, Histogram) = Nothing
			Dim aHist As IDictionary(Of String, Histogram) = Nothing

			Dim pMean As IDictionary(Of String, Double) = Nothing
			Dim gMean As IDictionary(Of String, Double) = Nothing
			Dim uMean As IDictionary(Of String, Double) = Nothing
			Dim aMean As IDictionary(Of String, Double) = Nothing

			Dim pStd As IDictionary(Of String, Double) = Nothing
			Dim gStd As IDictionary(Of String, Double) = Nothing
			Dim uStd As IDictionary(Of String, Double) = Nothing
			Dim aStd As IDictionary(Of String, Double) = Nothing

			Dim pMM As IDictionary(Of String, Double) = Nothing
			Dim gMM As IDictionary(Of String, Double) = Nothing
			Dim uMM As IDictionary(Of String, Double) = Nothing
			Dim aMM As IDictionary(Of String, Double) = Nothing


			Dim tf() As Boolean = {True, False}
			Dim tf4()() As Boolean = {
				New Boolean() {False, False, False, False},
				New Boolean() {True, False, False, False},
				New Boolean() {False, True, False, False},
				New Boolean() {False, False, True, False},
				New Boolean() {False, False, False, True},
				New Boolean() {True, True, True, True}
			}

			'Total tests: 2^6 x 6^3 = 13,824 separate tests
			Dim testCount As Integer = 0
			For Each collectPerformanceStats As Boolean In tf
				For Each collectMemoryStats As Boolean In tf
					For Each collectGCStats As Boolean In tf
						For Each collectDataSetMetaData As Boolean In tf
							For Each collectScore As Boolean In tf
								For Each collectLearningRates As Boolean In tf
									For Each collectHistograms As Boolean() In tf4
										For Each collectMeanStdev As Boolean() In tf4
											For Each collectMM As Boolean() In tf4

												Dim report As New SbeStatsReport()
												report.reportIDs(Nothing, Nothing, Nothing, time)
												report.reportStatsCollectionDurationMS(duration)
												report.reportIterationCount(iterCount)
												If collectPerformanceStats Then
													report.reportPerformance(perfRuntime, perfTotalEx, perfTotalMB, perfEPS, perfMBPS)
												End If

												If collectMemoryStats Then
													report.reportMemoryUse(memJC, memJM, memOC, memOM, memDC, memDM)
												End If

												If collectGCStats Then
													report.reportGarbageCollection(gc1Name, gcdc1, gcdt1)
													report.reportGarbageCollection(gc2Name, gcdc2, gcdt2)
												End If

												If collectDataSetMetaData Then
													'TODO
												End If

												If collectScore Then
													report.reportScore(score)
												End If

												If collectLearningRates Then
													report.reportLearningRates(lrByParam)
												End If

												If collectHistograms(0) Then 'Param hist
													report.reportHistograms(StatsType.Parameters, pHist)
												End If
												If collectHistograms(1) Then
													report.reportHistograms(StatsType.Gradients, gHist)
												End If
												If collectHistograms(2) Then 'Update hist
													report.reportHistograms(StatsType.Updates, uHist)
												End If
												If collectHistograms(3) Then 'Act hist
													report.reportHistograms(StatsType.Activations, aHist)
												End If

												If collectMeanStdev(0) Then 'Param mean/stdev
													report.reportMean(StatsType.Parameters, pMean)
													report.reportStdev(StatsType.Parameters, pStd)
												End If
												If collectMeanStdev(1) Then 'Param mean/stdev
													report.reportMean(StatsType.Gradients, gMean)
													report.reportStdev(StatsType.Gradients, gStd)
												End If
												If collectMeanStdev(2) Then 'Update mean/stdev
													report.reportMean(StatsType.Updates, uMean)
													report.reportStdev(StatsType.Updates, uStd)
												End If
												If collectMeanStdev(3) Then 'Act mean/stdev
													report.reportMean(StatsType.Activations, aMean)
													report.reportStdev(StatsType.Activations, aStd)
												End If

												If collectMM(0) Then 'Param mean mag
													report.reportMeanMagnitudes(StatsType.Parameters, pMM)
												End If
												If collectMM(1) Then 'Param mean mag
													report.reportMeanMagnitudes(StatsType.Gradients, gMM)
												End If
												If collectMM(2) Then 'Update mm
													report.reportMeanMagnitudes(StatsType.Updates, uMM)
												End If
												If collectMM(3) Then 'Act mm
													report.reportMeanMagnitudes(StatsType.Activations, aMM)
												End If

												Dim bytes() As SByte = report.encode()

												Dim report2 As StatsReport = New SbeStatsReport()
												report2.decode(bytes)

												assertEquals(time, report2.TimeStamp)
												assertEquals(duration, report2.StatsCollectionDurationMs)
												assertEquals(iterCount, report2.IterationCount)
												If collectPerformanceStats Then
													assertEquals(perfRuntime, report2.TotalRuntimeMs)
													assertEquals(perfTotalEx, report2.TotalExamples)
													assertEquals(perfTotalMB, report2.TotalMinibatches)
													assertEquals(perfEPS, report2.ExamplesPerSecond, 0.0)
													assertEquals(perfMBPS, report2.MinibatchesPerSecond, 0.0)
													assertTrue(report2.hasPerformance())
												Else
													assertFalse(report2.hasPerformance())
												End If

												If collectMemoryStats Then
													assertEquals(memJC, report2.JvmCurrentBytes)
													assertEquals(memJM, report2.JvmMaxBytes)
													assertEquals(memOC, report2.OffHeapCurrentBytes)
													assertEquals(memOM, report2.OffHeapMaxBytes)
													assertArrayEquals(memDC, report2.DeviceCurrentBytes)
													assertArrayEquals(memDM, report2.DeviceMaxBytes)

													assertTrue(report2.hasMemoryUse())
												Else
													assertFalse(report2.hasMemoryUse())
												End If

												If collectGCStats Then
													Dim gcs As IList(Of Pair(Of String, Integer())) = report2.getGarbageCollectionStats()
													assertEquals(2, gcs.Count)
													assertNullOrZeroLength(gcs(0).getFirst())
													assertArrayEquals(New Integer() {gcdc1, gcdt1}, gcs(0).getSecond())
													assertNullOrZeroLength(gcs(1).getFirst())
													assertArrayEquals(New Integer() {gcdc2, gcdt2}, gcs(1).getSecond())
													assertTrue(report2.hasGarbageCollection())
												Else
													assertFalse(report2.hasGarbageCollection())
												End If

												If collectDataSetMetaData Then
													'TODO
												End If

												If collectScore Then
													assertEquals(score, report2.Score, 0.0)
													assertTrue(report2.hasScore())
												Else
													assertFalse(report2.hasScore())
												End If

												If collectLearningRates Then
													assertNull(report2.getLearningRates())
												Else
													assertFalse(report2.hasLearningRates())
												End If

												assertNull(report2.getHistograms(StatsType.Parameters))
												assertFalse(report2.hasHistograms(StatsType.Parameters))

												assertNull(report2.getHistograms(StatsType.Gradients))
												assertFalse(report2.hasHistograms(StatsType.Gradients))

												assertNull(report2.getHistograms(StatsType.Updates))
												assertFalse(report2.hasHistograms(StatsType.Updates))

												assertNull(report2.getHistograms(StatsType.Activations))
												assertFalse(report2.hasHistograms(StatsType.Activations))

												assertNull(report2.getMean(StatsType.Parameters))
												assertNull(report2.getStdev(StatsType.Parameters))
												assertFalse(report2.hasSummaryStats(StatsType.Parameters, SummaryType.Mean))
												assertFalse(report2.hasSummaryStats(StatsType.Parameters, SummaryType.Stdev))

												assertNull(report2.getMean(StatsType.Gradients))
												assertNull(report2.getStdev(StatsType.Gradients))
												assertFalse(report2.hasSummaryStats(StatsType.Gradients, SummaryType.Mean))
												assertFalse(report2.hasSummaryStats(StatsType.Gradients, SummaryType.Stdev))

												assertNull(report2.getMean(StatsType.Updates))
												assertNull(report2.getStdev(StatsType.Updates))
												assertFalse(report2.hasSummaryStats(StatsType.Updates, SummaryType.Mean))
												assertFalse(report2.hasSummaryStats(StatsType.Updates, SummaryType.Stdev))

												assertNull(report2.getMean(StatsType.Activations))
												assertNull(report2.getStdev(StatsType.Activations))
												assertFalse(report2.hasSummaryStats(StatsType.Activations, SummaryType.Mean))
												assertFalse(report2.hasSummaryStats(StatsType.Activations, SummaryType.Stdev))

												assertNull(report2.getMeanMagnitudes(StatsType.Parameters))
												assertFalse(report2.hasSummaryStats(StatsType.Parameters, SummaryType.MeanMagnitudes))

												assertNull(report2.getMeanMagnitudes(StatsType.Gradients))
												assertFalse(report2.hasSummaryStats(StatsType.Gradients, SummaryType.MeanMagnitudes))

												assertNull(report2.getMeanMagnitudes(StatsType.Updates))
												assertFalse(report2.hasSummaryStats(StatsType.Updates, SummaryType.MeanMagnitudes))

												assertNull(report2.getMeanMagnitudes(StatsType.Activations))
												assertFalse(report2.hasSummaryStats(StatsType.Activations, SummaryType.MeanMagnitudes))

												'Check standard Java serialization
												Dim baos As New MemoryStream()
												Dim oos As New ObjectOutputStream(baos)
												oos.writeObject(report)
												oos.close()

												Dim javaBytes() As SByte = baos.toByteArray()
												Dim ois As New ObjectInputStream(New MemoryStream(javaBytes))
												Dim report3 As SbeStatsReport = CType(ois.readObject(), SbeStatsReport)

												assertEquals(report, report3)

												testCount += 1
											Next collectMM
										Next collectMeanStdev
									Next collectHistograms
								Next collectLearningRates
							Next collectScore
						Next collectDataSetMetaData
					Next collectGCStats
				Next collectMemoryStats
			Next collectPerformanceStats

			assertEquals(13824, testCount)
		End Sub

	End Class

End Namespace