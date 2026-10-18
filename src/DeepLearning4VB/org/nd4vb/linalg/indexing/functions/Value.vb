Imports System
Imports DeepLearning4VB.org.nd4vb.common.function

'
' *  ******************************************************************************
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

Namespace org.nd4vb.linalg.indexing.functions

    ''' <summary>
    ''' Returns a fixed number whatever the input. Java <c>Number</c> is <see cref="Object"/> holding a boxed .NET number.
    ''' </summary>
    Public Class Value
        Implements [Function](Of Object, Object)

        Private ReadOnly number As Object

        ''' <param name="number"> the number to return </param>
        Public Sub New(ByVal number As Object)
            Me.number = number
        End Sub

        Public Overridable Function apply(ByVal input As Object) As Object Implements [Function](Of Object, Object).apply
            Return Me.number
        End Function

    End Class

End Namespace
