Imports System.Threading
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports DL4JResources = org.deeplearning4j.common.resources.DL4JResources
Imports WorkspaceMode = org.deeplearning4j.nn.conf.WorkspaceMode
Imports LeNet = org.deeplearning4j.zoo.model.LeNet
Imports NASNet = org.deeplearning4j.zoo.model.NASNet
Imports SimpleCNN = org.deeplearning4j.zoo.model.SimpleCNN
Imports UNet = org.deeplearning4j.zoo.model.UNet
Imports COCOLabels = org.deeplearning4j.zoo.util.darknet.COCOLabels
Imports DarknetLabels = org.deeplearning4j.zoo.util.darknet.DarknetLabels
Imports ImageNetLabels = org.deeplearning4j.zoo.util.imagenet.ImageNetLabels
Imports org.junit.jupiter.api
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.deeplearning4j.zoo


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Tag(TagNames.FILE_IO) @Tag(TagNames.DL4J_OLD_API) @NativeTag @Tag(TagNames.LONG_TEST) @Tag(TagNames.LARGE_RESOURCES) public class TestDownload extends org.deeplearning4j.BaseDL4JTest
	Public Class TestDownload
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir static java.nio.file.Path sharedTempDir;
		Friend Shared sharedTempDir As Path

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return If(IntegrationTests, 480000L, 60000L)
			End Get
		End Property



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeAll public static void before() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Sub before()
			DL4JResources.setBaseDirectory(sharedTempDir.toFile())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterAll public static void after()
		Public Shared Sub after()
			DL4JResources.resetBaseDirectoryLocation()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDownloadAllModels() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testDownloadAllModels()

			' iterate through each available model
			Dim models() As ZooModel

			If IntegrationTests Then
				models = New ZooModel(){ LeNet.builder().build(), SimpleCNN.builder().build(), UNet.builder().build(), NASNet.builder().build()}
			Else
				models = New ZooModel(){ LeNet.builder().build(), SimpleCNN.builder().build()}
			End If



			For i As Integer = 0 To models.Length - 1
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				log.info("Testing zoo model " & models(i).GetType().FullName)
				Dim model As ZooModel = models(i)

				For Each pretrainedType As PretrainedType In System.Enum.GetValues(GetType(PretrainedType))
					If model.pretrainedAvailable(pretrainedType) Then
						model.initPretrained(pretrainedType)
					End If
				Next pretrainedType

				' clean up for current model
				Nd4j.WorkspaceManager.destroyAllWorkspacesForCurrentThread()
				System.GC.Collect()
				Thread.Sleep(1000)
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testLabelsDownload() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testLabelsDownload()
			assertEquals("person", (New COCOLabels()).getLabel(0))
			assertEquals("kit fox", (New DarknetLabels(True)).getLabel(0))
			assertEquals("n02119789", (New DarknetLabels(False)).getLabel(0))
			assertEquals("tench", (New ImageNetLabels()).getLabel(0))
		End Sub
	End Class

End Namespace