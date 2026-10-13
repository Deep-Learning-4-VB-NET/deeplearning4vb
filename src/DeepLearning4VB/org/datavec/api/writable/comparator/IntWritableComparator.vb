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
Imports Writable = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.writable.comparator

    ''' <summary>Orders writables by <c>toInt()</c>. Instances have no state, so all are equal.</summary>
    <Serializable>
    Public Class IntWritableComparator
        Implements WritableComparator

        Public Overridable Function Compare(ByVal o1 As Writable, ByVal o2 As Writable) As Integer Implements System.Collections.Generic.IComparer(Of Writable).Compare
            Return o1.toInt().CompareTo(o2.toInt())
        End Function

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            Return o IsNot Nothing AndAlso o.GetType() Is Me.GetType()
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return 1
        End Function

        Public Overrides Function ToString() As String
            Return "IntWritableComparator()"
        End Function
    End Class

End Namespace
