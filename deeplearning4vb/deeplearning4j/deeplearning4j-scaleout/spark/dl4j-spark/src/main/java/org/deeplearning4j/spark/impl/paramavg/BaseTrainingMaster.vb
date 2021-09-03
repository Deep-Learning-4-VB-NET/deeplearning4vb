Imports System
Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Configuration = org.apache.hadoop.conf.Configuration
Imports FileSystem = org.apache.hadoop.fs.FileSystem
Imports Path = org.apache.hadoop.fs.Path
Imports SparkContext = org.apache.spark.SparkContext
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports StorageLevel = org.apache.spark.storage.StorageLevel
Imports SerializableHadoopConfig = org.datavec.spark.util.SerializableHadoopConfig
Imports StatsStorageRouter = org.deeplearning4j.core.storage.StatsStorageRouter
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports org.deeplearning4j.spark.api
Imports BatchAndExportDataSetsFunction = org.deeplearning4j.spark.data.BatchAndExportDataSetsFunction
Imports BatchAndExportMultiDataSetsFunction = org.deeplearning4j.spark.data.BatchAndExportMultiDataSetsFunction
Imports ParameterAveragingTrainingMasterStats = org.deeplearning4j.spark.impl.paramavg.stats.ParameterAveragingTrainingMasterStats
Imports ExportSupport = org.deeplearning4j.spark.impl.paramavg.util.ExportSupport
Imports StorageLevelDeserializer = org.deeplearning4j.spark.util.serde.StorageLevelDeserializer
Imports StorageLevelSerializer = org.deeplearning4j.spark.util.serde.StorageLevelSerializer
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports JsonAutoDetect = org.nd4j.shade.jackson.annotation.JsonAutoDetect
Imports PropertyAccessor = org.nd4j.shade.jackson.annotation.PropertyAccessor
Imports JsonFactory = org.nd4j.shade.jackson.core.JsonFactory
Imports DeserializationFeature = org.nd4j.shade.jackson.databind.DeserializationFeature
Imports MapperFeature = org.nd4j.shade.jackson.databind.MapperFeature
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper
Imports SerializationFeature = org.nd4j.shade.jackson.databind.SerializationFeature
Imports JsonDeserialize = org.nd4j.shade.jackson.databind.annotation.JsonDeserialize
Imports JsonSerialize = org.nd4j.shade.jackson.databind.annotation.JsonSerialize
Imports YAMLFactory = org.nd4j.shade.jackson.dataformat.yaml.YAMLFactory

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

