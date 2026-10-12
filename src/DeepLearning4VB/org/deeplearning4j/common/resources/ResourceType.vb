Imports System
Imports System.Runtime.CompilerServices

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

Namespace org.deeplearning4j.common.resources

    ''' <summary>Kinds of downloadable resources, each stored in its own sub-directory of the DL4J resources directory.</summary>
    Public Enum ResourceType
        DATASET
        ZOO_MODEL
        RESOURCE
    End Enum

    Public Module ResourceTypeExtensions

        ''' <summary>The sub-directory name used for this resource type.</summary>
        <Extension>
        Public Function resourceName(ByVal type As ResourceType) As String
            Select Case type
                Case ResourceType.DATASET
                    Return "data"
                Case ResourceType.ZOO_MODEL
                    Return "models"
                Case ResourceType.RESOURCE
                    Return "resources"
                Case Else
                    Return type.ToString()
            End Select
        End Function

        ''' <summary>Parses the enum constant name (Java <c>ResourceType.valueOf</c>).</summary>
        Public Function ResourceTypeValueOf(ByVal name As String) As ResourceType
            Dim result As ResourceType
            If [Enum].TryParse(name, False, result) AndAlso [Enum].IsDefined(GetType(ResourceType), result) Then
                Return result
            End If
            Throw New ArgumentException("No enum constant ResourceType." & name)
        End Function

    End Module

End Namespace
