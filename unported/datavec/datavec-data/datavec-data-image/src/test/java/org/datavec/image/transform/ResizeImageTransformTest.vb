Imports Frame = org.bytedeco.javacv.Frame
Imports ImageWritable = org.datavec.image.data.ImageWritable
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
import static org.junit.jupiter.api.Assertions.assertEquals
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
'ORIGINAL LINE: @DisplayName("Resize Image Transform Test") @NativeTag @Tag(TagNames.FILE_IO) class ResizeImageTransformTest
	Friend Class ResizeImageTransformTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach void setUp() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub setUp()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Resize Upscale 1") void testResizeUpscale1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testResizeUpscale1()
			Dim srcImg As ImageWritable = TestImageTransform.makeRandomImage(32, 32, 3)
			Dim transform As New ResizeImageTransform(200, 200)
			Dim dstImg As ImageWritable = transform.transform(srcImg)
			Dim f As Frame = dstImg.Frame
			assertEquals(f.imageWidth, 200)
			assertEquals(f.imageHeight, 200)
			Dim coordinates() As Single = { 100, 200 }
			Dim transformed() As Single = transform.query(coordinates)
			assertEquals(200f * 100 / 32, transformed(0), 0)
			assertEquals(200f * 200 / 32, transformed(1), 0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Resize Downscale") void testResizeDownscale() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testResizeDownscale()
			Dim srcImg As ImageWritable = TestImageTransform.makeRandomImage(571, 443, 3)
			Dim transform As New ResizeImageTransform(200, 200)
			Dim dstImg As ImageWritable = transform.transform(srcImg)
			Dim f As Frame = dstImg.Frame
			assertEquals(f.imageWidth, 200)
			assertEquals(f.imageHeight, 200)
			Dim coordinates() As Single = { 300, 400 }
			Dim transformed() As Single = transform.query(coordinates)
			assertEquals(200f * 300 / 443, transformed(0), 0)
			assertEquals(200f * 400 / 571, transformed(1), 0)
		End Sub
	End Class

End Namespace