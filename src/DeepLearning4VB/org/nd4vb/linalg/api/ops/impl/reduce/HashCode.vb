Imports System
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp

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

Namespace org.nd4vb.linalg.api.ops.impl.reduce

    ''' <summary>The custom op <c>hashcode</c> (Java <c>org.nd4j.linalg.api.ops.impl.reduce.HashCode</c>).</summary>
    Public Class HashCode
        Inherits DynamicCustomOp

        Public Sub New()
            '
        End Sub

        Public Sub New(ByVal array As INDArray)
            Me.inputArgumentsField.Add(array)
        End Sub

        Public Sub New(ByVal array As INDArray, ByVal result As INDArray)
            Me.New(array)
            Preconditions.checkArgument(result.dataType() = DataType.LONG AndAlso result.Scalar, "HashCode op expects LONG scalar as output")

            Me.outputArgumentsField.Add(result)
        End Sub

        Public Overrides Function opName() As String
            Return "hashcode"
        End Function

    End Class

End Namespace
