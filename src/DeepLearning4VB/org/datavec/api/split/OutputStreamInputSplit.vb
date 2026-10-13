Imports System
Imports System.Collections.Generic
Imports System.IO

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

    ''' <summary>An input split that wraps a single output stream (for record writers).</summary>
    Public Class OutputStreamInputSplit
        Implements InputSplit

        Private outputStreamValue As Stream

        Public Sub New(ByVal outputStream As Stream)
            Me.outputStreamValue = outputStream
        End Sub

        Public Function getOutputStream() As Stream
            Return outputStreamValue
        End Function

        Public Sub setOutputStream(ByVal outputStream As Stream)
            outputStreamValue = outputStream
        End Sub

        Public Property OutputStream As Stream
            Get
                Return outputStreamValue
            End Get
            Set(ByVal value As Stream)
                outputStreamValue = value
            End Set
        End Property

        Public Overridable Function canWriteToLocation(ByVal location As Uri) As Boolean Implements InputSplit.canWriteToLocation
            Return True
        End Function

        Public Overridable Function addNewLocation() As String Implements InputSplit.addNewLocation
            Return Nothing
        End Function

        Public Overridable Function addNewLocation(ByVal location As String) As String Implements InputSplit.addNewLocation
            Throw New NotSupportedException()
        End Function

        Public Overridable Sub updateSplitLocations(ByVal reset As Boolean) Implements InputSplit.updateSplitLocations
            Throw New NotSupportedException()
        End Sub

        Public Overridable Function needsBootstrapForWrite() As Boolean Implements InputSplit.needsBootstrapForWrite
            Return False
        End Function

        Public Overridable Sub bootStrapForWrite() Implements InputSplit.bootStrapForWrite
        End Sub

        Public Overridable Function openOutputStreamFor(ByVal location As String) As Stream Implements InputSplit.openOutputStreamFor
            Return outputStreamValue
        End Function

        Public Overridable Function openInputStreamFor(ByVal location As String) As Stream Implements InputSplit.openInputStreamFor
            Throw New NotSupportedException()
        End Function

        Public Overridable Function length() As Long Implements InputSplit.length
            Throw New NotSupportedException()
        End Function

        Public Overridable Function locations() As Uri() Implements InputSplit.locations
            Return New Uri() {}
        End Function

        Public Overridable Function locationsIterator() As IEnumerator(Of Uri) Implements InputSplit.locationsIterator
            Throw New NotSupportedException()
        End Function

        Public Overridable Function locationsPathIterator() As IEnumerator(Of String) Implements InputSplit.locationsPathIterator
            Throw New NotSupportedException()
        End Function

        Public Overridable Sub reset() Implements InputSplit.reset
            'No op
            If Not resetSupported() Then
                Throw New NotSupportedException("Reset not supported from streams")
            End If
        End Sub

        Public Overridable Function resetSupported() As Boolean Implements InputSplit.resetSupported
            Return False
        End Function

    End Class

End Namespace
