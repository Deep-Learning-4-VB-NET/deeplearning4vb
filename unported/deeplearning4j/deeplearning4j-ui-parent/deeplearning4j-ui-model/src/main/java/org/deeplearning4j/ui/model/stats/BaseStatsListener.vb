Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Threading
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports IOUtils = org.apache.commons.io.IOUtils
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports DL4JClassLoading = org.deeplearning4j.common.config.DL4JClassLoading
Imports StatsStorageRouter = org.deeplearning4j.core.storage.StatsStorageRouter
Imports StorageMetaData = org.deeplearning4j.core.storage.StorageMetaData
Imports RoutingIterationListener = org.deeplearning4j.core.storage.listener.RoutingIterationListener
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports Model = org.deeplearning4j.nn.api.Model
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports org.deeplearning4j.ui.model.stats.api
Imports FileStatsStorage = org.deeplearning4j.ui.model.storage.FileStatsStorage
Imports InMemoryStatsStorage = org.deeplearning4j.ui.model.storage.InMemoryStatsStorage
Imports DefaultStatsInitializationConfiguration = org.deeplearning4j.ui.model.stats.impl.DefaultStatsInitializationConfiguration
Imports DefaultStatsUpdateConfiguration = org.deeplearning4j.ui.model.stats.impl.DefaultStatsUpdateConfiguration
Imports UIDProvider = org.deeplearning4j.core.util.UIDProvider
Imports DataTypeUtil = org.nd4j.linalg.api.buffer.util.DataTypeUtil
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives
Imports NativeOps = org.nd4j.nativeblas.NativeOps
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder

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

Namespace org.deeplearning4j.ui.model.stats


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public abstract class BaseStatsListener implements org.deeplearning4j.core.storage.listener.RoutingIterationListener
	<Serializable>
	Public MustInherit Class BaseStatsListener
		Implements RoutingIterationListener

		Public Const TYPE_ID As String = "StatsListener"

		Private Enum StatType
			Mean
			Stdev
			MeanMagnitude
		End Enum

		Private router As StatsStorageRouter
