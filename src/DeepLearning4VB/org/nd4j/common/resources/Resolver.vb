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

Namespace org.nd4j.common.resources

    ''' <summary>
    ''' Resolves resource paths (test data, models) to local files. Implementations are consulted by
    ''' <see cref="Resources"/> in order of <see cref="priority"/>; register additional ones with
    ''' <c>ND4JClassLoading.registerService(Of Resolver)</c>. File locations are path strings.
    ''' </summary>
    Public Interface Resolver

        ''' <summary>Lower values are consulted first.</summary>
        Function priority() As Integer

        Function exists(ByVal resourcePath As String) As Boolean

        Function directoryExists(ByVal dirPath As String) As Boolean

        ''' <summary>The local file for the resource, downloading or extracting it if necessary.</summary>
        Function asFile(ByVal resourcePath As String) As String

        Function asStream(ByVal resourcePath As String) As Stream

        Sub copyDirectory(ByVal dirPath As String, ByVal destinationDir As String)

        Function hasLocalCache() As Boolean

        Function localCacheRoot() As String

        Function normalizePath(ByVal path As String) As String
    End Interface

End Namespace
