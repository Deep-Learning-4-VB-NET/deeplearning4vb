Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports DoubleWritable = org.datavec.api.writable.DoubleWritable
Imports NDArrayWritable = org.datavec.api.writable.NDArrayWritable
Imports Writable = org.datavec.api.writable.Writable
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.common.function

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

Namespace org.datavec.local.transforms.misc


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public class NDArrayToWritablesFunction implements org.nd4j.common.function.@Function<org.nd4j.linalg.api.ndarray.INDArray, java.util.List<org.datavec.api.writable.Writable>>
	Public Class NDArrayToWritablesFunction
		Implements [Function](Of INDArray, IList(Of Writable))

		Private useNdarrayWritable As Boolean = False

		Public Sub New()
			useNdarrayWritable = False
		End Sub

		Public Overridable Function apply(ByVal arr As INDArray) As IList(Of Writable)
			If arr.rows() <> 1 Then
				Throw New System.NotSupportedException("Only NDArray row vectors can be converted to list" & " of Writables (found " & arr.rows() & " rows)")
			End If
			Dim record As IList(Of Writable) = New List(Of Writable)()
			If useNdarrayWritable Then
				record.Add(New NDArrayWritable(arr))
			Else
				Dim i As Integer = 0
				Do While i < arr.columns()
					record.Add(New DoubleWritable(arr.getDouble(i)))
					i += 1
				Loop
			End If
			Return record
		End Function
	End Class

End Namespace