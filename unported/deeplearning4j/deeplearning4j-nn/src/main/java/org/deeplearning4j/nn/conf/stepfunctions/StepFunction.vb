Imports System
Imports JsonSubTypes = org.nd4j.shade.jackson.annotation.JsonSubTypes
Imports JsonTypeInfo = org.nd4j.shade.jackson.annotation.JsonTypeInfo
Imports [As] = org.nd4j.shade.jackson.annotation.JsonTypeInfo.As
Imports Id = org.nd4j.shade.jackson.annotation.JsonTypeInfo.Id

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

Namespace org.deeplearning4j.nn.conf.stepfunctions


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonTypeInfo(use = Id.NAME, include = @As.WRAPPER_OBJECT) @JsonSubTypes(value = {@JsonSubTypes.Type(value = DefaultStepFunction.class, name = "default"), @JsonSubTypes.Type(value = GradientStepFunction.class, name = "gradient"), @JsonSubTypes.Type(value = NegativeDefaultStepFunction.class, name = "negativeDefault"), @JsonSubTypes.Type(value = NegativeGradientStepFunction.class, name = "negativeGradient")}) public class StepFunction implements java.io.Serializable, Cloneable
	<Serializable>
	Public Class StepFunction
		Implements ICloneable

		Private Const serialVersionUID As Long = -1884835867123371330L

		Public Overrides Function clone() As StepFunction
			Try
'JAVA TO VB CONVERTER NOTE: The local variable clone was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
				Dim clone_Conflict As StepFunction = CType(MyBase.clone(), StepFunction)
				Return clone_Conflict
			Catch e As CloneNotSupportedException
				Throw New Exception(e)
			End Try
		End Function
	End Class

End Namespace