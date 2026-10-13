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

Namespace org.datavec.api.transform

    ''' <summary>
    ''' A transformation of a record (or of a sequence of records) into another record: the building block of a
    ''' transform process. Implementations are serializable and carry their input schema.
    ''' </summary>
    ' TODO(serde): polymorphic JSON attributes added in the serde wave
    ' (Java: @JsonInclude(NON_NULL) @JsonTypeInfo(use = CLASS, include = PROPERTY, property = "@class"))
    Public Interface Transform
        Inherits ColumnOp

        ''' <summary>
        ''' Transform a writable
        ''' in to another writable </summary>
        ''' <param name="writables"> the record to transform </param>
        ''' <returns> the transformed writable </returns>
        Function map(ByVal writables As IList(Of writable.Writable)) As IList(Of writable.Writable)

        ''' <summary>
        ''' Transform a sequence </summary>
        Function mapSequence(ByVal sequence As IList(Of IList(Of writable.Writable))) As IList(Of IList(Of writable.Writable))

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
