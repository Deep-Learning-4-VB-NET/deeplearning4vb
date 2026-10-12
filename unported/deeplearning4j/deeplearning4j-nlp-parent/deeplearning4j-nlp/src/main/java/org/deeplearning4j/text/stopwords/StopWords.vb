Imports System
Imports System.Collections.Generic
Imports IOUtils = org.apache.commons.io.IOUtils
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource

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

Namespace org.deeplearning4j.text.stopwords


	Public Class StopWords

		Private Shared stopWords As IList(Of String)

		Private Sub New()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") public static java.util.List<String> getStopWords()
		Public Shared ReadOnly Property StopWords As IList(Of String)
			Get
    
				Try
					If stopWords Is Nothing Then
						stopWords = IOUtils.readLines((New ClassPathResource("/stopwords.txt")).InputStream)
					End If
				Catch e As IOException
					Throw New Exception(e)
				End Try
				Return stopWords
			End Get
		End Property

	End Class

End Namespace