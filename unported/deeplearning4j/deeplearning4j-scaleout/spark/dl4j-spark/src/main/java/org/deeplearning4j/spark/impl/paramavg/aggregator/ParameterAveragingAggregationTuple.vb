Imports System
Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports Persistable = org.deeplearning4j.core.storage.Persistable
Imports StorageMetaData = org.deeplearning4j.core.storage.StorageMetaData
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

Namespace org.deeplearning4j.spark.impl.paramavg.aggregator


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data @Builder public class ParameterAveragingAggregationTuple implements java.io.Serializable
	<Serializable>
	Public Class ParameterAveragingAggregationTuple
		Private ReadOnly parametersSum As INDArray
		Private ReadOnly updaterStateSum As INDArray
		Private ReadOnly scoreSum As Double
		Private ReadOnly aggregationsCount As Integer
		Private ReadOnly sparkTrainingStats As SparkTrainingStats
		Private ReadOnly listenerMetaData As ICollection(Of StorageMetaData)
		Private ReadOnly listenerStaticInfo As ICollection(Of Persistable)
		Private ReadOnly listenerUpdates As ICollection(Of Persistable)
	End Class

End Namespace