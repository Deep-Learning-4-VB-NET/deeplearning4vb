Imports TrainingResult = org.deeplearning4j.spark.api.TrainingResult
Imports org.deeplearning4j.spark.api

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

	Public MustInherit Class BaseTrainingWorker(Of R As org.deeplearning4j.spark.api.TrainingResult)
		Implements TrainingWorker(Of R)

		Public MustOverride ReadOnly Property InstanceId As Long Implements TrainingWorker(Of R).getInstanceId
		Public MustOverride ReadOnly Property DataConfiguration As WorkerConfiguration Implements TrainingWorker(Of R).getDataConfiguration
		Public MustOverride Function getFinalResultWithStats(ByVal graph As org.deeplearning4j.nn.graph.ComputationGraph) As org.nd4j.common.primitives.Pair(Of R, org.deeplearning4j.spark.api.stats.SparkTrainingStats) Implements TrainingWorker(Of R).getFinalResultWithStats
		Public MustOverride Function getFinalResultWithStats(ByVal network As org.deeplearning4j.nn.multilayer.MultiLayerNetwork) As org.nd4j.common.primitives.Pair(Of R, org.deeplearning4j.spark.api.stats.SparkTrainingStats) Implements TrainingWorker(Of R).getFinalResultWithStats
		Public MustOverride Function getFinalResultNoDataWithStats() As org.nd4j.common.primitives.Pair(Of R, org.deeplearning4j.spark.api.stats.SparkTrainingStats) Implements TrainingWorker(Of R).getFinalResultNoDataWithStats
		Public MustOverride ReadOnly Property FinalResultNoData As TrainingResult
		Public MustOverride Function getFinalResult(ByVal graph As org.deeplearning4j.nn.graph.ComputationGraph) As TrainingResult
		Public MustOverride Function getFinalResult(ByVal network As org.deeplearning4j.nn.multilayer.MultiLayerNetwork) As TrainingResult
		Public MustOverride Function processMinibatchWithStats(ByVal dataSet As org.nd4j.linalg.dataset.api.MultiDataSet, ByVal graph As org.deeplearning4j.nn.graph.ComputationGraph, ByVal isLast As Boolean) As org.nd4j.common.primitives.Pair(Of R, org.deeplearning4j.spark.api.stats.SparkTrainingStats)
		Public MustOverride Function processMinibatchWithStats(ByVal dataSet As org.nd4j.linalg.dataset.api.DataSet, ByVal graph As org.deeplearning4j.nn.graph.ComputationGraph, ByVal isLast As Boolean) As org.nd4j.common.primitives.Pair(Of R, org.deeplearning4j.spark.api.stats.SparkTrainingStats)
		Public MustOverride Function processMinibatchWithStats(ByVal dataSet As org.nd4j.linalg.dataset.api.DataSet, ByVal network As org.deeplearning4j.nn.multilayer.MultiLayerNetwork, ByVal isLast As Boolean) As org.nd4j.common.primitives.Pair(Of R, org.deeplearning4j.spark.api.stats.SparkTrainingStats)
		Public MustOverride Function processMinibatch(ByVal dataSet As org.nd4j.linalg.dataset.api.MultiDataSet, ByVal graph As org.deeplearning4j.nn.graph.ComputationGraph, ByVal isLast As Boolean) As TrainingResult
		Public MustOverride Function processMinibatch(ByVal dataSet As org.nd4j.linalg.dataset.api.DataSet, ByVal graph As org.deeplearning4j.nn.graph.ComputationGraph, ByVal isLast As Boolean) As TrainingResult
		Public MustOverride Function processMinibatch(ByVal dataSet As org.nd4j.linalg.dataset.api.DataSet, ByVal network As org.deeplearning4j.nn.multilayer.MultiLayerNetwork, ByVal isLast As Boolean) As TrainingResult
		Public MustOverride ReadOnly Property InitialModelGraph As org.deeplearning4j.nn.graph.ComputationGraph Implements TrainingWorker(Of R).getInitialModelGraph
		Public MustOverride ReadOnly Property InitialModel As org.deeplearning4j.nn.multilayer.MultiLayerNetwork Implements TrainingWorker(Of R).getInitialModel
		Public MustOverride Sub addHook(ByVal trainingHook As org.deeplearning4j.spark.api.TrainingHook)
		Public MustOverride Sub removeHook(ByVal trainingHook As org.deeplearning4j.spark.api.TrainingHook)
	End Class

End Namespace