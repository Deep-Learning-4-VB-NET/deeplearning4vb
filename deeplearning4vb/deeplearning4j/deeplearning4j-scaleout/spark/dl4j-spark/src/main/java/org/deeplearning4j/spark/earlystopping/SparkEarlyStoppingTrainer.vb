Imports SparkContext = org.apache.spark.SparkContext
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports org.deeplearning4j.earlystopping
Imports org.deeplearning4j.earlystopping
Imports org.deeplearning4j.earlystopping.listener
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports org.deeplearning4j.spark.api
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

Namespace org.deeplearning4j.spark.earlystopping

	Public Class SparkEarlyStoppingTrainer
		Inherits BaseSparkEarlyStoppingTrainer(Of MultiLayerNetwork)

		Private sparkNet As SparkDl4jMultiLayer

		Public Sub New(ByVal sc As SparkContext, ByVal trainingMaster As TrainingMaster, ByVal esConfig As EarlyStoppingConfiguration(Of MultiLayerNetwork), ByVal net As MultiLayerNetwork, ByVal train As JavaRDD(Of DataSet))
			Me.New(New JavaSparkContext(sc), trainingMaster, esConfig, net, train, Nothing)
		End Sub

		Public Sub New(ByVal sc As JavaSparkContext, ByVal trainingMaster As TrainingMaster, ByVal esConfig As EarlyStoppingConfiguration(Of MultiLayerNetwork), ByVal net As MultiLayerNetwork, ByVal train As JavaRDD(Of DataSet))
			Me.New(sc, trainingMaster, esConfig, net, train, Nothing)
		End Sub

		Public Sub New(ByVal sc As SparkContext, ByVal trainingMaster As TrainingMaster, ByVal esConfig As EarlyStoppingConfiguration(Of MultiLayerNetwork), ByVal net As MultiLayerNetwork, ByVal train As JavaRDD(Of DataSet), ByVal listener As EarlyStoppingListener(Of MultiLayerNetwork))
			Me.New(New JavaSparkContext(sc), trainingMaster, esConfig, net, train, listener)
		End Sub

		Public Sub New(ByVal sc As JavaSparkContext, ByVal trainingMaster As TrainingMaster, ByVal esConfig As EarlyStoppingConfiguration(Of MultiLayerNetwork), ByVal net As MultiLayerNetwork, ByVal train As JavaRDD(Of DataSet), ByVal listener As EarlyStoppingListener(Of MultiLayerNetwork))
			MyBase.New(sc, esConfig, net, train, Nothing, listener)
			sparkNet = New SparkDl4jMultiLayer(sc, net, trainingMaster)
		End Sub


		Protected Friend Overrides Sub fit(ByVal data As JavaRDD(Of DataSet))
			sparkNet.fit(data)
		End Sub

		Protected Friend Overrides Sub fitMulti(ByVal data As JavaRDD(Of MultiDataSet))
			Throw New System.NotSupportedException("Not supported")
		End Sub

		Protected Friend Overrides ReadOnly Property Score As Double
			Get
				Return sparkNet.Score
			End Get
		End Property

		Public Overrides Function pretrain() As EarlyStoppingResult(Of MultiLayerNetwork)
			Throw New System.NotSupportedException("Not supported")
		End Function
	End Class

End Namespace