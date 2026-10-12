Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.junit.jupiter.api.Assertions.assertArrayEquals
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue
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
Namespace org.deeplearning4j.datasets.iterator

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Random Data Set Iterator Test") @NativeTag @Tag(TagNames.FILE_IO) class RandomDataSetIteratorTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class RandomDataSetIteratorTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test DSI") void testDSI()
		Friend Overridable Sub testDSI()
			Dim iter As DataSetIterator = New RandomDataSetIterator(5, New Long() { 3, 4 }, New Long() { 3, 5 }, RandomDataSetIterator.Values.RANDOM_UNIFORM, RandomDataSetIterator.Values.ONE_HOT)
			Dim count As Integer = 0
			Do While iter.MoveNext()
				count += 1
				Dim ds As DataSet = iter.Current
				assertArrayEquals(New Long() { 3, 4 }, ds.Features.shape())
				assertArrayEquals(New Long() { 3, 5 }, ds.Labels.shape())
				assertTrue(ds.Features.minNumber().doubleValue() >= 0.0 AndAlso ds.Features.maxNumber().doubleValue() <= 1.0)
				assertEquals(Nd4j.ones(3), ds.Labels.sum(1))
			Loop
			assertEquals(5, count)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test MDSI") void testMDSI()
		Friend Overridable Sub testMDSI()
			Nd4j.Random.setSeed(12345)
			Dim iter As MultiDataSetIterator = (New RandomMultiDataSetIterator.Builder(5)).addFeatures(New Long() { 3, 4 }, RandomMultiDataSetIterator.Values.INTEGER_0_100).addFeatures(New Long() { 3, 5 }, RandomMultiDataSetIterator.Values.BINARY).addLabels(New Long() { 3, 6 }, RandomMultiDataSetIterator.Values.ZEROS).build()
			Dim count As Integer = 0
			Do While iter.MoveNext()
				count += 1
				Dim mds As MultiDataSet = iter.Current
				assertEquals(2, mds.numFeatureArrays())
				assertEquals(1, mds.numLabelsArrays())
				assertArrayEquals(New Long() { 3, 4 }, mds.getFeatures(0).shape())
				assertArrayEquals(New Long() { 3, 5 }, mds.getFeatures(1).shape())
				assertArrayEquals(New Long() { 3, 6 }, mds.getLabels(0).shape())
				assertTrue(mds.getFeatures(0).minNumber().doubleValue() >= 0 AndAlso mds.getFeatures(0).maxNumber().doubleValue() <= 100.0 AndAlso mds.getFeatures(0).maxNumber().doubleValue() > 2.0)
				assertTrue(mds.getFeatures(1).minNumber().doubleValue() = 0.0 AndAlso mds.getFeatures(1).maxNumber().doubleValue() = 1.0)
				assertEquals(0.0, mds.getLabels(0).sumNumber().doubleValue(), 0.0)
			Loop
			assertEquals(5, count)
		End Sub
	End Class

End Namespace