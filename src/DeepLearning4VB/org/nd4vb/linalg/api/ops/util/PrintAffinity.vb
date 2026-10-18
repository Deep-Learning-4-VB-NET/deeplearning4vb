Imports System
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

Namespace org.nd4vb.linalg.api.ops.util

    ''' <summary>The custom op <c>print_affinity</c> (Java <c>org.nd4j.linalg.api.ops.util.PrintAffinity</c>).</summary>
    Public Class PrintAffinity
        Inherits DynamicCustomOp

        Public Sub New()
            '
        End Sub

        Public Sub New(ByVal array As INDArray)
            inputArgumentsField.Add(array)
        End Sub

        Public Overrides Function opName() As String
            Return "print_affinity"
        End Function

    End Class

End Namespace
