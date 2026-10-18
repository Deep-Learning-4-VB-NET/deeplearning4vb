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

Namespace org.datavec.api.transform.stringreduce

    ''' <summary>
    ''' A reduction of string values into one string per record. Implementations are serializable.
    ''' </summary>
    ' Java: @JsonInclude(NON_NULL) @JsonTypeInfo(use = CLASS, include = PROPERTY, property = "@class")
    <JsonClassType>
    <JsonIncludeNonNull>
    Public Interface IStringReducer

        ''' <summary>
        ''' The input schema (Java <c>getInputSchema</c>/<c>setInputSchema</c>).
        ''' </summary>
        Property InputSchema As SchemaT

        ''' <summary>
        ''' The output schema, given the input schema.
        ''' </summary>
        ''' <param name="schema"> the input schema </param>
        Function transform(ByVal schema As SchemaT) As SchemaT

        ''' <summary>
        ''' Reduces each record (list of values) of the list into one value.
        ''' </summary>
        ''' <param name="examplesList"> the records </param>
        ''' <returns> one reduced value per record </returns>
        Function reduce(ByVal examplesList As IList(Of IList(Of WritableT))) As IList(Of WritableT)

        ''' <summary>
        ''' The input columns (Java <c>getInputColumns()</c>).
        ''' </summary>
        ReadOnly Property InputColumns As IList(Of String)

    End Interface

End Namespace
