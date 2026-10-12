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

	Public Class RmsPropUpdater
		Inherits DynamicCustomOp

		Public Sub New()
			'
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public RmsPropUpdater(@NonNull INDArray gradients, @NonNull INDArray state, double lr, double decay, double epsilon)
		Public Sub New(ByVal gradients As INDArray, ByVal state As INDArray, ByVal lr As Double, ByVal decay As Double, ByVal epsilon As Double)
			Me.New(gradients, state, gradients, state, lr, decay, epsilon)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public RmsPropUpdater(@NonNull INDArray gradients, @NonNull INDArray state, @NonNull INDArray updates, @NonNull INDArray updatedState, double lr, double decay, double epsilon)
		Public Sub New(ByVal gradients As INDArray, ByVal state As INDArray, ByVal updates As INDArray, ByVal updatedState As INDArray, ByVal lr As Double, ByVal decay As Double, ByVal epsilon As Double)
			addInputArgument(gradients, state)
			addOutputArgument(updates, updatedState)
			addTArgument(lr, decay, epsilon)
		End Sub

		Public Overrides Function opName() As String
			Return "rms_prop_updater"
		End Function
	End Class

End Namespace