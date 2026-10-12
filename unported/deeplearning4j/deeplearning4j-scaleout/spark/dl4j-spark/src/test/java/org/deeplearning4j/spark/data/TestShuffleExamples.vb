Imports System
Imports System.Collections.Generic
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports BaseSparkTest = org.deeplearning4j.spark.BaseSparkTest
Imports SparkUtils = org.deeplearning4j.spark.util.SparkUtils
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
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

Namespace org.deeplearning4j.spark.data


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class TestShuffleExamples extends org.deeplearning4j.spark.BaseSparkTest
	<Serializable>
	Public Class TestShuffleExamples
		Inherits BaseSparkTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testShuffle()
		Public Overridable Sub testShuffle()
			Dim list As IList(Of DataSet) = New List(Of DataSet)()
			For i As Integer = 0 To 9
				Dim f As INDArray = Nd4j.valueArrayOf(New Integer() {10, 1}, i)
				Dim l As INDArray = f.dup()

				Dim ds As New DataSet(f, l)
				list.Add(ds)
			Next i

			Dim rdd As JavaRDD(Of DataSet) = sc.parallelize(list)

			Dim shuffled As JavaRDD(Of DataSet) = SparkUtils.shuffleExamples(rdd, 10, 10)

			Dim shuffledList As IList(Of DataSet) = shuffled.collect()

			Dim totalExampleCount As Integer = 0
			For Each ds As DataSet In shuffledList
				totalExampleCount += ds.Features.length()
	'            System.out.println(Arrays.toString(ds.getFeatures().data().asFloat()));

				assertEquals(ds.Features, ds.Labels)
			Next ds

			assertEquals(100, totalExampleCount)
		End Sub
	End Class

End Namespace