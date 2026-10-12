Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports FileUtils = org.apache.commons.io.FileUtils
Imports FilenameUtils = org.apache.commons.io.FilenameUtils
Imports ArrayUtils = org.apache.commons.lang3.ArrayUtils
Imports org.junit.jupiter.api
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports Arguments = org.junit.jupiter.params.provider.Arguments
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports OpExecutioner = org.nd4j.linalg.api.ops.executioner.OpExecutioner
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.function
Imports Downloader = org.nd4j.common.resources.Downloader
Imports ArchiveUtils = org.nd4j.common.util.ArchiveUtils

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

Namespace org.nd4j.imports.tfgraphs




'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Tag(TagNames.LONG_TEST) @Tag(TagNames.LARGE_RESOURCES) public class TFGraphTestZooModels
	Public Class TFGraphTestZooModels
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir static java.nio.file.Path classTestDir;
		Friend Shared classTestDir As Path


		Public Shared ReadOnly IGNORE_REGEXES() As String = { "xlnet_cased_L-24_H-1024_A-16", "deeplabv3_xception_ade20k_train", "gpt-2_117M", "ssd_mobilenet_v1_0.75_depth_300x300_coco14_sync_2018_07_03", "ssd_mobilenet_v1_coco_2018_01_28", "faster_rcnn_resnet101_coco_2018_01_28", "deeplabv3_pascal_train_aug_2018_01_04"}

		Public Shared ReadOnly IGNORE_REGEXES_LIBND4J_EXEC_ONLY() As String = { "PorV-RNN", "temperature_bidirectional_63", "temperature_stacked_63", "text_gen_81", "deeplabv3_pascal_train_aug_2018_01_04" }


		Public Shared currentTestDir As File

		Public Shared ReadOnly BASE_MODEL_DL_DIR As New File(BaseModelDir, ".nd4jtests")

		Private Const BASE_DIR As String = "tf_graphs/zoo_models"
		Private Const MODEL_FILENAME As String = "tf_model.txt"

		Private inputs As IDictionary(Of String, INDArray)
		Private predictions As IDictionary(Of String, INDArray)
		Private modelName As String
		Private localTestDir As File

		Public Shared ReadOnly Property BaseModelDir As String
			Get
				Dim s As String = System.getProperty("org.nd4j.tests.modeldir")
				If s IsNot Nothing AndAlso s.Length > 0 Then
					Return s
				End If
				Return System.getProperty("user.home")
			End Get
		End Property

		Public Shared ReadOnly LOADER As BiFunction(Of File, String, SameDiff) = New RemoteCachingLoader()

		Public Class RemoteCachingLoader
			Implements BiFunction(Of File, String, SameDiff)

			Public Overridable Function apply(ByVal file As File, ByVal name As String) As SameDiff
				Try
					Dim s As String = FileUtils.readFileToString(file, StandardCharsets.UTF_8).replaceAll(vbCrLf,vbLf)
					Dim split() As String = s.Split(vbLf, True)
					If split.Length <> 2 AndAlso split.Length <> 3 Then
						Throw New System.InvalidOperationException("Invalid file: expected 2 lines with URL and MD5 hash, or 3 lines with " & "URL, MD5 hash and file name. Got " & split.Length & " lines")
					End If
					Dim url As String = split(0)
					Dim md5 As String = split(1)

					Dim localDir As New File(BASE_MODEL_DL_DIR, name)
					If Not localDir.exists() Then
						localDir.mkdirs()
					End If

					Dim filename As String = FilenameUtils.getName(url)
					Dim localFile As New File(localDir, filename)

					If localFile.exists() AndAlso Not Downloader.checkMD5OfFile(md5, localFile) Then
						log.info("Deleting local file: does not match MD5. {}", localFile.getAbsolutePath())
						localFile.delete()
					End If

					If Not localFile.exists() Then
						log.info("Starting resource download from: {} to {}", url, localFile.getAbsolutePath())
						Downloader.download(name, New URL(url), localFile, md5, 3)
					End If

					Dim modelFile As File

					If filename.EndsWith(".pb", StringComparison.Ordinal) Then
						modelFile = localFile
					ElseIf filename.EndsWith(".tar.gz", StringComparison.Ordinal) OrElse filename.EndsWith(".tgz", StringComparison.Ordinal) Then
						Dim files As IList(Of String) = ArchiveUtils.tarGzListFiles(localFile)
						Dim toExtract As String = Nothing
						If split.Length = 3 Then
							'Extract specific file
							toExtract = split(2)
						Else
							Dim pbFiles As IList(Of String) = New List(Of String)()
							For Each f As String In files
								If f.EndsWith(".pb", StringComparison.Ordinal) Then
									pbFiles.Add(f)
								End If
							Next f

							If pbFiles.Count = 1 Then
								toExtract = pbFiles(0)
							ElseIf pbFiles.Count = 0 Then
								toExtract = Nothing
							Else
								'Multiple files... try to find "frozen_inference_graph.pb"
								For Each str As String In pbFiles
									If str.EndsWith("frozen_inference_graph.pb", StringComparison.Ordinal) Then
										toExtract = str
									End If
								Next str
								If toExtract Is Nothing Then
									Throw New System.InvalidOperationException("Found multiple .pb files in archive: " & localFile & " - pb files in archive: " & pbFiles)
								End If
							End If
						End If
						Preconditions.checkState(toExtract IsNot Nothing, "Found no .pb files in archive: %s", localFile.getAbsolutePath())

						Preconditions.checkNotNull(currentTestDir, "currentTestDir has not been set (is null)")
						modelFile = New File(currentTestDir, "tf_model.pb")
						ArchiveUtils.tarGzExtractSingleFile(localFile, modelFile, toExtract)
					ElseIf filename.EndsWith(".zip", StringComparison.Ordinal) Then
						Throw New System.InvalidOperationException("ZIP support - not yet implemented")
					Else
						Throw New System.InvalidOperationException("Unknown format: " & filename)
					End If

