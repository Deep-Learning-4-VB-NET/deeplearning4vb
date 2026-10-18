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

Namespace org.nd4vb.linalg.api.ops.impl.transforms

    ''' <summary>The custom op <c>histogram</c> (Java <c>org.nd4j.linalg.api.ops.impl.transforms.Histogram</c>).</summary>
    Public Class Histogram
        Inherits DynamicCustomOp

        Private numBins As Long

        Public Sub New()

        End Sub

        Public Sub New(ByVal input As INDArray, ByVal output As INDArray)
            Preconditions.checkArgument(output.Z, "Histogram op output should have integer data type")

            numBins = output.length()
            inputArgumentsField.Add(input)
            outputArgumentsField.Add(output)
            iArguments.Add(numBins)
        End Sub

        Public Sub New(ByVal input As INDArray, ByVal numBins As Long)
            Me.numBins = numBins
            inputArgumentsField.Add(input)
            iArguments.Add(numBins)
        End Sub

        Public Overrides Function opName() As String
            Return "histogram"
        End Function

    End Class

End Namespace
