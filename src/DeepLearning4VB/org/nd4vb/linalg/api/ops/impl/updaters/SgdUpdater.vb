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

Namespace org.nd4vb.linalg.api.ops.impl.updaters

    ''' <summary>The custom op <c>sgd_updater</c> (Java <c>org.nd4j.linalg.api.ops.impl.updaters.SgdUpdater</c>).</summary>
    Public Class SgdUpdater
        Inherits DynamicCustomOp

        Public Sub New()
            '
        End Sub

        Public Sub New(ByVal input As INDArray, ByVal lr As Double)
            Me.New(input, input, lr)
        End Sub

        Public Sub New(ByVal input As INDArray, ByVal output As INDArray, ByVal lr As Double)
            addInputArgument(input)
            addOutputArgument(output)
            addTArgument(lr)
        End Sub

        Public Overrides Function opName() As String
            Return "sgd_updater"
        End Function

    End Class

End Namespace
