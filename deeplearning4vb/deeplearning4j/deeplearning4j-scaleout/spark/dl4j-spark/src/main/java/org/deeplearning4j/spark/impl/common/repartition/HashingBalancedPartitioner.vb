Imports System
Imports System.Collections.Generic
Imports Predicate = org.nd4j.shade.guava.base.Predicate
Imports Collections2 = org.nd4j.shade.guava.collect.Collections2
Imports Partitioner = org.apache.spark.Partitioner
Imports Tuple2 = scala.Tuple2
import static org.nd4j.shade.guava.base.Preconditions.checkArgument
import static org.nd4j.shade.guava.base.Preconditions.checkNotNull

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


	Public Class HashingBalancedPartitioner
		Inherits Partitioner

		Private ReadOnly numClasses As Integer ' Total number of element classes
'JAVA TO VB CONVERTER NOTE: The field numPartitions was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly numPartitions_Conflict As Integer ' Total number of partitions
		' partitionWeightsByClass : numClasses lists of numPartitions elements
		' where each element is the partition's relative share of its # of elements w.r.t the per-partition mean
		' e.g. we have 3 partitions, with red and blue elements, red is indexed by 0, blue by 1:
		'  [ r, r, r, r, b, b, b ], [r, b, b], [b, b, b, b, b, r, r]
		' avg # red elems per partition : 2.33
		' avg # blue elems per partition : 3.33
		' partitionWeightsByClass = [[1.714, .429, .857], [0.9, 0.6, 1.5]]
		Private partitionWeightsByClass As IList(Of IList(Of Double))

		' The cumulative distribution of jump probabilities of extra elements by partition, by class
		' 0 for partitions that already have enough elements
		Private jumpTable As IList(Of IList(Of Double))
		Private r As Random

		Public Sub New(ByVal partitionWeightsByClass As IList(Of IList(Of Double)))
			Dim pw As IList(Of IList(Of Double)) = checkNotNull(partitionWeightsByClass)
			checkArgument(pw.Count > 0, "Partition weights are required")
			checkArgument(pw.Count >= 1, "There should be at least one element class")
			checkArgument(Not checkNotNull(pw(0)).isEmpty(), "At least one partition is required")
			Me.numClasses = pw.Count
			Me.numPartitions_Conflict = pw(0).Count
			For i As Integer = 1 To pw.Count - 1
				checkArgument(checkNotNull(pw(i)).size() = Me.numPartitions_Conflict, "Non-consistent partition weight specification")
				' you also should have sum(pw.get(i)) = this.numPartitions
			Next i
			Me.partitionWeightsByClass = partitionWeightsByClass ' p_(j, i)

			Dim jumpsByClass As IList(Of IList(Of Double)) = New List(Of IList(Of Double))()
			For j As Integer = 0 To numClasses - 1
				Dim totalImbalance As Double? = 0R ' i_j = sum(max(1 - p_(j, i), 0) , i = 1..numPartitions)
				For i As Integer = 0 To numPartitions_Conflict - 1
					totalImbalance += If(partitionWeightsByClass(j)(i) >= 0, Math.Max(1 - partitionWeightsByClass(j)(i), 0), 0)
				Next i
				Dim sumProb As Double? = 0R
				Dim cumulProbsThisClass As IList(Of Double) = New List(Of Double)()
				For i As Integer = 0 To numPartitions_Conflict - 1
					If partitionWeightsByClass(j)(i) >= 0 AndAlso (totalImbalance > 0 OrElse sumProb >= 1) Then
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
						Dim thisPartitionRelProb As Double? = Math.Max(1 - partitionWeightsByClass(j)(i), 0) / totalImbalance.Value
						If thisPartitionRelProb > 0 Then
							sumProb += thisPartitionRelProb
							cumulProbsThisClass.Add(sumProb)
						Else
							cumulProbsThisClass.Add(0R)
						End If
					Else
						' There's no more imbalance, every jumpProb is > 1
						cumulProbsThisClass.Add(0R)
					End If
				Next i
				jumpsByClass.Add(cumulProbsThisClass)
			Next j

			Me.jumpTable = jumpsByClass
		End Sub

		Public Overrides Function numPartitions() As Integer
			Dim list As IList(Of Double) = partitionWeightsByClass(0)
			Dim count As Integer = 0
			For Each d As Double? In list
				If d >= 0 Then
					count += 1
				End If
			Next d
			Return count
		End Function

		Public Overrides Function getPartition(ByVal key As Object) As Integer
			checkArgument(TypeOf key Is Tuple2, "The key should be in the form: Tuple2(SparkUID, class) ...")
			Dim uidNclass As Tuple2(Of Long, Integer) = DirectCast(key, Tuple2(Of Long, Integer))
			Dim uid As Long? = uidNclass._1()
			Dim partitionId As Integer? = CInt(Math.Truncate(uid Mod numPartitions_Conflict))
			Dim elementClass As Integer? = uidNclass._2()

			Dim jumpProbability As Double? = Math.Max(1R - 1R / partitionWeightsByClass(elementClass)(partitionId), 0R)
			Dim rand As New LinearCongruentialGenerator(uid)

			Dim thisJumps As Double? = rand.nextDouble()
			Dim thisPartition As Integer? = partitionId
			If thisJumps < jumpProbability Then
				' Where do we jump ?
				Dim jumpsTo As IList(Of Double) = jumpTable(elementClass)
				Dim destination As Double? = rand.nextDouble()
				Dim probe As Integer? = 0

				Do While jumpsTo(probe) < destination
					probe += 1
				Loop
				thisPartition = probe
			End If

			Return thisPartition
		End Function

		' Multiplier chosen for nice distribution properties when successive random values are used to form tuples,
		' which is the case with Spark's uid.
		' See P. L'Ecuyer. Tables of Linear Congruential Generators of Different Sizes and Good Lattice
		' Structure. In Mathematics of Computation 68 (225): pages 249–260
		'
		Friend NotInheritable Class LinearCongruentialGenerator
			Friend state As Long

			Public Sub New(ByVal seed As Long)
				Me.state = seed
			End Sub

			Public Function nextDouble() As Double
				state = 2862933555777941757L * state + 1
				Return (CDbl(CInt(CLng(CULng(state) >> 33)) + 1)) / (&H1.0p31)
			End Function
		End Class
	End Class

End Namespace