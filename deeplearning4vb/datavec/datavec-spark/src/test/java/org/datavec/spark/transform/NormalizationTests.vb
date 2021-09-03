Imports System
Imports System.Collections.Generic
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports Dataset = org.apache.spark.sql.Dataset
Imports Row = org.apache.spark.sql.Row
Imports Schema = org.datavec.api.transform.schema.Schema
Imports RecordConverter = org.datavec.api.util.ndarray.RecordConverter
Imports DoubleWritable = org.datavec.api.writable.DoubleWritable
Imports Writable = org.datavec.api.writable.Writable
Imports BaseSparkTest = org.datavec.spark.BaseSparkTest
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataNormalization = org.nd4j.linalg.dataset.api.preprocessor.DataNormalization
Imports NormalizerMinMaxScaler = org.nd4j.linalg.dataset.api.preprocessor.NormalizerMinMaxScaler
Imports NormalizerStandardize = org.nd4j.linalg.dataset.api.preprocessor.NormalizerStandardize
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.datavec.spark.transform



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class NormalizationTests extends org.datavec.spark.BaseSparkTest
	<Serializable>
	Public Class NormalizationTests
		Inherits BaseSparkTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMeanStdZeros()
		Public Overridable Sub testMeanStdZeros()
			Dim data As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			Dim builder As New Schema.Builder()
			Dim numColumns As Integer = 6
			For i As Integer = 0 To numColumns - 1
				builder.addColumnDouble(i.ToString())
			Next i

			Nd4j.Random.setSeed(12345)

			Dim arr As INDArray = Nd4j.rand(DataType.FLOAT, 5, numColumns)
			For i As Integer = 0 To 4
				Dim record As IList(Of Writable) = New List(Of Writable)(numColumns)
				data.Add(record)
				For j As Integer = 0 To numColumns - 1
					record.Add(New DoubleWritable(arr.getDouble(i, j)))
				Next j
			Next i


			Dim schema As Schema = builder.build()
			Dim rdd As JavaRDD(Of IList(Of Writable)) = sc.parallelize(data)
			Dim dataFrame As Dataset(Of Row) = DataFrames.toDataFrame(schema, rdd)

			'assert equivalent to the ndarray pre processing
			Dim zeroToOne As DataNormalization = New NormalizerMinMaxScaler()
			zeroToOne.fit(New DataSet(arr.dup(), arr.dup()))
			Dim zeroToOnes As INDArray = arr.dup()
			zeroToOne.transform(New DataSet(zeroToOnes, zeroToOnes))
			Dim rows As IList(Of Row) = Normalization.stdDevMeanColumns(dataFrame, dataFrame.columns())
			Dim assertion As INDArray = DataFrames.toMatrix(rows)
			Dim expStd As INDArray = arr.std(True, True, 0)
			Dim std As INDArray = assertion.getRow(0, True)
			assertTrue(expStd.equalsWithEps(std, 1e-3))
			'compare mean
			Dim expMean As INDArray = arr.mean(True, 0)
			assertTrue(expMean.equalsWithEps(assertion.getRow(1, True), 1e-3))

		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void normalizationTests()
		Public Overridable Sub normalizationTests()
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

			Dim arr As INDArray = RecordConverter.toMatrix(DataType.DOUBLE, data)

			Dim schema As Schema = builder.build()
			Dim rdd As JavaRDD(Of IList(Of Writable)) = sc.parallelize(data)
			assertEquals(schema, DataFrames.fromStructType(DataFrames.fromSchema(schema)))
			assertEquals(rdd.collect(), DataFrames.toRecords(DataFrames.toDataFrame(schema, rdd)).Second.collect())

			Dim dataFrame As Dataset(Of Row) = DataFrames.toDataFrame(schema, rdd)
			dataFrame.show()
			Normalization.zeromeanUnitVariance(dataFrame).show()
			Normalization.normalize(dataFrame).show()

			'assert equivalent to the ndarray pre processing
			Dim standardScaler As New NormalizerStandardize()
			standardScaler.fit(New DataSet(arr.dup(), arr.dup()))
			Dim standardScalered As INDArray = arr.dup()
			standardScaler.transform(New DataSet(standardScalered, standardScalered))
			Dim zeroToOne As DataNormalization = New NormalizerMinMaxScaler()
			zeroToOne.fit(New DataSet(arr.dup(), arr.dup()))
			Dim zeroToOnes As INDArray = arr.dup()
			zeroToOne.transform(New DataSet(zeroToOnes, zeroToOnes))

			Dim zeroMeanUnitVarianceDataFrame As INDArray = RecordConverter.toMatrix(DataType.DOUBLE, Normalization.zeromeanUnitVariance(schema, rdd).collect())
			Dim zeroMeanUnitVarianceDataFrameZeroToOne As INDArray = RecordConverter.toMatrix(DataType.DOUBLE, Normalization.normalize(schema, rdd).collect())
			assertEquals(standardScalered, zeroMeanUnitVarianceDataFrame)
			assertTrue(zeroToOnes.equalsWithEps(zeroMeanUnitVarianceDataFrameZeroToOne, 1e-1))

		End Sub

	End Class

End Namespace