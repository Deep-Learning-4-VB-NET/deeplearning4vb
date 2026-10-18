Imports System
Imports EnvironmentalAction = DeepLearning4VB.org.nd4vb.linalg.env.EnvironmentalAction
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb
Imports DebugMode = DeepLearning4VB.org.nd4vb.linalg.api.memory.enums.DebugMode

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

Namespace org.nd4vb.linalg.env.impl

    ''' <summary>
    ''' Handles the <c>ND4VB_BYPASS_WS</c> environment variable (Java <c>ND4J_BYPASS_WS</c>): when true, workspaces are bypassed (debug mode BYPASS_EVERYTHING).
    ''' </summary>
    Public Class WorkspacesBypassAction
        Implements EnvironmentalAction

        ''' <summary>The environment variable this action handles.</summary>
        Public Overridable Function targetVariable() As String Implements EnvironmentalAction.targetVariable
            Return "ND4VB_BYPASS_WS"
        End Function

        ''' <summary>Applies the value of the environment variable.</summary>
        Public Overridable Sub process(ByVal value As String) Implements EnvironmentalAction.process
            If javaBoolean(value) Then
                Nd4vbT.WorkspaceManager.DebugMode = DebugMode.BYPASS_EVERYTHING
            End If
        End Sub

        ''' <summary>Java <c>Boolean.valueOf</c>: "true" in any case is True, anything else (also Nothing) False.</summary>
        Friend Shared Function javaBoolean(ByVal value As String) As Boolean
            Return String.Equals(value, "true", StringComparison.OrdinalIgnoreCase)
        End Function

    End Class

End Namespace
