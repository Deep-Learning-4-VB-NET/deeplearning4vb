Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports JavaPairRDD = org.apache.spark.api.java.JavaPairRDD
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports Repartitioner = org.deeplearning4j.spark.api.Repartitioner
Imports org.deeplearning4j.spark.impl.common
Imports EqualPartitioner = org.deeplearning4j.spark.impl.common.repartition.EqualPartitioner
Imports SparkUtils = org.deeplearning4j.spark.util.SparkUtils
Imports MathUtils = org.nd4j.common.util.MathUtils
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
'ORIGINAL LINE: @Slf4j public class EqualRepartitioner implements org.deeplearning4j.spark.api.Repartitioner
	<Serializable>
	Public Class EqualRepartitioner
		Implements Repartitioner

		Public Overridable Function repartition(Of T)(ByVal rdd As JavaRDD(Of T), ByVal minObjectsPerPartition As Integer, ByVal numExecutors As Integer) As JavaRDD(Of T) Implements Repartitioner.repartition
			'minObjectsPerPartition: intentionally not used here

			'Repartition: either always, or origNumPartitions != numWorkers

			'First: count number of elements in each partition. Need to know this so we can work out how to properly index each example,
			' so we can in turn create properly balanced partitions after repartitioning
			'Because the objects (DataSets etc) should be small, this should be OK

			'Count each partition...
			Dim partitionCounts As IList(Of Tuple2(Of Integer, Integer)) = rdd.mapPartitionsWithIndex(New CountPartitionsFunction(Of T)(), True).collect()
			Return repartition(rdd, numExecutors, partitionCounts)
		End Function


		Public Shared Function repartition(Of T)(ByVal rdd As JavaRDD(Of T), ByVal numPartitions As Integer, ByVal partitionCounts As IList(Of Tuple2(Of Integer, Integer))) As JavaRDD(Of T)
			Dim totalObjects As Integer = 0
			Dim initialPartitions As Integer = partitionCounts.Count

			For Each t2 As Tuple2(Of Integer, Integer) In partitionCounts
				totalObjects += t2._2()
			Next t2

			'Check if already correct
			Dim minAllowable As Integer = CInt(Math.Truncate(Math.Floor(totalObjects / CDbl(numPartitions))))
			Dim maxAllowable As Integer = CInt(Math.Truncate(Math.Ceiling(totalObjects / CDbl(numPartitions))))

			Dim repartitionRequired As Boolean = False
			For Each t2 As Tuple2(Of Integer, Integer) In partitionCounts
				If t2._2() < minAllowable OrElse t2._2() > maxAllowable Then
					repartitionRequired = True
					Exit For
				End If
			Next t2

			If initialPartitions = numPartitions AndAlso Not repartitionRequired Then
				'Don't need to do any repartitioning here - already in the format we want
				Return rdd
			End If

			'Index each element for repartitioning (can only do manual repartitioning on a JavaPairRDD)
			Dim pairIndexed As JavaPairRDD(Of Integer, T) = SparkUtils.indexedRDD(rdd)

			'Handle remainder.
			'We'll randomly allocate one of these to a single partition, with no partition getting more than 1 (otherwise, imbalanced)
			'Given that we don't know exactly how Spark will allocate partitions to workers, we are probably better off doing
			' this randomly rather than "first N get +1" or "every M get +1" as this could introduce poor load balancing
			Dim remainder As Integer = totalObjects Mod numPartitions
			Dim remainderPartitions() As Integer = Nothing
			If remainder > 0 Then
				remainderPartitions = New Integer(remainder - 1){}
				Dim temp(numPartitions - 1) As Integer
				For i As Integer = 0 To temp.Length - 1
					temp(i) = i
				Next i
				MathUtils.shuffleArray(temp, New Random())
				Array.Copy(temp, 0, remainderPartitions, 0, remainder)
			End If

			Dim partitionSizeExRemainder As Integer = totalObjects \ numPartitions
			pairIndexed = pairIndexed.partitionBy(New EqualPartitioner(numPartitions, partitionSizeExRemainder, remainderPartitions))
			Return pairIndexed.values()
		End Function
	End Class

End Namespace