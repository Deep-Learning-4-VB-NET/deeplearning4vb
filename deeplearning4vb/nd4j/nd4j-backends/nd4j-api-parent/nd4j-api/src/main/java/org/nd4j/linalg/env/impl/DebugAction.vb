Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
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

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class DebugAction implements org.nd4j.linalg.env.EnvironmentalAction
	Public Class DebugAction
		Implements EnvironmentalAction

		Public Overridable Function targetVariable() As String Implements EnvironmentalAction.targetVariable
			Return "ND4J_DEBUG"
		End Function

		Public Overridable Sub process(ByVal value As String) Implements EnvironmentalAction.process
			Dim v As val = Convert.ToBoolean(value)

			Nd4j.Executioner.enableDebugMode(v)
		End Sub
	End Class

End Namespace