Imports System.Collections.Generic
Imports SparkContext = org.apache.spark.SparkContext
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports DataSetLoader = org.deeplearning4j.core.loader.DataSetLoader
Imports MultiDataSetLoader = org.deeplearning4j.core.loader.MultiDataSetLoader
Imports StatsStorageRouter = org.deeplearning4j.core.storage.StatsStorageRouter
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports SparkTrainingStats = org.deeplearning4j.spark.api.stats.SparkTrainingStats
Imports SparkComputationGraph = org.deeplearning4j.spark.impl.graph.SparkComputationGraph
Imports SparkDl4jMultiLayer = org.deeplearning4j.spark.impl.multilayer.SparkDl4jMultiLayer
Imports DataSet = org.nd4j.linalg.dataset.DataSet
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

Namespace org.deeplearning4j.spark.api


	Public Interface TrainingMaster(Of R As TrainingResult, W As TrainingWorker(Of R))


		''' <summary>
		''' Remove a training hook from the worker </summary>
		''' <param name="trainingHook"> the training hook to remove </param>
		Sub removeHook(ByVal trainingHook As TrainingHook)

		''' <summary>
		''' Add a hook for the master for pre and post training </summary>
		''' <param name="trainingHook"> the training hook to add </param>
		Sub addHook(ByVal trainingHook As TrainingHook)

		''' <summary>
		''' Get the TrainingMaster configuration as JSON
		''' </summary>
		Function toJson() As String

		''' <summary>
		''' Get the TrainingMaster configuration as YAML
		''' </summary>
		Function toYaml() As String

		''' <summary>
		''' Get the worker instance for this training master
		''' </summary>
		''' <param name="network"> Current SparkDl4jMultiLayer </param>
		''' <returns> Worker instance </returns>
		Function getWorkerInstance(ByVal network As SparkDl4jMultiLayer) As W

		''' <summary>
		''' Get the worker instance for this training master
		''' </summary>
		''' <param name="graph"> Current SparkComputationGraph </param>
		''' <returns> Worker instance </returns>
		Function getWorkerInstance(ByVal graph As SparkComputationGraph) As W

		''' <summary>
		''' Train the SparkDl4jMultiLayer with the specified data set
		''' </summary>
		''' <param name="network">      Current network state </param>
		''' <param name="trainingData"> Data to train on </param>
		Sub executeTraining(ByVal network As SparkDl4jMultiLayer, ByVal trainingData As JavaRDD(Of DataSet))


		''' <summary>
		''' Fit the network using a list of paths for serialized DataSet objects.
		''' </summary>
		''' <param name="network">           Current network state </param>
		''' <param name="trainingDataPaths"> Data to train on </param>
		Sub executeTrainingPaths(ByVal network As SparkDl4jMultiLayer, ByVal graph As SparkComputationGraph, ByVal trainingDataPaths As JavaRDD(Of String), ByVal dsLoader As DataSetLoader, ByVal mdsLoader As MultiDataSetLoader)

		''' <summary>
		''' Train the SparkComputationGraph with the specified data set
		''' </summary>
		''' <param name="graph">        Current network state </param>
		''' <param name="trainingData"> Data to train on </param>
		Sub executeTraining(ByVal graph As SparkComputationGraph, ByVal trainingData As JavaRDD(Of DataSet))

		''' <summary>
		''' Train the SparkComputationGraph with the specified data set
		''' </summary>
		''' <param name="graph">        Current network state </param>
		''' <param name="trainingData"> Data to train on </param>
		Sub executeTrainingMDS(ByVal graph As SparkComputationGraph, ByVal trainingData As JavaRDD(Of MultiDataSet))

		''' <summary>
		''' Set whether the training statistics should be collected. Training statistics may include things like per-epoch run times,
		''' time spent waiting for data, etc.
		''' <para>
		''' These statistics are primarily used for debugging and optimization, in order to gain some insight into what aspects
		''' of network training are taking the most time.
		''' 
		''' </para>
		''' </summary>
		''' <param name="collectTrainingStats"> If true: collecting training statistics will be </param>
		WriteOnly Property CollectTrainingStats As Boolean

		''' <summary>
		''' Get the current setting for collectTrainingStats
		''' </summary>
		ReadOnly Property IsCollectTrainingStats As Boolean

		''' <summary>
		''' Return the training statistics. Note that this may return null, unless setCollectTrainingStats has been set first
		''' </summary>
		''' <returns> Training statistics </returns>
		ReadOnly Property TrainingStats As SparkTrainingStats

		''' <summary>
		''' Set the iteration listeners. These should be called after every averaging (or similar) operation in the TrainingMaster,
		''' though the exact behaviour may be dependent on each TrainingListener
		''' </summary>
		''' <param name="listeners"> Listeners to set </param>
		WriteOnly Property Listeners As ICollection(Of TrainingListener)


		''' <summary>
		''' Set the iteration listeners and the StatsStorageRouter. This is typically used for UI functionality: for example,
		''' setListeners(new FileStatsStorage(myFile), Collections.singletonList(new StatsListener(null))). This will pass a
		''' StatsListener to each worker, and then shuffle the results back to the specified FileStatsStorage instance (which
		''' can then be attached to the UI or loaded later)
		''' </summary>
		''' <param name="router">       StatsStorageRouter in which to place the results </param>
		''' <param name="listeners">    Listeners </param>
		Sub setListeners(ByVal router As StatsStorageRouter, ByVal listeners As ICollection(Of TrainingListener))

		''' <summary>
		''' Attempt to delete any temporary files generated by this TrainingMaster.
		''' Depending on the configuration, no temporary files may be generated.
		''' </summary>
		''' <param name="sc"> JavaSparkContext (used to access HDFS etc file systems, when required) </param>
		''' <returns> True if deletion was successful (or, no files to delete); false otherwise. </returns>
		Function deleteTempFiles(ByVal sc As JavaSparkContext) As Boolean

		''' <summary>
		''' Attempt to delete any temporary files generated by this TrainingMaster.
		''' Depending on the configuration, no temporary files may be generated.
		''' </summary>
		''' <param name="sc"> SparkContext (used to access HDFS etc file systems, when required) </param>
		''' <returns> True if deletion was successful (or, no files to delete); false otherwise. </returns>
		Function deleteTempFiles(ByVal sc As SparkContext) As Boolean
	End Interface

End Namespace