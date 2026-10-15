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

Imports System

Namespace org.datavec.api.transform.quality.columns

    ''' <summary>Counts of valid, invalid, missing and total values of one column (base of the per-type quality reports).</summary>
    ' Java: @AllArgsConstructor @Data
    <Serializable>
    Public MustInherit Class ColumnQuality

        Protected Friend ReadOnly countValid As Long
        Protected Friend ReadOnly countInvalid As Long
        Protected Friend ReadOnly countMissing As Long
        Protected Friend ReadOnly countTotal As Long

        Protected Sub New(ByVal countValid As Long, ByVal countInvalid As Long, ByVal countMissing As Long, ByVal countTotal As Long)
            Me.countValid = countValid
            Me.countInvalid = countInvalid
            Me.countMissing = countMissing
            Me.countTotal = countTotal
        End Sub

        ''' <summary>Java <c>getCountValid()</c> (Lombok).</summary>
        Public Function getCountValid() As Long
            Return countValid
        End Function

        ''' <summary>Java <c>getCountInvalid()</c> (Lombok).</summary>
        Public Function getCountInvalid() As Long
            Return countInvalid
        End Function

        ''' <summary>Java <c>getCountMissing()</c> (Lombok).</summary>
        Public Function getCountMissing() As Long
            Return countMissing
        End Function

        ''' <summary>Java <c>getCountTotal()</c> (Lombok).</summary>
        Public Function getCountTotal() As Long
            Return countTotal
        End Function

        ''' <summary>Equal when the other object is of the same class and has the same four counts (subclasses add their fields).</summary>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            If obj Is Nothing OrElse obj.GetType() IsNot Me.GetType() Then Return False
            Dim other As ColumnQuality = DirectCast(obj, ColumnQuality)
            Return countValid = other.countValid AndAlso countInvalid = other.countInvalid AndAlso
                   countMissing = other.countMissing AndAlso countTotal = other.countTotal
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Objects.hash(countValid, countInvalid, countMissing, countTotal)
        End Function

        Public Overrides Function ToString() As String
            Return "countValid=" & countValid & ", countInvalid=" & countInvalid & ", countMissing=" & countMissing & ", countTotal=" & countTotal
        End Function

    End Class

End Namespace
