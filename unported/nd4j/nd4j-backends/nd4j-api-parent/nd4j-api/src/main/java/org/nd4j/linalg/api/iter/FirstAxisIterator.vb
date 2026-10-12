Imports System.Collections.Generic
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

Namespace org.nd4j.linalg.api.iter


	Public Class FirstAxisIterator
		Implements IEnumerator(Of Object)

		Private iterateOver As INDArray
		Private i As Integer = 0


		''' 
		''' <param name="iterateOver"> </param>
		Public Sub New(ByVal iterateOver As INDArray)
			Me.iterateOver = iterateOver
		End Sub

		Public Overrides Function hasNext() As Boolean
			Return i < iterateOver.slices()
		End Function

		Public Overrides Sub remove()

		End Sub

		Public Overrides Function [next]() As Object
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: org.nd4j.linalg.api.ndarray.INDArray s = iterateOver.slice(i++);
			Dim s As INDArray = iterateOver.slice(i)
				i += 1
			If s.Scalar Then
				Return s.getDouble(0)
			Else
				Return s
			End If
		End Function

	End Class

End Namespace