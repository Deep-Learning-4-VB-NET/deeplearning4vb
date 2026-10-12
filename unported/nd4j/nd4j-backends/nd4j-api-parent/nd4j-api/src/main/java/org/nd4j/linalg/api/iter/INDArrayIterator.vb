Imports System.Collections.Generic
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Shape = org.nd4j.linalg.api.shape.Shape

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


	''' <summary>
	''' @author Adam Gibson
	''' </summary>
	Public Class INDArrayIterator
		Implements IEnumerator(Of Double)

		Private iterateOver As INDArray
		Private i As Integer = 0


		''' 
		''' <param name="iterateOver"> </param>
		Public Sub New(ByVal iterateOver As INDArray)
			Me.iterateOver = iterateOver
		End Sub

		Public Overrides Function hasNext() As Boolean
			Return i < iterateOver.length()
		End Function

		Public Overrides Sub remove()

		End Sub

		Public Overrides Function [next]() As Double?
'JAVA TO VB CONVERTER TODO TASK: The following line contains an assignment within expression that was not extracted by Java to VB Converter:
'ORIGINAL LINE: return iterateOver.getDouble(iterateOver.ordering() == "c"c ? org.nd4j.linalg.api.shape.Shape.ind2subC(iterateOver, i++) : org.nd4j.linalg.api.shape.Shape.ind2sub(iterateOver, i++));
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
			Dim tempVar = iterateOver.getDouble(If(iterateOver.ordering() = "c"c, Shape.ind2subC(iterateOver, i++), Shape.ind2sub(iterateOver, i)))
				i += 1
				Return tempVar
		End Function
	End Class

End Namespace