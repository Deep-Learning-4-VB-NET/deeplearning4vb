Imports SparkContext = org.apache.spark.SparkContext
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports org.deeplearning4j.earlystopping
Imports org.deeplearning4j.earlystopping
Imports org.deeplearning4j.earlystopping.listener
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports org.deeplearning4j.spark.api
Imports SparkComputationGraph = org.deeplearning4j.spark.impl.graph.SparkComputationGraph
Imports DataSetToMultiDataSetFn = org.deeplearning4j.spark.impl.graph.dataset.DataSetToMultiDataSetFn
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

Namespace org.deeplearning4j.spark.earlystopping

	Public Class SparkEarlyStoppingGraphTrainer
		Inherits BaseSparkEarlyStoppingTrainer(Of ComputationGraph)

		Private sparkNet As SparkComputationGraph

		Public Sub New(ByVal sc As SparkContext, ByVal trainingMaster As TrainingMaster, ByVal esConfig As EarlyStoppingConfiguration(Of ComputationGraph), ByVal net As ComputationGraph, ByVal train As JavaRDD(Of MultiDataSet), ByVal examplesPerFit As Integer, ByVal totalExamples As Integer)
			Me.New(New JavaSparkContext(sc), trainingMaster, esConfig, net, train, Nothing)
		End Sub

		Public Sub New(ByVal sc As JavaSparkContext, ByVal trainingMaster As TrainingMaster, ByVal esConfig As EarlyStoppingConfiguration(Of ComputationGraph), ByVal net As ComputationGraph, ByVal train As JavaRDD(Of MultiDataSet), ByVal examplesPerFit As Integer, ByVal totalExamples As Integer)
			Me.New(sc, trainingMaster, esConfig, net, train, Nothing)
		End Sub

		Public Sub New(ByVal sc As SparkContext, ByVal trainingMaster As TrainingMaster, ByVal esConfig As EarlyStoppingConfiguration(Of ComputationGraph), ByVal net As ComputationGraph, ByVal train As JavaRDD(Of MultiDataSet))
			Me.New(New JavaSparkContext(sc), trainingMaster, esConfig, net, train, Nothing)
		End Sub

		Public Sub New(ByVal sc As JavaSparkContext, ByVal trainingMaster As TrainingMaster, ByVal esConfig As EarlyStoppingConfiguration(Of ComputationGraph), ByVal net As ComputationGraph, ByVal train As JavaRDD(Of MultiDataSet))
			Me.New(sc, trainingMaster, esConfig, net, train, Nothing)
		End Sub

		Public Sub New(ByVal sc As JavaSparkContext, ByVal trainingMaster As TrainingMaster, ByVal esConfig As EarlyStoppingConfiguration(Of ComputationGraph), ByVal net As ComputationGraph, ByVal train As JavaRDD(Of MultiDataSet), ByVal listener As EarlyStoppingListener(Of ComputationGraph))
			MyBase.New(sc, esConfig, net, Nothing, train, listener)
			Me.sparkNet = New SparkComputationGraph(sc, net, trainingMaster)
		End Sub


		Protected Friend Overrides Sub fit(ByVal data As JavaRDD(Of DataSet))
			fitMulti(data.map(New DataSetToMultiDataSetFn()))
		End Sub

		Protected Friend Overrides Sub fitMulti(ByVal data As JavaRDD(Of MultiDataSet))
			sparkNet.fitMultiDataSet(data)
		End Sub

		Protected Friend Overrides ReadOnly Property Score As Double
			Get
				Return sparkNet.Score
			End Get
		End Property

		Public Overrides Function pretrain() As EarlyStoppingResult(Of ComputationGraph)
			Throw New System.NotSupportedException("Not supported")
		End Function
	End Class

End Namespace