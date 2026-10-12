Imports System.IO
Imports Base64 = org.apache.commons.net.util.Base64
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

Namespace org.nd4j.serde.base64


	''' <summary>
	''' NDArray as base 64
	''' 
	''' @author Adam Gibson
	''' </summary>
	Public Class Nd4jBase64

		Private Sub New()
		End Sub

		''' <summary>
		''' Returns an ndarray
		''' as base 64 </summary>
		''' <param name="arr"> the array to write </param>
		''' <returns> the base 64 representation of the binary
		''' ndarray </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static String base64String(org.nd4j.linalg.api.ndarray.INDArray arr) throws IOException
		Public Shared Function base64String(ByVal arr As INDArray) As String
			Dim bos As New MemoryStream()
			Dim dos As New DataOutputStream(bos)
			Nd4j.write(arr, dos)
			Return Base64.encodeBase64String(bos.toByteArray())
		End Function

		''' <summary>
		''' Create an ndarray from a base 64
		''' representation </summary>
		''' <param name="base64"> the base 64 to convert </param>
		''' <returns> the ndarray from base 64 </returns>
		Public Shared Function fromBase64(ByVal base64 As String) As INDArray
			Dim arr() As SByte = Base64.decodeBase64(base64)
			Dim bis As New MemoryStream(arr)
			Dim dis As New DataInputStream(bis)
			Return Nd4j.read(dis)
		End Function
	End Class

End Namespace