Imports Classifier = org.deeplearning4j.nn.api.Classifier
Imports Layer = org.deeplearning4j.nn.api.Layer
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

Namespace org.deeplearning4j.nn.api.layers

	Public Interface IOutputLayer
		Inherits Layer, Classifier

		''' <summary>
		''' Returns true if labels are required
		''' for this output layer </summary>
		''' <returns> true if this output layer needs labels or not </returns>
		Function needsLabels() As Boolean

		''' <summary>
		''' Set the labels array for this output layer
		''' </summary>
		''' <param name="labels"> Labels array to set </param>
		Property Labels As INDArray


		''' <summary>
		''' Compute score after labels and input have been set.
		''' </summary>
		''' <param name="fullNetworkRegScore"> Regularization score (l1/l2/weight decay) for the entire network </param>
		''' <param name="training">            whether score should be calculated at train or test time (this affects things like application of
		'''                            dropout, etc) </param>
		''' <returns> score (loss function) </returns>
		Function computeScore(ByVal fullNetworkRegScore As Double, ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As Double

		''' <summary>
		''' Compute the score for each example individually, after labels and input have been set.
		''' </summary>
		''' <param name="fullNetworkRegScore"> Regularization score (l1/l2/weight decay) for the entire network </param>
		''' <returns> A column INDArray of shape [numExamples,1], where entry i is the score of the ith example </returns>
		Function computeScoreForExamples(ByVal fullNetworkRegScore As Double, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray


	End Interface

End Namespace