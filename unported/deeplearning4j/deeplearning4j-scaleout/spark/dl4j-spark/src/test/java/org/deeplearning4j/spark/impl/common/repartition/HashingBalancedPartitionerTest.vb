Imports System
Imports System.Collections.Generic
Imports HashPartitioner = org.apache.spark.HashPartitioner
Imports JavaPairRDD = org.apache.spark.api.java.JavaPairRDD
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports Function2 = org.apache.spark.api.java.function.Function2
Imports PairFunction = org.apache.spark.api.java.function.PairFunction
Imports BaseSparkTest = org.deeplearning4j.spark.BaseSparkTest
Imports LinearCongruentialGenerator = org.deeplearning4j.spark.impl.common.repartition.HashingBalancedPartitioner.LinearCongruentialGenerator
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Tuple2 = scala.Tuple2
import static org.junit.jupiter.api.Assertions.assertTrue

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
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class HashingBalancedPartitionerTest extends org.deeplearning4j.spark.BaseSparkTest
	<Serializable>
	Public Class HashingBalancedPartitionerTest
		Inherits BaseSparkTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void hashingBalancedPartitionerDoesBalance()
		Public Overridable Sub hashingBalancedPartitionerDoesBalance()
			' partitionWeightsByClass = [[1.714, .429, .857], [0.9, 0.6, 1.5]]
			Dim reds As IList(Of Double) = New List(Of Double) From {1.714R, 0.429R, .857R}
			Dim blues As IList(Of Double) = New List(Of Double) From {0.9R, 0.6R, 1.5R}
			Dim partitionWeights As IList(Of IList(Of Double)) = New List(Of IList(Of Double)) From {reds, blues}

			Dim hbp As New HashingBalancedPartitioner(partitionWeights)
			Dim l As IList(Of Tuple2(Of Integer, String)) = New List(Of Tuple2(Of Integer, String))()

			For i As Integer = 0 To 3
				l.Add(New Tuple2(Of Integer, String)(0, "red"))
			Next i
			For i As Integer = 0 To 2
				l.Add(New Tuple2(Of Integer, String)(0, "blue"))
			Next i
			For i As Integer = 0 To 0
				l.Add(New Tuple2(Of Integer, String)(1, "red"))
			Next i
			For i As Integer = 0 To 1
				l.Add(New Tuple2(Of Integer, String)(1, "blue"))
			Next i
			For i As Integer = 0 To 1
				l.Add(New Tuple2(Of Integer, String)(2, "red"))
			Next i
			For i As Integer = 0 To 4
				l.Add(New Tuple2(Of Integer, String)(2, "blue"))
			Next i
			' This should give exactly the sought distribution
			Dim rdd As JavaPairRDD(Of Integer, String) = JavaPairRDD.fromJavaRDD(sc.parallelize(l)).partitionBy(New HashPartitioner(3))

			' Let's reproduce UIDs
			Dim indexedRDD As JavaPairRDD(Of Tuple2(Of Long, Integer), String) = rdd.zipWithUniqueId().mapToPair(New PairFunctionAnonymousInnerClass(Me))

			Dim testList As IList(Of Tuple2(Of Tuple2(Of Long, Integer), String)) = indexedRDD.collect()

			Dim colorCountsByPartition()() As Integer = { New Integer(1){}, New Integer(1){}, New Integer(1){} }
			For Each val As Tuple2(Of Tuple2(Of Long, Integer), String) In testList
	'            System.out.println(val);
				Dim partition As Integer? = hbp.getPartition(val._1())
	'            System.out.println(partition);

				If val._2().Equals("red") Then
					colorCountsByPartition(partition)(0) += 1
				Else
					colorCountsByPartition(partition)(1) += 1
				End If
			Next val

	'        for (int i = 0; i < 3; i++) {
	'            System.out.println(Arrays.toString(colorCountsByPartition[i]));
	'        }
			For i As Integer = 0 To 2
				' avg red per partition : 2.33
				assertTrue(colorCountsByPartition(i)(0) >= 1 AndAlso colorCountsByPartition(i)(0) < 4)
				' avg blue per partition : 3.33
				assertTrue(colorCountsByPartition(i)(1) >= 2 AndAlso colorCountsByPartition(i)(1) < 5)
			Next i

		End Sub

		Private Class PairFunctionAnonymousInnerClass
			Inherits PairFunction(Of Tuple2(Of Tuple2(Of Integer, String), Long), Tuple2(Of Long, Integer), String)

			Private ReadOnly outerInstance As HashingBalancedPartitionerTest

			Public Sub New(ByVal outerInstance As HashingBalancedPartitionerTest)
				Me.outerInstance = outerInstance
			End Sub

			Public Overrides Function [call](ByVal payLoadNuid As Tuple2(Of Tuple2(Of Integer, String), Long)) As Tuple2(Of Tuple2(Of Long, Integer), String)
				Dim uid As Long? = payLoadNuid._2()
				Dim value As String = payLoadNuid._1()._2()
				Dim elemClass As Integer? = If(value.Equals("red"), 0, 1)
				Return New Tuple2(Of Tuple2(Of Long, Integer), String)(New Tuple2(Of Long, Integer)(uid, elemClass), value)
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void hashPartitionerBalancesAtScale()
		Public Overridable Sub hashPartitionerBalancesAtScale()
			Dim r As New LinearCongruentialGenerator(10000)
			Dim elements As IList(Of String) = New List(Of String)()
			For i As Integer = 0 To 9999
				' The red occur towards the end
				If r.nextDouble() < (CDbl(i) / 10000R) Then
					elements.Add("red")
				End If
				' The blue occur towards the front
				If r.nextDouble() < (1 - CDbl(i) / 10000R) Then
					elements.Add("blue")
				End If
			Next i
			Dim countRed As Integer? = 0
			Dim countBlue As Integer? = 0
			For Each elem As String In elements
				If elem.Equals("red") Then
					countRed += 1
				Else
					countBlue += 1
				End If
			Next elem
			Dim rdd As JavaRDD(Of String) = sc.parallelize(elements)
			Dim indexedRDD As JavaPairRDD(Of Tuple2(Of Long, Integer), String) = rdd.zipWithUniqueId().mapToPair(New PairFunctionAnonymousInnerClass2(Me))

			Dim numPartitions As Integer? = indexedRDD.getNumPartitions()

			' rdd and indexedRDD have the same partition distribution
			Dim partitionTuples As IList(Of Tuple2(Of Integer, Integer)) = rdd.mapPartitionsWithIndex(New CountRedBluePartitionsFunction(Me), True).collect()
			Dim redWeights As IList(Of Double) = New List(Of Double)()
			Dim blueWeights As IList(Of Double) = New List(Of Double)()
			Dim avgRed As Single? = CSng(countRed) / numPartitions.Value
			Dim avgBlue As Single? = CSng(countBlue) / numPartitions.Value
			For i As Integer = 0 To partitionTuples.Count - 1
				Dim counts As Tuple2(Of Integer, Integer) = partitionTuples(i)
				redWeights.Add(CDbl(counts._1()) / avgRed.Value)
				blueWeights.Add(CDbl(counts._2()) / avgBlue.Value)
			Next i
			Dim partitionWeights As IList(Of IList(Of Double)) = New List(Of IList(Of Double)) From {redWeights, blueWeights}


			Dim hbp As New HashingBalancedPartitioner(partitionWeights)

			Dim testList As IList(Of Tuple2(Of Tuple2(Of Long, Integer), String)) = indexedRDD.collect()

