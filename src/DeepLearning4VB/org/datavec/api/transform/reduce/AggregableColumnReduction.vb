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

Imports System.Collections.Generic
Imports DeepLearning4VB.org.datavec.api.transform.ops
Imports DeepLearning4VB.org.datavec.api.transform.serde
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.reduce

    ''' <summary>
    ''' A custom reduction of one column, expressed as a combinable accumulator (see
    ''' <see cref="Reducer.Builder.customReduction"/>). It may produce several output columns. Implementations are
    ''' serializable.
    ''' </summary>
    ' Java: @JsonInclude(NON_NULL) @JsonTypeInfo(use = CLASS, include = PROPERTY, property = "@class")
    <JsonClassType>
    <JsonIncludeNonNull>
    Public Interface AggregableColumnReduction
        Inherits ColumnOp

        ''' <summary>
        ''' A new accumulator for the reduction of a single column.
        ''' <b>Note</b>: the values passed to the accumulator are the values of a single <b>column</b> in a
        ''' reduction window, NOT single rows.
        ''' </summary>
        ''' <returns> the accumulator; its result holds one value per output column </returns>
        Function reduceOp() As IAggregableReduceOp(Of WritableT, IList(Of WritableT))

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
