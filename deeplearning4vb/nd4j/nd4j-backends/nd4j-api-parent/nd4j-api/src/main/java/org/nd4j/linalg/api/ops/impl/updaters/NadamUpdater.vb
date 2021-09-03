Imports NonNull = lombok.NonNull
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp

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

Namespace org.nd4j.linalg.api.ops.impl.updaters

	Public Class NadamUpdater
		Inherits DynamicCustomOp

		Public Sub New()
			'
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public NadamUpdater(@NonNull INDArray gradients, @NonNull INDArray stateV, @NonNull INDArray stateM, double lr, double beta1, double beta2, double epsilon, int iteration)
		Public Sub New(ByVal gradients As INDArray, ByVal stateV As INDArray, ByVal stateM As INDArray, ByVal lr As Double, ByVal beta1 As Double, ByVal beta2 As Double, ByVal epsilon As Double, ByVal iteration As Integer)
			Me.New(gradients, stateV, stateM, gradients, stateV, stateM, lr, beta1, beta2, epsilon, iteration)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public NadamUpdater(@NonNull INDArray gradients, @NonNull INDArray stateV, @NonNull INDArray stateM, @NonNull INDArray updates, @NonNull INDArray updatedStateV, @NonNull INDArray updatedStateM, double lr, double beta1, double beta2, double epsilon, int iteration)
		Public Sub New(ByVal gradients As INDArray, ByVal stateV As INDArray, ByVal stateM As INDArray, ByVal updates As INDArray, ByVal updatedStateV As INDArray, ByVal updatedStateM As INDArray, ByVal lr As Double, ByVal beta1 As Double, ByVal beta2 As Double, ByVal epsilon As Double, ByVal iteration As Integer)
			addInputArgument(gradients, stateV, stateM)
			addOutputArgument(updates, updatedStateV, updatedStateM)
			addTArgument(lr, beta1, beta2, epsilon)
			addIArgument(iteration)
		End Sub

		Public Overrides Function opName() As String
			Return "nadam_updater"
		End Function
	End Class

End Namespace