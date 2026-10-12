Imports System
Imports System.Collections.Generic
Imports StandardDeviation = org.apache.commons.math3.stat.descriptive.moment.StandardDeviation
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports Column = org.apache.spark.sql.Column
Imports Dataset = org.apache.spark.sql.Dataset
Imports Row = org.apache.spark.sql.Row
Imports Schema = org.datavec.api.transform.schema.Schema
Imports RecordConverter = org.datavec.api.util.ndarray.RecordConverter
Imports DoubleWritable = org.datavec.api.writable.DoubleWritable
Imports Writable = org.datavec.api.writable.Writable
Imports BaseSparkTest = org.datavec.spark.BaseSparkTest
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.junit.jupiter.api.Assertions.assertEquals

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

Namespace org.datavec.spark.transform


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) @Tag(TagNames.LONG_TEST) @Tag(TagNames.LARGE_RESOURCES) public class DataFramesTests extends org.datavec.spark.BaseSparkTest
	<Serializable>
	Public Class DataFramesTests
		Inherits BaseSparkTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMinMax()
		Public Overridable Sub testMinMax()
			Dim arr As INDArray = Nd4j.linspace(1, 10, 10).broadcast(10, 10)
			Dim i As Integer = 0
			Do While i < arr.rows()
				arr.getRow(i).addi(i)
				i += 1
			Loop

			Dim records As IList(Of IList(Of Writable)) = RecordConverter.toRecords(arr)
			Dim builder As New Schema.Builder()
			Dim numColumns As Integer = 10
			For i As Integer = 0 To numColumns - 1
				builder.addColumnDouble(i.ToString())
			Next i
			Dim schema As Schema = builder.build()
			Dim dataFrame As Dataset(Of Row) = DataFrames.toDataFrame(schema, sc.parallelize(records))
			dataFrame.show()
			dataFrame.describe(DataFrames.toArray(schema.getColumnNames())).show()
			'        System.out.println(Normalization.minMaxColumns(dataFrame,schema.getColumnNames()));
			'        System.out.println(Normalization.stdDevMeanColumns(dataFrame,schema.getColumnNames()));

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDataFrameConversions()
		Public Overridable Sub testDataFrameConversions()
			Dim data As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			Dim builder As New Schema.Builder()
			Dim numColumns As Integer = 6
			For i As Integer = 0 To numColumns - 1
				builder.addColumnDouble(i.ToString())
			Next i

			For i As Integer = 0 To 4
				Dim record As IList(Of Writable) = New List(Of Writable)(numColumns)
				data.Add(record)
				For j As Integer = 0 To numColumns - 1
					record.Add(New DoubleWritable(1.0))
				Next j

			Next i

			Dim schema As Schema = builder.build()
			Dim rdd As JavaRDD(Of IList(Of Writable)) = sc.parallelize(data)
			assertEquals(schema, DataFrames.fromStructType(DataFrames.fromSchema(schema)))
			assertEquals(rdd.collect(), DataFrames.toRecords(DataFrames.toDataFrame(schema, rdd)).Second.collect())

			Dim dataFrame As Dataset(Of Row) = DataFrames.toDataFrame(schema, rdd)
			dataFrame.show()
			Dim mean As Column = DataFrames.mean(dataFrame, "0")
			Dim std As Column = DataFrames.std(dataFrame, "0")
			dataFrame.withColumn("0", dataFrame.col("0").minus(mean)).show()
			dataFrame.withColumn("0", dataFrame.col("0").divide(std)).show()

	'           DataFrame desc = dataFrame.describe(dataFrame.columns());
	'        dataFrame.show();
	'        System.out.println(dataFrame.agg(avg("0"), dataFrame.col("0")));
	'        dataFrame.withColumn("0",dataFrame.col("0").minus(avg(dataFrame.col("0"))));
	'        dataFrame.show();
	'        
	'        
	'        for(String column : dataFrame.columns()) {
	'            System.out.println(DataFrames.mean(desc,column));
	'            System.out.println(DataFrames.min(desc,column));
	'            System.out.println(DataFrames.max(desc,column));
	'            System.out.println(DataFrames.std(desc,column));
	'        
	'        }
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testNormalize()
		Public Overridable Sub testNormalize()
			Dim data As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()

			data.Add(New List(Of Writable) From {
				New DoubleWritable(1),
				New DoubleWritable(10)
			})
			data.Add(New List(Of Writable) From {
				New DoubleWritable(2),
				New DoubleWritable(20)
			})
			data.Add(New List(Of Writable) From {
				New DoubleWritable(3),
				New DoubleWritable(30)
			})


			Dim expMinMax As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			expMinMax.Add(New List(Of Writable) From {
				New DoubleWritable(0.0),
				New DoubleWritable(0.0)
			})
			expMinMax.Add(New List(Of Writable) From {
				New DoubleWritable(0.5),
				New DoubleWritable(0.5)
			})
			expMinMax.Add(New List(Of Writable) From {
				New DoubleWritable(1.0),
				New DoubleWritable(1.0)
			})

			Dim m1 As Double = (1 + 2 + 3) / 3.0
			Dim s1 As Double = (New StandardDeviation()).evaluate(New Double() {1, 2, 3})
			Dim m2 As Double = (10 + 20 + 30) / 3.0
			Dim s2 As Double = (New StandardDeviation()).evaluate(New Double() {10, 20, 30})

			Dim expStandardize As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			expStandardize.Add(New List(Of Writable) From {
				New DoubleWritable((1 - m1) / s1),
				New DoubleWritable((10 - m2) / s2)
			})
			expStandardize.Add(New List(Of Writable) From {
				New DoubleWritable((2 - m1) / s1),
				New DoubleWritable((20 - m2) / s2)
			})
			expStandardize.Add(New List(Of Writable) From {
				New DoubleWritable((3 - m1) / s1),
				New DoubleWritable((30 - m2) / s2)
			})

			Dim rdd As JavaRDD(Of IList(Of Writable)) = sc.parallelize(data)

			Dim schema As Schema = (New Schema.Builder()).addColumnInteger("c0").addColumnDouble("c1").build()


			Dim normalized As JavaRDD(Of IList(Of Writable)) = Normalization.normalize(schema, rdd)
			Dim standardize As JavaRDD(Of IList(Of Writable)) = Normalization.zeromeanUnitVariance(schema, rdd)

			Dim comparator As IComparer(Of IList(Of Writable)) = New ComparatorFirstCol()

			Dim c As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))(normalized.collect())
			Dim c2 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))(standardize.collect())
			c.Sort(comparator)
			c2.Sort(comparator)

			'        System.out.println("Normalized:");
			'        System.out.println(c);
			'        System.out.println("Standardized:");
			'        System.out.println(c2);

			For i As Integer = 0 To expMinMax.Count - 1
				Dim exp As IList(Of Writable) = expMinMax(i)
				Dim act As IList(Of Writable) = c(i)

				For j As Integer = 0 To exp.Count - 1
					assertEquals(exp(j).toDouble(), act(j).toDouble(), 1e-6)
				Next j
			Next i

			For i As Integer = 0 To expStandardize.Count - 1
				Dim exp As IList(Of Writable) = expStandardize(i)
				Dim act As IList(Of Writable) = c2(i)

				For j As Integer = 0 To exp.Count - 1
					assertEquals(exp(j).toDouble(), act(j).toDouble(), 1e-6)
				Next j
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDataFrameSequenceNormalization()
		Public Overridable Sub testDataFrameSequenceNormalization()
			Dim sequences As IList(Of IList(Of IList(Of Writable))) = New List(Of IList(Of IList(Of Writable)))()

			Dim seq1 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			seq1.Add(New List(Of Writable) From {
				New DoubleWritable(1),
				New DoubleWritable(10),
				New DoubleWritable(100)
			})
			seq1.Add(New List(Of Writable) From {
				New DoubleWritable(2),
				New DoubleWritable(20),
				New DoubleWritable(200)
			})
			seq1.Add(New List(Of Writable) From {
				New DoubleWritable(3),
				New DoubleWritable(30),
				New DoubleWritable(300)
			})

			Dim seq2 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			seq2.Add(New List(Of Writable) From {
				New DoubleWritable(4),
				New DoubleWritable(40),
				New DoubleWritable(400)
			})
			seq2.Add(New List(Of Writable) From {
				New DoubleWritable(5),
				New DoubleWritable(50),
				New DoubleWritable(500)
			})

			sequences.Add(seq1)
			sequences.Add(seq2)

			Dim schema As Schema = (New Schema.Builder()).addColumnInteger("c0").addColumnDouble("c1").addColumnLong("c2").build()

			Dim rdd As JavaRDD(Of IList(Of IList(Of Writable))) = sc.parallelize(sequences)

			Dim normalized As JavaRDD(Of IList(Of IList(Of Writable))) = Normalization.normalizeSequence(schema, rdd, 0, 1)
			Dim standardized As JavaRDD(Of IList(Of IList(Of Writable))) = Normalization.zeroMeanUnitVarianceSequence(schema, rdd)


			'Min/max normalization:
			Dim expSeq1MinMax As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			expSeq1MinMax.Add(New List(Of Writable) From {
				New DoubleWritable((1 - 1.0) / (5.0 - 1.0)),
				New DoubleWritable((10 - 10.0) / (50.0 - 10.0)),
				New DoubleWritable((100 - 100.0) / (500.0 - 100.0))
			})
			expSeq1MinMax.Add(New List(Of Writable) From {
				New DoubleWritable((2 - 1.0) / (5.0 - 1.0)),
				New DoubleWritable((20 - 10.0) / (50.0 - 10.0)),
				New DoubleWritable((200 - 100.0) / (500.0 - 100.0))
			})
			expSeq1MinMax.Add(New List(Of Writable) From {
				New DoubleWritable((3 - 1.0) / (5.0 - 1.0)),
				New DoubleWritable((30 - 10.0) / (50.0 - 10.0)),
				New DoubleWritable((300 - 100.0) / (500.0 - 100.0))
			})

			Dim expSeq2MinMax As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			expSeq2MinMax.Add(New List(Of Writable) From {
				New DoubleWritable((4 - 1.0) / (5.0 - 1.0)),
				New DoubleWritable((40 - 10.0) / (50.0 - 10.0)),
				New DoubleWritable((400 - 100.0) / (500.0 - 100.0))
			})
			expSeq2MinMax.Add(New List(Of Writable) From {
				New DoubleWritable((5 - 1.0) / (5.0 - 1.0)),
				New DoubleWritable((50 - 10.0) / (50.0 - 10.0)),
				New DoubleWritable((500 - 100.0) / (500.0 - 100.0))
			})


			Dim norm As IList(Of IList(Of IList(Of Writable))) = New List(Of IList(Of IList(Of Writable)))(normalized.collect())
			norm.Sort(New ComparatorSeqLength())
			assertEquals(2, norm.Count)

			'        System.out.println(norm);

			For i As Integer = 0 To 1
				Dim seqExp As IList(Of IList(Of Writable)) = (If(i = 0, expSeq1MinMax, expSeq2MinMax))
				For j As Integer = 0 To seqExp.Count - 1
					Dim stepExp As IList(Of Writable) = seqExp(j)
					Dim stepAct As IList(Of Writable) = norm(i)(j)
					For k As Integer = 0 To stepExp.Count - 1
						assertEquals(stepExp(k).toDouble(), stepAct(k).toDouble(), 1e-6)
					Next k
				Next j
			Next i



			'Standardize:
			Dim m1 As Double = (1 + 2 + 3 + 4 + 5) / 5.0
			Dim s1 As Double = (New StandardDeviation()).evaluate(New Double() {1, 2, 3, 4, 5})
			Dim m2 As Double = (10 + 20 + 30 + 40 + 50) / 5.0
			Dim s2 As Double = (New StandardDeviation()).evaluate(New Double() {10, 20, 30, 40, 50})
			Dim m3 As Double = (100 + 200 + 300 + 400 + 500) / 5.0
			Dim s3 As Double = (New StandardDeviation()).evaluate(New Double() {100, 200, 300, 400, 500})

			Dim expSeq1Std As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			expSeq1Std.Add(New List(Of Writable) From {
				New DoubleWritable((1 - m1) / s1),
				New DoubleWritable((10 - m2) / s2),
				New DoubleWritable((100 - m3) / s3)
			})
			expSeq1Std.Add(New List(Of Writable) From {
				New DoubleWritable((2 - m1) / s1),
				New DoubleWritable((20 - m2) / s2),
				New DoubleWritable((200 - m3) / s3)
			})
			expSeq1Std.Add(New List(Of Writable) From {
				New DoubleWritable((3 - m1) / s1),
				New DoubleWritable((30 - m2) / s2),
				New DoubleWritable((300 - m3) / s3)
			})

			Dim expSeq2Std As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			expSeq2Std.Add(New List(Of Writable) From {
				New DoubleWritable((4 - m1) / s1),
				New DoubleWritable((40 - m2) / s2),
				New DoubleWritable((400 - m3) / s3)
			})
			expSeq2Std.Add(New List(Of Writable) From {
				New DoubleWritable((5 - m1) / s1),
				New DoubleWritable((50 - m2) / s2),
				New DoubleWritable((500 - m3) / s3)
			})


			Dim std As IList(Of IList(Of IList(Of Writable))) = New List(Of IList(Of IList(Of Writable)))(standardized.collect())
			std.Sort(New ComparatorSeqLength())
			assertEquals(2, std.Count)

			'        System.out.println(std);

			For i As Integer = 0 To 1
				Dim seqExp As IList(Of IList(Of Writable)) = (If(i = 0, expSeq1Std, expSeq2Std))
				For j As Integer = 0 To seqExp.Count - 1
					Dim stepExp As IList(Of Writable) = seqExp(j)
					Dim stepAct As IList(Of Writable) = std(i)(j)
					For k As Integer = 0 To stepExp.Count - 1
						assertEquals(stepExp(k).toDouble(), stepAct(k).toDouble(), 1e-6)
					Next k
				Next j
			Next i
		End Sub


		Private Class ComparatorFirstCol
			Implements IComparer(Of IList(Of Writable))

			Public Overridable Function Compare(ByVal o1 As IList(Of Writable), ByVal o2 As IList(Of Writable)) As Integer Implements IComparer(Of IList(Of Writable)).Compare
				Return Integer.compare(o1(0).toInt(), o2(0).toInt())
			End Function
		End Class

		Private Class ComparatorSeqLength
			Implements IComparer(Of IList(Of IList(Of Writable)))

			Public Overridable Function Compare(ByVal o1 As IList(Of IList(Of Writable)), ByVal o2 As IList(Of IList(Of Writable))) As Integer Implements IComparer(Of IList(Of IList(Of Writable))).Compare
				Return -Integer.compare(o1.Count, o2.Count)
			End Function
		End Class
	End Class

End Namespace