Imports System
Imports ResidualPostProcessor = org.deeplearning4j.optimize.solvers.accumulation.encoding.ResidualPostProcessor
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

Namespace org.deeplearning4j.optimize.solvers.accumulation.encoding.residual

	<Serializable>
	Public Class NoOpResidualPostProcessor
		Implements ResidualPostProcessor

		Public Overridable Sub processResidual(ByVal iteration As Integer, ByVal epoch As Integer, ByVal lastThreshold As Double, ByVal residualVector As INDArray) Implements ResidualPostProcessor.processResidual
			'No op
		End Sub

		Public Overridable Function clone() As ResidualPostProcessor
			Return New NoOpResidualPostProcessor()
		End Function
	End Class

End Namespace