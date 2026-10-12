Imports InvalidStepException = org.deeplearning4j.exception.InvalidStepException
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

Namespace org.deeplearning4j.optimize.api


	Public Interface LineOptimizer
		''' <summary>
		''' Line optimizer </summary>
		''' <param name="parameters"> the parameters to optimize </param>
		''' <param name="gradient"> the gradient </param>
		''' <param name="searchDirection">  the point/direction to go in </param>
		''' <returns> the last step size used </returns>
		''' <exception cref="InvalidStepException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: double optimize(org.nd4j.linalg.api.ndarray.INDArray parameters, org.nd4j.linalg.api.ndarray.INDArray gradient, org.nd4j.linalg.api.ndarray.INDArray searchDirection, org.deeplearning4j.nn.workspace.LayerWorkspaceMgr workspaceMgr) throws org.deeplearning4j.exception.InvalidStepException;
		Function optimize(ByVal parameters As INDArray, ByVal gradient As INDArray, ByVal searchDirection As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Double



	End Interface

End Namespace