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

Namespace org.datavec.api.io

    ''' <summary>
    ''' A <see cref="writable.Writable"/> with a natural ordering. This non-generic form is the Java raw type
    ''' (<c>compareTo(Object)</c>), which is what the numeric writables implement.
    ''' </summary>
    Public Interface WritableComparable
        Inherits writable.Writable, IComparable

    End Interface

    ''' <summary>The typed form, <c>WritableComparable&lt;T&gt;</c>.</summary>
    Public Interface WritableComparable(Of T)
        Inherits WritableComparable, IComparable(Of T)

    End Interface

End Namespace
