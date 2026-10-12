Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports JavaPairRDD = org.apache.spark.api.java.JavaPairRDD
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports PairFunction = org.apache.spark.api.java.function.PairFunction
Imports DoubleWritable = org.datavec.api.writable.DoubleWritable
Imports Text = org.datavec.api.writable.Text
Imports Writable = org.datavec.api.writable.Writable
Imports BaseSparkTest = org.datavec.spark.BaseSparkTest
Imports SequenceWritablesToStringFunction = org.datavec.spark.transform.misc.SequenceWritablesToStringFunction
Imports WritablesToStringFunction = org.datavec.spark.transform.misc.WritablesToStringFunction
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Tuple2 = scala.Tuple2
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

Namespace org.datavec.spark.functions


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) public class TestWritablesToStringFunctions extends org.datavec.spark.BaseSparkTest
	<Serializable>
	Public Class TestWritablesToStringFunctions
		Inherits BaseSparkTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCGroup()
		Public Overridable Sub testCGroup()
			Dim leftMap As IList(Of Tuple2(Of String, String)) = New List(Of Tuple2(Of String, String))()
			Dim rightMap As IList(Of Tuple2(Of String, String)) = New List(Of Tuple2(Of String, String))()

			leftMap.Add(New Tuple2(Of String, String)("cat","adam"))
			leftMap.Add(New Tuple2(Of String, String)("dog","adam"))

			rightMap.Add(New Tuple2(Of String, String)("fish","alex"))
			rightMap.Add(New Tuple2(Of String, String)("cat","alice"))
			rightMap.Add(New Tuple2(Of String, String)("dog","steve"))

			Dim pets As IList(Of String) = New List(Of String) From {"cat","dog"}



			Dim sc As JavaSparkContext = Context
			Dim left As JavaPairRDD(Of String, String) = sc.parallelize(leftMap).mapToPair(CType(Function(stringStringTuple2) stringStringTuple2, PairFunction(Of Tuple2(Of String, String), String, String)))

			Dim right As JavaPairRDD(Of String, String) = sc.parallelize(rightMap).mapToPair(CType(Function(stringStringTuple2) stringStringTuple2, PairFunction(Of Tuple2(Of String, String), String, String)))

			Console.WriteLine(left.cogroup(right).collect())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testWritablesToString() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testWritablesToString()

			Dim l As IList(Of Writable) = New List(Of Writable) From {
				New DoubleWritable(1.5),
				New Text("someValue")
			}
			Dim expected As String = l(0).ToString() & "," & l(1).ToString()

			assertEquals(expected, (New WritablesToStringFunction(",")).call(l))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSequenceWritablesToString() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSequenceWritablesToString()

			Dim l As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(New DoubleWritable(1.5), New Text("someValue")), Arrays.asList(New DoubleWritable(2.5), New Text("otherValue"))}

			Dim expected As String = l(0)(0).ToString() & "," & l(0)(1).ToString() & vbLf & l(1)(0).ToString() & "," & l(1)(1).ToString()

			assertEquals(expected, (New SequenceWritablesToStringFunction(",")).call(l))
		End Sub
	End Class

End Namespace