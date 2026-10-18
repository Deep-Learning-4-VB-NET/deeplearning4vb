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
Imports DeepLearning4VB.org.datavec.api.transform.serde
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.sequence

    ''' <summary>
    ''' Compares the time steps (records) of a sequence, for example to sort the time steps collected into a sequence
    ''' (Java: <c>Comparator&lt;List&lt;Writable&gt;&gt;</c>). Implementations are serializable.
    ''' </summary>
    ' Java: @JsonInclude(NON_NULL) @JsonTypeInfo(use = CLASS, include = PROPERTY, property = "@class")
    <JsonClassType>
    <JsonIncludeNonNull>
    Public Interface SequenceComparator
        Inherits IComparer(Of IList(Of WritableT))

        ''' <summary>
        ''' Sets the schema of the sequences to compare (Java <c>setSchema</c>).
        ''' </summary>
        WriteOnly Property Schema As SchemaT

    End Interface

End Namespace
