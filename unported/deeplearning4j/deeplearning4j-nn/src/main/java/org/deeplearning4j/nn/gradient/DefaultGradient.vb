Imports System
Imports System.Collections.Generic
Imports Setter = lombok.Setter
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
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

Namespace org.deeplearning4j.nn.gradient


	<Serializable>
	Public Class DefaultGradient
		Implements Gradient

		Public Const DEFAULT_FLATTENING_ORDER As Char = "f"c
		Private gradients As IDictionary(Of String, INDArray) = New LinkedHashMap(Of String, INDArray)()
		Private flatteningOrders As IDictionary(Of String, Char)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter private org.nd4j.linalg.api.ndarray.INDArray flattenedGradient;
		Private flattenedGradient As INDArray

		Public Sub New()
		End Sub

		Public Sub New(ByVal flattenedGradient As INDArray)
			Me.flattenedGradient = flattenedGradient
		End Sub

		Public Overridable Function gradientForVariable() As IDictionary(Of String, INDArray) Implements Gradient.gradientForVariable
			Return gradients
		End Function

		Public Overridable Function gradient(ByVal order As IList(Of String)) As INDArray Implements Gradient.gradient
			Dim toFlatten As IList(Of INDArray) = New List(Of INDArray)()
			If flatteningOrders Is Nothing Then
				For Each s As String In order
					If Not gradients.ContainsKey(s) Then
						Continue For
					End If
					toFlatten.Add(gradients(s))
				Next s
			Else
				For Each s As String In order
					If Not gradients.ContainsKey(s) Then
						Continue For
					End If
					If flatteningOrders.ContainsKey(s) AndAlso flatteningOrders(s) <> DEFAULT_FLATTENING_ORDER Then
						'Arrays with non-default order get flattened to row vector first, then everything is flattened to f order
						'TODO revisit this, and make more efficient
						toFlatten.Add(Nd4j.toFlattened(flatteningOrders(s), gradients(s)))
					Else
						toFlatten.Add(gradients(s))
					End If
				Next s
			End If
			Dim ret As INDArray = Nd4j.toFlattened(DEFAULT_FLATTENING_ORDER, toFlatten)
			Return ret.reshape("c"c, 1, ret.length())
		End Function

		Private Sub flattenGradient()
			If flatteningOrders IsNot Nothing Then
				'Arrays with non-default order get flattened to row vector first, then everything is flattened to f order
				'TODO revisit this, and make more efficient
				Dim toFlatten As IList(Of INDArray) = New List(Of INDArray)()
				For Each entry As KeyValuePair(Of String, INDArray) In gradients.SetOfKeyValuePairs()
					If flatteningOrders.ContainsKey(entry.Key) AndAlso flatteningOrders(entry.Key) <> DEFAULT_FLATTENING_ORDER Then
						'Specific flattening order for this array, that isn't the default
						toFlatten.Add(Nd4j.toFlattened(flatteningOrders(entry.Key), entry.Value))
					Else
						'default flattening order for this array
						toFlatten.Add(entry.Value)
					End If
				Next entry
				flattenedGradient = Nd4j.toFlattened(DEFAULT_FLATTENING_ORDER, toFlatten)
			ElseIf gradients.Values.Count > 0 Then 'Edge case: can be empty for nets with 0 params
				'Standard case: flatten all to f order
				flattenedGradient = Nd4j.toFlattened(DEFAULT_FLATTENING_ORDER, gradients.Values)

			End If
			If flattenedGradient.rank() = 1 Then
				flattenedGradient = flattenedGradient.reshape("c"c, 1, flattenedGradient.length())
			End If
		End Sub

		Public Overridable Function gradient() As INDArray Implements Gradient.gradient
			If flattenedGradient IsNot Nothing Then
				Return flattenedGradient
			End If
			flattenGradient()
			Return flattenedGradient
		End Function

		Public Overridable Sub clear() Implements Gradient.clear
			gradients.Clear()
		End Sub

		Public Overridable Function getGradientFor(ByVal variable As String) As INDArray Implements Gradient.getGradientFor
			Return gradients(variable)
		End Function

		Public Overridable Function setGradientFor(ByVal variable As String, ByVal newGradient As INDArray) As INDArray Implements Gradient.setGradientFor
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: org.nd4j.linalg.api.ndarray.INDArray last = gradients.put(variable, newGradient);
			gradients(variable) = newGradient
				Dim last As INDArray = gradients(variable)
			' TODO revisit whether setGradientFor should update the gradient that can be pulled from this object in any form - currently does not update flattened
			' use of unitialized var for flattengradient in backprop is generating an error in gradient calc if bellow is used
			'        flattenGradient();
			Return last
		End Function

		Public Overridable Function setGradientFor(ByVal variable As String, ByVal gradient As INDArray, ByVal flatteningOrder As Char?) As INDArray Implements Gradient.setGradientFor
			Dim last As INDArray = setGradientFor(variable, gradient)

			If flatteningOrder IsNot Nothing Then
				If flatteningOrders Is Nothing Then
					flatteningOrders = New LinkedHashMap(Of String, Char)()
				End If
				flatteningOrders(variable) = flatteningOrder
			End If
			Return last
		End Function

		Public Overridable Function flatteningOrderForVariable(ByVal variable As String) As Char? Implements Gradient.flatteningOrderForVariable
			If flatteningOrders Is Nothing Then
				Return Nothing
			End If
			Return flatteningOrders(variable)
		End Function


		Public Overrides Function ToString() As String
			Return "DefaultGradient{" & "gradients=" & gradients + (If(flatteningOrders IsNot Nothing, flatteningOrders, "")) & "}"c
		End Function
	End Class

End Namespace