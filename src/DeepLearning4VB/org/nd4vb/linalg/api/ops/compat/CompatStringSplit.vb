Imports System
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
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

Namespace org.nd4vb.linalg.api.ops.compat

    ''' <summary>The custom op <c>compat_string_split</c> (Java <c>org.nd4j.linalg.api.ops.compat.CompatStringSplit</c>).</summary>
    Public Class CompatStringSplit
        Inherits DynamicCustomOp

        Public Sub New()
            '
        End Sub

        Public Sub New(ByVal strings As INDArray, ByVal delimiter As INDArray)
            Preconditions.checkArgument(strings.S AndAlso delimiter.S, "Input arrays must have one of UTF types")
            inputArgumentsField.Add(strings)
            inputArgumentsField.Add(delimiter)
        End Sub

        Public Sub New(ByVal strings As INDArray, ByVal delimiter As INDArray, ByVal indices As INDArray, ByVal values As INDArray)
            Me.New(strings, delimiter)

            outputArgumentsField.Add(indices)
            outputArgumentsField.Add(values)
        End Sub

        Public Overrides Function opName() As String
            Return "compat_string_split"
        End Function

    End Class

End Namespace
