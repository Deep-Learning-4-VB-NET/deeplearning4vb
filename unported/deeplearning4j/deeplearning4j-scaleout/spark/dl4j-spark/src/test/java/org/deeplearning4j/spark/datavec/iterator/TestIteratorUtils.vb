Imports System
Imports System.Collections.Generic
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports CSVRecordReader = org.datavec.api.records.reader.impl.csv.CSVRecordReader
Imports FileSplit = org.datavec.api.split.FileSplit
Imports Writable = org.datavec.api.writable.Writable
Imports StringToWritablesFunction = org.datavec.spark.transform.misc.StringToWritablesFunction
Imports RecordReaderMultiDataSetIterator = org.deeplearning4j.datasets.datavec.RecordReaderMultiDataSetIterator
Imports BaseSparkTest = org.deeplearning4j.spark.BaseSparkTest
Imports Test = org.junit.jupiter.api.Test
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
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

Namespace org.deeplearning4j.spark.datavec.iterator


	<Serializable>
	Public Class TestIteratorUtils
		Inherits BaseSparkTest

		Public Overrides ReadOnly Property DataType As DataType
			Get
				Return DataType.FLOAT
			End Get
		End Property

		Public Overrides ReadOnly Property DefaultFPDataType As DataType
			Get
				Return DataType.FLOAT
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testIrisRRMDSI() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testIrisRRMDSI()

			Dim cpr As New ClassPathResource("iris.txt")
			Dim f As File = cpr.File
			Dim rr As RecordReader = New CSVRecordReader()
			rr.initialize(New org.datavec.api.Split.FileSplit(f))

			Dim rrmdsi1 As RecordReaderMultiDataSetIterator = (New RecordReaderMultiDataSetIterator.Builder(1)).addReader("reader", rr).addInput("reader", 0, 3).addOutputOneHot("reader", 4, 3).build()

			Dim rrmdsi2 As RecordReaderMultiDataSetIterator = (New RecordReaderMultiDataSetIterator.Builder(1)).addReader("reader", New SparkSourceDummyReader(0)).addInput("reader", 0, 3).addOutputOneHot("reader", 4, 3).build()

			Dim expected As IList(Of MultiDataSet) = New List(Of MultiDataSet)(150)
			Do While rrmdsi1.MoveNext()
				expected.Add(rrmdsi1.Current)
			Loop

			Dim rdd As JavaRDD(Of IList(Of Writable)) = sc.textFile(f.getPath()).coalesce(1).map(New StringToWritablesFunction(New CSVRecordReader()))

			Dim mdsRdd As JavaRDD(Of MultiDataSet) = IteratorUtils.mapRRMDSI(rdd, rrmdsi2)

			Dim act As IList(Of MultiDataSet) = mdsRdd.collect()

			assertEquals(expected, act)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRRMDSIJoin() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testRRMDSIJoin()

			Dim cpr1 As New ClassPathResource("spark/rrmdsi/file1.txt")
			Dim cpr2 As New ClassPathResource("spark/rrmdsi/file2.txt")

			Dim rr1 As RecordReader = New CSVRecordReader()
			rr1.initialize(New org.datavec.api.Split.FileSplit(cpr1.File))
			Dim rr2 As RecordReader = New CSVRecordReader()
			rr2.initialize(New org.datavec.api.Split.FileSplit(cpr2.File))

			Dim rrmdsi1 As RecordReaderMultiDataSetIterator = (New RecordReaderMultiDataSetIterator.Builder(1)).addReader("r1", rr1).addReader("r2", rr2).addInput("r1", 1, 2).addOutput("r2",1,2).build()

			Dim rrmdsi2 As RecordReaderMultiDataSetIterator = (New RecordReaderMultiDataSetIterator.Builder(1)).addReader("r1", New SparkSourceDummyReader(0)).addReader("r2", New SparkSourceDummyReader(1)).addInput("r1", 1, 2).addOutput("r2",1,2).build()

			Dim expected As IList(Of MultiDataSet) = New List(Of MultiDataSet)(3)
			Do While rrmdsi1.MoveNext()
				expected.Add(rrmdsi1.Current)
			Loop

			Dim rdd1 As JavaRDD(Of IList(Of Writable)) = sc.textFile(cpr1.File.getPath()).coalesce(1).map(New StringToWritablesFunction(New CSVRecordReader()))
			Dim rdd2 As JavaRDD(Of IList(Of Writable)) = sc.textFile(cpr2.File.getPath()).coalesce(1).map(New StringToWritablesFunction(New CSVRecordReader()))

			Dim list As IList(Of JavaRDD(Of IList(Of Writable))) = New List(Of JavaRDD(Of IList(Of Writable))) From {rdd1, rdd2}
			Dim mdsRdd As JavaRDD(Of MultiDataSet) = IteratorUtils.mapRRMDSI(list, Nothing, New Integer(){0, 0}, Nothing, False, rrmdsi2)

			Dim act As IList(Of MultiDataSet) = mdsRdd.collect()


			expected = New List(Of MultiDataSet)(expected)
			act = New List(Of MultiDataSet)(act)
			Dim comp As IComparer(Of MultiDataSet) = New ComparatorAnonymousInnerClass(Me)

			expected.Sort(comp)
			act.Sort(comp)

			assertEquals(expected, act)
		End Sub

		Private Class ComparatorAnonymousInnerClass
			Implements IComparer(Of MultiDataSet)

			Private ReadOnly outerInstance As TestIteratorUtils

			Public Sub New(ByVal outerInstance As TestIteratorUtils)
				Me.outerInstance = outerInstance
			End Sub

			Public Function Compare(ByVal d1 As MultiDataSet, ByVal d2 As MultiDataSet) As Integer Implements IComparer(Of MultiDataSet).Compare
				Return d1.getFeatures(0).getDouble(0).CompareTo(d2.getFeatures(0).getDouble(0))
			End Function
		End Class

	End Class

End Namespace