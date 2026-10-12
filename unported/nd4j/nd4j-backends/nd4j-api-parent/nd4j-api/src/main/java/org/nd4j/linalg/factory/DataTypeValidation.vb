Imports val = lombok.val
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
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

Namespace org.nd4j.linalg.factory

	''' <summary>
	''' Data opType validation
	''' 
	''' @author Adam Gibson
	''' </summary>
	Public Class DataTypeValidation
		Public Shared Sub assertDouble(ParamArray ByVal d() As INDArray)
			For Each d1 As INDArray In d
				assertDouble(d1)
			Next d1
		End Sub

		Public Shared Sub assertFloat(ParamArray ByVal d2() As INDArray)
			For Each d3 As INDArray In d2
				assertFloat(d3)
			Next d3
		End Sub

		Public Shared Sub assertDouble(ByVal d As INDArray)
			If d.data().dataType() <> DataType.DOUBLE Then
				Throw New System.InvalidOperationException("Given ndarray does not have data opType double")
			End If
		End Sub

		Public Shared Sub assertFloat(ByVal d2 As INDArray)
			If d2.data().dataType() <> DataType.FLOAT Then
				Throw New System.InvalidOperationException("Given ndarray does not have data opType float")
			End If
		End Sub

		Public Shared Sub assertSameDataType(ParamArray ByVal indArrays() As INDArray)
			If indArrays Is Nothing OrElse indArrays.Length < 2 Then
				Return
			End If
			Dim type As DataType = indArrays(0).data().dataType()
			For i As Integer = 1 To indArrays.Length - 1
				Dim t As val = indArrays(i).data().dataType()
				Preconditions.checkState(t = type, "Data types must be same: got %s and %s", type, t)
			Next i
		End Sub
	End Class

End Namespace