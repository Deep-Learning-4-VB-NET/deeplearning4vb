Imports System
Imports System.Collections.Generic
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports Hdf5Archive = org.deeplearning4j.nn.modelimport.keras.Hdf5Archive
Imports KerasModelImport = org.deeplearning4j.nn.modelimport.keras.KerasModelImport
Imports InvalidKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
Imports UnsupportedKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
Imports KerasModelUtils = org.deeplearning4j.nn.modelimport.keras.utils.KerasModelUtils
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports ThresholdAlgorithm = org.deeplearning4j.optimize.solvers.accumulation.encoding.ThresholdAlgorithm
Imports FixedThresholdAlgorithm = org.deeplearning4j.optimize.solvers.accumulation.encoding.threshold.FixedThresholdAlgorithm
Imports RDDTrainingApproach = org.deeplearning4j.spark.api.RDDTrainingApproach
Imports Repartition = org.deeplearning4j.spark.api.Repartition
Imports RepartitionStrategy = org.deeplearning4j.spark.api.RepartitionStrategy
Imports org.deeplearning4j.spark.api
Imports SparkComputationGraph = org.deeplearning4j.spark.impl.graph.SparkComputationGraph
Imports SparkDl4jMultiLayer = org.deeplearning4j.spark.impl.multilayer.SparkDl4jMultiLayer
Imports ParameterAveragingTrainingMaster = org.deeplearning4j.spark.impl.paramavg.ParameterAveragingTrainingMaster
Imports DefaultRepartitioner = org.deeplearning4j.spark.impl.repartitioner.DefaultRepartitioner
Imports SharedTrainingMaster = org.deeplearning4j.spark.parameterserver.training.SharedTrainingMaster
Imports VoidConfiguration = org.nd4j.parameterserver.distributed.conf.VoidConfiguration

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

