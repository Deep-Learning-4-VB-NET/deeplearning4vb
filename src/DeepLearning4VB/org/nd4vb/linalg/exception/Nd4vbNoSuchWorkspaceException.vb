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

Namespace org.nd4vb.linalg.exception

    ''' <summary>Thrown when a workspace does not exist (Java <c>Nd4jNoSuchWorkspaceException</c>, a <c>RuntimeException</c>).</summary>
    Public Class Nd4vbNoSuchWorkspaceException
        Inherits System.Exception

        Public Sub New(ByVal msg As String)
            MyBase.New(msg)
        End Sub

        Public Sub New(ByVal msg As String, ByVal cause As System.Exception)
            MyBase.New(msg, cause)
        End Sub

    End Class

End Namespace
