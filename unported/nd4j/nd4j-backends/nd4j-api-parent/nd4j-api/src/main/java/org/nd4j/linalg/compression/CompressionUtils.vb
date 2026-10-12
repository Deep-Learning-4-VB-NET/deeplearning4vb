Imports NonNull = lombok.NonNull
Imports DataTypeEx = org.nd4j.linalg.api.buffer.DataTypeEx

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

Namespace org.nd4j.linalg.compression

	Public Class CompressionUtils

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static boolean goingToDecompress(@NonNull DataTypeEx from, @NonNull DataTypeEx to)
		Public Shared Function goingToDecompress(ByVal from As DataTypeEx, ByVal [to] As DataTypeEx) As Boolean
			' TODO: eventually we want FLOAT16 here
			If [to].Equals(DataTypeEx.FLOAT) OrElse [to].Equals(DataTypeEx.DOUBLE) Then
				Return True
			End If

			Return False
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static boolean goingToCompress(@NonNull DataTypeEx from, @NonNull DataTypeEx to)
		Public Shared Function goingToCompress(ByVal from As DataTypeEx, ByVal [to] As DataTypeEx) As Boolean
			If Not goingToDecompress(from, [to]) AndAlso goingToDecompress([to], from) Then
				Return True
			End If

			Return False
		End Function
	End Class

End Namespace