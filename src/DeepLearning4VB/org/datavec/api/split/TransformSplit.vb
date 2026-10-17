Imports System
Imports System.Collections.Generic
Imports System.IO
Imports DeepLearning4VB.org.datavec.api.util.files
Imports DeepLearning4VB.org.nd4vb.common.collection

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

    ''' <summary>An input split that applies a transformation to every URI of another split.</summary>
    Public Class TransformSplit
        Inherits BaseInputSplit

        Private ReadOnly sourceSplit As BaseInputSplit
        Private ReadOnly transform As URITransform

        ''' <summary>
        ''' Apply a given transformation to the raw URI objects
        ''' </summary>
        ''' <param name="sourceSplit"> the split with URIs to transform </param>
        ''' <param name="transform"> transform operation that returns a new URI based on an input URI </param>
        ''' <exception cref="UriFormatException"> thrown if the transformed URI is malformed </exception>
        Public Sub New(ByVal sourceSplit As BaseInputSplit, ByVal transform As URITransform)
            If sourceSplit Is Nothing Then Throw New ArgumentNullException(NameOf(sourceSplit), "sourceSplit is marked non-null but is null")
            If transform Is Nothing Then Throw New ArgumentNullException(NameOf(transform), "transform is marked non-null but is null")
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
        ''' <exception cref="UriFormatException"> thrown if the transformed URI is malformed </exception>
        Public Shared Function ofSearchReplace(ByVal sourceSplit As BaseInputSplit, ByVal search As String, ByVal replace As String) As TransformSplit
            If search Is Nothing Then Throw New ArgumentNullException(NameOf(search), "search is marked non-null but is null")
            If replace Is Nothing Then Throw New ArgumentNullException(NameOf(replace), "replace is marked non-null but is null")
            Return New TransformSplit(sourceSplit, New SearchReplaceTransform(search, replace))
        End Function

        Private Class SearchReplaceTransform
            Implements URITransform

            Private ReadOnly search As String
            Private ReadOnly replace As String

            Public Sub New(ByVal search As String, ByVal replace As String)
                Me.search = search
                Me.replace = replace
            End Sub

            Public Function apply(ByVal uri As Uri) As Uri Implements URITransform.apply
                Return URIUtil.createURI(URIUtil.toUriString(uri).Replace(search, replace))
            End Function

        End Class

        Private Sub initialize()
            splitLength = sourceSplit.length()
            uriStrings = New CompactHeapStringList()
            Dim iter As IEnumerator(Of Uri) = sourceSplit.locationsIterator()
            Do While iter.MoveNext()
                Dim uri As Uri = iter.Current
                uri = transform.apply(uri)
                uriStrings.Add(URIUtil.toUriString(uri))
            Loop
        End Sub

        ''' <summary>The number of locations, taken from the source split (Java's <c>length</c> field).</summary>
        Public Overrides Function length() As Long
            Return splitLength
        End Function

        Public Overrides Sub updateSplitLocations(ByVal reset As Boolean)
            sourceSplit.updateSplitLocations(reset)
        End Sub

        Public Overrides Function needsBootstrapForWrite() As Boolean
            Return sourceSplit.needsBootstrapForWrite()
        End Function

        Public Overrides Sub bootStrapForWrite()
            sourceSplit.bootStrapForWrite()
        End Sub

        Public Overrides Function openOutputStreamFor(ByVal location As String) As Stream
            Return sourceSplit.openOutputStreamFor(location)
        End Function

        Public Overrides Function openInputStreamFor(ByVal location As String) As Stream
            Return sourceSplit.openInputStreamFor(location)
        End Function

        Public Overrides Sub reset()
            'No op: BaseInputSplit doesn't support randomization directly, and TransformSplit doesn't either
        End Sub

        Public Overrides Function resetSupported() As Boolean
            Return True
        End Function

        ''' <summary>A transformation from one URI to another.</summary>
        Public Interface URITransform
            Function apply(ByVal uri As Uri) As Uri
        End Interface

    End Class

End Namespace
