Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports SingletonDataSetIterator = org.nd4j.linalg.dataset.adapter.SingletonDataSetIterator
Imports ImageMultiPreProcessingScaler = org.nd4j.linalg.dataset.api.preprocessor.ImageMultiPreProcessingScaler
Imports ImagePreProcessingScaler = org.nd4j.linalg.dataset.api.preprocessor.ImagePreProcessingScaler
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
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

Namespace org.nd4j.linalg.dataset


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.NDARRAY_ETL) @NativeTag public class ImagePreProcessortTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class ImagePreProcessortTest
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void simpleImageTest(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub simpleImageTest(ByVal backend As Nd4jBackend)
			Dim rChannels As INDArray = Nd4j.zeros(DataType.FLOAT, 10, 10).addi(128)
			Dim gChannels As INDArray = Nd4j.zeros(DataType.FLOAT, 10, 10).addi(64)
			Dim bChannels As INDArray = Nd4j.zeros(DataType.FLOAT, 10, 10).addi(255)
			Dim image As INDArray = Nd4j.vstack(rChannels, gChannels, bChannels).reshape(1, 3, 10, 10)
			Dim orig As INDArray = image.dup()

			'System.out.println(Arrays.toString(image.shape()));
			Dim ds As New DataSet(image, Nd4j.ones(1, 1))
			Dim myScaler As New ImagePreProcessingScaler()
			'So this should scale to 0.5,0.25 and 1;
			Dim expected As INDArray = image.mul(0)
			expected.slice(0, 1).addi(0.5)
			expected.slice(1, 1).addi(0.25)
			expected.slice(2, 1).addi(1.0)
			myScaler.transform(ds)
			assertTrue(Transforms.abs(ds.Features.sub(expected)).maxNumber().doubleValue() <= 0.01)

			'Now giving it 16 bits instead of the default
			'System.out.println(Arrays.toString(image.shape()));
			ds = New DataSet(image, Nd4j.ones(1, 1))
			myScaler = New ImagePreProcessingScaler(0, 1, 16)
			'So this should scale to 0.5,0.25 and 1;
			expected = image.mul(0)
			expected.slice(0, 1).addi(0.5 / 256)
			expected.slice(1, 1).addi(0.25 / 256)
			expected.slice(2, 1).addi(1.0 / 256)
			myScaler.transform(ds)
			assertTrue(Transforms.abs(ds.Features.sub(expected)).maxNumber().doubleValue() <= 0.01)

			'So this should not change the value
			Dim before As INDArray = ds.Features.dup()
			myScaler = New ImagePreProcessingScaler(0, 1, 1)
			myScaler.transform(ds)
			assertTrue(Transforms.abs(ds.Features.sub(before)).maxNumber().doubleValue() <= 0.0001)

			'Scaling back up should give the same results
			myScaler = New ImagePreProcessingScaler(0, (256 * 256 * 256 - 1), 1)
			myScaler.transform(ds)
			assertTrue(Transforms.abs(ds.Features.sub(image)).maxNumber().doubleValue() <= 1)

			'Revert:
			before = orig.dup()
			myScaler = New ImagePreProcessingScaler(0, 1, 1)
			myScaler.transform(before)
			myScaler.revertFeatures(before)
			assertEquals(orig, before)


			'Test labels (segmentation case)
			before = orig.dup()
			myScaler = New ImagePreProcessingScaler(0, 1)
			myScaler.transformLabel(before)
			expected = orig.div(255)
			assertEquals(expected, before)
			myScaler.revertLabels(before)
			assertEquals(orig, before)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void simpleImageTestMulti(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub simpleImageTestMulti(ByVal backend As Nd4jBackend)
			Dim rChannels As INDArray = Nd4j.zeros(10, 10).addi(128)
			Dim gChannels As INDArray = Nd4j.zeros(10, 10).addi(64)
			Dim bChannels As INDArray = Nd4j.zeros(10, 10).addi(255)
			Dim image As INDArray = Nd4j.vstack(rChannels, gChannels, bChannels).reshape(3, 10, 10)
			Dim orig As INDArray = image.dup()

			'System.out.println(Arrays.toString(image.shape()));
			Dim ds As New MultiDataSet(New INDArray(){Nd4j.valueArrayOf(10, 100.0), image.reshape(ChrW(1), 3, 10, 10)}, New INDArray(){Nd4j.ones(1, 1)})
			Dim myScaler As New ImageMultiPreProcessingScaler(1)
			'So this should scale to 0.5,0.25 and 1;
			Dim expected As INDArray = image.mul(0)
			expected.slice(0, 0).addi(0.5)
			expected.slice(1, 0).addi(0.25)
			expected.slice(2, 0).addi(1.0)
			myScaler.transform(ds)
			assertEquals(Nd4j.valueArrayOf(10, 100.0), ds.getFeatures(0))
			assertTrue(Transforms.abs(ds.getFeatures(1).sub(expected)).maxNumber().doubleValue() <= 0.01)

			'Now giving it 16 bits instead of the default
			'System.out.println(Arrays.toString(image.shape()));
			ds = New MultiDataSet(New INDArray(){Nd4j.valueArrayOf(10, 100.0), image.reshape(ChrW(1), 3, 10, 10)}, New INDArray(){Nd4j.ones(1, 1)})
			myScaler = New ImageMultiPreProcessingScaler(0.0, 1.0, 16, New Integer(){1})
			'So this should scale to 0.5,0.25 and 1;
			expected = image.mul(0)
			expected.slice(0, 0).addi(0.5 / 256)
			expected.slice(1, 0).addi(0.25 / 256)
			expected.slice(2, 0).addi(1.0 / 256)
			myScaler.transform(ds)
			assertEquals(Nd4j.valueArrayOf(10, 100.0), ds.getFeatures(0))
			assertTrue(Transforms.abs(ds.getFeatures(1).sub(expected)).maxNumber().doubleValue() <= 0.01)

			'So this should not change the value
			Dim before As INDArray = ds.getFeatures(1).dup()
			myScaler = New ImageMultiPreProcessingScaler(0.0, 1.0, New Integer(){1})
			myScaler.transform(ds)
			assertTrue(Transforms.abs(ds.getFeatures(1).sub(before)).maxNumber().doubleValue() <= 0.0001)

			'Scaling back up should give the same results
			myScaler = New ImageMultiPreProcessingScaler(0.0, (256.0 * 256 * 256 - 1), New Integer(){1})
			myScaler.transform(ds)
			assertTrue(Transforms.abs(ds.getFeatures(1).sub(image)).maxNumber().doubleValue() <= 1)

			'Revert:
			before = orig.dup()
			myScaler = New ImageMultiPreProcessingScaler(0.0, 1.0, 1, New Integer(){1})
			Dim beforeDS As New MultiDataSet(New INDArray(){Nothing, before}, New INDArray(){Nothing})
			myScaler.transform(beforeDS)
			myScaler.revertFeatures(beforeDS.Features)
			assertEquals(orig, before)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSegmentation(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSegmentation(ByVal backend As Nd4jBackend)

			Dim f As INDArray = Nd4j.math().floor(Nd4j.rand(DataType.FLOAT, 3, 3, 16, 16).muli(255))
			Dim l As INDArray = Nd4j.math().floor(Nd4j.rand(DataType.FLOAT, 3, 10, 8, 8).muli(255))

			Dim s As New ImagePreProcessingScaler()
			s.fitLabel(True)

			s.fit(New DataSet(f,l))

			Dim expF As INDArray = f.div(255)
			Dim expL As INDArray = l.div(255)

			Dim d As New DataSet(f.dup(), l.dup())
			s.transform(d)
			assertEquals(expF, d.Features)
			assertEquals(expL, d.Labels)


			s.fit(New SingletonDataSetIterator(New DataSet(f, l)))

			Dim f2 As INDArray = f.dup()
			Dim l2 As INDArray = l.dup()
			s.transform(f2)
			s.transformLabel(l2)
			assertEquals(expF, f2)
			assertEquals(expL, l2)

			s.revertFeatures(f2)
			s.revertLabels(l2)
			assertEquals(f, f2)
			assertEquals(l, l2)
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace