Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports Repartitioner = org.deeplearning4j.spark.api.Repartitioner
Imports org.deeplearning4j.spark.impl.common
Imports Tuple2 = scala.Tuple2

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

Namespace org.deeplearning4j.spark.impl.repartitioner


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class DefaultRepartitioner implements org.deeplearning4j.spark.api.Repartitioner
	<Serializable>
	Public Class DefaultRepartitioner
		Implements Repartitioner

		Public Const DEFAULT_MAX_PARTITIONS As Integer = 5000

		Private ReadOnly maxPartitions As Integer

		''' <summary>
		''' Create a DefaultRepartitioner with the default maximum number of partitions, <seealso cref="DEFAULT_MAX_PARTITIONS"/>
		''' </summary>
		Public Sub New()
			Me.New(DEFAULT_MAX_PARTITIONS)
		End Sub

		''' 
		''' <param name="maxPartitions"> Maximum number of partitions </param>
		Public Sub New(ByVal maxPartitions As Integer)
			Me.maxPartitions = maxPartitions
		End Sub


		Public Overridable Function repartition(Of T)(ByVal rdd As JavaRDD(Of T), ByVal minObjectsPerPartition As Integer, ByVal numExecutors As Integer) As JavaRDD(Of T) Implements Repartitioner.repartition
			'Num executors intentionally not used

			'Count each partition...
			Dim partitionCounts As IList(Of Tuple2(Of Integer, Integer)) = rdd.mapPartitionsWithIndex(New CountPartitionsFunction(Of T)(), True).collect()
			Dim totalObjects As Integer = 0
			For Each t2 As Tuple2(Of Integer, Integer) In partitionCounts
				totalObjects += t2._2()
			Next t2

			'Now, we want 'minObjectsPerPartition' in each partition... up to a maximum number of partitions
			Dim numPartitions As Integer
			If totalObjects \ minObjectsPerPartition > maxPartitions Then
				'Need more than the minimum, to avoid exceeding the maximum
				numPartitions = maxPartitions
			Else
				numPartitions = CInt(Math.Truncate(Math.Ceiling(totalObjects / CDbl(minObjectsPerPartition))))
			End If
			Return EqualRepartitioner.repartition(rdd, numPartitions, partitionCounts)
		End Function

		Public Overrides Function ToString() As String
			Return "DefaultRepartitioner(maxPartitions=" & maxPartitions & ")"
		End Function
	End Class

End Namespace