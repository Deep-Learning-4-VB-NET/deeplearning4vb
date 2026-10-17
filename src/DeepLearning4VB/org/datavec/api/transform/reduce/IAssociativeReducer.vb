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
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.reduce

    ''' <summary>
    ''' A reduction of a group of records (for example all the records with the same key) into one record, expressed as
    ''' an associative aggregation: reduce(List(reduce(List(l1, l2)), l3)) = reduce(List(l1, reduce(List(l2, l3))).
    ''' Implementations are serializable.
    ''' </summary>
    ' TODO(serde): polymorphic JSON attributes added in the serde wave
    ' (Java: @JsonInclude(NON_NULL) @JsonTypeInfo(use = NAME, include = WRAPPER_OBJECT)
    '  @JsonSubTypes({@Type(value = Reducer.class, name = "Reducer")}))
    Public Interface IAssociativeReducer

        ''' <summary>
        ''' The input schema (Java <c>getInputSchema</c>/<c>setInputSchema</c>).
        ''' </summary>
        Property InputSchema As SchemaT

        ''' <summary>
        ''' The output schema (one reduced record), given the input schema.
        ''' </summary>
        ''' <param name="schema"> the input schema </param>
        Function transform(ByVal schema As SchemaT) As SchemaT

        ''' <summary>
        ''' An aggregation that has the property that
        ''' reduce(List(reduce(List(l1, l2)), l3)) = reduce(List(l1, reduce(List(l2, l3)))
        ''' </summary>
        Function aggregableReducer() As IAggregableReduceOp(Of IList(Of WritableT), IList(Of WritableT))

        ''' <summary>
        ''' The key columns (Java <c>getKeyColumns()</c>).
        ''' </summary>
        ReadOnly Property KeyColumns As IList(Of String)

    End Interface

End Namespace
