Imports System
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb

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

Namespace org.nd4vb.linalg.api.ops.custom

    ''' <summary>The custom op <c>barnes_symmetrized</c> (Java <c>org.nd4j.linalg.api.ops.custom.BarnesHutSymmetrize</c>).</summary>
    Public Class BarnesHutSymmetrize
        Inherits DynamicCustomOp

        Private output As INDArray
        Private outCols As INDArray

        Public Sub New()
        End Sub

        Public Sub New(ByVal rowP As INDArray, ByVal colP As INDArray, ByVal valP As INDArray, ByVal N As Long, ByVal outRows As INDArray)

            Dim rowCounts As INDArray = Nd4vbT.create(N)
            For n_Conflict As Integer = 0 To N - 1
                Dim begin As Integer = rowP.getInt(n_Conflict)
                Dim [end] As Integer = rowP.getInt(n_Conflict + 1)
                For i As Integer = begin To [end] - 1
                    Dim present As Boolean = False
                    Dim m As Integer = rowP.getInt(colP.getInt(i))
                    Do While m < rowP.getInt(colP.getInt(i) + 1)
                        If colP.getInt(m) = n_Conflict Then
                            present = True
                        End If
                        m += 1
                    Loop
                    If present Then
                        rowCounts.putScalar(n_Conflict, rowCounts.getDouble(n_Conflict) + 1)

                    Else
                        rowCounts.putScalar(n_Conflict, rowCounts.getDouble(n_Conflict) + 1)
                        rowCounts.putScalar(colP.getInt(i), rowCounts.getDouble(colP.getInt(i)) + 1)
                    End If
                Next i
            Next n_Conflict
            Dim outputCols As Integer = rowCounts.sum(Integer.MaxValue).getInt(0)
            output = Nd4vbT.create(1, outputCols)
            outCols = Nd4vbT.create(New Integer(){1, outputCols}, DataType.INT)

            inputArgumentsField.Add(rowP)
            inputArgumentsField.Add(colP)
            inputArgumentsField.Add(valP)

            outputArgumentsField.Add(outRows)
            outputArgumentsField.Add(outCols)
            outputArgumentsField.Add(output)

            iArguments.Add(N)
        End Sub

        Public Overridable ReadOnly Property SymmetrizedValues As INDArray
            Get
                Return output
            End Get
        End Property

        Public Overridable ReadOnly Property SymmetrizedCols As INDArray
            Get
                Return outCols
            End Get
        End Property

        Public Overrides Function opName() As String
            Return "barnes_symmetrized"
        End Function

    End Class

End Namespace
