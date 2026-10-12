Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports ImageWritable = org.datavec.image.data.ImageWritable
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames

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
Namespace org.datavec.image.transform

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Json Yaml Test") @NativeTag @Tag(TagNames.FILE_IO) @Tag(TagNames.JACKSON_SERDE) class JsonYamlTest
	Friend Class JsonYamlTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Json Yaml Image Transform Process") void testJsonYamlImageTransformProcess() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testJsonYamlImageTransformProcess()
			Dim seed As Integer = 12345
			Dim random As New Random(seed)
			' from org.bytedeco.javacpp.opencv_imgproc
			Dim COLOR_BGR2Luv As Integer = 50
			Dim CV_BGR2GRAY As Integer = 6
			Dim itp As ImageTransformProcess = (New ImageTransformProcess.Builder()).colorConversionTransform(COLOR_BGR2Luv).cropImageTransform(10).equalizeHistTransform(CV_BGR2GRAY).flipImageTransform(0).resizeImageTransform(300, 300).rotateImageTransform(30).scaleImageTransform(3).warpImageTransform(CSng(0.5)).build()
			Dim asJson As String = itp.toJson()
			Dim asYaml As String = itp.toYaml()
			' System.out.println(asJson);
			' System.out.println("\n\n\n");
			' System.out.println(asYaml);
			Dim img As ImageWritable = TestImageTransform.makeRandomImage(0, 0, 3)
			Dim imgJson As New ImageWritable(img.Frame.clone())
			Dim imgYaml As New ImageWritable(img.Frame.clone())
			Dim imgAll As New ImageWritable(img.Frame.clone())
			Dim itpFromJson As ImageTransformProcess = ImageTransformProcess.fromJson(asJson)
			Dim itpFromYaml As ImageTransformProcess = ImageTransformProcess.fromYaml(asYaml)
			Dim transformList As IList(Of ImageTransform) = itp.getTransformList()
			Dim transformListJson As IList(Of ImageTransform) = itpFromJson.getTransformList()
			Dim transformListYaml As IList(Of ImageTransform) = itpFromYaml.getTransformList()
			For i As Integer = 0 To transformList.Count - 1
				Dim it As ImageTransform = transformList(i)
				Dim itJson As ImageTransform = transformListJson(i)
				Dim itYaml As ImageTransform = transformListYaml(i)
				Console.WriteLine(i & vbTab & it)
				img = it.transform(img)
				imgJson = itJson.transform(imgJson)
				imgYaml = itYaml.transform(imgYaml)
				If TypeOf it Is RandomCropTransform Then
					assertTrue(img.Frame.imageHeight = imgJson.Frame.imageHeight)
					assertTrue(img.Frame.imageWidth = imgJson.Frame.imageWidth)
					assertTrue(img.Frame.imageHeight = imgYaml.Frame.imageHeight)
					assertTrue(img.Frame.imageWidth = imgYaml.Frame.imageWidth)
				ElseIf TypeOf it Is FilterImageTransform Then
					assertEquals(img.Frame.imageHeight, imgJson.Frame.imageHeight)
					assertEquals(img.Frame.imageWidth, imgJson.Frame.imageWidth)
					assertEquals(img.Frame.imageChannels, imgJson.Frame.imageChannels)
					assertEquals(img.Frame.imageHeight, imgYaml.Frame.imageHeight)
					assertEquals(img.Frame.imageWidth, imgYaml.Frame.imageWidth)
					assertEquals(img.Frame.imageChannels, imgYaml.Frame.imageChannels)
				Else
					assertEquals(img, imgJson)
					assertEquals(img, imgYaml)
				End If
			Next i
			imgAll = itp.execute(imgAll)
			assertEquals(imgAll, img)
		End Sub
	End Class

End Namespace