Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Logger = DeepLearning4VB.org.slf4j.Logger
Imports LoggerFactory = DeepLearning4VB.org.slf4j.LoggerFactory

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

Namespace org.nd4j.common.config

    ''' <summary>
    ''' Loads types by name and discovers service implementations.
    ''' .NET has no class loaders, so the Java "class loader" is modelled as the set of assemblies
    ''' that are searched (by default every assembly loaded in the current AppDomain), and
    ''' <c>java.util.ServiceLoader</c> is modelled as explicit registration through
    ''' <see cref="registerService"/> plus a scan of those assemblies for concrete implementations
    ''' with a public parameterless constructor.
    ''' </summary>
    Public NotInheritable Class ND4JClassLoading
        Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(ND4JClassLoading))
        Private Shared assemblySource As Func(Of IEnumerable(Of Assembly)) = AddressOf DefaultAssemblies
        Private Shared ReadOnly registered As New Dictionary(Of Type, List(Of Object))()
        Private Shared ReadOnly gate As New Object()

        Private Sub New()
        End Sub

        Private Shared Function DefaultAssemblies() As IEnumerable(Of Assembly)
            Return AppDomain.CurrentDomain.GetAssemblies().Where(Function(a) Not a.IsDynamic).ToArray()
        End Function

        ''' <summary>The assemblies searched by <see cref="loadClassByName(String)"/> and <see cref="loadService(Of S)"/>.</summary>
        Public Shared Property Nd4jAssemblies As Func(Of IEnumerable(Of Assembly))
            Get
                Return assemblySource
            End Get
            Set(ByVal value As Func(Of IEnumerable(Of Assembly)))
                assemblySource = If(value, New Func(Of IEnumerable(Of Assembly))(AddressOf DefaultAssemblies))
                log.debug("Global assembly set for ND4J was changed.")
            End Set
        End Property

        ''' <summary>Searches the assembly of the given type first, then the assemblies of the current AppDomain.</summary>
        Public Shared Sub setNd4jClassloaderFromClass(ByVal clazz As Type)
            If clazz Is Nothing Then Throw New ArgumentNullException(NameOf(clazz))
            Dim own As Assembly = clazz.Assembly
            Nd4jAssemblies = Function() New Assembly() {own}.Concat(DefaultAssemblies().Where(Function(a) a IsNot own)).ToArray()
        End Sub

        Public Shared Sub resetNd4jClassloader()
            assemblySource = AddressOf DefaultAssemblies
        End Sub

        Public Shared Function classPresentOnClasspath(ByVal className As String) As Boolean
            Return loadClassByName(className, False) IsNot Nothing
        End Function

        ''' <summary>Loads a type by full name (optionally assembly qualified). Returns Nothing when it cannot be found.</summary>
        Public Shared Function loadClassByName(ByVal className As String) As Type
            Return loadClassByName(className, True)
        End Function

        ''' <summary>Loads a type by full name; when <paramref name="initialize"/> is true its static constructor is run, as in Java.</summary>
        Public Shared Function loadClassByName(ByVal className As String, ByVal initialize As Boolean) As Type
            If String.IsNullOrEmpty(className) Then Return Nothing
            Dim t As Type = Nothing
            Try
                t = Type.GetType(className, False)
            Catch ex As Exception
                ' malformed assembly-qualified names etc. fall through to the assembly scan
            End Try
            If t Is Nothing Then
                For Each asm As Assembly In assemblySource()
                    Try
                        t = asm.GetType(className, False)
                    Catch ex As Exception
                        t = Nothing
                    End Try
                    If t IsNot Nothing Then Exit For
                Next asm
            End If
            If t Is Nothing Then
                log.trace("Cannot find class [{}] in the configured assemblies.", className)
                Return Nothing
            End If
            If initialize Then
                RuntimeHelpers.RunClassConstructor(t.TypeHandle)
            End If
            Return t
        End Function

        ''' <summary>Registers a service implementation returned by <see cref="loadService(Of S)"/> (replaces META-INF/services).</summary>
        Public Shared Sub registerService(Of S)(ByVal instance As S)
            If instance Is Nothing Then Throw New ArgumentNullException(NameOf(instance))
            SyncLock gate
                Dim list As List(Of Object) = Nothing
                If Not registered.TryGetValue(GetType(S), list) Then
                    list = New List(Of Object)()
                    registered(GetType(S)) = list
                End If
                list.Add(instance)
            End SyncLock
        End Sub

        ''' <summary>Removes every registration made with <see cref="registerService"/>.</summary>
        Public Shared Sub clearRegisteredServices()
            SyncLock gate
                registered.Clear()
            End SyncLock
        End Sub

        ''' <summary>
        ''' Returns the registered implementations of <typeparamref name="S"/> followed by one instance of
        ''' every concrete type assignable to <typeparamref name="S"/> with a public parameterless
        ''' constructor found in the configured assemblies.
        ''' </summary>
        Public Shared Function loadService(Of S)() As IList(Of S)
            Return loadService(GetType(S)).Cast(Of S)().ToList()
        End Function

        Public Shared Function loadService(ByVal serviceType As Type) As IList(Of Object)
            If serviceType Is Nothing Then Throw New ArgumentNullException(NameOf(serviceType))
            Dim result As New List(Of Object)()
            SyncLock gate
                Dim list As List(Of Object) = Nothing
                If registered.TryGetValue(serviceType, list) Then
                    result.AddRange(list)
                End If
            End SyncLock
            For Each asm As Assembly In assemblySource()
                For Each t As Type In SafeGetTypes(asm)
                    If t Is Nothing OrElse t.IsAbstract OrElse t.IsInterface OrElse t.IsGenericTypeDefinition Then Continue For
                    If Not serviceType.IsAssignableFrom(t) Then Continue For
                    If result.Any(Function(o) o.GetType() Is t) Then Continue For
                    Dim ctor As ConstructorInfo = t.GetConstructor(Type.EmptyTypes)
                    If ctor Is Nothing Then Continue For
                    Try
                        result.Add(ctor.Invoke(Nothing))
                    Catch ex As Exception
                        log.warn("Could not instantiate service implementation {}: {}", t.FullName, ex.Message)
                    End Try
                Next t
            Next asm
            Return result
        End Function

        Private Shared Function SafeGetTypes(ByVal asm As Assembly) As IEnumerable(Of Type)
            Try
                Return asm.GetTypes()
            Catch ex As ReflectionTypeLoadException
                Return ex.Types.Where(Function(t) t IsNot Nothing)
            Catch ex As Exception
                Return Enumerable.Empty(Of Type)()
            End Try
        End Function
    End Class

End Namespace
