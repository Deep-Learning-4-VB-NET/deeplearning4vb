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

Imports System.Text.Json.Serialization

Namespace org.datavec.api.transform

    ''' <summary>The reductions a reducer can apply to a column. Serializes by name.</summary>
    <JsonConverter(GetType(JsonStringEnumConverter))>
    Public Enum ReduceOp
        Prod
        Min
        Max
        ''' <summary>Max - Min</summary>
        Range
        ''' <summary>String operations : concatenate, concatenate with commuted arguments</summary>
        Append
        Prepend
        Sum
        Mean
        ''' <summary>with Bessel's correction</summary>
        Stdev
        ''' <summary>without</summary>
        UncorrectedStdDev
        ''' <summary>with Bessel's correction</summary>
        Variance
        ''' <summary>without</summary>
        PopulationVariance
        Count
        CountUnique
        ''' <summary>First value</summary>
        TakeFirst
        ''' <summary>Last value</summary>
        TakeLast
    End Enum

End Namespace
