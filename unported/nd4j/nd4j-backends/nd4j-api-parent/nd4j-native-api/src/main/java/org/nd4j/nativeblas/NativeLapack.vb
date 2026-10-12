Imports System.Runtime.InteropServices

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

Namespace org.nd4j.nativeblas


	Public Class NativeLapack

		Public Sub New()
		End Sub
		' LU decomoposition of a general matrix

		''' <summary>
		''' LU decomposiiton of a matrix </summary>
		''' <param name="M"> </param>
		''' <param name="N"> </param>
		''' <param name="A"> </param>
		''' <param name="lda"> </param>
		''' <param name="IPIV"> </param>
		''' <param name="INFO"> </param>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub dgetrf(ByVal extraPointers() As Long, ByVal M As Integer, ByVal N As Integer, ByVal A As Long, ByVal lda As Integer, ByVal IPIV() As Integer, ByVal INFO As Integer)
		End Sub

		' generate inverse of a matrix given its LU decomposition

		''' <summary>
		''' Generate inverse ggiven LU decomp </summary>
		''' <param name="N"> </param>
		''' <param name="A"> </param>
		''' <param name="lda"> </param>
		''' <param name="IPIV"> </param>
		''' <param name="WORK"> </param>
		''' <param name="lwork"> </param>
		''' <param name="INFO"> </param>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub dgetri(ByVal extraPointers() As Long, ByVal N As Integer, ByVal A As Long, ByVal lda As Integer, ByVal IPIV() As Integer, ByVal WORK As Long, ByVal lwork As Integer, ByVal INFO As Integer)
		End Sub

		' LU decomoposition of a general matrix

		''' <summary>
		''' LU decomposiiton of a matrix </summary>
		''' <param name="M"> </param>
		''' <param name="N"> </param>
		''' <param name="A"> </param>
		''' <param name="lda"> </param>
		''' <param name="IPIV"> </param>
		''' <param name="INFO"> </param>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub sgetrf(ByVal extraPointers() As Long, ByVal M As Integer, ByVal N As Integer, ByVal A As Long, ByVal lda As Integer, ByVal IPIV() As Integer, ByVal INFO As Integer)
		End Sub

		' generate inverse of a matrix given its LU decomposition

		''' <summary>
		''' Generate inverse ggiven LU decomp </summary>
		''' <param name="N"> </param>
		''' <param name="A"> </param>
		''' <param name="lda"> </param>
		''' <param name="IPIV"> </param>
		''' <param name="WORK"> </param>
		''' <param name="lwork"> </param>
		''' <param name="INFO"> </param>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub sgetri(ByVal extraPointers() As Long, ByVal N As Integer, ByVal A As Long, ByVal lda As Integer, ByVal IPIV() As Integer, ByVal WORK As Long, ByVal lwork As Integer, ByVal INFO As Integer)
		End Sub
	End Class

End Namespace