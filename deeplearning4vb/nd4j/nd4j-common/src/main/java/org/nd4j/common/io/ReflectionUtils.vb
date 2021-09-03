Imports System
Imports System.Collections
Imports System.Reflection
Imports System.Linq

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

Namespace org.nd4j.common.io


	Public MustInherit Class ReflectionUtils
		Private Shared ReadOnly CGLIB_RENAMED_METHOD_PATTERN As Pattern = Pattern.compile("CGLIB\$(.+)\$\d+")
		Public Shared COPYABLE_FIELDS As ReflectionUtils.FieldFilter = New FieldFilterAnonymousInnerClass()

		Private Class FieldFilterAnonymousInnerClass
			Implements ReflectionUtils.FieldFilter

			Public Function matches(ByVal field As System.Reflection.FieldInfo) As Boolean Implements ReflectionUtils.FieldFilter.matches
				Return Not Modifier.isStatic(field.getModifiers()) AndAlso Not Modifier.isFinal(field.getModifiers())
			End Function
		End Class
		Public Shared NON_BRIDGED_METHODS As ReflectionUtils.MethodFilter = New MethodFilterAnonymousInnerClass()

		Private Class MethodFilterAnonymousInnerClass
			Implements ReflectionUtils.MethodFilter

			Public Function matches(ByVal method As System.Reflection.MethodInfo) As Boolean Implements ReflectionUtils.MethodFilter.matches
				Return Not method.isBridge()
			End Function
		End Class
		Public Shared USER_DECLARED_METHODS As ReflectionUtils.MethodFilter = New MethodFilterAnonymousInnerClass2()

		Private Class MethodFilterAnonymousInnerClass2
			Implements ReflectionUtils.MethodFilter

			Public Function matches(ByVal method As System.Reflection.MethodInfo) As Boolean Implements ReflectionUtils.MethodFilter.matches
				Return Not method.isBridge() AndAlso method.getDeclaringClass() <> GetType(Object)
			End Function
		End Class

		Public Sub New()
		End Sub

		Public Shared Function findField(ByVal clazz As Type, ByVal name As String) As System.Reflection.FieldInfo
			Return findField(clazz, name, Nothing)
		End Function

		Public Shared Function findField(ByVal clazz As Type, ByVal name As String, ByVal type As Type) As System.Reflection.FieldInfo
			Assert.notNull(clazz, "Class must not be null")
			Assert.isTrue(name IsNot Nothing OrElse type IsNot Nothing, "Either name or opType of the field must be specified")

			Dim searchType As Type = clazz
			Do While Not GetType(Object).Equals(searchType) AndAlso searchType IsNot Nothing
				Dim fields() As System.Reflection.FieldInfo = searchType.GetFields(BindingFlags.DeclaredOnly Or BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.Static Or BindingFlags.Instance)
				Dim arr$() As System.Reflection.FieldInfo = fields
				Dim len$ As Integer = fields.Length

				For i$ As Integer = 0 To len$ - 1
					Dim field As System.Reflection.FieldInfo = arr$(i$)
					If (name Is Nothing OrElse name.Equals(field.getName())) AndAlso (type Is Nothing OrElse type.Equals(field.getType())) Then
						Return field
					End If
				Next i$
				searchType = searchType.BaseType
			Loop

			Return Nothing
		End Function

		Public Shared Sub setField(ByVal field As System.Reflection.FieldInfo, ByVal target As Object, ByVal value As Object)
			Try
				field.set(target, value)
			Catch var4 As IllegalAccessException
				handleReflectionException(var4)
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Throw New System.InvalidOperationException("Unexpected reflection exception - " & var4.GetType().FullName & ": " & var4.Message)
			End Try
		End Sub

		Public Shared Function getField(ByVal field As System.Reflection.FieldInfo, ByVal target As Object) As Object
			Try
				Return field.get(target)
			Catch var3 As IllegalAccessException
				handleReflectionException(var3)
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Throw New System.InvalidOperationException("Unexpected reflection exception - " & var3.GetType().FullName & ": " & var3.Message)
			End Try
		End Function

		Public Shared Function findMethod(ByVal clazz As Type, ByVal name As String) As System.Reflection.MethodInfo
			Return findMethod(clazz, name, New Type(){})
		End Function

		Public Shared Function findMethod(ByVal clazz As Type, ByVal name As String, ParamArray ByVal paramTypes() As Type) As System.Reflection.MethodInfo
			Assert.notNull(clazz, "Class must not be null")
			Assert.notNull(name, "Method name must not be null")

			Dim searchType As Type = clazz
			Do While searchType IsNot Nothing
				Dim methods() As System.Reflection.MethodInfo = If(searchType.IsInterface, searchType.GetMethods(), searchType.GetMethods(BindingFlags.DeclaredOnly Or BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.Static Or BindingFlags.Instance))
				Dim arr$() As System.Reflection.MethodInfo = methods
				Dim len$ As Integer = methods.Length

				For i$ As Integer = 0 To len$ - 1
					Dim method As System.Reflection.MethodInfo = arr$(i$)
					If name.Equals(method.getName()) AndAlso (paramTypes Is Nothing OrElse paramTypes.SequenceEqual(method.getParameterTypes())) Then
						Return method
					End If
				Next i$
				searchType = searchType.BaseType
			Loop

			Return Nothing
		End Function

		Public Shared Function invokeMethod(ByVal method As System.Reflection.MethodInfo, ByVal target As Object) As Object
			Return invokeMethod(method, target, New Object(){})
		End Function

		Public Shared Function invokeMethod(ByVal method As System.Reflection.MethodInfo, ByVal target As Object, ParamArray ByVal args() As Object) As Object
			Try
				Return method.invoke(target, args)
			Catch var4 As Exception
				handleReflectionException(var4)
				Throw New System.InvalidOperationException("Should never get here")
			End Try
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static Object invokeJdbcMethod(Method method, Object target) throws java.sql.SQLException
		Public Shared Function invokeJdbcMethod(ByVal method As System.Reflection.MethodInfo, ByVal target As Object) As Object
			Return invokeJdbcMethod(method, target, New Object(){})
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static Object invokeJdbcMethod(Method method, Object target, Object... args) throws java.sql.SQLException
		Public Shared Function invokeJdbcMethod(ByVal method As System.Reflection.MethodInfo, ByVal target As Object, ParamArray ByVal args() As Object) As Object
			Try
				Return method.invoke(target, args)
			Catch var4 As IllegalAccessException
				handleReflectionException(var4)
			Catch var5 As InvocationTargetException
				If TypeOf var5.getTargetException() Is SQLException Then
					Throw CType(var5.getTargetException(), SQLException)
				End If

				handleInvocationTargetException(var5)
			End Try

			Throw New System.InvalidOperationException("Should never get here")
		End Function

		Public Shared Sub handleReflectionException(ByVal ex As Exception)
			If TypeOf ex Is NoSuchMethodException Then
				Throw New System.InvalidOperationException("Method not found: " & ex.Message)
			ElseIf TypeOf ex Is IllegalAccessException Then
				Throw New System.InvalidOperationException("Could not access method: " & ex.Message)
			Else
				If TypeOf ex Is InvocationTargetException Then
					handleInvocationTargetException(CType(ex, InvocationTargetException))
				End If

				If TypeOf ex Is Exception Then
					Throw CType(ex, Exception)
				Else
					Throw New UndeclaredThrowableException(ex)
				End If
			End If
		End Sub

		Public Shared Sub handleInvocationTargetException(ByVal ex As InvocationTargetException)
			rethrowRuntimeException(ex.getTargetException())
		End Sub

		Public Shared Sub rethrowRuntimeException(ByVal ex As Exception)
			If TypeOf ex Is Exception Then
				Throw CType(ex, Exception)
			ElseIf TypeOf ex Is Exception Then
				Throw CType(ex, Exception)
			Else
				Throw New UndeclaredThrowableException(ex)
			End If
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void rethrowException(Throwable ex) throws Exception
		Public Shared Sub rethrowException(ByVal ex As Exception)
			If TypeOf ex Is Exception Then
				Throw CType(ex, Exception)
			ElseIf TypeOf ex Is Exception Then
				Throw CType(ex, Exception)
			Else
				Throw New UndeclaredThrowableException(ex)
			End If
		End Sub

		Public Shared Function declaresException(ByVal method As System.Reflection.MethodInfo, ByVal exceptionType As Type) As Boolean
			Assert.notNull(method, "Method must not be null")
			Dim declaredExceptions() As Type = method.getExceptionTypes()
			Dim arr$() As Type = declaredExceptions
			Dim len$ As Integer = declaredExceptions.Length

			For i$ As Integer = 0 To len$ - 1
				Dim declaredException As Type = arr$(i$)
				If declaredException.IsAssignableFrom(exceptionType) Then
					Return True
				End If
			Next i$

			Return False
		End Function

		Public Shared Function isPublicStaticFinal(ByVal field As System.Reflection.FieldInfo) As Boolean
			Dim modifiers As Integer = field.getModifiers()
			Return Modifier.isPublic(modifiers) AndAlso Modifier.isStatic(modifiers) AndAlso Modifier.isFinal(modifiers)
		End Function

		Public Shared Function isEqualsMethod(ByVal method As System.Reflection.MethodInfo) As Boolean
			If method IsNot Nothing AndAlso method.getName().Equals("equals") Then
				Dim paramTypes() As Type = method.getParameterTypes()
				Return paramTypes.Length = 1 AndAlso paramTypes(0) = GetType(Object)
			Else
				Return False
			End If
		End Function

		Public Shared Function isHashCodeMethod(ByVal method As System.Reflection.MethodInfo) As Boolean
			Return method IsNot Nothing AndAlso method.getName().Equals("hashCode") AndAlso method.getParameterTypes().length = 0
		End Function

		Public Shared Function isToStringMethod(ByVal method As System.Reflection.MethodInfo) As Boolean
			Return method IsNot Nothing AndAlso method.getName().Equals("toString") AndAlso method.getParameterTypes().length = 0
		End Function

		Public Shared Function isObjectMethod(ByVal method As System.Reflection.MethodInfo) As Boolean
			Try
				GetType(Object).getDeclaredMethod(method.getName(), method.getParameterTypes())
				Return True
			Catch var2 As SecurityException
				Return False
			Catch var3 As NoSuchMethodException
				Return False
			End Try
		End Function

		Public Shared Function isCglibRenamedMethod(ByVal renamedMethod As System.Reflection.MethodInfo) As Boolean
			Return CGLIB_RENAMED_METHOD_PATTERN.matcher(renamedMethod.getName()).matches()
		End Function

		Public Shared Sub makeAccessible(ByVal field As System.Reflection.FieldInfo)
			If (Not Modifier.isPublic(field.getModifiers()) OrElse Not Modifier.isPublic(field.getDeclaringClass().getModifiers()) OrElse Modifier.isFinal(field.getModifiers())) AndAlso Not field.isAccessible() Then
				field.setAccessible(True)
			End If

		End Sub

		Public Shared Sub makeAccessible(ByVal method As System.Reflection.MethodInfo)
			If (Not Modifier.isPublic(method.getModifiers()) OrElse Not Modifier.isPublic(method.getDeclaringClass().getModifiers())) AndAlso Not method.isAccessible() Then
				method.setAccessible(True)
			End If

		End Sub

		Public Shared Sub makeAccessible(Of T1)(ByVal ctor As System.Reflection.ConstructorInfo(Of T1))
			If (Not Modifier.isPublic(ctor.getModifiers()) OrElse Not Modifier.isPublic(ctor.getDeclaringClass().getModifiers())) AndAlso Not ctor.isAccessible() Then
				ctor.setAccessible(True)
			End If

		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void doWithMethods(@Class clazz, ReflectionUtils.MethodCallback mc) throws IllegalArgumentException
		Public Shared Sub doWithMethods(ByVal clazz As Type, ByVal mc As ReflectionUtils.MethodCallback)
			doWithMethods(clazz, mc, Nothing)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void doWithMethods(@Class clazz, ReflectionUtils.MethodCallback mc, ReflectionUtils.MethodFilter mf) throws IllegalArgumentException
		Public Shared Sub doWithMethods(ByVal clazz As Type, ByVal mc As ReflectionUtils.MethodCallback, ByVal mf As ReflectionUtils.MethodFilter)
			Dim methods() As System.Reflection.MethodInfo = clazz.GetMethods(BindingFlags.DeclaredOnly Or BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.Static Or BindingFlags.Instance)
			Dim arr$() As System.Reflection.MethodInfo = methods
			Dim len$ As Integer = methods.Length

			Dim i$ As Integer
			For i$ = 0 To len$ - 1
				Dim superIfc As System.Reflection.MethodInfo = arr$(i$)
				If mf Is Nothing OrElse mf.matches(superIfc) Then
					Try
						mc.doWith(superIfc)
					Catch var9 As IllegalAccessException
						Throw New System.InvalidOperationException("Shouldn't be illegal to access method '" & superIfc.getName() & "': " & var9)
					End Try
				End If
			Next i$

			If clazz.BaseType IsNot Nothing Then
				doWithMethods(clazz.BaseType, mc, mf)
			ElseIf clazz.IsInterface Then
				Dim var10() As Type = clazz.GetInterfaces()
				len$ = var10.Length

				For i$ = 0 To len$ - 1
					Dim var11 As Type = var10(i$)
					doWithMethods(var11, mc, mf)
				Next i$
			End If

		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static Method[] getAllDeclaredMethods(@Class leafClass) throws IllegalArgumentException
		Public Shared Function getAllDeclaredMethods(ByVal leafClass As Type) As System.Reflection.MethodInfo()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.ArrayList methods = new java.util.ArrayList(32);
			Dim methods As New ArrayList(32)
			doWithMethods(leafClass, New MethodCallbackAnonymousInnerClass(methods))
			Return CType(methods.ToArray(GetType(System.Reflection.MethodInfo)), System.Reflection.MethodInfo())
		End Function

		Private Class MethodCallbackAnonymousInnerClass
			Implements ReflectionUtils.MethodCallback

			Private methods As ArrayList

			Public Sub New(ByVal methods As ArrayList)
				Me.methods = methods
			End Sub

			Public Sub doWith(ByVal method As System.Reflection.MethodInfo) Implements ReflectionUtils.MethodCallback.doWith
				methods.Add(method)
			End Sub
		End Class

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static Method[] getUniqueDeclaredMethods(@Class leafClass) throws IllegalArgumentException
		Public Shared Function getUniqueDeclaredMethods(ByVal leafClass As Type) As System.Reflection.MethodInfo()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.ArrayList methods = new java.util.ArrayList(32);
			Dim methods As New ArrayList(32)
			doWithMethods(leafClass, New MethodCallbackAnonymousInnerClass2(methods))
			Return CType(methods.ToArray(GetType(System.Reflection.MethodInfo)), System.Reflection.MethodInfo())
		End Function

		Private Class MethodCallbackAnonymousInnerClass2
			Implements ReflectionUtils.MethodCallback

			Private methods As ArrayList

			Public Sub New(ByVal methods As ArrayList)
				Me.methods = methods
			End Sub

			Public Sub doWith(ByVal method As System.Reflection.MethodInfo) Implements ReflectionUtils.MethodCallback.doWith
				Dim knownSignature As Boolean = False
				Dim methodBeingOverriddenWithCovariantReturnType As System.Reflection.MethodInfo = Nothing
				Dim i$ As System.Collections.IEnumerator = methods.GetEnumerator()

				Do While i$.MoveNext()
					Dim existingMethod As System.Reflection.MethodInfo = CType(i$.Current, System.Reflection.MethodInfo)
					If method.getName().Equals(existingMethod.getName()) AndAlso method.getParameterTypes().SequenceEqual(existingMethod.getParameterTypes()) Then
						If existingMethod.getReturnType() <> method.getReturnType() AndAlso existingMethod.getReturnType().isAssignableFrom(method.getReturnType()) Then
							methodBeingOverriddenWithCovariantReturnType = existingMethod
							Exit Do
						End If

						knownSignature = True
						Exit Do
					End If
				Loop

				If methodBeingOverriddenWithCovariantReturnType IsNot Nothing Then
					methods.Remove(methodBeingOverriddenWithCovariantReturnType)
				End If

				If Not knownSignature AndAlso Not ReflectionUtils.isCglibRenamedMethod(method) Then
					methods.Add(method)
				End If

			End Sub
		End Class

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void doWithFields(@Class clazz, ReflectionUtils.FieldCallback fc) throws IllegalArgumentException
		Public Shared Sub doWithFields(ByVal clazz As Type, ByVal fc As ReflectionUtils.FieldCallback)
			doWithFields(clazz, fc, Nothing)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void doWithFields(@Class clazz, ReflectionUtils.FieldCallback fc, ReflectionUtils.FieldFilter ff) throws IllegalArgumentException
		Public Shared Sub doWithFields(ByVal clazz As Type, ByVal fc As ReflectionUtils.FieldCallback, ByVal ff As ReflectionUtils.FieldFilter)
			Dim targetClass As Type = clazz

			Do
				Dim fields() As System.Reflection.FieldInfo = targetClass.GetFields(BindingFlags.DeclaredOnly Or BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.Static Or BindingFlags.Instance)
				Dim arr$() As System.Reflection.FieldInfo = fields
				Dim len$ As Integer = fields.Length

				For i$ As Integer = 0 To len$ - 1
					Dim field As System.Reflection.FieldInfo = arr$(i$)
					If ff Is Nothing OrElse ff.matches(field) Then
						Try
							fc.doWith(field)
						Catch var10 As IllegalAccessException
							Throw New System.InvalidOperationException("Shouldn't be illegal to access field '" & field.getName() & "': " & var10)
						End Try
					End If
				Next i$

				targetClass = targetClass.BaseType
			Loop While targetClass IsNot Nothing AndAlso targetClass <> GetType(Object)

		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void shallowCopyFieldState(final Object src, final Object dest) throws IllegalArgumentException
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
		Public Shared Sub shallowCopyFieldState(ByVal src As Object, ByVal dest As Object)
			If src Is Nothing Then
				Throw New System.ArgumentException("Source for field copy cannot be null")
			ElseIf dest Is Nothing Then
				Throw New System.ArgumentException("Destination for field copy cannot be null")
			ElseIf Not src.GetType().IsAssignableFrom(dest.GetType()) Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Throw New System.ArgumentException("Destination class [" & dest.GetType().FullName & "] must be same or subclass as source class [" & src.GetType().FullName & "]")
			Else
				doWithFields(src.GetType(), New FieldCallbackAnonymousInnerClass(src, dest)
			   , COPYABLE_FIELDS)
			End If
		End Sub

		Private Class FieldCallbackAnonymousInnerClass
			Implements ReflectionUtils.FieldCallback

			Private src As Object
			Private dest As Object

			Public Sub New(ByVal src As Object, ByVal dest As Object)
				Me.src = src
				Me.dest = dest
			End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void doWith(Field field) throws IllegalArgumentException, IllegalAccessException
			Public Sub doWith(ByVal field As System.Reflection.FieldInfo) Implements ReflectionUtils.FieldCallback.doWith
				ReflectionUtils.makeAccessible(field)
				Dim srcValue As Object = field.get(src)
				field.set(dest, srcValue)
			End Sub
		End Class

		''' <summary>
		''' Create a new instance of the specified <seealso cref="Class"/> by invoking
		''' the constructor whose argument list matches the types of the supplied
		''' arguments.
		''' 
		''' <para>Provided class must have a public constructor.</para>
		''' </summary>
		''' <param name="clazz"> the class to instantiate; never {@code null} </param>
		''' <param name="args"> the arguments to pass to the constructor, none of which may
		'''             be {@code null} </param>
		''' <returns> the new instance; never {@code null} </returns>
		Public Shared Function newInstance(Of T)(ByVal clazz As Type(Of T), ParamArray ByVal args() As Object) As T
			Objects.requireNonNull(clazz, "Class must not be null")
			Objects.requireNonNull(args, "Argument array must not be null")
			If Arrays.asList(args).contains(Nothing) Then
				Throw New Exception("Individual arguments must not be null")
			End If

			Try
'JAVA TO VB CONVERTER TODO TASK: Method reference constructor syntax is not converted by Java to VB Converter:
				Dim parameterTypes() As Type = args.Select(AddressOf Object.getClass).ToArray(Type()::New)
				Dim constructor As System.Reflection.ConstructorInfo(Of T) = clazz.getDeclaredConstructor(parameterTypes)

				If Not Modifier.isPublic(constructor.getModifiers()) Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					Throw New System.ArgumentException(String.Format("Class [{0}] must have public constructor in order to be instantiated.", clazz.FullName))
				End If

				Return constructor.Invoke(args)
			Catch instantiationException As Exception
				Throw New Exception(instantiationException)
			End Try
		End Function

		Public Interface FieldFilter
			Function matches(ByVal var1 As System.Reflection.FieldInfo) As Boolean
		End Interface

		Public Interface FieldCallback
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: void doWith(Field var1) throws IllegalArgumentException, IllegalAccessException;
			Sub doWith(ByVal var1 As System.Reflection.FieldInfo)
		End Interface

		Public Interface MethodFilter
			Function matches(ByVal var1 As System.Reflection.MethodInfo) As Boolean
		End Interface

		Public Interface MethodCallback
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: void doWith(Method var1) throws IllegalArgumentException, IllegalAccessException;
			Sub doWith(ByVal var1 As System.Reflection.MethodInfo)
		End Interface
	End Class

End Namespace