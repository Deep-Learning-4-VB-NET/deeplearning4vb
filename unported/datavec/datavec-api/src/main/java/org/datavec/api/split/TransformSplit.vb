Imports System.Collections.Generic
Imports System.IO
Imports NonNull = lombok.NonNull
Imports CompactHeapStringList = org.nd4j.common.collection.CompactHeapStringList

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


	Public Class TransformSplit
		Inherits BaseInputSplit

		Private ReadOnly sourceSplit As BaseInputSplit
		Private ReadOnly transform As URITransform

		''' <summary>
		''' Apply a given transformation to the raw URI objects
		''' </summary>
		''' <param name="sourceSplit"> the split with URIs to transform </param>
		''' <param name="transform"> transform operation that returns a new URI based on an input URI </param>
		''' <exception cref="URISyntaxException"> thrown if the transformed URI is malformed </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public TransformSplit(@NonNull BaseInputSplit sourceSplit, @NonNull URITransform transform) throws java.net.URISyntaxException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Sub New(ByVal sourceSplit As BaseInputSplit, ByVal transform As URITransform)
			Me.sourceSplit = sourceSplit
			Me.transform = transform
			initialize()
		End Sub

		''' <summary>
		''' Static factory method, replace the string version of the URI with a simple search-replace pair
		''' </summary>
		''' <param name="sourceSplit"> the split with URIs to transform </param>
		''' <param name="search"> the string to search </param>
		''' <param name="replace"> the string to replace with </param>
		''' <exception cref="URISyntaxException"> thrown if the transformed URI is malformed </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static TransformSplit ofSearchReplace(@NonNull BaseInputSplit sourceSplit, @NonNull final String search, @NonNull final String replace) throws java.net.URISyntaxException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
		Public Shared Function ofSearchReplace(ByVal sourceSplit As BaseInputSplit, ByVal search As String, ByVal replace As String) As TransformSplit
			Return New TransformSplit(sourceSplit, New URITransformAnonymousInnerClass(search, replace))
		End Function

		Private Class URITransformAnonymousInnerClass
			Implements URITransform

			Private search As String
			Private replace As String

			Public Sub New(ByVal search As String, ByVal replace As String)
				Me.search = search
				Me.replace = replace
			End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public java.net.URI apply(java.net.URI uri) throws java.net.URISyntaxException
			Public Function apply(ByVal uri As URI) As URI Implements URITransform.apply
				Return New URI(uri.ToString().Replace(search, replace))
			End Function
		End Class

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void initialize() throws java.net.URISyntaxException
		Private Sub initialize()
			length_Conflict = sourceSplit.length()
			uriStrings = New CompactHeapStringList()
			Dim iter As IEnumerator(Of URI) = sourceSplit.locationsIterator()
			Do While iter.MoveNext()
				Dim uri As URI = iter.Current
				uri = transform.apply(uri)
				uriStrings.Add(uri.ToString())
			Loop
		End Sub


		Public Overrides Sub updateSplitLocations(ByVal reset As Boolean)
			sourceSplit.updateSplitLocations(reset)
		End Sub

		Public Overrides Function needsBootstrapForWrite() As Boolean
			Return sourceSplit.needsBootstrapForWrite()
		End Function

		Public Overrides Sub bootStrapForWrite()
			sourceSplit.bootStrapForWrite()
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public OutputStream openOutputStreamFor(String location) throws Exception
		Public Overrides Function openOutputStreamFor(ByVal location As String) As Stream
			Return sourceSplit.openOutputStreamFor(location)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public InputStream openInputStreamFor(String location) throws Exception
		Public Overrides Function openInputStreamFor(ByVal location As String) As Stream
			Return sourceSplit.openInputStreamFor(location)
		End Function

		Public Overrides Sub reset()
			'No op: BaseInputSplit doesn't support randomization directly, and TransformSplit doesn't either
		End Sub

		Public Overrides Function resetSupported() As Boolean
			Return True
		End Function

		Public Interface URITransform
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: java.net.URI apply(java.net.URI uri) throws java.net.URISyntaxException;
			Function apply(ByVal uri As URI) As URI
		End Interface
	End Class

End Namespace