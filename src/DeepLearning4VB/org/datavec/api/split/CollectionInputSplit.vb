Imports System
Imports System.Collections.Generic
Imports System.IO
Imports DeepLearning4VB.org.datavec.api.util.files

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

    ''' <summary>An input split over a fixed collection of URIs.</summary>
    Public Class CollectionInputSplit
        Inherits BaseInputSplit

        Public Sub New(ByVal array() As Uri)
            Me.New(DirectCast(Arrays.asList(array), ICollection(Of Uri)))
        End Sub

        Public Sub New(ByVal list As ICollection(Of Uri))
            uriStrings = New List(Of String)(list.Count)
            For Each uri As Uri In list
                uriStrings.Add(URIUtil.toUriString(uri))
            Next uri
        End Sub

        Public Overrides Sub updateSplitLocations(ByVal reset As Boolean)
        End Sub

        Public Overrides Function needsBootstrapForWrite() As Boolean
            Return False
        End Function

        Public Overrides Sub bootStrapForWrite()
        End Sub

        Public Overrides Function openOutputStreamFor(ByVal location As String) As Stream
            Return Nothing
        End Function

        Public Overrides Function openInputStreamFor(ByVal location As String) As Stream
            Return Nothing
        End Function

        Public Overrides Function length() As Long
            Return uriStrings.Count
        End Function

        Public Overrides Sub reset()
            'No op
        End Sub

        Public Overrides Function resetSupported() As Boolean
            Return True
        End Function

    End Class

End Namespace
