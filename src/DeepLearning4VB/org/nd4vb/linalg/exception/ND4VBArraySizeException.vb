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

    ''' <summary>
    ''' Thrown when an array is too long to be copied into a .NET array (Java <c>ND4JArraySizeException</c>;
    ''' the Java message spoke of a "JVM array").
    ''' </summary>
    Public Class ND4VBArraySizeException
        Inherits ND4VBException

        Public Sub New()
            MyBase.New("INDArray length is too big to fit into a .NET array")
        End Sub

    End Class

End Namespace
