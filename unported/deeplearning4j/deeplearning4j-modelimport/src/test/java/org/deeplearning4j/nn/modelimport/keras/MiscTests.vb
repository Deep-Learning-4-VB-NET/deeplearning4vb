Imports System
Imports System.IO
Imports System.Threading
Imports FileUtils = org.apache.commons.io.FileUtils
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports DL4JKerasModelValidator = org.deeplearning4j.nn.modelimport.keras.utils.DL4JKerasModelValidator
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports Timeout = org.junit.jupiter.api.Timeout
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports Resources = org.nd4j.common.resources.Resources
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
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

Namespace org.deeplearning4j.nn.modelimport.keras


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled @Tag(TagNames.FILE_IO) @Tag(TagNames.KERAS) @NativeTag public class MiscTests extends org.deeplearning4j.BaseDL4JTest
	Public Class MiscTests
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(60000L) public void testMultiThreadedLoading() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMultiThreadedLoading()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.io.File f = org.nd4j.common.resources.Resources.asFile("modelimport/keras/examples/mnist_mlp/mnist_mlp_tf_keras_1_model.h5");
			Dim f As File = Resources.asFile("modelimport/keras/examples/mnist_mlp/mnist_mlp_tf_keras_1_model.h5")

			Dim numThreads As Integer = 4
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.concurrent.CountDownLatch latch = new java.util.concurrent.CountDownLatch(numThreads);
			Dim latch As New System.Threading.CountdownEvent(numThreads)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.concurrent.atomic.AtomicInteger errors = new java.util.concurrent.atomic.AtomicInteger();
			Dim errors As New AtomicInteger()
			For i As Integer = 0 To numThreads - 1
				Call (New Thread(Sub()
				Try
					For i As Integer = 0 To 19
						'System.out.println("Iteration " + i + ": " + Thread.currentThread().getId());
						Try
							'System.out.println("About to load: " + Thread.currentThread().getId());
							Dim kerasModel_Conflict As KerasSequentialModel = (New KerasModel()).modelBuilder().modelHdf5Filename(f.getAbsolutePath()).enforceTrainingConfig(False).buildSequential()
							'System.out.println("Loaded Keras: " + Thread.currentThread().getId());

							Dim model As MultiLayerNetwork = kerasModel_Conflict.MultiLayerNetwork
							Thread.Sleep(50)
						Catch t As Exception
							t.printStackTrace()
							errors.getAndIncrement()
						End Try

					Next i
				Catch t As Exception
					t.printStackTrace()
					errors.getAndIncrement()
				Finally
					latch.Signal()
				End Try
				End Sub)).Start()
			Next i

			Dim result As Boolean = latch.await(30000, TimeUnit.MILLISECONDS)
			assertTrue(result,"Latch did not get to 0")
			assertEquals(0, errors.get(),"Number of errors")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(60000L) public void testLoadFromStream() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testLoadFromStream()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.io.File f = org.nd4j.common.resources.Resources.asFile("modelimport/keras/examples/mnist_mlp/mnist_mlp_tf_keras_1_model.h5");
			Dim f As File = Resources.asFile("modelimport/keras/examples/mnist_mlp/mnist_mlp_tf_keras_1_model.h5")

			Using [is] As Stream = New java.io.BufferedInputStream(New FileStream(f, FileMode.Open, FileAccess.Read))
				Dim model As MultiLayerNetwork = KerasModelImport.importKerasSequentialModelAndWeights([is])
				assertNotNull(model)
			End Using
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(60000L) public void testModelValidatorSequential(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testModelValidatorSequential(ByVal testDir As Path)
			Dim f As File = testDir.toFile()

			'Test not existent file:
			Dim fNonExistent As New File("doesntExist.h5")
			Dim vr0 As ValidationResult = DL4JKerasModelValidator.validateKerasSequential(fNonExistent)
			assertFalse(vr0.isValid())
			assertEquals("Keras Sequential Model HDF5", vr0.getFormatType())
			assertTrue(vr0.getIssues().get(0).contains("exist"),vr0.getIssues().get(0))
			Console.WriteLine(vr0.ToString())

			'Test empty file:
			Dim fEmpty As New File(f, "empty.h5")
			fEmpty.createNewFile()
			assertTrue(fEmpty.exists())
			Dim vr1 As ValidationResult = DL4JKerasModelValidator.validateKerasSequential(fEmpty)
			assertEquals("Keras Sequential Model HDF5", vr1.getFormatType())
			assertFalse(vr1.isValid())
			assertTrue(vr1.getIssues().get(0).contains("empty"),vr1.getIssues().get(0))
			Console.WriteLine(vr1.ToString())

			'Test directory (not zip file)
			Dim directory As New File(f, "dir")
			Dim created As Boolean = directory.mkdir()
			assertTrue(created)
			Dim vr2 As ValidationResult = DL4JKerasModelValidator.validateKerasSequential(directory)
			assertEquals("Keras Sequential Model HDF5", vr2.getFormatType())
			assertFalse(vr2.isValid())
			assertTrue(vr2.getIssues().get(0).contains("directory"),vr2.getIssues().get(0))
			Console.WriteLine(vr2.ToString())

			'Test Keras HDF5 format:
			Dim fText As New File(f, "text.txt")
			FileUtils.writeStringToFile(fText, "Not a hdf5 file :)", StandardCharsets.UTF_8)
			Dim vr3 As ValidationResult = DL4JKerasModelValidator.validateKerasSequential(fText)
			assertEquals("Keras Sequential Model HDF5", vr3.getFormatType())
			assertFalse(vr3.isValid())
			Dim s As String = vr3.getIssues().get(0)
			assertTrue(s.Contains("Keras") AndAlso s.Contains("Sequential") AndAlso s.Contains("corrupt"),s)
			Console.WriteLine(vr3.ToString())

			'Test corrupted npy format:
			Dim fValid As File = Resources.asFile("modelimport/keras/examples/mnist_mlp/mnist_mlp_tf_keras_1_model.h5")
			Dim numpyBytes() As SByte = FileUtils.readFileToByteArray(fValid)
			For i As Integer = 0 To 29
				numpyBytes(i) = 0
			Next i
			Dim fCorrupt As New File(f, "corrupt.h5")
			FileUtils.writeByteArrayToFile(fCorrupt, numpyBytes)

			Dim vr4 As ValidationResult = DL4JKerasModelValidator.validateKerasSequential(fCorrupt)
			assertEquals("Keras Sequential Model HDF5", vr4.getFormatType())
			assertFalse(vr4.isValid())
			s = vr4.getIssues().get(0)
			assertTrue(s.Contains("Keras") AndAlso s.Contains("Sequential") AndAlso s.Contains("corrupt"),s)
			Console.WriteLine(vr4.ToString())


			'Test valid npy format:
			Dim vr5 As ValidationResult = DL4JKerasModelValidator.validateKerasSequential(fValid)
			assertEquals("Keras Sequential Model HDF5", vr5.getFormatType())
			assertTrue(vr5.isValid())
			assertNull(vr5.getIssues())
			assertNull(vr5.getException())
			Console.WriteLine(vr4.ToString())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(60000L) public void testModelValidatorFunctional(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testModelValidatorFunctional(ByVal testDir As Path)
			Dim f As File = testDir.toFile()
			'String modelPath = "modelimport/keras/examples/functional_lstm/lstm_functional_tf_keras_2.h5";

			'Test not existent file:
			Dim fNonExistent As New File("doesntExist.h5")
			Dim vr0 As ValidationResult = DL4JKerasModelValidator.validateKerasFunctional(fNonExistent)
			assertFalse(vr0.isValid())
			assertEquals("Keras Functional Model HDF5", vr0.getFormatType())
			assertTrue(vr0.getIssues().get(0).contains("exist"),vr0.getIssues().get(0))
			Console.WriteLine(vr0.ToString())

			'Test empty file:
			Dim fEmpty As New File(f, "empty.h5")
			fEmpty.createNewFile()
			assertTrue(fEmpty.exists())
			Dim vr1 As ValidationResult = DL4JKerasModelValidator.validateKerasFunctional(fEmpty)
			assertEquals("Keras Functional Model HDF5", vr1.getFormatType())
			assertFalse(vr1.isValid())
			assertTrue(vr1.getIssues().get(0).contains("empty"),vr1.getIssues().get(0))
			Console.WriteLine(vr1.ToString())

			'Test directory (not zip file)
			Dim directory As New File(f, "dir")
			Dim created As Boolean = directory.mkdir()
			assertTrue(created)
			Dim vr2 As ValidationResult = DL4JKerasModelValidator.validateKerasFunctional(directory)
			assertEquals("Keras Functional Model HDF5", vr2.getFormatType())
			assertFalse(vr2.isValid())
			assertTrue(vr2.getIssues().get(0).contains("directory"),vr2.getIssues().get(0))
			Console.WriteLine(vr2.ToString())

			'Test Keras HDF5 format:
			Dim fText As New File(f, "text.txt")
			FileUtils.writeStringToFile(fText, "Not a hdf5 file :)", StandardCharsets.UTF_8)
			Dim vr3 As ValidationResult = DL4JKerasModelValidator.validateKerasFunctional(fText)
			assertEquals("Keras Functional Model HDF5", vr3.getFormatType())
			assertFalse(vr3.isValid())
			Dim s As String = vr3.getIssues().get(0)
			assertTrue(s.Contains("Keras") AndAlso s.Contains("Functional") AndAlso s.Contains("corrupt"),s)
			Console.WriteLine(vr3.ToString())

			'Test corrupted npy format:
			Dim fValid As File = Resources.asFile("modelimport/keras/examples/mnist_mlp/mnist_mlp_tf_keras_1_model.h5")
			Dim numpyBytes() As SByte = FileUtils.readFileToByteArray(fValid)
			For i As Integer = 0 To 29
				numpyBytes(i) = 0
			Next i
			Dim fCorrupt As New File(f, "corrupt.h5")
			FileUtils.writeByteArrayToFile(fCorrupt, numpyBytes)

			Dim vr4 As ValidationResult = DL4JKerasModelValidator.validateKerasFunctional(fCorrupt)
			assertEquals("Keras Functional Model HDF5", vr4.getFormatType())
			assertFalse(vr4.isValid())
			s = vr4.getIssues().get(0)
			assertTrue(s.Contains("Keras") AndAlso s.Contains("Functional") AndAlso s.Contains("corrupt"),s)
			Console.WriteLine(vr4.ToString())


			'Test valid npy format:
			Dim vr5 As ValidationResult = DL4JKerasModelValidator.validateKerasFunctional(fValid)
			assertEquals("Keras Functional Model HDF5", vr5.getFormatType())
			assertTrue(vr5.isValid())
			assertNull(vr5.getIssues())
			assertNull(vr5.getException())
			Console.WriteLine(vr4.ToString())
		End Sub
	End Class

End Namespace