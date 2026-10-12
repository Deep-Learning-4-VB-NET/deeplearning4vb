Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports InvalidKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives
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

Namespace org.deeplearning4j.nn.modelimport.keras.preprocessing.sequence

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.KERAS) @NativeTag public class TimeSeriesGeneratorTest extends org.deeplearning4j.BaseDL4JTest
	Public Class TimeSeriesGeneratorTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void tsGeneratorTest() throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub tsGeneratorTest()
			Dim data As INDArray = Nd4j.create(50, 10)
			Dim targets As INDArray = Nd4j.create(50, 10)


			Dim length As Integer = 10
			Dim samplingRate As Integer = 2
			Dim stride As Integer = 1
			Dim startIndex As Integer = 0
			Dim endIndex As Integer = 49
			Dim batchSize As Integer = 1

			Dim shuffle As Boolean = False
			Dim reverse As Boolean = False

			Dim gen As New TimeSeriesGenerator(data, targets, length, samplingRate, stride, startIndex, endIndex, shuffle, reverse, batchSize)

			assertEquals(length, gen.getLength())
			assertEquals(startIndex + length, gen.getStartIndex())
			assertEquals(reverse, gen.isReverse())
			assertEquals(shuffle, gen.isShuffle())
			assertEquals(endIndex, gen.getEndIndex())
			assertEquals(batchSize, gen.getBatchSize())
			assertEquals(samplingRate, gen.getSamplingRate())
			assertEquals(stride, gen.getStride())

			Dim [next] As Pair(Of INDArray, INDArray) = gen.next(0)
		End Sub
	End Class

End Namespace