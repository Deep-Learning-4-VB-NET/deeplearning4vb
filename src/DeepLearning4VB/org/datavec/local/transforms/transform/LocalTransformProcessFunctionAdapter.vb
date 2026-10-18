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
Imports DeepLearning4VB.org.datavec.local.transforms.functions
Imports TransformProcessT = DeepLearning4VB.org.datavec.api.transform.TransformProcess
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.local.transforms.transform

    ''' <summary>
    ''' Executes a whole transform process on one record (<c>TransformProcess.execute</c>): returns the transformed record,
    ''' or nothing if a filter removed it.
    ''' </summary>
    <Serializable>
    Public Class LocalTransformProcessFunctionAdapter
        Implements FlatMapFunctionAdapter(Of IList(Of WritableT), IList(Of WritableT))

        Private ReadOnly transformProcess As TransformProcessT

        Public Sub New(ByVal transformProcess As TransformProcessT)
            Me.transformProcess = transformProcess
        End Sub

        Public Overridable Function [call](ByVal v1 As IList(Of WritableT)) As IList(Of IList(Of WritableT)) Implements FlatMapFunctionAdapter(Of IList(Of WritableT), IList(Of WritableT)).call
            Dim newList As IList(Of WritableT) = transformProcess.execute(v1)
            If newList Is Nothing Then
                Return New List(Of IList(Of WritableT))() 'Example was filtered out
            Else
                Return New List(Of IList(Of WritableT)) From {newList}
            End If
        End Function

    End Class

End Namespace
