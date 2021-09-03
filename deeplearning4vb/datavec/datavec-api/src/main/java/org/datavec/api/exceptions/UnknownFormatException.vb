﻿Imports System

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

Namespace org.datavec.api.exceptions

	''' <summary>
	''' @author sonali
	''' </summary>
	Public Class UnknownFormatException
		Inherits Exception

		Public Sub New()
		End Sub

		Public Sub New(ByVal message As String)
			MyBase.New(message)
		End Sub

		Public Sub New(ByVal message As String, ByVal cause As Exception)
			MyBase.New(message, cause)
		End Sub

		Public Sub New(ByVal cause As Exception)
			MyBase.New(cause)
		End Sub

		Public Sub New(ByVal message As String, ByVal cause As Exception, ByVal enableSuppression As Boolean, ByVal writableStackTrace As Boolean)
			MyBase.New(message, cause, enableSuppression, writableStackTrace)
		End Sub
	End Class

End Namespace