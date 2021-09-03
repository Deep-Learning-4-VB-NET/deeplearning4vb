Imports System
Imports System.IO
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

Namespace org.nd4j.context


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class Nd4jContext implements java.io.Serializable
	<Serializable>
	Public Class Nd4jContext

'JAVA TO VB CONVERTER NOTE: The field conf was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private conf_Conflict As Properties
'JAVA TO VB CONVERTER NOTE: The field INSTANCE was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared INSTANCE_Conflict As New Nd4jContext()

		Private Sub New()
			conf_Conflict = New Properties()
			conf_Conflict.putAll(System.getProperties())
		End Sub

		Public Shared ReadOnly Property Instance As Nd4jContext
			Get
				Return INSTANCE_Conflict
			End Get
		End Property

		''' <summary>
		''' Load the additional properties from an input stream and load all system properties
		''' </summary>
		''' <param name="inputStream"> </param>
		Public Overridable Sub updateProperties(ByVal inputStream As Stream)
			Try
				conf_Conflict.load(inputStream)
				conf_Conflict.putAll(System.getProperties())
			Catch e As IOException
				log.warn("Error loading system properties from input stream", e)
			End Try
		End Sub

		''' <summary>
		''' Get the configuration for nd4j
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property Conf As Properties
			Get
				Return conf_Conflict
			End Get
		End Property
	End Class

End Namespace