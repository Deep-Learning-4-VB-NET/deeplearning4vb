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

Namespace org.datavec.api.transform.condition

    ''' <summary>
    ''' A boolean condition on a record (or a sequence of records): used by filters, conditional transforms and
    ''' conditional reductions. The <c>InputSchema</c> property is inherited from <see cref="ColumnOp"/>.
    ''' </summary>
    ' TODO(serde): polymorphic JSON attributes added in the serde wave
    ' (Java: @JsonInclude(NON_NULL) @JsonTypeInfo(use = CLASS, include = PROPERTY, property = "@class"))
    Public Interface Condition
        Inherits ColumnOp

        ''' <summary>
        ''' Is the condition satisfied for the current input/example?<br>
        ''' Returns true if condition is satisfied, or false otherwise.
        ''' </summary>
        ''' <param name="list"> Current example </param>
        ''' <returns> true if condition satisfied, false otherwise </returns>
        Function condition(ByVal list As IList(Of writable.Writable)) As Boolean

        ''' <summary>
        ''' Condition on arbitrary input </summary>
        ''' <param name="input"> the input to return
        '''              the condition for </param>
        ''' <returns> true if the condition is met
        ''' false otherwise </returns>
        Function condition(ByVal input As Object) As Boolean

        ''' <summary>
        ''' Is the condition satisfied for the current input/sequence?<br>
        ''' Returns true if condition is satisfied, or false otherwise.
        ''' </summary>
        ''' <param name="sequence"> Current sequence </param>
        ''' <returns> true if condition satisfied, false otherwise </returns>
        Function conditionSequence(ByVal sequence As IList(Of IList(Of writable.Writable))) As Boolean

        ''' <summary>
        ''' Condition on arbitrary input </summary>
        ''' <param name="sequence"> the sequence to
        '''                 do a condition on </param>
        ''' <returns> true if the condition for the sequence is met false otherwise </returns>
        Function conditionSequence(ByVal sequence As Object) As Boolean

    End Interface

End Namespace
