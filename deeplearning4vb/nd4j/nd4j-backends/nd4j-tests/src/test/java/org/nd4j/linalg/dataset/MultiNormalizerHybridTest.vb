Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MultiNormalizerHybrid = org.nd4j.linalg.dataset.api.preprocessor.MultiNormalizerHybrid
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
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

Namespace org.nd4j.linalg.dataset

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.NDARRAY_ETL) @NativeTag @Tag(TagNames.FILE_IO) public class MultiNormalizerHybridTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class MultiNormalizerHybridTest
		Inherits BaseNd4jTestWithBackends

		Private SUT As MultiNormalizerHybrid
		Private data As MultiDataSet
		Private dataCopy As MultiDataSet

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp()
		Public Overridable Sub setUp()
			SUT = New MultiNormalizerHybrid()
			data = New MultiDataSet(New INDArray() {
				Nd4j.create(New Single()() {
					New Single() {1, 2},
					New Single() {3, 4}
			}), Nd4j.create(New Single()() {
				New Single() {3, 4},
				New Single() {5, 6}
			})
			},
			New INDArray() {
			Nd4j.create(New Single()() {
			New Single() {10, 11},
			New Single() {12, 13}
			}), Nd4j.create(New Single()() {
				New Single() {14, 15},
				New Single() {16, 17}
			})})
			dataCopy = data.copy()
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNoNormalizationByDefault(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNoNormalizationByDefault(ByVal backend As Nd4jBackend)
			SUT.fit(data)
			SUT.preProcess(data)
			assertEquals(dataCopy, data)

			SUT.revert(data)
			assertEquals(dataCopy, data)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGlobalNormalization(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGlobalNormalization(ByVal backend As Nd4jBackend)
			SUT.standardizeAllInputs().minMaxScaleAllOutputs(-10, 10).fit(data)
			SUT.preProcess(data)

			Dim expected As New MultiDataSet(New INDArray() {
				Nd4j.create(New Single()() {
					New Single() {-1, -1},
					New Single() {1, 1}
			}), Nd4j.create(New Single()() {
				New Single() {-1, -1},
				New Single() {1, 1}
			})
			},
			New INDArray() {
			Nd4j.create(New Single()() {
			New Single() {-10, -10},
			New Single() {10, 10}
			}), Nd4j.create(New Single()() {
				New Single() {-10, -10},
				New Single() {10, 10}
			})})

			assertEquals(expected, data)

			SUT.revert(data)
			assertEquals(dataCopy, data)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSpecificInputOutputNormalization(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSpecificInputOutputNormalization(ByVal backend As Nd4jBackend)
			SUT.minMaxScaleAllInputs().standardizeInput(1).standardizeOutput(0).fit(data)
			SUT.preProcess(data)

			Dim expected As New MultiDataSet(New INDArray() {
				Nd4j.create(New Single()() {
					New Single() {0, 0},
					New Single() {1, 1}
			}), Nd4j.create(New Single()() {
				New Single() {-1, -1},
				New Single() {1, 1}
			})
			},
			New INDArray() {
			Nd4j.create(New Single()() {
			New Single() {-1, -1},
			New Single() {1, 1}
			}), Nd4j.create(New Single()() {
				New Single() {14, 15},
				New Single() {16, 17}
			})})

			assertEquals(expected, data)

			SUT.revert(data)
			assertEquals(dataCopy, data)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMasking(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMasking(ByVal backend As Nd4jBackend)
			Dim timeSeries As New MultiDataSet(New INDArray() {
				Nd4j.create(New Single() {1, 2, 3, 4, 5, 0, 7, 0}).reshape(ChrW(2), 2, 2)
			},
			New INDArray() {
				Nd4j.create(New Single() {0, 20, 0, 40, 50, 60, 70, 80}).reshape(ChrW(2), 2, 2)
			},
			New INDArray() {
				Nd4j.create(New Single()() {
					New Single() {1, 1},
					New Single() {1, 0}
			})
			},
			New INDArray() {
			Nd4j.create(New Single()() {
			New Single() {0, 1},
			New Single() {1, 1}
			})})
			Dim timeSeriesCopy As MultiDataSet = timeSeries.copy()

			SUT.minMaxScaleAllInputs(-10, 10).minMaxScaleAllOutputs(-10, 10).fit(timeSeries)
			SUT.preProcess(timeSeries)

			Dim expected As New MultiDataSet(New INDArray() {
				Nd4j.create(New Single() {-10, -5, -10, -5, 10, 0, 10, 0}).reshape(ChrW(2), 2, 2)
			},
			New INDArray() {
				Nd4j.create(New Single() {0, -10, 0, -10, 5, 10, 5, 10}).reshape(ChrW(2), 2, 2)
			},
			New INDArray() {
				Nd4j.create(New Single()() {
					New Single() {1, 1},
					New Single() {1, 0}
			})
			},
			New INDArray() {
			Nd4j.create(New Single()() {
			New Single() {0, 1},
			New Single() {1, 1}
			})})

			assertEquals(expected, timeSeries)

			SUT.revert(timeSeries)

			assertEquals(timeSeriesCopy, timeSeries)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDataSetWithoutLabels(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDataSetWithoutLabels(ByVal backend As Nd4jBackend)
			SUT.standardizeAllInputs().standardizeAllOutputs().fit(data)

			data.Labels = Nothing
			data.LabelsMaskArray = Nothing

			SUT.preProcess(data)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDataSetWithoutFeatures(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDataSetWithoutFeatures(ByVal backend As Nd4jBackend)
			SUT.standardizeAllInputs().standardizeAllOutputs().fit(data)

			data.Features = Nothing
			data.FeaturesMaskArrays = Nothing

			SUT.preProcess(data)
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace