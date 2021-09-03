Imports System.Collections.Generic
Imports ArrayUtils = org.apache.commons.lang3.ArrayUtils
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
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

Namespace org.nd4j.linalg.api.ops.impl.shape


	Public Class Permute
		Inherits Transpose

		Private reverseDims() As Integer

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ParamArray ByVal permuteDims() As Integer)
			MyBase.New(sameDiff, i_v)
			Me.permuteDims = permuteDims
			Me.reverseDims = New Integer(permuteDims.Length - 1){}
			For i As Integer = 0 To reverseDims.Length - 1
				reverseDims(i) = ArrayUtils.IndexOf(permuteDims, i)
			Next i
			addIArgument(permuteDims)
		End Sub

		Public Sub New(ByVal input As INDArray, ByVal result As INDArray, ParamArray ByVal permuteDims() As Integer)
			MyBase.New(input, result)
			Me.permuteDims = permuteDims
			Me.reverseDims = New Integer(permuteDims.Length - 1){}
			For i As Integer = 0 To reverseDims.Length - 1
				reverseDims(i) = ArrayUtils.IndexOf(permuteDims, i)
			Next i
			addIArgument(permuteDims)
		End Sub

		Public Sub New(ByVal sd As SameDiff, ByVal input As SDVariable, ByVal permuteDims As SDVariable)
			MyBase.New(sd, input, permuteDims)
		End Sub

		Public Sub New(ByVal input As INDArray, ParamArray ByVal permuteDims() As Integer)
			MyBase.New(input, Nothing)
			Me.permuteDims = permuteDims
			addIArgument(permuteDims)
		End Sub

		Public Sub New()
		End Sub

		Public Overrides Function opName() As String
			Return "permute"
		End Function

		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Dim ret As SDVariable
			If args().Length = 1 Then
				'Static dimensions
				ret = sameDiff.permute(i_v(0), reverseDims)
			Else
				'Dynamic dimensions
				ret = sameDiff.permute(i_v(0), sameDiff.invertPermutation(arg(1)))
			End If
			Return Collections.singletonList(ret)
		End Function

		Public Overrides Function tensorflowName() As String
			Throw New NoOpNameFoundException("No tensorflow op opName found for " & opName())
		End Function

		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
		End Function
	End Class

End Namespace