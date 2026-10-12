Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports FileUtils = org.apache.commons.io.FileUtils
Imports IOUtils = org.apache.commons.io.IOUtils
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Adam = org.nd4j.linalg.learning.config.Adam
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

Namespace org.deeplearning4j.util



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.FILE_IO) public class ModelValidatorTests extends org.deeplearning4j.BaseDL4JTest
	Public Class ModelValidatorTests
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMultiLayerNetworkValidation(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMultiLayerNetworkValidation(ByVal testDir As Path)
			Dim f As File = testDir.toFile()

			'Test non-existent file
			Dim f0 As New File(f, "doesntExist.bin")
			Dim vr0 As ValidationResult = DL4JModelValidator.validateMultiLayerNetwork(f0)
			assertFalse(vr0.isValid())
			assertTrue(vr0.getIssues().get(0).contains("exist"))
			assertEquals("MultiLayerNetwork", vr0.getFormatType())
			assertEquals(GetType(MultiLayerNetwork), vr0.getFormatClass())
			assertNull(vr0.getException())
	'        System.out.println(vr0.toString());

			'Test empty file
			Dim f1 As New File(f, "empty.bin")
			f1.createNewFile()
			assertTrue(f1.exists())
			Dim vr1 As ValidationResult = DL4JModelValidator.validateMultiLayerNetwork(f1)
			assertFalse(vr1.isValid())
			assertTrue(vr1.getIssues().get(0).contains("empty"))
			assertEquals("MultiLayerNetwork", vr1.getFormatType())
			assertEquals(GetType(MultiLayerNetwork), vr1.getFormatClass())
			assertNull(vr1.getException())
	'        System.out.println(vr1.toString());

			'Test invalid zip file
			Dim f2 As New File(f, "notReallyZip.zip")
			FileUtils.writeStringToFile(f2, "This isn't actually a zip file", StandardCharsets.UTF_8)
			Dim vr2 As ValidationResult = DL4JModelValidator.validateMultiLayerNetwork(f2)
			assertFalse(vr2.isValid())
			Dim s As String = vr2.getIssues().get(0)
			assertTrue(s.Contains("zip") AndAlso s.Contains("corrupt"), s)
			assertEquals("MultiLayerNetwork", vr2.getFormatType())
			assertEquals(GetType(MultiLayerNetwork), vr2.getFormatClass())
			assertNotNull(vr2.getException())
	'        System.out.println(vr2.toString());

			'Test valid zip, but missing configuration
			Dim f3 As New File(f, "modelNoConfig.zip")
			SimpleNet.save(f3)
			Using zipfs As java.nio.file.FileSystem = java.nio.file.FileSystems.newFileSystem(java.net.URI.create("jar:" & f3.toURI().ToString()), java.util.Collections.singletonMap("create", "false"))
				Dim p As Path = zipfs.getPath(ModelSerializer.CONFIGURATION_JSON)
				Files.delete(p)
			End Using
			Dim vr3 As ValidationResult = DL4JModelValidator.validateMultiLayerNetwork(f3)
			assertFalse(vr3.isValid())
			s = vr3.getIssues().get(0)
			assertEquals(1, vr3.getIssues().size())
			assertTrue(s.Contains("missing") AndAlso s.Contains("configuration"), s)
			assertEquals("MultiLayerNetwork", vr3.getFormatType())
			assertEquals(GetType(MultiLayerNetwork), vr3.getFormatClass())
			assertNull(vr3.getException())
	'        System.out.println(vr3.toString());


			'Test valid sip, but missing params
			Dim f4 As New File(f, "modelNoParams.zip")
			SimpleNet.save(f4)
			Using zipfs As java.nio.file.FileSystem = java.nio.file.FileSystems.newFileSystem(java.net.URI.create("jar:" & f4.toURI().ToString()), java.util.Collections.singletonMap("create", "false"))
				Dim p As Path = zipfs.getPath(ModelSerializer.COEFFICIENTS_BIN)
				Files.delete(p)
			End Using
			Dim vr4 As ValidationResult = DL4JModelValidator.validateMultiLayerNetwork(f4)
			assertFalse(vr4.isValid())
			s = vr4.getIssues().get(0)
			assertEquals(1, vr4.getIssues().size())
			assertTrue(s.Contains("missing") AndAlso s.Contains("coefficients"), s)
			assertEquals("MultiLayerNetwork", vr4.getFormatType())
			assertEquals(GetType(MultiLayerNetwork), vr4.getFormatClass())
			assertNull(vr4.getException())
	'        System.out.println(vr4.toString());


			'Test valid model
			Dim f5 As New File(f, "modelValid.zip")
			SimpleNet.save(f5)
			Dim vr5 As ValidationResult = DL4JModelValidator.validateMultiLayerNetwork(f5)
			assertTrue(vr5.isValid())
			assertNull(vr5.getIssues())
			assertEquals("MultiLayerNetwork", vr5.getFormatType())
			assertEquals(GetType(MultiLayerNetwork), vr5.getFormatClass())
			assertNull(vr5.getException())
	'        System.out.println(vr5.toString());


			'Test valid model with corrupted JSON
			Dim f6 As New File(f, "modelBadJson.zip")
			SimpleNet.save(f6)
			Using zf As New java.util.zip.ZipFile(f5), zo As New java.util.zip.ZipOutputStream(New java.io.BufferedOutputStream(New FileStream(f6, FileMode.Create, FileAccess.Write)))
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: java.util.Iterator<? extends java.util.zip.ZipEntry> e = zf.entries();
				Dim e As IEnumerator(Of ZipEntry) = zf.entries()
				Do While e.MoveNext()
					Dim ze As ZipEntry = e.Current
					zo.putNextEntry(New ZipEntry(ze.getName()))
					If ze.getName().Equals(ModelSerializer.CONFIGURATION_JSON) Then
						zo.write("totally not valid json! - {}".GetBytes(Encoding.UTF8))
					Else
						Dim bytes() As SByte
						Using zis As New java.util.zip.ZipInputStream(zf.getInputStream(ze))
							bytes = IOUtils.toByteArray(zis)
						End Using
						zo.write(bytes)
	'                    System.out.println("WROTE: " + ze.getName());
					End If
				Loop
			End Using
			Dim vr6 As ValidationResult = DL4JModelValidator.validateMultiLayerNetwork(f6)
			assertFalse(vr6.isValid())
			s = vr6.getIssues().get(0)
			assertEquals(1, vr6.getIssues().size())
			assertTrue(s.Contains("JSON") AndAlso s.Contains("valid") AndAlso s.Contains("MultiLayerConfiguration"), s)
			assertEquals("MultiLayerNetwork", vr6.getFormatType())
			assertEquals(GetType(MultiLayerNetwork), vr6.getFormatClass())
			assertNotNull(vr6.getException())
	'        System.out.println(vr6.toString());
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testComputationGraphNetworkValidation(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testComputationGraphNetworkValidation(ByVal testDir As Path)
			Dim f As File = testDir.toFile()

			'Test non-existent file
			Dim f0 As New File(f, "doesntExist.bin")
			Dim vr0 As ValidationResult = DL4JModelValidator.validateComputationGraph(f0)
			assertFalse(vr0.isValid())
			assertTrue(vr0.getIssues().get(0).contains("exist"))
			assertEquals("ComputationGraph", vr0.getFormatType())
			assertEquals(GetType(ComputationGraph), vr0.getFormatClass())
			assertNull(vr0.getException())
	'        System.out.println(vr0.toString());

			'Test empty file
			Dim f1 As New File(f, "empty.bin")
			f1.createNewFile()
			assertTrue(f1.exists())
			Dim vr1 As ValidationResult = DL4JModelValidator.validateComputationGraph(f1)
			assertFalse(vr1.isValid())
			assertTrue(vr1.getIssues().get(0).contains("empty"))
			assertEquals("ComputationGraph", vr1.getFormatType())
			assertEquals(GetType(ComputationGraph), vr1.getFormatClass())
			assertNull(vr1.getException())
	'        System.out.println(vr1.toString());

			'Test invalid zip file
			Dim f2 As New File(f, "notReallyZip.zip")
			FileUtils.writeStringToFile(f2, "This isn't actually a zip file", StandardCharsets.UTF_8)
			Dim vr2 As ValidationResult = DL4JModelValidator.validateComputationGraph(f2)
			assertFalse(vr2.isValid())
			Dim s As String = vr2.getIssues().get(0)
			assertTrue(s.Contains("zip") AndAlso s.Contains("corrupt"), s)
			assertEquals("ComputationGraph", vr2.getFormatType())
			assertEquals(GetType(ComputationGraph), vr2.getFormatClass())
			assertNotNull(vr2.getException())
	'        System.out.println(vr2.toString());

			'Test valid zip, but missing configuration
			Dim f3 As New File(f, "modelNoConfig.zip")
			SimpleNet.save(f3)
			Using zipfs As java.nio.file.FileSystem = java.nio.file.FileSystems.newFileSystem(java.net.URI.create("jar:" & f3.toURI().ToString()), java.util.Collections.singletonMap("create", "false"))
				Dim p As Path = zipfs.getPath(ModelSerializer.CONFIGURATION_JSON)
				Files.delete(p)
			End Using
			Dim vr3 As ValidationResult = DL4JModelValidator.validateComputationGraph(f3)
			assertFalse(vr3.isValid())
			s = vr3.getIssues().get(0)
			assertEquals(1, vr3.getIssues().size())
			assertTrue(s.Contains("missing") AndAlso s.Contains("configuration"), s)
			assertEquals("ComputationGraph", vr3.getFormatType())
			assertEquals(GetType(ComputationGraph), vr3.getFormatClass())
			assertNull(vr3.getException())
	'        System.out.println(vr3.toString());


			'Test valid sip, but missing params
			Dim f4 As New File(f, "modelNoParams.zip")
			SimpleNet.save(f4)
			Using zipfs As java.nio.file.FileSystem = java.nio.file.FileSystems.newFileSystem(java.net.URI.create("jar:" & f4.toURI().ToString()), java.util.Collections.singletonMap("create", "false"))
				Dim p As Path = zipfs.getPath(ModelSerializer.COEFFICIENTS_BIN)
				Files.delete(p)
			End Using
			Dim vr4 As ValidationResult = DL4JModelValidator.validateComputationGraph(f4)
			assertFalse(vr4.isValid())
			s = vr4.getIssues().get(0)
			assertEquals(1, vr4.getIssues().size())
			assertTrue(s.Contains("missing") AndAlso s.Contains("coefficients"), s)
			assertEquals("ComputationGraph", vr4.getFormatType())
			assertEquals(GetType(ComputationGraph), vr4.getFormatClass())
			assertNull(vr4.getException())
	'        System.out.println(vr4.toString());


			'Test valid model
			Dim f5 As New File(f, "modelValid.zip")
			SimpleNet.save(f5)
			Dim vr5 As ValidationResult = DL4JModelValidator.validateComputationGraph(f5)
			assertTrue(vr5.isValid())
			assertNull(vr5.getIssues())
			assertEquals("ComputationGraph", vr5.getFormatType())
			assertEquals(GetType(ComputationGraph), vr5.getFormatClass())
			assertNull(vr5.getException())
	'        System.out.println(vr5.toString());


			'Test valid model with corrupted JSON
			Dim f6 As New File(f, "modelBadJson.zip")
			SimpleNet.save(f6)
			Using zf As New java.util.zip.ZipFile(f5), zo As New java.util.zip.ZipOutputStream(New java.io.BufferedOutputStream(New FileStream(f6, FileMode.Create, FileAccess.Write)))
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: java.util.Iterator<? extends java.util.zip.ZipEntry> e = zf.entries();
				Dim e As IEnumerator(Of ZipEntry) = zf.entries()
				Do While e.MoveNext()
					Dim ze As ZipEntry = e.Current
					zo.putNextEntry(New ZipEntry(ze.getName()))
					If ze.getName().Equals(ModelSerializer.CONFIGURATION_JSON) Then
						zo.write("totally not valid json! - {}".GetBytes(Encoding.UTF8))
					Else
						Dim bytes() As SByte
						Using zis As New java.util.zip.ZipInputStream(zf.getInputStream(ze))
							bytes = IOUtils.toByteArray(zis)
						End Using
						zo.write(bytes)
	'                    System.out.println("WROTE: " + ze.getName());
					End If
				Loop
			End Using
			Dim vr6 As ValidationResult = DL4JModelValidator.validateComputationGraph(f6)
			assertFalse(vr6.isValid())
			s = vr6.getIssues().get(0)
			assertEquals(1, vr6.getIssues().size())
			assertTrue(s.Contains("JSON") AndAlso s.Contains("valid") AndAlso s.Contains("ComputationGraphConfiguration"), s)
			assertEquals("ComputationGraph", vr6.getFormatType())
			assertEquals(GetType(ComputationGraph), vr6.getFormatClass())
			assertNotNull(vr6.getException())
	'        System.out.println(vr6.toString());
		End Sub



		Public Shared ReadOnly Property SimpleNet As MultiLayerNetwork
			Get
    
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).updater(New Adam(0.01)).list().layer((New DenseLayer.Builder()).nIn(10).nOut(10).build()).layer((New DenseLayer.Builder()).nIn(10).nOut(10).build()).layer((New OutputLayer.Builder()).nIn(10).nOut(10).build()).build()
    
				Dim net As New MultiLayerNetwork(conf)
				net.init()
    
				Return net
			End Get
		End Property

		Public Shared ReadOnly Property SimpleCG As ComputationGraph
			Get
				Return SimpleNet.toComputationGraph()
			End Get
		End Property
	End Class

End Namespace