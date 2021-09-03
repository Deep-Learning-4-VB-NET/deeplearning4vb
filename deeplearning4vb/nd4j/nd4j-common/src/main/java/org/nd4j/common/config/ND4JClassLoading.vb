Imports System
Imports System.Threading
Imports Slf4j = lombok.extern.slf4j.Slf4j

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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public final class ND4JClassLoading
	Public NotInheritable Class ND4JClassLoading
'JAVA TO VB CONVERTER NOTE: The field nd4jClassloader was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared nd4jClassloader_Conflict As ClassLoader = Thread.CurrentThread.getContextClassLoader()

		Private Sub New()
		End Sub

		Public Shared Property Nd4jClassloader As ClassLoader
			Get
				Return ND4JClassLoading.nd4jClassloader_Conflict
			End Get
			Set(ByVal nd4jClassloader As ClassLoader)
				ND4JClassLoading.nd4jClassloader_Conflict = nd4jClassloader
				log.debug("Global class-loader for ND4J was changed.")
			End Set
		End Property

		Public Shared WriteOnly Property Nd4jClassloaderFromClass As Type
			Set(ByVal clazz As Type)
				Nd4jClassloader = clazz.getClassLoader()
			End Set
		End Property


		Public Shared Function classPresentOnClasspath(ByVal className As String) As Boolean
			Return classPresentOnClasspath(className, nd4jClassloader_Conflict)
		End Function

		Public Shared Function classPresentOnClasspath(ByVal className As String, ByVal classLoader As ClassLoader) As Boolean
			Return loadClassByName(className, False, classLoader) IsNot Nothing
		End Function

		Public Shared Function loadClassByName(Of T)(ByVal className As String) As Type(Of T)
			Return loadClassByName(className, True, nd4jClassloader_Conflict)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") public static <T> @Class<T> loadClassByName(String className, boolean initialize, ClassLoader classLoader)
		Public Shared Function loadClassByName(Of T)(ByVal className As String, ByVal initialize As Boolean, ByVal classLoader As ClassLoader) As Type(Of T)
			Try
				Return CType(Type.GetType(className, initialize, classLoader), Type(Of T))
			Catch classNotFoundException As ClassNotFoundException
				log.trace(String.Format("Cannot find class [{0}] of provided class-loader.", className))
				Return Nothing
			End Try
		End Function

		Public Shared Function loadService(Of S)(ByVal serviceClass As Type(Of S)) As ServiceLoader(Of S)
			Return loadService(serviceClass, nd4jClassloader_Conflict)
		End Function

		Public Shared Function loadService(Of S)(ByVal serviceClass As Type(Of S), ByVal classLoader As ClassLoader) As ServiceLoader(Of S)
			Return ServiceLoader.load(serviceClass, classLoader)
		End Function
	End Class

End Namespace