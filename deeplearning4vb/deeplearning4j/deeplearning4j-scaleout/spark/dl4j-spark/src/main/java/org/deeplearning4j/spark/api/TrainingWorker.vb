Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports SparkTrainingStats = org.deeplearning4j.spark.api.stats.SparkTrainingStats
Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
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

Namespace org.deeplearning4j.spark.api


	Public Interface TrainingWorker(Of R As TrainingResult)

		''' <summary>
		''' Remove a training hook from the worker </summary>
		''' <param name="trainingHook"> the training hook to remove </param>
		Sub removeHook(ByVal trainingHook As TrainingHook)

		''' <summary>
		''' Add a training hook to be used
		''' during training of the worker </summary>
		''' <param name="trainingHook"> the training hook to add </param>
		Sub addHook(ByVal trainingHook As TrainingHook)

		''' <summary>
		''' Get the initial model when training a MultiLayerNetwork/SparkDl4jMultiLayer
		''' </summary>
		''' <returns> Initial model for this worker </returns>
		ReadOnly Property InitialModel As MultiLayerNetwork

		''' <summary>
		''' Get the initial model when training a ComputationGraph/SparkComputationGraph
		''' </summary>
		''' <returns> Initial model for this worker </returns>
		ReadOnly Property InitialModelGraph As ComputationGraph

		''' <summary>
		''' Process (fit) a minibatch for a MultiLayerNetwork
		''' </summary>
		''' <param name="dataSet"> Data set to train on </param>
		''' <param name="network"> Network to train </param>
		''' <param name="isLast">  If true: last data set currently available. If false: more data sets will be processed for this executor </param>
		''' <returns> Null, or a training result if training should be terminated immediately. </returns>
		Function processMinibatch(ByVal dataSet As DataSet, ByVal network As MultiLayerNetwork, ByVal isLast As Boolean) As R

		''' <summary>
		''' Process (fit) a minibatch for a ComputationGraph
		''' </summary>
		''' <param name="dataSet"> Data set to train on </param>
		''' <param name="graph">   Network to train </param>
		''' <param name="isLast">  If true: last data set currently available. If false: more data sets will be processed for this executor </param>
		''' <returns> Null, or a training result if training should be terminated immediately. </returns>
		Function processMinibatch(ByVal dataSet As DataSet, ByVal graph As ComputationGraph, ByVal isLast As Boolean) As R

		''' <summary>
		''' Process (fit) a minibatch for a ComputationGraph using a MultiDataSet
		''' </summary>
		''' <param name="dataSet"> Data set to train on </param>
		''' <param name="graph">   Network to train </param>
		''' <param name="isLast">  If true: last data set currently available. If false: more data sets will be processed for this executor </param>
		''' <returns> Null, or a training result if training should be terminated immediately. </returns>
		Function processMinibatch(ByVal dataSet As MultiDataSet, ByVal graph As ComputationGraph, ByVal isLast As Boolean) As R

		''' <summary>
		''' As per <seealso cref="processMinibatch(DataSet, MultiLayerNetwork, Boolean)"/> but used when <seealso cref="SparkTrainingStats"/> are being collecte
		''' </summary>
		Function processMinibatchWithStats(ByVal dataSet As DataSet, ByVal network As MultiLayerNetwork, ByVal isLast As Boolean) As Pair(Of R, SparkTrainingStats)

		''' <summary>
		''' As per <seealso cref="processMinibatch(DataSet, ComputationGraph, Boolean)"/> but used when <seealso cref="SparkTrainingStats"/> are being collected
		''' </summary>
		Function processMinibatchWithStats(ByVal dataSet As DataSet, ByVal graph As ComputationGraph, ByVal isLast As Boolean) As Pair(Of R, SparkTrainingStats)

		''' <summary>
		''' As per <seealso cref="processMinibatch(MultiDataSet, ComputationGraph, Boolean)"/> but used when <seealso cref="SparkTrainingStats"/> are being collected
		''' </summary>
		Function processMinibatchWithStats(ByVal dataSet As MultiDataSet, ByVal graph As ComputationGraph, ByVal isLast As Boolean) As Pair(Of R, SparkTrainingStats)

		''' <summary>
		''' Get the final result to be returned to the driver
		''' </summary>
		''' <param name="network"> Current state of the network </param>
		''' <returns> Result to return to the driver </returns>
		Function getFinalResult(ByVal network As MultiLayerNetwork) As R

		''' <summary>
		''' Get the final result to be returned to the driver
		''' </summary>
		''' <param name="graph"> Current state of the network </param>
		''' <returns> Result to return to the driver </returns>
		Function getFinalResult(ByVal graph As ComputationGraph) As R

		''' <summary>
		''' Get the final result to be returned to the driver, if no data was available for this executor
		''' </summary>
		''' <returns> Result to return to the driver </returns>
		ReadOnly Property FinalResultNoData As R

		''' <summary>
		''' As per <seealso cref="getFinalResultNoData()"/> but used when <seealso cref="SparkTrainingStats"/> are being collected
		''' </summary>
		ReadOnly Property FinalResultNoDataWithStats As Pair(Of R, SparkTrainingStats)

		''' <summary>
		''' As per <seealso cref="getFinalResult(MultiLayerNetwork)"/> but used when <seealso cref="SparkTrainingStats"/> are being collected
		''' </summary>
		Function getFinalResultWithStats(ByVal network As MultiLayerNetwork) As Pair(Of R, SparkTrainingStats)

		''' <summary>
		''' As per <seealso cref="getFinalResult(ComputationGraph)"/> but used when <seealso cref="SparkTrainingStats"/> are being collected
		''' </summary>
		Function getFinalResultWithStats(ByVal graph As ComputationGraph) As Pair(Of R, SparkTrainingStats)

		''' <summary>
		''' Get the <seealso cref="WorkerConfiguration"/> that contains information such as minibatch sizes, etc
		''' </summary>
		''' <returns> Worker configuration </returns>
		ReadOnly Property DataConfiguration As WorkerConfiguration

		ReadOnly Property InstanceId As Long
	End Interface

End Namespace