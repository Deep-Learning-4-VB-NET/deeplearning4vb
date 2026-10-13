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

Namespace org.datavec.api.io

    ''' <summary>A comparer that can also compare the serialized (binary) form of its values.</summary>
    Public Interface RawComparator(Of T)
        Inherits IComparer(Of T)

        Overloads Function compare(ByVal b1() As Byte, ByVal s1 As Integer, ByVal l1 As Integer, ByVal b2() As Byte, ByVal s2 As Integer, ByVal l2 As Integer) As Integer

    End Interface

End Namespace
