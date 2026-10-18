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

Namespace org.datavec.api.transform.sequence.window

    ''' <summary>
    ''' A WindowFunction splits a sequence into a set of (possibly overlapping) sub-sequences.
    ''' It is a general-purpose interface that can support many different types of windowing functions,
    ''' for example time windows. Implementations are serializable.
    ''' </summary>
    ' Java: @JsonInclude(NON_NULL) @JsonTypeInfo(use = CLASS, include = PROPERTY, property = "@class")
    <JsonClassType>
    <JsonIncludeNonNull>
    Public Interface WindowFunction

        ''' <summary>
        ''' Apply the windowing function to the given sequence </summary>
        ''' <param name="sequence"> the input sequence </param>
        ''' <returns> the sequence with the window function applied </returns>
        Function applyToSequence(ByVal sequence As IList(Of IList(Of WritableT))) As IList(Of IList(Of IList(Of WritableT)))

        ''' <summary>
        ''' The input schema (Java <c>getInputSchema</c>/<c>setInputSchema</c>).
        ''' </summary>
        Property InputSchema As SchemaT

        ''' <summary>
        ''' Get the output schema, given the input schema. Typically the output schema is the same as the input schema,
        ''' but not necessarily (for example, if the window function adds columns for the window start/end times) </summary>
        ''' <param name="inputSchema">    Schema of the input data </param>
        ''' <returns> Schema of the output windows </returns>
        Function transform(ByVal inputSchema As SchemaT) As SchemaT

    End Interface

End Namespace
