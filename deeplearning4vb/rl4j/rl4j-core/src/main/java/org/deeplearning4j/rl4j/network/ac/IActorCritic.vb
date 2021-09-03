Imports System
Imports System.IO
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

Namespace org.deeplearning4j.rl4j.network.ac


	<Obsolete>
	Public Interface IActorCritic(Of NN As IActorCritic)
		Inherits NeuralNet(Of NN)

		'FIRST SHOULD BE VALUE AND SECOND IS SOFTMAX POLICY. DONT MESS THIS UP OR ELSE ASYNC THREAD IS BROKEN (maxQ) !
		Function outputAll(ByVal batch As INDArray) As INDArray()

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: void save(java.io.OutputStream streamValue, java.io.OutputStream streamPolicy) throws java.io.IOException;
		Sub save(ByVal streamValue As Stream, ByVal streamPolicy As Stream)

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: void save(String pathValue, String pathPolicy) throws java.io.IOException;
		Sub save(ByVal pathValue As String, ByVal pathPolicy As String)

	End Interface

End Namespace