Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports Logger = DeepLearning4VB.org.slf4j.Logger
Imports LoggerFactory = DeepLearning4VB.org.slf4j.LoggerFactory
Imports ND4VBClassLoading = DeepLearning4VB.org.nd4vb.common.config.ND4VBClassLoading
Imports StrumpfResolver = DeepLearning4VB.org.nd4vb.common.resources.strumpf.StrumpfResolver

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

Namespace org.nd4vb.common.resources

    ''' <summary>
    ''' Entry point for resolving test resources and data files through the registered
    ''' <see cref="Resolver"/>s (the Strumpf resolver plus any registered or discovered ones).
    ''' </summary>
    Public Class Resources
        Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(Resources))
        Private Shared instanceValue As Resources
        Private Shared ReadOnly gate As New Object()

        Protected Friend ReadOnly resolvers As IList(Of Resolver)

        Protected Friend Sub New()
            resolvers = New List(Of Resolver)()
            resolvers.Add(New StrumpfResolver())
            For Each resolver As Resolver In ND4VBClassLoading.loadService(Of Resolver)()
                If resolvers.Any(Function(r) r.GetType() Is resolver.GetType()) Then Continue For
                resolvers.Add(resolver)
            Next resolver
            ' Sort resolvers by priority: check resolvers with lower numbers first (stable sort)
            Dim sorted = resolvers.OrderBy(Function(r) r.priority()).ToList()
            resolvers.Clear()
            For Each r In sorted
                resolvers.Add(r)
            Next r
        End Sub

        Private Shared ReadOnly Property INSTANCE As Resources
            Get
                SyncLock gate
                    If instanceValue Is Nothing Then
                        instanceValue = New Resources()
                    End If
                    Return instanceValue
                End SyncLock
            End Get
        End Property

        ''' <summary>Discards the cached resolver list so that newly registered resolvers or changed system properties are picked up.</summary>
        Public Shared Sub reset()
            SyncLock gate
                instanceValue = Nothing
            End SyncLock
        End Sub

        Public Shared Function exists(ByVal resourcePath As String) As Boolean
            Return INSTANCE.resourceExists(resourcePath)
        End Function

        Public Shared Function asFile(ByVal resourcePath As String) As String
            Return INSTANCE.getAsFile(resourcePath)
        End Function

        Public Shared Function asStream(ByVal resourcePath As String) As Stream
            Return INSTANCE.getAsStream(resourcePath)
        End Function

        Public Shared Sub copyDirectory(ByVal directoryPath As String, ByVal destinationDir As String)
            INSTANCE.copyDir(directoryPath, destinationDir)
        End Sub

        Public Shared Function normalizePath(ByVal path As String) As String
            Return INSTANCE.normalize(path)
        End Function

        Protected Friend Overridable Function resourceExists(ByVal resourcePath As String) As Boolean
            If resourcePath Is Nothing Then Throw New ArgumentNullException(NameOf(resourcePath))
            For Each r As Resolver In resolvers
                If r.exists(resourcePath) Then
                    Return True
                End If
            Next r
            Return False
        End Function

        Protected Friend Overridable Function getAsFile(ByVal resourcePath As String) As String
            If resourcePath Is Nothing Then Throw New ArgumentNullException(NameOf(resourcePath))
            For Each r As Resolver In resolvers
                If r.exists(resourcePath) Then
                    Return r.asFile(resourcePath)
                End If
            Next r
            Throw New InvalidOperationException("Cannot resolve resource (not found): none of " & resolvers.Count & " resolvers can resolve resource """ & resourcePath & """ - available resolvers: " & DescribeResolvers())
        End Function

        Public Overridable Function getAsStream(ByVal resourcePath As String) As Stream
            If resourcePath Is Nothing Then Throw New ArgumentNullException(NameOf(resourcePath))
            For Each r As Resolver In resolvers
                If r.exists(resourcePath) Then
                    log.debug("Resolved resource with resolver {} for path {}", r.GetType().FullName, resourcePath)
                    Return r.asStream(resourcePath)
                End If
            Next r
            Throw New InvalidOperationException("Cannot resolve resource (not found): none of " & resolvers.Count & " resolvers can resolve resource """ & resourcePath & """ - available resolvers: " & DescribeResolvers())
        End Function

        Public Overridable Sub copyDir(ByVal directoryPath As String, ByVal destinationDir As String)
            For Each r As Resolver In resolvers
                If r.directoryExists(directoryPath) Then
                    r.copyDirectory(directoryPath, destinationDir)
                    Return
                End If
            Next r
        End Sub

        Public Overridable Function normalize(ByVal path As String) As String
            For Each r As Resolver In resolvers
                path = r.normalizePath(path)
            Next r
            Return path
        End Function

        Private Function DescribeResolvers() As String
            Return "[" & String.Join(", ", resolvers.Select(Function(r) r.GetType().Name)) & "]"
        End Function

    End Class

End Namespace
