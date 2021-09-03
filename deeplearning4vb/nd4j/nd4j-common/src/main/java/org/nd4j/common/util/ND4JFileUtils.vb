Imports System
Imports ND4JSystemProperties = org.nd4j.common.config.ND4JSystemProperties

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

Namespace org.nd4j.common.util


	Public Class ND4JFileUtils

		Private Sub New()
		End Sub

		''' <summary>
		''' Create a temporary file in the location specified by <seealso cref="ND4JSystemProperties.ND4J_TEMP_DIR_PROPERTY"/> if set,
		''' or the default temporary directory (usually specified by java.io.tmpdir system property) </summary>
		''' <param name="prefix"> Prefix for generating file's name; must be at least 3 characeters </param>
		''' <param name="suffix"> Suffix for generating file's name; may be null (".tmp" will be used if null) </param>
		''' <returns> A temporary file </returns>
		Public Shared Function createTempFile(ByVal prefix As String, ByVal suffix As String) As File
			Dim p As String = System.getProperty(ND4JSystemProperties.ND4J_TEMP_DIR_PROPERTY)
			Try
				If p Is Nothing OrElse p.Length = 0 Then
					Return File.createTempFile(prefix, suffix)
				Else
					Return File.createTempFile(prefix, suffix, New File(p))
				End If
			Catch e As IOException
				Throw New Exception("Error creating temporary file", e)
			End Try
		End Function

		''' <summary>
		''' Get the temporary directory. This is the location specified by <seealso cref="ND4JSystemProperties.ND4J_TEMP_DIR_PROPERTY"/> if set,
		''' or the default temporary directory (usually specified by java.io.tmpdir system property) </summary>
		''' <returns> Temporary directory </returns>
		Public Shared ReadOnly Property TempDir As File
			Get
				Dim p As String = System.getProperty(ND4JSystemProperties.ND4J_TEMP_DIR_PROPERTY)
				If p Is Nothing OrElse p.Length = 0 Then
					Return New File(System.getProperty("java.io.tmpdir"))
				Else
					Return New File(p)
				End If
			End Get
		End Property

	End Class

End Namespace