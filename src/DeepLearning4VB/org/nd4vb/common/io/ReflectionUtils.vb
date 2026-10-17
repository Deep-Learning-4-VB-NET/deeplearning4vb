Imports System
Imports System.Collections.Generic
Imports System.Reflection
Imports System.Runtime.ExceptionServices
Imports System.Text.RegularExpressions

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
    ''' Reflection helpers, a copy of Spring's <c>org.springframework.util.ReflectionUtils</c> ported to
    ''' <see cref="System.Reflection"/>: Java <c>Class</c>/<c>Field</c>/<c>Method</c> are
    ''' <see cref="Type"/>/<see cref="FieldInfo"/>/<see cref="MethodInfo"/>.
    ''' <para>Differences from the Java original:
    ''' .NET reflection reads and writes non-public members without changing their accessibility, so
    ''' <c>makeAccessible</c> is a no-op; there are no checked exceptions or throws clauses, so
    ''' <see cref="declaresException"/> is always False and the <c>rethrow*</c> helpers just rethrow
    ''' (Java's <c>UndeclaredThrowableException</c> is never needed); there are no bridge methods, so
    ''' <see cref="NON_BRIDGED_METHODS"/> accepts every method; the <c>java.lang.Object</c> methods are
    ''' recognised by their .NET names (<c>Equals</c>, <c>GetHashCode</c>, <c>ToString</c>);
    ''' <c>invokeJdbcMethod</c> behaves like <c>invokeMethod</c> because every target exception is
    ''' rethrown unwrapped. The callback and filter interfaces are kept, and the <c>doWith*</c> methods
    ''' also accept <see cref="Action(Of T)"/> / <see cref="Func(Of T, TResult)"/> delegates.</para>
    ''' </summary>
    Public MustInherit Class ReflectionUtils

        ''' <summary>Members declared on the type itself, whatever their visibility (Java: <c>getDeclaredFields</c>/<c>getDeclaredMethods</c>).</summary>
        Private Shared ReadOnly DECLARED As BindingFlags = BindingFlags.DeclaredOnly Or BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.Static Or BindingFlags.Instance

        Private Shared ReadOnly CGLIB_RENAMED_METHOD_PATTERN As New Regex("^CGLIB\$(.+)\$\d+$", RegexOptions.CultureInvariant)

        Public Interface FieldFilter
            Function matches(ByVal field As FieldInfo) As Boolean
        End Interface

        Public Interface FieldCallback
            Sub doWith(ByVal field As FieldInfo)
        End Interface

        Public Interface MethodFilter
            Function matches(ByVal method As MethodInfo) As Boolean
        End Interface

        Public Interface MethodCallback
            Sub doWith(ByVal method As MethodInfo)
        End Interface

        Private NotInheritable Class DelegateFieldFilter
            Implements FieldFilter

            Private ReadOnly predicate As Func(Of FieldInfo, Boolean)

            Public Sub New(ByVal predicate As Func(Of FieldInfo, Boolean))
                Me.predicate = predicate
            End Sub

            Public Function matches(ByVal field As FieldInfo) As Boolean Implements FieldFilter.matches
                Return predicate(field)
            End Function
        End Class

        Private NotInheritable Class DelegateFieldCallback
            Implements FieldCallback

            Private ReadOnly action As Action(Of FieldInfo)

            Public Sub New(ByVal action As Action(Of FieldInfo))
                Me.action = action
            End Sub

            Public Sub doWith(ByVal field As FieldInfo) Implements FieldCallback.doWith
                action(field)
            End Sub
        End Class

        Private NotInheritable Class DelegateMethodFilter
            Implements MethodFilter

            Private ReadOnly predicate As Func(Of MethodInfo, Boolean)

            Public Sub New(ByVal predicate As Func(Of MethodInfo, Boolean))
                Me.predicate = predicate
            End Sub

            Public Function matches(ByVal method As MethodInfo) As Boolean Implements MethodFilter.matches
                Return predicate(method)
            End Function
        End Class

        Private NotInheritable Class DelegateMethodCallback
            Implements MethodCallback

            Private ReadOnly action As Action(Of MethodInfo)

            Public Sub New(ByVal action As Action(Of MethodInfo))
                Me.action = action
            End Sub

            Public Sub doWith(ByVal method As MethodInfo) Implements MethodCallback.doWith
                action(method)
            End Sub
        End Class

        ''' <summary>Instance fields that are neither static nor final (<c>ReadOnly</c>/<c>Const</c>).</summary>
        Public Shared ReadOnly COPYABLE_FIELDS As FieldFilter = New DelegateFieldFilter(Function(field As FieldInfo) Not field.IsStatic AndAlso Not field.IsInitOnly AndAlso Not field.IsLiteral)

        ''' <summary>Java excluded compiler-generated bridge methods; .NET has none, so every method matches.</summary>
        Public Shared ReadOnly NON_BRIDGED_METHODS As MethodFilter = New DelegateMethodFilter(Function(method As MethodInfo) True)

        ''' <summary>Methods not declared on <see cref="Object"/>.</summary>
        Public Shared ReadOnly USER_DECLARED_METHODS As MethodFilter = New DelegateMethodFilter(Function(method As MethodInfo) method.DeclaringType IsNot GetType(Object))

        Public Sub New()
        End Sub

        Public Shared Function findField(ByVal clazz As Type, ByVal name As String) As FieldInfo
            Return findField(clazz, name, Nothing)
        End Function

        ''' <summary>Finds a field by name and/or exact type, searching the type and its base types (not <see cref="Object"/>).</summary>
        Public Shared Function findField(ByVal clazz As Type, ByVal name As String, ByVal type As Type) As FieldInfo
            Assert.notNull(clazz, "Class must not be null")
            Assert.isTrue(name IsNot Nothing OrElse type IsNot Nothing, "Either name or opType of the field must be specified")
            Dim searchType As Type = clazz
            Do While searchType IsNot Nothing AndAlso searchType IsNot GetType(Object)
                For Each field As FieldInfo In searchType.GetFields(DECLARED)
                    If (name Is Nothing OrElse name.Equals(field.Name)) AndAlso (type Is Nothing OrElse type.Equals(field.FieldType)) Then
                        Return field
                    End If
                Next field
                searchType = searchType.BaseType
            Loop
            Return Nothing
        End Function

        Public Shared Sub setField(ByVal field As FieldInfo, ByVal target As Object, ByVal value As Object)
            Try
                field.SetValue(target, value)
            Catch ex As MemberAccessException
                handleReflectionException(ex)
                Throw New InvalidOperationException("Unexpected reflection exception - " & ex.GetType().FullName & ": " & ex.Message)
            End Try
        End Sub

        Public Shared Function getField(ByVal field As FieldInfo, ByVal target As Object) As Object
            Try
                Return field.GetValue(target)
            Catch ex As MemberAccessException
                handleReflectionException(ex)
                Throw New InvalidOperationException("Unexpected reflection exception - " & ex.GetType().FullName & ": " & ex.Message)
            End Try
        End Function

        Public Shared Function findMethod(ByVal clazz As Type, ByVal name As String) As MethodInfo
            Return findMethod(clazz, name, New Type() {})
        End Function

        ''' <summary>
        ''' Finds a method by name and exact parameter types, searching the type, its base types and (for
        ''' interfaces) the inherited interfaces. A Nothing <paramref name="paramTypes"/> matches any parameters.
        ''' </summary>
        Public Shared Function findMethod(ByVal clazz As Type, ByVal name As String, ByVal ParamArray paramTypes() As Type) As MethodInfo
            Assert.notNull(clazz, "Class must not be null")
            Assert.notNull(name, "Method name must not be null")
            Dim searchType As Type = clazz
            Do While searchType IsNot Nothing
                Dim methods() As MethodInfo = If(searchType.IsInterface, searchType.GetMethods(), searchType.GetMethods(DECLARED))
                For Each method As MethodInfo In methods
                    If name.Equals(method.Name) AndAlso (paramTypes Is Nothing OrElse parameterTypesMatch(method, paramTypes)) Then
                        Return method
                    End If
                Next method
                If searchType.IsInterface Then
                    ' Java's Class.getMethods() on an interface includes the methods of its super-interfaces
                    For Each superIfc As Type In searchType.GetInterfaces()
                        Dim inherited As MethodInfo = findMethod(superIfc, name, paramTypes)
                        If inherited IsNot Nothing Then
                            Return inherited
                        End If
                    Next superIfc
                End If
                searchType = searchType.BaseType
            Loop
            Return Nothing
        End Function

        Private Shared Function parameterTypesOf(ByVal method As MethodBase) As Type()
            Dim parameters() As ParameterInfo = method.GetParameters()
            Dim types(parameters.Length - 1) As Type
            For i As Integer = 0 To parameters.Length - 1
                types(i) = parameters(i).ParameterType
            Next i
            Return types
        End Function

        Private Shared Function parameterTypesMatch(ByVal method As MethodBase, ByVal paramTypes() As Type) As Boolean
            Dim parameters() As ParameterInfo = method.GetParameters()
            If parameters.Length <> paramTypes.Length Then
                Return False
            End If
            For i As Integer = 0 To parameters.Length - 1
                If parameters(i).ParameterType IsNot paramTypes(i) Then
                    Return False
                End If
            Next i
            Return True
        End Function

        ''' <summary>Invokes the method; an exception thrown by the target is rethrown unwrapped, reflection failures become <see cref="InvalidOperationException"/>.</summary>
        Public Shared Function invokeMethod(ByVal method As MethodInfo, ByVal target As Object, ByVal ParamArray args() As Object) As Object
            Try
                Return method.Invoke(target, args)
            Catch ex As Exception
                handleReflectionException(ex)
                Throw New InvalidOperationException("Should never get here")
            End Try
        End Function

        ''' <summary>
        ''' Java rethrew a target <c>SQLException</c> unwrapped and handled everything else like
        ''' <see cref="invokeMethod"/>; in .NET every target exception is rethrown unwrapped, so the
        ''' two behave the same.
        ''' </summary>
        Public Shared Function invokeJdbcMethod(ByVal method As MethodInfo, ByVal target As Object, ByVal ParamArray args() As Object) As Object
            Try
                Return method.Invoke(target, args)
            Catch ex As TargetInvocationException
                handleInvocationTargetException(ex)
            Catch ex As MemberAccessException
                handleReflectionException(ex)
            End Try
            Throw New InvalidOperationException("Should never get here")
        End Function

        ''' <summary>
        ''' Maps reflection failures the way Java did: a missing member or an access failure becomes
        ''' <see cref="InvalidOperationException"/> (Java: <c>IllegalStateException</c>), a
        ''' <see cref="TargetInvocationException"/> rethrows the target's exception, anything else is rethrown.
        ''' </summary>
        Public Shared Sub handleReflectionException(ByVal ex As Exception)
            If TypeOf ex Is MissingMemberException Then
                Throw New InvalidOperationException("Method not found: " & ex.Message, ex)
            End If
            If TypeOf ex Is MemberAccessException Then
                Throw New InvalidOperationException("Could not access method: " & ex.Message, ex)
            End If
            If TypeOf ex Is TargetInvocationException Then
                handleInvocationTargetException(DirectCast(ex, TargetInvocationException))
            End If
            rethrowRuntimeException(ex)
        End Sub

        ''' <summary>Rethrows the exception thrown by the invoked member (the wrapper itself if there is none).</summary>
        Public Shared Sub handleInvocationTargetException(ByVal ex As TargetInvocationException)
            rethrowRuntimeException(If(ex.InnerException, ex))
        End Sub

        ''' <summary>
        ''' Rethrows the exception keeping its stack trace. Java only rethrew <c>RuntimeException</c>s
        ''' and <c>Error</c>s as they were and wrapped checked exceptions; .NET has no checked exceptions.
        ''' </summary>
        Public Shared Sub rethrowRuntimeException(ByVal ex As Exception)
            If ex Is Nothing Then
                Throw New ArgumentNullException(NameOf(ex))
            End If
            ExceptionDispatchInfo.Capture(ex).Throw()
        End Sub

        ''' <summary>Same as <see cref="rethrowRuntimeException"/> (Java: also rethrew checked exceptions as they were).</summary>
        Public Shared Sub rethrowException(ByVal ex As Exception)
            rethrowRuntimeException(ex)
        End Sub

        ''' <summary>.NET methods have no throws clause, so this is always False (Java inspected <c>getExceptionTypes()</c>).</summary>
        Public Shared Function declaresException(ByVal method As MethodInfo, ByVal exceptionType As Type) As Boolean
            Assert.notNull(method, "Method must not be null")
            Return False
        End Function

        ''' <summary>True for public static fields that are <c>ReadOnly</c> or <c>Const</c>.</summary>
        Public Shared Function isPublicStaticFinal(ByVal field As FieldInfo) As Boolean
            Return field.IsPublic AndAlso field.IsStatic AndAlso (field.IsInitOnly OrElse field.IsLiteral)
        End Function

        ''' <summary>True for <c>Equals(Object)</c> (Java: <c>equals(Object)</c>).</summary>
        Public Shared Function isEqualsMethod(ByVal method As MethodInfo) As Boolean
            If method Is Nothing OrElse Not method.Name.Equals("Equals") Then
                Return False
            End If
            Dim parameters() As ParameterInfo = method.GetParameters()
            Return parameters.Length = 1 AndAlso parameters(0).ParameterType Is GetType(Object)
        End Function

        ''' <summary>True for <c>GetHashCode()</c> (Java: <c>hashCode()</c>).</summary>
        Public Shared Function isHashCodeMethod(ByVal method As MethodInfo) As Boolean
            Return method IsNot Nothing AndAlso method.Name.Equals("GetHashCode") AndAlso method.GetParameters().Length = 0
        End Function

        ''' <summary>True for <c>ToString()</c> (Java: <c>toString()</c>).</summary>
        Public Shared Function isToStringMethod(ByVal method As MethodInfo) As Boolean
            Return method IsNot Nothing AndAlso method.Name.Equals("ToString") AndAlso method.GetParameters().Length = 0
        End Function

        ''' <summary>True if <see cref="Object"/> declares a method with the same name and parameter types.</summary>
        Public Shared Function isObjectMethod(ByVal method As MethodInfo) As Boolean
            If method Is Nothing Then
                Return False
            End If
            Try
                Return GetType(Object).GetMethod(method.Name, DECLARED, Nothing, parameterTypesOf(method), Nothing) IsNot Nothing
            Catch ex As AmbiguousMatchException
                Return False
            End Try
        End Function

        ''' <summary>True if the method name has the shape of a CGLIB-renamed method (<c>CGLIB$name$0</c>).</summary>
        Public Shared Function isCglibRenamedMethod(ByVal renamedMethod As MethodInfo) As Boolean
            Return CGLIB_RENAMED_METHOD_PATTERN.IsMatch(renamedMethod.Name)
        End Function

        ''' <summary>No-op: .NET reflection can read and write non-public and read-only fields without <c>setAccessible</c>.</summary>
        Public Shared Sub makeAccessible(ByVal field As FieldInfo)
        End Sub

        ''' <summary>No-op: .NET reflection can invoke non-public methods without <c>setAccessible</c>.</summary>
        Public Shared Sub makeAccessible(ByVal method As MethodInfo)
        End Sub

        ''' <summary>No-op: .NET reflection can invoke non-public constructors without <c>setAccessible</c>.</summary>
        Public Shared Sub makeAccessible(ByVal ctor As ConstructorInfo)
        End Sub

        Public Shared Sub doWithMethods(ByVal clazz As Type, ByVal mc As MethodCallback)
            doWithMethods(clazz, mc, DirectCast(Nothing, MethodFilter))
        End Sub

        ''' <summary>
        ''' Calls the callback for every method declared on the type, its base types and (for interfaces)
        ''' its super-interfaces that passes the filter (Nothing accepts all), most derived type first.
        ''' </summary>
        Public Shared Sub doWithMethods(ByVal clazz As Type, ByVal mc As MethodCallback, ByVal mf As MethodFilter)
            For Each method As MethodInfo In clazz.GetMethods(DECLARED)
                If mf Is Nothing OrElse mf.matches(method) Then
                    Try
                        mc.doWith(method)
                    Catch ex As MemberAccessException
                        Throw New InvalidOperationException("Shouldn't be illegal to access method '" & method.Name & "': " & ex.ToString(), ex)
                    End Try
                End If
            Next method
            If clazz.BaseType IsNot Nothing Then
                doWithMethods(clazz.BaseType, mc, mf)
            ElseIf clazz.IsInterface Then
                For Each superIfc As Type In clazz.GetInterfaces()
                    doWithMethods(superIfc, mc, mf)
                Next superIfc
            End If
        End Sub

        Public Shared Sub doWithMethods(ByVal clazz As Type, ByVal mc As Action(Of MethodInfo))
            doWithMethods(clazz, New DelegateMethodCallback(mc), DirectCast(Nothing, MethodFilter))
        End Sub

        Public Shared Sub doWithMethods(ByVal clazz As Type, ByVal mc As Action(Of MethodInfo), ByVal mf As Func(Of MethodInfo, Boolean))
            Dim filter As MethodFilter = Nothing
            If mf IsNot Nothing Then
                filter = New DelegateMethodFilter(mf)
            End If
            doWithMethods(clazz, New DelegateMethodCallback(mc), filter)
        End Sub

        Public Shared Sub doWithMethods(ByVal clazz As Type, ByVal mc As Action(Of MethodInfo), ByVal mf As MethodFilter)
            doWithMethods(clazz, New DelegateMethodCallback(mc), mf)
        End Sub

        ''' <summary>All methods declared on the type and its base types / super-interfaces, most derived type first.</summary>
        Public Shared Function getAllDeclaredMethods(ByVal leafClass As Type) As MethodInfo()
            Dim methods As New List(Of MethodInfo)(32)
            doWithMethods(leafClass, Sub(method As MethodInfo) methods.Add(method))
            Return methods.ToArray()
        End Function

        ''' <summary>
        ''' Like <see cref="getAllDeclaredMethods"/> but with one entry per signature: the most derived
        ''' declaration wins (a base declaration with a wider return type replaces it, as in Java).
        ''' </summary>
        Public Shared Function getUniqueDeclaredMethods(ByVal leafClass As Type) As MethodInfo()
            Dim methods As New List(Of MethodInfo)(32)
            doWithMethods(leafClass, Sub(method As MethodInfo) addUniqueMethod(methods, method))
            Return methods.ToArray()
        End Function

        Private Shared Sub addUniqueMethod(ByVal methods As List(Of MethodInfo), ByVal method As MethodInfo)
            Dim knownSignature As Boolean = False
            Dim methodBeingOverriddenWithCovariantReturnType As MethodInfo = Nothing
            For Each existingMethod As MethodInfo In methods
                If method.Name.Equals(existingMethod.Name) AndAlso parameterTypesMatch(method, parameterTypesOf(existingMethod)) Then
                    If existingMethod.ReturnType IsNot method.ReturnType AndAlso existingMethod.ReturnType.IsAssignableFrom(method.ReturnType) Then
                        methodBeingOverriddenWithCovariantReturnType = existingMethod
                    Else
                        knownSignature = True
                    End If
                    Exit For
                End If
            Next existingMethod
            If methodBeingOverriddenWithCovariantReturnType IsNot Nothing Then
                methods.Remove(methodBeingOverriddenWithCovariantReturnType)
            End If
            If Not knownSignature AndAlso Not isCglibRenamedMethod(method) Then
                methods.Add(method)
            End If
        End Sub

        Public Shared Sub doWithFields(ByVal clazz As Type, ByVal fc As FieldCallback)
            doWithFields(clazz, fc, DirectCast(Nothing, FieldFilter))
        End Sub

        ''' <summary>
        ''' Calls the callback for every field declared on the type and its base types (excluding
        ''' <see cref="Object"/>) that passes the filter (Nothing accepts all), most derived type first.
        ''' </summary>
        Public Shared Sub doWithFields(ByVal clazz As Type, ByVal fc As FieldCallback, ByVal ff As FieldFilter)
            Dim targetClass As Type = clazz
            Do
                For Each field As FieldInfo In targetClass.GetFields(DECLARED)
                    If ff Is Nothing OrElse ff.matches(field) Then
                        Try
                            fc.doWith(field)
                        Catch ex As MemberAccessException
                            Throw New InvalidOperationException("Shouldn't be illegal to access field '" & field.Name & "': " & ex.ToString(), ex)
                        End Try
                    End If
                Next field
                targetClass = targetClass.BaseType
            Loop While targetClass IsNot Nothing AndAlso targetClass IsNot GetType(Object)
        End Sub

        Public Shared Sub doWithFields(ByVal clazz As Type, ByVal fc As Action(Of FieldInfo))
            doWithFields(clazz, New DelegateFieldCallback(fc), DirectCast(Nothing, FieldFilter))
        End Sub

        Public Shared Sub doWithFields(ByVal clazz As Type, ByVal fc As Action(Of FieldInfo), ByVal ff As Func(Of FieldInfo, Boolean))
            Dim filter As FieldFilter = Nothing
            If ff IsNot Nothing Then
                filter = New DelegateFieldFilter(ff)
            End If
            doWithFields(clazz, New DelegateFieldCallback(fc), filter)
        End Sub

        Public Shared Sub doWithFields(ByVal clazz As Type, ByVal fc As Action(Of FieldInfo), ByVal ff As FieldFilter)
            doWithFields(clazz, New DelegateFieldCallback(fc), ff)
        End Sub

        ''' <summary>Copies every <see cref="COPYABLE_FIELDS"/> field of <paramref name="src"/> to <paramref name="dest"/>, which must be of the same type or a subtype.</summary>
        Public Shared Sub shallowCopyFieldState(ByVal src As Object, ByVal dest As Object)
            If src Is Nothing Then
                Throw New ArgumentException("Source for field copy cannot be null")
            End If
            If dest Is Nothing Then
                Throw New ArgumentException("Destination for field copy cannot be null")
            End If
            If Not src.GetType().IsAssignableFrom(dest.GetType()) Then
                Throw New ArgumentException("Destination class [" & dest.GetType().FullName & "] must be same or subclass as source class [" & src.GetType().FullName & "]")
            End If
            doWithFields(src.GetType(), Sub(field As FieldInfo)
                                            makeAccessible(field)
                                            field.SetValue(dest, field.GetValue(src))
                                        End Sub, COPYABLE_FIELDS)
        End Sub

        ''' <summary>
        ''' Creates an instance of <paramref name="clazz"/> through the public constructor whose parameter
        ''' types are exactly the runtime types of <paramref name="args"/>, none of which may be Nothing.
        ''' A missing constructor or a failing constructor throws <see cref="InvalidOperationException"/>
        ''' (Java: <c>RuntimeException</c>), a non-public one <see cref="ArgumentException"/>.
        ''' </summary>
        Public Shared Function newInstance(Of T)(ByVal clazz As Type, ByVal ParamArray args() As Object) As T
            Objects.requireNonNull(clazz, "Class must not be null")
            Objects.requireNonNull(args, "Argument array must not be null")
            Dim parameterTypes(args.Length - 1) As Type
            For i As Integer = 0 To args.Length - 1
                If args(i) Is Nothing Then
                    Throw New ArgumentException("Individual arguments must not be null")
                End If
                parameterTypes(i) = args(i).GetType()
            Next i
            Dim constructor As ConstructorInfo = clazz.GetConstructor(BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic, Nothing, parameterTypes, Nothing)
            If constructor Is Nothing Then
                Throw New InvalidOperationException(String.Format("Class [{0}] has no constructor taking ({1}).", clazz.FullName, String.Join(", ", parameterTypes.Select(Function(p) p.FullName))))
            End If
            If Not constructor.IsPublic Then
                Throw New ArgumentException(String.Format("Class [{0}] must have public constructor in order to be instantiated.", clazz.FullName))
            End If
            Try
                Return CType(constructor.Invoke(args), T)
            Catch ex As TargetInvocationException
                Throw New InvalidOperationException("Could not instantiate [" & clazz.FullName & "]: " & ex.ToString(), ex)
            End Try
        End Function
    End Class

End Namespace
