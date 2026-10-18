Imports System

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

Namespace org.nd4vb.linalg.util

    ''' <summary>
    ''' Conversions between <c>Long()</c> and <c>Integer()</c>.
    ''' </summary>
    ''' @author raver119@gmail.com
    Public Class LongUtils

        ''' <summary>
        ''' Narrows every element to <see cref="Integer"/>; values outside the <see cref="Integer"/> range wrap around like
        ''' Java's <c>(int)</c> cast.
        ''' </summary>
        Public Shared Function toInts(ByVal array() As Long) As Integer()
            Dim ret(array.Length - 1) As Integer
            For e As Integer = 0 To array.Length - 1
                ret(e) = Arrays.WrapToInt32(array(e))
            Next e
            Return ret
        End Function

        ''' <summary>Widens every element to <see cref="Long"/>.</summary>
        Public Shared Function toLongs(ByVal array() As Integer) As Long()
            Dim ret(array.Length - 1) As Long
            For e As Integer = 0 To array.Length - 1
                ret(e) = array(e)
            Next e
            Return ret
        End Function

    End Class

End Namespace
