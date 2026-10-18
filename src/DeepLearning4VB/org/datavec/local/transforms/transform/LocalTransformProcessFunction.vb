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

Imports System.Collections.Generic
Imports TransformProcessT = DeepLearning4VB.org.datavec.api.transform.TransformProcess
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.local.transforms.transform

    ''' <summary>
    ''' The flat-map function form of <see cref="LocalTransformProcessFunctionAdapter"/>: errors are wrapped in an
    ''' <see cref="System.InvalidOperationException"/>.
    ''' </summary>
    Public Class LocalTransformProcessFunction
        Inherits BaseFlatMapFunctionAdaptee(Of IList(Of WritableT), IList(Of WritableT))

        Public Sub New(ByVal transformProcess As TransformProcessT)
            MyBase.New(New LocalTransformProcessFunctionAdapter(transformProcess))
        End Sub

    End Class

End Namespace
