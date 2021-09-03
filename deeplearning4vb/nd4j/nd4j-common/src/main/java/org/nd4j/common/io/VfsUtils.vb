Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory

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




	Public MustInherit Class VfsUtils
		Private Shared ReadOnly logger As Logger = LoggerFactory.getLogger(GetType(VfsUtils))
		Private Const VFS2_PKG As String = "org.jboss.virtual."
		Private Const VFS3_PKG As String = "org.jboss.vfs."
		Private Const VFS_NAME As String = "VFS"
		Private Shared version As VfsUtils.VFS_VER
		Private Shared VFS_METHOD_GET_ROOT_URL As System.Reflection.MethodInfo = Nothing
		Private Shared VFS_METHOD_GET_ROOT_URI As System.Reflection.MethodInfo = Nothing
		Private Shared VIRTUAL_FILE_METHOD_EXISTS As System.Reflection.MethodInfo = Nothing
		Private Shared VIRTUAL_FILE_METHOD_GET_INPUT_STREAM As System.Reflection.MethodInfo
		Private Shared VIRTUAL_FILE_METHOD_GET_SIZE As System.Reflection.MethodInfo
		Private Shared VIRTUAL_FILE_METHOD_GET_LAST_MODIFIED As System.Reflection.MethodInfo
		Private Shared VIRTUAL_FILE_METHOD_TO_URL As System.Reflection.MethodInfo
		Private Shared VIRTUAL_FILE_METHOD_TO_URI As System.Reflection.MethodInfo
		Private Shared VIRTUAL_FILE_METHOD_GET_NAME As System.Reflection.MethodInfo
		Private Shared VIRTUAL_FILE_METHOD_GET_PATH_NAME As System.Reflection.MethodInfo
		Private Shared VIRTUAL_FILE_METHOD_GET_CHILD As System.Reflection.MethodInfo
		Protected Friend Shared VIRTUAL_FILE_VISITOR_INTERFACE As Type
		Protected Friend Shared VIRTUAL_FILE_METHOD_VISIT As System.Reflection.MethodInfo
		Private Shared VFS_UTILS_METHOD_IS_NESTED_FILE As System.Reflection.MethodInfo = Nothing
		Private Shared VFS_UTILS_METHOD_GET_COMPATIBLE_URI As System.Reflection.MethodInfo = Nothing
		Private Shared VISITOR_ATTRIBUTES_FIELD_RECURSE As System.Reflection.FieldInfo = Nothing
		Private Shared GET_PHYSICAL_FILE As System.Reflection.MethodInfo = Nothing

		Public Sub New()
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected static Object invokeVfsMethod(java.lang.reflect.Method method, Object target, Object... args) throws java.io.IOException
		Protected Friend Shared Function invokeVfsMethod(ByVal method As System.Reflection.MethodInfo, ByVal target As Object, ParamArray ByVal args() As Object) As Object
			Try
				Return method.invoke(target, args)
			Catch var5 As InvocationTargetException
				Dim targetEx As Exception = var5.getTargetException()
				If TypeOf targetEx Is IOException Then
					Throw CType(targetEx, IOException)
				End If

				ReflectionUtils.handleInvocationTargetException(var5)
			Catch var6 As Exception
				ReflectionUtils.handleReflectionException(var6)
			End Try

			Throw New System.InvalidOperationException("Invalid code path reached")
		End Function

		Friend Shared Function exists(ByVal vfsResource As Object) As Boolean
			Try
				Return DirectCast(invokeVfsMethod(VIRTUAL_FILE_METHOD_EXISTS, vfsResource, New Object(){}), Boolean?).Value
			Catch var2 As IOException
				Return False
			End Try
		End Function

		Friend Shared Function isReadable(ByVal vfsResource As Object) As Boolean
			Try
				Return DirectCast(invokeVfsMethod(VIRTUAL_FILE_METHOD_GET_SIZE, vfsResource, New Object(){}), Long?).Value > 0L
			Catch var2 As IOException
				Return False
			End Try
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: static long getSize(Object vfsResource) throws java.io.IOException
		Friend Shared Function getSize(ByVal vfsResource As Object) As Long
			Return DirectCast(invokeVfsMethod(VIRTUAL_FILE_METHOD_GET_SIZE, vfsResource, New Object(){}), Long?).Value
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: static long getLastModified(Object vfsResource) throws java.io.IOException
		Friend Shared Function getLastModified(ByVal vfsResource As Object) As Long
			Return DirectCast(invokeVfsMethod(VIRTUAL_FILE_METHOD_GET_LAST_MODIFIED, vfsResource, New Object(){}), Long?).Value
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: static java.io.InputStream getInputStream(Object vfsResource) throws java.io.IOException
		Friend Shared Function getInputStream(ByVal vfsResource As Object) As Stream
			Return DirectCast(invokeVfsMethod(VIRTUAL_FILE_METHOD_GET_INPUT_STREAM, vfsResource, New Object(){}), Stream)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: static java.net.URL getURL(Object vfsResource) throws java.io.IOException
		Friend Shared Function getURL(ByVal vfsResource As Object) As URL
			Return DirectCast(invokeVfsMethod(VIRTUAL_FILE_METHOD_TO_URL, vfsResource, New Object(){}), URL)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: static java.net.URI getURI(Object vfsResource) throws java.io.IOException
		Friend Shared Function getURI(ByVal vfsResource As Object) As URI
			Return DirectCast(invokeVfsMethod(VIRTUAL_FILE_METHOD_TO_URI, vfsResource, New Object(){}), URI)
		End Function

		Friend Shared Function getName(ByVal vfsResource As Object) As String
			Try
				Return DirectCast(invokeVfsMethod(VIRTUAL_FILE_METHOD_GET_NAME, vfsResource, New Object(){}), String)
			Catch var2 As IOException
				Throw New System.InvalidOperationException("Cannot get resource name", var2)
			End Try
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: static Object getRelative(java.net.URL url) throws java.io.IOException
		Friend Shared Function getRelative(ByVal url As URL) As Object
			Return invokeVfsMethod(VFS_METHOD_GET_ROOT_URL, Nothing, New Object() {url})
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: static Object getChild(Object vfsResource, String path) throws java.io.IOException
		Friend Shared Function getChild(ByVal vfsResource As Object, ByVal path As String) As Object
			Return invokeVfsMethod(VIRTUAL_FILE_METHOD_GET_CHILD, vfsResource, New Object() {path})
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: static java.io.File getFile(Object vfsResource) throws java.io.IOException
		Friend Shared Function getFile(ByVal vfsResource As Object) As File
			If VfsUtils.VFS_VER.V2.Equals(version) Then
				If DirectCast(invokeVfsMethod(VFS_UTILS_METHOD_IS_NESTED_FILE, Nothing, New Object() {vfsResource}), Boolean?).Value Then
					Throw New IOException("File resolution not supported for nested resource: " & vfsResource)
				Else
					Try
						Return New File(DirectCast(invokeVfsMethod(VFS_UTILS_METHOD_GET_COMPATIBLE_URI, Nothing, New Object() {vfsResource}), URI))
					Catch var2 As Exception
						Throw New IOException("Failed to obtain File reference for " & vfsResource, var2)
					End Try
				End If
			Else
				Return DirectCast(invokeVfsMethod(GET_PHYSICAL_FILE, vfsResource, New Object(){}), File)
			End If
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: static Object getRoot(java.net.URI url) throws java.io.IOException
		Friend Shared Function getRoot(ByVal url As URI) As Object
			Return invokeVfsMethod(VFS_METHOD_GET_ROOT_URI, Nothing, New Object() {url})
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected static Object getRoot(java.net.URL url) throws java.io.IOException
		Protected Friend Shared Function getRoot(ByVal url As URL) As Object
			Return invokeVfsMethod(VFS_METHOD_GET_ROOT_URL, Nothing, New Object() {url})
		End Function

		Protected Friend Shared Function doGetVisitorAttribute() As Object
			Return ReflectionUtils.getField(VISITOR_ATTRIBUTES_FIELD_RECURSE, Nothing)
		End Function

		Protected Friend Shared Function doGetPath(ByVal resource As Object) As String
			Return DirectCast(ReflectionUtils.invokeMethod(VIRTUAL_FILE_METHOD_GET_PATH_NAME, resource), String)
		End Function

		Shared Sub New()
			Dim loader As ClassLoader = GetType(VfsUtils).getClassLoader()

			Dim pkg As String
			Dim vfsClass As Type
			Try
				vfsClass = loader.loadClass("org.jboss.vfs.VFS")
				version = VfsUtils.VFS_VER.V3
				pkg = "org.jboss.vfs."
				If logger.isDebugEnabled() Then
					logger.debug("JBoss VFS packages for JBoss AS 6 found")
				End If
			Catch var9 As ClassNotFoundException
				If logger.isDebugEnabled() Then
					logger.debug("JBoss VFS packages for JBoss AS 6 not found; falling back to JBoss AS 5 packages")
				End If

				Try
					vfsClass = loader.loadClass("org.jboss.virtual.VFS")
					version = VfsUtils.VFS_VER.V2
					pkg = "org.jboss.virtual."
					If logger.isDebugEnabled() Then
						logger.debug("JBoss VFS packages for JBoss AS 5 found")
					End If
				Catch var8 As ClassNotFoundException
					logger.error("JBoss VFS packages (for both JBoss AS 5 and 6) were not found - JBoss VFS support disabled")
					Throw New System.InvalidOperationException("Cannot detect JBoss VFS packages", var8)
				End Try
			End Try

			Try
				Dim ex As String = If(VfsUtils.VFS_VER.V3.Equals(version), "getChild", "getRoot")
				VFS_METHOD_GET_ROOT_URL = ReflectionUtils.findMethod(vfsClass, ex, New Type() {GetType(URL)})
				VFS_METHOD_GET_ROOT_URI = ReflectionUtils.findMethod(vfsClass, ex, New Type() {GetType(URI)})
				Dim virtualFile As Type = loader.loadClass(pkg & "VirtualFile")
				VIRTUAL_FILE_METHOD_EXISTS = ReflectionUtils.findMethod(virtualFile, "exists")
				VIRTUAL_FILE_METHOD_GET_INPUT_STREAM = ReflectionUtils.findMethod(virtualFile, "openStream")
				VIRTUAL_FILE_METHOD_GET_SIZE = ReflectionUtils.findMethod(virtualFile, "getSize")
				VIRTUAL_FILE_METHOD_GET_LAST_MODIFIED = ReflectionUtils.findMethod(virtualFile, "getLastModified")
				VIRTUAL_FILE_METHOD_TO_URI = ReflectionUtils.findMethod(virtualFile, "toURI")
				VIRTUAL_FILE_METHOD_TO_URL = ReflectionUtils.findMethod(virtualFile, "toURL")
				VIRTUAL_FILE_METHOD_GET_NAME = ReflectionUtils.findMethod(virtualFile, "getName")
				VIRTUAL_FILE_METHOD_GET_PATH_NAME = ReflectionUtils.findMethod(virtualFile, "getPathName")
				GET_PHYSICAL_FILE = ReflectionUtils.findMethod(virtualFile, "getPhysicalFile")
				ex = If(VfsUtils.VFS_VER.V3.Equals(version), "getChild", "findChild")
				VIRTUAL_FILE_METHOD_GET_CHILD = ReflectionUtils.findMethod(virtualFile, ex, New Type() {GetType(String)})
				Dim utilsClass As Type = loader.loadClass(pkg & "VFSUtils")
				VFS_UTILS_METHOD_GET_COMPATIBLE_URI = ReflectionUtils.findMethod(utilsClass, "getCompatibleURI", New Type() {virtualFile})
				VFS_UTILS_METHOD_IS_NESTED_FILE = ReflectionUtils.findMethod(utilsClass, "isNestedFile", New Type() {virtualFile})
				VIRTUAL_FILE_VISITOR_INTERFACE = loader.loadClass(pkg & "VirtualFileVisitor")
				VIRTUAL_FILE_METHOD_VISIT = ReflectionUtils.findMethod(virtualFile, "visit", New Type() {VIRTUAL_FILE_VISITOR_INTERFACE})
				Dim visitorAttributesClass As Type = loader.loadClass(pkg & "VisitorAttributes")
				VISITOR_ATTRIBUTES_FIELD_RECURSE = ReflectionUtils.findField(visitorAttributesClass, "RECURSE")
			Catch var7 As ClassNotFoundException
				Throw New System.InvalidOperationException("Could not detect the JBoss VFS infrastructure", var7)
			End Try
		End Sub

		Private NotInheritable Class VFS_VER
			Public Shared ReadOnly V2 As New VFS_VER("V2", InnerEnum.V2)
			Public Shared ReadOnly V3 As New VFS_VER("V3", InnerEnum.V3)

			Private Shared ReadOnly valueList As New List(Of VFS_VER)()

			Shared Sub New()
				valueList.Add(V2)
				valueList.Add(V3)
			End Sub

			Public Enum InnerEnum
				V2
				V3
			End Enum

			Public ReadOnly innerEnumValue As InnerEnum
			Private ReadOnly nameValue As String
			Private ReadOnly ordinalValue As Integer
			Private Shared nextOrdinal As Integer = 0

			Private Sub New(ByVal name As String, ByVal thisInnerEnumValue As InnerEnum)
				nameValue = name
				ordinalValue = nextOrdinal
				nextOrdinal += 1
				innerEnumValue = thisInnerEnumValue
			End Sub

			Friend Sub New(ByVal name As String, ByVal thisInnerEnumValue As InnerEnum, ByVal outerInstance As VfsUtils)
				Me.outerInstance = outerInstance

				nameValue = name
				ordinalValue = nextOrdinal
				nextOrdinal += 1
				innerEnumValue = thisInnerEnumValue
			End Sub

			Public Shared Function values() As VFS_VER()
				Return valueList.ToArray()
			End Function

			Public Function ordinal() As Integer
				Return ordinalValue
			End Function

			Public Overrides Function ToString() As String
				Return nameValue
			End Function

			Public Shared Operator =(ByVal one As VFS_VER, ByVal two As VFS_VER) As Boolean
				Return one.innerEnumValue = two.innerEnumValue
			End Operator

			Public Shared Operator <>(ByVal one As VFS_VER, ByVal two As VFS_VER) As Boolean
				Return one.innerEnumValue <> two.innerEnumValue
			End Operator

			Public Shared Function valueOf(ByVal name As String) As VFS_VER
				For Each enumInstance As VFS_VER In VFS_VER.valueList
					If enumInstance.nameValue = name Then
						Return enumInstance
					End If
				Next
				Throw New System.ArgumentException(name)
			End Function
		End Class
	End Class

End Namespace