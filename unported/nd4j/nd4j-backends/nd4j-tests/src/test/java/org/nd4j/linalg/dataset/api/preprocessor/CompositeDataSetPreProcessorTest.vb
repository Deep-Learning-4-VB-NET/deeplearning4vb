Imports System
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports org.junit.jupiter.api.Assertions

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

Namespace org.nd4j.linalg.dataset.api.preprocessor

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.NDARRAY_ETL) @NativeTag public class CompositeDataSetPreProcessorTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class CompositeDataSetPreProcessorTest
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void when_preConditionsIsNull_expect_NullPointerException(org.nd4j.linalg.factory.Nd4jBackend backend)
		 Public Overridable Sub when_preConditionsIsNull_expect_NullPointerException(ByVal backend As Nd4jBackend)
			assertThrows(GetType(System.NullReferenceException),Sub()
			Dim sut As New CompositeDataSetPreProcessor()
			sut.preProcess(Nothing)
			End Sub)


		 End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void when_dataSetIsEmpty_expect_emptyDataSet(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub when_dataSetIsEmpty_expect_emptyDataSet(ByVal backend As Nd4jBackend)
			' Assemble
			Dim sut As New CompositeDataSetPreProcessor()
			Dim ds As New DataSet(Nothing, Nothing)

			' Act
			sut.preProcess(ds)

			' Assert
			assertTrue(ds.Empty)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void when_notStoppingOnEmptyDataSet_expect_allPreProcessorsCalled(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub when_notStoppingOnEmptyDataSet_expect_allPreProcessorsCalled(ByVal backend As Nd4jBackend)
			' Assemble
			Dim preProcessor1 As New TestDataSetPreProcessor(True)
			Dim preProcessor2 As New TestDataSetPreProcessor(True)
			Dim sut As New CompositeDataSetPreProcessor(preProcessor1, preProcessor2)
			Dim ds As New DataSet(Nd4j.rand(2, 2), Nothing)

			' Act
			sut.preProcess(ds)

			' Assert
			assertTrue(preProcessor1.hasBeenCalled)
			assertTrue(preProcessor2.hasBeenCalled)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void when_stoppingOnEmptyDataSetAndFirstPreProcessorClearDS_expect_firstPreProcessorsCalled(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub when_stoppingOnEmptyDataSetAndFirstPreProcessorClearDS_expect_firstPreProcessorsCalled(ByVal backend As Nd4jBackend)
			' Assemble
			Dim preProcessor1 As New TestDataSetPreProcessor(True)
			Dim preProcessor2 As New TestDataSetPreProcessor(True)
			Dim sut As New CompositeDataSetPreProcessor(True, preProcessor1, preProcessor2)
			Dim ds As New DataSet(Nd4j.rand(2, 2), Nothing)

			' Act
			sut.preProcess(ds)

			' Assert
			assertTrue(preProcessor1.hasBeenCalled)
			assertFalse(preProcessor2.hasBeenCalled)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void when_stoppingOnEmptyDataSet_expect_firstPreProcessorsCalled(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub when_stoppingOnEmptyDataSet_expect_firstPreProcessorsCalled(ByVal backend As Nd4jBackend)
			' Assemble
			Dim preProcessor1 As New TestDataSetPreProcessor(False)
			Dim preProcessor2 As New TestDataSetPreProcessor(False)
			Dim sut As New CompositeDataSetPreProcessor(True, preProcessor1, preProcessor2)
			Dim ds As New DataSet(Nd4j.rand(2, 2), Nothing)

			' Act
			sut.preProcess(ds)

			' Assert
			assertTrue(preProcessor1.hasBeenCalled)
			assertTrue(preProcessor2.hasBeenCalled)
		End Sub

		<Serializable>
		Public Class TestDataSetPreProcessor
			Implements DataSetPreProcessor

			Friend ReadOnly clearDataSet As Boolean

			Public hasBeenCalled As Boolean

			Public Sub New(ByVal clearDataSet As Boolean)
				Me.clearDataSet = clearDataSet
			End Sub

			Public Overridable Sub preProcess(ByVal dataSet As org.nd4j.linalg.dataset.api.DataSet)
				hasBeenCalled = True
				If clearDataSet Then
					dataSet.Features = Nothing
				End If
			End Sub
		End Class

	End Class

End Namespace