'JAVA TO VB CONVERTER NOTE: The local variable apply was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
					Dim apply_Conflict As SameDiff = TFGraphTestAllHelper.LOADER.apply(modelFile, name)
					'"suggest" a GC before running the model to mitigate OOM
					System.GC.Collect()
					Return apply_Conflict
				Catch e As IOException
					Throw New Exception(e)
				End Try
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeAll public static void beforeClass()
		Public Shared Sub beforeClass()
			Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.SCOPE_PANIC
			Nd4j.setDefaultDataTypes(DataType.FLOAT, DataType.FLOAT)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static java.util.stream.Stream<org.junit.jupiter.params.provider.Arguments> data() throws java.io.IOException
		Public Shared Function data() As Stream(Of Arguments)
			classTestDir.toFile().mkdir()
			Dim baseDir As File = classTestDir.toFile() ' new File(System.getProperty("java.io.tmpdir"), UUID.randomUUID().toString());
			Dim params As IList(Of Object()) = TFGraphTestAllHelper.fetchTestParams(BASE_DIR, MODEL_FILENAME, TFGraphTestAllHelper.ExecuteWith.SAMEDIFF, baseDir)
			Return params.Select(AddressOf Arguments.of)
		End Function

		Private Shared isPPC As Boolean? = Nothing

		Public Shared ReadOnly Property PPC As Boolean
			Get
				If isPPC Is Nothing Then
					'/mnt/jenkins/workspace/deeplearning4j-bugfix-tests-linux-ppc64le-cpu/
					Dim f As New File("")
					Dim path As String = f.getAbsolutePath()
					log.info("Current directory: {}", path)
					isPPC = path.Contains("ppc64le")
				End If
				Return isPPC
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @ParameterizedTest @MethodSource("#data") public void testOutputOnly(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testOutputOnly(ByVal testDir As Path)
			If PPC Then
	'            
	'            Ugly hack to temporarily disable tests on PPC only on CI
	'            Issue logged here: https://github.com/eclipse/deeplearning4j/issues/7657
	'            These will be re-enabled for PPC once fixed - in the mean time, remaining tests will be used to detect and prevent regressions
	'             

				log.warn("TEMPORARILY SKIPPING TEST ON PPC ARCHITECTURE DUE TO KNOWN JVM CRASH ISSUES - SEE https://github.com/eclipse/deeplearning4j/issues/7657")
				'OpValidationSuite.ignoreFailing();
			End If

	'        if(!modelName.startsWith("ssd_mobilenet_v1_coco_2018_01_28")){
	'        if(!modelName.startsWith("ssd_mobilenet_v1_0.75_depth_300x300_coco14_sync_2018_07_03")){
	'        if(!modelName.startsWith("faster_rcnn_resnet101_coco_2018_01_28")){
	'            OpValidationSuite.ignoreFailing();
	'        }
			currentTestDir = testDir.toFile()

	'        Nd4j.getExecutioner().setProfilingMode(OpExecutioner.ProfilingMode.NAN_PANIC);
			Nd4j.MemoryManager.AutoGcWindow = 2000

			Nd4j.create(1)
			If ArrayUtils.contains(IGNORE_REGEXES, modelName) Then
				log.info(vbLf & vbTab & "IGNORE MODEL ON REGEX: {} - regex {}", modelName, modelName)
			   ' OpValidationSuite.ignoreFailing();
			End If

			Dim maxRE As Double? = 1e-3
			Dim minAbs As Double? = 1e-4
			currentTestDir = testDir.toFile()
			log.info("----- SameDiff Exec: {} -----", modelName)
			TFGraphTestAllHelper.checkOnlyOutput(inputs, predictions, modelName, BASE_DIR, MODEL_FILENAME, TFGraphTestAllHelper.ExecuteWith.SAMEDIFF, LOADER, maxRE, minAbs, False)

			If ArrayUtils.contains(IGNORE_REGEXES_LIBND4J_EXEC_ONLY, modelName) Then
				log.warn(vbLf & vbTab & "IGNORING MODEL FOR LIBND4J EXECUTION ONLY: ")
				Return
			End If

			'Libnd4j exec:
	'        
	'        //AB 2019/10/19 - Libnd4j execution disabled pending execution rewrite
	'        currentTestDir = testDir.newFolder();
	'        log.info("----- Libnd4j Exec: {} -----", modelName);
	'        TFGraphTestAllHelper.checkOnlyOutput(inputs, predictions, modelName, BASE_DIR, MODEL_FILENAME, TFGraphTestAllHelper.ExecuteWith.LIBND4J,
	'                LOADER, maxRE, minAbs);
	'         
		End Sub
	End Class

End Namespace