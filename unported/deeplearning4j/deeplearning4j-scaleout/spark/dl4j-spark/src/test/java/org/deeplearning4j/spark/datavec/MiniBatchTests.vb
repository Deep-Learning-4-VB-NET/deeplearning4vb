Imports System
Imports System.Collections.Generic
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports [Function] = org.apache.spark.api.java.function.Function
Imports Configuration = org.datavec.api.conf.Configuration
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports SVMLightRecordReader = org.datavec.api.records.reader.impl.misc.SVMLightRecordReader
Imports BaseSparkTest = org.deeplearning4j.spark.BaseSparkTest
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory
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

Namespace org.deeplearning4j.spark.datavec


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class MiniBatchTests extends org.deeplearning4j.spark.BaseSparkTest
	<Serializable>
	Public Class MiniBatchTests
		Inherits BaseSparkTest

		Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(MiniBatchTests))

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMiniBatches() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMiniBatches()
			log.info("Setting up Spark Context...")
			Dim lines As JavaRDD(Of String) = sc.textFile((New ClassPathResource("svmLight/iris_svmLight_0.txt")).TempFileFromArchive.toURI().ToString()).cache()
			Dim count As Long = lines.count()
			assertEquals(300, count)
			' gotta map this to a Matrix/INDArray
			Dim rr As RecordReader = New SVMLightRecordReader()
			Dim c As New Configuration()
			c.set(SVMLightRecordReader.NUM_FEATURES, "5")
			rr.Conf = c
			Dim points As JavaRDD(Of DataSet) = lines.map(New RecordReaderFunction(rr, 4, 3)).cache()
			count = points.count()
			assertEquals(300, count)

			Dim collect As IList(Of DataSet) = points.collect()

			points = points.repartition(1)
			Dim miniBatches As JavaRDD(Of DataSet) = (New RDDMiniBatches(10, points)).miniBatchesJava()
			count = miniBatches.count()
			Dim list As IList(Of DataSet) = miniBatches.collect()
			assertEquals(30, count) 'Expect exactly 30 from 1 partition... could be more for multiple input partitions

			lines.unpersist()
			points.unpersist()
			miniBatches.map(New DataSetAssertionFunction())
		End Sub


		Public Class DataSetAssertionFunction
			Implements [Function](Of DataSet, Object)

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public Object call(org.nd4j.linalg.dataset.DataSet dataSet) throws Exception
			Public Overrides Function [call](ByVal dataSet As DataSet) As Object
				assertTrue(dataSet.Features.columns() = 150)
				assertTrue(dataSet.numExamples() = 30)
				Return Nothing
			End Function
		End Class


	End Class

End Namespace