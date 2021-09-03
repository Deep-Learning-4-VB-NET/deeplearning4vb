Imports WorldState = com.microsoft.msr.malmo.WorldState
Imports Box = org.deeplearning4j.rl4j.space.Box
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

	Public Class MalmoDescretePositionPolicy
		Implements MalmoObservationPolicy

		Friend observationSpace As New MalmoObservationSpacePosition()

		Public Overridable Function isObservationConsistant(ByVal world_state As WorldState, ByVal original_world_state As WorldState) As Boolean Implements MalmoObservationPolicy.isObservationConsistant
			Dim last_observation As Box = observationSpace.getObservation(world_state)
			Dim old_observation As Box = observationSpace.getObservation(original_world_state)

			Dim newvalues As INDArray = If(old_observation Is Nothing, Nothing, old_observation.Data)
			Dim oldvalues As INDArray = If(last_observation Is Nothing, Nothing, last_observation.Data)

			Return Not (world_state.getObservations().isEmpty() OrElse world_state.getRewards().isEmpty() OrElse oldvalues.eq(newvalues).all())
		End Function

	End Class

End Namespace