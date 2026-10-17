Imports System

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

Namespace org.nd4vb.common.io

    ''' <summary>
    ''' A readable resource (file, embedded resource, ...). File locations are path strings and
    ''' URLs are <see cref="Uri"/> values; the Java original used java.io.File and java.net.URL.
    ''' </summary>
    Public Interface Resource
        Inherits InputStreamSource

        ''' <summary>Whether the resource physically exists.</summary>
        Function exists() As Boolean

        ''' <summary>Whether the content can be read (an existing, non-directory resource).</summary>
        ReadOnly Property Readable As Boolean

        ''' <summary>Whether this resource represents a handle with an open stream.</summary>
        ReadOnly Property Open As Boolean

        ''' <summary>The URI of the resource; throws <see cref="System.IO.FileNotFoundException"/> if it has none.</summary>
        ReadOnly Property URI As Uri

        ''' <summary>The file-system path of the resource; throws <see cref="System.IO.FileNotFoundException"/> if it is not a file.</summary>
        ReadOnly Property File As String

        ''' <summary>Content length in bytes.</summary>
        Function contentLength() As Long

        ''' <summary>Last-modified time as milliseconds since the Unix epoch.</summary>
        Function lastModified() As Long

        ''' <summary>Creates a resource relative to this one.</summary>
        Function createRelative(ByVal relativePath As String) As Resource

        ''' <summary>The file name (without directory), or Nothing.</summary>
        ReadOnly Property Filename As String

        ''' <summary>A description for error messages.</summary>
        ReadOnly Property Description As String
    End Interface

End Namespace
