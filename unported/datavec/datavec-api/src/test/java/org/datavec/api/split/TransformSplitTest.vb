Imports System.Collections.Generic
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
import static org.junit.jupiter.api.Assertions.assertArrayEquals
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith
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
Namespace org.datavec.api.split

	''' <summary>
	''' @author Ede Meijer
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Transform Split Test") @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) class TransformSplitTest extends org.nd4j.common.tests.BaseND4JTest
	Friend Class TransformSplitTest
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Transform") void testTransform() throws java.net.URISyntaxException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testTransform()
			Dim inputFiles As ICollection(Of URI) = asList(New URI("file:///foo/bar/../0.csv"), New URI("file:///foo/1.csv"))
			Dim SUT As InputSplit = New TransformSplit(New CollectionInputSplit(inputFiles), New URITransformAnonymousInnerClass(Me))
			assertArrayEquals(New URI() {
				New URI("file:///foo/0.csv"),
				New URI("file:///foo/1.csv")
			}, SUT.locations())
		End Sub

		Private Class URITransformAnonymousInnerClass
			Implements TransformSplit.URITransform

			Private ReadOnly outerInstance As TransformSplitTest

			Public Sub New(ByVal outerInstance As TransformSplitTest)
				Me.outerInstance = outerInstance
			End Sub


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public java.net.URI apply(java.net.URI uri) throws java.net.URISyntaxException
			Public Function apply(ByVal uri As URI) As URI Implements TransformSplit.URITransform.apply
				Return uri.normalize()
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Search Replace") void testSearchReplace() throws java.net.URISyntaxException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testSearchReplace()
			Dim inputFiles As ICollection(Of URI) = asList(New URI("file:///foo/1-in.csv"), New URI("file:///foo/2-in.csv"))
			Dim SUT As InputSplit = TransformSplit.ofSearchReplace(New CollectionInputSplit(inputFiles), "-in.csv", "-out.csv")
			assertArrayEquals(New URI() {
				New URI("file:///foo/1-out.csv"),
				New URI("file:///foo/2-out.csv")
			}, SUT.locations())
		End Sub
	End Class

End Namespace