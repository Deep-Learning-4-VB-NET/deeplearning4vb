Imports System
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType

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

Namespace org.nd4vb.linalg.api.ops

    ''' <summary>
    ''' Transform operation:
    ''' stores the result in an ndarray
    '''
    ''' @author Adam Gibson
    ''' </summary>
    Public Interface TransformOp
        Inherits Op

        ''' <summary>
        ''' This method returns datatype for result array wrt given inputs
        ''' @return
        ''' </summary>
        Function resultType() As DataType

        Function resultType(ByVal opContext As OpContext) As DataType

        ''' <summary>
        ''' The op type the executioner dispatches on (Java <c>getOpType()</c>; a method rather than a property named
        ''' <c>OpType</c>, which would clash with <c>DifferentialFunction.opType()</c>: VB names are case-insensitive).
        ''' </summary>
        Function getOpType() As Op.Type

        Function validateDataTypes(ByVal opContext As OpContext, ByVal experimentalMode As Boolean) As Boolean
    End Interface

End Namespace
