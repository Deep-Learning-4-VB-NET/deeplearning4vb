Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Nd4jBlas = org.nd4j.nativeblas.Nd4jBlas
Imports org.bytedeco.openblas.global.openblas_nolapack

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

Namespace org.nd4j.linalg.cpu.nativecpu.blas

	''' <summary>
	''' Implementation of Nd4jBlas with OpenBLAS/MKL
	''' 
	''' @author saudet
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class CpuBlas extends org.nd4j.nativeblas.Nd4jBlas
	Public Class CpuBlas
		Inherits Nd4jBlas

		''' <summary>
		''' Converts a character
		''' to its proper enum
		''' for row (c) or column (f) ordering
		''' default is row major
		''' </summary>
		Friend Shared Function convertOrder(ByVal from As Integer) As Integer
			Select Case from
				Case "c"c, "C"c
					Return CblasRowMajor
				Case "f"c, "F"c
					Return CblasColMajor
				Case Else
					Return CblasColMajor
			End Select
		End Function

		''' <summary>
		''' Converts a character to its proper enum
		''' t -> transpose
		''' n -> no transpose
		''' c -> conj
		''' </summary>
		Friend Shared Function convertTranspose(ByVal from As Integer) As Integer
			Select Case from
				Case "t"c, "T"c
					Return CblasTrans
				Case "n"c, "N"c
					Return CblasNoTrans
				Case "c"c, "C"c
					Return CblasConjTrans
				Case Else
					Return CblasNoTrans
			End Select
		End Function

		''' <summary>
		''' Upper or lower
		''' U/u -> upper
		''' L/l -> lower
		''' 
		''' Default is upper
		''' </summary>
		Friend Shared Function convertUplo(ByVal from As Integer) As Integer
			Select Case from
				Case "u"c, "U"c
					Return CblasUpper
				Case "l"c, "L"c
					Return CblasLower
				Case Else
					Return CblasUpper
			End Select
		End Function


		''' <summary>
		''' For diagonals:
		''' u/U -> unit
		''' n/N -> non unit
		''' 
		''' Default: unit
		''' </summary>
		Friend Shared Function convertDiag(ByVal from As Integer) As Integer
			Select Case from
				Case "u"c, "U"c
					Return CblasUnit
				Case "n"c, "N"c
					Return CblasNonUnit
				Case Else
					Return CblasUnit
			End Select
		End Function

		''' <summary>
		''' Side of a matrix, left or right
		''' l /L -> left
		''' r/R -> right
		''' default: left
		''' </summary>
		Friend Shared Function convertSide(ByVal from As Integer) As Integer
			Select Case from
				Case "l"c, "L"c
					Return CblasLeft
				Case "r"c, "R"c
					Return CblasRight
				Case Else
					Return CblasLeft
			End Select
		End Function

		Public Overrides Property MaxThreads As Integer
			Set(ByVal num As Integer)
				blas_set_num_threads(num)
			End Set
			Get
				Return blas_get_num_threads()
			End Get
		End Property


		Public Overrides ReadOnly Property BlasVendorId As Integer
			Get
				Return blas_get_vendor()
			End Get
		End Property
	End Class

End Namespace