Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
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

Namespace org.nd4j.linalg.api.ops.custom

	Public Class BarnesHutSymmetrize
		Inherits DynamicCustomOp

		Private output As INDArray
		Private outCols As INDArray

		Public Sub New()
		End Sub

		Public Sub New(ByVal rowP As INDArray, ByVal colP As INDArray, ByVal valP As INDArray, ByVal N As Long, ByVal outRows As INDArray)

			Dim rowCounts As INDArray = Nd4j.create(N)
'JAVA TO VB CONVERTER NOTE: The variable n was renamed since Visual Basic will not allow local variables with the same name as parameters or other local variables:
			For n_Conflict As Integer = 0 To N - 1
				Dim begin As Integer = rowP.getInt(n_Conflict)
				Dim [end] As Integer = rowP.getInt(n_Conflict + 1)
				For i As Integer = begin To [end] - 1
					Dim present As Boolean = False
					Dim m As Integer = rowP.getInt(colP.getInt(i))
					Do While m < rowP.getInt(colP.getInt(i) + 1)
						If colP.getInt(m) = n_Conflict Then
							present = True
						End If
						m += 1
					Loop
					If present Then
						rowCounts.putScalar(n_Conflict, rowCounts.getDouble(n_Conflict) + 1)

					Else
						rowCounts.putScalar(n_Conflict, rowCounts.getDouble(n_Conflict) + 1)
						rowCounts.putScalar(colP.getInt(i), rowCounts.getDouble(colP.getInt(i)) + 1)
					End If
				Next i
			Next n_Conflict
			Dim outputCols As Integer = rowCounts.sum(Integer.MaxValue).getInt(0)
			output = Nd4j.create(1, outputCols)
			outCols = Nd4j.create(New Integer(){1, outputCols}, DataType.INT)

			inputArguments_Conflict.Add(rowP)
			inputArguments_Conflict.Add(colP)
			inputArguments_Conflict.Add(valP)

			outputArguments_Conflict.Add(outRows)
			outputArguments_Conflict.Add(outCols)
			outputArguments_Conflict.Add(output)

			iArguments.Add(N)
		End Sub

		Public Overridable ReadOnly Property SymmetrizedValues As INDArray
			Get
				Return output
			End Get
		End Property

		Public Overridable ReadOnly Property SymmetrizedCols As INDArray
			Get
				Return outCols
			End Get
		End Property

		Public Overrides Function opName() As String
			Return "barnes_symmetrized"
		End Function
	End Class

End Namespace