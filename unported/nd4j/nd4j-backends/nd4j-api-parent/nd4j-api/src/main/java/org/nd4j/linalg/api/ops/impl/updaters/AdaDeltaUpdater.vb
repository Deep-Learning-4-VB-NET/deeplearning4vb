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

	Public Class AdaDeltaUpdater
		Inherits DynamicCustomOp

		Public Sub New()
			'
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public AdaDeltaUpdater(@NonNull INDArray gradients, @NonNull INDArray stateMsg, @NonNull INDArray stateMsdx, double rho, double epsilon)
		Public Sub New(ByVal gradients As INDArray, ByVal stateMsg As INDArray, ByVal stateMsdx As INDArray, ByVal rho As Double, ByVal epsilon As Double)
			Me.New(gradients, stateMsg, stateMsdx, gradients, stateMsg, stateMsdx, rho, epsilon)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public AdaDeltaUpdater(@NonNull INDArray gradients, @NonNull INDArray stateMsg, @NonNull INDArray stateMsdx, @NonNull INDArray updates, @NonNull INDArray updatedStateMsg, @NonNull INDArray updatedStateMsdx, double rho, double epsilon)
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