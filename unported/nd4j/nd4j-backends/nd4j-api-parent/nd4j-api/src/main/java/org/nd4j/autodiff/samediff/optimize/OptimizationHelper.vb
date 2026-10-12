Imports Getter = lombok.Getter
Imports ArrayHolder = org.nd4j.autodiff.samediff.ArrayHolder
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports VariableType = org.nd4j.autodiff.samediff.VariableType
Imports OptimizedGraphArrayHolder = org.nd4j.autodiff.samediff.array.OptimizedGraphArrayHolder
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports org.nd4j.common.function
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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


Namespace org.nd4j.autodiff.samediff.optimize


	Public Class OptimizationHelper

		Private ReadOnly originalGraph As SameDiff
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final java.util.Properties properties;
		Private ReadOnly properties As Properties
		Private setConstantHolder As Boolean = False
		Private setVariableHolder As Boolean = False

		Public Sub New(ByVal originalGraph As SameDiff, ByVal properties As Properties)
			Me.originalGraph = originalGraph
			Me.properties = properties
		End Sub

		Public Overridable Function arrayRecoveryFunction(ByVal arrayName As String, ByVal fn As Supplier(Of INDArray)) As OptimizationHelper
			Dim v As SDVariable = originalGraph.getVariable(arrayName)
			Preconditions.checkState(v.getVariableType() = VariableType.VARIABLE OrElse v.getVariableType() = VariableType.CONSTANT, "Can only set an array recovery function for a variable or a constant")

			If v.getVariableType() = VariableType.VARIABLE Then
				Dim h As ArrayHolder = originalGraph.getVariablesArrays()
				If Not setVariableHolder Then
					originalGraph.setVariablesArrays(New OptimizedGraphArrayHolder(h))
					h = originalGraph.getVariablesArrays()
					setVariableHolder = True
				End If
				DirectCast(h, OptimizedGraphArrayHolder).setFunction(arrayName, fn)
			Else
				Dim h As ArrayHolder = originalGraph.getConstantArrays()
				If Not setConstantHolder Then
					originalGraph.setConstantArrays(New OptimizedGraphArrayHolder(h))
					h = originalGraph.getConstantArrays()
					setConstantHolder = True
				End If
				DirectCast(h, OptimizedGraphArrayHolder).setFunction(arrayName, fn)
			End If

			Return Me
		End Function

	End Class

End Namespace