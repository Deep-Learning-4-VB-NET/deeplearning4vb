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

Namespace org.deeplearning4vb.common.config

    ''' <summary>Names of the environment variables DL4VB reads.</summary>
    Public NotInheritable Class DL4VBEnvironmentVars

        Private Sub New()
        End Sub

        ''' <summary>Applicability: deeplearning4vb-scaleout. Description: the IP address of the Spark master used by the parameter server.</summary>
        Public Const DL4VB_VOID_IP As String = "DL4VB_VOID_IP"

    End Class

End Namespace
