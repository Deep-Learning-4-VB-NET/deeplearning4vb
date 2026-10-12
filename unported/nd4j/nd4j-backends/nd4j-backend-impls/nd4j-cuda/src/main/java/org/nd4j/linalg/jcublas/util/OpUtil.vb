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

Namespace org.nd4j.linalg.jcublas.util


	''' <summary>
	''' @author Adam Gibson
	''' </summary>
	Public Class OpUtil

		Private Sub New()
		End Sub

		''' <summary>
		''' Return op for the given character
		''' throws an @link{IllegalArgumentException}
		''' for any charcter != n t or c </summary>
		''' <param name="op"> the character to get the op for </param>
		''' <returns> the op for the given character </returns>
		Public Shared Function getOp(ByVal op As Char) As Integer
	'        
	'        op = Character.toLowerCase(op);
	'        switch(op) {
	'            case 'n': return cublasOperation.CUBLAS_OP_N;
	'            case 't' : return cublasOperation.CUBLAS_OP_T;
	'            case 'c' : return cublasOperation.CUBLAS_OP_C;
	'            default: throw new IllegalArgumentException("No op found");
	'        }
	'        
			Throw New System.NotSupportedException()
		End Function

	End Class

End Namespace