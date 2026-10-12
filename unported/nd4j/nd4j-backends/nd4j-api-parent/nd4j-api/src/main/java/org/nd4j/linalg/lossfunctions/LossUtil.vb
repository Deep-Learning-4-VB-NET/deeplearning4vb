Imports System.Linq
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

Namespace org.nd4j.linalg.lossfunctions


	Public Class LossUtil

		''' 
		''' <param name="to"> </param>
		''' <param name="mask">
		''' @return </param>
		Public Shared Function isPerOutputMasking(ByVal [to] As INDArray, ByVal mask As INDArray) As Boolean
			Return Not mask.ColumnVector OrElse [to].shape().SequenceEqual(mask.shape())
		End Function

		''' 
		''' <param name="to"> </param>
		''' <param name="mask"> </param>
		Public Shared Sub applyMask(ByVal [to] As INDArray, ByVal mask As INDArray)
			'Two possibilities exist: it's *per example* masking, or it's *per output* masking
			'These cases have different mask shapes. Per example: column vector. Per output: same shape as score array
			If mask.ColumnVectorOrScalar Then
				[to].muliColumnVector(mask.castTo([to].dataType()))
			ElseIf [to].shape().SequenceEqual(mask.shape()) Then
				[to].muli(mask.castTo([to].dataType()))
			Else
				Throw New System.InvalidOperationException("Invalid mask array: per-example masking should be a column vector, " & "per output masking arrays should be the same shape as the labels array. Mask shape: " & Arrays.toString(mask.shape()) & ", output shape: " & Arrays.toString([to].shape()))
			End If
		End Sub
	End Class

End Namespace