Imports System.Collections.Generic
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.common.primitives
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr

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

Namespace org.deeplearning4j.nn.api.layers


	Public Interface RecurrentLayer
		Inherits Layer

		''' <summary>
		''' Do one or more time steps using the previous time step state stored in stateMap.<br>
		''' Can be used to efficiently do forward pass one or n-steps at a time (instead of doing
		''' forward pass always from t=0)<br>
		''' If stateMap is empty, default initialization (usually zeros) is used<br>
		''' Implementations also update stateMap at the end of this method
		''' </summary>
		''' <param name="input"> Input to this layer </param>
		''' <returns> activations </returns>
		Function rnnTimeStep(ByVal input As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray

		''' <summary>
		''' Returns a shallow copy of the RNN stateMap (that contains the stored history for use in methods such
		''' as rnnTimeStep
		''' </summary>
		Function rnnGetPreviousState() As IDictionary(Of String, INDArray)

		''' <summary>
		''' Set the stateMap (stored history). Values set using this method will be used in next call to rnnTimeStep()
		''' </summary>
		Sub rnnSetPreviousState(ByVal stateMap As IDictionary(Of String, INDArray))

		''' <summary>
		''' Reset/clear the stateMap for rnnTimeStep() and tBpttStateMap for rnnActivateUsingStoredState()
		''' </summary>
		Sub rnnClearPreviousState()

		''' <summary>
		''' Similar to rnnTimeStep, this method is used for activations using the state
		''' stored in the stateMap as the initialization. However, unlike rnnTimeStep this
		''' method does not alter the stateMap; therefore, unlike rnnTimeStep, multiple calls to
		''' this method (with identical input) will:<br>
		''' (a) result in the same output<br>
		''' (b) leave the state maps (both stateMap and tBpttStateMap) in an identical state
		''' </summary>
		''' <param name="input">             Layer input </param>
		''' <param name="training">          if true: training. Otherwise: test </param>
		''' <param name="storeLastForTBPTT"> If true: store the final state in tBpttStateMap for use in truncated BPTT training </param>
		''' <returns> Layer activations </returns>
		Function rnnActivateUsingStoredState(ByVal input As INDArray, ByVal training As Boolean, ByVal storeLastForTBPTT As Boolean, ByVal workspaceMg As LayerWorkspaceMgr) As INDArray

		''' <summary>
		''' Get the RNN truncated backpropagations through time (TBPTT) state for the recurrent layer.
		''' The TBPTT state is used to store intermediate activations/state between updating parameters when doing
		''' TBPTT learning
		''' </summary>
		''' <returns> State for the RNN layer </returns>
		Function rnnGetTBPTTState() As IDictionary(Of String, INDArray)

		''' <summary>
		''' Set the RNN truncated backpropagations through time (TBPTT) state for the recurrent layer.
		''' The TBPTT state is used to store intermediate activations/state between updating parameters when doing
		''' TBPTT learning
		''' </summary>
		''' <param name="state"> TBPTT state to set </param>
		Sub rnnSetTBPTTState(ByVal state As IDictionary(Of String, INDArray))

		''' <summary>
		''' Truncated BPTT equivalent of Layer.backpropGradient().
		''' Primary difference here is that forward pass in the context of BPTT is that we do
		''' forward pass using stored state for truncated BPTT vs. from zero initialization
		''' for standard BPTT.
		''' </summary>
		Function tbpttBackpropGradient(ByVal epsilon As INDArray, ByVal tbpttBackLength As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)


	End Interface

End Namespace