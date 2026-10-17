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
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.sequence

    ''' <summary>
    ''' Splits one sequence into several sequences. Implementations are serializable.
    ''' </summary>
    ' TODO(serde): polymorphic JSON attributes added in the serde wave
    ' (Java: @JsonInclude(NON_NULL) @JsonTypeInfo(use = CLASS, include = PROPERTY, property = "@class"))
    Public Interface SequenceSplit

        ''' <summary>
        ''' Split a sequence in to multiple time steps </summary>
        ''' <param name="sequence"> the sequence to split </param>
        ''' <returns> the sequences </returns>
        Function split(ByVal sequence As IList(Of IList(Of WritableT))) As IList(Of IList(Of IList(Of WritableT)))

        ''' <summary>
        ''' The input schema of this split (Java <c>setInputSchema</c>).
        ''' </summary>
        Property InputSchema As SchemaT

    End Interface

End Namespace
