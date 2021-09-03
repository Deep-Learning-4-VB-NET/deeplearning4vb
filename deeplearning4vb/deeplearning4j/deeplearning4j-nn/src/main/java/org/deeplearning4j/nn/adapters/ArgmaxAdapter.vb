Imports System
Imports val = lombok.val
Imports org.nd4j.adapters
Imports Preconditions = org.nd4j.common.base.Preconditions
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

Namespace org.deeplearning4j.nn.adapters

	<Serializable>
	Public Class ArgmaxAdapter
		Implements OutputAdapter(Of Integer())

		''' <summary>
		''' This method does conversion from INDArrays to int[], where each element will represents position of the highest element in output INDArray
		''' I.e. Array of {0.25, 0.1, 0.5, 0.15} will return int array with length of 1, and value {2}
		''' </summary>
		''' <param name="outputs">
		''' @return </param>
		Public Overridable Function apply(ParamArray ByVal outputs() As INDArray) As Integer()
			Preconditions.checkArgument(outputs.Length = 1, "Argmax adapter can have only 1 output")
			Dim array As val = outputs(0)
			Preconditions.checkArgument(array.rank() < 3, "Argmax adapter requires 2D or 1D output")
			Dim result As val = If(array.rank() = 2, New Integer(CInt(array.size(0)) - 1){}, New Integer(0){})

			If array.rank() = 2 Then
				Dim t As val = Nd4j.argMax(array, 1)
				For e As Integer = 0 To t.length() - 1
					result(e) = CInt(Math.Truncate(t.getDouble(e)))
				Next e
			Else
				result(0) = CInt(Math.Truncate(Nd4j.argMax(array, Integer.MaxValue).getDouble(0)))
			End If

			Return result
		End Function
	End Class

End Namespace