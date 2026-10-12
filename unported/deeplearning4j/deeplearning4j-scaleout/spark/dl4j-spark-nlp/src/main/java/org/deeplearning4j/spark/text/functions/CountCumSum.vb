Imports Accumulator = org.apache.spark.Accumulator
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports MaxPerPartitionAccumulator = org.deeplearning4j.spark.text.accumulators.MaxPerPartitionAccumulator
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

Namespace org.deeplearning4j.spark.text.functions


	''' <summary>
	''' @author jeffreytang
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") public class CountCumSum
	Public Class CountCumSum

		' Starting variables
		Private sc As JavaSparkContext
		Private sentenceCountRDD As JavaRDD(Of AtomicLong)

		' Variables to fill in as we go
		Private foldWithinPartitionRDD As JavaRDD(Of AtomicLong)
		Private broadcastedMaxPerPartitionCounter As Broadcast(Of Counter(Of Integer))
		Private cumSumRDD As JavaRDD(Of Long)

		' Constructor
		Public Sub New(ByVal sentenceCountRDD As JavaRDD(Of AtomicLong))
			Me.sentenceCountRDD = sentenceCountRDD
			Me.sc = New JavaSparkContext(sentenceCountRDD.context())
		End Sub

		' Getter
		Public Overridable ReadOnly Property CumSumRDD As JavaRDD(Of Long)
			Get
				If cumSumRDD IsNot Nothing Then
					Return cumSumRDD
				Else
					Throw New IllegalAccessError("Cumulative Sum list not defined. Call buildCumSum() first.")
				End If
			End Get
		End Property

		' For each equivalent for partitions
		Public Overridable Sub actionForMapPartition(ByVal rdd As JavaRDD)
			' Action to fill the accumulator
			rdd.foreachPartition(New MapPerPartitionVoidFunction())
		End Sub

		' Do cum sum within the partition
		Public Overridable Sub cumSumWithinPartition()

			' Accumulator to get the max of the cumulative sum in each partition
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.apache.spark.Accumulator<org.nd4j.common.primitives.Counter<Integer>> maxPerPartitionAcc = sc.accumulator(new org.nd4j.common.primitives.Counter<Integer>(), new org.deeplearning4j.spark.text.accumulators.MaxPerPartitionAccumulator());
			Dim maxPerPartitionAcc As Accumulator(Of Counter(Of Integer)) = sc.accumulator(New Counter(Of Integer)(), New MaxPerPartitionAccumulator())
			' Partition mapping to fold within partition
			foldWithinPartitionRDD = sentenceCountRDD.mapPartitionsWithIndex(New FoldWithinPartitionFunction(maxPerPartitionAcc), True).cache()
			actionForMapPartition(foldWithinPartitionRDD)

			' Broadcast the counter (partition index : sum of count) to all workers
			broadcastedMaxPerPartitionCounter = sc.broadcast(maxPerPartitionAcc.value())
		End Sub

		Public Overridable Sub cumSumBetweenPartition()

			cumSumRDD = foldWithinPartitionRDD.mapPartitionsWithIndex(New FoldBetweenPartitionFunction(broadcastedMaxPerPartitionCounter), True).setName("cumSumRDD").cache()
			foldWithinPartitionRDD.unpersist()
		End Sub

		Public Overridable Function buildCumSum() As JavaRDD(Of Long)
			cumSumWithinPartition()
			cumSumBetweenPartition()
			Return getCumSumRDD()
		End Function
	End Class

End Namespace