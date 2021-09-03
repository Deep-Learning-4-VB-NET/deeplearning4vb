﻿Imports System.Collections.Generic
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

Namespace org.datavec.api.transform


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonInclude(JsonInclude.Include.NON_NULL) @JsonTypeInfo(use = JsonTypeInfo.Id.@CLASS, include = JsonTypeInfo.@As.@PROPERTY, property = "@class") public interface Transform extends java.io.Serializable, ColumnOp
	Public Interface Transform
		Inherits ColumnOp

		''' <summary>
		''' Transform a writable
		''' in to another writable </summary>
		''' <param name="writables"> the record to transform </param>
		''' <returns> the transformed writable </returns>
		Function map(ByVal writables As IList(Of Writable)) As IList(Of Writable)

		''' <summary>
		''' Transform a sequence </summary>
		Function mapSequence(ByVal sequence As IList(Of IList(Of Writable))) As IList(Of IList(Of Writable))

		''' <summary>
		''' Transform an object
		''' in to another object </summary>
		''' <param name="input"> the record to transform </param>
		''' <returns> the transformed writable </returns>
		Function map(ByVal input As Object) As Object

		''' <summary>
		''' Transform a sequence </summary>
		Function mapSequence(ByVal sequence As Object) As Object


	End Interface

End Namespace