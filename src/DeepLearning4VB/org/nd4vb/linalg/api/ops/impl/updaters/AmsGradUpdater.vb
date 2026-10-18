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

    ''' <summary>The custom op <c>ams_grad_updater</c> (Java <c>org.nd4j.linalg.api.ops.impl.updaters.AmsGradUpdater</c>).</summary>
    Public Class AmsGradUpdater
        Inherits DynamicCustomOp

        Public Sub New()
            '
        End Sub

        Public Sub New(ByVal gradients As INDArray, ByVal stateV As INDArray, ByVal stateM As INDArray, ByVal stateH As INDArray, ByVal lr As Double, ByVal beta1 As Double, ByVal beta2 As Double, ByVal epsilon As Double, ByVal iteration As Integer)
            Me.New(gradients, stateV, stateM, stateH, gradients, stateV, stateM, stateH, lr, beta1, beta2, epsilon, iteration)
        End Sub

        Public Sub New(ByVal gradients As INDArray, ByVal stateV As INDArray, ByVal stateM As INDArray, ByVal stateH As INDArray, ByVal updates As INDArray, ByVal updatedStateV As INDArray, ByVal updatedStateM As INDArray, ByVal updatedStateH As INDArray, ByVal lr As Double, ByVal beta1 As Double, ByVal beta2 As Double, ByVal epsilon As Double, ByVal iteration As Integer)
            addInputArgument(gradients, stateV, stateM, stateH)
            addOutputArgument(updates, updatedStateV, updatedStateM, updatedStateH)
            addTArgument(lr, beta1, beta2, epsilon)
            addIArgument(iteration)
        End Sub

        Public Overrides Function opName() As String
            Return "ams_grad_updater"
        End Function

    End Class

End Namespace
