Imports System
Imports System.Collections.Generic
Imports GraphVertex = org.deeplearning4j.nn.conf.graph.GraphVertex
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

Namespace org.deeplearning4j.nn.conf.layers.samediff



	<Serializable>
	Public MustInherit Class SameDiffLambdaVertex
		Inherits SameDiffVertex

		<NonSerialized>
		Protected Friend inputs As VertexInputs

		''' <summary>
		''' The defineVertex method is used to define the foward pass for the vertex
		''' </summary>
		''' <param name="sameDiff"> SameDiff instance to use to define the vertex </param>
		''' <param name="inputs">   Layer input variable </param>
		''' <returns> The output variable (orresponding to the output activations for the vertex) </returns>
		Public MustOverride Function defineVertex(ByVal sameDiff As SameDiff, ByVal inputs As VertexInputs) As SDVariable

		Public Overrides Function defineVertex(ByVal sameDiff As SameDiff, ByVal layerInput As IDictionary(Of String, SDVariable), ByVal paramTable As IDictionary(Of String, SDVariable), ByVal maskVars As IDictionary(Of String, SDVariable)) As SDVariable
			Dim vi As VertexInputs = getInputs(sameDiff)
			Dim i As Integer = 0
			If vi.map.Count = 0 AndAlso layerInput.Count > 0 Then
				For Each v As SDVariable In layerInput.Values
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: vi.map.put(i++, v);
					vi.map(i) = v
						i += 1
				Next v
			End If
			Return defineVertex(sameDiff, getInputs(sameDiff))
		End Function

		Public Overrides Sub defineParametersAndInputs(ByVal params As SDVertexParams)
			'Parameters are no op, for lamda vertex - but inputs are NOT
			Dim temp As SameDiff = SameDiff.create()
			Dim tempInputs As New VertexInputs(Me, temp)
			defineVertex(temp, tempInputs)
			Dim list As IList(Of String) = New List(Of String)()
			For Each i As Integer? In tempInputs.map.Keys
				list.Add(tempInputs.map(i).name())
			Next i
			params.defineInputs(CType(list, List(Of String)).ToArray())
		End Sub

		Public Overrides Sub initializeParameters(ByVal params As IDictionary(Of String, INDArray))
			'No op, for lambda vertex
		End Sub

		Public Overrides Function clone() As GraphVertex
			Try
				Return Me.GetType().GetConstructor().newInstance()
			Catch e As Exception
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Throw New Exception("Unable to create new instance of class " & Me.GetType().FullName & " from no-arg constructor")
			End Try
		End Function

		Protected Friend Overridable Function getInputs(ByVal sd As SameDiff) As VertexInputs
			If inputs Is Nothing Then
				inputs = New VertexInputs(Me, sd)
			End If
			Return inputs
		End Function

		Public Class VertexInputs
			Private ReadOnly outerInstance As SameDiffLambdaVertex


			Friend sameDiff As SameDiff
			Friend map As IDictionary(Of Integer, SDVariable) = New LinkedHashMap(Of Integer, SDVariable)()

			Protected Friend Sub New(ByVal outerInstance As SameDiffLambdaVertex, ByVal sd As SameDiff)
				Me.outerInstance = outerInstance
				Me.sameDiff = sd
			End Sub

			Public Overridable Function getInput(ByVal inputNum As Integer) As SDVariable
				Preconditions.checkArgument(inputNum >= 0, "Input number must be >= 0." & "Got: %s", inputNum)

				If Not map.ContainsKey(inputNum) Then
					'Lazily define extra input variable as required
					Dim var As SDVariable = sameDiff.var("var_" & inputNum, outerInstance.dataType_Conflict, -1) 'TODO is this shape safe?
					map(inputNum) = var
				End If

				Return map(inputNum)
			End Function
		End Class
	End Class

End Namespace