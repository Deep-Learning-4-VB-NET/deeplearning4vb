Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Optimizer = org.nd4j.autodiff.samediff.optimize.Optimizer
Imports OptimizerSet = org.nd4j.autodiff.samediff.optimize.OptimizerSet
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
Namespace org.nd4j.autodiff.optimization.util


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class OptTestConfig
	Public Class OptTestConfig

		Private original As SameDiff
		Private placeholders As IDictionary(Of String, INDArray)
		Private outputs As IList(Of String)
		Private tempFolder As File
		Private mustApply As IDictionary(Of String, Type)
		Private optimizerSets As IList(Of OptimizerSet)

		Public Shared Function builder() As Builder
			Return New Builder()
		End Function

		Public Class Builder

'JAVA TO VB CONVERTER NOTE: The field original was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend original_Conflict As SameDiff
'JAVA TO VB CONVERTER NOTE: The field placeholders was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend placeholders_Conflict As IDictionary(Of String, INDArray)
'JAVA TO VB CONVERTER NOTE: The field outputs was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend outputs_Conflict As IList(Of String)
'JAVA TO VB CONVERTER NOTE: The field tempFolder was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend tempFolder_Conflict As File
'JAVA TO VB CONVERTER NOTE: The field mustApply was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend mustApply_Conflict As IDictionary(Of String, Type)
'JAVA TO VB CONVERTER NOTE: The field optimizerSets was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend optimizerSets_Conflict As IList(Of OptimizerSet)

'JAVA TO VB CONVERTER NOTE: The parameter tempFolder was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function tempFolder(ByVal tempFolder_Conflict As File) As Builder
				Me.tempFolder_Conflict = tempFolder_Conflict
				Return Me
			End Function

			Public Overridable Function original(ByVal sd As SameDiff) As Builder
				original_Conflict = sd
				Return Me
			End Function

			Public Overridable Function placeholder(ByVal ph As String, ByVal arr As INDArray) As Builder
				If placeholders_Conflict Is Nothing Then
					placeholders_Conflict = New Dictionary(Of String, INDArray)()
				End If
				placeholders(ph) = arr
				Return Me
			End Function

			Public Overridable Function placeholders(ByVal map As IDictionary(Of String, INDArray)) As Builder
				placeholders_Conflict = map
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter outputs was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function outputs(ParamArray ByVal outputs_Conflict() As String) As Builder
				Me.outputs_Conflict = New List(Of String) From {outputs_Conflict}
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter outputs was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function outputs(ByVal outputs_Conflict As IList(Of String)) As Builder
				Me.outputs_Conflict = outputs_Conflict
				Return Me
			End Function

			Public Overridable Function mustApply(ByVal opName As String, ByVal optimizerClass As Type) As Builder
				If mustApply_Conflict Is Nothing Then
					mustApply_Conflict = New Dictionary(Of String, Type)()
				End If
				mustApply(opName) = optimizerClass
				Return Me
			End Function

			Public Overridable Function optimizerSets(ByVal list As IList(Of OptimizerSet)) As Builder
				Me.optimizerSets_Conflict = list
				Return Me
			End Function

			Public Overridable Function build() As OptTestConfig
				Dim c As New OptTestConfig()
				c.original = original_Conflict
				c.placeholders = placeholders_Conflict
				c.outputs = outputs_Conflict
				c.tempFolder = tempFolder_Conflict
				c.mustApply = mustApply_Conflict
				c.optimizerSets = optimizerSets_Conflict
				Return c
			End Function

		End Class

	End Class
End Namespace