Namespace org.deeplearning4j.spark.impl.paramavg


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public abstract class BaseTrainingMaster<R extends TrainingResult, W extends TrainingWorker<R>> implements TrainingMaster<R, W>
	Public MustInherit Class BaseTrainingMaster(Of R As TrainingResult, W As TrainingWorker(Of R))
		Implements TrainingMaster(Of R, W)

		Public MustOverride Sub setListeners(ByVal router As StatsStorageRouter, ByVal listeners As ICollection(Of TrainingListener))
		Public MustOverride WriteOnly Property Listeners As ICollection(Of TrainingListener)
		Public MustOverride ReadOnly Property TrainingStats As org.deeplearning4j.spark.api.stats.SparkTrainingStats Implements TrainingMaster(Of R, W).getTrainingStats
		Public MustOverride ReadOnly Property IsCollectTrainingStats As Boolean Implements TrainingMaster(Of R, W).getIsCollectTrainingStats
		Public MustOverride WriteOnly Property CollectTrainingStats As Boolean
		Public MustOverride Sub executeTrainingMDS(ByVal graph As org.deeplearning4j.spark.impl.graph.SparkComputationGraph, ByVal trainingData As JavaRDD(Of MultiDataSet)) Implements TrainingMaster(Of R, W).executeTrainingMDS
		Public MustOverride Sub executeTraining(ByVal graph As org.deeplearning4j.spark.impl.graph.SparkComputationGraph, ByVal trainingData As JavaRDD(Of DataSet)) Implements TrainingMaster(Of R, W).executeTraining
		Public MustOverride Sub executeTrainingPaths(ByVal network As org.deeplearning4j.spark.impl.multilayer.SparkDl4jMultiLayer, ByVal graph As org.deeplearning4j.spark.impl.graph.SparkComputationGraph, ByVal trainingDataPaths As JavaRDD(Of String), ByVal dsLoader As org.deeplearning4j.core.loader.DataSetLoader, ByVal mdsLoader As org.deeplearning4j.core.loader.MultiDataSetLoader) Implements TrainingMaster(Of R, W).executeTrainingPaths
		Public MustOverride Sub executeTraining(ByVal network As org.deeplearning4j.spark.impl.multilayer.SparkDl4jMultiLayer, ByVal trainingData As JavaRDD(Of DataSet)) Implements TrainingMaster(Of R, W).executeTraining
		Public MustOverride Function getWorkerInstance(ByVal graph As org.deeplearning4j.spark.impl.graph.SparkComputationGraph) As TrainingWorker(Of R)
		Public MustOverride Function getWorkerInstance(ByVal network As org.deeplearning4j.spark.impl.multilayer.SparkDl4jMultiLayer) As TrainingWorker(Of R)
		Public MustOverride Function toYaml() As String Implements TrainingMaster(Of R, W).toYaml
		Public MustOverride Function toJson() As String Implements TrainingMaster(Of R, W).toJson
		Public MustOverride Sub addHook(ByVal trainingHook As org.deeplearning4j.spark.api.TrainingHook)
		Public MustOverride Sub removeHook(ByVal trainingHook As org.deeplearning4j.spark.api.TrainingHook)
