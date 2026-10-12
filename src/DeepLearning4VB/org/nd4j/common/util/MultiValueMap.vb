Imports System.Collections.Generic

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

Namespace org.nd4j.common.util

    ''' <summary>
    ''' A dictionary that maps each key to a list of values.
    ''' </summary>
    Public Interface MultiValueMap(Of K, V)
        Inherits IDictionary(Of K, IList(Of V))

        ''' <summary>Returns the first value for the key, or the default value if the key is absent.</summary>
        Function getFirst(ByVal key As K) As V

        ''' <summary>Appends the value to the list for the key.</summary>
        Overloads Sub add(ByVal key As K, ByVal value As V)

        ''' <summary>Replaces the values for the key with a single value.</summary>
        Sub [set](ByVal key As K, ByVal value As V)

        ''' <summary>Replaces the values of every key in <paramref name="values"/>.</summary>
        Sub setAll(ByVal values As IDictionary(Of K, V))

        ''' <summary>Returns a dictionary with the first value of each key.</summary>
        Function toSingleValueMap() As IDictionary(Of K, V)
    End Interface

End Namespace