Namespace org.deeplearning4j.spark.parameterserver.modelimport.elephas


	''' <summary>
	''' Reads HDF5-persisted Elephas models stored with `model.save()` for both underlying
	''' `Sequential` and `Model` Keras models
	''' 
	''' @author Max Pumperla
	''' 
	''' </summary>
	Public Class ElephasModelImport

		Private Const DISTRIBUTED_CONFIG As String = "distributed_config"
		Private Const APPROACH As RDDTrainingApproach = RDDTrainingApproach.Export

		''' <summary>
		''' Load Elephas model stored using model.save(...) in case that the underlying Keras
		''' model is a functional `Model` instance, which corresponds to a DL4J SparkComputationGraph.
		''' </summary>
		''' <param name="sparkContext">                            Java SparkContext </param>
		''' <param name="modelHdf5Filename">                       Path to HDF5 archive storing Elephas Model </param>
		''' <returns> SparkComputationGraph                  Spark computation graph
		''' </returns>
		''' <exception cref="IOException">                            IO exception </exception>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
		''' <seealso cref= SparkComputationGraph </seealso>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.deeplearning4j.spark.impl.graph.SparkComputationGraph importElephasModelAndWeights(org.apache.spark.api.java.JavaSparkContext sparkContext, String modelHdf5Filename) throws IOException, UnsupportedKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Shared Function importElephasModelAndWeights(ByVal sparkContext As JavaSparkContext, ByVal modelHdf5Filename As String) As SparkComputationGraph
			Dim model As ComputationGraph = KerasModelImport.importKerasModelAndWeights(modelHdf5Filename, True)

			Dim distributedProperties As IDictionary(Of String, Object) = distributedTrainingMap(modelHdf5Filename)
			Dim tm As TrainingMaster = getTrainingMaster(distributedProperties)

			Return New SparkComputationGraph(sparkContext, model, tm)
		End Function

		''' <summary>
		''' Load Elephas model stored using model.save(...) in case that the underlying Keras
		''' model is a functional `Sequential` instance, which corresponds to a DL4J SparkDl4jMultiLayer.
		''' </summary>
		''' <param name="sparkContext">                            Java SparkContext </param>
		''' <param name="modelHdf5Filename">                       Path to HDF5 archive storing Elephas model </param>
		''' <returns> SparkDl4jMultiLayer                    Spark computation graph
		''' </returns>
		''' <exception cref="IOException">                            IO exception </exception>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
		''' <seealso cref= SparkDl4jMultiLayer </seealso>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.deeplearning4j.spark.impl.multilayer.SparkDl4jMultiLayer importElephasSequentialModelAndWeights(org.apache.spark.api.java.JavaSparkContext sparkContext, String modelHdf5Filename) throws IOException, UnsupportedKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Shared Function importElephasSequentialModelAndWeights(ByVal sparkContext As JavaSparkContext, ByVal modelHdf5Filename As String) As SparkDl4jMultiLayer
			Dim model As MultiLayerNetwork = KerasModelImport.importKerasSequentialModelAndWeights(modelHdf5Filename, True)

			Dim distributedProperties As IDictionary(Of String, Object) = distributedTrainingMap(modelHdf5Filename)
			Dim tm As TrainingMaster = getTrainingMaster(distributedProperties)

			Return New SparkDl4jMultiLayer(sparkContext, model, tm)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static java.util.Map<String, Object> distributedTrainingMap(String modelHdf5Filename) throws UnsupportedKerasConfigurationException, java.io.IOException
		Private Shared Function distributedTrainingMap(ByVal modelHdf5Filename As String) As IDictionary(Of String, Object)
			Dim archive As New Hdf5Archive(modelHdf5Filename)
			Dim initialModelJson As String = archive.readAttributeAsJson(DISTRIBUTED_CONFIG)
			Return KerasModelUtils.parseJsonString(initialModelJson)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static org.deeplearning4j.spark.api.TrainingMaster getTrainingMaster(java.util.Map<String, Object> distributedProperties) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Private Shared Function getTrainingMaster(ByVal distributedProperties As IDictionary(Of String, Object)) As TrainingMaster
			Dim innerConfig As System.Collections.IDictionary = DirectCast(distributedProperties("config"), System.Collections.IDictionary)

			Dim numWorkers As Integer? = CType(innerConfig("num_workers"), Integer?)
			Dim batchSize As Integer = CInt(Math.Truncate(innerConfig("batch_size")))

			Dim mode As String = "synchronous"
			If innerConfig.Contains("mode") Then
				mode = CStr(innerConfig("mode"))
			Else
				Throw New InvalidKerasConfigurationException("Couldn't find mode field.")
			End If

			' TODO: Create InvalidElephasConfigurationException
			Dim collectStats As Boolean = False
			If innerConfig.Contains("collect_stats") Then
				collectStats = CBool(innerConfig("collect_stats"))
			End If

			Dim numBatchesPrefetch As Integer = 0
			If innerConfig.Contains("num_batches_prefetch") Then
				numBatchesPrefetch = CInt(Math.Truncate(innerConfig("num_batches_prefetch")))
			End If


		Dim tm As TrainingMaster
			If mode.Equals("synchronous") Then
				Dim averagingFrequency As Integer = 5
				If innerConfig.Contains("averaging_frequency") Then
					averagingFrequency = CInt(Math.Truncate(innerConfig("averaging_frequency")))
				End If

				tm = (New ParameterAveragingTrainingMaster.Builder(numWorkers, batchSize)).collectTrainingStats(collectStats).batchSizePerWorker(batchSize).averagingFrequency(averagingFrequency).workerPrefetchNumBatches(numBatchesPrefetch).aggregationDepth(2).repartionData(Repartition.Always).rddTrainingApproach(APPROACH).repartitionStrategy(RepartitionStrategy.Balanced).saveUpdater(False).build()
			ElseIf mode.Equals("asynchronous") Then
				Dim updateThreshold As Double = 1e-3
				If innerConfig.Contains("update_threshold") Then
					updateThreshold = CDbl(innerConfig("update_threshold"))
				End If
				Dim thresholdAlgorithm As ThresholdAlgorithm = New FixedThresholdAlgorithm(updateThreshold)

				Dim voidConfiguration As VoidConfiguration = VoidConfiguration.builder().build()
				tm = (New SharedTrainingMaster.Builder(voidConfiguration, batchSize)).thresholdAlgorithm(thresholdAlgorithm).batchSizePerWorker(batchSize).collectTrainingStats(collectStats).workerPrefetchNumBatches(numBatchesPrefetch).rddTrainingApproach(APPROACH).repartitioner(New DefaultRepartitioner()).build()
			Else
				Throw New InvalidKerasConfigurationException("Unknown mode " & mode)
			End If
			Return tm
		End Function
	End Class

End Namespace