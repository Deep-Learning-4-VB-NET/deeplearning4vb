Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable

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

Namespace org.nd4j.linalg.api.ops.impl.layers.recurrent.config

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @Builder public class GRUCellConfiguration
	Public Class GRUCellConfiguration
	'    
	'    Inputs:
	'    x        input [bS x inSize]
	'    hLast    previous cell output [bS x numUnits],  that is at previous time step t-1
	'    Wru      RU weights - [bS, 2*numUnits] - reset and update gates
	'    Wc       C weights - [bS, numUnits] - cell gate
	'    bru      r and u biases, [2*numUnits] - reset and update gates
	'    bc       c biases, [numUnits] - cell gate
	'     

		Private xt, hLast, Wru, Wc, bru, bc As SDVariable

		Public Overridable Function args() As SDVariable()
			Return New SDVariable() {xt, hLast, Wru, Wc, bru, bc}
		End Function

	End Class

End Namespace