Imports System.Collections.Generic
Imports System.Numerics

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

Namespace org.nd4j.common.util

	''' 
	''' <summary>
	''' @author dmtrl
	''' </summary>


	Friend Class Factorial

		''' <summary>
		''' The list of all factorials as a vector.
		''' </summary>
		Friend Shared a As IList(Of BigInteger) = New List(Of BigInteger)()

		''' <summary>
		''' ctor().
		''' Initialize the vector of the factorials with 0!=1 and 1!=1.
		''' </summary>
		Public Sub New()
			If a.Count = 0 Then
				a.Add(BigInteger.One)
				a.Add(BigInteger.One)
			End If
		End Sub

		''' <summary>
		''' Compute the factorial of the non-negative integer.
		''' </summary>
		''' <param name="n"> the argument to the factorial, non-negative. </param>
		''' <returns> the factorial of n. </returns>
		Public Overridable Function at(ByVal n As Integer) As BigInteger
			Do While a.Count <= n
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int lastn = a.size() - 1;
				Dim lastn As Integer = a.Count - 1
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.math.BigInteger nextn = java.math.BigInteger.valueOf(lastn + 1);
				Dim nextn As BigInteger = BigInteger.valueOf(lastn + 1)
				a.Add(a(lastn) * nextn)
			Loop
			Return a(n)
		End Function
	End Class ' Factorial

End Namespace