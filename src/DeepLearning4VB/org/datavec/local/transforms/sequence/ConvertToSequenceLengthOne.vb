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
Imports DeepLearning4VB.org.nd4vb.common.function
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.local.transforms.sequence

    ''' <summary>Turns a record into a sequence of one time step (ConvertToSequence in single step mode).</summary>
    Public Class ConvertToSequenceLengthOne
        Implements [Function](Of IList(Of WritableT), IList(Of IList(Of WritableT)))

        Public Overridable Function apply(ByVal writables As IList(Of WritableT)) As IList(Of IList(Of WritableT)) Implements [Function](Of IList(Of WritableT), IList(Of IList(Of WritableT))).apply
            Return New List(Of IList(Of WritableT))(1) From {writables}
        End Function

    End Class

End Namespace
