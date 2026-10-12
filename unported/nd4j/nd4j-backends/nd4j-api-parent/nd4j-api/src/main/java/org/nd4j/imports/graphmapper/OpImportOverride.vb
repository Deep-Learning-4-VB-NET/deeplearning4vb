Imports System.Collections.Generic
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

Namespace org.nd4j.imports.graphmapper


	Public Interface OpImportOverride(Of GRAPH_TYPE, NODE_TYPE, ATTR_TYPE)

		''' <summary>
		''' Initialize the operation and return its output variables
		''' </summary>
		Function initFromTensorFlow(ByVal inputs As IList(Of SDVariable), ByVal controlDepInputs As IList(Of SDVariable), ByVal nodeDef As NODE_TYPE, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, ATTR_TYPE), ByVal graph As GRAPH_TYPE) As IList(Of SDVariable)

	End Interface

End Namespace