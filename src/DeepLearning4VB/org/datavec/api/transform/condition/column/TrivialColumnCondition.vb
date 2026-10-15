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
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.condition.column

    ''' <summary>A column condition that is always satisfied.</summary>
    ' Java: @JsonIgnoreProperties({"schema"}) @Data; the JSON property is "name".
    <Serializable>
    Public Class TrivialColumnCondition
        Inherits BaseColumnCondition

        Public Sub New(ByVal name As String)
            MyBase.New(name, DEFAULT_SEQUENCE_CONDITION_MODE)
        End Sub

        Public Overrides Function ToString() As String
            Return "Trivial(" & columnNameField & ")"
        End Function

        Public Overrides Function columnCondition(ByVal writable As WritableT) As Boolean
            Return True
        End Function

        Public Overrides Function condition(ByVal writables As IList(Of WritableT)) As Boolean
            Return True
        End Function

        Public Overrides Function condition(ByVal input As Object) As Boolean
            Return True
        End Function

    End Class

End Namespace
