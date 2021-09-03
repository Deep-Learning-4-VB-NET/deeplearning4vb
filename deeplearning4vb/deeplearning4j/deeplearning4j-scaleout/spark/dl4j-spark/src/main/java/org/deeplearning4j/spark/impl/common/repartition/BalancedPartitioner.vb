Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Partitioner = org.apache.spark.Partitioner

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

Namespace org.deeplearning4j.spark.impl.common.repartition


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class BalancedPartitioner extends org.apache.spark.Partitioner
	Public Class BalancedPartitioner
		Inherits Partitioner

'JAVA TO VB CONVERTER NOTE: The field numPartitions was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly numPartitions_Conflict As Integer 'Total number of partitions
		Private ReadOnly elementsPerPartition As Integer
		Private ReadOnly remainder As Integer
		Private r As Random

		Public Sub New(ByVal numPartitions As Integer, ByVal elementsPerPartition As Integer, ByVal remainder As Integer)
			Me.numPartitions_Conflict = numPartitions
			Me.elementsPerPartition = elementsPerPartition
			Me.remainder = remainder
		End Sub

		Public Overrides Function numPartitions() As Integer
			Return numPartitions_Conflict
		End Function

		Public Overrides Function getPartition(ByVal key As Object) As Integer
			Dim elementIdx As Integer = key.GetHashCode()

			'First 'remainder' executors get elementsPerPartition+1 each; the remainder get
			' elementsPerPartition each. This is because the total number of examples might not be an exact multiple
			' of the number of cores in the cluster

			'Work out: which partition it belongs to...
			If elementIdx <= (elementsPerPartition + 1) * remainder Then
				'This goes into one of the larger partitions (of size elementsPerPartition+1)
				Dim outputPartition As Integer = elementIdx \ (elementsPerPartition + 1)
				If outputPartition >= numPartitions_Conflict Then
					'Should never happen, unless there's some up-stream problem with calculating elementsPerPartition
					outputPartition = Random.Next(numPartitions_Conflict)
					log.trace("Random partition assigned (1): elementIdx={}, numPartitions={}, elementsPerPartition={}, remainder={}", elementIdx, numPartitions_Conflict, elementsPerPartition, remainder)
				End If
				Return outputPartition
			Else
				'This goes into one of the standard size partitions (of size elementsPerPartition)
				Dim numValsInLargerPartitions As Integer = remainder * (elementsPerPartition + 1)
				Dim idxInSmallerPartitions As Integer = elementIdx - numValsInLargerPartitions
				Dim smallPartitionIdx As Integer = idxInSmallerPartitions \ elementsPerPartition
				Dim outputPartition As Integer = remainder + smallPartitionIdx
				If outputPartition >= numPartitions_Conflict Then
					'Should never happen, unless there's some up-stream problem with calculating elementsPerPartition
					outputPartition = Random.Next(numPartitions_Conflict)
					log.trace("Random partition assigned (2): elementIdx={}, numPartitions={}, elementsPerPartition={}, remainder={}", elementIdx, numPartitions_Conflict, elementsPerPartition, remainder)
				End If
				Return outputPartition
			End If
		End Function

		Private ReadOnly Property Random As Random
			Get
				SyncLock Me
					If r Is Nothing Then
						r = New Random()
					End If
					Return r
				End SyncLock
			End Get
		End Property
	End Class

End Namespace