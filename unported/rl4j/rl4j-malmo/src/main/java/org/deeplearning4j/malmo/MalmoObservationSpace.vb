Imports org.deeplearning4j.rl4j.space
Imports WorldState = com.microsoft.msr.malmo.WorldState

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

Namespace org.deeplearning4j.malmo

	Public MustInherit Class MalmoObservationSpace
		Implements ObservationSpace(Of MalmoBox)

		Public MustOverride ReadOnly Property High As org.nd4j.linalg.api.ndarray.INDArray Implements ObservationSpace(Of MalmoBox).getHigh
		Public MustOverride ReadOnly Property Low As org.nd4j.linalg.api.ndarray.INDArray Implements ObservationSpace(Of MalmoBox).getLow
		Public MustOverride ReadOnly Property Shape As Integer() Implements ObservationSpace(Of MalmoBox).getShape
		Public MustOverride ReadOnly Property Name As String Implements ObservationSpace(Of MalmoBox).getName
		Public MustOverride Function getObservation(ByVal world_state As WorldState) As MalmoBox
	End Class

End Namespace