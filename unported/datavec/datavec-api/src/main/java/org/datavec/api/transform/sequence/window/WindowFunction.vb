Imports System.Collections.Generic
Imports Schema = org.datavec.api.transform.schema.Schema
Imports Writable = org.datavec.api.writable.Writable
Imports JsonInclude = org.nd4j.shade.jackson.annotation.JsonInclude
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

Namespace org.datavec.api.transform.sequence.window


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonInclude(JsonInclude.Include.NON_NULL) @JsonTypeInfo(use = JsonTypeInfo.Id.@CLASS, include = JsonTypeInfo.@As.@PROPERTY, property = "@class") public interface WindowFunction extends java.io.Serializable
	Public Interface WindowFunction

		''' <summary>
		''' Apply the windowing function to the given sequence </summary>
		''' <param name="sequence"> the input sequence </param>
		''' <returns> the sequence with the window function applied </returns>
		Function applyToSequence(ByVal sequence As IList(Of IList(Of Writable))) As IList(Of IList(Of IList(Of Writable)))

		''' 
		''' <param name="schema"> </param>
		Property InputSchema As Schema


		''' <summary>
		''' Get the output schema, given the input schema. Typically the output schema is the same as the input schema,
		''' but not necessarily (for example, if the window function adds columns for the window start/end times) </summary>
		''' <param name="inputSchema">    Schema of the input data </param>
		''' <returns> Schema of the output windows </returns>
		Function transform(ByVal inputSchema As Schema) As Schema


	End Interface

End Namespace