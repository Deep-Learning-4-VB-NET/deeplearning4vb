Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports ND4JClassLoading = org.nd4j.common.config.ND4JClassLoading

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

Namespace org.deeplearning4j.common.config


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class DL4JClassLoading
	Public Class DL4JClassLoading
'JAVA TO VB CONVERTER NOTE: The field dl4jClassloader was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared dl4jClassloader_Conflict As ClassLoader = ND4JClassLoading.Nd4jClassloader

		Private Sub New()
		End Sub

		Public Shared Property Dl4jClassloader As ClassLoader
			Get
				Return DL4JClassLoading.dl4jClassloader_Conflict
			End Get
			Set(ByVal dl4jClassloader As ClassLoader)
				DL4JClassLoading.dl4jClassloader_Conflict = dl4jClassloader
				log.debug("Global class-loader for DL4J was changed.")
			End Set
		End Property

		Public Shared WriteOnly Property Dl4jClassloaderFromClass As Type
			Set(ByVal clazz As Type)
				Dl4jClassloader = clazz.getClassLoader()
			End Set
		End Property


		Public Shared Function classPresentOnClasspath(ByVal className As String) As Boolean
			Return classPresentOnClasspath(className, dl4jClassloader_Conflict)
		End Function

		Public Shared Function classPresentOnClasspath(ByVal className As String, ByVal classLoader As ClassLoader) As Boolean
			Return loadClassByName(className, False, classLoader) IsNot Nothing
		End Function

		Public Shared Function loadClassByName(Of T)(ByVal className As String) As Type(Of T)
			Return loadClassByName(className, True, dl4jClassloader_Conflict)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") public static <T> @Class<T> loadClassByName(String className, boolean initialize, ClassLoader classLoader)
		Public Shared Function loadClassByName(Of T)(ByVal className As String, ByVal initialize As Boolean, ByVal classLoader As ClassLoader) As Type(Of T)
			Try
				Return CType(Type.GetType(className, initialize, classLoader), Type(Of T))
			Catch classNotFoundException As ClassNotFoundException
				log.error(String.Format("Cannot find class [{0}] of provided class-loader.", className))
				Return Nothing
			End Try
		End Function

		Public Shared Function createNewInstance(Of T)(ByVal className As String) As T
			Return createNewInstance(className, GetType(Object), New Object(){}) 'or null;
		End Function

		Public Shared Function createNewInstance(Of T)(ByVal className As String, ByVal args() As Object) As T

			Return createNewInstance(className, GetType(Object), args)
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to the Java 'super' constraint:
'ORIGINAL LINE: public static <T> T createNewInstance(String className, @Class<? super T> superclass)
		Public Shared Function createNewInstance(Of T, T1)(ByVal className As String, ByVal superclass As Type(Of T1)) As T
			Return createNewInstance(className, superclass, New Type(){}, New Object(){})
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to the Java 'super' constraint:
'ORIGINAL LINE: public static <T> T createNewInstance(String className, @Class<? super T> superclass, Object[] args)
		Public Shared Function createNewInstance(Of T, T1)(ByVal className As String, ByVal superclass As Type(Of T1), ByVal args() As Object) As T
			Dim parameterTypes(args.Length - 1) As Type
			For i As Integer = 0 To args.Length - 1
				Dim arg As Object = args(i)
				Objects.requireNonNull(arg)
				parameterTypes(i) = arg.GetType()
			Next i

			Return createNewInstance(className, superclass, parameterTypes, args)
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to the Java 'super' constraint:
'ORIGINAL LINE: public static <T> T createNewInstance(String className, @Class<? super T> superclass, @Class[] parameterTypes, Object [] args)
		Public Shared Function createNewInstance(Of T, T1)(ByVal className As String, ByVal superclass As Type(Of T1), ByVal parameterTypes() As Type, ByVal args() As Object) As T
			Try
				Dim loadedClass As Type(Of Object) = DL4JClassLoading.loadClassByName(className)
				Preconditions.checkNotNull(loadedClass,"Attempted to load class " & className & " but failed. No class found with this name.")
				Return CType(loadedClass.asSubclass(superclass).getDeclaredConstructor(parameterTypes).newInstance(args), T)
			Catch instantiationException As Exception When TypeOf instantiationException Is InstantiationException OrElse TypeOf instantiationException Is IllegalAccessException OrElse TypeOf instantiationException Is InvocationTargetException OrElse TypeOf instantiationException Is NoSuchMethodException
				log.error(String.Format("Cannot create instance of class '{0}'.", className), instantiationException)
				Throw New Exception(instantiationException)
			End Try
		End Function

		Public Shared Function loadService(Of S)(ByVal serviceClass As Type(Of S)) As ServiceLoader(Of S)
			Return loadService(serviceClass, dl4jClassloader_Conflict)
		End Function

		Public Shared Function loadService(Of S)(ByVal serviceClass As Type(Of S), ByVal classLoader As ClassLoader) As ServiceLoader(Of S)
			Return ServiceLoader.load(serviceClass, classLoader)
		End Function
	End Class

End Namespace