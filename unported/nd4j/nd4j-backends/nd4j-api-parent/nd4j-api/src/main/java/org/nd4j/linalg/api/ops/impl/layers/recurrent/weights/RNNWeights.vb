Imports System
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil

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

Namespace org.nd4j.linalg.api.ops.impl.layers.recurrent.weights


	Public MustInherit Class RNNWeights
		Public MustOverride Function args() As SDVariable()

		Public MustOverride Function arrayArgs() As INDArray()

		Protected Friend Shared Function filterNonNull(Of T)(ParamArray ByVal args() As T) As T()
			Dim count As Integer = 0
			For i As Integer = 0 To args.Length - 1
				If args(i) IsNot Nothing Then
					count += 1
				End If
			Next i
			Dim [out]() As T = CType(Array.CreateInstance(args.GetType().GetElementType(), count), T())
			Dim j As Integer=0
			For i As Integer = 0 To args.Length - 1
				If args(i) IsNot Nothing Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: out[j++] = args[i];
					[out](j) = args(i)
						j += 1
				End If
			Next i
			Return [out]
		End Function

		Public Overridable Function argsWithInputs(ParamArray ByVal inputs() As SDVariable) As SDVariable()
			Return ArrayUtil.combine(inputs, args())
		End Function

		Public Overridable Function argsWithInputs(ParamArray ByVal inputs() As INDArray) As INDArray()
			Return ArrayUtil.combine(inputs, arrayArgs())
		End Function


	End Class

End Namespace