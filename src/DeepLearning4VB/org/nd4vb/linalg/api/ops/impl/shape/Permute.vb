Imports System
Imports System.Collections.Generic
Imports NoOpNameFoundException = DeepLearning4VB.org.nd4vb.imports.NoOpNameFoundException
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
#End If

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

Namespace org.nd4vb.linalg.api.ops.impl.shape

    ''' <summary>Java <c>org.nd4j.linalg.api.ops.impl.shape.Permute</c>.</summary>
    Public Class Permute
        Inherits Transpose

        Private reverseDims() As Integer

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ParamArray ByVal permuteDims() As Integer)
			MyBase.New(sameDiff, i_v)
			Me.permuteDims = permuteDims
			Me.reverseDims = New Integer(permuteDims.Length - 1){}
			For i As Integer = 0 To reverseDims.Length - 1
				reverseDims(i) = ArrayUtils.IndexOf(permuteDims, i)
			Next i
			addIArgument(permuteDims)
		End Sub
#End If

        Public Sub New(ByVal input As INDArray, ByVal result As INDArray, ParamArray ByVal permuteDims() As Integer)
            MyBase.New(input, result)
            Me.permuteDims = permuteDims
            Me.reverseDims = New Integer(permuteDims.Length - 1){}
            For i As Integer = 0 To reverseDims.Length - 1
                reverseDims(i) = Array.IndexOf(permuteDims, i)
            Next i
            addIArgument(permuteDims)
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sd As SameDiff, ByVal input As SDVariable, ByVal permuteDims As SDVariable)
			MyBase.New(sd, input, permuteDims)
		End Sub
#End If

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

#If SAMEDIFF Then
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
#End If

        Public Overrides Function tensorflowName() As String
            Throw New NoOpNameFoundException("No tensorflow op opName found for " & opName())
        End Function

        Public Overrides Function onnxName() As String
            Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
        End Function

    End Class

End Namespace
