Imports System
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
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

Namespace org.nd4vb.linalg.api.ops.util

    ''' <summary>The custom op <c>print_variable</c> (Java <c>org.nd4j.linalg.api.ops.util.PrintVariable</c>).</summary>
    Public Class PrintVariable
        Inherits DynamicCustomOp

        Public Sub New()
            '
        End Sub

        Public Sub New(ByVal array As INDArray, ByVal printSpecial As Boolean)
            inputArgumentsField.Add(array)
            bArguments.Add(printSpecial)
        End Sub

        Public Sub New(ByVal array As INDArray)
            Me.New(array, False)
        End Sub

        Public Sub New(ByVal array As INDArray, ByVal message As String, ByVal printSpecial As Boolean)
            Me.New(array, Nd4vbT.create(message), printSpecial)
        End Sub

        Public Sub New(ByVal array As INDArray, ByVal message As String)
            Me.New(array, Nd4vbT.create(message), False)
        End Sub

        Public Sub New(ByVal array As INDArray, ByVal message As INDArray, ByVal printSpecial As Boolean)
            Me.New(array, printSpecial)
            Preconditions.checkArgument(message.S, "Message argument should have String data type, but got [" & message.dataType() & "] instead")
            inputArgumentsField.Add(message)
        End Sub

        Public Sub New(ByVal array As INDArray, ByVal message As INDArray)
            Me.New(array, message, False)
        End Sub

        Public Overrides Function opName() As String
            Return "print_variable"
        End Function

    End Class

End Namespace
