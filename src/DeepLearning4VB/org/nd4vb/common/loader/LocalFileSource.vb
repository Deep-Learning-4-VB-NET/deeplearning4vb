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

Namespace org.nd4vb.common.loader

    ''' <summary>A <see cref="Source"/> backed by a file on the local file system.</summary>
    Public Class LocalFileSource
        Implements Source

        Private ReadOnly pathValue As String

        Public Sub New(ByVal path As String)
            pathValue = path
        End Sub

        ''' <summary>Opens a new buffered read stream over the file. The caller owns the stream.</summary>
        Public Overridable ReadOnly Property InputStream As Stream Implements Source.InputStream
            Get
                Return New BufferedStream(New FileStream(pathValue, FileMode.Open, FileAccess.Read))
            End Get
        End Property

        Public Overridable ReadOnly Property Path As String Implements Source.Path
            Get
                Return pathValue
            End Get
        End Property

        Public Function getInputStream() As Stream
            Return InputStream
        End Function

        Public Function getPath() As String
            Return pathValue
        End Function
    End Class

End Namespace
