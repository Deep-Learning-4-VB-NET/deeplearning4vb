Imports System.Collections.Generic
Imports System.IO
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports ND4JClassLoading = org.nd4j.common.config.ND4JClassLoading
Imports StrumpfResolver = org.nd4j.common.resources.strumpf.StrumpfResolver

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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class Resources
	Public Class Resources
		Private Shared INSTANCE As New Resources()

		Protected Friend ReadOnly resolvers As IList(Of Resolver)

		Protected Friend Sub New()
			Dim loader As ServiceLoader(Of Resolver) = ND4JClassLoading.loadService(GetType(Resolver))

			resolvers = New List(Of Resolver)()
			resolvers.Add(New StrumpfResolver())
			For Each resolver As Resolver In loader
				resolvers.Add(resolver)
			Next resolver

			'Sort resolvers by priority: check resolvers with lower numbers first
			resolvers.Sort(New ComparatorAnonymousInnerClass(Me))
		End Sub

		Private Class ComparatorAnonymousInnerClass
			Implements IComparer(Of Resolver)

			Private ReadOnly outerInstance As Resources

			Public Sub New(ByVal outerInstance As Resources)
				Me.outerInstance = outerInstance
			End Sub

			Public Function Compare(ByVal r1 As Resolver, ByVal r2 As Resolver) As Integer Implements IComparer(Of Resolver).Compare
				Return Integer.compare(r1.priority(), r2.priority())
			End Function
		End Class

		''' <summary>
		''' Check if the specified resource exists (can be resolved by any method) hence can be loaded by <seealso cref="asFile(String)"/>
		''' or <seealso cref="asStream(String)"/>
		''' </summary>
		''' <param name="resourcePath"> Path of the resource to be resolved </param>
		''' <returns> Whether the resource can be resolved or not </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static boolean exists(@NonNull String resourcePath)
		Public Shared Function exists(ByVal resourcePath As String) As Boolean
			Return INSTANCE.resourceExists(resourcePath)
		End Function

		''' <summary>
		''' Get the specified resource as a local file.
		''' If it cannot be found (i.e., <seealso cref="exists(String)"/> returns false) this method will throw an exception.
		''' </summary>
		''' <param name="resourcePath"> Path of the resource to get </param>
		''' <returns> Resource file </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static java.io.File asFile(@NonNull String resourcePath)
		Public Shared Function asFile(ByVal resourcePath As String) As File
			Return INSTANCE.getAsFile(resourcePath)
		End Function

		''' <summary>
		''' Get the specified resource as an input stream.<br>
		''' If it cannot be found (i.e., <seealso cref="exists(String)"/> returns false) this method will throw an exception.
		''' </summary>
		''' <param name="resourcePath"> Path of the resource to get </param>
		''' <returns> Resource stream </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static java.io.InputStream asStream(@NonNull String resourcePath)
		Public Shared Function asStream(ByVal resourcePath As String) As Stream
			Return INSTANCE.getAsStream(resourcePath)
		End Function

		''' <summary>
		''' Copy the contents of the specified directory (path) to the specified destination directory, resolving any resources in the process
		''' </summary>
		''' <param name="directoryPath">  Directory to copy contents of </param>
		''' <param name="destinationDir"> Destination </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static void copyDirectory(@NonNull String directoryPath, @NonNull File destinationDir)
		Public Shared Sub copyDirectory(ByVal directoryPath As String, ByVal destinationDir As File)
			INSTANCE.copyDir(directoryPath, destinationDir)
		End Sub

		''' <summary>
		''' Normalize the path that may be a resource reference.
		''' For example: "someDir/myFile.zip.resource_reference" --> "someDir/myFile.zip"
		''' Returns null if the file cannot be resolved.
		''' If the file is not a reference, the original path is returned
		''' </summary>
		Public Shared Function normalizePath(ByVal path As String) As String
			Return INSTANCE.normalize(path)
		End Function

		Protected Friend Overridable Function resourceExists(ByVal resourcePath As String) As Boolean
			For Each r As Resolver In resolvers
				If r.exists(resourcePath) Then
					Return True
				End If
			Next r

			Return False
		End Function

		Protected Friend Overridable Function getAsFile(ByVal resourcePath As String) As File
			For Each r As Resolver In resolvers
				If r.exists(resourcePath) Then
					Return r.asFile(resourcePath)
				End If
			Next r

			Throw New System.InvalidOperationException("Cannot resolve resource (not found): none of " & resolvers.Count & " resolvers can resolve resource """ & resourcePath & """ - available resolvers: " & resolvers.ToString())
		End Function

		Public Overridable Function getAsStream(ByVal resourcePath As String) As Stream
			For Each r As Resolver In resolvers
				If r.exists(resourcePath) Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					log.debug("Resolved resource with resolver " & r.GetType().FullName & " for path " & resourcePath)
					Return r.asStream(resourcePath)
				End If
			Next r

			Throw New System.InvalidOperationException("Cannot resolve resource (not found): none of " & resolvers.Count & " resolvers can resolve resource """ & resourcePath & """ - available resolvers: " & resolvers.ToString())
		End Function

		Public Overridable Sub copyDir(ByVal directoryPath As String, ByVal destinationDir As File)
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

	End Class

End Namespace