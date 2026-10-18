Imports System
Imports System.Collections.Generic
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

Namespace org.nd4vb.linalg.api.ops.compression

    ''' <summary>The custom op <c>encode_threshold</c> (Java <c>org.nd4j.linalg.api.ops.compression.EncodeThreshold</c>).</summary>
    Public Class EncodeThreshold
        Inherits DynamicCustomOp

        Protected Friend threshold As Single = 1e-3f
        Protected Friend boundary As Integer = Integer.MaxValue

        Public Sub New()
            '
        End Sub

        Public Sub New(ByVal updates As INDArray, ByVal threshold As Single)
            Me.New(updates, threshold, Integer.MaxValue)
        End Sub

        Public Sub New(ByVal updates As INDArray, ByVal encoded As INDArray, ByVal threshold As Single, ByVal boundary As Integer)
            Me.New(updates, threshold, boundary)

            addOutputArgument(updates, encoded)
        End Sub

        Public Sub New(ByVal updates As INDArray, ByVal threshold As Single, ByVal boundary As Integer)
            addInputArgument(updates)

            addTArgument(threshold)
            addIArgument(boundary)

            Me.threshold = threshold
            Me.boundary = boundary
        End Sub

        Public Overrides Function opName() As String
            Return "encode_threshold"
        End Function

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Return New List(Of DataType) From {inputArguments(0).dataType(), DataType.INT32}
        End Function

    End Class

End Namespace
