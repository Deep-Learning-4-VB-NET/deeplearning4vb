Imports System
Imports System.Linq
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports NdIndexIterator = org.nd4j.linalg.api.iter.NdIndexIterator
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

Namespace org.nd4j.autodiff.validation.functions


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data public class RelErrorFn implements org.nd4j.common.function.@Function<org.nd4j.linalg.api.ndarray.INDArray,String>
	Public Class RelErrorFn
		Implements [Function](Of INDArray, String)

		Private ReadOnly expected As INDArray
		Private ReadOnly maxRelativeError As Double
		Private ReadOnly minAbsoluteError As Double


		Public Overridable Function apply(ByVal actual As INDArray) As String
			'TODO switch to binary relative error ops
			If Not expected.shape().SequenceEqual(actual.shape()) Then
				Throw New System.InvalidOperationException("Shapes differ! " & Arrays.toString(expected.shape()) & " vs " & Arrays.toString(actual.shape()))
			End If

			Dim iter As New NdIndexIterator(expected.shape())
			Do While iter.MoveNext()
				Dim [next]() As Long = iter.Current
				Dim d1 As Double = expected.getDouble([next])
				Dim d2 As Double = actual.getDouble([next])
				If d1 = 0.0 AndAlso d2 = 0 Then
					Continue Do
				End If
				If Math.Abs(d1-d2) < minAbsoluteError Then
					Continue Do
				End If
				Dim re As Double = Math.Abs(d1-d2) / (Math.Abs(d1) + Math.Abs(d2))
				If re > maxRelativeError Then
					Return "Failed on relative error at position " & Arrays.toString([next]) & ": relativeError=" & re & ", maxRE=" & maxRelativeError & ", absError=" & Math.Abs(d1-d2) & ", minAbsError=" & minAbsoluteError & " - values (" & d1 & "," & d2 & ")"
				End If
			Loop
			Return Nothing
		End Function
	End Class

End Namespace