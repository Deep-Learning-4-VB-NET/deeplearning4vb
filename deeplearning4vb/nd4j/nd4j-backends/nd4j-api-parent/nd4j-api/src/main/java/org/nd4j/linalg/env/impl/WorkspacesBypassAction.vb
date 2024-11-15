﻿Imports System
Imports val = lombok.val
Imports DebugMode = org.nd4j.linalg.api.memory.enums.DebugMode
Imports EnvironmentalAction = org.nd4j.linalg.env.EnvironmentalAction
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.nd4j.linalg.env.impl

	Public Class WorkspacesBypassAction
		Implements EnvironmentalAction

		Public Overridable Function targetVariable() As String Implements EnvironmentalAction.targetVariable
			Return "ND4J_BYPASS_WS"
		End Function

		Public Overridable Sub process(ByVal value As String) Implements EnvironmentalAction.process
			Dim v As val = Convert.ToBoolean(value).booleanValue()

			If v Then
				Nd4j.WorkspaceManager.DebugMode = DebugMode.BYPASS_EVERYTHING
			End If
		End Sub
	End Class

End Namespace