'JAVA TO VB CONVERTER NOTE: The field jsonMapper was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend Shared jsonMapper_Conflict As ObjectMapper
'JAVA TO VB CONVERTER NOTE: The field yamlMapper was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend Shared yamlMapper_Conflict As ObjectMapper

		Protected Friend collectTrainingStats As Boolean
		Protected Friend stats As ParameterAveragingTrainingMasterStats.ParameterAveragingTrainingMasterStatsHelper

		Protected Friend lastExportedRDDId As Integer = Integer.MinValue
		Protected Friend lastRDDExportPath As String
		Protected Friend batchSizePerWorker As Integer
		Protected Friend exportDirectory As String = Nothing
		Protected Friend rng As Random

		Protected Friend trainingMasterUID As String

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter @Getter protected System.Nullable<Boolean> workerTogglePeriodicGC;
		Protected Friend workerTogglePeriodicGC As Boolean?
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter @Getter protected System.Nullable<Integer> workerPeriodicGCFrequency;
		Protected Friend workerPeriodicGCFrequency As Integer?
		Protected Friend statsStorage As StatsStorageRouter

		'Listeners etc
		Protected Friend listeners As IList(Of TrainingListener)


		Protected Friend repartition As Repartition
		Protected Friend repartitionStrategy As RepartitionStrategy
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonSerialize(using = org.deeplearning4j.spark.util.serde.StorageLevelSerializer.class) @JsonDeserialize(using = org.deeplearning4j.spark.util.serde.StorageLevelDeserializer.class) protected org.apache.spark.storage.StorageLevel storageLevel;
		Protected Friend storageLevel As StorageLevel
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonSerialize(using = org.deeplearning4j.spark.util.serde.StorageLevelSerializer.class) @JsonDeserialize(using = org.deeplearning4j.spark.util.serde.StorageLevelDeserializer.class) protected org.apache.spark.storage.StorageLevel storageLevelStreams = org.apache.spark.storage.StorageLevel.MEMORY_ONLY();
		Protected Friend storageLevelStreams As StorageLevel = StorageLevel.MEMORY_ONLY()
		Protected Friend rddTrainingApproach As RDDTrainingApproach = RDDTrainingApproach.Export

		Protected Friend broadcastHadoopConfig As Broadcast(Of SerializableHadoopConfig)

		Protected Friend Sub New()

		End Sub


		Protected Friend Shared ReadOnly Property JsonMapper As ObjectMapper
			Get
				SyncLock GetType(BaseTrainingMaster)
					If jsonMapper_Conflict Is Nothing Then
						jsonMapper_Conflict = getNewMapper(New JsonFactory())
					End If
					Return jsonMapper_Conflict
				End SyncLock
			End Get
		End Property

		Protected Friend Shared ReadOnly Property YamlMapper As ObjectMapper
			Get
				SyncLock GetType(BaseTrainingMaster)
					If yamlMapper_Conflict Is Nothing Then
						yamlMapper_Conflict = getNewMapper(New YAMLFactory())
					End If
					Return yamlMapper_Conflict
				End SyncLock
			End Get
		End Property

		Protected Friend Shared Function getNewMapper(ByVal jsonFactory As JsonFactory) As ObjectMapper
			Dim om As New ObjectMapper(jsonFactory)
			om.configure(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES, False)
			om.configure(SerializationFeature.FAIL_ON_EMPTY_BEANS, False)
			om.configure(MapperFeature.SORT_PROPERTIES_ALPHABETICALLY, True)
			om.enable(SerializationFeature.INDENT_OUTPUT)
			om.setVisibility(PropertyAccessor.ALL, JsonAutoDetect.Visibility.NONE)
			om.setVisibility(PropertyAccessor.FIELD, JsonAutoDetect.Visibility.ANY)
			Return om
		End Function



		Protected Friend Overridable Function exportIfRequired(ByVal sc As JavaSparkContext, ByVal trainingData As JavaRDD(Of DataSet)) As JavaRDD(Of String)
			ExportSupport.assertExportSupported(sc)
			If collectTrainingStats Then
				stats.logExportStart()
			End If

			'Two possibilities here:
			' 1. We've seen this RDD before (i.e., multiple epochs training case)
			' 2. We have not seen this RDD before
			'    (a) And we haven't got any stored data -> simply export
			'    (b) And we previously exported some data from a different RDD -> delete the last data
			Dim currentRDDUid As Integer = trainingData.id() 'Id is a "A unique ID for this RDD (within its SparkContext)."

			Dim baseDir As String
			If lastExportedRDDId = Integer.MinValue Then
				'Haven't seen a RDD<DataSet> yet in this training master -> export data
				baseDir = export(trainingData)
			Else
				If lastExportedRDDId = currentRDDUid Then
					'Use the already-exported data again for another epoch
					baseDir = getBaseDirForRDD(trainingData)
				Else
					'The new RDD is different to the last one
					' Clean up the data for the last one, and export
					deleteTempDir(sc, lastRDDExportPath)
					baseDir = export(trainingData)
				End If
			End If

			If collectTrainingStats Then
				stats.logExportEnd()
			End If

			Return sc.textFile(baseDir & "paths/")
		End Function

		Protected Friend Overridable Function exportIfRequiredMDS(ByVal sc As JavaSparkContext, ByVal trainingData As JavaRDD(Of MultiDataSet)) As JavaRDD(Of String)
			ExportSupport.assertExportSupported(sc)
			If collectTrainingStats Then
				stats.logExportStart()
			End If

			'Two possibilities here:
			' 1. We've seen this RDD before (i.e., multiple epochs training case)
			' 2. We have not seen this RDD before
			'    (a) And we haven't got any stored data -> simply export
			'    (b) And we previously exported some data from a different RDD -> delete the last data
			Dim currentRDDUid As Integer = trainingData.id() 'Id is a "A unique ID for this RDD (within its SparkContext)."

			Dim baseDir As String
			If lastExportedRDDId = Integer.MinValue Then
				'Haven't seen a RDD<DataSet> yet in this training master -> export data
				baseDir = exportMDS(trainingData)
			Else
				If lastExportedRDDId = currentRDDUid Then
					'Use the already-exported data again for another epoch
					baseDir = getBaseDirForRDD(trainingData)
				Else
					'The new RDD is different to the last one
					' Clean up the data for the last one, and export
					deleteTempDir(sc, lastRDDExportPath)
					baseDir = exportMDS(trainingData)
				End If
			End If

			If collectTrainingStats Then
				stats.logExportEnd()
			End If

			Return sc.textFile(baseDir & "paths/")
		End Function

		Protected Friend Overridable Function export(ByVal trainingData As JavaRDD(Of DataSet)) As String
			Dim baseDir As String = getBaseDirForRDD(trainingData)
			Dim dataDir As String = baseDir & "data/"
			Dim pathsDir As String = baseDir & "paths/"

			log.info("Initiating RDD<DataSet> export at {}", baseDir)
			Dim paths As JavaRDD(Of String) = trainingData.mapPartitionsWithIndex(New BatchAndExportDataSetsFunction(batchSizePerWorker, dataDir), True)
			paths.saveAsTextFile(pathsDir)
			log.info("RDD<DataSet> export complete at {}", baseDir)

			lastExportedRDDId = trainingData.id()
			lastRDDExportPath = baseDir
			Return baseDir
		End Function

		Protected Friend Overridable Function exportMDS(ByVal trainingData As JavaRDD(Of MultiDataSet)) As String
			Dim baseDir As String = getBaseDirForRDD(trainingData)
			Dim dataDir As String = baseDir & "data/"
			Dim pathsDir As String = baseDir & "paths/"

			log.info("Initiating RDD<MultiDataSet> export at {}", baseDir)
			Dim paths As JavaRDD(Of String) = trainingData.mapPartitionsWithIndex(New BatchAndExportMultiDataSetsFunction(batchSizePerWorker, dataDir), True)
			paths.saveAsTextFile(pathsDir)
			log.info("RDD<MultiDataSet> export complete at {}", baseDir)

			lastExportedRDDId = trainingData.id()
			lastRDDExportPath = baseDir
			Return baseDir
		End Function

		Protected Friend Overridable Function getBaseDirForRDD(Of T1)(ByVal rdd As JavaRDD(Of T1)) As String
			If exportDirectory Is Nothing Then
				exportDirectory = getDefaultExportDirectory(rdd.context())
			End If

			Return exportDirectory + (If(exportDirectory.EndsWith("/", StringComparison.Ordinal), "", "/")) & trainingMasterUID & "/" & rdd.id() & "/"
		End Function

		Protected Friend Overridable Function deleteTempDir(ByVal sc As JavaSparkContext, ByVal tempDirPath As String) As Boolean
			log.info("Attempting to delete temporary directory: {}", tempDirPath)

			Dim hadoopConfiguration As Configuration = sc.hadoopConfiguration()
			Dim fileSystem As FileSystem
			Try
				fileSystem = FileSystem.get(New URI(tempDirPath), hadoopConfiguration)
			Catch e As Exception When TypeOf e Is URISyntaxException OrElse TypeOf e Is IOException
				Throw New Exception(e)
			End Try

			Try
				fileSystem.delete(New Path(tempDirPath), True)
				log.info("Deleted temporary directory: {}", tempDirPath)
				Return True
			Catch e As IOException
				log.warn("Could not delete temporary directory: {}", tempDirPath, e)
				Return False
			End Try
		End Function

		Protected Friend Overridable Function getDefaultExportDirectory(ByVal sc As SparkContext) As String
			Dim hadoopTmpDir As String = sc.hadoopConfiguration().get("hadoop.tmp.dir")
			If Not hadoopTmpDir.EndsWith("/", StringComparison.Ordinal) AndAlso Not hadoopTmpDir.EndsWith("\", StringComparison.Ordinal) Then
				hadoopTmpDir = hadoopTmpDir & "/"
			End If
			Return hadoopTmpDir & "dl4j/"
		End Function


		Public Overridable Function deleteTempFiles(ByVal sc As JavaSparkContext) As Boolean Implements TrainingMaster(Of R, W).deleteTempFiles
			Return lastRDDExportPath Is Nothing OrElse deleteTempDir(sc, lastRDDExportPath)
		End Function

		Public Overridable Function deleteTempFiles(ByVal sc As SparkContext) As Boolean Implements TrainingMaster(Of R, W).deleteTempFiles
			Return deleteTempFiles(New JavaSparkContext(sc))
		End Function
	End Class

End Namespace