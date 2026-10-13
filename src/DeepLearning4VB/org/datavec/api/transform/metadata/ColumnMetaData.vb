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

Imports ColumnTypeEnum = DeepLearning4VB.org.datavec.api.transform.ColumnType

Namespace org.datavec.api.transform.metadata

    ''' <summary>
    ''' The metadata of one column of a schema: its name, its <see cref="ColumnTypeEnum"/> and the rules that
    ''' decide whether a value is valid for it. Implementations are serializable and cloneable (the Java
    ''' <c>Cloneable</c> marker is replaced by the explicit <see cref="clone"/> method).
    ''' </summary>
    ' TODO(serde): polymorphic JSON attributes added in the serde wave
    ' (Java: @JsonInclude(NON_NULL) @JsonTypeInfo(use = CLASS, include = PROPERTY, property = "@class"))
    Public Interface ColumnMetaData

        ''' <summary>
        ''' Get the name of the specified column </summary>
        ''' <returns> Name of the column </returns>
        Property Name As String

        ''' <summary>Java-style accessor for <see cref="Name"/>.</summary>
        Function getName() As String

        ''' <summary>Java-style mutator for <see cref="Name"/>.</summary>
        Sub setName(ByVal name As String)

        ''' <summary>
        ''' Get the type of column
        ''' </summary>
        ReadOnly Property ColumnType As ColumnTypeEnum

        ''' <summary>Java-style accessor for <see cref="ColumnType"/>.</summary>
        Function getColumnType() As ColumnTypeEnum

        ''' <summary>
        ''' Is the given Writable valid for this column, given the column type and any restrictions given by the
        ''' ColumnMetaData object?
        ''' </summary>
        ''' <param name="writable"> Writable to check </param>
        ''' <returns> true if value, false if invalid </returns>
        Function isValid(ByVal writable As writable.Writable) As Boolean

        ''' <summary>
        ''' Is the given object valid for this column,
        ''' given the column type and any
        ''' restrictions given by the
        ''' ColumnMetaData object?
        ''' </summary>
        ''' <param name="input"> object to check </param>
        ''' <returns> true if value, false if invalid </returns>
        Function isValid(ByVal input As Object) As Boolean

        ''' <summary>
        ''' A copy of this metadata (VB has no covariant return types, so implementations return the interface).
        ''' </summary>
        Function clone() As ColumnMetaData
    End Interface

End Namespace
