Imports System.Collections.Generic
Imports Data = lombok.Data
Imports Persistable = org.deeplearning4j.core.storage.Persistable
Imports StorageMetaData = org.deeplearning4j.core.storage.StorageMetaData
Imports TrainingResult = org.deeplearning4j.spark.api.TrainingResult
Imports SparkTrainingStats = org.deeplearning4j.spark.api.stats.SparkTrainingStats
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

Namespace org.deeplearning4j.spark.impl.paramavg


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class ParameterAveragingTrainingResult implements org.deeplearning4j.spark.api.TrainingResult
	Public Class ParameterAveragingTrainingResult
		Implements TrainingResult

		Private ReadOnly parameters As INDArray
		Private ReadOnly updaterState As INDArray
		Private ReadOnly score As Double
		Private sparkTrainingStats As SparkTrainingStats

		Private ReadOnly listenerMetaData As ICollection(Of StorageMetaData)
		Private ReadOnly listenerStaticInfo As ICollection(Of Persistable)
		Private ReadOnly listenerUpdates As ICollection(Of Persistable)


		Public Sub New(ByVal parameters As INDArray, ByVal updaterState As INDArray, ByVal score As Double, ByVal listenerMetaData As ICollection(Of StorageMetaData), ByVal listenerStaticInfo As ICollection(Of Persistable), ByVal listenerUpdates As ICollection(Of Persistable))
			Me.New(parameters, updaterState, score, Nothing, listenerMetaData, listenerStaticInfo, listenerUpdates)
		End Sub

		Public Sub New(ByVal parameters As INDArray, ByVal updaterState As INDArray, ByVal score As Double, ByVal sparkTrainingStats As SparkTrainingStats, ByVal listenerMetaData As ICollection(Of StorageMetaData), ByVal listenerStaticInfo As ICollection(Of Persistable), ByVal listenerUpdates As ICollection(Of Persistable))
			Me.parameters = parameters
			Me.updaterState = updaterState
			Me.score = score
			Me.sparkTrainingStats = sparkTrainingStats

			Me.listenerMetaData = listenerMetaData
			Me.listenerStaticInfo = listenerStaticInfo
			Me.listenerUpdates = listenerUpdates
		End Sub

		Public Overridable WriteOnly Property Stats Implements TrainingResult.setStats As SparkTrainingStats
			Set(ByVal sparkTrainingStats As SparkTrainingStats)
				Me.sparkTrainingStats = sparkTrainingStats
			End Set
		End Property
	End Class

End Namespace