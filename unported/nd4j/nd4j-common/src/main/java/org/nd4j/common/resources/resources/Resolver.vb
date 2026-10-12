Imports System.IO

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


	Public Interface Resolver

		''' <summary>
		''' Priority of this resolver. 0 is highest priority (check first), larger values are lower priority (check last)
		''' </summary>
		Function priority() As Integer

		''' <summary>
		''' Determine if the specified file resource can be resolved by <seealso cref="asFile(String)"/> and <seealso cref="asStream(String)"/>
		''' </summary>
		''' <param name="resourcePath"> Path of the resource to be resolved </param>
		''' <returns> True if this resolver is able to resolve the resource file - i.e., whether it is a valid path and exists </returns>
		Function exists(ByVal resourcePath As String) As Boolean

		''' <summary>
		''' Determine if the specified directory resource can be resolved by <seealso cref="copyDirectory(String, File)"/>
		''' </summary>
		''' <param name="dirPath"> Path of the directory resource to be resolved </param>
		''' <returns> True if this resolver is able to resolve the directory - i.e., whether it is a valid path and exists </returns>
		Function directoryExists(ByVal dirPath As String) As Boolean

		''' <summary>
		''' Get the specified resources as a standard local file.
		''' Note that the resource must exist as determined by <seealso cref="exists(String)"/>
		''' </summary>
		''' <param name="resourcePath"> Path of the resource. </param>
		''' <returns> The local file version of the resource </returns>
		Function asFile(ByVal resourcePath As String) As File

		''' <summary>
		''' Get the specified resources as an input stream.
		''' Note that the resource must exist as determined by <seealso cref="exists(String)"/>
		''' </summary>
		''' <param name="resourcePath"> Path of the resource. </param>
		''' <returns> The resource as an input stream </returns>
		Function asStream(ByVal resourcePath As String) As Stream

		''' <summary>
		''' Copy the directory resource (recursively) to the specified destination directory
		''' </summary>
		''' <param name="dirPath">        Path of the resource directory to resolve </param>
		''' <param name="destinationDir"> Where the files should be copied to </param>
		Sub copyDirectory(ByVal dirPath As String, ByVal destinationDir As File)

		''' <returns> True if the resolver has a local cache directory, as returned by <seealso cref="localCacheRoot()"/> </returns>
		Function hasLocalCache() As Boolean

		''' <returns> Root directory of the local cache, or null if <seealso cref="hasLocalCache()"/> returns false </returns>
		Function localCacheRoot() As File

		''' <summary>
		''' Normalize the path that may be a resource reference.
		''' For example: "someDir/myFile.zip.resource_reference" --> "someDir/myFile.zip"
		''' Returns null if the file cannot be resolved.
		''' If the file is not a reference, the original path is returned
		''' </summary>
		Function normalizePath(ByVal path As String) As String
	End Interface

End Namespace