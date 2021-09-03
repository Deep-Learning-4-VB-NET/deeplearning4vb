Imports System
Imports System.Collections.Generic
Imports AccessLevel = lombok.AccessLevel
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Setter = lombok.Setter
Imports Listener = org.nd4j.autodiff.listeners.Listener
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
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

Namespace org.nd4j.autodiff.samediff.config

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter public class BatchOutputConfig
	Public Class BatchOutputConfig
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter(lombok.AccessLevel.NONE) private org.nd4j.autodiff.samediff.SameDiff sd;
		Private sd As SameDiff

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NonNull private java.util.List<String> outputs = new java.util.ArrayList<>();
		Private outputs As IList(Of String) = New List(Of String)()

		Private placeholders As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NonNull private java.util.List<org.nd4j.autodiff.listeners.Listener> listeners = new java.util.ArrayList<>();
'JAVA TO VB CONVERTER NOTE: The field listeners was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private listeners_Conflict As IList(Of Listener) = New List(Of Listener)()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BatchOutputConfig(@NonNull SameDiff sd)
		Public Sub New(ByVal sd As SameDiff)
			Me.sd = sd
		End Sub

		''' <summary>
		''' Add required outputs
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BatchOutputConfig output(@NonNull String... outputs)
		Public Overridable Function output(ParamArray ByVal outputs() As String) As BatchOutputConfig
			CType(Me.outputs, List(Of String)).AddRange(New List(Of String) From {outputs})
			Return Me
		End Function

		''' <summary>
		''' Add required outputs
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BatchOutputConfig output(@NonNull SDVariable... outputs)
		Public Overridable Function output(ParamArray ByVal outputs() As SDVariable) As BatchOutputConfig
			Dim outNames(outputs.Length - 1) As String
			For i As Integer = 0 To outputs.Length - 1
				outNames(i) = outputs(i).name()
			Next i

			Return output(outNames)
		End Function

		''' <summary>
		''' Add all variables as required outputs
		''' </summary>
		Public Overridable Function outputAll() As BatchOutputConfig
			Return output(CType(sd.variables(), List(Of SDVariable)).ToArray())
		End Function

		''' <summary>
		''' Add a placeholder value for a specified variable
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BatchOutputConfig input(@NonNull String variable, @NonNull INDArray placeholder)
		Public Overridable Function input(ByVal variable As String, ByVal placeholder As INDArray) As BatchOutputConfig
			Preconditions.checkState(Not placeholders.ContainsKey(variable), "Placeholder for variable %s already specified", variable)

			Preconditions.checkNotNull(sd.getVariable(variable), "Variable %s does not exist in this SameDiff graph", variable)

			placeholders(variable) = placeholder
			Return Me
		End Function

		''' <summary>
		''' See <seealso cref="input(String, INDArray)"/>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BatchOutputConfig input(@NonNull SDVariable variable, @NonNull INDArray placeholder)
		Public Overridable Function input(ByVal variable As SDVariable, ByVal placeholder As INDArray) As BatchOutputConfig
			Return input(variable.name(), placeholder)
		End Function

		''' <summary>
		''' Calls <seealso cref="input(String, INDArray)"/> on each entry in the map.
		''' </summary>
		Public Overridable Function inputs(ByVal placeholders As IDictionary(Of String, INDArray)) As BatchOutputConfig

			If placeholders Is Nothing Then
				Me.placeholders = Nothing
				Return Me
			End If

			For Each e As KeyValuePair(Of String, INDArray) In placeholders.SetOfKeyValuePairs()
				input(e.Key, e.Value)
			Next e

			Return Me
		End Function

		''' <summary>
		''' Add listeners for this operation
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BatchOutputConfig listeners(@NonNull Listener... listeners)
'JAVA TO VB CONVERTER NOTE: The parameter listeners was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Public Overridable Function listeners(ParamArray ByVal listeners_Conflict() As Listener) As BatchOutputConfig
			CType(Me.listeners_Conflict, List(Of Listener)).AddRange(New List(Of Listener) From {listeners_Conflict})
			Return Me
		End Function

		''' @deprecated Use <seealso cref="output()"/> 
		<Obsolete("Use <seealso cref=""output()""/>")>
		Public Overridable Function exec() As IDictionary(Of String, INDArray)
			Return output()
		End Function

		''' <summary>
		''' Do inference and return the results
		''' </summary>
		Public Overridable Function output() As IDictionary(Of String, INDArray)
			Return sd.output(placeholders, listeners_Conflict, CType(outputs, List(Of String)).ToArray())
		End Function

		''' @deprecated Use <seealso cref="outputSingle()"/> 
		<Obsolete("Use <seealso cref=""outputSingle()""/>")>
		Public Overridable Function execSingle() As INDArray
			Return outputSingle()
		End Function

		''' <summary>
		''' Do inference and return the results for the single output
		''' 
		''' Only works if exactly one output is specified
		''' </summary>
		Public Overridable Function outputSingle() As INDArray
			Preconditions.checkState(outputs.Count = 1, "Can only use execSingle() when exactly one output is specified, there were %s", outputs.Count)
			Return exec()(outputs(0))
		End Function
	End Class

End Namespace