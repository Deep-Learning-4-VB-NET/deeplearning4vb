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

Imports System
Imports ColumnTypeEnum = DeepLearning4VB.org.datavec.api.transform.ColumnType

Namespace org.datavec.api.transform.analysis.columns

    ''' <summary>
    ''' The analysis (summary statistics) of one column. The implementations are <see cref="SerializableAttribute"/>.
    ''' </summary>
    ' Java: @JsonInclude(JsonInclude.Include.NON_NULL) @JsonTypeInfo(use = CLASS, include = PROPERTY, property = "@class")
    ' TODO(serde): polymorphic JSON attributes added in the serde wave
    Public Interface ColumnAnalysis

        ''' <summary>Number of values analysed.</summary>
        ReadOnly Property CountTotal As Long

        ''' <summary>The type of the analysed column.</summary>
        ReadOnly Property ColumnType As ColumnTypeEnum

        ''' <summary>Java-style accessor for <see cref="CountTotal"/>.</summary>
        Function getCountTotal() As Long

        ''' <summary>Java-style accessor for <see cref="ColumnType"/>.</summary>
        Function getColumnType() As ColumnTypeEnum

    End Interface

End Namespace
