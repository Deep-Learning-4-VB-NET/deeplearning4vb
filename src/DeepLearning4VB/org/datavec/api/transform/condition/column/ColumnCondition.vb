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

Namespace org.datavec.api.transform.condition.column

    ''' <summary>
    ''' A <see cref="Condition"/> on the value of a single column. The Java interface constant
    ''' <c>DEFAULT_SEQUENCE_CONDITION_MODE</c> (= <see cref="SequenceConditionMode.Or"/>) lives on
    ''' <see cref="BaseColumnCondition.DEFAULT_SEQUENCE_CONDITION_MODE"/>, as VB interfaces cannot hold fields.
    ''' All other members (<c>condition</c>, <c>conditionSequence</c>, <c>transform</c>, <c>InputSchema</c>, the
    ''' column name accessors) are inherited from <see cref="Condition"/> and <see cref="ColumnOp"/>.
    ''' </summary>
    Public Interface ColumnCondition
        Inherits Condition

        ''' <summary>
        ''' Returns whether the given element
        ''' meets the condition set by this operation </summary>
        ''' <param name="writable"> the element to test </param>
        ''' <returns> true if the condition is met
        ''' false otherwise </returns>
        Function columnCondition(ByVal writable As writable.Writable) As Boolean
    End Interface

End Namespace
