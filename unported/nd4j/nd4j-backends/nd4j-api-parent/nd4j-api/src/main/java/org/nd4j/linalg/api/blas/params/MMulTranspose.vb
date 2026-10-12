Imports System
Imports System.Collections.Generic
Imports Builder = lombok.Builder
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Getter = lombok.Getter
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

Namespace org.nd4j.linalg.api.blas.params


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @EqualsAndHashCode public class MMulTranspose implements java.io.Serializable
	<Serializable>
	Public Class MMulTranspose
'JAVA TO VB CONVERTER NOTE: The field allFalse was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared allFalse_Conflict As MMulTranspose = MMulTranspose.builder().build()
		Private transposeA As Boolean
		Private transposeB As Boolean
		Private transposeResult As Boolean


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder public MMulTranspose(boolean transposeA, boolean transposeB, boolean transposeResult)
		Public Sub New(ByVal transposeA As Boolean, ByVal transposeB As Boolean, ByVal transposeResult As Boolean)
			Me.transposeA = transposeA
			Me.transposeB = transposeB
			Me.transposeResult = transposeResult
		End Sub

		''' <summary>
		''' Returns the default transpose
		''' where all are false
		''' 
		''' @return
		''' </summary>
		Public Shared Function allFalse() As MMulTranspose
			Return allFalse_Conflict
		End Function

		''' <summary>
		''' Execute the matrix multiplication: A x B
		''' Note that if a or b have transposeA/B == true, then this is done internally.
		''' Also, if transposeResult == true, then this is also done internally - i.e., the result array - if present -
		''' should not be transposed beforehand. </summary>
		''' <param name="a">      A array </param>
		''' <param name="b">      B array </param>
		''' <param name="result"> Result array (pre resultArrayTranspose if required). May be null. </param>
		''' <returns> Result array </returns>
		Public Overridable Function exec(ByVal a As INDArray, ByVal b As INDArray, ByVal result As INDArray) As INDArray
			a = transposeIfReq(transposeA, a)
			b = transposeIfReq(transposeB, b)
			If result Is Nothing Then
				Dim ret As INDArray = a.mmul(b)
				Return transposeIfReq(transposeResult, ret)
			Else

				If Not transposeResult Then
					Return a.mmuli(b, result)
				Else
					Return a.mmuli(b, result).transpose()
				End If
			End If
		End Function

		Private Shared Function transposeIfReq(ByVal transpose As Boolean, ByVal x As INDArray) As INDArray
			If transpose Then
				If x.rank() = 2 Then
					Return x.transpose()
				End If
				If x.rank() = 3 Then
					Return x.permute(0, 2, 1)
				End If
			End If
			Return x
		End Function

		Public Overridable Function getValue(ByVal [property] As System.Reflection.FieldInfo) As Object
			Try
				Return [property].get(Me)
			Catch e As IllegalAccessException
				Throw New Exception(e)
			End Try
		End Function

		Public Overridable Function toProperties() As IDictionary(Of String, Object)
			Dim ret As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			ret("transposeA") = transposeA
			ret("transposeB") = transposeB
			ret("transposeResult") = transposeResult
			Return ret
		End Function

		Public Overridable WriteOnly Property Properties As IDictionary(Of String, Object)
			Set(ByVal properties As IDictionary(Of String, Object))
				If properties.ContainsKey("transposeA") Then
					transposeA = DirectCast(properties("transposeA"), Boolean?)
				End If
				If properties.ContainsKey("transposeB") Then
					transposeB = DirectCast(properties("transposeB"), Boolean?)
				End If
				If properties.ContainsKey("transposeResult") Then
					transposeResult = DirectCast(properties("transposeResult"), Boolean?)
				End If
			End Set
		End Property
	End Class

End Namespace