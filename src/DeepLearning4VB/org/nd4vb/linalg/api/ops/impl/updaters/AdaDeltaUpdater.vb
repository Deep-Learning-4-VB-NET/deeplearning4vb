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

    ''' <summary>The custom op <c>ada_delta_updater</c> (Java <c>org.nd4j.linalg.api.ops.impl.updaters.AdaDeltaUpdater</c>).</summary>
    Public Class AdaDeltaUpdater
        Inherits DynamicCustomOp

        Public Sub New()
            '
        End Sub

        Public Sub New(ByVal gradients As INDArray, ByVal stateMsg As INDArray, ByVal stateMsdx As INDArray, ByVal rho As Double, ByVal epsilon As Double)
            Me.New(gradients, stateMsg, stateMsdx, gradients, stateMsg, stateMsdx, rho, epsilon)
        End Sub

        Public Sub New(ByVal gradients As INDArray, ByVal stateMsg As INDArray, ByVal stateMsdx As INDArray, ByVal updates As INDArray, ByVal updatedStateMsg As INDArray, ByVal updatedStateMsdx As INDArray, ByVal rho As Double, ByVal epsilon As Double)
            addInputArgument(gradients, stateMsg, stateMsdx)
            addOutputArgument(updates, updatedStateMsg, updatedStateMsdx)
            addTArgument(rho, epsilon)
        End Sub

        Public Overrides Function opName() As String
            Return "ada_delta_updater"
        End Function

    End Class

End Namespace
