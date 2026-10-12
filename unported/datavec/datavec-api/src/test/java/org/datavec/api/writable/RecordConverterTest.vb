Imports System.Collections.Generic
Imports Tag = org.junit.jupiter.api.Tag
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Lists = org.nd4j.shade.guava.collect.Lists
Imports Schema = org.datavec.api.transform.schema.Schema
Imports RecordConverter = org.datavec.api.util.ndarray.RecordConverter
Imports Test = org.junit.jupiter.api.Test
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.junit.jupiter.api.Assertions.assertEquals
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith

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
Namespace org.datavec.api.writable

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Record Converter Test") @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) class RecordConverterTest extends org.nd4j.common.tests.BaseND4JTest
	Friend Class RecordConverterTest
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("To Records _ Pass In Classification Data Set _ Expect ND Array And Int Writables") void toRecords_PassInClassificationDataSet_ExpectNDArrayAndIntWritables()
		Friend Overridable Sub toRecords_PassInClassificationDataSet_ExpectNDArrayAndIntWritables()
			Dim feature1 As INDArray = Nd4j.create(New Double() { 4, -5.7, 10, -0.1 }, New Long() { 1, 4 }, DataType.FLOAT)
			Dim feature2 As INDArray = Nd4j.create(New Double() { 11, .7, -1.3, 4 }, New Long() { 1, 4 }, DataType.FLOAT)
			Dim label1 As INDArray = Nd4j.create(New Double() { 0, 0, 1, 0 }, New Long() { 1, 4 }, DataType.FLOAT)
			Dim label2 As INDArray = Nd4j.create(New Double() { 0, 1, 0, 0 }, New Long() { 1, 4 }, DataType.FLOAT)
			Dim dataSet As New DataSet(Nd4j.vstack(Lists.newArrayList(feature1, feature2)), Nd4j.vstack(Lists.newArrayList(label1, label2)))
			Dim writableList As IList(Of IList(Of Writable)) = RecordConverter.toRecords(dataSet)
			assertEquals(2, writableList.Count)
			testClassificationWritables(feature1, 2, writableList(0))
			testClassificationWritables(feature2, 1, writableList(1))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("To Records _ Pass In Regression Data Set _ Expect ND Array And Double Writables") void toRecords_PassInRegressionDataSet_ExpectNDArrayAndDoubleWritables()
		Friend Overridable Sub toRecords_PassInRegressionDataSet_ExpectNDArrayAndDoubleWritables()
			Dim feature As INDArray = Nd4j.create(New Double() { 4, -5.7, 10, -0.1 }, New Long() { 1, 4 }, DataType.FLOAT)
			Dim label As INDArray = Nd4j.create(New Double() { .5, 2, 3, .5 }, New Long() { 1, 4 }, DataType.FLOAT)
			Dim dataSet As New DataSet(feature, label)
			Dim writableList As IList(Of IList(Of Writable)) = RecordConverter.toRecords(dataSet)
			Dim results As IList(Of Writable) = writableList(0)
			Dim ndArrayWritable As NDArrayWritable = DirectCast(results(0), NDArrayWritable)
			assertEquals(1, writableList.Count)
			assertEquals(5, results.Count)
			assertEquals(feature, ndArrayWritable.get())
			Dim i As Integer = 0
			Do While i < label.shape()(1)
				Dim doubleWritable As DoubleWritable = DirectCast(results(i + 1), DoubleWritable)
				assertEquals(label.getDouble(i), doubleWritable.get(), 0)
				i += 1
			Loop
		End Sub

		Private Sub testClassificationWritables(ByVal expectedFeatureVector As INDArray, ByVal expectLabelIndex As Integer, ByVal writables As IList(Of Writable))
			Dim ndArrayWritable As NDArrayWritable = DirectCast(writables(0), NDArrayWritable)
			Dim intWritable As IntWritable = DirectCast(writables(1), IntWritable)
			assertEquals(2, writables.Count)
			assertEquals(expectedFeatureVector, ndArrayWritable.get())
			assertEquals(expectLabelIndex, intWritable.get())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test ND Array Writable Concat") void testNDArrayWritableConcat()
		Friend Overridable Sub testNDArrayWritableConcat()
			Dim l As IList(Of Writable) = New List(Of Writable) From {Of Writable}
			Dim exp As INDArray = Nd4j.create(New Double() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 1 }, New Long() { 1, 10 }, DataType.FLOAT)
			Dim act As INDArray = RecordConverter.toArray(DataType.FLOAT, l)
			assertEquals(exp, act)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test ND Array Writable Concat To Matrix") void testNDArrayWritableConcatToMatrix()
		Friend Overridable Sub testNDArrayWritableConcatToMatrix()
			Dim l1 As IList(Of Writable) = New List(Of Writable) From {Of Writable}
			Dim l2 As IList(Of Writable) = New List(Of Writable) From {Of Writable}
			Dim exp As INDArray = Nd4j.create(New Double()() {
				New Double() { 1, 2, 3, 4, 5 },
				New Double() { 6, 7, 8, 9, 10 }
			}).castTo(DataType.FLOAT)
			Dim act As INDArray = RecordConverter.toMatrix(DataType.FLOAT, New List(Of IList(Of Writable)) From {l1, l2})
			assertEquals(exp, act)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test To Record With List Of Object") void testToRecordWithListOfObject()
		Friend Overridable Sub testToRecordWithListOfObject()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.List<Object> list = java.util.Arrays.asList((Object) 3, 7.0f, "Foo", "Bar", 1.0, 3f, 3L, 7, 0L);
			Dim list As IList(Of Object) = New List(Of Object) From {DirectCast(3, Object), 7.0f, "Foo", "Bar", 1.0, 3f, 3L, 7, 0L}
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.datavec.api.transform.schema.Schema schema = new org.datavec.api.transform.schema.Schema.Builder().addColumnInteger("a").addColumnFloat("b").addColumnString("c").addColumnCategorical("d", "Bar", "Baz").addColumnDouble("e").addColumnFloat("f").addColumnLong("g").addColumnInteger("h").addColumnTime("i", java.util.TimeZone.getDefault()).build();
			Dim schema As Schema = (New Schema.Builder()).addColumnInteger("a").addColumnFloat("b").addColumnString("c").addColumnCategorical("d", "Bar", "Baz").addColumnDouble("e").addColumnFloat("f").addColumnLong("g").addColumnInteger("h").addColumnTime("i", TimeZone.getDefault()).build()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.List<Writable> record = org.datavec.api.util.ndarray.RecordConverter.toRecord(schema, list);
			Dim record As IList(Of Writable) = RecordConverter.toRecord(schema, list)
			assertEquals(record(0).toInt(), 3)
			assertEquals(record(1).toFloat(), 7f, 1e-6)
			assertEquals(record(2).ToString(), "Foo")
			assertEquals(record(3).ToString(), "Bar")
			assertEquals(record(4).toDouble(), 1.0, 1e-6)
			assertEquals(record(5).toFloat(), 3f, 1e-6)
			assertEquals(record(6).toLong(), 3L)
			assertEquals(record(7).toInt(), 7)
			assertEquals(record(8).toLong(), 0)
		End Sub
	End Class

End Namespace