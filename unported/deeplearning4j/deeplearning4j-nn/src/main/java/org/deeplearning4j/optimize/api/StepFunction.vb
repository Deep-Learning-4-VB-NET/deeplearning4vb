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

Namespace org.deeplearning4j.optimize.api


	Public Interface StepFunction

		''' <summary>
		''' Step with the given parameters </summary>
		''' <param name="x"> the current parameters </param>
		''' <param name="line"> the line to step </param>
		''' <param name="step"> </param>
		Sub [step](ByVal x As INDArray, ByVal line As INDArray, ByVal [step] As Double)


		''' <summary>
		''' Step with no parameters
		''' </summary>
		Sub [step](ByVal x As INDArray, ByVal line As INDArray)


		Sub [step]()


	End Interface

End Namespace