Imports System
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports org.deeplearning4j.rl4j.network
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

Namespace org.deeplearning4j.rl4j.network.dqn

	<Obsolete>
	Public Interface IDQN(Of NN As IDQN)
		Inherits NeuralNet(Of NN)

		Sub fit(ByVal input As INDArray, ByVal labels As INDArray)

		Function gradient(ByVal input As INDArray, ByVal label As INDArray) As Gradient()

		Function gradient(ByVal input As INDArray, ByVal label() As INDArray) As Gradient()
	End Interface

End Namespace