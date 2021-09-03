Imports System
Imports System.Collections.Generic
Imports Platform = com.sun.jna.Platform
Imports Partitioner = org.apache.spark.Partitioner
Imports JavaPairRDD = org.apache.spark.api.java.JavaPairRDD
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports BaseSparkTest = org.deeplearning4j.spark.BaseSparkTest
Imports Repartition = org.deeplearning4j.spark.api.Repartition
Imports RepartitionStrategy = org.deeplearning4j.spark.api.RepartitionStrategy
Imports org.deeplearning4j.spark.impl.common
Imports DefaultRepartitioner = org.deeplearning4j.spark.impl.repartitioner.DefaultRepartitioner
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Tuple2 = scala.Tuple2
Imports org.junit.jupiter.api.Assertions

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

Namespace org.deeplearning4j.spark.util


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class TestRepartitioning extends org.deeplearning4j.spark.BaseSparkTest
	<Serializable>
	Public Class TestRepartitioning
		Inherits BaseSparkTest

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return If(IntegrationTests, 240000, 60000)
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRepartitioning()
		Public Overridable Sub testRepartitioning()
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			Dim list As IList(Of String) = New List(Of String)()
			For i As Integer = 0 To 999
				list.Add(i.ToString())
			Next i

			Dim rdd As JavaRDD(Of String) = sc.parallelize(list)
			rdd = rdd.repartition(200)

			Dim rdd2 As JavaRDD(Of String) = SparkUtils.repartitionBalanceIfRequired(rdd, Repartition.Always, 100, 10)
			assertFalse(rdd = rdd2) 'Should be different objects due to repartitioning

			assertEquals(10, rdd2.partitions().size())
			For i As Integer = 0 To 9
				Dim partition As IList(Of String) = rdd2.collectPartitions(New Integer() {i})(0)
	'            System.out.println("Partition " + i + " size: " + partition.size());
				assertEquals(100, partition.Count) 'Should be exactly 100, for the util method (but NOT spark .repartition)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRepartitioning2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testRepartitioning2()
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			Dim ns() As Integer
			If IntegrationTests Then
				ns = New Integer(){320, 321, 25600, 25601, 25615}
			Else
				ns = New Integer(){320, 2561}
			End If

			For Each n As Integer In ns

				Dim list As IList(Of String) = New List(Of String)()
				For i As Integer = 0 To n - 1
					list.Add(i.ToString())
				Next i

				Dim rdd As JavaRDD(Of String) = sc.parallelize(list)
				rdd.repartition(65)

				Dim totalDataSetObjectCount As Integer = n
				Dim dataSetObjectsPerSplit As Integer = 8 * 4 * 10
				Dim valuesPerPartition As Integer = 10
				Dim nPartitions As Integer = 32

				Dim splits() As JavaRDD(Of String) = org.deeplearning4j.spark.util.SparkUtils.balancedRandomSplit(totalDataSetObjectCount, dataSetObjectsPerSplit, rdd, (New Random()).nextLong())

				Dim counts As IList(Of Integer) = New List(Of Integer)()
				Dim partitionCountList As IList(Of IList(Of Tuple2(Of Integer, Integer))) = New List(Of IList(Of Tuple2(Of Integer, Integer)))()
				'            System.out.println("------------------------");
				'            System.out.println("Partitions Counts:");
				For Each split As JavaRDD(Of String) In splits
					Dim repartitioned As JavaRDD(Of String) = SparkUtils.repartition(split, Repartition.Always, RepartitionStrategy.Balanced, valuesPerPartition, nPartitions)
					Dim partitionCounts As IList(Of Tuple2(Of Integer, Integer)) = repartitioned.mapPartitionsWithIndex(New CountPartitionsFunction(Of String)(), True).collect()
					'                System.out.println(partitionCounts);
					partitionCountList.Add(partitionCounts)
					counts.Add(CInt(split.count()))
				Next split

				'            System.out.println(counts.size());
				'            System.out.println(counts);


				Dim expNumPartitionsWithMore As Integer = totalDataSetObjectCount Mod nPartitions
				Dim actNumPartitionsWithMore As Integer = 0
				For Each l As IList(Of Tuple2(Of Integer, Integer)) In partitionCountList
					assertEquals(nPartitions, l.Count)

					For Each t2 As Tuple2(Of Integer, Integer) In l
						Dim partitionSize As Integer = t2._2()
						If partitionSize > valuesPerPartition Then
							actNumPartitionsWithMore += 1
						End If
					Next t2
				Next l

				assertEquals(expNumPartitionsWithMore, actNumPartitionsWithMore)
			Next n
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRepartitioning3()
		Public Overridable Sub testRepartitioning3()
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			'Initial partitions (idx, count) - [(0,29), (1,29), (2,29), (3,34), (4,34), (5,35), (6,34)]

			Dim ints As IList(Of Integer) = New List(Of Integer)()
			For i As Integer = 0 To 223
				ints.Add(i)
			Next i

			Dim rdd As JavaRDD(Of Integer) = sc.parallelize(ints)
			Dim pRdd As JavaPairRDD(Of Integer, Integer) = SparkUtils.indexedRDD(rdd)
			Dim initial As JavaPairRDD(Of Integer, Integer) = pRdd.partitionBy(New PartitionerAnonymousInnerClass(Me))

			Dim partitionCounts As IList(Of Tuple2(Of Integer, Integer)) = initial.values().mapPartitionsWithIndex(New CountPartitionsFunction(Of Integer)(), True).collect()

	'        System.out.println(partitionCounts);

			Dim initialExpected As IList(Of Tuple2(Of Integer, Integer)) = New List(Of Tuple2(Of Integer, Integer)) From {
				New Tuple2(Of Tuple2(Of Integer, Integer))(0,29),
				New Tuple2(Of )(1,29),
				New Tuple2(Of )(2,29),
				New Tuple2(Of )(3,34),
				New Tuple2(Of )(4,34),
				New Tuple2(Of )(5,35),
				New Tuple2(Of )(6,34)
			}
			assertEquals(initialExpected, partitionCounts)


			Dim afterRepartition As JavaRDD(Of Integer) = SparkUtils.repartitionBalanceIfRequired(initial.values(), Repartition.Always, 2, 112)
			Dim partitionCountsAfter As IList(Of Tuple2(Of Integer, Integer)) = afterRepartition.mapPartitionsWithIndex(New CountPartitionsFunction(Of Integer)(), True).collect()
	'        System.out.println(partitionCountsAfter);

			For Each t2 As Tuple2(Of Integer, Integer) In partitionCountsAfter
				assertEquals(2, CInt(Math.Truncate(t2._2())))
			Next t2
		End Sub

		Private Class PartitionerAnonymousInnerClass
			Inherits Partitioner

			Private ReadOnly outerInstance As TestRepartitioning

			Public Sub New(ByVal outerInstance As TestRepartitioning)
				Me.outerInstance = outerInstance
			End Sub

			Public Overrides Function getPartition(ByVal key As Object) As Integer
				Dim i As Integer = DirectCast(key, Integer?)
				If i < 29 Then
					Return 0
				ElseIf i < 29+29 Then
					Return 1
				ElseIf i < 29+29+29 Then
					Return 2
				ElseIf i < 29+29+29+34 Then
					Return 3
				ElseIf i < 29+29+29+34+34 Then
					Return 4
				ElseIf i < 29+29+29+34+34+35 Then
					Return 5
				Else
					Return 6
				End If
			End Function
			Public Overrides Function numPartitions() As Integer
				Return 7
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRepartitioning4()
		Public Overridable Sub testRepartitioning4()
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			Dim ints As IList(Of Integer) = New List(Of Integer)()
			For i As Integer = 0 To 7039
				ints.Add(i)
			Next i

			Dim rdd As JavaRDD(Of Integer) = sc.parallelize(ints)

			Dim afterRepartition As JavaRDD(Of Integer) = (New DefaultRepartitioner()).repartition(rdd, 1, 32)
			Dim partitionCountsAfter As IList(Of Tuple2(Of Integer, Integer)) = afterRepartition.mapPartitionsWithIndex(New CountPartitionsFunction(Of Integer)(), True).collect()

			Dim min As Integer = Integer.MaxValue
			Dim max As Integer = Integer.MinValue
			Dim minIdx As Integer = 0
			Dim maxIdx As Integer = 0
			For Each t2 As Tuple2(Of Integer, Integer) In partitionCountsAfter
				min = Math.Min(min, t2._2())
				max = Math.Max(max, t2._2())
				If min = t2._2() Then
					minIdx = t2._1()
				End If
				If max = t2._2() Then
					maxIdx = t2._1()
				End If
			Next t2

	'        System.out.println("min: " + min + "\t@\t" + minIdx);
	'        System.out.println("max: " + max + "\t@\t" + maxIdx);

			assertEquals(1, min)
			assertEquals(2, max)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRepartitioningApprox()
		Public Overridable Sub testRepartitioningApprox()
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			Dim list As IList(Of String) = New List(Of String)()
			For i As Integer = 0 To 999
				list.Add(i.ToString())
			Next i

			Dim rdd As JavaRDD(Of String) = sc.parallelize(list)
			rdd = rdd.repartition(200)

			Dim rdd2 As JavaRDD(Of String) = SparkUtils.repartitionApproximateBalance(rdd, Repartition.Always, 10)
			assertFalse(rdd = rdd2) 'Should be different objects due to repartitioning

			assertEquals(10, rdd2.partitions().size())

			For i As Integer = 0 To 9
				Dim partition As IList(Of String) = rdd2.collectPartitions(New Integer() {i})(0)
	'            System.out.println("Partition " + i + " size: " + partition.size());
				assertTrue(partition.Count >= 90 AndAlso partition.Count <= 110)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRepartitioningApproxReverse()
		Public Overridable Sub testRepartitioningApproxReverse()
			Dim list As IList(Of String) = New List(Of String)()
			For i As Integer = 0 To 999
				list.Add(i.ToString())
			Next i

			' initial # of partitions = cores, probably < 100
			Dim rdd As JavaRDD(Of String) = sc.parallelize(list)

			Dim rdd2 As JavaRDD(Of String) = SparkUtils.repartitionApproximateBalance(rdd, Repartition.Always, 100)
			assertFalse(rdd = rdd2) 'Should be different objects due to repartitioning

			assertEquals(100, rdd2.partitions().size())
		End Sub


	End Class

End Namespace