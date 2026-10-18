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

    ''' <summary>The custom op <c>compat_sparse_to_dense</c> (Java <c>org.nd4j.linalg.api.ops.compat.CompatSparseToDense</c>).</summary>
    Public Class CompatSparseToDense
        Inherits DynamicCustomOp

        Public Sub New()
            '
        End Sub

        Public Sub New(ByVal indices As INDArray, ByVal shape As INDArray, ByVal values As INDArray)
            Preconditions.checkArgument(shape.Z AndAlso indices.Z, "Shape & indices arrays must have one integer data types")
            inputArgumentsField.Add(indices)
            inputArgumentsField.Add(shape)
            inputArgumentsField.Add(values)
        End Sub

        Public Sub New(ByVal indices As INDArray, ByVal shape As INDArray, ByVal values As INDArray, ByVal defaultVaule As INDArray)
            Me.New(indices, shape, values)
            Preconditions.checkArgument(defaultVaule.dataType() = values.dataType(), "Values array must have the same data type as defaultValue array")
            inputArgumentsField.Add(defaultVaule)
        End Sub

        Public Overrides Function opName() As String
            Return "compat_sparse_to_dense"
        End Function

    End Class

End Namespace
