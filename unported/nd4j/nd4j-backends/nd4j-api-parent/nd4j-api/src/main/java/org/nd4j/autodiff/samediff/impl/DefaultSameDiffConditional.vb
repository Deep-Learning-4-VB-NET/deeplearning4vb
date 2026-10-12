Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports SameDiffConditional = org.nd4j.autodiff.samediff.SameDiffConditional
Imports SameDiffFunctionDefinition = org.nd4j.autodiff.samediff.SameDiffFunctionDefinition

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

Namespace org.nd4j.autodiff.samediff.impl

	Public Class DefaultSameDiffConditional
		Implements SameDiffConditional

		Public Overridable Function eval(ByVal context As SameDiff, ByVal body As SameDiffFunctionDefinition, ByVal inputVars() As SDVariable) As SDVariable
			context.defineFunction("eval", body, inputVars)
			context.invokeFunctionOn("eval", context)
	'        return new ArrayList<>(context.getFunctionInstancesById().values()).get(context.getFunctionInstancesById().size() - 1).outputVariables()[0];
			Throw New System.NotSupportedException("Not yet reimplemented")
		End Function
	End Class

End Namespace