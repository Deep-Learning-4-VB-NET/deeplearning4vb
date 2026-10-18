Imports System

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

Namespace org.nd4vb.linalg.indexing.conditions

    ''' <summary>
    ''' True when the input is infinite (tested in single precision, as in Java).
    ''' Condition mode 8 (see <see cref="Condition"/>).
    ''' </summary>
    Public Class IsInfinite
        Inherits BaseCondition

        Public Sub New()
            MyBase.New(-1)
        End Sub

        Public Overrides Function condtionNum() As Integer
            Return 8
        End Function

        Public Overrides Function apply(ByVal input As Object) As Boolean
            Return Single.IsInfinity(floatValue(input))
        End Function

    End Class

End Namespace
