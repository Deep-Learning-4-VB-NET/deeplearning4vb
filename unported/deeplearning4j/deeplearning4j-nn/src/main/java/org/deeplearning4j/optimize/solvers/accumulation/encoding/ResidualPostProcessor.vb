Imports System
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

Namespace org.deeplearning4j.optimize.solvers.accumulation.encoding


	Public Interface ResidualPostProcessor
		Inherits ICloneable

		''' <param name="iteration">      Current iteration </param>
		''' <param name="epoch">          Current epoch </param>
		''' <param name="lastThreshold">  Last threshold that was used </param>
		''' <param name="residualVector"> The current residual vector. Should be modified in-place </param>
		Sub processResidual(ByVal iteration As Integer, ByVal epoch As Integer, ByVal lastThreshold As Double, ByVal residualVector As INDArray)

		Function clone() As ResidualPostProcessor
	End Interface

End Namespace