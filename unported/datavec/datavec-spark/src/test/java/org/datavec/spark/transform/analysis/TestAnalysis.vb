Imports System
Imports System.Collections.Generic
Imports TDigest = com.tdunning.math.stats.TDigest
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports StatCounter = org.apache.spark.util.StatCounter
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports CSVRecordReader = org.datavec.api.records.reader.impl.csv.CSVRecordReader
Imports FileSplit = org.datavec.api.split.FileSplit
Imports DataAnalysis = org.datavec.api.transform.analysis.DataAnalysis
Imports org.datavec.api.transform.analysis.columns
Imports BinaryMetaData = org.datavec.api.transform.metadata.BinaryMetaData
Imports BooleanMetaData = org.datavec.api.transform.metadata.BooleanMetaData
Imports DataQualityAnalysis = org.datavec.api.transform.quality.DataQualityAnalysis
Imports Schema = org.datavec.api.transform.schema.Schema
Imports HtmlAnalysis = org.datavec.api.transform.ui.HtmlAnalysis
Imports org.datavec.api.writable
Imports AnalyzeLocal = org.datavec.local.transforms.AnalyzeLocal
Imports BaseSparkTest = org.datavec.spark.BaseSparkTest
Imports AnalyzeSpark = org.datavec.spark.transform.AnalyzeSpark
Imports DateTimeZone = org.joda.time.DateTimeZone
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
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

