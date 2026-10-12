Imports System
Imports System.Collections.Generic
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports TransformProcess = org.datavec.api.transform.TransformProcess
Imports Schema = org.datavec.api.transform.schema.Schema
Imports NumericalColumnComparator = org.datavec.api.transform.sequence.comparator.NumericalColumnComparator
Imports LongWritable = org.datavec.api.writable.LongWritable
Imports Text = org.datavec.api.writable.Text
Imports Writable = org.datavec.api.writable.Writable
Imports BaseSparkTest = org.datavec.spark.BaseSparkTest
Imports SparkTransformExecutor = org.datavec.spark.transform.SparkTransformExecutor
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TagNames = org.nd4j.common.tests.tags.TagNames
import static org.junit.jupiter.api.Assertions.assertEquals
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

Namespace org.datavec.spark.transform.sequence


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) public class TestConvertToSequence extends org.datavec.spark.BaseSparkTest
	<Serializable>
	Public Class TestConvertToSequence
		Inherits BaseSparkTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testConvertToSequenceCompoundKey()
		Public Overridable Sub testConvertToSequenceCompoundKey()

			Dim s As Schema = (New Schema.Builder()).addColumnsString("key1", "key2").addColumnLong("time").build()

			Dim allExamples As IList(Of IList(Of Writable))
			allExamples = New List(Of IList(Of Writable)) From {Arrays.asList(New Text("k1a"), New Text("k2a"), New LongWritable(10)), Arrays.asList(New Text("k1b"), New Text("k2b"), New LongWritable(10)), Arrays.asList(New Text("k1a"), New Text("k2a"), New LongWritable(-10)), Arrays.asList(New Text("k1b"), New Text("k2b"), New LongWritable(5)), Arrays.asList(New Text("k1a"), New Text("k2a"), New LongWritable(0))}

			Dim tp As TransformProcess = (New TransformProcess.Builder(s)).convertToSequence(Arrays.asList("key1", "key2"), New NumericalColumnComparator("time")).build()

			Dim rdd As JavaRDD(Of IList(Of Writable)) = sc.parallelize(allExamples)

			Dim [out] As IList(Of IList(Of IList(Of Writable))) = SparkTransformExecutor.executeToSequence(rdd, tp).collect()

			assertEquals(2, [out].Count)
			Dim seq0 As IList(Of IList(Of Writable))
			Dim seq1 As IList(Of IList(Of Writable))
			If [out](0).Count = 3 Then
				seq0 = [out](0)
				seq1 = [out](1)
			Else
				seq0 = [out](1)
				seq1 = [out](0)
			End If

			Dim expSeq0 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(New Text("k1a"), New Text("k2a"), New LongWritable(-10)), Arrays.asList(New Text("k1a"), New Text("k2a"), New LongWritable(0)), Arrays.asList(New Text("k1a"), New Text("k2a"), New LongWritable(10))}

			Dim expSeq1 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(New Text("k1b"), New Text("k2b"), New LongWritable(5)), Arrays.asList(New Text("k1b"), New Text("k2b"), New LongWritable(10))}

			assertEquals(expSeq0, seq0)
			assertEquals(expSeq1, seq1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testConvertToSequenceLength1()
		Public Overridable Sub testConvertToSequenceLength1()

			Dim s As Schema = (New Schema.Builder()).addColumnsString("string").addColumnLong("long").build()

			Dim allExamples As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(New Text("a"), New LongWritable(0)), Arrays.asList(New Text("b"), New LongWritable(1)), Arrays.asList(New Text("c"), New LongWritable(2))}

			Dim tp As TransformProcess = (New TransformProcess.Builder(s)).convertToSequence().build()

			Dim rdd As JavaRDD(Of IList(Of Writable)) = sc.parallelize(allExamples)

			Dim [out] As JavaRDD(Of IList(Of IList(Of Writable))) = SparkTransformExecutor.executeToSequence(rdd, tp)

			Dim out2 As IList(Of IList(Of IList(Of Writable))) = [out].collect()

			assertEquals(3, out2.Count)

			For i As Integer = 0 To 2
				assertTrue(out2.Contains(Collections.singletonList(allExamples(i))))
			Next i
		End Sub
	End Class

End Namespace