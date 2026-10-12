Imports System.Collections.Generic
Imports VocLabelProvider = org.datavec.image.recordreader.objdetect.impl.VocLabelProvider
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
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

Namespace org.datavec.image.recordreader.objdetect


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.JAVA_ONLY) public class TestVocLabelProvider
	Public Class TestVocLabelProvider
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testVocLabelProvider(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testVocLabelProvider(ByVal testDir As Path)

			Dim f As File = testDir.toFile()
			Call (New ClassPathResource("datavec-data-image/voc/2007/")).copyDirectory(f)

			Dim path As String = f.getAbsolutePath() 'new ClassPathResource("voc/2007/JPEGImages/000005.jpg").getFile().getParentFile().getParent();

			Dim lp As ImageObjectLabelProvider = New VocLabelProvider(path)

			Dim img5 As String = (New File(f, "JPEGImages/000005.jpg")).getPath()

			Dim l5 As IList(Of ImageObject) = lp.getImageObjectsForPath(img5)
			assertEquals(5, l5.Count)

			Dim exp5 As IList(Of ImageObject) = New List(Of ImageObject) From {
				New ImageObject(263, 211, 324, 339, "chair"),
				New ImageObject(165, 264, 253, 372, "chair"),
				New ImageObject(5, 244, 67, 374, "chair"),
				New ImageObject(241, 194, 295, 299, "chair"),
				New ImageObject(277, 186, 312, 220, "chair")
			}

			assertEquals(exp5, l5)


			Dim img7 As String = (New File(f, "JPEGImages/000007.jpg")).getPath()
			Dim exp7 As IList(Of ImageObject) = Collections.singletonList(New ImageObject(141, 50, 500, 330, "car"))

			assertEquals(exp7, lp.getImageObjectsForPath(img7))
		End Sub

	End Class

End Namespace