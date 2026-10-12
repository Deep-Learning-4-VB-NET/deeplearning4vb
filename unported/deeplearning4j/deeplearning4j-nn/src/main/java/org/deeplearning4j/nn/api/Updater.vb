Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
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

Namespace org.deeplearning4j.nn.api


	''' <summary>
	''' Update the model
	''' 
	''' @author Adam Gibson
	''' </summary>
	Public Interface Updater

		''' <summary>
		''' Set the internal (historical) state view array for this updater
		''' </summary>
		''' <param name="layer">      Layer that this updater belongs to </param>
		''' <param name="viewArray">  View array </param>
		''' <param name="initialize"> Whether to initialize the array or not </param>
		Sub setStateViewArray(ByVal layer As Trainable, ByVal viewArray As INDArray, ByVal initialize As Boolean)

		''' <returns> the view array for this updater </returns>
		ReadOnly Property StateViewArray As INDArray

		''' <summary>
		''' Updater: updates the model
		''' </summary>
		''' <param name="layer"> </param>
		''' <param name="gradient"> </param>
		''' <param name="iteration"> </param>
		Sub update(ByVal layer As Trainable, ByVal gradient As Gradient, ByVal iteration As Integer, ByVal epoch As Integer, ByVal miniBatchSize As Integer, ByVal workspaceMgr As LayerWorkspaceMgr)
	End Interface

End Namespace