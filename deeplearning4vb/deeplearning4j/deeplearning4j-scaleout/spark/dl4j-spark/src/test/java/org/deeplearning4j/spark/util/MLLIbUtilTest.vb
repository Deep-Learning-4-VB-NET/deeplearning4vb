Imports System
Imports System.Collections.Generic
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports Matrices = org.apache.spark.mllib.linalg.Matrices
Imports Matrix = org.apache.spark.mllib.linalg.Matrix
Imports LabeledPoint = org.apache.spark.mllib.regression.LabeledPoint
Imports IrisDataSetIterator = org.deeplearning4j.datasets.iterator.impl.IrisDataSetIterator
Imports BaseSparkTest = org.deeplearning4j.spark.BaseSparkTest
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory
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

Namespace org.deeplearning4j.spark.util



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class MLLIbUtilTest extends org.deeplearning4j.spark.BaseSparkTest
	<Serializable>
	Public Class MLLIbUtilTest
		Inherits BaseSparkTest

		Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(MLLIbUtilTest))

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMlLibTest()
		Public Overridable Sub testMlLibTest()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim dataSet As DataSet = (New IrisDataSetIterator(150, 150)).next()
			Dim list As IList(Of DataSet) = dataSet.asList()
			Dim data As JavaRDD(Of DataSet) = sc.parallelize(list)
			Dim mllLibData As JavaRDD(Of LabeledPoint) = MLLibUtil.fromDataSet(sc, data)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testINDtoMLMatrix()
		Public Overridable Sub testINDtoMLMatrix()
			Dim matIND As INDArray = Nd4j.rand(23, 100)

			Dim matMl As Matrix = MLLibUtil.toMatrix(matIND)

			assertTrue(matrixEquals(matMl, matIND, 0.01))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMltoINDMatrix()
		Public Overridable Sub testMltoINDMatrix()
			Dim matMl As Matrix = Matrices.randn(23, 100, New Random(3949955))

			Dim matIND As INDArray = MLLibUtil.toMatrix(matMl)
			log.info("matrix shape: {}", Arrays.toString(matIND.shapeInfoDataBuffer().asInt()))

			assertTrue(matrixEquals(matMl, matIND, 0.01))
		End Sub

		Private Function matrixEquals(ByVal mlMatrix As Matrix, ByVal indMatrix As INDArray, ByVal eps As Double?) As Boolean
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int mlRows = mlMatrix.numRows();
			Dim mlRows As Integer = mlMatrix.numRows()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int mlCols = mlMatrix.numCols();
			Dim mlCols As Integer = mlMatrix.numCols()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int indRows = indMatrix.rows();
			Dim indRows As Integer = indMatrix.rows()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int indCols = indMatrix.columns();
			Dim indCols As Integer = indMatrix.columns()

			If mlRows <> indRows Then
				Return False
			End If
			If mlCols <> indCols Then
				Return False
			End If

			For i As Integer = 0 To mlRows - 1
				For j As Integer = 0 To mlCols - 1
					Dim delta As Double = Math.Abs(mlMatrix.apply(i, j) - indMatrix.getDouble(i, j))
					If delta > eps Then
						Return False
					End If
				Next j
			Next i
			Return True
		End Function

	End Class

End Namespace