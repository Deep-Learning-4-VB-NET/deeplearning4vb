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

Namespace org.nd4j.autodiff.execution.input

	Public Interface OperandsAdapter(Of T)

		''' <summary>
		''' This method must return collection of graph inputs as Operands
		''' @return
		''' </summary>
		Function input(ByVal input As T) As Operands

		''' <summary>
		''' This method returns adopted result of graph execution
		''' @return
		''' </summary>
		Function output(ByVal operands As Operands) As T
	End Interface

End Namespace