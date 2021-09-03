Imports System.Collections.Generic
Imports ColumnOp = org.datavec.api.transform.ColumnOp
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports org.datavec.api.transform.ops
Imports Writable = org.datavec.api.writable.Writable

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


	Public Interface AggregableColumnReduction
		Inherits ColumnOp

		''' <summary>
		''' Reduce a single column.
		''' <b>Note</b>: The {@code List<Writable>}
		''' here is a single <b>column</b> in a reduction window,
		''' and NOT the single row
		''' (as is usually the case for {@code List<Writable>} instances
		''' </summary>
		''' <param name="columnData"> The Writable objects for a column </param>
		''' <returns> Writable containing the reduced data </returns>
		Function reduceOp() As IAggregableReduceOp(Of Writable, IList(Of Writable))

		''' <summary>
		''' Post-reduce: what is the name of the column?
		''' For example, "myColumn" -> "mean(myColumn)"
		''' </summary>
		''' <param name="columnInputName"> Name of the column before reduction </param>
		''' <returns> Name of the column after the reduction </returns>
		Function getColumnsOutputName(ByVal columnInputName As String) As IList(Of String)

		''' <summary>
		''' Post-reduce: what is the metadata (type, etc) for this column?
		''' For example: a "count unique" operation on a String (StringMetaData) column would return an Integer (IntegerMetaData) column
		''' </summary>
		''' <param name="columnInputMeta"> Metadata for the column, before reduce </param>
		''' <returns> Metadata for the column, after the reduction </returns>
		Function getColumnOutputMetaData(ByVal newColumnName As IList(Of String), ByVal columnInputMeta As ColumnMetaData) As IList(Of ColumnMetaData)

	End Interface

End Namespace