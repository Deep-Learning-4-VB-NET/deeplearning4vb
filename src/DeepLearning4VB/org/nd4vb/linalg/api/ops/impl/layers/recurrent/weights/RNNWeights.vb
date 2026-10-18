Imports System
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports ArrayUtil = DeepLearning4VB.org.nd4vb.common.util.ArrayUtil
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
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

Namespace org.nd4vb.linalg.api.ops.impl.layers.recurrent.weights

    ''' <summary>Java <c>org.nd4j.linalg.api.ops.impl.layers.recurrent.weights.RNNWeights</c>.</summary>
    Public MustInherit Class RNNWeights

#If SAMEDIFF Then
		Public MustOverride Function args() As SDVariable()
#End If

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
                    [out](j) = args(i)
                        j += 1
                End If
            Next i
            Return [out]
        End Function

#If SAMEDIFF Then
		Public Overridable Function argsWithInputs(ParamArray ByVal inputs() As SDVariable) As SDVariable()
			Return ArrayUtil.combine(inputs, args())
		End Function
#End If

        Public Overridable Function argsWithInputs(ParamArray ByVal inputs() As INDArray) As INDArray()
            Return ArrayUtil.combine(inputs, arrayArgs())
        End Function

    End Class

End Namespace
