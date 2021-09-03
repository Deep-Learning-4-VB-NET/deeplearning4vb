Imports System
Imports Platform = com.sun.jna.Platform
Imports FileUtils = org.apache.commons.io.FileUtils
Imports BaseSparkTest = org.deeplearning4j.spark.BaseSparkTest
Imports SparkDataValidation = org.deeplearning4j.spark.util.data.SparkDataValidation
Imports ValidationResult = org.deeplearning4j.spark.util.data.ValidationResult
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.MultiDataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertFalse
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
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class TestValidation extends org.deeplearning4j.spark.BaseSparkTest
	<Serializable>
	Public Class TestValidation
		Inherits BaseSparkTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDataSetValidation(@TempDir Path folder) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testDataSetValidation(ByVal folder As Path)
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			Dim f As File = folder.toFile()

			For i As Integer = 0 To 2
				Dim ds As New DataSet(Nd4j.create(1,10), Nd4j.create(1,10))
				ds.save(New File(f, i & ".bin"))
			Next i

			Dim r As ValidationResult = SparkDataValidation.validateDataSets(sc, f.toURI().ToString())
			Dim exp As ValidationResult = ValidationResult.builder().countTotal(3).countTotalValid(3).build()
			assertEquals(exp, r)

			'Add a DataSet that is corrupt (can't be loaded)
			Dim f3 As New File(f, "3.bin")
			FileUtils.writeStringToFile(f3, "This isn't a DataSet!")
			r = SparkDataValidation.validateDataSets(sc, f.toURI().ToString())
			exp = ValidationResult.builder().countTotal(4).countTotalValid(3).countTotalInvalid(1).countLoadingFailure(1).build()
			assertEquals(exp, r)
			f3.delete()


			'Add a DataSet with missing features:
			Call (New DataSet(Nothing, Nd4j.create(1,10))).save(f3)

			r = SparkDataValidation.validateDataSets(sc, f.toURI().ToString())
			exp = ValidationResult.builder().countTotal(4).countTotalValid(3).countTotalInvalid(1).countMissingFeatures(1).build()
			assertEquals(exp, r)

			r = SparkDataValidation.deleteInvalidDataSets(sc, f.toURI().ToString())
			exp.setCountInvalidDeleted(1)
			assertEquals(exp, r)
			assertFalse(f3.exists())
			For i As Integer = 0 To 2
				assertTrue((New File(f,i & ".bin")).exists())
			Next i

			'Add DataSet with incorrect labels shape:
			Call (New DataSet(Nd4j.create(1,10), Nd4j.create(1,20))).save(f3)
			r = SparkDataValidation.validateDataSets(sc, f.toURI().ToString(), New Integer(){-1, 10}, New Integer(){-1, 10})
			exp = ValidationResult.builder().countTotal(4).countTotalValid(3).countTotalInvalid(1).countInvalidLabels(1).build()

			assertEquals(exp, r)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMultiDataSetValidation(@TempDir Path folder) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMultiDataSetValidation(ByVal folder As Path)
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			Dim f As File = folder.toFile()

			For i As Integer = 0 To 2
				Dim ds As New MultiDataSet(Nd4j.create(1,10), Nd4j.create(1,10))
				ds.save(New File(f, i & ".bin"))
			Next i

			Dim r As ValidationResult = SparkDataValidation.validateMultiDataSets(sc, f.toURI().ToString())
			Dim exp As ValidationResult = ValidationResult.builder().countTotal(3).countTotalValid(3).build()
			assertEquals(exp, r)

			'Add a MultiDataSet that is corrupt (can't be loaded)
			Dim f3 As New File(f, "3.bin")
			FileUtils.writeStringToFile(f3, "This isn't a MultiDataSet!")
			r = SparkDataValidation.validateMultiDataSets(sc, f.toURI().ToString())
			exp = ValidationResult.builder().countTotal(4).countTotalValid(3).countTotalInvalid(1).countLoadingFailure(1).build()
			assertEquals(exp, r)
			f3.delete()


			'Add a MultiDataSet with missing features:
			Call (New MultiDataSet(Nothing, Nd4j.create(1,10))).save(f3)

			r = SparkDataValidation.validateMultiDataSets(sc, f.toURI().ToString())
			exp = ValidationResult.builder().countTotal(4).countTotalValid(3).countTotalInvalid(1).countMissingFeatures(1).build()
			assertEquals(exp, r)

			r = SparkDataValidation.deleteInvalidMultiDataSets(sc, f.toURI().ToString())
			exp.setCountInvalidDeleted(1)
			assertEquals(exp, r)
			assertFalse(f3.exists())
			For i As Integer = 0 To 2
				assertTrue((New File(f,i & ".bin")).exists())
			Next i

			'Add MultiDataSet with incorrect labels shape:
			Call (New MultiDataSet(Nd4j.create(1,10), Nd4j.create(1,20))).save(f3)
			r = SparkDataValidation.validateMultiDataSets(sc, f.toURI().ToString(), Arrays.asList(New Integer(){-1, 10}), Arrays.asList(New Integer(){-1, 10}))
			exp = ValidationResult.builder().countTotal(4).countTotalValid(3).countTotalInvalid(1).countInvalidLabels(1).build()
			f3.delete()
			assertEquals(exp, r)

			'Add a MultiDataSet with incorrect number of feature arrays:
			Call (New MultiDataSet(New INDArray(){Nd4j.create(1,10), Nd4j.create(1,10)}, New INDArray(){Nd4j.create(1,10)})).save(f3)
			r = SparkDataValidation.validateMultiDataSets(sc, f.toURI().ToString(), Arrays.asList(New Integer(){-1, 10}), Arrays.asList(New Integer(){-1, 10}))
			exp = ValidationResult.builder().countTotal(4).countTotalValid(3).countTotalInvalid(1).countInvalidFeatures(1).build()
			assertEquals(exp, r)


			r = SparkDataValidation.deleteInvalidMultiDataSets(sc, f.toURI().ToString(), Arrays.asList(New Integer(){-1, 10}), Arrays.asList(New Integer(){-1, 10}))
			exp.setCountInvalidDeleted(1)
			assertEquals(exp, r)
			assertFalse(f3.exists())
		End Sub

	End Class

End Namespace