'JAVA TO VB CONVERTER NOTE: The field initConfig was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly initConfig_Conflict As StatsInitializationConfiguration
'JAVA TO VB CONVERTER NOTE: The field updateConfig was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private updateConfig_Conflict As StatsUpdateConfiguration
'JAVA TO VB CONVERTER NOTE: The field sessionID was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private sessionID_Conflict As String
'JAVA TO VB CONVERTER NOTE: The field workerID was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private workerID_Conflict As String

		<NonSerialized>
		Private gcBeans As IList(Of GarbageCollectorMXBean)
		Private gcStatsAtLastReport As IDictionary(Of String, Pair(Of Long, Long))

		'NOTE: may have multiple models, due to multiple pretrain layers all using the same StatsListener
		Private modelInfos As IList(Of ModelInfo) = New List(Of ModelInfo)()

		Private activationHistograms As IDictionary(Of String, Histogram)
		Private meanActivations As IDictionary(Of String, Double) 'TODO replace with Eclipse collections primitive maps...
		Private stdevActivations As IDictionary(Of String, Double)
		Private meanMagActivations As IDictionary(Of String, Double)

		Private gradientHistograms As IDictionary(Of String, Histogram)
		Private meanGradients As IDictionary(Of String, Double) 'TODO replace with Eclipse collections primitive maps...
		Private stdevGradient As IDictionary(Of String, Double)
		Private meanMagGradients As IDictionary(Of String, Double)

		<Serializable>
		Private Class ModelInfo
			Friend ReadOnly model As Model
			Friend initTime As Long
			Friend lastReportTime As Long = -1
			Friend lastReportIteration As Integer = -1
			Friend examplesSinceLastReport As Integer = 0
			Friend minibatchesSinceLastReport As Integer = 0

			Friend totalExamples As Long = 0
			Friend totalMinibatches As Long = 0

			Friend iterCount As Integer = 0

			Friend Sub New(ByVal model As Model)
				Me.model = model
			End Sub
		End Class

		Private Function getModelInfo(ByVal model As Model) As ModelInfo
			Dim mi As ModelInfo = Nothing
			For Each m As ModelInfo In modelInfos
				If m.model Is model Then
					mi = m
					Exit For
				End If
			Next m
			If mi Is Nothing Then
				mi = New ModelInfo(model)
				modelInfos.Add(mi)
			End If
			Return mi
		End Function

		''' <summary>
		''' Create a StatsListener with network information collected at every iteration.
		''' </summary>
		''' <param name="router"> Where/how to store the calculated stats. For example, <seealso cref="InMemoryStatsStorage"/> or
		'''               <seealso cref="FileStatsStorage"/> </param>
		Public Sub New(ByVal router As StatsStorageRouter)
			Me.New(router, Nothing, Nothing, Nothing, Nothing)
		End Sub

		''' <summary>
		''' Create a StatsListener with network information collected every n >= 1 time steps
		''' </summary>
		''' <param name="router">            Where/how to store the calculated stats. For example, <seealso cref="InMemoryStatsStorage"/> or
		'''                          <seealso cref="FileStatsStorage"/> </param>
		''' <param name="listenerFrequency"> Frequency with which to collect stats information </param>
		Public Sub New(ByVal router As StatsStorageRouter, ByVal listenerFrequency As Integer)
			Me.New(router, Nothing, (New DefaultStatsUpdateConfiguration.Builder()).reportingFrequency(listenerFrequency).build(), Nothing, Nothing)
		End Sub

		Public Sub New(ByVal router As StatsStorageRouter, ByVal initConfig As StatsInitializationConfiguration, ByVal updateConfig As StatsUpdateConfiguration, ByVal sessionID As String, ByVal workerID As String)
			Me.router = router
			If initConfig Is Nothing Then
				Me.initConfig_Conflict = New DefaultStatsInitializationConfiguration(True, True, True)
			Else
				Me.initConfig_Conflict = initConfig
			End If
			If updateConfig Is Nothing Then
				Me.updateConfig_Conflict = (New DefaultStatsUpdateConfiguration.Builder()).build()
			Else
				Me.updateConfig_Conflict = updateConfig
			End If
			If sessionID Is Nothing Then
				'TODO handle syncing session IDs across different listeners in the same model...
				Me.sessionID_Conflict = System.Guid.randomUUID().ToString()
			Else
				Me.sessionID_Conflict = sessionID
			End If
			If workerID Is Nothing Then
				Me.workerID_Conflict = UIDProvider.JVMUID & "_" & Thread.CurrentThread.getId()
			Else
				Me.workerID_Conflict = workerID
			End If
		End Sub

		Public MustOverride ReadOnly Property NewInitializationReport As StatsInitializationReport

		Public MustOverride ReadOnly Property NewStatsReport As StatsReport

		'    public abstract StorageMetaData getNewStorageMetaData();
		Public MustOverride Function getNewStorageMetaData(ByVal initTime As Long, ByVal sessionID As String, ByVal workerID As String) As StorageMetaData
		'                                                          Class<? extends StatsInitializationReport> initializationReportClass,
		'                                                          Class<? extends StatsReport> statsReportClass);
		'new SbeStorageMetaData(initTime, getSessionID(model), TYPE_ID, workerID, SbeStatsInitializationReport.class, SbeStatsReport.class);


		Public Overridable ReadOnly Property InitConfig As StatsInitializationConfiguration
			Get
				Return initConfig_Conflict
			End Get
		End Property

		Public Overridable Property UpdateConfig As StatsUpdateConfiguration
			Get
				Return updateConfig_Conflict
			End Get
			Set(ByVal newConfig As StatsUpdateConfiguration)
				Me.updateConfig_Conflict = newConfig
			End Set
		End Property


		Public Overridable Property StorageRouter Implements RoutingIterationListener.setStorageRouter As StatsStorageRouter
			Set(ByVal router As StatsStorageRouter)
				Me.router = router
			End Set
			Get
				Return router
			End Get
		End Property


		Public Overridable Property WorkerID Implements RoutingIterationListener.setWorkerID As String
			Set(ByVal workerID As String)
				Me.workerID_Conflict = workerID
			End Set
			Get
				Return workerID_Conflict
			End Get
		End Property


		Public Overridable Property SessionID Implements RoutingIterationListener.setSessionID As String
			Set(ByVal sessionID As String)
				Me.sessionID_Conflict = sessionID
			End Set
			Get
				Return sessionID_Conflict
			End Get
		End Property


		Private Function getSessionID(ByVal model As Model) As String
			If TypeOf model Is MultiLayerNetwork OrElse TypeOf model Is ComputationGraph Then
				Return sessionID_Conflict
			End If
			If TypeOf model Is Layer Then
				'Keep in mind MultiLayerNetwork implements Layer also...
				Dim l As Layer = DirectCast(model, Layer)
				Dim layerIdx As Integer = l.Index
				Return sessionID_Conflict & "_layer" & layerIdx
			End If
			Return sessionID_Conflict 'Should never happen
		End Function

		Public Overridable Sub onEpochStart(ByVal model As Model)

		End Sub

		Public Overridable Sub onEpochEnd(ByVal model As Model)

		End Sub

		Public Overridable Sub onForwardPass(ByVal model As Model, ByVal activations As IList(Of INDArray))
			Dim iterCount As Integer = getModelInfo(model).iterCount
			If calcFromActivations() AndAlso (iterCount = 0 OrElse iterCount Mod updateConfig_Conflict.reportingFrequency() = 0) Then
				'Assumption: we have input, layer 0, layer 1, ...
				Dim activationsMap As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
				Dim count As Integer = 0
				For Each arr As INDArray In activations
					Dim layerName As String = (If(count = 0, "input", (count - 1).ToString()))
					activationsMap(layerName) = arr
					count += 1
				Next arr
				onForwardPass(model, activationsMap)
			End If
		End Sub

		Public Overridable Sub onForwardPass(ByVal model As Model, ByVal activations As IDictionary(Of String, INDArray))
			Dim iterCount As Integer = getModelInfo(model).iterCount
			If calcFromActivations() AndAlso updateConfig_Conflict.reportingFrequency() > 0 AndAlso (iterCount = 0 OrElse iterCount Mod updateConfig_Conflict.reportingFrequency() = 0) Then
				If updateConfig_Conflict.collectHistograms(StatsType.Activations) Then
					activationHistograms = getHistograms(activations, updateConfig_Conflict.numHistogramBins(StatsType.Activations))
				End If
				If updateConfig_Conflict.collectMean(StatsType.Activations) Then
					meanActivations = calculateSummaryStats(activations, StatType.Mean)
				End If
				If updateConfig_Conflict.collectStdev(StatsType.Activations) Then
					stdevActivations = calculateSummaryStats(activations, StatType.Stdev)
				End If
				If updateConfig_Conflict.collectMeanMagnitudes(StatsType.Activations) Then
					meanMagActivations = calculateSummaryStats(activations, StatType.MeanMagnitude)
				End If
			End If
		End Sub

		Public Overridable Sub onGradientCalculation(ByVal model As Model)
			Dim iterCount As Integer = getModelInfo(model).iterCount
			If calcFromGradients() AndAlso updateConfig_Conflict.reportingFrequency() > 0 AndAlso (iterCount = 0 OrElse iterCount Mod updateConfig_Conflict.reportingFrequency() = 0) Then
				Dim g As Gradient = model.gradient()
				If updateConfig_Conflict.collectHistograms(StatsType.Gradients) Then
					gradientHistograms = getHistograms(g.gradientForVariable(), updateConfig_Conflict.numHistogramBins(StatsType.Gradients))
				End If

				If updateConfig_Conflict.collectMean(StatsType.Gradients) Then
					meanGradients = calculateSummaryStats(g.gradientForVariable(), StatType.Mean)
				End If
				If updateConfig_Conflict.collectStdev(StatsType.Gradients) Then
					stdevGradient = calculateSummaryStats(g.gradientForVariable(), StatType.Stdev)
				End If
				If updateConfig_Conflict.collectMeanMagnitudes(StatsType.Gradients) Then
					meanMagGradients = calculateSummaryStats(g.gradientForVariable(), StatType.MeanMagnitude)
				End If
			End If
		End Sub

		Private Function calcFromActivations() As Boolean
			Return updateConfig_Conflict.collectMean(StatsType.Activations) OrElse updateConfig_Conflict.collectStdev(StatsType.Activations) OrElse updateConfig_Conflict.collectMeanMagnitudes(StatsType.Activations) OrElse updateConfig_Conflict.collectHistograms(StatsType.Activations)
		End Function

		Private Function calcFromGradients() As Boolean
			Return updateConfig_Conflict.collectMean(StatsType.Gradients) OrElse updateConfig_Conflict.collectStdev(StatsType.Gradients) OrElse updateConfig_Conflict.collectMeanMagnitudes(StatsType.Gradients) OrElse updateConfig_Conflict.collectHistograms(StatsType.Gradients)
		End Function

		Public Overridable Sub onBackwardPass(ByVal model As Model)
			'No op
		End Sub

		Public Overridable Sub iterationDone(ByVal model As Model, ByVal iteration As Integer, ByVal epoch As Integer)

			Dim modelInfo As ModelInfo = getModelInfo(model)
			Dim backpropParamsOnly As Boolean = Me.backpropParamsOnly(model)

			Dim currentTime As Long = Time
			If modelInfo.iterCount = 0 Then
				modelInfo.initTime = currentTime
				doInit(model)
			End If

			If updateConfig_Conflict.collectPerformanceStats() Then
				updateExamplesMinibatchesCounts(model)
			End If

			If updateConfig_Conflict.reportingFrequency() > 1 AndAlso (iteration = 0 OrElse iteration Mod updateConfig_Conflict.reportingFrequency() <> 0) Then
				modelInfo.iterCount = iteration
				Return
			End If

			Dim report As StatsReport = NewStatsReport
			report.reportIDs(getSessionID(model), TYPE_ID, workerID_Conflict, DateTimeHelper.CurrentUnixTimeMillis()) 'TODO support NTP time

			'--- Performance and System Stats ---
			If updateConfig_Conflict.collectPerformanceStats() Then
				'Stats to collect: total runtime, total examples, total minibatches, iterations/second, examples/second
				Dim examplesPerSecond As Double
				Dim minibatchesPerSecond As Double
				If modelInfo.iterCount = 0 Then
					'Not possible to work out perf/second: first iteration...
					examplesPerSecond = 0.0
					minibatchesPerSecond = 0.0
				Else
					Dim deltaTimeMS As Long = currentTime - modelInfo.lastReportTime
					examplesPerSecond = 1000.0 * modelInfo.examplesSinceLastReport / deltaTimeMS
					minibatchesPerSecond = 1000.0 * modelInfo.minibatchesSinceLastReport / deltaTimeMS
				End If
				Dim totalRuntimeMS As Long = currentTime - modelInfo.initTime
				report.reportPerformance(totalRuntimeMS, modelInfo.totalExamples, modelInfo.totalMinibatches, examplesPerSecond, minibatchesPerSecond)

				modelInfo.examplesSinceLastReport = 0
				modelInfo.minibatchesSinceLastReport = 0
			End If

			If updateConfig_Conflict.collectMemoryStats() Then

				Dim runtime As Runtime = Runtime.getRuntime()
				Dim jvmTotal As Long = runtime.totalMemory()
				Dim jvmMax As Long = runtime.maxMemory()

				'Off-heap memory
				Dim offheapTotal As Long = Pointer.totalBytes()
				Dim offheapMax As Long = Pointer.maxBytes()

				'GPU
				Dim gpuCurrentBytes() As Long = Nothing
				Dim gpuMaxBytes() As Long = Nothing
				Dim nativeOps As NativeOps = NativeOpsHolder.Instance.getDeviceNativeOps()
				Dim nDevices As Integer = nativeOps.AvailableDevices
				If nDevices > 0 Then
					gpuCurrentBytes = New Long(nDevices - 1){}
					gpuMaxBytes = New Long(nDevices - 1){}
					For i As Integer = 0 To nDevices - 1
						Try
							gpuMaxBytes(i) = nativeOps.getDeviceTotalMemory(0)
							gpuCurrentBytes(i) = gpuMaxBytes(i) - nativeOps.getDeviceFreeMemory(0)
						Catch e As Exception
							log.error("",e)
						End Try
					Next i
				End If

				report.reportMemoryUse(jvmTotal, jvmMax, offheapTotal, offheapMax, gpuCurrentBytes, gpuMaxBytes)
			End If

			If updateConfig_Conflict.collectGarbageCollectionStats() Then
				If modelInfo.lastReportIteration = -1 OrElse gcBeans Is Nothing Then
					'Haven't reported GC stats before...
					gcBeans = ManagementFactory.getGarbageCollectorMXBeans()
					gcStatsAtLastReport = New Dictionary(Of String, Pair(Of Long, Long))()
					For Each bean As GarbageCollectorMXBean In gcBeans
						Dim count As Long = bean.getCollectionCount()
						Dim timeMs As Long = bean.getCollectionTime()
						gcStatsAtLastReport(bean.getName()) = New Pair(Of Long, Long)(count, timeMs)
					Next bean
				Else
					For Each bean As GarbageCollectorMXBean In gcBeans
						Dim count As Long = bean.getCollectionCount()
						Dim timeMs As Long = bean.getCollectionTime()
						Dim lastStats As Pair(Of Long, Long) = gcStatsAtLastReport(bean.getName())
						Dim deltaGCCount As Long = count - lastStats.First
						Dim deltaGCTime As Long = timeMs - lastStats.Second

						lastStats.First = count
						lastStats.Second = timeMs
						report.reportGarbageCollection(bean.getName(), CInt(deltaGCCount), CInt(deltaGCTime))
					Next bean
				End If
			End If

			'--- General ---
			report.reportScore(model.score()) 'Always report score

			If updateConfig_Conflict.collectLearningRates() Then
				Dim lrs As IDictionary(Of String, Double) = New Dictionary(Of String, Double)()
				If TypeOf model Is MultiLayerNetwork Then
					'Need to append "0_", "1_" etc to param names from layers...
					Dim layerIdx As Integer = 0
					For Each l As Layer In (DirectCast(model, MultiLayerNetwork)).Layers
						Dim conf As NeuralNetConfiguration = l.conf()
						Dim paramkeys As IList(Of String) = l.conf().getLayer().initializer().paramKeys(l.conf().getLayer())
						For Each s As String In paramkeys
							Dim lr As Double = conf.getLayer().getUpdaterByParam(s).getLearningRate(l.IterationCount, l.EpochCount)
							If Double.IsNaN(lr) Then
								'Edge case: No-Op updater, AdaDelta etc - don't have a LR hence return NaN for IUpdater.getLearningRate
								lr = 0.0
							End If
							lrs(layerIdx & "_" & s) = lr
						Next s
						layerIdx += 1
					Next l
				ElseIf TypeOf model Is ComputationGraph Then
					For Each l As Layer In (DirectCast(model, ComputationGraph)).Layers
						Dim conf As NeuralNetConfiguration = l.conf()
						Dim layerName As String = conf.getLayer().getLayerName()
						Dim paramkeys As IList(Of String) = l.conf().getLayer().initializer().paramKeys(l.conf().getLayer())
						For Each s As String In paramkeys
							Dim lr As Double = conf.getLayer().getUpdaterByParam(s).getLearningRate(l.IterationCount, l.EpochCount)
							If Double.IsNaN(lr) Then
								'Edge case: No-Op updater, AdaDelta etc - don't have a LR hence return NaN for IUpdater.getLearningRate
								lr = 0.0
							End If
							lrs(layerName & "_" & s) = lr
						Next s
					Next l
				ElseIf TypeOf model Is Layer Then
					Dim l As Layer = DirectCast(model, Layer)
					Dim paramkeys As IList(Of String) = l.conf().getLayer().initializer().paramKeys(l.conf().getLayer())
					For Each s As String In paramkeys
						Dim lr As Double = l.conf().getLayer().getUpdaterByParam(s).getLearningRate(l.IterationCount, l.EpochCount)
						lrs(s) = lr
					Next s
				End If
				report.reportLearningRates(lrs)
			End If


			'--- Histograms ---

			If updateConfig_Conflict.collectHistograms(StatsType.Parameters) Then
				Dim paramHistograms As IDictionary(Of String, Histogram) = getHistograms(model.paramTable(backpropParamsOnly), updateConfig_Conflict.numHistogramBins(StatsType.Parameters))
				report.reportHistograms(StatsType.Parameters, paramHistograms)
			End If

			If updateConfig_Conflict.collectHistograms(StatsType.Gradients) Then
				report.reportHistograms(StatsType.Gradients, gradientHistograms)
			End If

			If updateConfig_Conflict.collectHistograms(StatsType.Updates) Then
				Dim updateHistograms As IDictionary(Of String, Histogram) = getHistograms(model.gradient().gradientForVariable(), updateConfig_Conflict.numHistogramBins(StatsType.Updates))
				report.reportHistograms(StatsType.Updates, updateHistograms)
			End If

			If updateConfig_Conflict.collectHistograms(StatsType.Activations) Then
				report.reportHistograms(StatsType.Activations, activationHistograms)
			End If


			'--- Summary Stats: Mean, Variance, Mean Magnitudes ---

			If updateConfig_Conflict.collectMean(StatsType.Parameters) Then
				Dim meanParams As IDictionary(Of String, Double) = calculateSummaryStats(model.paramTable(backpropParamsOnly), StatType.Mean)
				report.reportMean(StatsType.Parameters, meanParams)
			End If

			If updateConfig_Conflict.collectMean(StatsType.Gradients) Then
				report.reportMean(StatsType.Gradients, meanGradients)
			End If

			If updateConfig_Conflict.collectMean(StatsType.Updates) Then
				Dim meanUpdates As IDictionary(Of String, Double) = calculateSummaryStats(model.gradient().gradientForVariable(), StatType.Mean)
				report.reportMean(StatsType.Updates, meanUpdates)
			End If

			If updateConfig_Conflict.collectMean(StatsType.Activations) Then
				report.reportMean(StatsType.Activations, meanActivations)
			End If


			If updateConfig_Conflict.collectStdev(StatsType.Parameters) Then
				Dim stdevParams As IDictionary(Of String, Double) = calculateSummaryStats(model.paramTable(backpropParamsOnly), StatType.Stdev)
				report.reportStdev(StatsType.Parameters, stdevParams)
			End If

			If updateConfig_Conflict.collectStdev(StatsType.Gradients) Then
				report.reportStdev(StatsType.Gradients, stdevGradient)
			End If

			If updateConfig_Conflict.collectStdev(StatsType.Updates) Then
				Dim stdevUpdates As IDictionary(Of String, Double) = calculateSummaryStats(model.gradient().gradientForVariable(), StatType.Stdev)
				report.reportStdev(StatsType.Updates, stdevUpdates)
			End If

			If updateConfig_Conflict.collectStdev(StatsType.Activations) Then
				report.reportStdev(StatsType.Activations, stdevActivations)
			End If


			If updateConfig_Conflict.collectMeanMagnitudes(StatsType.Parameters) Then
				Dim meanMagParams As IDictionary(Of String, Double) = calculateSummaryStats(model.paramTable(backpropParamsOnly), StatType.MeanMagnitude)
				report.reportMeanMagnitudes(StatsType.Parameters, meanMagParams)
			End If

			If updateConfig_Conflict.collectMeanMagnitudes(StatsType.Gradients) Then
				report.reportMeanMagnitudes(StatsType.Gradients, meanMagGradients)
			End If

			If updateConfig_Conflict.collectMeanMagnitudes(StatsType.Updates) Then
				Dim meanMagUpdates As IDictionary(Of String, Double) = calculateSummaryStats(model.gradient().gradientForVariable(), StatType.MeanMagnitude)
				report.reportMeanMagnitudes(StatsType.Updates, meanMagUpdates)
			End If

			If updateConfig_Conflict.collectMeanMagnitudes(StatsType.Activations) Then
				report.reportMeanMagnitudes(StatsType.Activations, meanMagActivations)
			End If


			Dim endTime As Long = Time
			report.reportStatsCollectionDurationMS(CInt(endTime - currentTime)) 'Amount of time required to alculate all histograms, means etc.
			modelInfo.lastReportTime = currentTime
			modelInfo.lastReportIteration = iteration
			report.reportIterationCount(iteration)

			Me.router.putUpdate(report)

			modelInfo.iterCount = iteration
			activationHistograms = Nothing
			meanActivations = Nothing
			stdevActivations = Nothing
			meanMagActivations = Nothing
			gradientHistograms = Nothing
			meanGradients = Nothing
			stdevGradient = Nothing
			meanMagGradients = Nothing
		End Sub

		Private ReadOnly Property Time As Long
			Get
				'Abstraction to allow NTP to be plugged in later...
				Return DateTimeHelper.CurrentUnixTimeMillis()
			End Get
		End Property

		Private Sub doInit(ByVal model As Model)
			Dim backpropParamsOnly As Boolean = Me.backpropParamsOnly(model)
			Dim initTime As Long = DateTimeHelper.CurrentUnixTimeMillis() 'TODO support NTP
			Dim initReport As StatsInitializationReport = NewInitializationReport
			initReport.reportIDs(getSessionID(model), TYPE_ID, workerID_Conflict, initTime)

			If initConfig_Conflict.collectSoftwareInfo() Then
				Dim osBean As OperatingSystemMXBean = ManagementFactory.getOperatingSystemMXBean()
				Dim runtime As RuntimeMXBean = ManagementFactory.getRuntimeMXBean()

				Dim arch As String = osBean.getArch()
				Dim osName As String = osBean.getName()
				Dim jvmName As String = runtime.getVmName()
				Dim jvmVersion As String = System.getProperty("java.version")
				Dim jvmSpecVersion As String = runtime.getSpecVersion()

'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Dim nd4jBackendClass As String = Nd4j.NDArrayFactory.GetType().FullName
				Dim nd4jDataTypeName As String = DataTypeUtil.DtypeFromContext.name()

				Dim hostname As String = Environment.GetEnvironmentVariable("COMPUTERNAME")
				If hostname Is Nothing OrElse hostname.Length = 0 Then
					Try
						Dim proc As Process = Runtime.getRuntime().exec("hostname")
						Using stream As Stream = proc.getInputStream()
							hostname = IOUtils.toString(stream)
						End Using
					Catch e As Exception
					End Try
				End If

				Dim p As Properties = Nd4j.Executioner.EnvironmentInformation
				Dim envInfo As IDictionary(Of String, String) = New Dictionary(Of String, String)()
				For Each e As KeyValuePair(Of Object, Object) In p.entrySet()
					Dim v As Object = e.Value
					Dim value As String = (If(v Is Nothing, "", v.ToString()))
					envInfo(e.Key.ToString()) = value
				Next e

				initReport.reportSoftwareInfo(arch, osName, jvmName, jvmVersion, jvmSpecVersion, nd4jBackendClass, nd4jDataTypeName, hostname, UIDProvider.JVMUID, envInfo)
			End If

			If initConfig_Conflict.collectHardwareInfo() Then
				Dim availableProcessors As Integer = Runtime.getRuntime().availableProcessors()
				Dim nativeOps As NativeOps = NativeOpsHolder.Instance.getDeviceNativeOps()
				Dim nDevices As Integer = nativeOps.AvailableDevices

				Dim deviceTotalMem() As Long = Nothing
				Dim deviceDescription() As String = Nothing 'TODO
				If nDevices > 0 Then
					deviceTotalMem = New Long(nDevices - 1){}
					deviceDescription = New String(nDevices - 1){}
					For i As Integer = 0 To nDevices - 1
						Try
							deviceTotalMem(i) = nativeOps.getDeviceTotalMemory(i)
							deviceDescription(i) = nativeOps.getDeviceName(i)
							If nDevices > 1 Then
								deviceDescription(i) = deviceDescription(i) & " (" & i & ")"
							End If
						Catch e As Exception
							log.debug("Error getting device info", e)
						End Try
					Next i
				End If
				Dim jvmMaxMemory As Long = Runtime.getRuntime().maxMemory()
				Dim offheapMaxMemory As Long = Pointer.maxBytes()

				initReport.reportHardwareInfo(availableProcessors, nDevices, jvmMaxMemory, offheapMaxMemory, deviceTotalMem, deviceDescription, UIDProvider.HardwareUID)
			End If

			If initConfig_Conflict.collectModelInfo() Then
				Dim jsonConf As String
				Dim numLayers As Integer
				Dim numParams As Long
				If TypeOf model Is MultiLayerNetwork Then
					Dim net As MultiLayerNetwork = (DirectCast(model, MultiLayerNetwork))
					jsonConf = net.LayerWiseConfigurations.toJson()
					numLayers = net.getnLayers()
					numParams = net.numParams()
				ElseIf TypeOf model Is ComputationGraph Then
					Dim cg As ComputationGraph = (DirectCast(model, ComputationGraph))
					jsonConf = cg.Configuration.toJson()
					numLayers = cg.NumLayers
					numParams = cg.numParams()
				ElseIf TypeOf model Is Layer Then
					Dim l As Layer = DirectCast(model, Layer)
					jsonConf = l.conf().toJson()
					numLayers = 1
					numParams = l.numParams()
				Else
					Throw New Exception("Invalid model: Expected MultiLayerNetwork or ComputationGraph. Got: " & (If(model Is Nothing, Nothing, model.GetType())))
				End If

				Dim paramMap As IDictionary(Of String, INDArray) = model.paramTable(backpropParamsOnly)
				Dim paramNames(paramMap.Count - 1) As String
				Dim i As Integer = 0
				For Each s As String In paramMap.Keys 'Assuming sensible iteration order - LinkedHashMaps are used in MLN/CG for example
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: paramNames[i++] = s;
					paramNames(i) = s
						i += 1
				Next s

'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				initReport.reportModelInfo(model.GetType().FullName, jsonConf, paramNames, numLayers, numParams)
			End If

			Dim meta As StorageMetaData = getNewStorageMetaData(initTime, getSessionID(model), workerID_Conflict)

			router.putStorageMetaData(meta)
			router.putStaticInfo(initReport) 'TODO error handling
		End Sub

		Private devPointers As IDictionary(Of Integer, Pointer) = New Dictionary(Of Integer, Pointer)()

		Private Function getDevicePointer(ByVal device As Integer) As Pointer
			SyncLock Me
				If devPointers.ContainsKey(device) Then
					Return devPointers(device)
				End If
				Try
					Dim pointer As Pointer = DL4JClassLoading.createNewInstance("org.nd4j.jita.allocator.pointers.CudaPointer", GetType(Pointer), New Type() { GetType(Long) }, New Object(){CLng(device)})
        
					devPointers(device) = pointer
					Return pointer
				Catch t As Exception
					devPointers(device) = Nothing 'Stops attempting the failure again later...
					Return Nothing
				End Try
			End SyncLock
		End Function

		Private Sub updateExamplesMinibatchesCounts(ByVal model As Model)
			Dim modelInfo As ModelInfo = getModelInfo(model)
			Dim examplesThisMinibatch As Integer = 0
			If TypeOf model Is MultiLayerNetwork Then
				examplesThisMinibatch = model.batchSize()
			ElseIf TypeOf model Is ComputationGraph Then
				examplesThisMinibatch = model.batchSize()
			ElseIf TypeOf model Is Layer Then
				examplesThisMinibatch = DirectCast(model, Layer).InputMiniBatchSize
			End If
			modelInfo.examplesSinceLastReport += examplesThisMinibatch
			modelInfo.totalExamples += examplesThisMinibatch
			modelInfo.minibatchesSinceLastReport += 1
			modelInfo.totalMinibatches += 1
		End Sub

		Private Function backpropParamsOnly(ByVal model As Model) As Boolean
			'For pretrain layers (VAE, AE) we *do* want pretrain params also; for MLN and CG we only want backprop params
			' as we only have backprop gradients
			Return TypeOf model Is MultiLayerNetwork OrElse TypeOf model Is ComputationGraph
		End Function

		Private Shared Function calculateSummaryStats(ByVal source As IDictionary(Of String, INDArray), ByVal statType As StatType) As IDictionary(Of String, Double)
			Dim [out] As IDictionary(Of String, Double) = New LinkedHashMap(Of String, Double)()

			If source Is Nothing Then
				Return [out]
			End If

			For Each entry As KeyValuePair(Of String, INDArray) In source.SetOfKeyValuePairs()
				Dim name As String = entry.Key
				Dim value As Double
				Select Case statType
					Case org.deeplearning4j.ui.model.stats.BaseStatsListener.StatType.Mean
						value = entry.Value.meanNumber().doubleValue()
					Case org.deeplearning4j.ui.model.stats.BaseStatsListener.StatType.Stdev
						value = entry.Value.stdNumber().doubleValue()
					Case org.deeplearning4j.ui.model.stats.BaseStatsListener.StatType.MeanMagnitude
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
						value = entry.Value.norm1Number().doubleValue() / entry.Value.length()
					Case Else
						Throw New Exception() 'Should never happen
				End Select
				[out](name) = value
			Next entry
			Return [out]
		End Function

		Private Shared Function getHistograms(ByVal map As IDictionary(Of String, INDArray), ByVal nBins As Integer) As IDictionary(Of String, Histogram)
			Dim [out] As IDictionary(Of String, Histogram) = New LinkedHashMap(Of String, Histogram)()

			If map Is Nothing Then
				Return [out]
			End If

			For Each entry As KeyValuePair(Of String, INDArray) In map.SetOfKeyValuePairs()

				Dim hOp As New org.nd4j.linalg.api.ops.impl.transforms.Histogram(entry.Value, nBins)
				Nd4j.exec(hOp)

				Dim bins As INDArray = hOp.getOutputArgument(0)
				Dim count(nBins - 1) As Integer
				For i As Integer = 0 To bins.length() - 1
					count(i) = CInt(Math.Truncate(bins.getDouble(i)))
				Next i

				Dim min As Double = entry.Value.minNumber().doubleValue()
				Dim max As Double = entry.Value.maxNumber().doubleValue()

				Dim h As New Histogram(min, max, nBins, count)

				[out](entry.Key) = h
			Next entry
			Return [out]
		End Function

		Public MustOverride Overrides Function clone() As BaseStatsListener
	End Class

End Namespace