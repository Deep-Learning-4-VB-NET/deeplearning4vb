Imports System
Imports System.Collections.Generic
Imports System.Reflection
Imports Logger = DeepLearning4VB.org.slf4j.Logger
Imports LoggerFactory = DeepLearning4VB.org.slf4j.LoggerFactory
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports ND4VBClassLoading = DeepLearning4VB.org.nd4vb.common.config.ND4VBClassLoading

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

Namespace org.deeplearning4vb.common.config

    ''' <summary>
    ''' Type loading and reflective instantiation for DL4VB. Delegates type lookup to
    ''' <see cref="ND4VBClassLoading"/> (assemblies instead of Java class loaders).
    ''' </summary>
    Public NotInheritable Class DL4VBClassLoading
        Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(DL4VBClassLoading))

        Private Sub New()
        End Sub

        ''' <summary>The assemblies searched for types; shared with ND4VB.</summary>
        Public Shared Property Dl4vbAssemblies As Func(Of IEnumerable(Of Assembly))
            Get
                Return ND4VBClassLoading.Nd4vbAssemblies
            End Get
            Set(ByVal value As Func(Of IEnumerable(Of Assembly)))
                ND4VBClassLoading.Nd4vbAssemblies = value
                log.debug("Global assembly set for DL4VB was changed.")
            End Set
        End Property

        Public Shared Sub setDl4vbClassloaderFromClass(ByVal clazz As Type)
            ND4VBClassLoading.setNd4vbClassloaderFromClass(clazz)
        End Sub

        Public Shared Function classPresentOnClasspath(ByVal className As String) As Boolean
            Return ND4VBClassLoading.classPresentOnClasspath(className)
        End Function

        Public Shared Function loadClassByName(ByVal className As String) As Type
            Return ND4VBClassLoading.loadClassByName(className, True)
        End Function

        Public Shared Function loadClassByName(ByVal className As String, ByVal initialize As Boolean) As Type
            Dim t As Type = ND4VBClassLoading.loadClassByName(className, initialize)
            If t Is Nothing Then
                log.error("Cannot find class [{}] in the configured assemblies.", className)
            End If
            Return t
        End Function

        ''' <summary>Creates an instance of the named type with its parameterless constructor.</summary>
        Public Shared Function createNewInstance(Of T)(ByVal className As String) As T
            Return createNewInstance(Of T)(className, GetType(T), Type.EmptyTypes, New Object() {})
        End Function

        ''' <summary>Creates an instance of the named type; the constructor is chosen from the runtime types of the (non-null) arguments.</summary>
        Public Shared Function createNewInstance(Of T)(ByVal className As String, ByVal ParamArray args() As Object) As T
            Return createNewInstance(Of T)(className, GetType(T), args)
        End Function

        ''' <summary>Creates an instance of the named type, which must be assignable to <paramref name="superclass"/>.</summary>
        Public Shared Function createNewInstance(Of T)(ByVal className As String, ByVal superclass As Type, ByVal ParamArray args() As Object) As T
            If args Is Nothing Then args = New Object() {}
            Dim parameterTypes(args.Length - 1) As Type
            For i As Integer = 0 To args.Length - 1
                Dim arg As Object = args(i)
                If arg Is Nothing Then
                    Throw New ArgumentNullException("args", "Argument " & i & " is Nothing; use the overload that takes explicit parameter types")
                End If
                parameterTypes(i) = arg.GetType()
            Next i
            Return createNewInstance(Of T)(className, superclass, parameterTypes, args)
        End Function

        ''' <summary>Creates an instance of the named type using the constructor with exactly the given parameter types.</summary>
        Public Shared Function createNewInstance(Of T)(ByVal className As String, ByVal superclass As Type, ByVal parameterTypes() As Type, ByVal args() As Object) As T
            Dim loadedClass As Type = loadClassByName(className)
            Preconditions.checkNotNull(loadedClass, "Attempted to load class " & className & " but failed. No class found with this name.")
            If superclass IsNot Nothing AndAlso Not superclass.IsAssignableFrom(loadedClass) Then
                Throw New InvalidCastException("Class " & className & " is not a " & superclass.FullName)
            End If
            If parameterTypes Is Nothing Then parameterTypes = Type.EmptyTypes
            If args Is Nothing Then args = New Object() {}
            Dim ctor As ConstructorInfo = loadedClass.GetConstructor(BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.Instance, Nothing, parameterTypes, Nothing)
            If ctor Is Nothing Then
                Dim names As New List(Of String)()
                For Each pt As Type In parameterTypes
                    names.Add(pt.Name)
                Next pt
                log.error("Cannot create instance of class '{}': no constructor ({})", className, String.Join(", ", names))
                Throw New MissingMethodException("Class " & className & " has no constructor with parameter types (" & String.Join(", ", names) & ")")
            End If
            Try
                Return CType(ctor.Invoke(args), T)
            Catch e As TargetInvocationException
                log.error("Cannot create instance of class '{}'.", className, e.InnerException)
                Throw New InvalidOperationException("Cannot create instance of class '" & className & "'", If(e.InnerException, e))
            End Try
        End Function

        Public Shared Function loadService(Of S)() As IList(Of S)
            Return ND4VBClassLoading.loadService(Of S)()
        End Function
    End Class

End Namespace
