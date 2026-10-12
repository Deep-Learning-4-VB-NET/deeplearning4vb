Imports System
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

Namespace org.deeplearning4j.rl4j.observation.transform.operation.historymerge

	Public Class HistoryStackAssembler
		Implements HistoryMergeAssembler

		''' <summary>
		''' Will return a new INDArray with one more dimension and with elements stacked along dimension 0.
		''' </summary>
		''' <param name="elements"> Array of INDArray </param>
		''' <returns> A new INDArray with 1 more dimension than the input elements </returns>
		Public Overridable Function assemble(ByVal elements() As INDArray) As INDArray Implements HistoryMergeAssembler.assemble
			' build the new shape
			Dim elementShape() As Long = elements(0).shape()
			Dim newShape(elementShape.Length) As Long
			newShape(0) = elements.Length
			Array.Copy(elementShape, 0, newShape, 1, elementShape.Length)

			' stack the elements in result on the dimension 0
			Dim result As INDArray = Nd4j.create(newShape)
			For i As Integer = 0 To elements.Length - 1
				result.putRow(i, elements(i))
			Next i
			Return result
		End Function
	End Class

End Namespace