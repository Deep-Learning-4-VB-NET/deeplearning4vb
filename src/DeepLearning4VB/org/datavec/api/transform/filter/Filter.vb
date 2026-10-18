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
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.filter

    ''' <summary>
    ''' Decides whether an example (or a sequence) is removed from the data. The <c>InputSchema</c>
    ''' property is inherited from <see cref="ColumnOp"/>.
    ''' </summary>
    ' Java: @JsonInclude(NON_NULL) @JsonTypeInfo(use = CLASS, include = PROPERTY, property = "@class")
    <JsonClassType>
    <JsonIncludeNonNull>
    Public Interface Filter
        Inherits ColumnOp

        ''' <param name="example"> Example </param>
        ''' <returns> true if example should be removed, false to keep </returns>
        Function removeExample(ByVal example As Object) As Boolean

        ''' <param name="sequence"> sequence example </param>
        ''' <returns> true if example should be removed, false to keep </returns>
        Function removeSequence(ByVal sequence As Object) As Boolean

        ''' <param name="writables"> Example </param>
        ''' <returns> true if example should be removed, false to keep </returns>
        Function removeExample(ByVal writables As IList(Of WritableT)) As Boolean

        ''' <param name="sequence"> sequence example </param>
        ''' <returns> true if example should be removed, false to keep </returns>
        Function removeSequence(ByVal sequence As IList(Of IList(Of WritableT))) As Boolean

    End Interface

End Namespace
