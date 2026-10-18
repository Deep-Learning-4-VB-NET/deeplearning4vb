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

Namespace org.nd4vb.linalg.api.blas

    ''' <summary>
    ''' A BLAS/LAPACK failure with the library's error code (Java <c>BlasException</c>, an <c>Error</c>; here a
    ''' <see cref="System.Exception"/>). The message is <c>message &amp; ": " &amp; errorCode</c>, as in Java.
    ''' </summary>
    Public Class BlasException
        Inherits System.Exception

        Public Const serialVersionUID As Long = &HdeadbeefL

        Public Const UNKNOWN_ERROR As Integer = -200

        ' return code from the library - non zero == err
        Friend errorCodeField As Integer

        Public Overridable ReadOnly Property ErrorCode As Integer
            Get
                Return errorCodeField
            End Get
        End Property

        ''' <summary>
        ''' Principal constructor - error message & error code </summary>
        ''' <param name="message"> the error message to put into the Exception </param>
        ''' <param name="errorCode"> the library error number </param>
        Public Sub New(ByVal message As String, ByVal errorCode As Integer)
            MyBase.New(message & ": " & errorCode)
            Me.errorCodeField = errorCode
        End Sub

        Public Sub New(ByVal message As String)
            Me.New(message, UNKNOWN_ERROR)
        End Sub

    End Class

End Namespace
