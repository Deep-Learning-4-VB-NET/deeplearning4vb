Imports System
Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports Persistable = org.deeplearning4j.core.storage.Persistable
Imports StorageMetaData = org.deeplearning4j.core.storage.StorageMetaData
Imports ThresholdAlgorithm = org.deeplearning4j.optimize.solvers.accumulation.encoding.ThresholdAlgorithm
Imports TrainingResult = org.deeplearning4j.spark.api.TrainingResult
Imports SparkTrainingStats = org.deeplearning4j.spark.api.stats.SparkTrainingStats
Imports BaseTrainingResult = org.deeplearning4j.spark.impl.paramavg.BaseTrainingResult
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

Namespace org.deeplearning4j.spark.parameterserver.training


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @AllArgsConstructor @Builder @NoArgsConstructor public class SharedTrainingResult extends org.deeplearning4j.spark.impl.paramavg.BaseTrainingResult implements org.deeplearning4j.spark.api.TrainingResult, java.io.Serializable
	<Serializable>
	Public Class SharedTrainingResult
		Inherits BaseTrainingResult
		Implements TrainingResult

		Private updaterStateArray As INDArray
		Private scoreSum As Double
		Private aggregationsCount As Integer
		Private sparkTrainingStats As SparkTrainingStats
		Private listenerMetaData As ICollection(Of StorageMetaData)
		Private listenerStaticInfo As ICollection(Of Persistable)
		Private listenerUpdates As ICollection(Of Persistable)
		Private minibatchesPerExecutor As IDictionary(Of String, Integer)
		Private thresholdAlgorithm As ThresholdAlgorithm


		Public Overrides WriteOnly Property Stats Implements TrainingResult.setStats As SparkTrainingStats
			Set(ByVal sparkTrainingStats As SparkTrainingStats)
				setSparkTrainingStats(sparkTrainingStats)
			End Set
		End Property
	End Class

End Namespace