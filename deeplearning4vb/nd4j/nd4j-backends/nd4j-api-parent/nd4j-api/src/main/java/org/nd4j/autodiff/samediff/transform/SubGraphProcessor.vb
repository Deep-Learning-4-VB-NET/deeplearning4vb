﻿Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff

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

Namespace org.nd4j.autodiff.samediff.transform


	Public Interface SubGraphProcessor

		''' <summary>
		''' Replace the subgraph, and return the new outputs that should replace the old outputs.<br>
		''' Note that the order of the outputs you return matters!<br>
		''' If the original outputs are [A,B,C] and you return output variables [X,Y,Z], then anywhere "A" was used as input
		''' will now use "X"; similarly Y replaces B, and Z replaces C.
		''' </summary>
		''' <param name="sd"> SameDiff instance </param>
		''' <param name="subGraph"> Subgraph to modify </param>
		''' <returns> New output variables </returns>
		Function processSubgraph(ByVal sd As SameDiff, ByVal subGraph As SubGraph) As IList(Of SDVariable)

	End Interface

End Namespace