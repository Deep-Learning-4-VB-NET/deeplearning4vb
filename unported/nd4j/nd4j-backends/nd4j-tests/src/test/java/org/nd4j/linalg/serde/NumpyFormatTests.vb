Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports FileUtils = org.apache.commons.io.FileUtils
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
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

Namespace org.nd4j.linalg.serde


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Tag(TagNames.FILE_IO) @Tag(TagNames.NDARRAY_SERDE) public class NumpyFormatTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class NumpyFormatTests
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir Path testDir;
		Friend testDir As Path

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testToNpyFormat(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testToNpyFormat(ByVal backend As Nd4jBackend)

			Dim dir As val = testDir.resolve("new-dir-" & System.Guid.randomUUID().ToString()).toFile()
			assertTrue(dir.mkdirs())
			Call (New ClassPathResource("numpy_arrays/")).copyDirectory(dir)

			Dim files() As File = dir.listFiles()
			Dim cnt As Integer = 0

			For Each f As File In files
				If Not f.getPath().EndsWith(".npy") Then
					log.warn("Skipping: {}", f)
					Continue For
				End If

				Dim path As String = f.getAbsolutePath()
				Dim lastDot As Integer = path.LastIndexOf("."c)
				Dim lastUnderscore As Integer = path.LastIndexOf("_"c)
				Dim dtype As String = path.Substring(lastUnderscore+1, lastDot - (lastUnderscore+1))
	'            System.out.println(path + " : " + dtype);

				Dim dt As DataType = DataType.fromNumpy(dtype)
				'System.out.println(dt);

				Dim arr As INDArray = Nd4j.arange(12).castTo(dt).reshape(ChrW(3), 4)
				Dim bytes() As SByte = Nd4j.toNpyByteArray(arr)
				Dim expected() As SByte = FileUtils.readFileToByteArray(f)
	'
	'            log.info("E: {}", Arrays.toString(expected));
	'            for( int i=0; i<expected.length; i++ ){
	'                System.out.print((char)expected[i]);
	'            }
	'
	'            System.out.println();System.out.println();
	'
	'            log.info("A: {}", Arrays.toString(bytes));
	'            for( int i=0; i<bytes.length; i++ ){
	'                System.out.print((char)bytes[i]);
	'            }
	'            System.out.println();
	'

				assertArrayEquals(expected, bytes,"Failed with file [" & f.getName() & "]")
				cnt += 1
			Next f

			assertTrue(cnt > 0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testToNpyFormatScalars(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testToNpyFormatScalars(ByVal backend As Nd4jBackend)
	'        File dir = new File("C:\\DL4J\\Git\\dl4j-test-resources\\src\\main\\resources\\numpy_arrays\\scalar");

			Dim dir As val = testDir.resolve("new-path0" & System.Guid.randomUUID().ToString()).toFile()
			dir.mkdirs()
			Call (New ClassPathResource("numpy_arrays/scalar/")).copyDirectory(dir)

			Dim files() As File = dir.listFiles()
			Dim cnt As Integer = 0

			For Each f As File In files
				If Not f.getPath().EndsWith(".npy") Then
					log.warn("Skipping: {}", f)
					Continue For
				End If

				Dim path As String = f.getAbsolutePath()
				Dim lastDot As Integer = path.LastIndexOf("."c)
				Dim lastUnderscore As Integer = path.LastIndexOf("_"c)
				Dim dtype As String = path.Substring(lastUnderscore+1, lastDot - (lastUnderscore+1))
	'            System.out.println(path + " : " + dtype);

				Dim dt As DataType = DataType.fromNumpy(dtype)
				'System.out.println(dt);

				Dim arr As INDArray = Nd4j.scalar(dt, 1)
				Dim bytes() As SByte = Nd4j.toNpyByteArray(arr)
				Dim expected() As SByte = FileUtils.readFileToByteArray(f)

	'            
	'            log.info("E: {}", Arrays.toString(expected));
	'            for( int i=0; i<expected.length; i++ ){
	'                System.out.print((char)expected[i]);
	'            }
	'
	'            System.out.println();System.out.println();
	'
	'            log.info("A: {}", Arrays.toString(bytes));
	'            for( int i=0; i<bytes.length; i++ ){
	'                System.out.print((char)bytes[i]);
	'            }
	'            System.out.println();
	'            

				assertArrayEquals(expected, bytes,"Failed with file [" & f.getName() & "]")
				cnt += 1

				Console.WriteLine()
			Next f

			assertTrue(cnt > 0)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNpzReading(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testNpzReading(ByVal backend As Nd4jBackend)

			Dim dir As val = testDir.resolve("new-folder-npz").toFile()
			dir.mkdirs()
			Call (New ClassPathResource("numpy_arrays/npz/")).copyDirectory(dir)

			Dim files() As File = dir.listFiles()
			Dim cnt As Integer = 0

			For Each f As File In files
				If Not f.getPath().EndsWith(".npz") Then
					log.warn("Skipping: {}", f)
					Continue For
				End If

				Dim path As String = f.getAbsolutePath()
				Dim lastDot As Integer = path.LastIndexOf("."c)
				Dim lastSlash As Integer = Math.Max(path.LastIndexOf("/"c), path.LastIndexOf("\"c))
				Dim dtype As String = path.Substring(lastSlash+1, lastDot - (lastSlash+1))
	'            System.out.println(path + " : " + dtype);

				Dim dt As DataType = DataType.fromNumpy(dtype)
				'System.out.println(dt);

				Dim arr As INDArray = Nd4j.arange(12).castTo(dt).reshape(ChrW(3), 4)
				Dim arr2 As INDArray = Nd4j.linspace(DataType.FLOAT, 0, 3, 10)

				Dim m As IDictionary(Of String, INDArray) = Nd4j.createFromNpzFile(f)
				assertEquals(2, m.Count)
				assertTrue(m.ContainsKey("firstArr"))
				assertTrue(m.ContainsKey("secondArr"))

				assertEquals(arr, m("firstArr"))
				assertEquals(arr2, m("secondArr"))
				cnt += 1
			Next f

			assertTrue(cnt > 0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTxtReading(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testTxtReading(ByVal backend As Nd4jBackend)
			Dim f As File = (New ClassPathResource("numpy_arrays/txt/arange_3,4_float32.txt")).File
			Dim arr As INDArray = Nd4j.readNumpy(DataType.FLOAT, f.getPath())

			Dim exp As INDArray = Nd4j.arange(12).castTo(DataType.FLOAT).reshape(ChrW(3), 4)
			assertEquals(exp, arr)

			arr = Nd4j.readNumpy(DataType.DOUBLE, f.getPath())

			assertEquals(exp.castTo(DataType.DOUBLE), arr)

			f = (New ClassPathResource("numpy_arrays/txt_tab/arange_3,4_float32.txt")).File
			arr = Nd4j.readNumpy(DataType.FLOAT, f.getPath(), vbTab)

			assertEquals(exp, arr)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNpy(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testNpy(ByVal backend As Nd4jBackend)
			For Each empty As Boolean In New Boolean(){False, True}
				Dim dir As val = testDir.resolve("new-dir-1-" & System.Guid.randomUUID().ToString()).toFile()
				assertTrue(dir.mkdirs())
				If Not empty Then
					Call (New ClassPathResource("numpy_arrays/npy/3,4/")).copyDirectory(dir)
				Else
					Call (New ClassPathResource("numpy_arrays/npy/0,3_empty/")).copyDirectory(dir)
				End If

				Dim files() As File = dir.listFiles()
				Dim cnt As Integer = 0

				For Each f As File In files
					If Not f.getPath().EndsWith(".npy") Then
						log.warn("Skipping: {}", f)
						Continue For
					End If

					Dim path As String = f.getAbsolutePath()
					Dim lastDot As Integer = path.LastIndexOf("."c)
					Dim lastUnderscore As Integer = path.LastIndexOf("_"c)
					Dim dtype As String = path.Substring(lastUnderscore + 1, lastDot - (lastUnderscore + 1))
	'                System.out.println(path + " : " + dtype);

					Dim dt As DataType = DataType.fromNumpy(dtype)
					'System.out.println(dt);

					Dim exp As INDArray
					If empty Then
						exp = Nd4j.create(dt, 0, 3)
					Else
						exp = Nd4j.arange(12).castTo(dt).reshape(ChrW(3), 4)
					End If
					Dim act As INDArray = Nd4j.createFromNpyFile(f)

					assertEquals(exp, act,"Failed with file [" & f.getName() & "]")
					cnt += 1
				Next f

				assertTrue(cnt > 0)
			Next empty
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testFromNumpyScalar(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testFromNumpyScalar(ByVal backend As Nd4jBackend)
			Dim [out] As val = Nd4j.createFromNpyFile((New ClassPathResource("numpy_oneoff/scalar.npy")).File)
			assertEquals(Nd4j.scalar(DataType.INT, 1), [out])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void readNumpyCorruptHeader1(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub readNumpyCorruptHeader1(ByVal backend As Nd4jBackend)
			assertThrows(GetType(Exception),Sub()
			Dim f As File = testDir.toFile()
			Dim fValid As File = (New ClassPathResource("numpy_arrays/arange_3,4_float32.npy")).File
			Dim numpyBytes() As SByte = FileUtils.readFileToByteArray(fValid)
			For i As Integer = 0 To 9
				numpyBytes(i) = 0
			Next i
			Dim fCorrupt As New File(f, "corrupt.npy")
			FileUtils.writeByteArrayToFile(fCorrupt, numpyBytes)
			Dim exp As INDArray = Nd4j.arange(12).castTo(DataType.FLOAT).reshape(ChrW(3), 4)
			Dim act1 As INDArray = Nd4j.createFromNpyFile(fValid)
			assertEquals(exp, act1)
			Dim probablyShouldntLoad As INDArray = Nd4j.createFromNpyFile(fCorrupt)
			Dim eq As Boolean = exp.Equals(probablyShouldntLoad)
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void readNumpyCorruptHeader2(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub readNumpyCorruptHeader2(ByVal backend As Nd4jBackend)
			assertThrows(GetType(Exception),Sub()
			Dim f As File = testDir.toFile()
			Dim fValid As File = (New ClassPathResource("numpy_arrays/arange_3,4_float32.npy")).File
			Dim numpyBytes() As SByte = FileUtils.readFileToByteArray(fValid)
			For i As Integer = 1 To 9
				numpyBytes(i) = 0
			Next i
			Dim fCorrupt As New File(f, "corrupt.npy")
			FileUtils.writeByteArrayToFile(fCorrupt, numpyBytes)
			Dim exp As INDArray = Nd4j.arange(12).castTo(DataType.FLOAT).reshape(ChrW(3), 4)
			Dim act1 As INDArray = Nd4j.createFromNpyFile(fValid)
			assertEquals(exp, act1)
			Dim probablyShouldntLoad As INDArray = Nd4j.createFromNpyFile(fCorrupt)
			Dim eq As Boolean = exp.Equals(probablyShouldntLoad)
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAbsentNumpyFile_1(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testAbsentNumpyFile_1(ByVal backend As Nd4jBackend)
			assertThrows(GetType(System.ArgumentException),Sub()
			Dim f As val = New File("pew-pew-zomg.some_extension_that_wont_exist")
			Dim act1 As INDArray = Nd4j.createFromNpyFile(f)
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Disabled public void testAbsentNumpyFile_2(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testAbsentNumpyFile_2(ByVal backend As Nd4jBackend)
			assertThrows(GetType(System.ArgumentException),Sub()
			Dim f As val = New File("c:/develop/batch-x-1.npy")
			Dim act1 As INDArray = Nd4j.createFromNpyFile(f)
			log.info("Array shape: {}; sum: {};", act1.shape(), act1.sumNumber().doubleValue())
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNumpyBoolean(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNumpyBoolean(ByVal backend As Nd4jBackend)
			Dim [out] As INDArray = Nd4j.createFromNpyFile(New File("c:/Users/raver/Downloads/error2.npy"))
	'        System.out.println(ArrayUtil.toList(ArrayUtil.toInts(out.shape())));
	'        System.out.println(out);
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace