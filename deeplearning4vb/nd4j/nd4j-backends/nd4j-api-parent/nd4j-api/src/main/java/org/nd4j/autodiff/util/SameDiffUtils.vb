Imports System
Imports System.Collections.Generic
Imports AccessLevel = lombok.AccessLevel
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports DifferentialFunction = org.nd4j.autodiff.functions.DifferentialFunction
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ExternalErrorsFunction = org.nd4j.linalg.api.ops.impl.layers.ExternalErrorsFunction
Imports ReductionShape = org.nd4j.linalg.api.ops.impl.shape.ReductionShape
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
Imports ND4JException = org.nd4j.linalg.exception.ND4JException
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

Namespace org.nd4j.autodiff.util


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor(access = AccessLevel.@PRIVATE) public class SameDiffUtils
	Public Class SameDiffUtils

		''' <summary>
		''' Stack batch outputs, like an output from <seealso cref="org.nd4j.autodiff.samediff.SameDiff.output(MultiDataSetIterator, String...)"/>
		''' </summary>
		Public Shared Function stackOutputs(ByVal outputs As IList(Of IDictionary(Of String, INDArray))) As IDictionary(Of String, INDArray)
			Dim outs As IDictionary(Of String, IList(Of INDArray)) = New Dictionary(Of String, IList(Of INDArray))()
			For Each batch As IDictionary(Of String, INDArray) In outputs
				For Each k As String In batch.Keys
					If Not outs.ContainsKey(k) Then
						outs(k) = New List(Of INDArray)()
					End If
					outs(k).Add(batch(k))
				Next k
			Next batch

			Dim ret As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			For Each k As String In outs.Keys
				Try
					ret(k) = Nd4j.concat(0, CType(outs(k), List(Of INDArray)).ToArray())
				Catch e As Exception
					Throw New ND4JException("Error concatenating batch outputs", e)
				End Try
			Next k
			Return ret
		End Function

		''' <summary>
		''' Get a list of batch outputs for a single variable from a list of batch outputs for all variables
		''' </summary>
		Public Shared Function getSingleOutput(ByVal outputs As IList(Of IDictionary(Of String, INDArray)), ByVal output As String) As IList(Of INDArray)
			Dim batches As IList(Of INDArray) = New List(Of INDArray)()
			For Each batch As IDictionary(Of String, INDArray) In outputs
				batches.Add(batch(output))
			Next batch

			Return batches
		End Function

		Public Shared Function externalErrors(ByVal sameDiff As SameDiff, ByVal externalGradients As IDictionary(Of String, INDArray), ParamArray ByVal inputs() As SDVariable) As ExternalErrorsFunction
			Preconditions.checkArgument(inputs IsNot Nothing AndAlso inputs.Length > 0, "Require at least one SDVariable to" & " be specified when using external errors: got %s", inputs)
			Dim fn As New ExternalErrorsFunction(sameDiff, New List(Of SDVariable) From {inputs}, externalGradients)
			fn.outputVariable()
			Return fn
		End Function

		Public Shared Function externalErrors(ByVal sameDiff As SameDiff, ByVal inputs() As SDVariable) As ExternalErrorsFunction
			Return externalErrors(sameDiff, Nothing, inputs)
		End Function



		''' <summary>
		''' Add 1s as required to the array make an array possible to be broadcast with the original (pre-reduce) array.
		''' <para>
		''' Example: if doing [a,b,c].sum(1), result is [a,c]. To 'undo' this in a way that can be auto-broadcast,
		''' we want to expand as required - i.e., [a,c] -> [a,1,c] which can be auto-broadcast with the original [a,b,c].
		''' This is typically only used with reduction operations backprop.
		''' 
		''' </para>
		''' </summary>
		''' <param name="origRank">   Rank of the original array, before the reduction was executed </param>
		''' <param name="reduceDims"> Dimensions that the original array was reduced from </param>
		''' <param name="toExpand">   Array to add 1s to the shape to (such that it can be </param>
		''' <returns> Reshaped array. </returns>
		Public Shared Function reductionBroadcastableWithOrigShape(ByVal origRank As Integer, ByVal reduceDims() As Integer, ByVal toExpand As SDVariable) As SDVariable
			If Shape.isWholeArray(origRank, reduceDims) Then
				'Output is [1,1] which is already broadcastable
				Return toExpand
			ElseIf origRank = 2 AndAlso reduceDims.Length = 1 Then
				'In this case: [a,b] -> [1,b] or [a,b] -> [a,1]
				'both are already broadcastable
				Return toExpand
			Else
				'Example: [a,b,c].sum(1) -> [a,c]... want [a,1,c]
				For Each d As Integer In reduceDims
					toExpand = toExpand.getSameDiff().expandDims(toExpand, d)
				Next d
				Return toExpand
			End If
		End Function

		Public Shared Function reductionBroadcastableWithOrigShape(ByVal origInput As SDVariable, ByVal axis As SDVariable, ByVal toExpand As SDVariable) As SDVariable
			Dim shape As SDVariable = origInput.shape()
			Dim reduceShape As SDVariable = reductionShape(shape, axis, True)
			Dim reshaped As SDVariable = toExpand.reshape(reduceShape)
			Return reshaped
		End Function

		Public Shared Function reductionShape(ByVal shape As SDVariable, ByVal axis As SDVariable, ByVal keepDim As Boolean) As SDVariable
			Return (New ReductionShape(shape.getSameDiff(), shape, axis, keepDim)).outputVariable()
		End Function

		Public Shared Sub validateDifferentialFunctionSameDiff(ByVal sameDiff As SameDiff, ByVal [function] As SDVariable, ByVal op As DifferentialFunction)

			Preconditions.checkState([function] IsNot Nothing, "Passed in function was null.")
			Preconditions.checkState([function].getSameDiff() Is sameDiff)

			Preconditions.checkState([function].getSameDiff() Is sameDiff, "Function applications must be contained " & "in same sameDiff. The left %s must match this function %s", [function], op)
		End Sub
	End Class

End Namespace