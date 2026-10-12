Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports FileUtils = org.apache.commons.io.FileUtils
Imports ArrayUtils = org.apache.commons.lang3.ArrayUtils
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports NDArrayCreationUtil = org.nd4j.linalg.checkutil.NDArrayCreationUtil
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
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

Namespace org.nd4j.linalg.api.ndarray


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.NDARRAY_SERDE) public class TestNdArrReadWriteTxt extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class TestNdArrReadWriteTxt
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir Path testDir;
		Friend testDir As Path

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void compareAfterWrite(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub compareAfterWrite(ByVal backend As Nd4jBackend)
			Dim ranksToCheck() As Integer = {0, 1, 2, 3, 4}
			For i As Integer = 0 To ranksToCheck.Length - 1
	'            log.info("Checking read write arrays with rank " + ranksToCheck[i]);
				compareArrays(ranksToCheck(i),ordering(), testDir)
			Next i
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void compareArrays(int rank, char ordering, java.nio.file.Path testDir) throws Exception
		Public Shared Sub compareArrays(ByVal rank As Integer, ByVal ordering As Char, ByVal testDir As Path)
			Dim all As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getTestMatricesWithVaryingShapes(rank,ordering, Nd4j.defaultFloatingPointType())
			Dim iter As IEnumerator(Of Pair(Of INDArray, String)) = all.GetEnumerator()
			Dim cnt As Integer = 0
			Do While iter.MoveNext()
				Dim dir As File = testDir.toFile()
				Dim currentPair As Pair(Of INDArray, String) = iter.Current
				Dim origArray As INDArray = currentPair.First
				'adding elements outside the bounds where print switches to scientific notation
				origArray.tensorAlongDimension(0,0).muli(0).addi(100000)
				origArray.putScalar(0,10001.1234)
	'            log.info("\nChecking shape ..." + currentPair.getSecond());
				'log.info("C:\n"+ origArray.dup('c').toString());
	'            log.info("F:\n"+ origArray.toString());
				Nd4j.writeTxt(origArray, (New File(dir, "someArr.txt")).getAbsolutePath())
				Dim readBack As INDArray = Nd4j.readTxt((New File(dir, "someArr.txt")).getAbsolutePath())
				assertEquals(origArray, readBack,vbLf & "Not equal on shape " & ArrayUtils.toString(origArray.shape()))
				cnt += 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNd4jReadWriteText(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testNd4jReadWriteText(ByVal backend As Nd4jBackend)

			Dim dir As File = testDir.toFile()
			Dim count As Integer = 0
			For Each testShape As val In New Long()(){
				New Long() {1, 1},
				New Long() {3, 1},
				New Long() {4, 5},
				New Long() {1, 2, 3},
				New Long() {2, 1, 3},
				New Long() {2, 3, 1},
				New Long() {2, 3, 4},
				New Long() {1, 2, 3, 4},
				New Long() {2, 3, 4, 2}
			}
				Dim l As IList(Of Pair(Of INDArray, String)) = Nothing
				Select Case testShape.length
					Case 2
						l = NDArrayCreationUtil.getAllTestMatricesWithShape(testShape(0), testShape(1), 12345, Nd4j.defaultFloatingPointType())
					Case 3
						l = NDArrayCreationUtil.getAll3dTestArraysWithShape(12345, testShape, Nd4j.defaultFloatingPointType())
					Case 4
						l = NDArrayCreationUtil.getAll4dTestArraysWithShape(12345, testShape, Nd4j.defaultFloatingPointType())
					Case Else
						Throw New Exception()
				End Select


				For Each p As Pair(Of INDArray, String) In l
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: java.io.File f = new java.io.File(dir, (count++) + ".txt");
					Dim f As New File(dir, (count) & ".txt")
						count += 1
					Nd4j.writeTxt(p.First, f.getAbsolutePath())

					Dim read As INDArray = Nd4j.readTxt(f.getAbsolutePath())
					Dim s As String = FileUtils.readFileToString(f, StandardCharsets.UTF_8)
	'                System.out.println(s);

					assertEquals(p.First, read)
				Next p
			Next testShape
		End Sub

		Public Overrides Function ordering() As Char
			Return "f"c
		End Function
	End Class

End Namespace