Imports System
Imports System.Diagnostics
Imports Platform = com.sun.jna.Platform
Imports SneakyThrows = lombok.SneakyThrows
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports SparkComputationGraph = org.deeplearning4j.spark.impl.graph.SparkComputationGraph
Imports SparkDl4jMultiLayer = org.deeplearning4j.spark.impl.multilayer.SparkDl4jMultiLayer
Imports ParameterAveragingTrainingMaster = org.deeplearning4j.spark.impl.paramavg.ParameterAveragingTrainingMaster
Imports BaseSparkTest = org.deeplearning4j.spark.parameterserver.BaseSparkTest
Imports SharedTrainingMaster = org.deeplearning4j.spark.parameterserver.training.SharedTrainingMaster
Imports BeforeAll = org.junit.jupiter.api.BeforeAll
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports Downloader = org.nd4j.common.resources.Downloader
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
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

Namespace org.deeplearning4j.spark.parameterserver.modelimport.elephas


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) @NativeTag @Slf4j public class TestElephasImport extends org.deeplearning4j.spark.parameterserver.BaseSparkTest
	<Serializable>
	Public Class TestElephasImport
		Inherits BaseSparkTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeAll @SneakyThrows public static void beforeAll()
		Public Shared Sub beforeAll()
			If Platform.isWindows() Then
				Dim hadoopHome As New File(System.getProperty("java.io.tmpdir"),"hadoop-tmp")
				Dim binDir As New File(hadoopHome,"bin")
				If Not binDir.exists() Then
					binDir.mkdirs()
				End If
				Dim outputFile As New File(binDir,"winutils.exe")
				If Not outputFile.exists() Then
					log.info("Fixing spark for windows")
					Downloader.download("winutils.exe", URI.create("https://github.com/cdarlint/winutils/blob/master/hadoop-2.6.5/bin/winutils.exe?raw=true").toURL(), outputFile,"db24b404d2331a1bec7443336a5171f1",3)
				End If

				System.setProperty("hadoop.home.dir", hadoopHome.getAbsolutePath())
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testElephasSequentialImport() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testElephasSequentialImport()
			Dim modelPath As String = "modelimport/elephas/elephas_sequential.h5"
			Dim model As SparkDl4jMultiLayer = importElephasSequential(sc, modelPath)
			' System.out.println(model.getNetwork().summary());
			assertTrue(TypeOf model.TrainingMaster Is ParameterAveragingTrainingMaster)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testElephasSequentialImportAsync() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testElephasSequentialImportAsync()
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
		   Dim modelPath As String = "modelimport/elephas/elephas_sequential_async.h5"
			Dim model As SparkDl4jMultiLayer = importElephasSequential(sc, modelPath)
			' System.out.println(model.getNetwork().summary());
			assertTrue(TypeOf model.TrainingMaster Is SharedTrainingMaster)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private org.deeplearning4j.spark.impl.multilayer.SparkDl4jMultiLayer importElephasSequential(org.apache.spark.api.java.JavaSparkContext sc, String modelPath) throws Exception
		Private Function importElephasSequential(ByVal sc As JavaSparkContext, ByVal modelPath As String) As SparkDl4jMultiLayer

			Dim modelResource As New ClassPathResource(modelPath, GetType(TestElephasImport).getClassLoader())
			Dim modelFile As File = createTempFile("tempModel", "h5")
			Files.copy(modelResource.InputStream, modelFile.toPath(), StandardCopyOption.REPLACE_EXISTING)
			Dim model As SparkDl4jMultiLayer = ElephasModelImport.importElephasSequentialModelAndWeights(sc, modelFile.getAbsolutePath())
			Return model
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testElephasModelImport() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testElephasModelImport()

			Dim modelPath As String = "modelimport/elephas/elephas_model.h5"
			Dim model As SparkComputationGraph = importElephasModel(sc, modelPath)
			' System.out.println(model.getNetwork().summary());
			assertTrue(TypeOf model.TrainingMaster Is ParameterAveragingTrainingMaster)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testElephasJavaAveragingModelImport() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testElephasJavaAveragingModelImport()

			Dim modelPath As String = "modelimport/elephas/java_param_averaging_model.h5"
			Dim model As SparkComputationGraph = importElephasModel(sc, modelPath)
			' System.out.println(model.getNetwork().summary());
			Debug.Assert(TypeOf model.TrainingMaster Is ParameterAveragingTrainingMaster)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testElephasJavaSharingModelImport() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testElephasJavaSharingModelImport()

			Dim modelPath As String = "modelimport/elephas/java_param_sharing_model.h5"
			Dim model As SparkComputationGraph = importElephasModel(sc, modelPath)
			' System.out.println(model.getNetwork().summary());
			Debug.Assert(TypeOf model.TrainingMaster Is SharedTrainingMaster)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testElephasModelImportAsync() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testElephasModelImportAsync()

			Dim modelPath As String = "modelimport/elephas/elephas_model_async.h5"
			Dim model As SparkComputationGraph = importElephasModel(sc, modelPath)
			' System.out.println(model.getNetwork().summary());
			assertTrue(TypeOf model.TrainingMaster Is SharedTrainingMaster)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private org.deeplearning4j.spark.impl.graph.SparkComputationGraph importElephasModel(org.apache.spark.api.java.JavaSparkContext sc, String modelPath) throws Exception
		Private Function importElephasModel(ByVal sc As JavaSparkContext, ByVal modelPath As String) As SparkComputationGraph

			Dim modelResource As New ClassPathResource(modelPath, GetType(TestElephasImport).getClassLoader())
			Dim modelFile As File = createTempFile("tempModel", "h5")
			Files.copy(modelResource.InputStream, modelFile.toPath(), StandardCopyOption.REPLACE_EXISTING)
			Dim model As SparkComputationGraph = ElephasModelImport.importElephasModelAndWeights(sc, modelFile.getAbsolutePath())
			Return model
		End Function
	End Class

End Namespace