'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim colorCountsByPartition[][] As Integer = new Integer[numPartitions][2]
			Dim colorCountsByPartition()() As Integer = RectangularArrays.RectangularIntegerArray(numPartitions, 2)
			For Each val As Tuple2(Of Tuple2(Of Long, Integer), String) In testList
				Dim partition As Integer? = hbp.getPartition(val._1())

				If val._2().Equals("red") Then
					colorCountsByPartition(partition)(0) += 1
				Else
					colorCountsByPartition(partition)(1) += 1
				End If
			Next val

	'        for (int i = 0; i < numPartitions; i++) {
	'            System.out.println(Arrays.toString(colorCountsByPartition[i]));
	'        }
	'
	'        System.out.println("Ideal red # per partition: " + avgRed);
	'        System.out.println("Ideal blue # per partition: " + avgBlue);

			For i As Integer = 0 To numPartitions.Value - 1
				' avg red per partition : 2.33
				assertTrue(colorCountsByPartition(i)(0) >= CLng(Math.Round(avgRed.Value * .99, MidpointRounding.AwayFromZero)) AndAlso colorCountsByPartition(i)(0) < CLng(Math.Round(avgRed.Value * 1.01, MidpointRounding.AwayFromZero)) + 1)
				' avg blue per partition : 3.33
				assertTrue(colorCountsByPartition(i)(1) >= CLng(Math.Round(avgBlue.Value * .99, MidpointRounding.AwayFromZero)) AndAlso colorCountsByPartition(i)(1) < CLng(Math.Round(avgBlue.Value * 1.01, MidpointRounding.AwayFromZero)) + 1)
			Next i


		End Sub

		Private Class PairFunctionAnonymousInnerClass2
			Inherits PairFunction(Of Tuple2(Of String, Long), Tuple2(Of Long, Integer), String)

			Private ReadOnly outerInstance As HashingBalancedPartitionerTest

			Public Sub New(ByVal outerInstance As HashingBalancedPartitionerTest)
				Me.outerInstance = outerInstance
			End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public scala.Tuple2<scala.Tuple2<Long, Integer>, String> call(scala.Tuple2<String, Long> stringLongTuple2) throws Exception
			Public Overrides Function [call](ByVal stringLongTuple2 As Tuple2(Of String, Long)) As Tuple2(Of Tuple2(Of Long, Integer), String)
				Dim elemClass As Integer? = If(stringLongTuple2._1().Equals("red"), 0, 1)
				Return New Tuple2(Of Tuple2(Of Long, Integer), String)(New Tuple2(Of Long, Integer)(stringLongTuple2._2(), elemClass), stringLongTuple2._1())
			End Function
		End Class

		Friend Class CountRedBluePartitionsFunction
			Implements Function2(Of Integer, IEnumerator(Of String), IEnumerator(Of Tuple2(Of Integer, Integer)))

			Private ReadOnly outerInstance As HashingBalancedPartitionerTest

			Public Sub New(ByVal outerInstance As HashingBalancedPartitionerTest)
				Me.outerInstance = outerInstance
			End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public Iterator<scala.Tuple2<Integer, Integer>> call(System.Nullable<Integer> v1, Iterator<String> v2) throws Exception
			Public Overrides Function [call](ByVal v1 As Integer?, ByVal v2 As IEnumerator(Of String)) As IEnumerator(Of Tuple2(Of Integer, Integer))

				Dim redCount As Integer = 0
				Dim blueCount As Integer = 0
				Do While v2.MoveNext()
					Dim elem As String = v2.Current
					If elem.Equals("red") Then
						redCount += 1
					Else
						blueCount += 1
					End If
				Loop

				Return Collections.singletonList(New Tuple2(Of )(redCount, blueCount)).GetEnumerator()
			End Function
		End Class

	End Class

End Namespace