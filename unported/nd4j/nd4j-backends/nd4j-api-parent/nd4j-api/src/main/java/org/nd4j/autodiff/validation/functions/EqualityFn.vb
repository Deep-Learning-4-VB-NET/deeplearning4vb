Imports Microsoft.VisualBasic
Imports AllArgsConstructor = lombok.AllArgsConstructor
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
'ORIGINAL LINE: @AllArgsConstructor public class EqualityFn implements org.nd4j.common.function.@Function<org.nd4j.linalg.api.ndarray.INDArray,String>
	Public Class EqualityFn
		Implements [Function](Of INDArray, String)

		Private ReadOnly expected As INDArray

		Public Overridable Function apply(ByVal actual As INDArray) As String
			If expected.Equals(actual) Then
				Return Nothing
			End If
			Return "INDArray equality failed:" & vbLf & "Expected:" & vbLf & expected & vbLf & "Actual:" & vbLf & actual
		End Function
	End Class

End Namespace