Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports org.nd4j.adapters
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

Namespace org.deeplearning4j.nn.adapters

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class Regression2dAdapter implements org.nd4j.adapters.OutputAdapter<double[][]>
	<Serializable>
	Public Class Regression2dAdapter
		Implements OutputAdapter(Of Double()())

		Public Overridable Function apply(ParamArray ByVal outputs() As INDArray) As Double()()
			Preconditions.checkArgument(outputs.Length = 1, "Argmax adapter can have only 1 output")
			Dim array As val = outputs(0)
			Preconditions.checkArgument(array.rank() < 3, "Argmax adapter requires 2D or 1D output")

			If array.rank() = 2 AndAlso Not array.isVector() Then
				Return array.toDoubleMatrix()
			Else
				Dim result As val = { New Double(CInt(array.length()) - 1){} }

				For e As Integer = 0 To array.length() - 1
					result(0)(e) = array.getDouble(e)
				Next e

				Return result
			End If
		End Function
	End Class

End Namespace