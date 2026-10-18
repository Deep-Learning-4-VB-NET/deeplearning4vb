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
Imports System.Collections.Generic
Imports DeepLearning4VB.org.nd4vb.common.function
Imports LocalTransformExecutorT = DeepLearning4VB.org.datavec.local.transforms.LocalTransformExecutor
Imports Logger = DeepLearning4VB.org.slf4j.Logger
Imports LoggerFactory = DeepLearning4VB.org.slf4j.LoggerFactory
Imports TransformT = DeepLearning4VB.org.datavec.api.transform.Transform
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.local.transforms.transform

    ''' <summary>
    ''' Applies a transform to one record (<c>Transform.map</c>). In the executor's try/catch mode
    ''' (<see cref="LocalTransformExecutorT.isTryCatch"/>) an exception is logged and an empty record is returned instead,
    ''' which the executor then drops.
    ''' </summary>
    Public Class LocalTransformFunction
        Implements [Function](Of IList(Of WritableT), IList(Of WritableT))

        Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(LocalTransformFunction))

        Private ReadOnly transform As TransformT

        Public Sub New(ByVal transform As TransformT)
            Me.transform = transform
        End Sub

        Public Overridable Function apply(ByVal v1 As IList(Of WritableT)) As IList(Of WritableT) Implements [Function](Of IList(Of WritableT), IList(Of WritableT)).apply
            If LocalTransformExecutorT.isTryCatch() Then
                Try
                    Return transform.map(v1)
                Catch e As Exception
                    log.warn("Error occurred " & e.ToString() & " on record " & recordToString(v1))
                    Return New List(Of WritableT)()
                End Try
            End If
            Return transform.map(v1)
        End Function

        Private Shared Function recordToString(ByVal record As IList(Of WritableT)) As String
            If record Is Nothing Then
                Return "null"
            End If
            Dim parts As New List(Of String)(record.Count)
            For Each w As WritableT In record
                parts.Add(If(w Is Nothing, "null", w.ToString()))
            Next w
            Return "[" & String.Join(", ", parts) & "]"
        End Function

    End Class

End Namespace
