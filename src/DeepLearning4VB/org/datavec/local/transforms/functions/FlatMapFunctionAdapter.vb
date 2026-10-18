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

Namespace org.datavec.local.transforms.functions

    ''' <summary>
    ''' A function that maps one input to zero or more outputs (a flat map). Wrapped by
    ''' <see cref="transforms.BaseFlatMapFunctionAdaptee(Of K, V)"/>, which turns any exception into an
    ''' <see cref="System.InvalidOperationException"/> (Java declared <c>throws Exception</c>).
    ''' </summary>
    ''' <typeparam name="T">the input type</typeparam>
    ''' <typeparam name="R">the type of each output</typeparam>
    Public Interface FlatMapFunctionAdapter(Of T, R)

        ''' <summary>Maps one input to its outputs.</summary>
        Function [call](ByVal t As T) As IList(Of R)

    End Interface

End Namespace
