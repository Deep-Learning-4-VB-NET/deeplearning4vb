Imports System.IO
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
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

Namespace org.deeplearning4j.ui.i18n


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data public class I18NResource
	Public Class I18NResource
		Private ReadOnly resource As String

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public java.io.InputStream getInputStream() throws java.io.IOException
		Public Overridable ReadOnly Property InputStream As Stream
			Get
				Return (New ClassPathResource(resource)).InputStream
			End Get
		End Property
	End Class

End Namespace