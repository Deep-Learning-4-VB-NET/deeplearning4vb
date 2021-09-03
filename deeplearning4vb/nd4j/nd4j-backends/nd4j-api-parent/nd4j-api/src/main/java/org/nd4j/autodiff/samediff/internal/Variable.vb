Imports System.Collections.Generic
Imports lombok
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable

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

Namespace org.nd4j.autodiff.samediff.internal


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @NoArgsConstructor @Data @Builder @EqualsAndHashCode(exclude = {"gradient", "variableIndex"}) public class Variable
	Public Class Variable
		Protected Friend name As String
		Protected Friend variable As SDVariable
		Protected Friend inputsForOp As IList(Of String)
		Protected Friend controlDepsForOp As IList(Of String) 'if a op control dependency (x -> opY) exists, then "opY" will be in this list
		Protected Friend controlDepsForVar As IList(Of String) 'if a variable control dependency (x -> varY) exists, then "varY" will be in this list
		Protected Friend outputOfOp As String 'Null for placeholders/constants. For array type SDVariables, the name of the op it's an output of
		Protected Friend controlDeps As IList(Of String) 'Control dependencies: name of ops that must be available before this variable is considered available for execution
		Protected Friend gradient As SDVariable 'Variable corresponding to the gradient of this variable
		Protected Friend variableIndex As Integer = -1
	End Class

End Namespace