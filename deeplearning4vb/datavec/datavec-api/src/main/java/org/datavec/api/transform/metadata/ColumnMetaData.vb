Imports System
Imports ColumnType = org.datavec.api.transform.ColumnType
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

Namespace org.datavec.api.transform.metadata


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonInclude(JsonInclude.Include.NON_NULL) @JsonTypeInfo(use = JsonTypeInfo.Id.@CLASS, include = JsonTypeInfo.@As.@PROPERTY, property = "@class") public interface ColumnMetaData extends java.io.Serializable, Cloneable
	Public Interface ColumnMetaData
		Inherits ICloneable

		''' <summary>
		''' Get the name of the specified column </summary>
		''' <returns> Name of the column </returns>
		Property Name As String


		''' <summary>
		''' Get the type of column
		''' </summary>
		ReadOnly Property ColumnType As ColumnType

		''' <summary>
		''' Is the given Writable valid for this column, given the column type and any restrictions given by the
		''' ColumnMetaData object?
		''' </summary>
		''' <param name="writable"> Writable to check </param>
		''' <returns> true if value, false if invalid </returns>
		Function isValid(ByVal writable As Writable) As Boolean

		''' <summary>
		''' Is the given object valid for this column,
		''' given the column type and any
		''' restrictions given by the
		''' ColumnMetaData object?
		''' </summary>
		''' <param name="input"> object to check </param>
		''' <returns> true if value, false if invalid </returns>
		Function isValid(ByVal input As Object) As Boolean

		''' 
		''' <summary>
		''' @return
		''' </summary>
		Function clone() As ColumnMetaData
	End Interface

End Namespace