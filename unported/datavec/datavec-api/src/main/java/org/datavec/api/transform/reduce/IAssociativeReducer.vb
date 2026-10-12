Imports System.Collections.Generic
Imports org.datavec.api.transform.ops
Imports Schema = org.datavec.api.transform.schema.Schema
Imports Writable = org.datavec.api.writable.Writable
Imports JsonInclude = org.nd4j.shade.jackson.annotation.JsonInclude
Imports JsonSubTypes = org.nd4j.shade.jackson.annotation.JsonSubTypes
Imports JsonTypeInfo = org.nd4j.shade.jackson.annotation.JsonTypeInfo

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

Namespace org.datavec.api.transform.reduce


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonInclude(JsonInclude.Include.NON_NULL) @JsonTypeInfo(use = JsonTypeInfo.Id.NAME, include = JsonTypeInfo.@As.WRAPPER_OBJECT) @JsonSubTypes(value = {@JsonSubTypes.Type(value = Reducer.class, name = "Reducer")}) public interface IAssociativeReducer extends java.io.Serializable
	Public Interface IAssociativeReducer

		''' 
		''' <param name="schema"> </param>
		Property InputSchema As Schema


		''' 
		''' <param name="schema">
		''' @return </param>
		Function transform(ByVal schema As Schema) As Schema

		''' <summary>
		''' An aggregation that has the property that
		''' reduce(List(reduce(List(l1, l2)), l3)) = reduce(List(l1, reduce(List(l2, l3)))
		''' @return
		''' </summary>
		Function aggregableReducer() As IAggregableReduceOp(Of IList(Of Writable), IList(Of Writable))

		''' 
		''' <summary>
		''' @return
		''' </summary>
		ReadOnly Property KeyColumns As IList(Of String)

	End Interface

End Namespace