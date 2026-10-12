Imports Schema = org.datavec.api.transform.schema.Schema

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


	Public Interface ColumnOp
		Inherits Operation(Of Schema, Schema)

		''' <summary>
		''' Set the input schema.
		''' </summary>
		Property InputSchema As Schema


		''' <summary>
		''' The output column name
		''' after the operation has been applied </summary>
		''' <returns> the output column name </returns>
		Function outputColumnName() As String

		''' <summary>
		''' The output column names
		''' This will often be the same as the input </summary>
		''' <returns> the output column names </returns>
		Function outputColumnNames() As String()

		''' <summary>
		''' Returns column names
		''' this op is meant to run on
		''' @return
		''' </summary>
		Function columnNames() As String()

		''' <summary>
		''' Returns a singular column name
		''' this op is meant to run on
		''' @return
		''' </summary>
		Function columnName() As String

	End Interface

End Namespace