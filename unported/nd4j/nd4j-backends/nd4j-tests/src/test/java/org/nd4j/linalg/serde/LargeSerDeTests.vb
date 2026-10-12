Imports System.IO
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
import static org.junit.jupiter.api.Assertions.assertArrayEquals
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

Namespace org.nd4j.linalg.serde



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Tag(TagNames.JACKSON_SERDE) @NativeTag @Tag(TagNames.LARGE_RESOURCES) @Tag(TagNames.LONG_TEST) public class LargeSerDeTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class LargeSerDeTests
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLargeArraySerDe_1(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testLargeArraySerDe_1(ByVal backend As Nd4jBackend)
			Dim arrayA As val = Nd4j.rand(New Long() {1, 135079944})
			'val arrayA = Nd4j.rand(new long[] {1, 13507});

			Dim tmpFile As val = File.createTempFile("sdsds", "sdsd")
			tmpFile.deleteOnExit()

			Using fos As lombok.val = New FileStream(tmpFile, FileMode.Create, FileAccess.Write), bos As lombok.val = New BufferedOutputStream(fos), dos As lombok.val = New DataOutputStream(bos)
				Nd4j.write(arrayA, dos)
			End Using


			Using fis As lombok.val = New FileStream(tmpFile, FileMode.Open, FileAccess.Read), bis As lombok.val = New BufferedInputStream(fis), dis As lombok.val = New DataInputStream(bis)
				Dim arrayB As val = Nd4j.read(dis)

				assertArrayEquals(arrayA.shape(), arrayB.shape())
				assertEquals(arrayA.length(), arrayB.length())
				assertEquals(arrayA, arrayB)
			End Using
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testLargeArraySerDe_2(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testLargeArraySerDe_2(ByVal backend As Nd4jBackend)
			Dim arrayA As INDArray = Nd4j.createUninitialized(100000, 12500)
			log.info("Shape: {}; Length: {}", arrayA.shape(), arrayA.length())

			Dim tmpFile As val = File.createTempFile("sdsds", "sdsd")
			tmpFile.deleteOnExit()

			log.info("Starting serialization...")
			Dim sS As val = DateTimeHelper.CurrentUnixTimeMillis()
			Using fos As lombok.val = New FileStream(tmpFile, FileMode.Create, FileAccess.Write), bos As lombok.val = New BufferedOutputStream(fos), dos As lombok.val = New DataOutputStream(bos)
				Nd4j.write(arrayA, dos)
				arrayA = Nothing
				System.GC.Collect()
			End Using
			System.GC.Collect()

			Dim sE As val = DateTimeHelper.CurrentUnixTimeMillis()

			log.info("Starting deserialization...")
			Dim dS As val = DateTimeHelper.CurrentUnixTimeMillis()
			Using fis As lombok.val = New FileStream(tmpFile, FileMode.Open, FileAccess.Read), bis As lombok.val = New BufferedInputStream(fis), dis As lombok.val = New DataInputStream(bis)
				arrayA = Nd4j.read(dis)
			End Using
			Dim dE As val = DateTimeHelper.CurrentUnixTimeMillis()

			log.info("Timings: {Ser : {} ms; De: {} ms;}", sE - sS, dE - dS)
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace