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



	Public Interface Resource
		Inherits InputStreamSource

		''' <summary>
		''' Whether the resource exists on the classpath
		''' @return
		''' </summary>
		Function exists() As Boolean

		''' 
		''' <summary>
		''' @return
		''' </summary>
		ReadOnly Property Readable As Boolean

		''' 
		''' <summary>
		''' @return
		''' </summary>
		ReadOnly Property Open As Boolean

		''' 
		''' <summary>
		''' @return </summary>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: java.net.URL getURL() throws java.io.IOException;
		ReadOnly Property URL As URL

		''' 
		''' <summary>
		''' @return </summary>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: java.net.URI getURI() throws java.io.IOException;
		ReadOnly Property URI As URI

		''' 
		''' <summary>
		''' @return </summary>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: java.io.File getFile() throws java.io.IOException;
		ReadOnly Property File As File

		''' 
		''' <summary>
		''' @return </summary>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: long contentLength() throws java.io.IOException;
		Function contentLength() As Long

		''' 
		''' <summary>
		''' @return </summary>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: long lastModified() throws java.io.IOException;
		Function lastModified() As Long

		''' 
		''' <param name="var1">
		''' @return </param>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: Resource createRelative(String var1) throws java.io.IOException;
		Function createRelative(ByVal var1 As String) As Resource

		''' 
		''' <summary>
		''' @return
		''' </summary>
		ReadOnly Property Filename As String

		''' 
		''' <summary>
		''' @return
		''' </summary>
		ReadOnly Property Description As String
	End Interface

End Namespace