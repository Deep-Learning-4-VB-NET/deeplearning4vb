Imports System.Collections.Generic
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


	''' <summary>
	''' Define whether the operation should be skipped during import
	''' </summary>
	Public Interface OpImportFilter(Of GRAPH_TYPE, NODE_TYPE, ATTR_TYPE)

		''' <summary>
		''' If true: the op should be skipped for import, and its output variables should not be created. If false: the op should be imported </summary>
		''' <param name="nodeDef">           Node </param>
		''' <param name="initWith">          SameDiff instance </param>
		''' <param name="attributesForNode"> Attributes for the node </param>
		''' <param name="graph">             Graph to import from </param>
		''' <returns> True if the op should be skipped during import </returns>
		Function skipOp(ByVal nodeDef As NODE_TYPE, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, ATTR_TYPE), ByVal graph As GRAPH_TYPE) As Boolean

	End Interface

End Namespace