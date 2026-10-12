Imports System

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

Namespace org.nd4j.linalg.api.blas

	Public Class BlasException
		Inherits Exception

		Public Const serialVersionUID As Long = &HdeadbeefL

		Public Const UNKNOWN_ERROR As Integer = -200

		' return code from the library - non zero == err
'JAVA TO VB CONVERTER NOTE: The field errorCode was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Friend errorCode_Conflict As Integer

		Public Overridable ReadOnly Property ErrorCode As Integer
			Get
				Return errorCode_Conflict
			End Get
		End Property

		''' <summary>
		''' Principal constructor - error message & error code </summary>
		''' <param name="message"> the error message to put into the Exception </param>
		''' <param name="errorCode"> the library error number </param>
		Public Sub New(ByVal message As String, ByVal errorCode As Integer)
			MyBase.New(message & ": " & errorCode)
			Me.errorCode_Conflict = errorCode
		End Sub

		Public Sub New(ByVal message As String)
			Me.New(message, UNKNOWN_ERROR)
		End Sub

	End Class

End Namespace