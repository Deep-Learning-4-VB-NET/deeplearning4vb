Imports System
Imports NonNull = lombok.NonNull
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.deeplearning4j.optimize.solvers.accumulation

	<Serializable>
	Public Class LocalHandler
		Implements MessageHandler

		<NonSerialized>
		Protected Friend accumulator As GradientsAccumulator

		Public Sub New()
			'
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void initialize(@NonNull GradientsAccumulator accumulator)
		Public Overridable Sub initialize(ByVal accumulator As GradientsAccumulator) Implements MessageHandler.initialize
			Me.accumulator = accumulator
		End Sub

		Public Overridable Function broadcastUpdates(ByVal updates As INDArray, ByVal iterationNumber As Integer, ByVal epochNumber As Integer) As Boolean Implements MessageHandler.broadcastUpdates
			' we just loop back data immediately
			accumulator.receiveUpdate(updates)

			updates.assign(0.0)

			Nd4j.Executioner.commit()

			Return True
		End Function
	End Class

End Namespace