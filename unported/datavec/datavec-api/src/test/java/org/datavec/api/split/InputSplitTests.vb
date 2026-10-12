Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Tag = org.junit.jupiter.api.Tag
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Files = org.nd4j.shade.guava.io.Files
Imports BalancedPathFilter = org.datavec.api.io.filters.BalancedPathFilter
Imports RandomPathFilter = org.datavec.api.io.filters.RandomPathFilter
Imports ParentPathLabelGenerator = org.datavec.api.io.labels.ParentPathLabelGenerator
Imports PatternPathLabelGenerator = org.datavec.api.io.labels.PatternPathLabelGenerator
Imports Test = org.junit.jupiter.api.Test
import static org.junit.jupiter.api.Assertions.assertEquals
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

Namespace org.datavec.api.split



	''' 
	''' <summary>
	''' @author saudet
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) public class InputSplitTests extends org.nd4j.common.tests.BaseND4JTest
	Public Class InputSplitTests
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSample() throws java.net.URISyntaxException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSample()
			Dim split As BaseInputSplit = New BaseInputSplitAnonymousInnerClass(Me)

			Dim random As New Random(42)
			Dim extensions() As String = {"tif", "jpg", "png", "jpeg", "bmp", "JPEG", "JPG", "TIF", "PNG"}
			Dim parentPathLabelGenerator As New ParentPathLabelGenerator()
			Dim patternPathLabelGenerator As New PatternPathLabelGenerator("_", 0)
			Dim randomPathFilter As New RandomPathFilter(random, extensions)
			Dim randomPathFilter2 As New RandomPathFilter(random, extensions, 7)
			Dim balancedPathFilter As New BalancedPathFilter(random, extensions, parentPathLabelGenerator, 0, 4, 0, 1)
			Dim balancedPathFilter2 As New BalancedPathFilter(random, extensions, patternPathLabelGenerator, 0, 4, 0, 1)

			Dim samples() As InputSplit = split.sample(randomPathFilter)
			assertEquals(1, samples.Length)
			assertEquals(11, samples(0).length())

			Dim samples2() As InputSplit = split.sample(randomPathFilter2)
			assertEquals(1, samples2.Length)
			assertEquals(7, samples2(0).length())

			Dim samples3() As InputSplit = split.sample(balancedPathFilter, 80, 20)
			assertEquals(2, samples3.Length)
			assertEquals(3, samples3(0).length())
			assertEquals(1, samples3(1).length())

			Dim samples4() As InputSplit = split.sample(balancedPathFilter2, 50, 50)
			assertEquals(2, samples4.Length)
			assertEquals(1, samples4(0).length())
			assertEquals(1, samples4(1).length())
		End Sub

		Private Class BaseInputSplitAnonymousInnerClass
			Inherits BaseInputSplit

			Private ReadOnly outerInstance As InputSplitTests

			Public Sub New(ByVal outerInstance As InputSplitTests)
				Me.outerInstance = outerInstance

				Dim paths() As String = {"label0/group1_img.tif", "label1/group1_img.jpg", "label2/group1_img.png", "label3/group1_img.jpeg", "label4/group1_img.bmp", "label5/group1_img.JPEG", "label0/group2_img.JPG", "label1/group2_img.TIF", "label2/group2_img.PNG", "label3/group2_img.jpg", "label4/group2_img.jpg", "label5/group2_img.wtf"}

				outerInstance.uriStrings = New List(Of String)(paths.Length)
				For Each s As String In paths
				outerInstance.uriStrings.Add("file:///" & s)
				Next s
			End Sub


					Public Overrides Sub updateSplitLocations(ByVal reset As Boolean)

					End Sub

					Public Overrides Function needsBootstrapForWrite() As Boolean
						Return False
					End Function

					Public Overrides Sub bootStrapForWrite()

					End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public OutputStream openOutputStreamFor(String location) throws Exception
					Public Overrides Function openOutputStreamFor(ByVal location As String) As Stream
						Return Nothing
					End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public InputStream openInputStreamFor(String location) throws Exception
					Public Overrides Function openInputStreamFor(ByVal location As String) As Stream
						Return Nothing
					End Function

					Public Overrides Sub reset()
						'No op
					End Sub

					Public Overrides Function resetSupported() As Boolean
						Return True
					End Function

		End Class


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testFileSplitBootstrap()
		Public Overridable Sub testFileSplitBootstrap()
			Dim tmpDir As File = Files.createTempDir()
			Dim boostrap As New FileSplit(tmpDir)
			assertTrue(boostrap.needsBootstrapForWrite())
			boostrap.bootStrapForWrite()
			assertTrue(tmpDir.listFiles() IsNot Nothing)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSampleNoBias() throws java.net.URISyntaxException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSampleNoBias()
			Dim random As New Random(7)
			Dim randomPathFilter As New RandomPathFilter(random, Nothing, 100)

			Dim paths(999) As URI
			For i As Integer = 0 To paths.Length - 1
				paths(i) = New URI("file:///label" & (i\100) & "/image" & i)
			Next i

			Dim notOnlyFirstLabel As Boolean = False
			Dim paths2() As URI = randomPathFilter.filter(paths)
			assertEquals(100, paths2.Length)
			For i As Integer = 0 To paths2.Length - 1
				If Not paths2(i).ToString().StartsWith("file:///label0/", StringComparison.Ordinal) Then
					notOnlyFirstLabel = True
				End If
			Next i
			assertTrue(notOnlyFirstLabel)
		End Sub

	End Class

End Namespace