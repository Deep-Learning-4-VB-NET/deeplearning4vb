Imports System
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

    ''' <summary>The custom op <c>cell_contains</c> (Java <c>org.nd4j.linalg.api.ops.custom.SpTreeCell</c>).</summary>
    Public Class SpTreeCell
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

        Public Sub New(ByVal corner As INDArray, ByVal width As INDArray, ByVal point As INDArray, ByVal N As Long, ByVal contains As Boolean)
            inputArgumentsField.Add(corner)
            inputArgumentsField.Add(width)
            inputArgumentsField.Add(point)

            iArguments.Add(N)

            outputArgumentsField.Add(Nd4vbT.scalar(contains))
        End Sub

        Public Overrides Function opName() As String
            Return "cell_contains"
        End Function

    End Class

End Namespace
