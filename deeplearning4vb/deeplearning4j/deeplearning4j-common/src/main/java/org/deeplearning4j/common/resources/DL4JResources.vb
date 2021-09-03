Imports System
Imports NonNull = lombok.NonNull
Imports DL4JSystemProperties = org.deeplearning4j.common.config.DL4JSystemProperties
Imports Preconditions = org.nd4j.common.base.Preconditions

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

Namespace org.deeplearning4j.common.resources


	Public Class DL4JResources

		''' @deprecated Use <seealso cref="DL4JSystemProperties.DL4J_RESOURCES_DIR_PROPERTY"/> 
		<Obsolete("Use <seealso cref=""DL4JSystemProperties.DL4J_RESOURCES_DIR_PROPERTY""/>")>
		Public Const DL4J_RESOURCES_DIR_PROPERTY As String = DL4JSystemProperties.DL4J_RESOURCES_DIR_PROPERTY
		''' @deprecated Use <seealso cref="DL4JSystemProperties.DL4J_RESOURCES_BASE_URL_PROPERTY"/> 
		<Obsolete("Use <seealso cref=""DL4JSystemProperties.DL4J_RESOURCES_BASE_URL_PROPERTY""/>")>
		Public Const DL4J_BASE_URL_PROPERTY As String = DL4JSystemProperties.DL4J_RESOURCES_BASE_URL_PROPERTY
		Private Const DL4J_DEFAULT_URL As String = "https://dl4jdata.blob.core.windows.net/"

'JAVA TO VB CONVERTER NOTE: The field baseDirectory was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared baseDirectory_Conflict As File
		Private Shared baseURL As String

		Shared Sub New()
			resetBaseDirectoryLocation()

			Dim [property] As String = System.getProperty(DL4JSystemProperties.DL4J_RESOURCES_BASE_URL_PROPERTY)
			If [property] IsNot Nothing Then
				baseURL = [property]
			Else
				baseURL = DL4J_DEFAULT_URL
			End If

		End Sub

		''' <summary>
		''' Set the base download URL for (most) DL4J datasets and models.<br>
		''' This usually doesn't need to be set manually unless there is some issue with the default location </summary>
		''' <param name="baseDownloadURL"> Base download URL to set. For example, https://dl4jdata.blob.core.windows.net/ </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static void setBaseDownloadURL(@NonNull String baseDownloadURL)
		Public Shared Property BaseDownloadURL As String
			Set(ByVal baseDownloadURL As String)
				baseURL = baseDownloadURL
			End Set
			Get
				Return baseURL
			End Get
		End Property


		''' <summary>
		''' Get the URL relative to the base URL.<br>
		''' For example, if baseURL is "https://dl4jdata.blob.core.windows.net/", and relativeToBase is "/datasets/iris.dat"
		''' this simply returns "https://dl4jdata.blob.core.windows.net/datasets/iris.dat"
		''' </summary>
		''' <param name="relativeToBase"> Relative URL </param>
		''' <returns> URL </returns>
		''' <exception cref="MalformedURLException"> For bad URL </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static java.net.URL getURL(String relativeToBase) throws java.net.MalformedURLException
		Public Shared Function getURL(ByVal relativeToBase As String) As URL
			Return New URL(getURLString(relativeToBase))
		End Function

		''' <summary>
		''' Get the URL relative to the base URL as a String.<br>
		''' For example, if baseURL is "https://dl4jdata.blob.core.windows.net/", and relativeToBase is "/datasets/iris.dat"
		''' this simply returns "https://dl4jdata.blob.core.windows.net/datasets/iris.dat"
		''' </summary>
		''' <param name="relativeToBase"> Relative URL </param>
		''' <returns> URL </returns>
		''' <exception cref="MalformedURLException"> For bad URL </exception>
		Public Shared Function getURLString(ByVal relativeToBase As String) As String
			If relativeToBase.StartsWith("/", StringComparison.Ordinal) Then
				relativeToBase = relativeToBase.Substring(1)
			End If
			Return baseURL & relativeToBase
		End Function

		''' <summary>
		''' Reset to the default directory, or the directory set via the <seealso cref="DL4JSystemProperties.DL4J_RESOURCES_DIR_PROPERTY"/> system property,
		''' org.deeplearning4j.resources.directory
		''' </summary>
		Public Shared Sub resetBaseDirectoryLocation()
			Dim [property] As String = System.getProperty(DL4JSystemProperties.DL4J_RESOURCES_DIR_PROPERTY)
			If [property] IsNot Nothing Then
				baseDirectory_Conflict = New File([property])
			Else
				baseDirectory_Conflict = New File(System.getProperty("user.home"), ".deeplearning4j")
			End If

			If Not baseDirectory_Conflict.exists() Then
				baseDirectory_Conflict.mkdirs()
			End If
		End Sub

		''' <summary>
		''' Set the base directory for local storage of files. Default is: {@code new File(System.getProperty("user.home"), ".deeplearning4j")} </summary>
		''' <param name="f"> Base directory to use for resources </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static void setBaseDirectory(@NonNull File f)
		Public Shared Property BaseDirectory As File
			Set(ByVal f As File)
				Preconditions.checkState(f.exists() AndAlso f.isDirectory(), "Specified base directory does not exist and/or is not a directory: %s", f.getAbsolutePath())
				baseDirectory_Conflict = f
			End Set
			Get
				Return baseDirectory_Conflict
			End Get
		End Property


		''' <summary>
		''' Get the storage location for the specified resource type and resource name </summary>
		''' <param name="resourceType"> Type of resource </param>
		''' <param name="resourceName"> Name of the resource </param>
		''' <returns> The root directory. Creates the directory and any parent directories, if required </returns>
		Public Shared Function getDirectory(ByVal resourceType As ResourceType, ByVal resourceName As String) As File
			Dim f As New File(baseDirectory_Conflict, resourceType.resourceName())
			f = New File(f, resourceName)
			f.mkdirs()
			Return f
		End Function
	End Class

End Namespace