Namespace org.datavec.spark.transform.analysis


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) public class TestAnalysis extends org.datavec.spark.BaseSparkTest
	<Serializable>
	Public Class TestAnalysis
		Inherits BaseSparkTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testAnalysis() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testAnalysis()

			Dim schema As Schema = (New Schema.Builder()).addColumnInteger("intCol").addColumnDouble("doubleCol").addColumnTime("timeCol", DateTimeZone.UTC).addColumnCategorical("catCol", "A", "B").addColumnNDArray("ndarray", New Long() {1, 10}).build()

			Dim data As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			data.Add(New List(Of Writable) From {DirectCast(New IntWritable(0), Writable), New DoubleWritable(1.0), New LongWritable(1000), New Text("A"), New NDArrayWritable(Nd4j.valueArrayOf(1, 10, 100.0))})
			data.Add(New List(Of Writable) From {DirectCast(New IntWritable(5), Writable), New DoubleWritable(0.0), New LongWritable(2000), New Text("A"), New NDArrayWritable(Nd4j.valueArrayOf(1, 10, 200.0))})
			data.Add(New List(Of Writable) From {DirectCast(New IntWritable(3), Writable), New DoubleWritable(10.0), New LongWritable(3000), New Text("A"), New NDArrayWritable(Nd4j.valueArrayOf(1, 10, 300.0))})
			data.Add(New List(Of Writable) From {DirectCast(New IntWritable(-1), Writable), New DoubleWritable(-1.0), New LongWritable(20000), New Text("B"), New NDArrayWritable(Nd4j.valueArrayOf(1, 10, 400.0))})

			Dim rdd As JavaRDD(Of IList(Of Writable)) = sc.parallelize(data)

			Dim da As DataAnalysis = AnalyzeSpark.analyze(schema, rdd)
			Dim daString As String = da.ToString()

	'        System.out.println(da);
			da.toJson()
			da.ToString()

			Dim ca As IList(Of ColumnAnalysis) = da.getColumnAnalysis()
			assertEquals(5, ca.Count)

			assertTrue(TypeOf ca(0) Is IntegerAnalysis)
			assertTrue(TypeOf ca(1) Is DoubleAnalysis)
			assertTrue(TypeOf ca(2) Is TimeAnalysis)
			assertTrue(TypeOf ca(3) Is CategoricalAnalysis)
			assertTrue(TypeOf ca(4) Is NDArrayAnalysis)

			Dim ia As IntegerAnalysis = DirectCast(ca(0), IntegerAnalysis)
			assertEquals(-1, ia.getMin())
			assertEquals(5, ia.getMax())
			assertEquals(4, ia.CountTotal)
			Dim itd As TDigest = ia.getDigest()
			assertEquals(-0.5, itd.quantile(0.25), 1e-9) ' right-biased linear approximations w/ few points
			assertEquals(1.5, itd.quantile(0.5), 1e-9)
			assertEquals(4.0, itd.quantile(0.75), 1e-9)
			assertEquals(5.0, itd.quantile(1), 1e-9)

			Dim dba As DoubleAnalysis = DirectCast(ca(1), DoubleAnalysis)
			assertEquals(-1.0, dba.getMin(), 0.0)
			assertEquals(10.0, dba.getMax(), 0.0)
			assertEquals(4, dba.CountTotal)
			Dim dtd As TDigest = dba.getDigest()
			assertEquals(-0.5, dtd.quantile(0.25), 1e-9) ' right-biased linear approximations w/ few points
			assertEquals(0.5, dtd.quantile(0.5), 1e-9)
			assertEquals(5.5, dtd.quantile(0.75), 1e-9)
			assertEquals(10.0, dtd.quantile(1), 1e-9)


			Dim ta As TimeAnalysis = DirectCast(ca(2), TimeAnalysis)
			assertEquals(1000, ta.getMin())
			assertEquals(20000, ta.getMax())
			assertEquals(4, ta.CountTotal)
			Dim ttd As TDigest = ta.getDigest()
			assertEquals(1500.0, ttd.quantile(0.25), 1e-9) ' right-biased linear approximations w/ few points
			assertEquals(2500.0, ttd.quantile(0.5), 1e-9)
			assertEquals(11500.0, ttd.quantile(0.75), 1e-9)
			assertEquals(20000.0, ttd.quantile(1), 1e-9)

			Dim cata As CategoricalAnalysis = DirectCast(ca(3), CategoricalAnalysis)
			Dim map As IDictionary(Of String, Long) = cata.getMapOfCounts()
			assertEquals(2, map.Keys.Count)
			assertEquals(3L, CLng(map("A")))
			assertEquals(1L, CLng(map("B")))

			Dim na As NDArrayAnalysis = DirectCast(ca(4), NDArrayAnalysis)
			assertEquals(4, na.CountTotal)
			assertEquals(0, na.getCountNull())
			assertEquals(10, na.getMinLength())
			assertEquals(10, na.getMaxLength())
			assertEquals(4 * 10, na.getTotalNDArrayValues())
			assertEquals(Collections.singletonMap(2, 4L), na.getCountsByRank())
			assertEquals(100.0, na.getMinValue(), 0.0)
			assertEquals(400.0, na.getMaxValue(), 0.0)

			assertNotNull(ia.getHistogramBuckets())
			assertNotNull(ia.getHistogramBucketCounts())

			assertNotNull(dba.getHistogramBuckets())
			assertNotNull(dba.getHistogramBucketCounts())

			assertNotNull(ta.getHistogramBuckets())
			assertNotNull(ta.getHistogramBucketCounts())

			assertNotNull(na.getHistogramBuckets())
			assertNotNull(na.getHistogramBucketCounts())

			Dim bucketsD() As Double = dba.getHistogramBuckets()
			Dim countD() As Long = dba.getHistogramBucketCounts()

			assertEquals(-1.0, bucketsD(0), 0.0)
			assertEquals(10.0, bucketsD(bucketsD.Length - 1), 0.0)
			assertEquals(1, countD(0))
			assertEquals(1, countD(countD.Length - 1))

			Dim f As File = Files.createTempFile("datavec_spark_analysis_UITest", ".html").toFile()
	'        System.out.println(f.getAbsolutePath());
			f.deleteOnExit()
			HtmlAnalysis.createHtmlAnalysisFile(da, f)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testAnalysisStdev()
		Public Overridable Sub testAnalysisStdev()
			'Test stdev calculations compared to Spark's stats calculation


			Dim r As New Random(12345)
			Dim l1 As IList(Of Double) = New List(Of Double)()
			Dim l2 As IList(Of Integer) = New List(Of Integer)()
			Dim l3 As IList(Of Long) = New List(Of Long)()

			Dim n As Integer = 10000
			For i As Integer = 0 To n - 1
				l1.Add(10 * r.NextDouble())
				l2.Add(-1000 + r.Next(2000))
				l3.Add(-1000L + r.Next(2000))
			Next i

			Dim l2d As IList(Of Double) = New List(Of Double)()
			For Each i As Integer? In l2
				l2d.Add(i.Value)
			Next i
			Dim l3d As IList(Of Double) = New List(Of Double)()
			For Each l As Long? In l3
				l3d.Add(l.Value)
			Next l


			Dim sc1 As StatCounter = sc.parallelizeDoubles(l1).stats()
			Dim sc2 As StatCounter = sc.parallelizeDoubles(l2d).stats()
			Dim sc3 As StatCounter = sc.parallelizeDoubles(l3d).stats()

			Dim sc1new As New org.datavec.api.transform.analysis.counter.StatCounter()
			For Each d As Double In l1
				sc1new.add(d)
			Next d

			assertEquals(sc1.sampleStdev(), sc1new.getStddev(False), 1e-6)


			Dim sparkCounters As IList(Of StatCounter) = New List(Of StatCounter)()
			Dim counters As IList(Of org.datavec.api.transform.analysis.counter.StatCounter) = New List(Of org.datavec.api.transform.analysis.counter.StatCounter)()
			For i As Integer = 0 To 9
				counters.Add(New org.datavec.api.transform.analysis.counter.StatCounter())
				sparkCounters.Add(New StatCounter())
			Next i
			For i As Integer = 0 To l1.Count - 1
				Dim idx As Integer = i Mod 10
				counters(idx).add(l1(i))
				sparkCounters(idx).merge(l1(i))
			Next i
			Dim counter As org.datavec.api.transform.analysis.counter.StatCounter = counters(0)
			Dim sparkCounter As StatCounter = sparkCounters(0)
			For i As Integer = 1 To 9
				counter.merge(counters(i))
				sparkCounter.merge(sparkCounters(i))
	'            System.out.println();
			Next i
			assertEquals(sc1.sampleStdev(), counter.getStddev(False), 1e-6)
			assertEquals(sparkCounter.sampleStdev(), counter.getStddev(False), 1e-6)

			Dim data As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			For i As Integer = 0 To n - 1
				Dim l As IList(Of Writable) = New List(Of Writable)()
				l.Add(New DoubleWritable(l1(i)))
				l.Add(New IntWritable(l2(i)))
				l.Add(New LongWritable(l3(i)))
				data.Add(l)
			Next i

			Dim schema As Schema = (New Schema.Builder()).addColumnDouble("d").addColumnInteger("i").addColumnLong("l").build()

			Dim rdd As JavaRDD(Of IList(Of Writable)) = sc.parallelize(data)
			Dim da As DataAnalysis = AnalyzeSpark.analyze(schema, rdd)

			Dim stdev1 As Double = sc1.sampleStdev()
			Dim stdev1a As Double = DirectCast(da.getColumnAnalysis("d"), DoubleAnalysis).getSampleStdev()
			Dim re1 As Double = Math.Abs(stdev1 - stdev1a) / (Math.Abs(stdev1) + Math.Abs(stdev1a))
			assertTrue(re1 < 1e-6)

			Dim stdev2 As Double = sc2.sampleStdev()
			Dim stdev2a As Double = DirectCast(da.getColumnAnalysis("i"), IntegerAnalysis).getSampleStdev()
			Dim re2 As Double = Math.Abs(stdev2 - stdev2a) / (Math.Abs(stdev2) + Math.Abs(stdev2a))
			assertTrue(re2 < 1e-6)

			Dim stdev3 As Double = sc3.sampleStdev()
			Dim stdev3a As Double = DirectCast(da.getColumnAnalysis("l"), LongAnalysis).getSampleStdev()
			Dim re3 As Double = Math.Abs(stdev3 - stdev3a) / (Math.Abs(stdev3) + Math.Abs(stdev3a))
			assertTrue(re3 < 1e-6)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSampleMostFrequent()
		Public Overridable Sub testSampleMostFrequent()

			Dim toParallelize As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			toParallelize.Add(New List(Of Writable) From {Of Writable})
			toParallelize.Add(New List(Of Writable) From {Of Writable})
			toParallelize.Add(New List(Of Writable) From {Of Writable})
			toParallelize.Add(New List(Of Writable) From {Of Writable})
			toParallelize.Add(New List(Of Writable) From {Of Writable})
			toParallelize.Add(New List(Of Writable) From {Of Writable})
			toParallelize.Add(New List(Of Writable) From {Of Writable})
			toParallelize.Add(New List(Of Writable) From {Of Writable})
			toParallelize.Add(New List(Of Writable) From {Of Writable})
			toParallelize.Add(New List(Of Writable) From {Of Writable})
			toParallelize.Add(New List(Of Writable) From {Of Writable})
			toParallelize.Add(New List(Of Writable) From {Of Writable})
			toParallelize.Add(New List(Of Writable) From {Of Writable})
			toParallelize.Add(New List(Of Writable) From {Of Writable})
			toParallelize.Add(New List(Of Writable) From {Of Writable})


			Dim rdd As JavaRDD(Of IList(Of Writable)) = sc.parallelize(toParallelize)

			Dim schema As Schema = (New Schema.Builder()).addColumnsString("irrelevant", "column").build()

			Dim map As IDictionary(Of Writable, Long) = AnalyzeSpark.sampleMostFrequentFromColumn(3, "column", schema, rdd)

			'        System.out.println(map);

			assertEquals(3, map.Count)
			assertEquals(4L, CLng(map(New Text("MostCommon"))))
			assertEquals(3L, CLng(map(New Text("SecondMostCommon"))))
			assertEquals(2L, CLng(map(New Text("ThirdMostCommon"))))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testAnalysisVsLocal() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testAnalysisVsLocal()

			Dim s As Schema = (New Schema.Builder()).addColumnsDouble("%d", 0, 3).addColumnInteger("label").build()

			Dim rr As RecordReader = New CSVRecordReader()
			rr.initialize(New org.datavec.api.Split.FileSplit((New ClassPathResource("iris.txt")).File))

			Dim toParallelize As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			Do While rr.hasNext()
				toParallelize.Add(rr.next())
			Loop

			Dim rdd As JavaRDD(Of IList(Of Writable)) = sc.parallelize(toParallelize).coalesce(1)


			rr.reset()
			Dim local As DataAnalysis = AnalyzeLocal.analyze(s, rr)
			Dim spark As DataAnalysis = AnalyzeSpark.analyze(s, rdd)

	'        assertEquals(local.toJson(), spark.toJson());
			assertEquals(local, spark)


			'Also quality analysis:
			rr.reset()
			Dim localQ As DataQualityAnalysis = AnalyzeLocal.analyzeQuality(s, rr)
			Dim sparkQ As DataQualityAnalysis = AnalyzeSpark.analyzeQuality(s, rdd)

			assertEquals(localQ, sparkQ)


			'And, check unique etc:
			rr.reset()
			Dim mapLocal As IDictionary(Of String, ISet(Of Writable)) = AnalyzeLocal.getUnique(s.getColumnNames(), s, rr)
			Dim mapSpark As IDictionary(Of String, IList(Of Writable)) = AnalyzeSpark.getUnique(s.getColumnNames(), s, rdd)

			assertEquals(mapLocal.Keys, mapSpark.Keys)
			For Each k As String In mapLocal.Keys
				assertEquals(mapLocal(k), New HashSet(Of Writable)(mapSpark(k)))
			Next k
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testAnalysisAllColTypes()
		Public Overridable Sub testAnalysisAllColTypes()

			Dim s As Schema = (New Schema.Builder()).addColumn(New BinaryMetaData("binary")).addColumn(New BooleanMetaData("boolean")).addColumnCategorical("categorical", "a", "b").addColumnDouble("double").addColumnFloat("float").addColumnInteger("integer").addColumnLong("long").addColumnNDArray("ndarray", New Long(){1, 4}).addColumnString("string").addColumnTime("time", TimeZone.getDefault()).build()

			Dim data As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(New BytesWritable(New SByte(2){}), New BooleanWritable(True), New Text("a"), New DoubleWritable(1.0), New FloatWritable(1.0f), New IntWritable(1), New LongWritable(1L), New NDArrayWritable(Nd4j.create(DataType.FLOAT, 1, 4)), New Text("text"), New LongWritable(100L)), Arrays.asList(New BytesWritable(New SByte(2){}), New BooleanWritable(False), New Text("b"), New DoubleWritable(0.0), New FloatWritable(0.0f), New IntWritable(0), New LongWritable(0L), New NDArrayWritable(Nd4j.create(DataType.FLOAT, 1, 4)), New Text("text2"), New LongWritable(101L))}

			Dim rdd As JavaRDD(Of IList(Of Writable)) = sc.parallelize(data)
			Dim da As DataAnalysis = AnalyzeSpark.analyze(s, rdd)
	'        System.out.println(da);
			da.ToString()
			da.toJson()
		End Sub

	End Class

End Namespace