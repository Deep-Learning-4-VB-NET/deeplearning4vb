Imports System
Imports System.IO
Imports FileUtils = org.apache.commons.io.FileUtils
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports Nd4jCommonValidator = org.nd4j.common.validation.Nd4jCommonValidator
Imports ValidationResult = org.nd4j.common.validation.ValidationResult
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

Namespace org.nd4j.linalg.util


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.FILE_IO) public class ValidationUtilTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class ValidationUtilTests
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir Path testDir;
		Friend testDir As Path

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testFileValidation(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testFileValidation(ByVal backend As Nd4jBackend)
			Dim f As File = testDir.toFile()

			'Test not existent file:
			Dim fNonExistent As New File("doesntExist.bin")
			Dim vr0 As ValidationResult = Nd4jCommonValidator.isValidFile(fNonExistent)
			assertFalse(vr0.isValid())
			assertTrue(vr0.getIssues().get(0).contains("exist"),vr0.getIssues().get(0))
	'        System.out.println(vr0.toString());

			'Test empty file:
			Dim fEmpty As New File(f, "0.bin")
			fEmpty.createNewFile()
			Dim vr1 As ValidationResult = Nd4jCommonValidator.isValidFile(fEmpty)
			assertFalse(vr1.isValid())
			assertTrue(vr1.getIssues().get(0).contains("empty"),vr1.getIssues().get(0))
	'        System.out.println(vr1.toString());

			'Test directory
			Dim directory As New File(f, "dir")
			Dim created As Boolean = directory.mkdir()
			assertTrue(created)
			Dim vr2 As ValidationResult = Nd4jCommonValidator.isValidFile(directory)
			assertFalse(vr2.isValid())
			assertTrue(vr2.getIssues().get(0).contains("directory"),vr2.getIssues().get(0))
	'        System.out.println(vr2.toString());

			'Test valid non-empty file - valid
			Dim f3 As New File(f, "1.txt")
			FileUtils.writeStringToFile(f3, "Test", StandardCharsets.UTF_8)
			Dim vr3 As ValidationResult = Nd4jCommonValidator.isValidFile(f3)
			assertTrue(vr3.isValid())
	'        System.out.println(vr3.toString());
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testZipValidation(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testZipValidation(ByVal backend As Nd4jBackend)
			Dim f As File = testDir.toFile()

			'Test not existent file:
			Dim fNonExistent As New File("doesntExist.zip")
			Dim vr0 As ValidationResult = Nd4jCommonValidator.isValidZipFile(fNonExistent, False)
			assertFalse(vr0.isValid())
			assertTrue(vr0.getIssues().get(0).contains("exist"),vr0.getIssues().get(0))
	'        System.out.println(vr0.toString());

			'Test empty zip:
			Dim fEmpty As File = (New ClassPathResource("validation/empty_zip.zip")).File
			assertTrue(fEmpty.exists())
			Dim vr1 As ValidationResult = Nd4jCommonValidator.isValidZipFile(fEmpty, False)
			assertFalse(vr1.isValid())
			assertTrue(vr1.getIssues().get(0).contains("empty"),vr1.getIssues().get(0))
	'        System.out.println(vr1.toString());

			'Test directory (not zip file)
			Dim directory As New File(f, "dir")
			Dim created As Boolean = directory.mkdir()
			assertTrue(created)
			Dim vr2 As ValidationResult = Nd4jCommonValidator.isValidFile(directory)
			assertFalse(vr2.isValid())
			assertTrue(vr2.getIssues().get(0).contains("directory"),vr2.getIssues().get(0))
	'        System.out.println(vr2.toString());

			'Test non-empty zip - valid
			Dim f3 As New File(f, "1.zip")
			Using z As New java.util.zip.ZipOutputStream(New java.io.BufferedOutputStream(New FileStream(f3, FileMode.Create, FileAccess.Write)))
				Dim ze As New ZipEntry("content.txt")
				z.putNextEntry(ze)
				z.write("Text content".GetBytes())
			End Using
			Dim vr3 As ValidationResult = Nd4jCommonValidator.isValidZipFile(f3, False)
			assertTrue(vr3.isValid())
	'        System.out.println(vr3.toString());

			'Test non-empty zip - but missing required entries
			Dim vr4 As ValidationResult = Nd4jCommonValidator.isValidZipFile(f3, False, "content.txt", "someFile1.bin", "someFile2.bin")
			assertFalse(vr4.isValid())
			assertEquals(1, vr4.getIssues().size())
			Dim s As String = vr4.getIssues().get(0)
			assertTrue(s.Contains("someFile1.bin") AndAlso s.Contains("someFile2.bin"),s)
			assertFalse(s.Contains("content.txt"),s)
	'        System.out.println(vr4.toString());
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testINDArrayTextValidation(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testINDArrayTextValidation(ByVal backend As Nd4jBackend)
			Dim f As File = testDir.toFile()

			'Test not existent file:
			Dim fNonExistent As New File("doesntExist.txt")
			Dim vr0 As ValidationResult = Nd4jValidator.validateINDArrayTextFile(fNonExistent)
			assertFalse(vr0.isValid())
			assertEquals("INDArray Text File", vr0.getFormatType())
			assertTrue(vr0.getIssues().get(0).contains("exist"),vr0.getIssues().get(0))
	'        System.out.println(vr0.toString());

			'Test empty file:
			Dim fEmpty As New File(f, "empty.txt")
			fEmpty.createNewFile()
			assertTrue(fEmpty.exists())
			Dim vr1 As ValidationResult = Nd4jValidator.validateINDArrayTextFile(fEmpty)
			assertEquals("INDArray Text File", vr1.getFormatType())
			assertFalse(vr1.isValid())
			assertTrue(vr1.getIssues().get(0).contains("empty"),vr1.getIssues().get(0))
	'        System.out.println(vr1.toString());

			'Test directory (not zip file)
			Dim directory As New File(f, "dir")
			Dim created As Boolean = directory.mkdir()
			assertTrue(created)
			Dim vr2 As ValidationResult = Nd4jValidator.validateINDArrayTextFile(directory)
			assertEquals("INDArray Text File", vr2.getFormatType())
			assertFalse(vr2.isValid())
			assertTrue(vr2.getIssues().get(0).contains("directory"),vr2.getIssues().get(0))
	'        System.out.println(vr2.toString());

			'Test non-INDArray format:
			Dim fText As New File(f, "text.txt")
			FileUtils.writeStringToFile(fText, "Not a INDArray .text file", StandardCharsets.UTF_8)
			Dim vr3 As ValidationResult = Nd4jValidator.validateINDArrayTextFile(fText)
			assertEquals("INDArray Text File", vr3.getFormatType())
			assertFalse(vr3.isValid())
			Dim s As String = vr3.getIssues().get(0)
			assertTrue(s.Contains("text") AndAlso s.Contains("INDArray") AndAlso s.Contains("corrupt"),s)
	'        System.out.println(vr3.toString());

			'Test corrupted txt format:
			Dim fValid As New File(f, "valid.txt")
			Dim arr As INDArray = Nd4j.arange(12).castTo(DataType.FLOAT).reshape(ChrW(3), 4)
			Nd4j.writeTxt(arr, fValid.getPath())
			Dim indarrayTxtBytes() As SByte = FileUtils.readFileToByteArray(fValid)
			For i As Integer = 0 To 29
				indarrayTxtBytes(i) = CSByte(Math.Truncate(AscW("a"c) + i))
			Next i
			Dim fCorrupt As New File(f, "corrupt.txt")
			FileUtils.writeByteArrayToFile(fCorrupt, indarrayTxtBytes)

			Dim vr4 As ValidationResult = Nd4jValidator.validateINDArrayTextFile(fCorrupt)
			assertEquals("INDArray Text File", vr4.getFormatType())
			assertFalse(vr4.isValid())
			s = vr4.getIssues().get(0)
			assertTrue(s.Contains("text") AndAlso s.Contains("INDArray") AndAlso s.Contains("corrupt"),s)
	'        System.out.println(vr4.toString());


			'Test valid npz format:
			Dim vr5 As ValidationResult = Nd4jValidator.validateINDArrayTextFile(fValid)
			assertEquals("INDArray Text File", vr5.getFormatType())
			assertTrue(vr5.isValid())
			assertNull(vr5.getIssues())
			assertNull(vr5.getException())
	'        System.out.println(vr4.toString());
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNpyValidation(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testNpyValidation(ByVal backend As Nd4jBackend)
			Dim f As File = testDir.toFile()

			'Test not existent file:
			Dim fNonExistent As New File("doesntExist.npy")
			Dim vr0 As ValidationResult = Nd4jValidator.validateNpyFile(fNonExistent)
			assertFalse(vr0.isValid())
			assertEquals("Numpy .npy File", vr0.getFormatType())
			assertTrue(vr0.getIssues().get(0).contains("exist"),vr0.getIssues().get(0))
	'        System.out.println(vr0.toString());

			'Test empty file:
			Dim fEmpty As New File(f, "empty.npy")
			fEmpty.createNewFile()
			assertTrue(fEmpty.exists())
			Dim vr1 As ValidationResult = Nd4jValidator.validateNpyFile(fEmpty)
			assertEquals("Numpy .npy File", vr1.getFormatType())
			assertFalse(vr1.isValid())
			assertTrue(vr1.getIssues().get(0).contains("empty"),vr1.getIssues().get(0))
	'        System.out.println(vr1.toString());

			'Test directory (not zip file)
			Dim directory As New File(f, "dir")
			Dim created As Boolean = directory.mkdir()
			assertTrue(created)
			Dim vr2 As ValidationResult = Nd4jValidator.validateNpyFile(directory)
			assertEquals("Numpy .npy File", vr2.getFormatType())
			assertFalse(vr2.isValid())
			assertTrue(vr2.getIssues().get(0).contains("directory"),vr2.getIssues().get(0))
	'        System.out.println(vr2.toString());

			'Test non-numpy format:
			Dim fText As New File(f, "text.txt")
			FileUtils.writeStringToFile(fText, "Not a numpy .npy file", StandardCharsets.UTF_8)
			Dim vr3 As ValidationResult = Nd4jValidator.validateNpyFile(fText)
			assertEquals("Numpy .npy File", vr3.getFormatType())
			assertFalse(vr3.isValid())
			Dim s As String = vr3.getIssues().get(0)
			assertTrue(s.Contains("npy") AndAlso s.ToLower().Contains("numpy") AndAlso s.Contains("corrupt"),s)
	'        System.out.println(vr3.toString());

			'Test corrupted npy format:
			Dim fValid As File = (New ClassPathResource("numpy_arrays/arange_3,4_float32.npy")).File
			Dim numpyBytes() As SByte = FileUtils.readFileToByteArray(fValid)
			For i As Integer = 0 To 29
				numpyBytes(i) = 0
			Next i
			Dim fCorrupt As New File(f, "corrupt.npy")
			FileUtils.writeByteArrayToFile(fCorrupt, numpyBytes)

			Dim vr4 As ValidationResult = Nd4jValidator.validateNpyFile(fCorrupt)
			assertEquals("Numpy .npy File", vr4.getFormatType())
			assertFalse(vr4.isValid())
			s = vr4.getIssues().get(0)
			assertTrue(s.Contains("npy") AndAlso s.ToLower().Contains("numpy") AndAlso s.Contains("corrupt"),s)
	'        System.out.println(vr4.toString());


			'Test valid npy format:
			Dim vr5 As ValidationResult = Nd4jValidator.validateNpyFile(fValid)
			assertEquals("Numpy .npy File", vr5.getFormatType())
			assertTrue(vr5.isValid())
			assertNull(vr5.getIssues())
			assertNull(vr5.getException())
	'        System.out.println(vr4.toString());
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNpzValidation(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testNpzValidation(ByVal backend As Nd4jBackend)

			Dim f As File = testDir.toFile()

			'Test not existent file:
			Dim fNonExistent As New File("doesntExist.npz")
			Dim vr0 As ValidationResult = Nd4jValidator.validateNpzFile(fNonExistent)
			assertFalse(vr0.isValid())
			assertEquals("Numpy .npz File", vr0.getFormatType())
			assertTrue(vr0.getIssues().get(0).contains("exist"),vr0.getIssues().get(0))
	'        System.out.println(vr0.toString());

			'Test empty file:
			Dim fEmpty As New File(f, "empty.npz")
			fEmpty.createNewFile()
			assertTrue(fEmpty.exists())
			Dim vr1 As ValidationResult = Nd4jValidator.validateNpzFile(fEmpty)
			assertEquals("Numpy .npz File", vr1.getFormatType())
			assertFalse(vr1.isValid())
			assertTrue(vr1.getIssues().get(0).contains("empty"),vr1.getIssues().get(0))
	'        System.out.println(vr1.toString());

			'Test directory (not zip file)
			Dim directory As New File(f, "dir")
			Dim created As Boolean = directory.mkdir()
			assertTrue(created)
			Dim vr2 As ValidationResult = Nd4jValidator.validateNpzFile(directory)
			assertEquals("Numpy .npz File", vr2.getFormatType())
			assertFalse(vr2.isValid())
			assertTrue(vr2.getIssues().get(0).contains("directory"),vr2.getIssues().get(0))
	'        System.out.println(vr2.toString());

			'Test non-numpy format:
			Dim fText As New File(f, "text.txt")
			FileUtils.writeStringToFile(fText, "Not a numpy .npz file", StandardCharsets.UTF_8)
			Dim vr3 As ValidationResult = Nd4jValidator.validateNpzFile(fText)
			assertEquals("Numpy .npz File", vr3.getFormatType())
			assertFalse(vr3.isValid())
			Dim s As String = vr3.getIssues().get(0)
			assertTrue(s.Contains("npz") AndAlso s.ToLower().Contains("numpy") AndAlso s.Contains("corrupt"),s)
	'        System.out.println(vr3.toString());

			'Test corrupted npz format:
			Dim fValid As File = (New ClassPathResource("numpy_arrays/npz/float32.npz")).File
			Dim numpyBytes() As SByte = FileUtils.readFileToByteArray(fValid)
			For i As Integer = 0 To 29
				numpyBytes(i) = 0
			Next i
			Dim fCorrupt As New File(f, "corrupt.npz")
			FileUtils.writeByteArrayToFile(fCorrupt, numpyBytes)

			Dim vr4 As ValidationResult = Nd4jValidator.validateNpzFile(fCorrupt)
			assertEquals("Numpy .npz File", vr4.getFormatType())
			assertFalse(vr4.isValid())
			s = vr4.getIssues().get(0)
			assertTrue(s.Contains("npz") AndAlso s.ToLower().Contains("numpy") AndAlso s.Contains("corrupt"),s)
	'        System.out.println(vr4.toString());


			'Test valid npz format:
			Dim vr5 As ValidationResult = Nd4jValidator.validateNpzFile(fValid)
			assertEquals("Numpy .npz File", vr5.getFormatType())
			assertTrue(vr5.isValid())
			assertNull(vr5.getIssues())
			assertNull(vr5.getException())
	'        System.out.println(vr4.toString());
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNumpyTxtValidation(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testNumpyTxtValidation(ByVal backend As Nd4jBackend)
			Dim f As File = testDir.toFile()

			'Test not existent file:
			Dim fNonExistent As New File("doesntExist.txt")
			Dim vr0 As ValidationResult = Nd4jValidator.validateNumpyTxtFile(fNonExistent, " ", StandardCharsets.UTF_8)
			assertFalse(vr0.isValid())
			assertEquals("Numpy text file", vr0.getFormatType())
			assertTrue(vr0.getIssues().get(0).contains("exist"),vr0.getIssues().get(0))
	'        System.out.println(vr0.toString());

			'Test empty file:
			Dim fEmpty As New File(f, "empty.txt")
			fEmpty.createNewFile()
			assertTrue(fEmpty.exists())
			Dim vr1 As ValidationResult = Nd4jValidator.validateNumpyTxtFile(fEmpty, " ", StandardCharsets.UTF_8)
			assertEquals("Numpy text file", vr1.getFormatType())
			assertFalse(vr1.isValid())
			assertTrue(vr1.getIssues().get(0).contains("empty"),vr1.getIssues().get(0))
	'        System.out.println(vr1.toString());

			'Test directory (not zip file)
			Dim directory As New File(f, "dir")
			Dim created As Boolean = directory.mkdir()
			assertTrue(created)
			Dim vr2 As ValidationResult = Nd4jValidator.validateNumpyTxtFile(directory, " ", StandardCharsets.UTF_8)
			assertEquals("Numpy text file", vr2.getFormatType())
			assertFalse(vr2.isValid())
			assertTrue(vr2.getIssues().get(0).contains("directory"),vr2.getIssues().get(0))
	'        System.out.println(vr2.toString());

			'Test non-numpy format:
			Dim fText As New File(f, "text.txt")
			FileUtils.writeStringToFile(fText, "Not a numpy .text file", StandardCharsets.UTF_8)
			Dim vr3 As ValidationResult = Nd4jValidator.validateNumpyTxtFile(fText, " ", StandardCharsets.UTF_8)
			assertEquals("Numpy text file", vr3.getFormatType())
			assertFalse(vr3.isValid())
			Dim s As String = vr3.getIssues().get(0)
			assertTrue(s.Contains("text") AndAlso s.ToLower().Contains("numpy") AndAlso s.Contains("corrupt"),s)
	'        System.out.println(vr3.toString());

			'Test corrupted txt format:
			Dim fValid As File = (New ClassPathResource("numpy_arrays/txt/arange_3,4_float32.txt")).File
			Dim numpyBytes() As SByte = FileUtils.readFileToByteArray(fValid)
			For i As Integer = 0 To 29
				numpyBytes(i) = CSByte(Math.Truncate(AscW("a"c) + i))
			Next i
			Dim fCorrupt As New File(f, "corrupt.txt")
			FileUtils.writeByteArrayToFile(fCorrupt, numpyBytes)

			Dim vr4 As ValidationResult = Nd4jValidator.validateNumpyTxtFile(fCorrupt, " ", StandardCharsets.UTF_8)
			assertEquals("Numpy text file", vr4.getFormatType())
			assertFalse(vr4.isValid())
			s = vr4.getIssues().get(0)
			assertTrue(s.Contains("text") AndAlso s.ToLower().Contains("numpy") AndAlso s.Contains("corrupt"),s)
	'        System.out.println(vr4.toString());


			'Test valid npz format:
			Dim vr5 As ValidationResult = Nd4jValidator.validateNumpyTxtFile(fValid, " ", StandardCharsets.UTF_8)
			assertEquals("Numpy text file", vr5.getFormatType())
			assertTrue(vr5.isValid())
			assertNull(vr5.getIssues())
			assertNull(vr5.getException())
	'        System.out.println(vr4.toString());
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testValidateSameDiff(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testValidateSameDiff(ByVal backend As Nd4jBackend)
			Nd4j.DataType = DataType.FLOAT

			Dim f As File = testDir.toFile()
			Dim sd As SameDiff = SameDiff.create()
			Dim v As SDVariable = sd.placeHolder("x", DataType.FLOAT, 3,4)
			Dim loss As SDVariable = v.std(True)

			Dim fOrig As New File(f, "sd_fb.fb")
			sd.asFlatFile(fOrig)


			'Test not existent file:
			Dim fNonExistent As New File("doesntExist.fb")
			Dim vr0 As ValidationResult = Nd4jValidator.validateSameDiffFlatBuffers(fNonExistent)
			assertFalse(vr0.isValid())
			assertEquals("SameDiff FlatBuffers file", vr0.getFormatType())
			assertTrue(vr0.getIssues().get(0).contains("exist"),vr0.getIssues().get(0))
	'        System.out.println(vr0.toString());

			'Test empty file:
			Dim fEmpty As New File(f, "empty.fb")
			fEmpty.createNewFile()
			assertTrue(fEmpty.exists())
			Dim vr1 As ValidationResult = Nd4jValidator.validateSameDiffFlatBuffers(fEmpty)
			assertEquals("SameDiff FlatBuffers file", vr1.getFormatType())
			assertFalse(vr1.isValid())
			assertTrue(vr1.getIssues().get(0).contains("empty"),vr1.getIssues().get(0))
	'        System.out.println(vr1.toString());

			'Test directory (not zip file)
			Dim directory As New File(f, "dir")
			Dim created As Boolean = directory.mkdir()
			assertTrue(created)
			Dim vr2 As ValidationResult = Nd4jValidator.validateSameDiffFlatBuffers(directory)
			assertEquals("SameDiff FlatBuffers file", vr2.getFormatType())
			assertFalse(vr2.isValid())
			assertTrue(vr2.getIssues().get(0).contains("directory"),vr2.getIssues().get(0))
	'        System.out.println(vr2.toString());

			'Test non-flatbuffers
			Dim fText As New File(f, "text.fb")
			FileUtils.writeStringToFile(fText, "Not a flatbuffers file :)", StandardCharsets.UTF_8)
			Dim vr3 As ValidationResult = Nd4jValidator.validateSameDiffFlatBuffers(fText)
			assertEquals("SameDiff FlatBuffers file", vr3.getFormatType())
			assertFalse(vr3.isValid())
			Dim s As String = vr3.getIssues().get(0)
			assertTrue(s.Contains("FlatBuffers") AndAlso s.Contains("SameDiff") AndAlso s.Contains("corrupt"),s)
	'        System.out.println(vr3.toString());

			'Test corrupted flatbuffers format:
			Dim fbBytes() As SByte = FileUtils.readFileToByteArray(fOrig)
			For i As Integer = 0 To 29
				fbBytes(i) = CSByte(Math.Truncate(AscW("a"c) + i))
			Next i
			Dim fCorrupt As New File(f, "corrupt.fb")
			FileUtils.writeByteArrayToFile(fCorrupt, fbBytes)

			Dim vr4 As ValidationResult = Nd4jValidator.validateSameDiffFlatBuffers(fCorrupt)
			assertEquals("SameDiff FlatBuffers file", vr4.getFormatType())
			assertFalse(vr4.isValid())
			s = vr4.getIssues().get(0)
			assertTrue(s.Contains("FlatBuffers") AndAlso s.Contains("SameDiff") AndAlso s.Contains("corrupt"),s)
	'        System.out.println(vr4.toString());


			'Test valid npz format:
			Dim vr5 As ValidationResult = Nd4jValidator.validateSameDiffFlatBuffers(fOrig)
			assertEquals("SameDiff FlatBuffers file", vr5.getFormatType())
			assertTrue(vr5.isValid())
			assertNull(vr5.getIssues())
			assertNull(vr5.getException())
	'        System.out.println(vr4.toString());
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace